//
//  CHHapticPatternPlayer_Protocol.swift
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import CoreHaptics

protocol CHHapticPatternPlayer_Protocol {
    func Start
    (
        playerPtr:  UnsafeRawPointer,
        startTime: Float,
        onError: ErrorWithPointerCallback
    );
    
    func Stop
    (
        playerPtr:  UnsafeRawPointer,
        stopTime: Float,
        onError: ErrorWithPointerCallback
    );
    
    func Get_IsMuted
    (
        playerPtr:  UnsafeRawPointer
    ) -> Bool;
    
    func Set_IsMuted
    (
        playerPtr:  UnsafeRawPointer,
        isMuted: Bool
    );
    
    func Destroy
    (
        playerPtr:  UnsafeRawPointer
    );
    
    func SendParameters
    (
        playerPtr: UnsafeRawPointer,
        request: CHWSendParametersRequest,
        onError: ErrorWithPointerCallback
    );
    
    func ScheduleParameterCurve
    (
        playerPtr: UnsafeRawPointer,
        curve: CHWHapticParameterCurve,
        atTime: Float,
        onError: ErrorWithPointerCallback
    );

}
