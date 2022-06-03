//
//  GKVoiceChat.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKVoiceChat_Free")
public func GKVoiceChat_Free
(
    pointer : UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKVoiceChat>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKVoiceChat_GetIsVoIPAllowed")
public func GKVoiceChat_GetIsVoIPAllowed
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    return GKVoiceChat.isVoIPAllowed();
}

@_cdecl("GKVoiceChat_Start")
public func GKVoiceChat_Start
(
    pointer: UnsafeMutableRawPointer
)
{
    let target = Unmanaged<GKVoiceChat>.fromOpaque(pointer).takeUnretainedValue();
    target.start();
}

@_cdecl("GKVoiceChat_Stop")
public func GKVoiceChat_Stop
(
    pointer: UnsafeMutableRawPointer
)
{
    let target = Unmanaged<GKVoiceChat>.fromOpaque(pointer).takeUnretainedValue();
    target.stop();
}

@_cdecl("GKVoiceChat_GetIsActive")
public func GKVoiceChat_GetIsActive
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let target = Unmanaged<GKVoiceChat>.fromOpaque(pointer).takeUnretainedValue();
    return target.isActive;
}

@_cdecl("GKVoiceChat_SetIsActive")
public func GKVoiceChat_SetIsActive
(
    pointer: UnsafeMutableRawPointer,
    value: Bool
)
{
    let target = Unmanaged<GKVoiceChat>.fromOpaque(pointer).takeUnretainedValue();
    target.isActive = value;
}

public typealias GKVoiceChatStateDidChangeHandler = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, GKVoiceChat.PlayerState) -> Void;

@_cdecl("GKVoiceChat_PlayerVoiceChatStateDidChangeHandler")
public func GKVoiceChat_PlayerVoiceChatStateDidChangeHandler
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKVoiceChatStateDidChangeHandler
)
{
    let target = Unmanaged<GKVoiceChat>.fromOpaque(pointer).takeUnretainedValue();
    target.playerVoiceChatStateDidChangeHandler = { player, state in
        callback(pointer, Unmanaged.passRetained(player).toOpaque(), state);
    };
}

@_cdecl("GKVoiceChat_SetPlayer")
public func GKVoiceChat_SetPlayer
(
    pointer: UnsafeMutableRawPointer,
    playerPtr: UnsafeMutableRawPointer,
    isMuted: Bool
)
{
    let target = Unmanaged<GKVoiceChat>.fromOpaque(pointer).takeUnretainedValue();
    let player = Unmanaged<GKPlayer>.fromOpaque(playerPtr).takeUnretainedValue();
    
    target.setPlayer(player, muted: isMuted);
}

@_cdecl("GKVoiceChat_GetVolume")
public func GKVoiceChat_GetVolume
(
    pointer: UnsafeMutableRawPointer
) -> Float
{
    let target = Unmanaged<GKVoiceChat>.fromOpaque(pointer).takeUnretainedValue();
    return target.volume;
}

@_cdecl("GKVoiceChat_SetVolume")
public func GKVoiceChat_SetVolume
(
    pointer: UnsafeMutableRawPointer,
    value: Float
)
{
    let target = Unmanaged<GKVoiceChat>.fromOpaque(pointer).takeUnretainedValue();
    target.volume = value;
}

@_cdecl("GKVoiceChat_GetName")
public func GKVoiceChat_GetName
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let target = Unmanaged<GKVoiceChat>.fromOpaque(pointer).takeUnretainedValue();
    return target.name.toCharPCopy();
}

@_cdecl("GKVoiceChat_GetPlayers")
public func GKVoiceChat_GetPlayers
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let target = Unmanaged<GKVoiceChat>.fromOpaque(pointer).takeUnretainedValue();
    return Unmanaged.passRetained(target.players as NSArray).toOpaque();
}
