//
//  GKLeaderboardEntry.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKLeaderboardEntry_GetRank")
public func GKLeaderboardEntry_GetRank
(
    pointer: UnsafeMutableRawPointer // GKLeaderboard.Entry
) -> Int
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKLeaderboard.Entry = pointer.takeUnretainedValue();
        return target.rank;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKLeaderboardEntry_GetScore")
public func GKLeaderboardEntry_GetScore
(
    pointer: UnsafeMutableRawPointer // GKLeaderboard.Entry
) -> Int
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKLeaderboard.Entry = pointer.takeUnretainedValue();
        return target.score;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKLeaderboardEntry_GetContext")
public func GKLeaderboardEntry_GetContext
(
    pointer: UnsafeMutableRawPointer // GKLeaderboard.Entry
) -> Int
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKLeaderboard.Entry = pointer.takeUnretainedValue();
        return target.context;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKLeaderboardEntry_GetDate")
public func GKLeaderboardEntry_GetDate
(
    pointer: UnsafeMutableRawPointer // GKLeaderboard.Entry
) -> TimeInterval // aka Double
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKLeaderboard.Entry = pointer.takeUnretainedValue();
        return target.date.timeIntervalSince1970;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKLeaderboardEntry_GetFormattedScore")
public func GKLeaderboardEntry_GetFormattedScore
(
    pointer: UnsafeMutableRawPointer // GKLeaderboard.Entry
) -> char_p
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKLeaderboard.Entry = pointer.takeUnretainedValue();
        return target.formattedScore.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKLeaderboardEntry_GetPlayer")
public func GKLeaderboardEntry_GetPlayer
(
    pointer: UnsafeMutableRawPointer // GKLeaderboard.Entry
) -> UnsafeMutablePointer<GKPlayer>
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKLeaderboard.Entry = pointer.takeUnretainedValue();
        return target.player.passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}
