//
//  CHHapticEngine_Unsupported.swift
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation

class CHHapticEngine_Unsupported : CHHapticEngine_Protocol {
    func Destroy(enginePtr: UnsafeRawPointer) {
        
    }
    
    func Start(enginePtr: UnsafeRawPointer, onError: ErrorWithPointerCallback) {
        
    }
    
    func Stop(enginePtr: UnsafeRawPointer, onError: ErrorWithPointerCallback) {
        
    }
    
    func PlayPatternFromUrl
    (
        enginePtr: UnsafeRawPointer,
        ahapUrl: char_p,
        onError: @escaping ErrorWithPointerCallback
    ){}
    
    func PlayPatternFromJson
    (
        enginePtr: UnsafeRawPointer,
        ahapJson: char_p,
        onError: @escaping ErrorWithPointerCallback
    ){}
    
    func MakePlayer(enginePtr: UnsafeRawPointer, ahapJson: char_p, onError: @convention(c) (CHWError) -> Void) -> UnsafeMutableRawPointer? {
        return UnsafeMutableRawPointer?.none;
    }
    
    func MakeAdvancedPlayer(enginePtr: UnsafeRawPointer, ahapJson: char_p, onFinishedPlaying: SuccessWithPointerCallback, onError: @convention(c) (CHWError) -> Void) -> UnsafeMutableRawPointer? {
        return UnsafeMutableRawPointer?.none;
    }
    
    func NotifyWhenPlayersFinished(enginePtr: UnsafeRawPointer, leaveEngineRunning: Bool, onAllPlayersFinished: AllPlayersFinishedCallback, onError: ErrorWithPointerCallback) {
        
    }
    
    func HardwareSupportsHaptics() -> Bool {
        return false;
    }
    
    func Get_IsAutoShutdownEnabled(enginePtr: UnsafeRawPointer) -> Bool {
        return false;
    }
    
    func Set_IsAutoShutdownEnabled(enginePtr: UnsafeRawPointer, isAutoShutdownEnabled: Bool) {
        
    }
    
    func Get_PlaysHapticsOnly(enginePtr: UnsafeRawPointer) -> Bool {
        return false;
    }
    
    func Set_PlaysHapticsOnly(enginePtr: UnsafeRawPointer, playsHapticsOnly: Bool) {
        
    }
    
    func Get_IsMutedForAudio(enginePtr: UnsafeRawPointer) -> Bool {
        return false;
    }
    
    func Set_IsMutedForAudio(enginePtr: UnsafeRawPointer, isMutedForAudio: Bool) {

    }
    
    func Get_IsMutedForHaptics(enginePtr: UnsafeRawPointer) -> Bool {
        return false;
    }
    
    func Set_IsMutedForHaptics(enginePtr: UnsafeRawPointer, isMutedForHaptics: Bool) {
        
    }
    
    func Get_CurrentTime(enginePtr: UnsafeRawPointer) -> Double {
        return 0;
    }
    
    func Create(onError: @convention(c) (CHWError) -> Void, onStopped: StoppedWithReasonCallback, onReset: SuccessWithPointerCallback) -> UnsafeMutableRawPointer? {
        return UnsafeMutableRawPointer?.none;
    }
}
