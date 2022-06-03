//
//  CHHapticPatternPlayer_Unsupported.swift
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation

class CHHapticPatternPlayer_Unsupported : CHHapticPatternPlayer_Protocol {
    
    func Start(playerPtr: UnsafeRawPointer, startTime: Float, onError: ErrorWithPointerCallback) {
    }
    
    func Stop(playerPtr: UnsafeRawPointer, stopTime: Float, onError: ErrorWithPointerCallback) {
    }
    
    func SendParameters(playerPtr: UnsafeRawPointer, request: CHWSendParametersRequest, onError: ErrorWithPointerCallback) {
    }
    
    func ScheduleParameterCurve(playerPtr: UnsafeRawPointer, curve: CHWHapticParameterCurve, atTime: Float, onError: ErrorWithPointerCallback) {
    }
    
    func Get_IsMuted(playerPtr: UnsafeRawPointer) -> Bool {
        return false;
    }
    
    func Set_IsMuted(playerPtr: UnsafeRawPointer, isMuted: Bool) {
    }
    
    func Destroy(playerPtr: UnsafeRawPointer) {
    }
}
