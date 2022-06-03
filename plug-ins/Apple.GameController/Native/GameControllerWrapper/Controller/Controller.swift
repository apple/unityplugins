//
//  Controller.swift
//  GameControllerWrapper
//
//  Copyright © 2020 Apple, Inc. All rights reserved.
//

import Foundation
import CoreGraphics
import CoreHaptics
import GameController

var _controllerMapping = ControllerDictionary();

// Notifications...
var _notificationHandler : GCWNotificationHandler? = nil;
var _onConnectedCallback : GCWControllerConnectionStateChangedCallback? = nil;
var _onDisconnectedcallback : GCWControllerConnectionStateChangedCallback? = nil;

let _emptyString : char_p = "".toCharPCopy();

@_cdecl("GameControllerWrapper_StartWirelessDiscovery")
public func GameControllerWrapper_StartWirelessDiscovery
(
    callback : @escaping SuccessCallback
)
{
    GCController.startWirelessControllerDiscovery {
        callback();
    };
}

@_cdecl("GameControllerWrapper_StopWirelessDiscovery")
public func GameControllerWrapper_StopWirelessDiscovery()
{
    GCController.stopWirelessControllerDiscovery();
}

@_cdecl("GameControllerWrapper_SetConnectionHandlers")
public func GameControllerWrapper_SetConnectionHandlers
(
    onConnected : @escaping GCWControllerConnectionStateChangedCallback,
    onDisconnected : @escaping GCWControllerConnectionStateChangedCallback
)
{
    _onConnectedCallback = onConnected;
    _onDisconnectedcallback = onDisconnected;
    
    _notificationHandler = GCWNotificationHandler();
    _notificationHandler?.setupHandlers();
}

fileprivate func _checkForUnregisteredControllers() {
    // Check for non registered controllers...
    for controller in GCController.controllers() {
        let key = _controllerMapping.elements.someKey(forValue: controller);
        
        // Register controller...
        if key == nil {
            let uuid = UUID().uuidString;
            _controllerMapping.updateValue(controller, forKey: uuid);
        }
    }
}

@_cdecl("GameControllerWrapper_GetConnectedControllers")
public func GameControllerWrapper_GetConnectedControllers() -> GCWGetConnectedControllersResponse
{
    _checkForUnregisteredControllers();
    
    var controllers : [GCWController] = [];
        
    // Generate controller from map...
    for (key, value) in _controllerMapping.elements {
        controllers.append(value.toGCWController(uid: key));
    }
    
    return GCWGetConnectedControllersResponse(
        controllers: controllers.toUnsafeMutablePointer(),
        controllerCount: Int32(controllers.count));
}

@_cdecl("GameControllerWrapper_GetSymbolForInputName")
public func GameControllerWrapper_GetSymbolForInputName
(
    uniqueId : char_p,
    inputName : GCWControllerInputName,
    symbolSize : Int,
    renderingMode : Int
) -> GCWGetSymbolForInputNameResponse
{
    if #available(macOS 10.16, tvOS 14, iOS 14, *) {
        let controller = _controllerMapping.elements[uniqueId.toString()];
        
        // Extended...
        if controller != nil && controller!.extendedGamepad != nil {
            if let symbolName = _getSymbolNameForExtendedInput(inputName: inputName, profile: controller!.extendedGamepad!) {
                #if os(macOS)
                    let config = NSImage.SymbolConfiguration(scale: NSImage.SymbolScale(rawValue: symbolSize)!);
                    let image = NSImage.init(systemSymbolName: symbolName, accessibilityDescription: nil)!.withSymbolConfiguration(config);
                #else
                    let config = UIImage.SymbolConfiguration(scale: UIImage.SymbolScale(rawValue: symbolSize)!);
                    let image = UIImage(systemName: symbolName, withConfiguration: config)?.withRenderingMode(UIImage.RenderingMode.init(rawValue: renderingMode)!);
                #endif
                if image != nil, let data = image!.pngData() {
                    return GCWGetSymbolForInputNameResponse(
                        width: Int32(image!.size.width),
                        height: Int32(image!.size.height),
                        data: data.toUCharP(),
                        dataLength: Int32(data.count));
                }
            }
        }
        
        // MicroGamepad...
        if controller != nil && controller!.microGamepad != nil {
            if let symbolName = _getSymbolNameForMicroInput(inputName: inputName, profile: controller!.microGamepad!) {
                #if os(macOS)
                    let config = NSImage.SymbolConfiguration(scale: NSImage.SymbolScale(rawValue: symbolSize)!);
                    let image = NSImage.init(systemSymbolName: symbolName, accessibilityDescription: nil)!.withSymbolConfiguration(config);
                #else
                    let config = UIImage.SymbolConfiguration(scale: UIImage.SymbolScale(rawValue: symbolSize)!);
                    let image = UIImage(systemName: symbolName, withConfiguration: config)?.withRenderingMode(UIImage.RenderingMode.init(rawValue: renderingMode)!);
                #endif
                if image != nil, let data = image!.pngData() {
                    return GCWGetSymbolForInputNameResponse(
                        width: Int32(image!.size.width),
                        height: Int32(image!.size.height),
                        data: data.toUCharP(),
                        dataLength: Int32(data.count));
                }
            }
        }
    }
    
    return GCWGetSymbolForInputNameResponse();
}

