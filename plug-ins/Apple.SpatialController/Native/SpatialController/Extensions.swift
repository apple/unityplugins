//
//  Extensions.swift
//  SpatialController
//
//  Copyright © 2025 Apple, Inc. All rights reserved.
//

import Foundation
import ARKit
import GameController

// MARK: - String Extensions

public extension String {
    /// Converts a Swift String to a C-compatible character pointer.
    ///
    /// This method creates a copy of the string as an `UnsafeMutablePointer<Int8>` to match
    /// `char *` expectations in C code.
    ///
    /// - Returns: An `UnsafeMutablePointer<Int8>` (char_p) containing a copy of the string.
    /// - Note: The caller is responsible for deallocating the returned pointer.
    /// - Remark: Doing this in Swift allows for C# to simply use string instead of char *.
    func toCharPCopy() -> char_p {
        let utfText = (self as NSString).utf8String!
        let pointer = UnsafeMutablePointer<Int8>.allocate(capacity: (8 * self.count) + 1)
        return strcpy(pointer, utfText)
    }
}

// MARK: - Character Pointer Extensions

public extension char_p {
    /// Converts a C character pointer to a Swift String.
    ///
    /// - Returns: A Swift String created from the C string.
    func toString() -> String {
        return String(cString: self)
    }
}

// MARK: - Data Extensions

public extension Data {
    /// Converts Data to an unsigned character pointer.
    ///
    /// Creates an `UnsafeMutablePointer<UInt8>` and copies the data bytes into it.
    ///
    /// - Returns: An `UnsafeMutablePointer<UInt8>` containing the data bytes.
    /// - Note: The caller is responsible for deallocating the returned pointer.
    func toUCharP() -> uchar_p {
        let pointer = uchar_p.allocate(capacity: self.count)
        let buffer = UnsafeMutableBufferPointer<UInt8>(start: pointer, count: self.count)
        _ = self.copyBytes(to: buffer)
        return pointer
    }
}

// MARK: - Unsigned Character Pointer Extensions

public extension uchar_p {
    /// Converts an unsigned character pointer back to Data.
    ///
    /// - Parameter count: The number of bytes to read from the pointer.
    /// - Returns: A Data object containing the bytes from the pointer.
    func toData(count : Int) -> Data {
        let buffer = UnsafeMutableBufferPointer<UInt8>(start: self, count: count)
        return Data(buffer: buffer)
    }
}

// MARK: - Error Extensions

public extension Error {
    /// Extracts the error code from an Error.
    ///
    /// - Returns: The error code as an integer.
    func code() -> Int {
        return (self as NSError).code
    }
    
    /// Converts a Swift Error to an SCError structure.
    ///
    /// - Returns: An SCError containing the error code and localized description.
    func toSCError() -> SCError {
        return SCError(
            code: Int32(self.code()),
            localizedDescription: self.localizedDescription.toCharPCopy())
    }
}

// MARK: - Array Extensions

public extension Array {
    /// Converts an Array to an UnsafeMutablePointer.
    ///
    /// Creates a pointer and copies all array elements into it.
    ///
    /// - Returns: An `UnsafeMutablePointer<Element>` containing the array elements.
    /// - Note: The caller is responsible for deallocating the returned pointer.
    func toUnsafeMutablePointer() -> UnsafeMutablePointer<Element> {
        let ptr = UnsafeMutablePointer<Element>.allocate(capacity: self.count)
        ptr.update(from: self, count: self.count)

        return ptr
    }
}

// MARK: - UnsafeMutablePointer Extensions

public extension UnsafeMutablePointer {
    /// Converts an UnsafeMutablePointer to an Array.
    ///
    /// - Parameter length: The number of elements to read from the pointer.
    /// - Returns: An array containing the elements from the pointer.
    func toArray(length : Int) -> [Pointee] {
        return Array(UnsafeBufferPointer(start: self, count: length))
    }
    
