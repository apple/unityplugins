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
    pointer : UnsafeMutablePointer<GKBasePlayer>
) -> char_p?
{
    let player = pointer.takeUnretainedValue();
    return player.displayName?.toCharPCopy();
}