@_cdecl("GameControllerWrapper_SetControllerLightColor")
public func GameControllerWrapper_SetControllerLightColor
(
    uniqueId : char_p,
    r : Float,
    g : Float,
    b : Float
)
{
    if #available(macOS 10.16, iOS 14, tvOS 14, *) {
        let controller = _controllerMapping.elements[uniqueId.toString()];
        if (controller != nil) {
            if let light = controller?.light {
                light.color = GCColor(red:r, green:g, blue:b);
            }
        }
    }
}

@available(OSX 11.00, tvOS 14, iOS 14.0, *)
fileprivate func _getSymbolNameForExtendedInput(inputName : GCWControllerInputName, profile : GCExtendedGamepad) -> String? {
    
    switch inputName {
    case ButtonHome:
        return profile.buttonHome?.sfSymbolsName;
    case ButtonMenu:
        return profile.buttonMenu.sfSymbolsName;
    case ButtonOptions:
        return profile.buttonOptions?.sfSymbolsName;
    case ButtonA:
        return profile.buttonA.sfSymbolsName;
    case ButtonB:
        return profile.buttonB.sfSymbolsName;
    case ButtonY:
        return profile.buttonY.sfSymbolsName;
    case ButtonX:
        return profile.buttonX.sfSymbolsName;
    case ShoulderRightFront:
        return profile.rightShoulder.sfSymbolsName;
    case ShoulderRightBack:
        return profile.rightTrigger.sfSymbolsName;
    case ShoulderLeftFront:
        return profile.leftShoulder.sfSymbolsName;
    case ShoulderLeftBack:
        return profile.leftTrigger.sfSymbolsName;
    case DpadHorizontal:
        return profile.dpad.sfSymbolsName;
    case DpadVertical:
        return profile.dpad.sfSymbolsName;
    case ThumbstickLeftHorizontal:
        return profile.leftThumbstick.sfSymbolsName;
    case ThumbstickLeftVertical:
        return profile.leftThumbstick.sfSymbolsName;
    case ThumbstickLeftButton:
        return profile.leftThumbstickButton?.sfSymbolsName;
    case ThumbstickRightHorizontal:
        return profile.rightThumbstick.sfSymbolsName;
    case ThumbstickRightVertical:
        return profile.rightThumbstick.sfSymbolsName;
    case ThumbstickRightButton:
        return profile.rightThumbstickButton?.sfSymbolsName;
    default:
        break;
    }
    
    // Check dualshock special names...
    if let dualshockProfile = profile as? GCDualShockGamepad {
        switch inputName {
        case TouchpadButton:
            return dualshockProfile.touchpadButton.sfSymbolsName;
        case TouchpadPrimaryHorizontal:
            return dualshockProfile.touchpadPrimary.sfSymbolsName;
        case TouchpadPrimaryVertical:
            return dualshockProfile.touchpadPrimary.sfSymbolsName;
        case TouchpadSecondaryHorizontal:
            return dualshockProfile.touchpadPrimary.sfSymbolsName;
        case TouchpadSecondaryVertical:
            return dualshockProfile.touchpadPrimary.sfSymbolsName;
        default:
            break;
        }
    }
    
    // DualSense
    if #available(macOS 11.3, iOS 14.5, tvOS 14.5, *) {
        if let dualSenseProfile = profile as? GCDualSenseGamepad {
            switch inputName {
            case TouchpadButton:
                return dualSenseProfile.touchpadButton.sfSymbolsName;
            case TouchpadPrimaryHorizontal:
                return dualSenseProfile.touchpadPrimary.sfSymbolsName;
            case TouchpadPrimaryVertical:
                return dualSenseProfile.touchpadPrimary.sfSymbolsName;
            case TouchpadSecondaryHorizontal:
                return dualSenseProfile.touchpadPrimary.sfSymbolsName;
            case TouchpadSecondaryVertical:
                return dualSenseProfile.touchpadPrimary.sfSymbolsName;
            default:
                break;
            }

        }
    }
    
    return nil;
}

