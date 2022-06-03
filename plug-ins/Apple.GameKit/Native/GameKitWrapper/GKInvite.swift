//
//  GKInvite.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKInvite_Free")
public func GKInvite_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKInvite>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKInvite_GetSender")
public func GKInvite_GetSender
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKInvite>.fromOpaque(pointer).takeUnretainedValue();
    return Unmanaged.passRetained(target.sender).toOpaque();
}

@_cdecl("GKInvite_GetPlayerAttributes")
public func GKInvite_GetPlayerAttributes
(
    pointer: UnsafeMutableRawPointer
) -> UInt32
{
    let target = Unmanaged<GKInvite>.fromOpaque(pointer).takeUnretainedValue();
    return target.playerAttributes;
}

@_cdecl("GKInvite_GetPlayerGroup")
public func GKInvite_GetPlayerGroup
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKInvite>.fromOpaque(pointer).takeUnretainedValue();
    return target.playerGroup;
}

@_cdecl("GKInvite_GetIsHosted")
public func GKInvite_GetIsHosted
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let target = Unmanaged<GKInvite>.fromOpaque(pointer).takeUnretainedValue();
    return target.isHosted;
}
