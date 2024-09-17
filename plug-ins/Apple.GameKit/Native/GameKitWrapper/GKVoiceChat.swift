//
//  GKVoiceChat.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKVoiceChat_GetIsVoIPAllowed")
public func GKVoiceChat_GetIsVoIPAllowed
(
    pointer: UnsafeMutablePointer<GKVoiceChat>
) -> Bool
{
    return GKVoiceChat.isVoIPAllowed();
}

@_cdecl("GKVoiceChat_Start")
public func GKVoiceChat_Start
(
    pointer: UnsafeMutablePointer<GKVoiceChat>
)
{
    let target = pointer.takeUnretainedValue();
    target.start();
}

@_cdecl("GKVoiceChat_Stop")
public func GKVoiceChat_Stop
(
    pointer: UnsafeMutablePointer<GKVoiceChat>
)
{
    let target = pointer.takeUnretainedValue();
    target.stop();
}

@_cdecl("GKVoiceChat_GetIsActive")
public func GKVoiceChat_GetIsActive
(
    pointer: UnsafeMutablePointer<GKVoiceChat>
) -> Bool
{
    let target = pointer.takeUnretainedValue();
    return target.isActive;
}

@_cdecl("GKVoiceChat_SetIsActive")
public func GKVoiceChat_SetIsActive
(
    pointer: UnsafeMutablePointer<GKVoiceChat>,
    value: Bool
)
{
    let target = pointer.takeUnretainedValue();
    target.isActive = value;
}

public typealias GKVoiceChatStateDidChangeHandler = @convention(c) (
    UnsafeMutablePointer<GKVoiceChat>,
    UnsafeMutablePointer<GKPlayer>,
    GKVoiceChat.PlayerState) -> Void;

@_cdecl("GKVoiceChat_PlayerVoiceChatStateDidChangeHandler")
public func GKVoiceChat_PlayerVoiceChatStateDidChangeHandler
(
    pointer: UnsafeMutablePointer<GKVoiceChat>,
    callback: @escaping GKVoiceChatStateDidChangeHandler
)
{
    let target = pointer.takeUnretainedValue();
    target.playerVoiceChatStateDidChangeHandler = { player, state in
        callback(pointer, player.passRetainedUnsafeMutablePointer(), state);
    };
}

@_cdecl("GKVoiceChat_SetPlayer")
public func GKVoiceChat_SetPlayer
(
    pointer: UnsafeMutablePointer<GKVoiceChat>,
    playerPtr: UnsafeMutablePointer<GKPlayer>,
    isMuted: Bool
)
{
    let target = pointer.takeUnretainedValue();
    let player = playerPtr.takeUnretainedValue();
    
    target.setPlayer(player, muted: isMuted);
}

@_cdecl("GKVoiceChat_GetVolume")
public func GKVoiceChat_GetVolume
(
    pointer: UnsafeMutablePointer<GKVoiceChat>
) -> Float
{
    let target = pointer.takeUnretainedValue();
    return target.volume;
}

@_cdecl("GKVoiceChat_SetVolume")
public func GKVoiceChat_SetVolume
(
    pointer: UnsafeMutablePointer<GKVoiceChat>,
    value: Float
)
{
    let target = pointer.takeUnretainedValue();
    target.volume = value;
}

@_cdecl("GKVoiceChat_GetName")
public func GKVoiceChat_GetName
(
    pointer: UnsafeMutablePointer<GKVoiceChat>
) -> char_p
{
    let target = pointer.takeUnretainedValue();
    return target.name.toCharPCopy();
}

@_cdecl("GKVoiceChat_GetPlayers")
public func GKVoiceChat_GetPlayers
(
    pointer: UnsafeMutablePointer<GKVoiceChat>
) -> UnsafeMutablePointer<NSArray> // NSArray<GKPlayer>
{
    let target = pointer.takeUnretainedValue();
    return (target.players as NSArray).passRetainedUnsafeMutablePointer();
}
