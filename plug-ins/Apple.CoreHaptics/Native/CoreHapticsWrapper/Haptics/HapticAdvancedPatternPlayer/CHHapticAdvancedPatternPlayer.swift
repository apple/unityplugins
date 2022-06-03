//
//  CHAdvancedHapticPatternPlayer.swift
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import CoreHaptics

fileprivate var _advancedPlayerImplementation : CHHapticAdvancedPatternPlayer_Protocol? = nil;

fileprivate func _instance() -> CHHapticAdvancedPatternPlayer_Protocol {
    if _advancedPlayerImplementation == nil {
        if #available(iOS 13, tvOS 14, macOS 11, *) {
            _advancedPlayerImplementation = CHHapticAdvancedPatternPlayer_Supported();
        } else {
            _advancedPlayerImplementation = CHHapticAdvancedPatternPlayer_Unsupported();
        }
    }
    
    return _advancedPlayerImplementation!;
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_Start")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_Start
(
    playerPtr:  UnsafeRawPointer,
    startTime: Float,
    onError: ErrorWithPointerCallback
)
{
    _instance().Start(playerPtr: playerPtr, startTime: startTime, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_Stop")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_Stop
(
    playerPtr:  UnsafeRawPointer,
    stopTime: Float,
    onError: ErrorWithPointerCallback
)
{
    _instance().Stop(playerPtr: playerPtr, stopTime: stopTime, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_Pause")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_Pause
(
    playerPtr:  UnsafeRawPointer,
    pauseTime: Float,
    onError: ErrorWithPointerCallback
)
{
    _instance().Pause(playerPtr: playerPtr, pauseTime: pauseTime, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_Resume")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_Resume
(
    playerPtr:  UnsafeRawPointer,
    resumeTime: Float,
    onError: ErrorWithPointerCallback
)
{
    _instance().Resume(playerPtr: playerPtr, resumeTime: resumeTime, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_Seek")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_Seek
(
    playerPtr:  UnsafeRawPointer,
    seekTime: Float,
    onError: ErrorWithPointerCallback
)
{
    _instance().Seek(playerPtr: playerPtr, seekTime: seekTime, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_GetIsMuted")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_GetIsMuted
(
    playerPtr:  UnsafeRawPointer
) -> Bool
{
    return _instance().Get_IsMuted(playerPtr: playerPtr);
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_SetIsMuted")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_SetIsMuted
(
    playerPtr:  UnsafeRawPointer,
    isMuted: Bool
)
{
    _instance().Set_IsMuted(playerPtr: playerPtr, isMuted: isMuted);
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_GetLoopEnabled")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_GetLoopEnabled
(
    playerPtr:  UnsafeRawPointer
) -> Bool
{
    return _instance().Get_LoopEnabled(playerPtr: playerPtr);
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_SetLoopEnabled")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_SetLoopEnabled
(
    playerPtr:  UnsafeRawPointer,
    loopEnabled: Bool
)
{
    _instance().Set_LoopEnabled(playerPtr: playerPtr, isLoopEnabled: loopEnabled);
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_GetLoopEnd")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_GetLoopEnd
(
    playerPtr:  UnsafeRawPointer
) -> Double
{
    return _instance().Get_LoopEnd(playerPtr: playerPtr);
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_SetLoopEnd")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_SetLoopEnd
(
    playerPtr:  UnsafeRawPointer,
    loopEnd: Double
)
{
    _instance().Set_LoopEnd(playerPtr: playerPtr, loopEnd: loopEnd);
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_GetPlaybackRate")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_GetPlaybackRate
(
    playerPtr:  UnsafeRawPointer
) -> Float
{
    return _instance().Get_PlaybackRate(playerPtr: playerPtr);
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_SetPlaybackRate")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_SetPlaybackRate
(
    playerPtr:  UnsafeRawPointer,
    rate: Float
)
{
    _instance().Set_PlaybackRate(playerPtr: playerPtr, rate: rate);
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_Destroy")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_Destroy
(
    playerPtr:  UnsafeRawPointer
)
{
    _instance().Destroy(playerPtr: playerPtr);
}

@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_SendParameters")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_SendParameters
(
    playerPtr: UnsafeRawPointer,
    request: CHWSendParametersRequest,
    onError: ErrorWithPointerCallback
)
{
    _instance().SendParameters(playerPtr: playerPtr, request: request, onError: onError);
}


@_cdecl("CoreHaptics_CHHapticAdvancedPatternPlayer_RegisterCompletionHandler")
public func CoreHaptics_CHHapticAdvancedPatternPlayer_RegisterCompletionHandler
(
    playerPtr: UnsafeRawPointer,
    handler: @escaping SuccessWithPointerCallback,
    onError: ErrorWithPointerCallback
)
{
    _instance().RegisterCompletionHandler(playerPtr: playerPtr, handler: handler, onError: onError)
}
