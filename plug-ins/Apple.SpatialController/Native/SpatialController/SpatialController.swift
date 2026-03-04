//
//  SpatialController.swift
//  SpatialController
//
//  Copyright © 2025 Apple, Inc. All rights reserved.
//

import Foundation
import CoreGraphics
import CoreHaptics
import GameController
import ARKit
import OSLog

var _spatialController: SpatialController?

extension Logger {
    private static let subsystem = "com.apple.SpatialController"
}

extension Logger {
    static let controller = Logger(subsystem: subsystem, category: "controller")
}

enum AccessoryTrackingAuthorizationState {
    case notSupported
    case notRequested
    case pending
    case notAuthorized
    case authorized
}

enum AccessoryTrackingState {
    case stopped
    case arTrackingError
    case runFailed
    case running
}

enum AccessoryState {
    case acquiring
    case acquireAccessoryFailed
    case acquired
}

class AccessoryTracker {
    let controller: GCController
    let uuid: String

    var accessory: Accessory?
    var state: AccessoryState
    var acquireTask: Task<(AccessoryState), Never>?
    var onCompletion: ((AccessoryState, Accessory?) -> Void)?

    init(controller: GCController, uuid: String)
    {
        self.controller = controller
        self.uuid = uuid
        state = .acquiring
    }

    deinit {
        acquireTask?.cancel()
    }

    func acquireAccessory() async -> AccessoryState {
        if acquireTask == nil {
            acquireTask = Task {
                let uuid = uuid
                do {
                    Logger.controller.log("SpatialController \(uuid): acquiring accessory...")
                    accessory = try await Accessory(device: controller)
                    if let accessory {
                        Logger.controller.log("SpatialController \(uuid): acquired accessory \(accessory.description)")
                        state = .acquired
                    } else {
                        Logger.controller.error("SpatialController \(uuid): acquire accessory failed (nil)")
                        state = .acquireAccessoryFailed
                    }
                } catch {
                    Logger.controller.error("SpatialController \(uuid): acquire accessory failed (exception)")
                    state = .acquireAccessoryFailed
                }
                return state
            }
        }
        return await acquireTask!.value
    }
}

struct ButtonTimestamps {
    var lastValueTimestamp: TimeInterval
    var lastPressedStateTimestamp: TimeInterval
    var isPressed: Bool
}

class TouchpadControllerState {
    var touchpadButton: ButtonTimestamps

    init(_ dualshockGamepad: GCDualShockGamepad)
    {
        touchpadButton = .init(lastValueTimestamp: 0.0, lastPressedStateTimestamp: 0.0, isPressed: false)
        dualshockGamepad.valueChangedHandler = { [weak self] gamepad, element in
            guard let self else { return }
            guard let dualshockGamepad = (gamepad as? GCDualShockGamepad) else { return }
            if element === dualshockGamepad.touchpadButton {
                let timeNow = CACurrentMediaTime()
                self.touchpadButton.lastValueTimestamp = timeNow
                if dualshockGamepad.touchpadButton.isPressed != self.touchpadButton.isPressed {
                    self.touchpadButton.lastPressedStateTimestamp = timeNow
                    self.touchpadButton.isPressed = dualshockGamepad.touchpadButton.isPressed
                }
            }
        }
    }

    init(_ dualsenseGamepad: GCDualSenseGamepad)
    {
        touchpadButton = .init(lastValueTimestamp: 0.0, lastPressedStateTimestamp: 0.0, isPressed: false)
        dualsenseGamepad.valueChangedHandler = { [weak self] gamepad, element in
            guard let self else { return }
            guard let dualsenseGamepad = (gamepad as? GCDualSenseGamepad) else { return }
            if element === dualsenseGamepad.touchpadButton {
                let timeNow = CACurrentMediaTime()
                self.touchpadButton.lastValueTimestamp = timeNow
                if dualsenseGamepad.touchpadButton.isPressed != self.touchpadButton.isPressed {
                    self.touchpadButton.lastPressedStateTimestamp = timeNow
                    self.touchpadButton.isPressed = dualsenseGamepad.touchpadButton.isPressed
                }
            }
        }
    }
}

class SpatialController {
    var controllerMapping = ControllerDictionary()
    // Notifications...
    var notificationHandler : SCNotificationHandler? = nil
    var onControllerConnectedCallback : SCControllerConnectionCallback? = nil
    var onControllerDisconnectedCallback : SCControllerConnectionCallback? = nil
    var onAccessoryConnectedCallback : SCAccessoryConnectionCallback? = nil
    var onAccessoryDisconnectedCallback : SCAccessoryConnectionCallback? = nil
    // Haptics
    @available(visionOS 1.0, macOS 10.16, iOS 14.0, tvOS 14.0, *)
    var controllerHapticEngines = [String : [GCHapticsLocality : CHHapticEngine]]()
    // ARKit Accessory tracking
    let arSession = ARKitSession()
    var accessoryTrackingProvider: AccessoryTrackingProvider?
    var authorizationTask: Task<(AccessoryTrackingAuthorizationState), Never>?
    var eventTask: Task<(), Never>?
    var updateTask: Task<(), Never>?
    var arTrackingAuthorizationState: AccessoryTrackingAuthorizationState = .notSupported
    var arTrackingState: AccessoryTrackingState = .stopped
    var accessoryTrackers = [String : AccessoryTracker]()
    // DualShock, DualSense touchpad support
    var touchpadStates = [String: TouchpadControllerState]()
    var enableMotionSensorsDefault: Bool = false

    enum UpdateTaskResult {
        case acquireAccessory(AccessoryTracker)
    }

    init() {
        if AccessoryTrackingProvider.isSupported {
            arTrackingAuthorizationState = .notRequested
            return
        }
    }

    deinit {
        authorizationTask?.cancel()
        updateTask?.cancel()
        eventTask?.cancel()
    }

    func observeTrackingState() {
        guard arTrackingAuthorizationState != .notSupported else {
            return
        }
        if eventTask != nil {
            return
        }
        eventTask = Task {
            for await event in arSession.events {
                switch event {
                case .dataProviderStateChanged(_, let newState, let error):
                    if newState == .stopped {
                        if error != nil {
                            Logger.controller.error("SpatialController: Accessory tracking stopped with error: \(error)")
                            arTrackingState = .arTrackingError
                        } else {
                            Logger.controller.log("SpatialController: Accessory tracking stopped")
                            arTrackingState = .stopped
                        }
                    } else if newState == .running {
                        Logger.controller.log("SpatialController: Accessory tracking is running")
                        arTrackingState = .running
                    }
                default:
                    break
                }
            }
            Logger.controller.log("SpatialController: Accessory tracking event task was cancelled")
        }
    }