    /// Converts an UnsafeMutablePointer to an Array with type casting.
    ///
    /// - Parameters:
    ///   - length: The number of elements to read from the pointer.
    /// - Returns: An array of type T containing the elements from the pointer.
    func toArray<T>(length : Int) -> [T] {
        let rawPtr = UnsafeMutableRawPointer(self)
        let ptr = rawPtr.bindMemory(to: T.self, capacity: length)
        let buffer = UnsafeBufferPointer(start: ptr, count: length)
        
        return Array(buffer)
    }
}

// MARK: - Dictionary Extensions

/// Support bidirectional dictionary look-ups.
extension Dictionary where Value: Equatable {
    /// Finds a key for a given value in the dictionary.
    ///
    /// - Parameter val: The value to search for.
    /// - Returns: The first key that maps to the given value, or nil if not found.
    func someKey(forValue val: Value) -> Key? {
        return first(where: { $1 == val })?.key
    }
}

// MARK: - Bool Extensions

extension Bool {
    /// Converts a Boolean value to a Float.
    ///
    /// - Returns: 1.0 if true, 0.0 if false.
    func toFloat() -> Float {
        return self ? 1 : 0
    }
}

// MARK: - GCController Extensions

extension GCController {
    /// Converts a GCController to an SCController structure.
    ///
    /// - Parameter uid: The unique identifier for the controller.
    /// - Returns: An SCController containing the controller's properties.
    func toSCController(uid : String) -> SCController {
        var hasBattery = false
        var hasMotion = false
        var hasLight = false
        var hasHaptics = false
        var scMotion = SCControllerMotionCapabilities(hasAttitude: SCFalse, hasRotationRate: SCFalse, hasGravityAndUserAcceleration: SCFalse, sensorsRequireManualActivation: SCFalse)
        var scHaptics = SCDeviceHaptics(supportedLocalities: 0)
        if #available(visionOS 1.0, macOS 10.16, iOS 14.0, tvOS 14.0, *) {
            hasBattery = self.battery != nil
            hasMotion = self.motion != nil
            hasLight = self.light != nil
            hasHaptics = self.haptics != nil
            if let motion {
                scMotion = motion.toSCControllerMotionCapabilities()
            }
            if let haptics {
                scHaptics = haptics.toSCDeviceHaptics()
            }
        }
        let vendorName: String = self.vendorName ?? ""
        return SCController(
            uniqueId: uid.toCharPCopy(),
            productCategory: self.productCategory.toCharPCopy(),
            vendorName: vendorName.toCharPCopy(),
            isAttachedToDevice: self.isAttachedToDevice ? SCTrue : SCFalse,
            hasBattery: hasBattery ? SCTrue : SCFalse,
            hasMotion: hasMotion ? SCTrue : SCFalse,
            hasLight: hasLight ? SCTrue : SCFalse,
            hasHaptics: hasHaptics ? SCTrue : SCFalse,
            motion: scMotion,
            haptics: scHaptics
        )
    }

    var isSpatialController : Bool {
        guard #available(visionOS 26.0, *) else {
            return false
        }
        return productCategory == GCProductCategorySpatialController || productCategory == GCProductCategorySpatialStylus
    }
}

extension GCDeviceHaptics {
    func toSCDeviceHaptics() -> SCDeviceHaptics {
        var supportedLocalities: UInt32 = 0
        if #available(visionOS 1.0, macOS 10.16, iOS 14.0, tvOS 14.0, *) {
            for locality in self.supportedLocalities {
                if let scLocality = locality.toSCHapticsLocality() {
                    supportedLocalities |= (1 << scLocality.rawValue)
                }
            }
        }
        return SCDeviceHaptics(supportedLocalities: supportedLocalities)
    }
}

extension GCQuaternion {
    func toSCVectorFloat4() -> SCVectorFloat4 {
        SCVectorFloat4(x: Float(x), y: Float(y), z: Float(z), w: Float(w))
    }

    static var identity: GCQuaternion {
        GCQuaternion(x: 0, y: 0, z: 0, w: 1)
    }
}