@available(OSX 11.00, tvOS 14, iOS 14.0, *)
fileprivate func _getSymbolNameForMicroInput(inputName : GCWControllerInputName, profile : GCMicroGamepad) -> String? {
    
    switch inputName {
    case ButtonMenu:
        return profile.buttonMenu.sfSymbolsName;
    case ButtonA:
        return profile.buttonA.sfSymbolsName;
    case ButtonX:
        return profile.buttonX.sfSymbolsName;
    case DpadHorizontal:
        return profile.dpad.xAxis.sfSymbolsName;
    case DpadVertical:
        return profile.dpad.yAxis.sfSymbolsName;
    default:
        return nil;
    }
}

@_cdecl("GameControllerWrapper_PollController")
public func GameControllerWrapper_PollController
(
    uniqueId : char_p
) -> GCWControllerInputState
{
    let controller = _controllerMapping.elements[uniqueId.toString()];
    var state = GCWControllerInputState();
    
    // Micro...
    if(controller != nil && controller?.microGamepad != nil) {
        _pollMicroController(controller: controller!, profile: controller!.microGamepad!, state: &state);
    }
    
    // Extended...
    if(controller != nil && controller?.extendedGamepad != nil) {
        _pollExtendedController(controller: controller!, profile: controller!.extendedGamepad!, state: &state);
    }
    
    return state;
}

fileprivate func _pollExtendedController
(
    controller : GCController,
    profile : GCExtendedGamepad,
    state : inout GCWControllerInputState)
{
    state.buttonMenu = profile.buttonMenu.value;
    state.buttonOptions = profile.buttonOptions?.value ?? 0;
    state.buttonA = profile.buttonA.value;
    state.buttonB = profile.buttonB.value;
    state.buttonY = profile.buttonY.value;
    state.buttonX = profile.buttonX.value;
    state.shoulderRightFront = profile.rightShoulder.value;
    state.shoulderRightBack = profile.rightTrigger.value;
    state.shoulderLeftFront = profile.leftShoulder.value;
    state.shoulderLeftBack = profile.leftTrigger.value;
    state.dpadHorizontal = profile.dpad.xAxis.value;
    state.dpadVertical = profile.dpad.yAxis.value;
    state.thumbstickLeftHorizontal = profile.leftThumbstick.xAxis.value;
    state.thumbstickLeftVertical = profile.leftThumbstick.yAxis.value;
    state.thumbstickLeftButton = profile.leftThumbstickButton?.value ?? 0;
    state.thumbstickRightHorizontal = profile.rightThumbstick.xAxis.value;
    state.thumbstickRightVertical = profile.rightThumbstick.yAxis.value;
    state.thumbstickRightButton = profile.rightThumbstickButton?.value ?? 0;
    state.batteryLevel = 0;
    state.batteryState = -1;
    
    if #available(iOS 14, macOS 10.16, tvOS 14, *) {
        if let dualshockProfile = profile as? GCDualShockGamepad {
            _pollDualshockController(controller: controller, profile: dualshockProfile, state: &state);
        }
    }
    
    if #available(macOS 11.3, iOS 14.5, tvOS 14.5, *) {
        if let dualSenseProfile = profile as? GCDualSenseGamepad {
            _pollDualSenseController(controller: controller, profile: dualSenseProfile, state: &state);
        }
    }
}