    func stopObservingTrackingState() {
        if eventTask != nil {
            eventTask?.cancel()
            eventTask = nil
        }
    }

    func acquireAccessoryTrackingAuthorization() {
        guard arTrackingAuthorizationState != .notSupported else {
            return
        }
        if authorizationTask != nil {
            return
        }
        arTrackingAuthorizationState = .pending
        authorizationTask = Task {
            let result = await arSession.requestAuthorization(for: [.accessoryTracking])
            if let authorizationStatus = result[.accessoryTracking] {
                switch authorizationStatus {
                case .notDetermined:
                    Logger.controller.log("SpatialController: Accessory tracking authorization is still pending")
                case .allowed:
                    Logger.controller.log("SpatialController: Accessory tracking authorized")
                    arTrackingAuthorizationState = .authorized
                case .denied:
                    Logger.controller.error("SpatialController: Accessory tracking authorization failed")
                    arTrackingAuthorizationState = .notAuthorized
                default:
                    Logger.controller.log("SpatialController: Accessory tracking authorization state is unknown (\(String(describing: authorizationStatus)))")
                }
            }
            return arTrackingAuthorizationState
        }
    }

    func stopAcquiringAccessoryTrackingAuthorization() {
        if authorizationTask != nil {
            Logger.controller.log("SpatialController: Stopping accessory tracking authorization task")
            authorizationTask?.cancel()
            authorizationTask = nil
        }
        if arTrackingAuthorizationState == .pending {
            arTrackingAuthorizationState = .notRequested
        }
    }

    func startAccessoryTracking(uuid: String, controller: GCController, onCompletion: @escaping (AccessoryState, Accessory?) -> Void)
    {
        guard arTrackingAuthorizationState != .notSupported else {
            Logger.controller.error("SpatialController \(uuid): Accessory tracking is not supported!")
            onCompletion(.acquireAccessoryFailed, nil)
            return
        }
        let accessoryTracker = AccessoryTracker(controller: controller, uuid: uuid)
        accessoryTracker.onCompletion = onCompletion
        accessoryTrackers.updateValue(accessoryTracker, forKey: uuid)

        Logger.controller.log("SpatialController \(uuid): Adding to accessory tracking")
        restartAccessoryTracking(of: Array(accessoryTrackers.values))
    }

    func stopAccessoryTracking(uuid: String) {
        if accessoryTrackers.removeValue(forKey: uuid) == nil {
            // REPORT ERROR: no such accessory
            Logger.controller.error("SpatialController \(uuid): Not found")
            return
        }
        Logger.controller.log("SpatialController \(uuid): Removing from accessory tracking")
        restartAccessoryTracking(of: Array(accessoryTrackers.values))
    }

    internal func restartAccessoryTracking(of trackers: [AccessoryTracker]) {
        observeTrackingState()
        acquireAccessoryTrackingAuthorization()
        if updateTask != nil {
            Logger.controller.log("SpatialController: Stopping accessory tracking task")
            updateTask?.cancel()
        }
        updateTask = Task {
            Logger.controller.log("SpatialController: Collecting \(trackers.count) accessories...")
            let accessories = await withTaskGroup(of: UpdateTaskResult.self) { group in
                for tracker in trackers {
                    group.addTask {
                        let _ = await tracker.acquireAccessory()
                        return .acquireAccessory(tracker)
                    }
                }

                var accessories: [Accessory] = []
                for await result in group {
                    switch result {
                    case let .acquireAccessory(tracker):
                        if tracker.state == .acquired {
                            let accessory = tracker.accessory!
                            Logger.controller.log("SpatialController: Collected accessory \(tracker.uuid) id:\(accessory.id) \(accessory.description)")
                            accessories.append(accessory)
                        } else {
                            Logger.controller.log("SpatialController: Discarded accessory \(tracker.uuid) in state \(tracker.state == .acquireAccessoryFailed ? "acquireAccessoryFailed" : "acquiring")")
                        }
                    }
                }
                return accessories
            }

            guard accessories.count > 0 else {
                Logger.controller.log("SpatialController: No active accessories to track")
                accessoryTrackingProvider = nil
                for tracker in trackers {
                    if tracker.onCompletion != nil {
                        tracker.onCompletion!(tracker.state, tracker.accessory)
                        tracker.onCompletion = nil
                    }
                }
                return
            }
            if Task.isCancelled {
                Logger.controller.log("SpatialController: Update task was cancelled before starting accessory tracking")
                return
            }
            if accessories.count == 1 {
                Logger.controller.log("SpatialController: Starting accessory tracking for 1 accessory...")
            } else {
                Logger.controller.log("SpatialController: Starting accessory tracking for \(accessories.count) accessories...")
            }
            accessoryTrackingProvider = AccessoryTrackingProvider(accessories: accessories)
            if let accessoryTrackingProvider {
                do {
                    try await arSession.run([accessoryTrackingProvider])
                } catch {
                    Logger.controller.error("SpatialController: Start accessory tracking failed")
                    arTrackingState = .runFailed
                    return
                }

                Logger.controller.error("SpatialController: Accessory tracking is running")
                for tracker in trackers {
                    if tracker.onCompletion != nil {
                        tracker.onCompletion!(tracker.state, tracker.accessory)
                        tracker.onCompletion = nil
                    }
                }

                for await _ in accessoryTrackingProvider.anchorUpdates {
                    // TODO: Consider exposing update callbacks out to Unity API
                    // These would have to be dispatched back to main to be
                    // thread safe for Unity.
                    // Logger.controller.log("SpatialController: anchor update \(update.event) \(update.anchor.accessory.inherentChirality) accessory:\(update.anchor.accessory.id)")
                }
            }
            Logger.controller.log("SpatialController: Accessory tracking update task ended")
        }
    }
}

@_cdecl("SpatialController_Init")
public func SpatialController_Init() {
    if _spatialController == nil {
        _spatialController = SpatialController()
        Logger.controller.log("Swift SpatialController class initialized")
    }
    guard let spatialController = _spatialController else {
        return
    }
    Logger.controller.log("SpatialController: Observing controller events and tracking state...")
    spatialController.observeTrackingState()
    spatialController.acquireAccessoryTrackingAuthorization()
    spatialController.notificationHandler = SCNotificationHandler()
    spatialController.notificationHandler?.setupHandlers()
}

