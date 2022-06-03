//
//  GKTurnBasedParticipant.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKTurnBasedParticipant_Free")
public func GKTurnBasedParticipant_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKTurnBasedParticipant>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKTurnBasedParticipant_GetPlayer")
public func GKTurnBasedParticipant_GetPlayer
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKTurnBasedParticipant>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.player != nil) {
        return Unmanaged.passRetained(target.player!).toOpaque();
    }

    return nil;
}

@_cdecl("GKTurnBasedParticipant_GetStatus")
public func GKTurnBasedParticipant_GetStatus
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKTurnBasedParticipant>.fromOpaque(pointer).takeUnretainedValue();
    return target.status.rawValue;
}

@_cdecl("GKTurnBasedParticipant_GetLastTurnDate")
public func GKTurnBasedParticipant_GetLastTurnDate
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKTurnBasedParticipant>.fromOpaque(pointer).takeUnretainedValue();
    return Int(target.lastTurnDate?.timeIntervalSince1970 ?? 0);
}

@_cdecl("GKTurnBasedParticipant_GetTimeoutDate")
public func GKTurnBasedParticipant_GetTimeoutDate
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKTurnBasedParticipant>.fromOpaque(pointer).takeUnretainedValue();
    return Int(target.timeoutDate?.timeIntervalSince1970 ?? 0);
}

@_cdecl("GKTurnBasedParticipant_GetMatchOutcome")
public func GKTurnBasedParticipant_GetMatchOutcome
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKTurnBasedParticipant>.fromOpaque(pointer).takeUnretainedValue();
    return target.matchOutcome.rawValue;
}

@_cdecl("GKTurnBasedParticipant_SetMatchOutcome")
public func GKTurnBasedParticipant_SetMatchOutcome
(
    pointer: UnsafeMutableRawPointer,
    value: Int
)
{
    let target = Unmanaged<GKTurnBasedParticipant>.fromOpaque(pointer).takeUnretainedValue();
    target.matchOutcome = GKTurnBasedMatch.Outcome.init(rawValue: value)!;
}
