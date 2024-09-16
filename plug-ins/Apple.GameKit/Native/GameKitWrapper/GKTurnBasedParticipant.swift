//
//  GKTurnBasedParticipant.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKTurnBasedParticipant_GetPlayer")
public func GKTurnBasedParticipant_GetPlayer
(
    pointer: UnsafeMutablePointer<GKTurnBasedParticipant>
) -> UnsafeMutablePointer<GKPlayer>?
{
    let target = pointer.takeUnretainedValue();
    return target.player?.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKTurnBasedParticipant_GetStatus")
public func GKTurnBasedParticipant_GetStatus
(
    pointer: UnsafeMutablePointer<GKTurnBasedParticipant>
) -> Int
{
    let target = pointer.takeUnretainedValue();
    return target.status.rawValue;
}

@_cdecl("GKTurnBasedParticipant_GetLastTurnDate")
public func GKTurnBasedParticipant_GetLastTurnDate
(
    pointer: UnsafeMutablePointer<GKTurnBasedParticipant>
) -> TimeInterval // aka Double
{
    let target = pointer.takeUnretainedValue();
    return target.lastTurnDate?.timeIntervalSince1970 ?? 0.0;
}

@_cdecl("GKTurnBasedParticipant_GetTimeoutDate")
public func GKTurnBasedParticipant_GetTimeoutDate
(
    pointer: UnsafeMutablePointer<GKTurnBasedParticipant>
) -> TimeInterval // aka Double
{
    let target = pointer.takeUnretainedValue();
    return target.timeoutDate?.timeIntervalSince1970 ?? 0.0;
}

@_cdecl("GKTurnBasedParticipant_GetMatchOutcome")
public func GKTurnBasedParticipant_GetMatchOutcome
(
    pointer: UnsafeMutablePointer<GKTurnBasedParticipant>
) -> Int
{
    let target = pointer.takeUnretainedValue();
    return target.matchOutcome.rawValue;
}

@_cdecl("GKTurnBasedParticipant_SetMatchOutcome")
public func GKTurnBasedParticipant_SetMatchOutcome
(
    pointer: UnsafeMutablePointer<GKTurnBasedParticipant>,
    value: Int
)
{
    let target = pointer.takeUnretainedValue();
    target.matchOutcome = GKTurnBasedMatch.Outcome.init(rawValue: value)!;
}
