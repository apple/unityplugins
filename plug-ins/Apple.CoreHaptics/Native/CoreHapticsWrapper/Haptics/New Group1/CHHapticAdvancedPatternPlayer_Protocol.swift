//
//  CHHapticAdvancedPatternPlayer_Protocol.swift
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation

protocol CHHapticAdvancedPatternPlayer_Protocol : CHHapticPatternPlayer_Protocol {
    
    func Pause(playerPtr:  UnsafeRawPointer, pauseTime: Float, onError: ErrorWithPointerCallback)
    
    func Resume(playerPtr:  UnsafeRawPointer, resumeTime: Float, onError: ErrorWithPointerCallback)
    
    func Seek(playerPtr:  UnsafeRawPointer, seekTime: Float, onError: ErrorWithPointerCallback)
    
    func Get_LoopEnabled(playerPtr: UnsafeRawPointer) -> Bool;
    
    func Set_LoopEnabled(playerPtr: UnsafeRawPointer, isLoopEnabled: Bool);
    
    func Get_LoopEnd(playerPtr: UnsafeRawPointer) -> Double;
    
    func Set_LoopEnd(playerPtr: UnsafeRawPointer, loopEnd: Double);
    
    func Get_PlaybackRate(playerPtr: UnsafeRawPointer) -> Float;
    
    func Set_PlaybackRate(playerPtr: UnsafeRawPointer, rate: Float);
    
    func RegisterCompletionHandler(playerPtr: UnsafeRawPointer, handler: @escaping SuccessWithPointerCallback, onError: (UnsafeRawPointer, CHWError) -> Void);
}

