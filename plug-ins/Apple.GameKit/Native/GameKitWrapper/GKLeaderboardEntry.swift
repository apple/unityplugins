//
//  GKLeaderboardEntry.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKLeaderboardEntry_Free")
public func GKLeaderboardEntry_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKLeaderboard.Entry>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKLeaderboardEntry_GetRank")
public func GKLeaderboardEntry_GetRank
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKLeaderboard.Entry>.fromOpaque(pointer).takeUnretainedValue();
    return target.rank;
}

@_cdecl("GKLeaderboardEntry_GetScore")
public func GKLeaderboardEntry_GetScore
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKLeaderboard.Entry>.fromOpaque(pointer).takeUnretainedValue();
    return target.score;
}

@_cdecl("GKLeaderboardEntry_GetContext")
public func GKLeaderboardEntry_GetContext
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKLeaderboard.Entry>.fromOpaque(pointer).takeUnretainedValue();
    return target.context;
}

@_cdecl("GKLeaderboardEntry_GetDate")
public func GKLeaderboardEntry_GetDate
(
    pointer: UnsafeMutableRawPointer
) -> Double
{
    let target = Unmanaged<GKLeaderboard.Entry>.fromOpaque(pointer).takeUnretainedValue();
    return target.date.timeIntervalSince1970;
}

@_cdecl("GKLeaderboardEntry_GetFormattedScore")
public func GKLeaderboardEntry_GetFormattedScore
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let target = Unmanaged<GKLeaderboard.Entry>.fromOpaque(pointer).takeUnretainedValue();
    return target.formattedScore.toCharPCopy();
}

@_cdecl("GKLeaderboardEntry_GetPlayer")
public func GKLeaderboardEntry_GetPlayer
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let target = Unmanaged<GKLeaderboard.Entry>.fromOpaque(pointer).takeUnretainedValue();
    return Unmanaged.passRetained(target.player).toOpaque();
}
