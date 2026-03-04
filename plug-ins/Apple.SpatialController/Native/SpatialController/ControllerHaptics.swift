//
//  ControllerHaptics.swift
//  SpatialController
//
//  Copyright © 2025 Apple, Inc. All rights reserved.
//

import Foundation
import CoreHaptics
import GameController
import OSLog

// MARK: - C API Functions

/// Creates a haptics engine for the specified controller and locality.
///
/// This function creates a new `CHHapticEngine` for the controller identified
/// by the given unique ID with the specified locality.
/// If an existing haptics engine is already associated with this controller
/// and locality, it will be shut down first.
/// Controllers which support multiple localities (excluding Default) allow
/// an independent engine per locality. See Controller.supportedLocalities.
///
/// - Parameters:
///   - uniqueId: Unique identifier of a connected controller.
///   - locality: Locality of haptics. Must be supported by controller.
/// - Returns: True if a haptics engine was created for controller uniqueId, or false if creation failed.
@_cdecl("SpatialController_CreateHapticsEngine")
public func SpatialController_CreateHapticsEngine
(
    uniqueId: char_p,
    locality: SCHapticsLocality
) -> Bool
{
    let uuid = uniqueId.toString()
    guard #available(visionOS 1.0, macOS 10.16, iOS 14.0, tvOS 14.0, *) else {
        Logger.controller.error("SpatialController: CreateHapticsEngine(\(uuid)): Haptics not supported!")
        return false
    }
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: CreateHapticsEngine(\(uuid)): Not initialized!")
        return false
    }
    guard let controller = spatialController.controllerMapping.elements[uuid] else {
        Logger.controller.error("SpatialController: CreateHapticsEngine(\(uuid)): No such controller!")
        return false
    }
    guard controller.haptics != nil else {
        Logger.controller.error("SpatialController: CreateHapticsEngine(\(uuid)): Controller does not have haptic support!")
        return false
    }
    guard let gcLocality = locality.toGCHapticsLocality() else {
        Logger.controller.error("SpatialController: CreateHapticsEngine(\(uuid)): Unknown SCHapticsLocality \(locality.rawValue)!")
        return false
    }

    shutdownExistingHapticsEngineForController(uuid: uuid, locality: gcLocality)
    guard createHapticsEngine(for: controller, withLocality: gcLocality, uuid: uuid) != nil else {
        Logger.controller.error("SpatialController: CreateHapticsEngine(\(uuid), locality:\(locality.rawValue)): Create haptic engine failed!")
        return false
    }
    return true
}

/// Stops a haptics engine for the specified controller and locality.
///
/// This function shuts down any `CHHapticEngine` for the controller identified
/// by the given unique ID with the specified locality, if running.
/// Controllers which support multiple localities (excluding Default) allow
/// an independent engine per locality. See Controller.supportedLocalities.
/// Engines with apparent locality overlap, such as All and Handles, can be
/// independently controlled, and their outputs are composed, so stopping
/// locality All, for instance, will not stop an engine running on Handles.
///
/// - Parameters:
///   - uniqueId: Unique identifier of a connected controller.
///   - locality: Locality of haptics. Must be supported by controller.
///   - onCompleted: If return value is true, onCompleted will be called when shutdown completes.
///   - context: User supplied opaque context pointer passed back to onCompleted callback.
/// - Returns: True if a haptics engine was shutdown for controller uniqueId, or false if no
///            engine needed to be shutdown.
@_cdecl("SpatialController_StopHapticsEngine")
public func SpatialController_StopHapticsEngine
(
    uniqueId: char_p,
    locality: SCHapticsLocality,
    onCompleted: @escaping SCStopHapticEngineCompletedCallback,
    context: UnsafeMutableRawPointer?
) -> Bool
{
    let uuid = uniqueId.toString()
    guard #available(visionOS 1.0, macOS 10.16, iOS 14.0, tvOS 14.0, *) else {
        Logger.controller.error("SpatialController: StopHapticsEngine(\(uuid)): Haptics not supported!")
        return false
    }
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: StopHapticsEngine(\(uuid)): Not initialized!")
        return false
    }
    guard let controller = spatialController.controllerMapping.elements[uuid] else {
        Logger.controller.error("SpatialController: StopHapticsEngine(\(uuid)): No such controller!")
        return false
    }
    guard controller.haptics != nil else {
        Logger.controller.error("SpatialController: StopHapticsEngine(\(uuid)): Controller does not have haptic support!")
        return false
    }
    guard let gcLocality = locality.toGCHapticsLocality() else {
        Logger.controller.error("SpatialController: StopHapticsEngine(\(uuid)): Unknown SCHapticsLocality \(locality.rawValue)!")
        return false
    }
    guard let engine = spatialController.controllerHapticEngines[uuid]?.removeValue(forKey: gcLocality) else {
        Logger.controller.log("SpatialController: StopHapticsEngine(\(uuid), \(String(describing: gcLocality))): No engine is running with this locality.")
        return false
    }

    engine.stop(completionHandler: { error in
        let scError: SCError = error?.toSCError() ?? .init(code: 0, localizedDescription: "".toCharPCopy())
        let uniqueIdBuffer = uuid.toCharPCopy()
        onCompleted(scError, context, uniqueIdBuffer, locality)
        uniqueIdBuffer.deallocate()
    })
    return true
}

