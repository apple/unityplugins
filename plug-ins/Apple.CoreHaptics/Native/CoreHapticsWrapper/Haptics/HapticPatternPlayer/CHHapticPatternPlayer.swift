//
//  CHHapticPatternPlayer.swift
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import CoreHaptics

fileprivate var _playerImplementation : CHHapticPatternPlayer_Protocol? = nil;

fileprivate func _instance() -> CHHapticPatternPlayer_Protocol {
    if _playerImplementation == nil {
        if #available(iOS 13, tvOS 14, macOS 11, *) {
            _playerImplementation = CHHapticPatternPlayer_Supported();
        } else {
            _playerImplementation = CHHapticPatternPlayer_Unsupported();
        }
    }
    
    return _playerImplementation!;
}

@_cdecl("CoreHaptics_CHHapticPatternPlayer_Start")
public func CoreHaptics_CHHapticPatternPlayer_Start
(
    playerPtr:  UnsafeRawPointer,
    startTime: Float,
    onError: ErrorWithPointerCallback
)
{
    _instance().Start(playerPtr: playerPtr, startTime: startTime, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticPatternPlayer_Stop")
public func CoreHaptics_CHHapticPatternPlayer_Stop
(
    playerPtr:  UnsafeRawPointer,
    stopTime: Float,
    onError: ErrorWithPointerCallback
)
{
    _instance().Stop(playerPtr: playerPtr, stopTime: stopTime, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticPatternPlayer_GetIsMuted")
public func CoreHaptics_CHHapticPatternPlayer_GetIsMuted
(
    playerPtr:  UnsafeRawPointer
) -> Bool
{
    return _instance().Get_IsMuted(playerPtr: playerPtr);
}

@_cdecl("CoreHaptics_CHHapticPatternPlayer_SetIsMuted")
public func CoreHaptics_CHHapticPatternPlayer_SetIsMuted
(
    playerPtr:  UnsafeRawPointer,
    isMuted: Bool
)
{
    _instance().Set_IsMuted(playerPtr: playerPtr, isMuted: isMuted);
}

@_cdecl("CoreHaptics_CHHapticPatternPlayer_Destroy")
public func CoreHaptics_CHHapticPatternPlayer_Destroy
(
    playerPtr:  UnsafeRawPointer
)
{
    _instance().Destroy(playerPtr: playerPtr);
}

@_cdecl("CoreHaptics_CHHapticPatternPlayer_SendParameters")
public func CoreHaptics_CHHapticPatternPlayer_SendParameters
(
    playerPtr: UnsafeRawPointer,
    request: CHWSendParametersRequest,
    onError: ErrorWithPointerCallback
)
{
    _instance().SendParameters(playerPtr: playerPtr, request: request, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticPatternPlayer_ScheduleParameterCurve")
public func CoreHaptics_CHHapticPatternPlayer_ScheduleParameterCurve
(
    playerPtr: UnsafeRawPointer,
    curve: CHWHapticParameterCurve,
    atTime: Float,
    onError: ErrorWithPointerCallback
)
{
    _instance().ScheduleParameterCurve(playerPtr: playerPtr, curve: curve, atTime: atTime, onError: onError)
}