extension GCRotationRate {
    func toSCVectorFloat3() -> SCVectorFloat3 {
        SCVectorFloat3(x: Float(x), y: Float(y), z: Float(z))
    }

    static var zero: GCRotationRate {
        GCRotationRate(x: 0, y: 0, z: 0)
    }
}

extension GCAcceleration {
    func toSCVectorFloat3() -> SCVectorFloat3 {
        SCVectorFloat3(x: Float(x), y: Float(y), z: Float(z))
    }

    static var zero: GCAcceleration {
        GCAcceleration(x: 0, y: 0, z: 0)
    }
}

extension GCMotion {
    func toSCControllerMotionCapabilities() -> SCControllerMotionCapabilities {
        SCControllerMotionCapabilities(
            hasAttitude: hasAttitude ? SCTrue : SCFalse,
            hasRotationRate: hasRotationRate ? SCTrue : SCFalse,
            hasGravityAndUserAcceleration: hasGravityAndUserAcceleration ? SCTrue : SCFalse,
            sensorsRequireManualActivation: sensorsRequireManualActivation ? SCTrue : SCFalse
        )
    }

    func toSCControllerMotionState() -> SCControllerMotionState {
        SCControllerMotionState(
            attitude: (hasAttitude ? attitude : .identity).toSCVectorFloat4(),
            rotationRate: (hasRotationRate ? rotationRate : .zero).toSCVectorFloat3(),
            acceleration: (hasGravityAndUserAcceleration ? acceleration : .zero).toSCVectorFloat3(),
            gravity: (hasGravityAndUserAcceleration ? gravity : .zero).toSCVectorFloat3(),
            userAcceleration: (hasGravityAndUserAcceleration ? userAcceleration : .zero).toSCVectorFloat3(),
            sensorsActive: sensorsActive ? SCTrue : SCFalse
        )
    }
}

// MARK: - SCController Extensions

extension SCController {
    /// Deallocates the memory used by the SCController's string properties.
    ///
    /// This method should be called when the SCController is no longer needed
    /// to prevent memory leaks.
    mutating func deallocate() {
        uniqueId.deallocate()
        uniqueId = nil
        productCategory.deallocate()
        productCategory = nil
        vendorName.deallocate()
        vendorName = nil
    }
}

// MARK: - SCController Array Extensions

public extension Array where Element == SCController {
    /// Converts an array of SCController to an SCControllers structure.
    ///
    /// - Returns: An SCControllers structure containing a pointer to the array and its count.
    func toSCControllers() -> SCControllers {
        SCControllers(ptr: self.toUnsafeMutablePointer(), count: Int32(self.count))
    }
}

// MARK: - SCControllers Extensions

extension SCControllers {
    /// Deallocates the memory used by the SCControllers structure.
    ///
    /// This method deallocates each controller in the array and then the array itself.
    mutating func deallocate() {
        if count > 0 {
            for var controller in ptr.toArray(length: Int(count)) {
                controller.deallocate()
            }
            ptr.deallocate()
            ptr = nil
            count = 0
        }
    }
}

// MARK: - SIMD Extensions

extension simd_float4x4 {
    /// Converts a simd_float4x4 to an SCTransformFloat4x4.
    ///
    /// - Returns: The same matrix as an SCTransformFloat4x4 type.
    func toSCTransformFloat4x4() -> SCTransformFloat4x4 {
        return self
    }
}

extension simd_float4 {
    func toSCVectorFloat4() -> SCVectorFloat4 {
        return SCVectorFloat4(x: x, y: y, z: z, w: w)
    }
}

extension SIMD3<Float> {
    /// Converts a SIMD3<Float> to an SCVectorFloat3.
    ///
    /// - Returns: An SCVectorFloat3 with the same x, y, z components.
    func toSCVectorFloat3() -> SCVectorFloat3 {
        return SCVectorFloat3(x: self.x, y: self.y, z: self.z)
    }
}

