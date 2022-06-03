//
//  ControllerHapticPatterns.swift
//  GameControllerWrapper
//
//  Copyright © 2020 Apple, Inc. All rights reserved.
//


import Foundation
import CoreHaptics
import GameController

@available(iOS 14.0, tvOS 14.0, macOS 10.16, *)
var _controllerEngines = [GCController : CHHapticEngine]();

@_cdecl("GameControllerWrapper_CreateHapticsEngine")
public func GameControllerWrapper_CreateHapticsEngine
(
    uniqueId: char_p,
    onError: ErrorCallback
) -> UnsafeMutableRawPointer?
{
    let controller = _controllerMapping.elements[uniqueId.toString()];
    
    if #available(iOS 14.0, tvOS 14, macOS 10.16, *) {
        if(controller != nil && controller?.haptics != nil) {
            shutdownExistingHapticsEngineForController(controller: controller!);
            
            if let engine = controller!.haptics!.createEngine(withLocality: GCHapticsLocality.default) {
                _controllerEngines.updateValue(engine, forKey: controller!);
                
                let pointer = Unmanaged.passRetained(engine).toOpaque();
                return pointer;
            }
        }
    }
    
    return UnsafeMutableRawPointer?.none;
}

@available(iOS 14.0, tvOS 14.0, macOS 10.16, *)
public func shutdownExistingHapticsEngineForController(controller : GCController)
{
    // Shutdown existing engines...
    if let existingEngine = _controllerEngines[controller] {
        _controllerEngines.removeValue(forKey: controller);
        existingEngine.stop(completionHandler: { error in });
    }
}
