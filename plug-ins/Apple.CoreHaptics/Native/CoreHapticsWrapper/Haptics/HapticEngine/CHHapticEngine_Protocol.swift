//
//  CHHapticEngine_Protocol.swift
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation

protocol CHHapticEngine_Protocol {
    func Create
    (
        onError: ErrorCallback,
        onStopped: @escaping StoppedWithReasonCallback,
        onReset: @escaping SuccessWithPointerCallback
    ) -> UnsafeMutableRawPointer?;
    
    func Destroy
    (
        enginePtr: UnsafeRawPointer
    );
    
    func Start
    (
        enginePtr: UnsafeRawPointer,
        onError: @escaping ErrorWithPointerCallback
    );
    
    func Stop
    (
        enginePtr: UnsafeRawPointer,
        onError : @escaping ErrorWithPointerCallback
    );
    
    func PlayPatternFromUrl
    (
        enginePtr: UnsafeRawPointer,
        ahapUrl: char_p,
        onError: @escaping ErrorWithPointerCallback
    );
    
    func PlayPatternFromJson
    (
        enginePtr: UnsafeRawPointer,
        ahapJson: char_p,
        onError: @escaping ErrorWithPointerCallback
    );
    
    func MakePlayer
    (
        enginePtr: UnsafeRawPointer,
        ahapJson: char_p,
        onError: ErrorCallback
    ) -> UnsafeMutableRawPointer?;
    
    func MakeAdvancedPlayer
    (
        enginePtr: UnsafeRawPointer,
        ahapJson: char_p,
        onFinishedPlaying: @escaping SuccessWithPointerCallback,
        onError: ErrorCallback
    ) -> UnsafeMutableRawPointer?;
    
    func NotifyWhenPlayersFinished
    (
        enginePtr: UnsafeRawPointer, leaveEngineRunning: Bool,
        onAllPlayersFinished: @escaping AllPlayersFinishedCallback,
        onError: @escaping ErrorWithPointerCallback
    );
    
    func HardwareSupportsHaptics() -> Bool;
    
    func Get_IsAutoShutdownEnabled
    (
        enginePtr: UnsafeRawPointer
    ) -> Bool;
    
    func Set_IsAutoShutdownEnabled
    (
        enginePtr: UnsafeRawPointer,
        isAutoShutdownEnabled: Bool
    );
    
    func Get_PlaysHapticsOnly
    (
        enginePtr: UnsafeRawPointer
    ) -> Bool;
    
    func Set_PlaysHapticsOnly
    (
        enginePtr: UnsafeRawPointer,
        playsHapticsOnly: Bool
    )
    
    func Get_IsMutedForAudio
    (
        enginePtr: UnsafeRawPointer
    ) -> Bool;
    
    func Set_IsMutedForAudio
    (
        enginePtr: UnsafeRawPointer,
        isMutedForAudio: Bool
    );
    
    func Get_IsMutedForHaptics
    (
        enginePtr: UnsafeRawPointer
    ) -> Bool;
    
    func Set_IsMutedForHaptics
    (
        enginePtr: UnsafeRawPointer,
        isMutedForHaptics: Bool
    );
    
    func Get_CurrentTime
    (
        enginePtr: UnsafeRawPointer
    ) -> Double;
}