// MARK: - GCColor Extensions

extension GCColor
{
    func toSCColor() -> SCColor {
        return SCColor(red: red, green: green, blue: blue)
    }
}

// MARK: - Accessory Extensions

extension Accessory.Chirality {
    /// Converts an Accessory.Chirality to an SCAccessoryChirality.
    ///
    /// - Returns: The corresponding SCAccessoryChirality value.
    func toSCAccessoryChirality() -> SCAccessoryChirality {
        switch self {
        case .unspecified:
            return SCAccessoryChiralityUnspecified
        case .left:
            return SCAccessoryChiralityLeft
        case .right:
            return SCAccessoryChiralityRight
        @unknown default:
            return SCAccessoryChiralityUnspecified
        }
    }
}

// MARK: - AccessoryAnchor Extensions

extension AccessoryAnchor.TrackingState {
    /// Converts an AccessoryAnchor.TrackingState to an SCAccessoryTrackingState.
    ///
    /// - Returns: The corresponding SCAccessoryTrackingState value.
    func toSCAccessoryAnchorTrackingState() -> SCAccessoryAnchorTrackingState {
        switch self {
        case .untracked:
            return SCAccessoryAnchorTrackingStateUntracked
        case .orientationTracked:
            return SCAccessoryAnchorTrackingStateOrientationTracked
        case .positionOrientationTracked:
            return SCAccessoryAnchorTrackingStatePositionOrientationTracked
        case .positionOrientationTrackedLowAccuracy:
            return SCAccessoryAnchorTrackingStatePositionOrientationTrackedLowAccuracy
        @unknown default:
            return SCAccessoryAnchorTrackingStateUntracked
        }
    }
}

// MARK: - Accessory.LocationName Extensions

extension Accessory.LocationName {
    /// Converts an Accessory.LocationName to an SCAccessoryLocation.
    ///
    /// - Returns: An SCAccessoryLocation containing the location name.
    func toSCAccessoryLocation() -> SCAccessoryLocation {
        SCAccessoryLocation(name: self.rawValue.toCharPCopy())
    }
}

extension SCAccessoryLocation {
    /// Deallocates the memory used by the SCAccessoryLocation's name property.
    mutating func deallocate() {
        name.deallocate()
        name = nil
    }
}

// MARK: - Accessory.LocationName Array Extensions

public extension Array where Element == Accessory.LocationName {
    /// Converts an array of Accessory.LocationName to an SCAccessoryLocations structure.
    ///
    /// - Returns: An SCAccessoryLocations structure containing the converted locations.
    func toSCAccessoryLocations() -> SCAccessoryLocations {
        let locations = self.map { $0.toSCAccessoryLocation() }
        return SCAccessoryLocations(ptr: locations.toUnsafeMutablePointer(), count: Int32(locations.count))
    }
}

extension SCAccessoryLocations {
    /// Deallocates the memory used by the SCAccessoryLocations structure.
    mutating func deallocate() {
        if count > 0 {
            for var location in ptr.toArray(length: Int(count)) {
                location.deallocate()
            }
            ptr.deallocate()
            ptr = nil
            count = 0
        }
    }
}

// MARK: - Accessory Extensions

extension Accessory {
    /// The GCController associated with this accessory, if available.
    ///
    /// - Returns: The GCController if the source is a device that conforms to GCController, nil otherwise.
    var controller: GCController? {
        switch (source) {
        case let .device(device):
            return device as? GCController
        default:
            return nil
        }
    }

    /// Converts an Accessory to an SCAccessory structure.
    ///
    /// - Parameter uid: The unique identifier for the accessory.
    /// - Returns: An SCAccessory containing the accessory's properties.
    func toSCAccessory(uid : String) -> SCAccessory {
        let usdzFile: String = self.usdzFile?.absoluteString ?? ""
        return SCAccessory(
            id: self.id.uuidString.toCharPCopy(),
            name: self.name.toCharPCopy(),
            usdzFile: usdzFile.toCharPCopy(),
            description: self.description.toCharPCopy(),
            inherentChirality: self.inherentChirality.toSCAccessoryChirality(),
            locations: self.locations.toSCAccessoryLocations(),
            source: self.controller!.toSCController(uid: uid)
        )
    }
}