@_cdecl("SpatialController_Destroy")
public func SpatialController_Destroy() {
    if _spatialController != nil {
        if let notificationHandler = _spatialController?.notificationHandler {
            notificationHandler.removeHandlers()
        }
        _spatialController = nil
        Logger.controller.log("Swift SpatialController class released")
    }
}

@_cdecl("SpatialController_SetControllerConnectionHandlers")
public func SpatialController_SetControllerConnectionHandlers
(
    onConnected : @escaping SCControllerConnectionCallback,
    onDisconnected : @escaping SCControllerConnectionCallback,
) -> Bool
{
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: SetControllerConnectionHandlers(): Not initialized!")
        return false
    }
    if let notificationHandler = _spatialController?.notificationHandler {
        notificationHandler.removeHandlers()
    }

    spatialController.onControllerConnectedCallback = onConnected
    spatialController.onControllerDisconnectedCallback = onDisconnected
    Logger.controller.log("SpatialController: Controller connection handlers are active")

    if !spatialController.controllerMapping.elements.isEmpty {
        Logger.controller.log("SpatialController: Notifying connection to already connected controllers...")
        for (uuid, controller) in spatialController.controllerMapping.elements {
            guard controller.isSpatialController else {
                Logger.controller.log("SpatialController: Non-spatial controller connected (category: \(controller.productCategory))")
                continue
            }
            spatialController.onControllerConnectedCallback?(controller.toSCController(uid: uuid))
        }
    }
    return true
}

@_cdecl("SpatialController_SetAccessoryConnectionHandlers")
public func SpatialController_SetAccessoryConnectionHandlers
(
    onConnected : @escaping SCAccessoryConnectionCallback,
    onDisconnected : @escaping SCAccessoryConnectionCallback,
) -> Bool
{
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: SetAccessoryConnectionHandlers(): Not initialized!")
        return false
    }

    spatialController.onAccessoryConnectedCallback = onConnected
    spatialController.onAccessoryDisconnectedCallback = onDisconnected
    Logger.controller.log("SpatialController: Accessory connection handlers are active")

    if !spatialController.accessoryTrackers.isEmpty {
        Logger.controller.log("SpatialController: Notifying connection to any already connected accessories...")
        for (uuid, accessoryTracker) in spatialController.accessoryTrackers {
            guard accessoryTracker.state == .acquired else {
                continue;
            }
            let accessory = accessoryTracker.accessory!
            let controller = accessory.controller!
            Logger.controller.log("SpatialController: Accessory for spatial controller \(uuid) connected (category: \(controller.productCategory))")
            spatialController.onAccessoryConnectedCallback?(accessory.toSCAccessory(uid: uuid))
        }
    }
    return true
}

#if false
// Unless we want to kick off the entire accessory acquire process here,
// there is no point in checking for unregistered GCControllers, because
// only those which successfully acquire accessory tracking are reported
// back as connected (spatial) controllers.
fileprivate func _checkForUnregisteredControllers() {
    // Check for non registered controllers...
    guard let controllerMapping = _spatialController?.controllerMapping else {
        return
    }
    for controller in GCController.controllers() {
        let key = controllerMapping.elements.someKey(forValue: controller)

        // Register controller...
        if key == nil {
            let uuid = UUID().uuidString
            controllerMapping.updateValue(controller, forKey: uuid)
        }
    }
}
#endif

@_cdecl("SpatialController_GetConnectedControllers")
public func SpatialController_GetConnectedControllers(
    callback: @escaping SCControllersCallback,
    context: UnsafeMutableRawPointer?
) -> Bool
{
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: GetConnectedControllers(): Not initialized!")
        return false
    }
#if false
    _checkForUnregisteredControllers()
#endif

    var controllers : [SCController] = []
    // Generate controller from map...
    for (key, value) in spatialController.controllerMapping.elements {
        controllers.append(value.toSCController(uid: key))
    }

    var scControllers = controllers.toSCControllers()
    callback(scControllers, context)

    // Deallocate all mutable unsafe pointers
    scControllers.deallocate()
    return true
}

@_cdecl("SpatialController_GetConnectedController")
public func SpatialController_GetConnectedController(
    uniqueId: char_p,
    callback: @escaping SCControllerCallback,
    context: UnsafeMutableRawPointer?
) -> Bool
{
    let uid = uniqueId.toString()
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: GetConnectedController(uid:\(uid)): Not initialized!")
        return false
    }
#if false
    _checkForUnregisteredControllers()
#endif
    guard let controller = spatialController.controllerMapping.elements[uid] else {
        Logger.controller.error("SpatialController: GetConnectedController(uid:\(uid)): No such controller!")
        return false
    }
    // Generate controller from map...
    var scController = controller.toSCController(uid: uid)
    callback(scController, context)

    // Deallocate all mutable unsafe pointers
    scController.deallocate()
    return true
}

@_cdecl("SpatialController_GetConnectedAccessories")
public func SpatialController_GetConnectedAccessories(
    callback: @escaping SCAccessoriesCallback,
    context: UnsafeMutableRawPointer?
) -> Bool
{
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: GetConnectedAccessories(): Not initialized!")
        return false
    }
#if false
    _checkForUnregisteredControllers()
#endif

    var accessories : [SCAccessory] = []
    // Generate controller from map...
    for (key, _) in spatialController.controllerMapping.elements {
        if let accessoryTracker = spatialController.accessoryTrackers[key], accessoryTracker.state == .acquired {
            accessories.append(accessoryTracker.accessory!.toSCAccessory(uid: key))
        }
    }

    var scAccessories = accessories.toSCAccessories()
    callback(scAccessories, context)

    // Deallocate all mutable unsafe pointers
    scAccessories.deallocate()
    return true
}

@_cdecl("SpatialController_GetConnectedAccessory")
public func SpatialController_GetConnectedAccessory(
    uniqueId: char_p,
    callback: @escaping SCAccessoryCallback,
    context: UnsafeMutableRawPointer?
) -> Bool
{
    let uid = uniqueId.toString()
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: GetConnectedAccessory(uid:\(uid)): Not initialized!")
        return false
    }
#if false
    _checkForUnregisteredControllers()
