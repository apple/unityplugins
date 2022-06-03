//
//  CHHapticEngine.swift
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//
import Foundation
import CoreHaptics

public typealias StoppedWithReasonCallback = @convention(c) (UnsafeRawPointer, Int32) -> Void;
public typealias AllPlayersFinishedCallback = @convention(c) (UnsafeRawPointer) -> Int32;

fileprivate var _implementation : CHHapticEngine_Protocol? = nil;

fileprivate func _instance() -> CHHapticEngine_Protocol {
    if _implementation == nil {
        if #available(iOS 13, tvOS 14.0, macOS 11, *) {
            _implementation = CHHapticEngine_Supported();
        } else {
            _implementation = CHHapticEngine_Unsupported();
        }
    }
    
    return _implementation!;
}

@_cdecl("CoreHaptics_CHHapticEngine_Create")
public func CoreHaptics_CHHapticEngine_Create
(
    onError: ErrorCallback,
    onStopped: @escaping StoppedWithReasonCallback,
    onReset: @escaping SuccessWithPointerCallback
) -> UnsafeMutableRawPointer?
{
    return _instance().Create(onError: onError, onStopped: onStopped, onReset: onReset);
}

@_cdecl("CoreHaptics_CHHapticEngine_Destroy")
public func CoreHaptics_CHHapticEngine_Destroy
(
    enginePtr: UnsafeRawPointer
)
{
    _instance().Destroy(enginePtr: enginePtr);
}