extension SCAccessory {
    /// Deallocates the memory used by the SCAccessory's string properties.
    mutating func deallocate() {
        id.deallocate()
        id = nil
        name.deallocate()
        name = nil
        usdzFile.deallocate()
        usdzFile = nil
        description.deallocate()
        description = nil
        source.deallocate()
    }
}

// MARK: - SCAccessory Array Extensions

public extension Array where Element == SCAccessory {
    /// Converts an array of SCAccessory to an SCAccessories structure.
    ///
    /// - Returns: An SCAccessories structure containing a pointer to the array and its count.
    func toSCAccessories() -> SCAccessories {
        SCAccessories(ptr: self.toUnsafeMutablePointer(), count: Int32(self.count))
    }
}

extension SCAccessories {
    /// Deallocates the memory used by the SCAccessories structure.
    mutating func deallocate() {
        if count > 0 {
            for var accessory in ptr.toArray(length: Int(count)) {
                accessory.deallocate()
            }
            ptr.deallocate()
            ptr = nil
            count = 0
        }
    }
}

// MARK: - AccessoryAnchor Extensions

extension AccessoryAnchor {
    /// Converts an AccessoryAnchor to an SCAccessoryAnchor structure.
    ///
    /// - Returns: An SCAccessoryAnchor containing the anchor's properties and state.
    func toSCAccessoryAnchor() -> SCAccessoryAnchor {
        var locationPoses: [SCPose] = []
        let corrections: [ARKitCoordinateSpace.Correction] = [ .none, .rendered ]
        for correction in corrections {
            for location in accessory.locations {
                let pose = coordinateSpace(for: location, correction: correction).ancestorFromSpaceTransformFloat()
                let position = pose.translation.vector
                let rotation = pose.rotation?.quaternion.vector ?? simd_float4(0, 0, 0, 1)
                let scPose = SCPose(position: position.toSCVectorFloat3(), rotation: rotation.toSCVectorFloat4())
                locationPoses.append(scPose)
            }
            let pose = coordinateSpace(correction: correction).ancestorFromSpaceTransformFloat()
            let position = pose.translation.vector
            let rotation = pose.rotation?.quaternion.vector ?? simd_float4(0, 0, 0, 1)
            let scPose = SCPose(position: position.toSCVectorFloat3(), rotation: rotation.toSCVectorFloat4())
            locationPoses.append(scPose)
        }

        return SCAccessoryAnchor(
            id: self.id.uuidString.toCharPCopy(),
            description: self.description.toCharPCopy(),
            originFromAnchorTransform: self.originFromAnchorTransform.toSCTransformFloat4x4(),
            velocity: self.velocity.toSCVectorFloat3(),
            angularVelocity: self.angularVelocity.toSCVectorFloat3(),
            timestamp: .init(time: self.timestamp),
            trackingState: self.trackingState.toSCAccessoryAnchorTrackingState(),
            heldChirality: self.heldChirality?.toSCAccessoryChirality() ?? SCAccessoryChiralityUnspecified,
            isHeld: (self.heldChirality != nil) ? SCTrue : SCFalse,
            isTracked: self.isTracked ? SCTrue : SCFalse,
            locationPoses: locationPoses.toSCPoses()
        )
    }
}

// MARK: - SCAccessoryAnchor Array Extensions

public extension Array where Element == SCAccessoryAnchor {
    /// Converts an array of SCAccessoryAnchor to an SCAccessoryAnchors structure.
    ///
    /// - Returns: An SCAccessoryAnchors structure containing a pointer to the array and its count.
    func toSCAccessoryAnchors() -> SCAccessoryAnchors {
        SCAccessoryAnchors(ptr: self.toUnsafeMutablePointer(), count: Int32(self.count))
    }
}

