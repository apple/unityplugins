//
//  GKMatchRequest.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKMatchRequest_Init")
public func GKMatchRequest_Init() -> UnsafeMutableRawPointer
{
    let request = GKMatchRequest.init();
    return Unmanaged.passRetained(request).toOpaque();
}

@_cdecl("GKMatchRequest_Free")
public func GKMatchRequest_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKMatchRequest>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKMatchRequest_SetMaxPlayers")
public func GKMatchRequest_SetMaxPlayers
(
    pointer: UnsafeMutableRawPointer,
    value: Int
)
{
    let target = Unmanaged<GKMatchRequest>.fromOpaque(pointer).takeUnretainedValue();
    target.maxPlayers = value;
}

@_cdecl("GKMatchRequest_GetMaxPlayers")
public func GKMatchRequest_GetMaxPlayers
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKMatchRequest>.fromOpaque(pointer).takeUnretainedValue();
    return target.maxPlayers;
}

@_cdecl("GKMatchRequest_SetMinPlayers")
public func GKMatchRequest_SetMinPlayers
(
    pointer: UnsafeMutableRawPointer,
    value: Int
)
{
    let target = Unmanaged<GKMatchRequest>.fromOpaque(pointer).takeUnretainedValue();
    target.minPlayers = value;
}

@_cdecl("GKMatchRequest_GetMinPlayers")
public func GKMatchRequest_GetMinPlayers
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKMatchRequest>.fromOpaque(pointer).takeUnretainedValue();
    return target.minPlayers;
}

@_cdecl("GKMatchRequest_GetPlayerGroup")
public func GKMatchRequest_GetPlayerGroup
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKMatchRequest>.fromOpaque(pointer).takeUnretainedValue();
    return target.playerGroup;
}

@_cdecl("GKMatchRequest_GetPlayerAttributes")
public func GKMatchRequest_GetPlayerAttributes
(
    pointer: UnsafeMutableRawPointer
) -> UInt32
{
    let target = Unmanaged<GKMatchRequest>.fromOpaque(pointer).takeUnretainedValue();
    return target.playerAttributes;
}

@_cdecl("GKMatchRequest_SetPlayerGroup")
public func GKMatchRequest_SetPlayerGroup
(
    pointer: UnsafeMutableRawPointer,
    value: Int
)
{
    let target = Unmanaged<GKMatchRequest>.fromOpaque(pointer).takeUnretainedValue();
    target.playerGroup = value;
}

@_cdecl("GKMatchRequest_SetPlayerAttributes")
public func GKMatchRequest_SetPlayerAttributes
(
    pointer: UnsafeMutableRawPointer,
    value: UInt32
)
{
    let target = Unmanaged<GKMatchRequest>.fromOpaque(pointer).takeUnretainedValue();
    target.playerAttributes = value;
}

@_cdecl("GKMatchRequest_GetInviteMessage")
public func GKMatchRequest_GetInviteMessage
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let target = Unmanaged<GKMatchRequest>.fromOpaque(pointer).takeUnretainedValue();
    return target.inviteMessage?.toCharPCopy();
}

@_cdecl("GKMatchRequest_SetInviteMessage")
public func GKMatchRequest_SetInviteMessage
(
    pointer: UnsafeMutableRawPointer,
    value: char_p
)
{
    let target = Unmanaged<GKMatchRequest>.fromOpaque(pointer).takeUnretainedValue();
    target.inviteMessage = value.toString();
}

@_cdecl("GKMatchRequest_GetRecipients")
public func GKMatchRequest_GetRecipients
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKMatchRequest>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.recipients != nil) {
        return Unmanaged.passRetained(target.recipients! as NSArray).toOpaque();
    }
    
    return nil;
}
    
@_cdecl("GKMatchRequest_SetRecipients")
public func GKMatchRequest_SetRecipients
(
    pointer: UnsafeMutableRawPointer,
    value: UnsafeMutableRawPointer
)
{
    let target = Unmanaged<GKMatchRequest>.fromOpaque(pointer).takeUnretainedValue();
    target.recipients = Unmanaged<NSArray>.fromOpaque(value).takeUnretainedValue() as? [GKPlayer];
}
