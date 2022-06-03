//
//  NSArrayExtensions.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("NSArray_GetGKTurnBasedParticipant")
public func NSArray_GetGKTurnBasedParticipant
(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> UnsafeMutableRawPointer?
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue() as! [GKTurnBasedParticipant];
    
    if(array.count > 0 && index < array.count) {
        return Unmanaged.passRetained(array[Int(index)]).toOpaque();
    }
    
    return nil;
}

@_cdecl("NSMutableArray_AddGKTurnBasedParticipant")
public func NSMutableArray_AddGKTurnBasedParticipant
(
    pointer: UnsafeMutableRawPointer,
    value: UnsafeMutableRawPointer
)
{
    let array = Unmanaged<NSMutableArray>.fromOpaque(pointer).takeUnretainedValue();
    array.add(Unmanaged<GKTurnBasedParticipant>.fromOpaque(value).takeUnretainedValue());
}

@_cdecl("NSArray_GetGKTurnBasedExchangeAt")
public func NSArray_GetGKTurnBasedExchangeAt
(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> UnsafeMutableRawPointer?
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue() as! [GKTurnBasedExchange];
    
    if(array.count > 0 && index < array.count) {
        return Unmanaged.passRetained(array[Int(index)]).toOpaque();
    }
    
    return nil;
}

@_cdecl("NSMutableArray_AddGKTurnBasedExchange")
public func NSMutableArray_AddGKTurnBasedExchange
(
    pointer: UnsafeMutableRawPointer,
    value: UnsafeMutableRawPointer
)
{
    let array = Unmanaged<NSMutableArray>.fromOpaque(pointer).takeUnretainedValue();
    array.add(Unmanaged<GKTurnBasedExchange>.fromOpaque(value).takeUnretainedValue());
}

@_cdecl("NSArray_GetGKTurnBasedExchangeReplyAt")
public func NSArray_GetGKTurnBasedExchangeReplyAt
(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> UnsafeMutableRawPointer?
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue() as! [GKTurnBasedExchangeReply];
    
    if(array.count > 0 && index < array.count) {
        return Unmanaged.passRetained(array[Int(index)]).toOpaque();
    }
    
    return nil;
}

@_cdecl("NSMutableArray_AddGKTurnBasedExchangeReply")
public func NSMutableArray_AddGKTurnBasedExchangeReply
(
    pointer: UnsafeMutableRawPointer,
    value: UnsafeMutableRawPointer
)
{
    let array = Unmanaged<NSMutableArray>.fromOpaque(pointer).takeUnretainedValue();
    array.add(Unmanaged<GKTurnBasedExchangeReply>.fromOpaque(value).takeUnretainedValue());
}

@_cdecl("NSArray_GetGKTurnBasedMatchAt")
public func NSArray_GetGKTurnBasedMatchAt
(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> UnsafeMutableRawPointer?
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue() as! [GKTurnBasedMatch];
    
    if(array.count > 0 && index < array.count) {
        return Unmanaged.passRetained(array[Int(index)]).toOpaque();
    }
    
    return nil;
}

@_cdecl("NSMutableArray_AddGKTurnBasedMatch")
public func NSMutableArray_AddGKTurnBasedMatch
(
    pointer: UnsafeMutableRawPointer,
    value: UnsafeMutableRawPointer
)
{
    let array = Unmanaged<NSMutableArray>.fromOpaque(pointer).takeUnretainedValue();
    array.add(Unmanaged<GKTurnBasedMatch>.fromOpaque(value).takeUnretainedValue());
}

@_cdecl("NSArray_GetGKAchievementAt")
public func NSArray_GetGKAchievementAt
(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> UnsafeMutableRawPointer?
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue() as! [GKAchievement];
    
    if(array.count > 0 && index < array.count) {
        return Unmanaged.passRetained(array[Int(index)]).toOpaque();
    }
    
    return nil;
}

@_cdecl("NSMutableArray_AddGKAchievement")
public func NSMutableArray_AddGKAchievement
(
    pointer: UnsafeMutableRawPointer,
    value: UnsafeMutableRawPointer
)
{
    let array = Unmanaged<NSMutableArray>.fromOpaque(pointer).takeUnretainedValue();
    array.add(Unmanaged<GKAchievement>.fromOpaque(value).takeUnretainedValue());
}

@_cdecl("NSArray_GetGKAchievementDescriptionAt")
public func NSArray_GetGKAchievementDescriptionAt
(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> UnsafeMutableRawPointer?
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue() as! [GKAchievementDescription];
    
    if(array.count > 0 && index < array.count) {
        return Unmanaged.passRetained(array[Int(index)]).toOpaque();
    }
    
    return nil;
}