#endif
    guard spatialController.controllerMapping.elements[uid] != nil else {
        Logger.controller.error("SpatialController: GetConnectedAccessory(uid:\(uid)): No such controller!")
        return false
    }
    // Generate controller from map...
    guard let accessoryTracker = spatialController.accessoryTrackers[uid], accessoryTracker.state == .acquired else {
        Logger.controller.error("SpatialController: GetConnectedAccessory(uid:\(uid)): Accessory not acquired yet")
        return false
    }

    var scAccessory = accessoryTracker.accessory!.toSCAccessory(uid: uid)
    callback(scAccessory, context)

    // Deallocate all mutable unsafe pointers
    scAccessory.deallocate()
    return true
}

@_cdecl("SpatialController_ConfigureDefaultControllerSetting")
public func SpatialController_ConfigureDefaultControllerSetting(
    setting: SCControllerSetting,
    value: Int32,
) -> Bool
{
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: ConfigureDefaultControllerSetting(): Not initialized!")
        return false
    }
    switch (setting) {
    case SCControllerSettingEnableMotionSensors:
        let sensorsActive = value != 0
        if spatialController.enableMotionSensorsDefault != sensorsActive {
            spatialController.enableMotionSensorsDefault = sensorsActive
            Logger.controller.log("SpatialController: ConfigureDefaultControllerSetting(setting:\(String(describing: setting))): \(sensorsActive ? "Enabling" : "Disabling") motion sensors by default")
        }
    default:
        Logger.controller.error("SpatialController: ConfigureDefaultControllerSetting(): Unknown setting \(setting.rawValue)!")
        return false
    }
    return true
}

@_cdecl("SpatialController_ConfigureControllerSetting")
public func SpatialController_ConfigureControllerSetting(
    uniqueId: char_p,
    setting: SCControllerSetting,
    value: Int32,
) -> Bool
{
    let uid = uniqueId.toString()
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: ConfigureController(uid:\(uid)): Not initialized!")
        return false
    }
    switch (setting) {
    case SCControllerSettingEnableMotionSensors:
        break
    default:
        Logger.controller.error("SpatialController: ConfigureController(uid:\(uid)): Unknown setting \(setting.rawValue)!")
        return false
    }
    if uid == "all" || uid == "*" {
        for (key, controller) in spatialController.controllerMapping.elements {
            switch (setting) {
            case SCControllerSettingEnableMotionSensors:
                guard let motion = controller.motion, motion.sensorsRequireManualActivation else {
                    continue
                }
                let sensorsActive = value != 0
                if controller.motion?.sensorsActive != sensorsActive {
                    Logger.controller.log("SpatialController: ConfigureController(uid:\(key). setting:\(String(describing: setting))): \(sensorsActive ? "Enabling" : "Disabling") motion sensors...");
                    controller.motion?.sensorsActive = sensorsActive
                }
            default:
                continue
            }
        }
        return true
    }
    guard let controller = spatialController.controllerMapping.elements[uid] else {
        Logger.controller.error("SpatialController: ConfigureController(uid:\(uid)): No such controller!")
        return false
    }
    switch (setting) {
    case SCControllerSettingEnableMotionSensors:
        guard let motion = controller.motion else {
            Logger.controller.error("SpatialController: ConfigureController(uid:\(uid). setting:\(String(describing: setting))): Controller does not have motion sensors!")
            return false
        }
        guard motion.sensorsRequireManualActivation else {
            Logger.controller.log("SpatialController: ConfigureController(uid:\(uid). setting:\(String(describing: setting))): Controller motion sensors do not support manual activation");
            return false
        }
        let sensorsActive = value != 0
        if controller.motion?.sensorsActive != sensorsActive {
            Logger.controller.log("SpatialController: ConfigureController(uid:\(uid). setting:\(String(describing: setting))): \(sensorsActive ? "Enabling" : "Disabling") motion sensors...");
            controller.motion?.sensorsActive = sensorsActive
        }
    default:
        return false
    }
    return true
}

@_cdecl("SpatialController_GetControllerInputInfoForInputName")
public func SpatialController_GetControllerInputInfoForInputName
(
    uniqueId : char_p,
    inputName : SCControllerInputName,
    callback : @escaping SCGetControllerInputInfoForInputNameCallback,
    context : UnsafeMutableRawPointer?
) -> Bool
{
    guard #available(visionOS 1.0, macOS 13.0, iOS 16.0, tvOS 16.0, *) else {
        // REPORT ERROR: OS feature not available?
        Logger.controller.error("SpatialController: GetControllerInputInfoForInputName(uid:\(uniqueId.toString())): Symbols feature is not available!")
        return false
    }
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: GetControllerInputInfoForInputName(uid:\(uniqueId.toString())): Not initialized!")
        return false
    }
    guard let controller = spatialController.controllerMapping.elements[uniqueId.toString()] else {
        Logger.controller.error("SpatialController: GetControllerInputInfoForInputName(uid:\(uniqueId.toString())): No such controller!")
        return false
    }
    // GCPhysicalDeviceInputState...
    let input = controller.input
    let symbolName = _getSymbolNameForInput(inputName: inputName, profile: input, extendedGamepad: controller.extendedGamepad)
    let localizedName = _getLocalizedNameForInput(inputName: inputName, profile: input, extendedGamepad: controller.extendedGamepad)
    guard symbolName != nil || localizedName != nil else {
        Logger.controller.error("SpatialController: GetControllerInputInfoForInputName(uid:\(uniqueId.toString()), inputName:\(inputName.rawValue)): Controller does not have name or symbol for inputName!")
        return false
    }

    var info = SCControllerInputInfo(localizedName: localizedName, symbolName: symbolName)
    callback(info, context, uniqueId, inputName)

    // Deallocate all mutable unsafe pointers
    info.deallocate()
    return true
}