@_cdecl("CoreHaptics_CHHapticEngine_Start")
public func CoreHaptics_CHHapticEngine_Start
(
    enginePtr: UnsafeRawPointer,
    onError: @escaping ErrorWithPointerCallback
)
{
    _instance().Start(enginePtr: enginePtr, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticEngine_Stop")
public func CoreHaptics_CHHapticEngine_Stop
(
    enginePtr: UnsafeRawPointer,
    onError : @escaping ErrorWithPointerCallback
)
{
    _instance().Stop(enginePtr: enginePtr, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticEngine_PlayPatternFromUrl")
public func CoreHaptics_CHHapticEngine_PlayPatternFromUrl
(
    enginePtr: UnsafeRawPointer,
    ahapUrl: char_p,
    onError: @escaping ErrorWithPointerCallback
)
{
    _instance().PlayPatternFromUrl(enginePtr: enginePtr, ahapUrl: ahapUrl, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticEngine_PlayPatternFromJson")
public func CoreHaptics_CHHapticEngine_PlayPatternFromJson
(
    enginePtr: UnsafeRawPointer,
    ahapJson: char_p,
    onError: @escaping ErrorWithPointerCallback
)
{
    _instance().PlayPatternFromJson(enginePtr: enginePtr, ahapJson: ahapJson, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticEngine_MakePlayer")
public func CoreHaptics_CHHapticEngine_MakePlayer
(
    enginePtr: UnsafeRawPointer,
    ahapJson: char_p,
    onError: ErrorCallback
) -> UnsafeMutableRawPointer?
{
    return _instance().MakePlayer(enginePtr: enginePtr, ahapJson: ahapJson, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticEngine_MakeAdvancedPlayer")
public func CoreHaptics_CHHapticEngine_MakeAdvancedPlayer
(
    enginePtr: UnsafeRawPointer,
    ahapJson: char_p,
    onFinishedPlaying: @escaping SuccessWithPointerCallback,
    onError: ErrorCallback
) -> UnsafeMutableRawPointer?
{
    return _instance().MakeAdvancedPlayer(enginePtr: enginePtr, ahapJson: ahapJson, onFinishedPlaying: onFinishedPlaying, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticEngine_NotifyWhenPlayersFinished")
public func CoreHaptics_CHHapticEngine_NotifyWhenPlayersFinished
(
    enginePtr: UnsafeRawPointer, leaveEngineRunning: Bool,
    onAllPlayersFinished: @escaping AllPlayersFinishedCallback,
    onError: @escaping ErrorWithPointerCallback
)
{
    _instance().NotifyWhenPlayersFinished(enginePtr: enginePtr, leaveEngineRunning: leaveEngineRunning, onAllPlayersFinished: onAllPlayersFinished, onError: onError);
}

@_cdecl("CoreHaptics_CHHapticEngine_HardwareSupportsHaptics")
public func CoreHaptics_CHHapticEngine_HardwareSupportsHaptics() -> Bool
{
    return _instance().HardwareSupportsHaptics();
}

@_cdecl("CoreHaptics_CHHapticEngine_Get_IsAutoShutdownEnabled")
public func CoreHaptics_CHHapticEngine_Get_IsAutoShutdownEnabled
(
    enginePtr: UnsafeRawPointer
) -> Bool
{
    return _instance().Get_IsAutoShutdownEnabled(enginePtr: enginePtr);
}

@_cdecl("CoreHaptics_CHHapticEngine_Set_IsAutoShutdownEnabled")
public func CoreHaptics_CHHapticEngine_Set_IsAutoShutdownEnabled
(
    enginePtr: UnsafeRawPointer,
    isAutoShutdownEnabled: Bool
)
{
    _instance().Set_IsAutoShutdownEnabled(enginePtr: enginePtr, isAutoShutdownEnabled: isAutoShutdownEnabled);
}

@_cdecl("CoreHaptics_CHHapticEngine_Get_PlaysHapticsOnly")
public func CoreHaptics_CHHapticEngine_Get_PlaysHapticsOnly
(
    enginePtr: UnsafeRawPointer
) -> Bool
{
    return _instance().Get_PlaysHapticsOnly(enginePtr: enginePtr);
}

@_cdecl("CoreHaptics_CHHapticEngine_Set_PlaysHapticsOnly")
public func CoreHaptics_CHHapticEngine_Set_PlaysHapticsOnly
(
    enginePtr: UnsafeRawPointer,
    playsHapticsOnly: Bool
)
{
    _instance().Set_PlaysHapticsOnly(enginePtr: enginePtr, playsHapticsOnly: playsHapticsOnly);
}

@_cdecl("CoreHaptics_CHHapticEngine_Get_IsMutedForAudio")
public func CoreHaptics_CHHapticEngine_Get_IsMutedForAudio
(
    enginePtr: UnsafeRawPointer
) -> Bool
{
    return _instance().Get_IsMutedForAudio(enginePtr: enginePtr);
}

@_cdecl("CoreHaptics_CHHapticEngine_Set_IsMutedForAudio")
public func CoreHaptics_CHHapticEngine_Set_IsMutedForAudio
(
    enginePtr: UnsafeRawPointer,
    isMutedForAudio: Bool
)
{
    _instance().Set_IsMutedForAudio(enginePtr: enginePtr, isMutedForAudio: isMutedForAudio);
}

@_cdecl("CoreHaptics_CHHapticEngine_Get_IsMutedForHaptics")
public func CoreHaptics_CHHapticEngine_Get_IsMutedForHaptics
(
    enginePtr: UnsafeRawPointer
) -> Bool
{
    return _instance().Get_IsMutedForHaptics(enginePtr: enginePtr);
}

@_cdecl("CoreHaptics_CHHapticEngine_Set_IsMutedForHaptics")
public func CoreHaptics_CHHapticEngine_Set_IsMutedForHaptics
(
    enginePtr: UnsafeRawPointer,
    isMutedForHaptics: Bool
)
{
    _instance().Set_IsMutedForHaptics(enginePtr: enginePtr, isMutedForHaptics: isMutedForHaptics);
}

@_cdecl("CoreHaptics_CHHapticEngine_Get_CurrentTime")
public func CoreHaptics_CHHapticEngine_Get_CurrentTime
(
    enginePtr: UnsafeRawPointer
) -> Double
{
    return _instance().Get_CurrentTime(enginePtr: enginePtr);
}
