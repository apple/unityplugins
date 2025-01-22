//
//  GKInvite.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKInvite_GetSender")
public func GKInvite_GetSender
(
    pointer: UnsafeMutablePointer<GKInvite>
) -> UnsafeMutablePointer<GKPlayer>?
{
    let target = pointer.takeUnretainedValue();
    return target.sender.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKInvite_GetPlayerAttributes")
public func GKInvite_GetPlayerAttributes
(
    pointer: UnsafeMutablePointer<GKInvite>
) -> UInt32
{
    let target = pointer.takeUnretainedValue();
    return target.playerAttributes;
}

@_cdecl("GKInvite_GetPlayerGroup")
public func GKInvite_GetPlayerGroup
(
    pointer: UnsafeMutablePointer<GKInvite>
) -> Int
{
    let target = pointer.takeUnretainedValue();
    return target.playerGroup;
}

@_cdecl("GKInvite_GetIsHosted")
public func GKInvite_GetIsHosted
(
    pointer: UnsafeMutablePointer<GKInvite>
) -> Bool
{
    let target = pointer.takeUnretainedValue();
    return target.isHosted;
}
