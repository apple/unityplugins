//
//  CHHapticAdvancedPatternPlayer_Unsupported.swift
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation

class CHHapticAdvancedPatternPlayer_Unsupported : CHHapticAdvancedPatternPlayer_Protocol {
    
    func Pause(playerPtr: UnsafeRawPointer, pauseTime: Float, onError: ErrorWithPointerCallback) {
        
    }
    
    func Resume(playerPtr: UnsafeRawPointer, resumeTime: Float, onError: ErrorWithPointerCallback) {
        
    }
    
    func Seek(playerPtr: UnsafeRawPointer, seekTime: Float, onError: ErrorWithPointerCallback) {
        
    }
    
    func Get_LoopEnabled(playerPtr: UnsafeRawPointer) -> Bool {
        return false;
    }
    
    func Set_LoopEnabled(playerPtr: UnsafeRawPointer, isLoopEnabled: Bool) {
        
    }
    
    func Get_LoopEnd(playerPtr: UnsafeRawPointer) -> Double {
        return 0;
    }
    
    func Set_LoopEnd(playerPtr: UnsafeRawPointer, loopEnd: Double) {
        
    }
    
    func Get_PlaybackRate(playerPtr: UnsafeRawPointer) -> Float {
        return 0;
    }
    
    func Set_PlaybackRate(playerPtr: UnsafeRawPointer, rate: Float) {
    
    }
    
    func Start(playerPtr: UnsafeRawPointer, startTime: Float, onError: ErrorWithPointerCallback) {
    }
    
    func Stop(playerPtr: UnsafeRawPointer, stopTime: Float, onError: ErrorWithPointerCallback) {
        
    }
    
    func Get_IsMuted(playerPtr: UnsafeRawPointer) -> Bool {
        return false;
    }
    
    func Set_IsMuted(playerPtr: UnsafeRawPointer, isMuted: Bool) {
    }
    
    func Destroy(playerPtr: UnsafeRawPointer) {
    }
    
    func SendParameters(playerPtr: UnsafeRawPointer, request: CHWSendParametersRequest, onError: @convention(c) (UnsafeRawPointer, CHWError) -> Void) {
    }
    
    func ScheduleParameterCurve(playerPtr: UnsafeRawPointer, curve: CHWHapticParameterCurve, atTime: Float, onError: ErrorWithPointerCallback) {
    }
    
    func RegisterCompletionHandler(playerPtr: UnsafeRawPointer, handler: SuccessWithPointerCallback, onError: (UnsafeRawPointer, CHWError) -> Void) {
    }
}
