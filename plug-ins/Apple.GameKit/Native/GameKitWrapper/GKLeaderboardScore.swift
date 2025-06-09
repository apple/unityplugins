//
//  GKLeaderboardScore.swift
//  GameKitWrapper
//
//  Copyright © 2025 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKLeaderboardScore_GetContext")
public func GKLeaderboardScore_GetContext
(
    thisPtr: UnsafeMutableRawPointer // GKLeaderboardScore
) -> UInt
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, *) {
        let thisObj: GKLeaderboardScore = thisPtr.takeUnretainedValue();
        return UInt(thisObj.context);
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKLeaderboardScore_SetContext")
public func GKLeaderboardScore_SetContext
(
    thisPtr: UnsafeMutableRawPointer, // GKLeaderboardScore
    context: UInt
)
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, *) {
        let thisObj: GKLeaderboardScore = thisPtr.takeUnretainedValue();
        thisObj.context = Int(context);
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKLeaderboardScore_GetLeaderboardID")
public func GKLeaderboardScore_GetLeaderboardID
(
    thisPtr: UnsafeMutableRawPointer // GKLeaderboardScore
) -> char_p
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, *) {
        let thisObj: GKLeaderboardScore = thisPtr.takeUnretainedValue();
        return thisObj.leaderboardID.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKLeaderboardScore_SetLeaderboardID")
public func GKLeaderboardScore_SetLeaderboardID
(
    thisPtr: UnsafeMutableRawPointer, // GKLeaderboardScore
    leaderboardID: char_p
)
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, *) {
        let thisObj: GKLeaderboardScore = thisPtr.takeUnretainedValue();
        thisObj.leaderboardID = leaderboardID.toString();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKLeaderboardScore_GetPlayer")
public func GKLeaderboardScore_GetPlayer
(
    thisPtr: UnsafeMutableRawPointer // GKLeaderboardScore
)  -> UnsafeMutablePointer<GKPlayer>
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, *) {
        let thisObj: GKLeaderboardScore = thisPtr.takeUnretainedValue();
        return thisObj.player.passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKLeaderboardScore_SetPlayer")
public func GKLeaderboardScore_SetPlayer
(
    thisPtr: UnsafeMutableRawPointer, // GKLeaderboardScore
    playerPtr: UnsafeMutablePointer<GKPlayer>
)
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, *) {
        let thisObj: GKLeaderboardScore = thisPtr.takeUnretainedValue();
        let playerObj = playerPtr.takeUnretainedValue();
        thisObj.player = playerObj;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKLeaderboardScore_GetValue")
public func GKLeaderboardScore_GetValue
(
    thisPtr: UnsafeMutableRawPointer // GKLeaderboardScore
) -> Int
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, *) {
        let thisObj: GKLeaderboardScore = thisPtr.takeUnretainedValue();
        return thisObj.value;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKLeaderboardScore_SetValue")
public func GKLeaderboardScore_SetValue
(
    thisPtr: UnsafeMutableRawPointer, // GKLeaderboardScore
    value: Int
)
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, *) {
        let thisObj: GKLeaderboardScore = thisPtr.takeUnretainedValue();
        thisObj.value = value;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

