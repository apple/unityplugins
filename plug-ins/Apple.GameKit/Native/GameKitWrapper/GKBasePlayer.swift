//
//  GKBasePlayer.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKBasePlayer_GetDisplayName")
public func GKBasePlayer_GetDisplayName
(
    pointer : UnsafeMutableRawPointer
) -> char_p?
{
    let player = Unmanaged<GKBasePlayer>.fromOpaque(pointer).takeUnretainedValue();
    return player.displayName?.toCharPCopy();
}