/// Plays haptics data on the specified controller and locality.
///
/// If a haptics engine has not yet been created with the specified locality,
/// attempts to create one first.
/// Controllers which support multiple localities (excluding Default) allow
/// an independent engine per locality. See Controller.supportedLocalities.
/// Engines with apparent locality overlap, such as All and Handles, can be
/// independently controlled, and their outputs are composed.
///
/// - Parameters:
///   - uniqueId: Unique identifier of a connected controller.
///   - data: Binary haptics data from a haptics data file in a supported format such as AHAP.
///   - dataLength: Length of binary haptics data in bytes.
///   - locality: Locality of haptics. Must be supported by controller.
///   - onFinished: If return value is true, onFinished will be called when playback finishes.
///   - context: User supplied opaque context pointer passed back to onFinished callback.
/// - Returns: True if haptics playback was started for controller uniqueId, or false if the operation failed.
@_cdecl("SpatialController_PlayHapticsData")
public func SpatialController_PlayHapticsData
(
    uniqueId: char_p,
    data: uchar_p,
    dataLength: Int,
    locality: SCHapticsLocality,
    onFinished: @escaping SCHapticEngineFinishedCallback,
    context: UnsafeMutableRawPointer?
) -> Bool
{
    let uuid = uniqueId.toString()
    guard #available(visionOS 1.0, macOS 10.16, iOS 14.0, tvOS 14.0, *) else {
        Logger.controller.error("SpatialController: PlayHapticsData(\(uuid)): Haptics not supported!")
        return false
    }
    guard let spatialController = _spatialController else {
        Logger.controller.error("SpatialController: PlayHapticsData(\(uuid)): Not initialized!")
        return false
    }
    guard let controller = spatialController.controllerMapping.elements[uniqueId.toString()] else {
        Logger.controller.error("SpatialController: PlayHapticsData(\(uuid)): No such controller!")
        return false
    }
    guard controller.haptics != nil else {
        Logger.controller.error("SpatialController: PlayHapticsData(\(uuid)): Controller does not have haptic support!")
        return false
    }
    guard let gcLocality = locality.toGCHapticsLocality() else {
        Logger.controller.error("SpatialController: PlayHapticsData(\(uuid)): Unknown SCHapticsLocality \(locality.rawValue)!")
        return false
    }
    guard dataLength > 0 else {
        Logger.controller.error("SpatialController: PlayHapticsData(\(uuid)): Invalid dataLength \(dataLength)!")
        return false
    }

    var engine: CHHapticEngine!
    if let existingEngine = spatialController.controllerHapticEngines[uuid]?[gcLocality] {
        engine = existingEngine
    } else if let newEngine = createHapticsEngine(for: controller, withLocality: gcLocality, uuid: uuid) {
        engine = newEngine
    }
    guard engine != nil else {
        Logger.controller.error("SpatialController: PlayHapticsData(\(uuid), locality:\(locality.rawValue)): Create haptic engine failed!")
        return false
    }

    let buffer = Data(bytes: data, count: dataLength)
    do {
        // Start the engine in case it's idle.
        try engine.start()
        // Tell the engine to play the pattern.
        try engine.playPattern(from: buffer)
        // Notify finished
        engine.notifyWhenPlayersFinished { error in
            let scError: SCError = error?.toSCError() ?? .init(code: 0, localizedDescription: "".toCharPCopy())
            let uniqueIdBuffer = uuid.toCharPCopy()
            let scAction = onFinished(scError, context, uniqueIdBuffer, locality)
            uniqueIdBuffer.deallocate()
            return scAction.toCHHapticEngineFinishedAction() ?? .leaveEngineRunning
        }
    } catch { // Engine startup errors
        Logger.controller.error("SpatialController: PlayHapticsData(\(uuid), locality:\(locality.rawValue)): Error playing data (\(dataLength) bytes): \(error).")
        return false
    }
    return true
}

// MARK: - Internal Functions

