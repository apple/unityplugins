//
//  CHHapticEngine_Supported.swift
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import CoreHaptics

@available(iOS 13, tvOS 14, macOS 11, *)
class CHHapticEngine_Supported : CHHapticEngine_Protocol {
    public func Create(onError: ErrorCallback, onStopped: @escaping StoppedWithReasonCallback, onReset: @escaping SuccessWithPointerCallback) -> UnsafeMutableRawPointer? {
        do {
            let engine = try CHHapticEngine();
            let pointer = Unmanaged.passRetained(engine).toOpaque();
            
            engine.stoppedHandler = { reason in
                onStopped(pointer, Int32(reason.rawValue));
            }
            
            engine.resetHandler = {
                onReset(pointer);
            };
            
            return pointer;
        } catch {
            onError(error.toCHWError());
            return UnsafeMutableRawPointer?.none;
        }
    }
    
    public func Destroy(enginePtr: UnsafeRawPointer) {
        _ = Unmanaged<CHHapticPatternPlayer>.fromOpaque(enginePtr).autorelease();
    }
    
    public func Start(enginePtr: UnsafeRawPointer, onError: @escaping ErrorWithPointerCallback) {
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        
        do {
            try engine.start();
        } catch {
            onError(enginePtr, error.toCHWError());
        }
    }
    
    public func Stop(enginePtr: UnsafeRawPointer, onError : @escaping ErrorWithPointerCallback) {
        let pointer = enginePtr;
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        
        engine.stop(completionHandler: { error in
           if(error != nil) {
                onError(pointer, error!.toCHWError());
                return;
           }
        });
    }
    
    func PlayPatternFromUrl(enginePtr: UnsafeRawPointer, ahapUrl: char_p, onError: @escaping ErrorWithPointerCallback) {
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        
        do {
            try engine.playPattern(from: URL(fileURLWithPath: ahapUrl.toString()));
        } catch {
            NSLog("Error occurred playing pattern from URL: \(error)")
            onError(enginePtr, error.toCHWError());
        }
    }
    
    func PlayPatternFromJson(enginePtr: UnsafeRawPointer, ahapJson: char_p, onError: @escaping ErrorWithPointerCallback) {
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        
        do {
            if let data = ahapJson.toString().data(using: .utf8) {
                try engine.playPattern(from: data);
            }
        } catch {
            NSLog("Error occurred playing JSON one-shot: \(error)")
            onError(enginePtr, error.toCHWError());
        }
    }
    
    public func MakePlayer(enginePtr: UnsafeRawPointer, ahapJson: char_p, onError: ErrorCallback ) -> UnsafeMutableRawPointer? {
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        
        if let data = ahapJson.toString().data(using: .utf8) {
            do {
                // Deserialize...
                let patternDictionary = try JSONSerialization.jsonObject(with: data, options: .mutableContainers) as! NSMutableDictionary;
                
                // Create pattern...
                let patternKey: [CHHapticPattern.Key : Any]? = patternDictionary as? [CHHapticPattern.Key : Any]
                if (patternKey == nil) {
                    return UnsafeMutableRawPointer?.none;
                }
                let hapticPattern = try CHHapticPattern(dictionary: patternKey!);
                
                // Create player...
                let player = try engine.makePlayer(with: hapticPattern);
                let pointer = Unmanaged.passRetained(player).toOpaque();
                return pointer;
            } catch {
                onError(error.toCHWError());
            }
        }
        
        return UnsafeMutableRawPointer?.none;
    }
    
    public func MakeAdvancedPlayer
    (
        enginePtr: UnsafeRawPointer,
        ahapJson: char_p,
        onFinishedPlaying: @escaping SuccessWithPointerCallback,
        onError: ErrorCallback
    ) -> UnsafeMutableRawPointer?
    {
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        
        if let data = ahapJson.toString().data(using: .utf8) {
            do {
                // Deserialize...
                let patternDictionary = try JSONSerialization.jsonObject(with: data, options: .mutableContainers) as! NSMutableDictionary;
                
                // Create pattern...
                let patternKey: [CHHapticPattern.Key : Any]? = patternDictionary as? [CHHapticPattern.Key : Any]
                if (patternKey == nil) {
                    return UnsafeMutableRawPointer?.none;
                }
                let hapticPattern = try CHHapticPattern(dictionary: patternKey!);
                
                // Create player...
                let player = try engine.makeAdvancedPlayer(with: hapticPattern);
                let pointer = Unmanaged.passRetained(player).toOpaque();
                
                player.completionHandler = { error in
                    if(error != nil) {
                        // TODO: Do we want to pass through?
                    }
                    
                    onFinishedPlaying(pointer);
                };
                
                return pointer;
            } catch {
                onError(error.toCHWError());
            }
        }
        
        return UnsafeMutableRawPointer?.none;
    }
    
    public func NotifyWhenPlayersFinished(enginePtr: UnsafeRawPointer, leaveEngineRunning: Bool, onAllPlayersFinished: @escaping AllPlayersFinishedCallback, onError: @escaping ErrorWithPointerCallback) {
        let pointer = enginePtr;
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        
        engine.notifyWhenPlayersFinished(finishedHandler: { error in
            if(error != nil) {
                onError(pointer, error!.toCHWError());
            }
            let _ = onAllPlayersFinished(pointer);
            return leaveEngineRunning ? CHHapticEngine.FinishedAction.leaveEngineRunning : CHHapticEngine.FinishedAction.stopEngine;
        });
    }
    
    public func HardwareSupportsHaptics() -> Bool {
        let hapticCapability = CHHapticEngine.capabilitiesForHardware()
        return hapticCapability.supportsHaptics;
    }
    
    public func Get_IsAutoShutdownEnabled(enginePtr: UnsafeRawPointer) -> Bool {
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        return engine.isAutoShutdownEnabled;
    }
    
    public func Set_IsAutoShutdownEnabled(enginePtr: UnsafeRawPointer, isAutoShutdownEnabled: Bool) {
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        engine.isAutoShutdownEnabled = isAutoShutdownEnabled;
    }
    
    public func Get_PlaysHapticsOnly(enginePtr: UnsafeRawPointer) -> Bool {
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        return engine.playsHapticsOnly;
    }
    
    public func Set_PlaysHapticsOnly(enginePtr: UnsafeRawPointer, playsHapticsOnly: Bool) {
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        engine.playsHapticsOnly = playsHapticsOnly;
    }
    
    public func Get_IsMutedForAudio(enginePtr: UnsafeRawPointer) -> Bool {
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        return engine.isMutedForAudio;
    }
    
    public func Set_IsMutedForAudio(enginePtr: UnsafeRawPointer, isMutedForAudio: Bool) {
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        engine.isMutedForAudio = isMutedForAudio;
    }
    
    public func Get_IsMutedForHaptics(enginePtr: UnsafeRawPointer) -> Bool {
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        return engine.isMutedForHaptics;
    }
    
    public func Set_IsMutedForHaptics (enginePtr: UnsafeRawPointer, isMutedForHaptics: Bool) {
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        engine.isMutedForHaptics = isMutedForHaptics;
    }
    
    public func Get_CurrentTime(enginePtr: UnsafeRawPointer) -> Double {
        let engine = Unmanaged<CHHapticEngine>.fromOpaque(enginePtr).takeUnretainedValue();
        return engine.currentTime;
    }
}