@_cdecl("NSMutableArray_AddGKAchievementDescription")
public func NSMutableArray_AddGKAchievementDescription
(
    pointer: UnsafeMutableRawPointer,
    value: UnsafeMutableRawPointer
)
{
    let array = Unmanaged<NSMutableArray>.fromOpaque(pointer).takeUnretainedValue();
    array.add(Unmanaged<GKAchievementDescription>.fromOpaque(value).takeUnretainedValue());
}

@_cdecl("NSArray_GetGKPlayerAt")
public func NSArray_GetGKPlayerAt
(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> UnsafeMutableRawPointer?
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue() as! [GKPlayer];
    
    if(array.count > 0 && index < array.count) {
        return Unmanaged.passRetained(array[Int(index)]).toOpaque();
    }
    
    return nil;
}

@_cdecl("NSMutableArray_AddGKPlayer")
public func NSMutableArray_AddGKPlayer
(
    pointer: UnsafeMutableRawPointer,
    value: UnsafeMutableRawPointer
)
{
    let array = Unmanaged<NSMutableArray>.fromOpaque(pointer).takeUnretainedValue();
    array.add(Unmanaged<GKPlayer>.fromOpaque(value).takeUnretainedValue());
}

@_cdecl("NSArray_GetGKLeaderboardAt")
public func NSArray_GetGKLeaderboardAt
(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> UnsafeMutableRawPointer?
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue() as! [GKLeaderboard];
    
    if(array.count > 0 && index < array.count) {
        return Unmanaged.passRetained(array[Int(index)]).toOpaque();
    }
    
    return nil;
}

@_cdecl("NSMutableArray_AddGKLeaderboard")
public func NSMutableArray_AddGKLeaderboard
(
    pointer: UnsafeMutableRawPointer,
    value: UnsafeMutableRawPointer
)
{
    let array = Unmanaged<NSMutableArray>.fromOpaque(pointer).takeUnretainedValue();
    array.add(Unmanaged<GKLeaderboard>.fromOpaque(value).takeUnretainedValue());
}

@_cdecl("NSArray_GetGKLeaderboardEntryAt")
public func NSArray_GetGKLeaderboardEntryAt
(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> UnsafeMutableRawPointer?
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue() as! [GKLeaderboard.Entry];
    
    if(array.count > 0 && index < array.count) {
        return Unmanaged.passRetained(array[Int(index)]).toOpaque();
    }
    
    return nil;
}

@_cdecl("NSMutableArray_AddGKLeaderboardEntry")
public func NSMutableArray_AddGKLeaderboardEntry
(
    pointer: UnsafeMutableRawPointer,
    value: UnsafeMutableRawPointer
)
{
    let array = Unmanaged<NSMutableArray>.fromOpaque(pointer).takeUnretainedValue();
    array.add(Unmanaged<GKLeaderboard.Entry>.fromOpaque(value).takeUnretainedValue());
}

@_cdecl("NSArray_GetGKLeaderboardSetAt")
public func NSArray_GetGKLeaderboardSetAt
(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> UnsafeMutableRawPointer?
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue() as! [GKLeaderboardSet];
    
    if(array.count > 0 && index < array.count) {
        return Unmanaged.passRetained(array[Int(index)]).toOpaque();
    }
    
    return nil;
}

@_cdecl("NSMutableArray_AddGKLeaderboardSet")
public func NSMutableArray_AddGKLeaderboardSet
(
    pointer: UnsafeMutableRawPointer,
    value: UnsafeMutableRawPointer
)
{
    let array = Unmanaged<NSMutableArray>.fromOpaque(pointer).takeUnretainedValue();
    array.add(Unmanaged<GKLeaderboardSet>.fromOpaque(value).takeUnretainedValue());
}

@_cdecl("NSArray_GetGKChallengeAt")
public func NSArray_GKChallengeAt
(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> UnsafeMutableRawPointer?
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue() as! [GKChallenge];
    
    if(array.count > 0 && index < array.count) {
        return Unmanaged.passRetained(array[Int(index)]).toOpaque();
    }
    
    return nil;
}

@_cdecl("NSMutableArray_AddGKChallenge")
public func NSMutableArray_AddGKChallenge
(
    pointer: UnsafeMutableRawPointer,
    value: UnsafeMutableRawPointer
)
{
    let array = Unmanaged<NSMutableArray>.fromOpaque(pointer).takeUnretainedValue();
    array.add(Unmanaged<GKChallenge>.fromOpaque(value).takeUnretainedValue());
}
