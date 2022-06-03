//
//  CHHapticAdvancedPatternPlayer_Supported.swift
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import CoreHaptics

@available(iOS 13, tvOS 14, macOS 11, *)
class CHHapticAdvancedPatternPlayer_Supported : CHHapticAdvancedPatternPlayer_Protocol {
    
    func Pause(playerPtr: UnsafeRawPointer, pauseTime: Float, onError: ErrorWithPointerCallback) {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        
        do {
            let timeToPause = pauseTime == 0 ? CHHapticTimeImmediate : TimeInterval(pauseTime)
            try player.pause(atTime: timeToPause);
        }
        catch {
            onError(playerPtr, error.toCHWError());
        }
    }
    
    func Resume(playerPtr: UnsafeRawPointer, resumeTime: Float, onError: ErrorWithPointerCallback) {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        
        do {
            let timeToResume = resumeTime == 0 ? CHHapticTimeImmediate : TimeInterval(resumeTime)
            try player.resume(atTime: timeToResume);
        }
        catch {
            onError(playerPtr, error.toCHWError());
        }
    }
    
    func Seek(playerPtr: UnsafeRawPointer, seekTime: Float, onError: ErrorWithPointerCallback) {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        
        do {
            try player.seek(toOffset: TimeInterval(seekTime));
        }
        catch {
            onError(playerPtr, error.toCHWError());
        }
    }
    
    func Get_LoopEnabled(playerPtr: UnsafeRawPointer) -> Bool {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        return player.loopEnabled;
    }
    
    func Set_LoopEnabled(playerPtr: UnsafeRawPointer, isLoopEnabled: Bool) {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        player.loopEnabled = isLoopEnabled;
    }
    
    func Get_LoopEnd(playerPtr: UnsafeRawPointer) -> Double {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        return player.loopEnd;
    }
    
    func Set_LoopEnd(playerPtr: UnsafeRawPointer, loopEnd: Double) {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        player.loopEnd = loopEnd;
    }
    
    func Get_PlaybackRate(playerPtr: UnsafeRawPointer) -> Float {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        return player.playbackRate;
    }
    
    func Set_PlaybackRate(playerPtr: UnsafeRawPointer, rate: Float) {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        player.playbackRate = rate;
    }
    
    func Get_IsMuted(playerPtr: UnsafeRawPointer) -> Bool {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        return player.isMuted;
    }
    
    func Set_IsMuted(playerPtr: UnsafeRawPointer, isMuted: Bool) {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        player.isMuted = isMuted;
    }
    
    func Start(playerPtr: UnsafeRawPointer, startTime: Float, onError: ErrorWithPointerCallback) {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        
        do {
            let timeToStart = startTime == 0 ? CHHapticTimeImmediate : TimeInterval(startTime)
            try player.start(atTime: timeToStart);
        }
        catch {
            onError(playerPtr, error.toCHWError());
        }
    }
    
    func Stop(playerPtr: UnsafeRawPointer, stopTime: Float, onError: ErrorWithPointerCallback) {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        
        do {
            let timeToStop = stopTime == 0 ? CHHapticTimeImmediate : TimeInterval(stopTime)
            try player.stop(atTime: timeToStop);
        }
        catch {
            onError(playerPtr, error.toCHWError());
        }
    }
    
    func Destroy(playerPtr: UnsafeRawPointer) {
        _ = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).autorelease();
    }
    
    func SendParameters(playerPtr: UnsafeRawPointer, request: CHWSendParametersRequest, onError: ErrorWithPointerCallback) {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        let dynamicParameters = request.parameters.toArray(length: Int(request.parametersLength));
        
        // Generate CHDynamicParameters...
        var parameters = [CHHapticDynamicParameter]();
        for p in dynamicParameters {
            let newParam = CHHapticDynamicParameter(parameterID: dynamicParameterForInt(Int(p.parameterID)), value: p.value, relativeTime: TimeInterval(p.relativeTime))
            parameters.append(newParam);
        }
        
        // Send...
        let timeToSend: TimeInterval = request.atTime == 0 ? CHHapticTimeImmediate : TimeInterval(request.atTime)
        do {
            try player.sendParameters(parameters, atTime: timeToSend);
        } catch {
            onError(playerPtr, error.toCHWError());
        }
    }
    
    func ScheduleParameterCurve(playerPtr: UnsafeRawPointer, curve: CHWHapticParameterCurve, atTime: Float, onError: ErrorWithPointerCallback)
    {
    }

    
    func RegisterCompletionHandler(playerPtr: UnsafeRawPointer, handler: @escaping SuccessWithPointerCallback, onError: (UnsafeRawPointer, CHWError) -> Void)
    {
        let player = Unmanaged<CHHapticAdvancedPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        
        player.completionHandler = { [handler] error in
            if(error != nil) {
                //
            }
            
            handler(playerPtr)
        };
    }
}