public extension Array where Element == SCPose {
    /// Converts an array of SCPose to an SCPoses structure.
    ///
    /// - Returns: An SCPoses structure containing a pointer to the array and its count.
    func toSCPoses() -> SCPoses {
        SCPoses(ptr: self.toUnsafeMutablePointer(), count: Int32(self.count))
    }
}

extension SCPoses {
    /// Deallocates the memory used by the SCPoses structure.
    /// This method deallocates only the array itself.
    mutating func deallocate() {
        if count > 0 {
            ptr.deallocate()
            ptr = nil
            count = 0
        }
    }
}

extension SCAccessoryAnchors {
    /// Deallocates the memory used by the SCAccessoryAnchors structure.
    mutating func deallocate() {
        if count > 0 {
            for var anchor in ptr.toArray(length: Int(count)) {
                anchor.deallocate()
            }
            ptr.deallocate()
            ptr = nil
            count = 0
        }
    }
}

extension SCAccessoryAnchor {
    /// Deallocates the memory used by the SCAccessoryAnchor's string properties.
    mutating func deallocate() {
        id.deallocate()
        id = nil
        description.deallocate()
        description = nil
        locationPoses.deallocate()
    }
}

// MARK: - SCSymbol Extensions

extension SCSymbol {
    /// Deallocates the memory used by the SCSymbol's data property.
    mutating func deallocate() {
        if dataLength > 0 {
            data.deallocate()
            data = nil
            dataLength = 0
        }
    }
}

// MARK: - SCControllerInputInfo Extensions

extension SCControllerInputInfo {
    /// Initializes an SCControllerInputInfo with optional string parameters.
    ///
    /// - Parameters:
    ///   - localizedName: The localized name of the input, or nil.
    ///   - symbolName: The symbol name of the input, or nil.
    init(localizedName: String?, symbolName: String?)
    {
        self = .init(
            localizedName: (localizedName ?? "").toCharPCopy(),
            symbolName: (symbolName ?? "").toCharPCopy()
        )
    }

    /// Deallocates the memory used by the SCControllerInputInfo's string properties.
    mutating func deallocate() {
        localizedName.deallocate()
        symbolName.deallocate()
    }
}

// MARK: - GCButtonElement Extensions

extension GCButtonElement {
    /// Converts a GCButtonElement to an SCButtonState.
    ///
    /// - Returns: An SCButtonState containing the button's current value.
    func toSCButtonState() -> SCButtonState {
        // pressedInput.canWrap?
        // touchInput?.lastTouchedStateTimestamp
        // forceInput?
        return SCButtonState(
            lastPressedStateTimestamp: .init(time: pressedInput.lastPressedStateTimestamp),
            lastValueTimestamp: .init(time: pressedInput.lastValueTimestamp),
            value: pressedInput.value,
            isAnalog: pressedInput.isAnalog ? SCTrue : SCFalse,
            isPressed: pressedInput.isPressed ? SCTrue : SCFalse,
            isTouched: (touchedInput?.isTouched ?? false) ? SCTrue : SCFalse,
        )
    }
}

extension GCControllerButtonInput {
    /// Converts a GCControllerButtonInput to an SCButtonState.
    ///
    /// - Returns: An SCButtonState containing the button's current value.
    func toSCButtonState(_ timestamps : ButtonTimestamps?) -> SCButtonState {
        // timestamps?.lastTouchedStateTimestamp
        return SCButtonState(
            lastPressedStateTimestamp: .init(time: timestamps?.lastPressedStateTimestamp ?? 0.0),
            lastValueTimestamp: .init(time: timestamps?.lastValueTimestamp ?? 0.0),
            value: self.value,
            isAnalog: self.isAnalog ? SCTrue : SCFalse,
            isPressed: self.isPressed ? SCTrue : SCFalse,
            isTouched: self.isTouched ? SCTrue : SCFalse,
        )
    }
}