/// Shuts down the haptics engine for the specified controller with the specified locality.
///
/// This function stops and removes a single `CHHapticEngine` currently associated with the
/// controller with the specified locality.
/// The engine is stopped asynchronously and removed from the controller haptic engines dictionary.
///
/// - Parameter controller: The target `GCController` to create the haptics engine for.
/// - Parameter locality: The `GCHapticsLocality` of the engine to be created.
/// - Parameter uuid: The UUID (as String) of the target controller.
@available(iOS 14.0, tvOS 14.0, macOS 10.16, *)
public func createHapticsEngine(for controller: GCController, withLocality locality: GCHapticsLocality, uuid: String) -> CHHapticEngine?
{
    guard let spatialController = _spatialController else {
        return nil
    }
    guard let haptics = controller.haptics else {
        return nil
    }
    guard let engine = haptics.createEngine(withLocality: locality) else {
        return nil
    }

    let engineName = uuid + ":" + locality.rawValue
    engine.stoppedHandler = { reason in
        Logger.controller.log("SpatialController: Controller \(engineName) haptics engine stopped because \(reason.message)")
    }
    engine.resetHandler = {
        // Try restarting the engine.
        Logger.controller.log("SpatialController: Controller \(engineName) haptics engine reset, restarting now...")
        do {
            try engine.start()
        } catch {
            Logger.controller.error("SpatialController: Controller \(engineName) haptics engine restart failed: \(error)")
        }
    }

    spatialController.controllerHapticEngines[uuid, default: [:]].updateValue(engine, forKey: locality)
    return engine
}

/// Shuts down all existing haptics engines for the specified controller.
///
/// This function stops and removes any `CHHapticEngine` currently associated with the controller.
/// The engines are stopped asynchronously and removed from the controller haptic engines dictionary.
///
/// - Parameter uuid: The UUID (as String) of the controller whose haptics engines should be shut down.
@available(iOS 14.0, tvOS 14.0, macOS 10.16, *)
public func shutdownExistingHapticsEnginesForController(uuid: String)
{
    guard let spatialController = _spatialController else {
        return
    }
    // Shutdown all existing engines for controller, if any...
    if let existingEngines = spatialController.controllerHapticEngines.removeValue(forKey: uuid) {
        for existingEngine in existingEngines.values {
            existingEngine.stop(completionHandler: { error in
                if let error {
                    Logger.controller.error("SpatialController: Controller \(uuid) haptics engines shutdown failed: \(error)")
                }
            })
        }
    }
}

/// Shuts down the haptics engine for the specified controller with the specified locality.
///
/// This function stops and removes a single `CHHapticEngine` currently associated with the
/// controller with the specified locality.
/// The engine is stopped asynchronously and removed from the controller haptic engines dictionary.
///
/// - Parameter uuid: The UUID (as String) of the controller whose haptics engine should be shut down.
/// - Parameter locality: The `GCHapticsLocality` of the engine to be shut down.
@available(iOS 14.0, tvOS 14.0, macOS 10.16, *)
public func shutdownExistingHapticsEngineForController(uuid: String, locality : GCHapticsLocality)
{
    guard let spatialController = _spatialController else {
        return
    }
    // Shutdown existing engine for locality, if any...
    if let existingEngine = spatialController.controllerHapticEngines[uuid]?.removeValue(forKey: locality) {
        existingEngine.stop(completionHandler: { error in
            if let error {
                Logger.controller.error("SpatialController: Controller \(uuid) haptics engine \(String(describing: locality)) shutdown failed: \(error)")
            }
        })
    }
}

extension CHHapticEngine.StoppedReason {
    var message: String {
        switch self {
        case .audioSessionInterrupt:
            return "the audio session was interrupted."
        case .applicationSuspended:
            return "the application was suspended."
        case .idleTimeout:
            return "an idle timeout occurred."
        case .systemError:
            return "a system error occurred."
        case .notifyWhenFinished:
            return "playback finished."
        case .engineDestroyed:
            return "the engine was destroyed."
        case .gameControllerDisconnect:
            return "the game controller disconnected."
        @unknown default:
            fatalError()
        }
    }
}

extension SCHapticEngineFinishedAction {
    func toCHHapticEngineFinishedAction() -> CHHapticEngine.FinishedAction? {
        switch (self) {
        case SCHapticEngineFinishedActionLeaveEngineRunning:
            .leaveEngineRunning
        case SCHapticEngineFinishedActionStopEngine:
            .stopEngine
        default:
            nil
        }
    }
}

extension CHHapticEngine.FinishedAction {
    func toSCHapticEngineFinishedAction() -> SCHapticEngineFinishedAction? {
        switch (self) {
        case .leaveEngineRunning:
            SCHapticEngineFinishedActionLeaveEngineRunning
        case .stopEngine:
            SCHapticEngineFinishedActionStopEngine
        default:
            nil
        }
    }
}