@_cdecl("SpatialController_GetSymbolForInputName")
public func SpatialController_GetSymbolForInputName
(
    uniqueId : char_p,
    inputName : SCControllerInputName,
    symbolScale : SCUIImageSymbolScale,
    renderingMode : SCUIImageRenderingMode,
    callback : @escaping SCGetSymbolForInputNameCallback,
    context : UnsafeMutableRawPointer?
) -> Bool
{
    guard #available(visionOS 1.0, macOS 13.0, iOS 16.0, tvOS 16.0, *) else {
        Logger.controller.error("SpatialController: GetSymbolForInputName(uid:\(uniqueId.toString())): Symbols feature is not available!")
        return false
    }
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: GetSymbolForInputName(uid:\(uniqueId.toString())): Not initialized!")
        return false
    }
    guard let controller = spatialController.controllerMapping.elements[uniqueId.toString()] else {
        Logger.controller.error("SpatialController: GetSymbolForInputName(uid:\(uniqueId.toString())): No such controller!")
        return false
    }
    // GCPhysicalDeviceInputState...
    let input = controller.input
    guard let symbolName = _getSymbolNameForInput(inputName: inputName, profile: input, extendedGamepad: controller.extendedGamepad) else {
        Logger.controller.error("SpatialController: GetSymbolForInputName(uid:\(uniqueId.toString()), inputName:\(inputName.rawValue)): Controller does not have symbol for inputName!")
        return false
    }
    guard let scale = symbolScale.toUIImageSymbolScale() else {
        Logger.controller.error("SpatialController: GetSymbolForInputName(uid:\(uniqueId.toString()), inputName:\(inputName.rawValue), symbolSize:\(symbolScale.rawValue), renderingMode:\(renderingMode.rawValue)): Unknown symbolSize!")
        return false
    }
    guard let mode = renderingMode.toUIImageRenderingMode() else {
        Logger.controller.error("SpatialController: GetSymbolForInputName(uid:\(uniqueId.toString()), inputName:\(inputName.rawValue), symbolSize:\(symbolScale.rawValue), renderingMode:\(renderingMode.rawValue)): Unknown renderingMode!")
        return false
    }
    let config = UIImage.SymbolConfiguration(scale: scale)
    let image = UIImage(systemName: symbolName, withConfiguration: config)?.withRenderingMode(mode)
    guard let image, let data = image.pngData() else {
        Logger.controller.error("SpatialController: GetSymbolForInputName(uid:\(uniqueId.toString()), inputName:\(inputName.rawValue), symbolSize:\(symbolScale.rawValue), renderingMode:\(renderingMode.rawValue)): No image data available for symbol \(symbolName)!")
        return false
    }
    var symbol = SCSymbol(
        width: Int32(image.size.width),
        height: Int32(image.size.height),
        data: data.toUCharP(),
        dataLength: Int32(data.count))
    callback(symbol, context, uniqueId, inputName, symbolScale, renderingMode)

    // Deallocate all mutable unsafe pointers
    symbol.deallocate()
    return true
}

@_cdecl("SpatialController_SetControllerLightColor")
public func SpatialController_SetControllerLightColor
(
    uniqueId : char_p,
    r : Float,
    g : Float,
    b : Float
) -> Bool
{
    guard #available(visionOS 1.0, macOS 10.16, iOS 14.0, tvOS 14.0, *) else {
        return false
    }
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: SetControllerLightColor(uid:\(uniqueId.toString())): Not initialized!")
        return false
    }
    guard let controller = spatialController.controllerMapping.elements[uniqueId.toString()] else {
        Logger.controller.error("SpatialController: SetControllerLightColor(uid:\(uniqueId.toString())): No such controller!")
        return false
    }
    guard let light = controller.light else {
        // REPORT ERROR: controller does not support setting light color
        return false
    }
    light.color = GCColor(red:r, green:g, blue:b)
    return true
}

@available(visionOS 1.0, macOS 13.0, iOS 16.0, tvOS 16.0, *)
fileprivate func _getSymbolNameForInput(inputName : SCControllerInputName, profile : GCDevicePhysicalInput) -> String? {
    if let buttonName = inputName.toGCButtonElementName() {
        return profile.buttons[buttonName]?.sfSymbolsName
    }
    if let directionPadName = inputName.toGCDirectionPadElementName() {
        return profile.dpads[directionPadName]?.sfSymbolsName
    }
    return nil
}

@available(visionOS 1.0, macOS 13.0, iOS 16.0, tvOS 16.0, *)
fileprivate func _getLocalizedNameForInput(inputName : SCControllerInputName, profile : GCDevicePhysicalInput) -> String? {
    if let buttonName = inputName.toGCButtonElementName() {
        return profile.buttons[buttonName]?.localizedName
    }
    if let directionPadName = inputName.toGCDirectionPadElementName() {
        return profile.dpads[directionPadName]?.localizedName
    }
    return nil
}

@available(visionOS 1.0, macOS 13.0, iOS 16.0, tvOS 16.0, *)
fileprivate func _getSymbolNameForInput(inputName : SCControllerInputName, extendedGamepad : GCExtendedGamepad?) -> String? {
    if let dualshockGamepad = extendedGamepad as? GCDualShockGamepad {
        switch inputName {
        case SCControllerInputNameButtonTouchpad:
            return dualshockGamepad.touchpadButton.sfSymbolsName
        case SCControllerInputNameDPadTouchpadPrimary:
            return dualshockGamepad.touchpadPrimary.sfSymbolsName
        case SCControllerInputNameDPadTouchpadSecondary:
            return dualshockGamepad.touchpadSecondary.sfSymbolsName
        default:
            return nil
        }
    }
    if let dualsenseGamepad = extendedGamepad as? GCDualSenseGamepad {
        switch inputName {
        case SCControllerInputNameButtonTouchpad:
            return dualsenseGamepad.touchpadButton.sfSymbolsName
        case SCControllerInputNameDPadTouchpadPrimary:
            return dualsenseGamepad.touchpadPrimary.sfSymbolsName
        case SCControllerInputNameDPadTouchpadSecondary:
            return dualsenseGamepad.touchpadSecondary.sfSymbolsName
        default:
            return nil
        }
    }
    return nil
}

@available(visionOS 1.0, macOS 13.0, iOS 16.0, tvOS 16.0, *)
fileprivate func _getLocalizedNameForInput(inputName : SCControllerInputName, extendedGamepad : GCExtendedGamepad?) -> String? {
    if let dualshockGamepad = extendedGamepad as? GCDualShockGamepad {
        switch inputName {
        case SCControllerInputNameButtonTouchpad:
            return dualshockGamepad.touchpadButton.localizedName
        case SCControllerInputNameDPadTouchpadPrimary:
            return dualshockGamepad.touchpadPrimary.localizedName
        case SCControllerInputNameDPadTouchpadSecondary:
            return dualshockGamepad.touchpadSecondary.localizedName
        default:
            return nil
        }
    }
    if let dualsenseGamepad = extendedGamepad as? GCDualSenseGamepad {
        switch inputName {
        case SCControllerInputNameButtonTouchpad:
            return dualsenseGamepad.touchpadButton.localizedName
        case SCControllerInputNameDPadTouchpadPrimary:
            return dualsenseGamepad.touchpadPrimary.localizedName
        case SCControllerInputNameDPadTouchpadSecondary:
            return dualsenseGamepad.touchpadSecondary.localizedName
        default:
            return nil
        }
    }
    return nil
}