// MARK: - GCDirectionPadElement Extensions

extension GCDirectionPadElement {
    /// Converts a GCDirectionPadElement to an SCDPadState.
    ///
    /// - Returns: An SCDPadState containing the directional pad's x and y axis values.
    func toSCDPadState() -> SCDPadState {
        return SCDPadState(xAxis: self.xAxis.value, yAxis: self.yAxis.value)
    }
}

extension GCControllerDirectionPad {
    /// Converts a GCControllerDirectionPad to an SCDPadState.
    ///
    /// - Returns: An SCDPadState containing the directional pad's x and y axis values.
    func toSCDPadState() -> SCDPadState {
        return SCDPadState(xAxis: self.xAxis.value, yAxis: self.yAxis.value)
    }
}

// MARK: - SCControllerInputState Extensions

extension SCControllerInputState {
    /// Deallocates the memory used by the SCControllerInputState's arrays.
    mutating func deallocate() {
        if buttons.count > 0 {
            buttons.ptr.deallocate()
            buttons.ptr = nil
            buttons.count = 0
        }
        if dpads.count > 0 {
            dpads.ptr.deallocate()
            dpads.ptr = nil
            dpads.count = 0
        }
    }
}

// MARK: - SCControllerState Extensions

extension SCControllerState {
    /// Deallocates the memory used by the SCControllerState's properties.
    mutating func deallocate() {
        input.deallocate()
        anchors.deallocate()
        if accessory != nil {
            accessory.deallocate()
            accessory = nil
        }
    }
}

// MARK: - enum Extensions

extension SCControllerInputName {
    @available(macOS 13.0, iOS 16.0, tvOS 16.0, *)
    func toGCButtonElementName() -> GCButtonElementName? {
        switch self {
        case SCControllerInputNameButtonHome:
            .home
        case SCControllerInputNameButtonMenu:
            .menu
        case SCControllerInputNameButtonOptions:
            .options
        case SCControllerInputNameButtonShare:
            .share
        case SCControllerInputNameButtonA:
            .a
        case SCControllerInputNameButtonB:
            .b
        case SCControllerInputNameButtonX:
            .x
        case SCControllerInputNameButtonY:
            .y
        case SCControllerInputNameButtonGrip:
            .grip
        case SCControllerInputNameButtonLeftShoulder:
            .leftShoulder
        case SCControllerInputNameButtonRightShoulder:
            .rightShoulder
        case SCControllerInputNameButtonLeftBumper:
            .leftBumper
        case SCControllerInputNameButtonRightBumper:
            .rightBumper
        case SCControllerInputNameButtonTrigger:
            .trigger
        case SCControllerInputNameButtonLeftTrigger:
            .leftTrigger
        case SCControllerInputNameButtonRightTrigger:
            .rightTrigger
        case SCControllerInputNameButtonThumbstick:
            .thumbstickButton
        case SCControllerInputNameButtonLeftThumbstick:
            .leftThumbstickButton
        case SCControllerInputNameButtonRightThumbstick:
            .rightThumbstickButton
        case SCControllerInputNameButtonStylusTip:
            .stylusTip
        case SCControllerInputNameButtonStylusPrimary:
            .stylusPrimaryButton
        case SCControllerInputNameButtonStylusSecondary:
            .stylusSecondaryButton
        default:
            nil
        }
    }

    @available(macOS 13.0, iOS 16.0, tvOS 16.0, *)
    func toGCDirectionPadElementName() -> GCDirectionPadElementName? {
        switch self {
        case SCControllerInputNameDPadDirectionPad:
            .directionPad
        case SCControllerInputNameDPadThumbstick:
            .thumbstick
        case SCControllerInputNameDPadLeftThumbstick:
            .leftThumbstick
        case SCControllerInputNameDPadRightThumbstick:
            .rightThumbstick
        default:
            nil
        }
    }
}