@available(macOS 11.3, iOS 14.5, tvOS 14.5, *)
fileprivate func _pollDualSenseController
(
    controller : GCController,
    profile : GCDualSenseGamepad,
    state : inout GCWControllerInputState
)
{
    state.touchpadButton              = profile.touchpadButton.value;
    state.touchpadPrimaryHorizontal   = profile.touchpadPrimary.xAxis.value;
    state.touchpadPrimaryVertical     = profile.touchpadPrimary.yAxis.value;
    state.touchpadSecondaryHorizontal = profile.touchpadSecondary.xAxis.value;
    state.touchpadSecondaryVertical   = profile.touchpadSecondary.yAxis.value;
}

@available(iOS 14, macOS 10.16, tvOS 14, *)
fileprivate func _pollDualshockController
(
    controller : GCController,
    profile : GCDualShockGamepad,
    state : inout GCWControllerInputState
)
{
    state.touchpadButton = profile.touchpadButton.value;
    state.touchpadPrimaryHorizontal = profile.touchpadPrimary.xAxis.value;
    state.touchpadPrimaryVertical = profile.touchpadPrimary.yAxis.value;
    state.touchpadSecondaryHorizontal = profile.touchpadPrimary.xAxis.value;
    state.touchpadSecondaryVertical = profile.touchpadPrimary.yAxis.value;
}

fileprivate func _pollMicroController
(
    controller : GCController,
    profile : GCMicroGamepad,
    state : inout GCWControllerInputState)
{

    state.buttonMenu = profile.buttonMenu.isPressed.toFloat();
    state.buttonA = profile.buttonA.isPressed.toFloat();
    state.buttonX = profile.buttonX.isPressed.toFloat();
    state.dpadHorizontal = profile.dpad.xAxis.value;
    state.dpadVertical = profile.dpad.yAxis.value;
}

class GCWNotificationHandler : NSObject  {
    public func setupHandlers() {
        // Add GCControllerDidConnect....
        NotificationCenter.default.addObserver(
            self,
            selector: #selector(onControllerConnected(notification:)),
            name: NSNotification.Name.GCControllerDidConnect,
            object: .none
        );
        
        // Add GCCOntrollerDidDisconnect...
        NotificationCenter.default.addObserver(
            self,
            selector: #selector(onControllerDisconnected(notification:)),
            name: NSNotification.Name.GCControllerDidDisconnect,
            object: .none
        );
    }
    
    @objc
    public func onControllerConnected(notification : NSNotification) {
        let controller = notification.object as! GCController;
        let uuid = _controllerMapping.elements.someKey(forValue: controller) ?? UUID().uuidString;
    
        _controllerMapping.updateValue(controller, forKey: uuid);
        _onConnectedCallback?(controller.toGCWController(uid: uuid));
    }
    
    @objc
    public func onControllerDisconnected(notification : NSNotification) {
        let controller = notification.object as! GCController;
        let uuid = _controllerMapping.elements.someKey(forValue: controller);
        
        if uuid != nil {
            _controllerMapping.removeValue(forKey: uuid!);
            _onDisconnectedcallback?(controller.toGCWController(uid: uuid!));
            
            // Remove any haptic engines for controller...
            if #available(iOS 14, tvOS 14.0, macOS 10.16, *) {
                shutdownExistingHapticsEngineForController(controller: controller);
            }
        }
    }
}