@available(visionOS 1.0, macOS 13.0, iOS 16.0, tvOS 16.0, *)
fileprivate func _getSymbolNameForInput(inputName : SCControllerInputName, profile : GCDevicePhysicalInput, extendedGamepad : GCExtendedGamepad?) -> String? {
    return _getSymbolNameForInput(inputName: inputName, profile: profile) ?? _getSymbolNameForInput(inputName: inputName, extendedGamepad: extendedGamepad)
}

@available(visionOS 1.0, macOS 13.0, iOS 16.0, tvOS 16.0, *)
fileprivate func _getLocalizedNameForInput(inputName : SCControllerInputName, profile : GCDevicePhysicalInput, extendedGamepad : GCExtendedGamepad?) -> String? {
    return _getLocalizedNameForInput(inputName: inputName, profile: profile) ?? _getLocalizedNameForInput(inputName: inputName, extendedGamepad: extendedGamepad)
}

@_cdecl("SpatialController_PollController")
public func SpatialController_PollController
(
    uniqueId : char_p,
    callback : @escaping SCControllerStateCallback,
    context : UnsafeMutableRawPointer?
) -> Bool
{
    var state = SCControllerState()
    let uuid = String(cString: uniqueId)
    guard let spatialController = _spatialController else {
        // REPORT ERROR: not initialized?
        Logger.controller.error("SpatialController: PollController(uid:\(uuid)): Not initialized!")
        return false
    }
    guard let controller = spatialController.controllerMapping.elements[uuid] else {
        // REPORT ERROR: no such controller?
        Logger.controller.error("SpatialController: PollController(uid:\(uuid)): No such controller!")
        return false
    }

    let touchpadState = controller.extendedGamepad is GCDualShockGamepad || controller.extendedGamepad is GCDualSenseGamepad ? spatialController.touchpadStates[uuid] : nil
    _pollDeviceInputState(controller: controller, profile: controller.input, touchpadState: touchpadState, input: &state.input)
    _pollControllerBattery(controller: controller, profile: controller.battery, battery: &state.battery)
    _pollControllerMotion(controller: controller, profile: controller.motion, motion: &state.motion)
    _pollControllerLight(controller: controller, profile: controller.light, light: &state.light)

    let accessory = spatialController.accessoryTrackers[uuid]?.accessory
    _pollSpatialController(accessory: accessory, accessoryAnchors: &state.anchors)

    if let accessory {
        state.accessory = [accessory.toSCAccessory(uid: uuid)].toUnsafeMutablePointer()
    } else {
        state.accessory = nil
    }

    callback(state, context, uniqueId)

    // Deallocate all mutable unsafe pointers
    state.deallocate()
    return true
}

fileprivate func _pollDeviceInputState(
    controller : GCController,
    profile : GCDevicePhysicalInputState,
    touchpadState : TouchpadControllerState?,
    input : inout SCControllerInputState)
{
    // Buttons...
    let allButtonNames: [SCControllerInputName] = [
        SCControllerInputNameButtonHome,
        SCControllerInputNameButtonMenu,
        SCControllerInputNameButtonOptions,
        SCControllerInputNameButtonShare,
        SCControllerInputNameButtonA,
        SCControllerInputNameButtonB,
        SCControllerInputNameButtonX,
        SCControllerInputNameButtonY,
        SCControllerInputNameButtonGrip,
        SCControllerInputNameButtonLeftShoulder,
        SCControllerInputNameButtonRightShoulder,
        SCControllerInputNameButtonLeftBumper,
        SCControllerInputNameButtonRightBumper,
        SCControllerInputNameButtonTrigger,
        SCControllerInputNameButtonLeftTrigger,
        SCControllerInputNameButtonRightTrigger,
        SCControllerInputNameButtonThumbstick,
        SCControllerInputNameButtonLeftThumbstick,
        SCControllerInputNameButtonRightThumbstick,
        // ButtonBackLeft(position),
        // ButtonBackRight(position),
        // ButtonArcade(row,column),
        SCControllerInputNameButtonStylusTip,
        SCControllerInputNameButtonStylusPrimary,
        SCControllerInputNameButtonStylusSecondary
    ]
    let allDPadNames: [SCControllerInputName] = [
        SCControllerInputNameDPadDirectionPad,
        SCControllerInputNameDPadThumbstick,
        SCControllerInputNameDPadLeftThumbstick,
        SCControllerInputNameDPadRightThumbstick,
    ]

    var buttonStates: [SCInputButtonState] = []
    for inputName in allButtonNames {
        if let buttonName = inputName.toGCButtonElementName() {
            if let button = profile.buttons[buttonName] {
                buttonStates.append(SCInputButtonState(name: inputName, state: button.toSCButtonState()))
            }
        }
    }
    var dpadStates: [SCInputDPadState] = []
    for inputName in allDPadNames {
        if let dpadName = inputName.toGCDirectionPadElementName() {
            if let dpad = profile.dpads[dpadName] {
                dpadStates.append(SCInputDPadState(name: inputName, state: dpad.toSCDPadState()))
            }
        }
    }
    if let gamepad = controller.extendedGamepad as? GCDualShockGamepad {
        buttonStates.append(SCInputButtonState(name: SCControllerInputNameButtonTouchpad, state: gamepad.touchpadButton.toSCButtonState(touchpadState?.touchpadButton)))
        dpadStates.append(SCInputDPadState(name: SCControllerInputNameDPadTouchpadPrimary, state: gamepad.touchpadPrimary.toSCDPadState()))
        dpadStates.append(SCInputDPadState(name: SCControllerInputNameDPadTouchpadSecondary, state: gamepad.touchpadSecondary.toSCDPadState()))
    } else if let gamepad = controller.extendedGamepad as? GCDualSenseGamepad {
        buttonStates.append(SCInputButtonState(name: SCControllerInputNameButtonTouchpad, state: gamepad.touchpadButton.toSCButtonState(touchpadState?.touchpadButton)))
        dpadStates.append(SCInputDPadState(name: SCControllerInputNameDPadTouchpadPrimary, state: gamepad.touchpadPrimary.toSCDPadState()))
        dpadStates.append(SCInputDPadState(name: SCControllerInputNameDPadTouchpadSecondary, state: gamepad.touchpadSecondary.toSCDPadState()))
    }

    input.buttons.count = Int32(buttonStates.count)
    input.buttons.ptr = buttonStates.toUnsafeMutablePointer()
    input.dpads.count = Int32(dpadStates.count)
    input.dpads.ptr = dpadStates.toUnsafeMutablePointer()
}