extension GCDeviceBattery.State {
    func toSCGCDeviceBatteryState() -> SCGCDeviceBatteryState? {
        switch self {
        case .unknown:
            SCGCDeviceBatteryStateUnknown
        case .discharging:
            SCGCDeviceBatteryStateDischarging
        case .charging:
            SCGCDeviceBatteryStateCharging
        case .full:
            SCGCDeviceBatteryStateFull
        default:
            nil
        }
    }
}

extension SCGCDeviceBatteryState {
    func toGCDeviceBatteryState() -> GCDeviceBattery.State? {
        switch self {
        case SCGCDeviceBatteryStateUnknown:
            .unknown
        case SCGCDeviceBatteryStateDischarging:
            .discharging
        case SCGCDeviceBatteryStateCharging:
            .charging
        case SCGCDeviceBatteryStateFull:
            .full
        default:
            nil
        }
    }
}

extension UIImage.SymbolScale {
    func toSCUIImageSymbolScale() -> SCUIImageSymbolScale? {
        switch self {
        case .default:
            SCUIImageSymbolScaleDefault
        case .unspecified:
            SCUIImageSymbolScaleUnspecified
        case .small:
            SCUIImageSymbolScaleSmall
        case .medium:
            SCUIImageSymbolScaleMedium
        case .large:
            SCUIImageSymbolScaleLarge
        default:
            nil
        }
    }
}

extension SCUIImageSymbolScale {
    func toUIImageSymbolScale() -> UIImage.SymbolScale? {
        switch self {
        case SCUIImageSymbolScaleDefault:
            .default
        case SCUIImageSymbolScaleUnspecified:
            .unspecified
        case SCUIImageSymbolScaleSmall:
            .small
        case SCUIImageSymbolScaleMedium:
            .medium
        case SCUIImageSymbolScaleLarge:
            .large
        default:
            nil
        }
    }
}

extension UIImage.RenderingMode {
    func toSCUIImageRenderingMode() -> SCUIImageRenderingMode? {
        switch self {
        case .automatic:
            SCUIImageRenderingModeAutomatic
        case .alwaysOriginal:
            SCUIImageRenderingModeAlwaysOriginal
        case .alwaysTemplate:
            SCUIImageRenderingModeAlwaysTemplate
        default:
            nil
        }
    }
}

extension SCUIImageRenderingMode {
    func toUIImageRenderingMode() -> UIImage.RenderingMode? {
        switch self {
        case SCUIImageRenderingModeAutomatic:
            .automatic
        case SCUIImageRenderingModeAlwaysOriginal:
            .alwaysOriginal
        case SCUIImageRenderingModeAlwaysTemplate:
            .alwaysTemplate
        default:
            nil
        }
    }
}

extension SCHapticsLocality {
    func toGCHapticsLocality() -> GCHapticsLocality? {
        switch self {
        case SCHapticsLocalityDefault:
            .default
        case SCHapticsLocalityAll:
            .all
        case SCHapticsLocalityHandles:
            .handles
        case SCHapticsLocalityLeftHandle:
            .leftHandle
        case SCHapticsLocalityRightHandle:
            .rightHandle
        case SCHapticsLocalityTriggers:
            .triggers
        case SCHapticsLocalityLeftTrigger:
            .leftTrigger
        case SCHapticsLocalityRightTrigger:
            .rightTrigger
        default:
            nil
        }
    }
}

extension GCHapticsLocality {
    func toSCHapticsLocality() -> SCHapticsLocality? {
        switch self {
        case .default:
            SCHapticsLocalityDefault
        case .all:
            SCHapticsLocalityAll
        case .handles:
            SCHapticsLocalityHandles
        case .leftHandle:
            SCHapticsLocalityLeftHandle
        case .rightHandle:
            SCHapticsLocalityRightHandle
        case .triggers:
            SCHapticsLocalityTriggers
        case .leftTrigger:
            SCHapticsLocalityLeftTrigger
        case .rightTrigger:
            SCHapticsLocalityRightTrigger
        default:
            nil
        }
    }
}
