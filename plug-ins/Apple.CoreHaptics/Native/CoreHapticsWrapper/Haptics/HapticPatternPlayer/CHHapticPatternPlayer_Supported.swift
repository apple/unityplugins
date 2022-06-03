//
//  CHHapticPatternPlayer_Supported.swift
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import CoreHaptics

@available(iOS 13, tvOS 14, macOS 11, *)
class CHHapticPatternPlayer_Supported : CHHapticPatternPlayer_Protocol {
    
    func Start(playerPtr: UnsafeRawPointer, startTime: Float, onError: ErrorWithPointerCallback) {
        let player = Unmanaged<CHHapticPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        
        do {
            let timeToStart = startTime == 0 ? CHHapticTimeImmediate : TimeInterval(startTime)
            try player.start(atTime: timeToStart);
        }
        catch {
            onError(playerPtr, error.toCHWError());
        }
    }
    
    func Stop(playerPtr: UnsafeRawPointer, stopTime: Float, onError: ErrorWithPointerCallback) {
        let player = Unmanaged<CHHapticPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        
        do {
            let timeToStop = stopTime == 0 ? CHHapticTimeImmediate : TimeInterval(stopTime)
            try player.stop(atTime: timeToStop);
        }
        catch {
            onError(playerPtr, error.toCHWError());
        }
    }
    
    func Get_IsMuted(playerPtr: UnsafeRawPointer) -> Bool {
        let player = Unmanaged<CHHapticPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        return player.isMuted;
    }
    
    func Set_IsMuted(playerPtr: UnsafeRawPointer, isMuted: Bool) {
        let player = Unmanaged<CHHapticPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        player.isMuted = isMuted;
    }
    
    func Destroy(playerPtr: UnsafeRawPointer) {
        _ = Unmanaged<CHHapticPatternPlayer>.fromOpaque(playerPtr).autorelease();
    }
    
    func SendParameters(playerPtr: UnsafeRawPointer, request: CHWSendParametersRequest, onError: ErrorWithPointerCallback) {
        let player = Unmanaged<CHHapticPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
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
        let player = Unmanaged<CHHapticPatternPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
        
        let timeToSend: TimeInterval = atTime == 0 ? CHHapticTimeImmediate : TimeInterval(atTime)
        do {
            try player.scheduleParameterCurve(curve.toCHHapticParameterCurve(), atTime: timeToSend);
        } catch {
            onError(playerPtr, error.toCHWError());
        }
    }

}