fileprivate func _pollControllerBattery(
    controller : GCController,
    profile : GCDeviceBattery?,
    battery : inout SCControllerBatteryState)
{
    if let profile {
        battery.level = profile.batteryLevel
        battery.state = profile.batteryState.toSCGCDeviceBatteryState() ?? SCGCDeviceBatteryStateUnknown
    } else {
        battery.level = 0
        battery.state = SCGCDeviceBatteryStateUnknown
    }
}

fileprivate func _pollControllerMotion(
    controller : GCController,
    profile : GCMotion?,
    motion : inout SCControllerMotionState)
{
    if let profile {
        motion = profile.toSCControllerMotionState()
    } else {
        motion = SCControllerMotionState(
            attitude: GCQuaternion.identity.toSCVectorFloat4(),
            rotationRate: GCRotationRate.zero.toSCVectorFloat3(),
            acceleration: GCAcceleration.zero.toSCVectorFloat3(),
            gravity: GCAcceleration.zero.toSCVectorFloat3(),
            userAcceleration: GCAcceleration.zero.toSCVectorFloat3(),
            sensorsActive: SCFalse
        )
    }
}

fileprivate func _pollControllerLight(
    controller : GCController,
    profile : GCDeviceLight?,
    light : inout SCControllerLightState)
{
    if let profile {
        light.color = profile.color.toSCColor();
    } else {
        light.color = SCColor(red: 0.0, green: 0.0, blue: 0.0)
    }
}

fileprivate func _pollSpatialController(accessory: Accessory?, accessoryAnchors: inout SCAccessoryAnchors)
{
    guard let accessory, let accessoryTrackingProvider = _spatialController?.accessoryTrackingProvider, _spatialController?.arTrackingState == .running else {
        accessoryAnchors.count = 0
        accessoryAnchors.ptr = nil
        return
    }
    let anchors = accessoryTrackingProvider.latestAnchors.filter { $0.accessory.id == accessory.id }
    if anchors.count == 0 {
        accessoryAnchors.count = 0
        accessoryAnchors.ptr = nil
        return
    }

    //anchors[0].coordinateSpace(correction: .none)

    accessoryAnchors = anchors.map { $0.toSCAccessoryAnchor() }.toSCAccessoryAnchors()
}

@_cdecl("SpatialController_GetCurrentTime")
public func SpatialController_GetCurrentTime() -> SCTimeValue
{
    return SCTimeValue(time: CACurrentMediaTime());
}

@_cdecl("SpatialController_GetPredictedNextFrameTime")
public func SpatialController_GetPredictedNextFrameTime() -> SCTimeValue
{
    let application = UIApplication.shared
    let scenes = application.connectedScenes
    guard let uiWindowScene = scenes.first(where: { $0 is UIWindowScene }) as? UIWindowScene else {
        Logger.controller.error("SpatialController: GetPredictedNextFrameTime(): Application has no UIWindowScene!")
        return SCTimeValue(time: -1.0)
    }
    guard let updateInfo = UIUpdateInfo.current(for: uiWindowScene) else {
        Logger.controller.error("SpatialController: GetPredictedNextFrameTime(): Application UIWindowScene has no UIUpdateInfo!")
        return SCTimeValue(time: -1.0)
    }
    return SCTimeValue(time: updateInfo.estimatedPresentationTime)
}

@_cdecl("SpatialController_PredictAnchor")
public func SpatialController_PredictAnchor
(
    uniqueId : char_p,
    time : SCTimeValue,
    callback : @escaping SCAccessoryAnchorCallback,
    context : UnsafeMutableRawPointer?
) -> Bool
{
    // let locationName = Accessory.LocationName(rawValue: String(cString: location.name))
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: PredictAnchor(uid:\(uniqueId.toString()): Not initialized!")
        return false
    }
    guard let accessory = spatialController.accessoryTrackers[uniqueId.toString()]?.accessory else {
        Logger.controller.error("SpatialController: PredictAnchor(uid:\(uniqueId.toString())): No such controller!")
        return false
    }
    guard let accessoryTrackingProvider = spatialController.accessoryTrackingProvider else {
        Logger.controller.error("SpatialController: PredictAnchor(uid:\(uniqueId.toString())): Accessory tracking is not running.")
        return false
    }
    let anchors = accessoryTrackingProvider.latestAnchors.filter { $0.accessory.id == accessory.id }
    if anchors.isEmpty {
        Logger.controller.error("SpatialController: PredictAnchor(uid:\(uniqueId.toString())): No anchors are tracked for this controller.")
        return false
    }
    guard let anchor = accessoryTrackingProvider.predictAnchor(for: anchors[0], at: time.time) else {
        Logger.controller.error("SpatialController: PredictAnchor(uid:\(uniqueId.toString())): Anchor prediction failed.")
        return false
    }
    var scAnchor = anchor.toSCAccessoryAnchor()
    var scAccessory = accessory.toSCAccessory(uid: uniqueId.toString())
    callback(scAnchor, scAccessory, context)

    // Deallocate all mutable unsafe pointers
    scAnchor.deallocate()
    scAccessory.deallocate()
    return true
}

extension AccessoryTrackingAuthorizationState {
    func toSCAccessoryTrackingAuthorizationState() -> SCAccessoryTrackingAuthorizationState {
        switch (self) {
        case .notSupported:
            SCAccessoryTrackingAuthorizationStateNotSupported
        case .notRequested:
            SCAccessoryTrackingAuthorizationStateNotRequested
        case .pending:
            SCAccessoryTrackingAuthorizationStatePending
        case .notAuthorized:
            SCAccessoryTrackingAuthorizationStateNotAuthorized
        case .authorized:
            SCAccessoryTrackingAuthorizationStateAuthorized
        }
    }
}

extension AccessoryTrackingState {
    func toSCAccessoryTrackingState() -> SCAccessoryTrackingState {
        switch (self) {
        case .stopped:
            SCAccessoryTrackingStateStopped
        case .arTrackingError:
            SCAccessoryTrackingStateARTrackingError
        case .runFailed:
            SCAccessoryTrackingStateARTrackingError
        case .running:
            SCAccessoryTrackingStateRunning
        }
    }
}

@_cdecl("SpatialController_GetAccessoryTrackingAuthorizationState")
public func SpatialController_GetAccessoryTrackingAuthorizationState() -> SCAccessoryTrackingAuthorizationState
{
    guard let spatialController = _spatialController else {
        // REPORT ERROR: not initialized?
        Logger.controller.error("SpatialController: GetAccessoryTrackingAuthorizationState(): Not initialized!")
        return SCAccessoryTrackingAuthorizationStateNotRequested
    }
    return spatialController.arTrackingAuthorizationState.toSCAccessoryTrackingAuthorizationState()
}

@_cdecl("SpatialController_GetAccessoryTrackingState")
public func SpatialController_GetAccessoryTrackingState() -> SCAccessoryTrackingState
{
    guard let spatialController = _spatialController else {
        // REPORT ERROR: not initialized?
        Logger.controller.error("SpatialController: GetAccessoryTrackingState(): Not initialized!")
        return SCAccessoryTrackingStateStopped
    }
    return spatialController.arTrackingState.toSCAccessoryTrackingState()

}

class SCNotificationHandler : NSObject  {
    public func setupHandlers() {
        // Add GCControllerDidConnect....
        NotificationCenter.default.addObserver(
            self,
            selector: #selector(onControllerConnected(notification:)),
            name: NSNotification.Name.GCControllerDidConnect,
            object: .none
        )

        // Add GCControllerDidDisconnect...
        NotificationCenter.default.addObserver(
            self,
            selector: #selector(onControllerDisconnected(notification:)),
            name: NSNotification.Name.GCControllerDidDisconnect,
            object: .none
        )
    }

    public func removeHandlers() {
        NotificationCenter.default.removeObserver(self)
    }

    @objc
    public func onControllerConnected(notification : NSNotification) {
        let controller = notification.object as! GCController
        Logger.controller.log("SpatialController: GCControllerDidConnect category: \(controller.productCategory)")

        guard let spatialController = _spatialController else {
            Logger.controller.error("SpatialController: INTERNAL ERROR: onControllerConnected while uninitialized!")
            return
        }
        let uuid = spatialController.controllerMapping.elements.someKey(forValue: controller) ?? UUID().uuidString
        spatialController.controllerMapping.updateValue(controller, forKey: uuid)
        if controller.isSpatialController {
            Logger.controller.log("SpatialController: Spatial controller \(uuid) connected (category: \(controller.productCategory))")
        } else if let dualshockGamepad = controller.extendedGamepad as? GCDualShockGamepad {
            spatialController.touchpadStates.updateValue(.init(dualshockGamepad), forKey: uuid)
            Logger.controller.log("SpatialController: Non-spatial DualShock controller \(uuid) connected (category: \(controller.productCategory))")
        } else if let dualsenseGamepad = controller.extendedGamepad as? GCDualSenseGamepad {
            spatialController.touchpadStates.updateValue(.init(dualsenseGamepad), forKey: uuid)
            Logger.controller.log("SpatialController: Non-spatial DualSense controller \(uuid) connected (category: \(controller.productCategory))")
        } else {
            Logger.controller.log("SpatialController: Non-spatial controller \(uuid) connected (category: \(controller.productCategory))")
        }
        spatialController.onControllerConnectedCallback?(controller.toSCController(uid: uuid))

        if let motion = controller.motion, motion.sensorsRequireManualActivation, spatialController.enableMotionSensorsDefault {
            Logger.controller.log("SpatialController: Controller \(uuid) enabling motion sensors on connection");
            controller.motion?.sensorsActive = true
        }

        guard controller.isSpatialController else {
            return
        }
        spatialController.startAccessoryTracking(uuid: uuid, controller: controller) { state, accessory in
            if state == .acquired {
                Logger.controller.log("SpatialController: Accessory for spatial controller \(uuid) connected (category: \(controller.productCategory))")
                spatialController.onAccessoryConnectedCallback?(accessory!.toSCAccessory(uid: uuid))
            }
        }
    }

    @objc
    public func onControllerDisconnected(notification : NSNotification) {
        let controller = notification.object as! GCController
        Logger.controller.log("SpatialController: GCControllerDidDisconnect category: \(controller.productCategory)")

        guard let spatialController = _spatialController else {
            Logger.controller.error("SpatialController: INTERNAL ERROR: onControllerConnected while uninitialized!")
            return
        }
        guard let uuid = spatialController.controllerMapping.elements.someKey(forValue: controller) else {
            Logger.controller.log("SpatialController: Unknown controller disconnected (category: \(controller.productCategory))")
            return
        }
        if let accessory = spatialController.accessoryTrackers[uuid]?.accessory {
            Logger.controller.log("SpatialController: Accessory for spatial controller \(uuid) disconnected (category: \(controller.productCategory))")
            spatialController.onAccessoryDisconnectedCallback?(accessory.toSCAccessory(uid: uuid))
        }
        if controller.isSpatialController {
            Logger.controller.log("SpatialController: Spatial controller \(uuid) disconnected (category: \(controller.productCategory))")
        } else if controller.extendedGamepad is GCDualShockGamepad {
            Logger.controller.log("SpatialController: Non-spatial DualShock controller \(uuid) disconnected (category: \(controller.productCategory))")
            spatialController.touchpadStates.removeValue(forKey: uuid)
        } else if controller.extendedGamepad is GCDualSenseGamepad {
            Logger.controller.log("SpatialController: Non-spatial DualSense controller \(uuid) disconnected (category: \(controller.productCategory))")
            spatialController.touchpadStates.removeValue(forKey: uuid)
        } else {
            Logger.controller.log("SpatialController: Non-spatial controller \(uuid) disconnected (category: \(controller.productCategory))")
        }
        spatialController.controllerMapping.removeValue(forKey: uuid)
        spatialController.onControllerDisconnectedCallback?(controller.toSCController(uid: uuid))

        // Remove any haptic engines for controller...
        if #available(visionOS 1.0, macOS 10.16, iOS 14.0, tvOS 14.0, *) {
            shutdownExistingHapticsEnginesForController(uuid: uuid)
        }
        spatialController.stopAccessoryTracking(uuid: uuid)
    }
}
