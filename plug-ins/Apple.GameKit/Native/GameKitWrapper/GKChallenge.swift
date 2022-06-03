//
//  GKChallenge.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKChallenge_Free")
public func GKChallenge_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKChallenge>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKChallenge_GetIssuingPlayer")
public func GKChallenge_GetIssuingPlayer
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKChallenge>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.issuingPlayer != nil) {
        return Unmanaged.passRetained(target.issuingPlayer!).toOpaque();
    }
    
    return nil;
}

@_cdecl("GKChallenge_GetReceivingPlayer")
public func GKChallenge_GetReceivingPlayer
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKChallenge>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.receivingPlayer != nil) {
        return Unmanaged.passRetained(target.receivingPlayer!).toOpaque();
    }
    
    return nil;
}

@_cdecl("GKChallenge_GetMessage")
public func GKChallenge_GetMessage
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let target = Unmanaged<GKChallenge>.fromOpaque(pointer).takeUnretainedValue();
    return target.message?.toCharPCopy();
}

@_cdecl("GKChallenge_GetState")
public func GKChallenge_GetState
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKChallenge>.fromOpaque(pointer).takeUnretainedValue();
    return target.state.rawValue;
}

@_cdecl("GKChallenge_GetIssueDate")
public func GKChallenge_GetIssueDate
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKChallenge>.fromOpaque(pointer).takeUnretainedValue();
    return Int(target.issueDate.timeIntervalSince1970);
}

@_cdecl("GKChallenge_GetCompletionDate")
public func GKChallenge_GetCompletionDate
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKChallenge>.fromOpaque(pointer).takeUnretainedValue();
    return Int(target.completionDate?.timeIntervalSince1970 ?? 0);
}

@_cdecl("GKChallenge_Decline")
public func GKChallenge_Decline
(
    pointer: UnsafeMutableRawPointer
)
{
    let target = Unmanaged<GKChallenge>.fromOpaque(pointer).takeUnretainedValue();
    target.decline();
}

@_cdecl("GKChallenge_LoadReceivedChallenges")
public func GKChallenge_LoadReceivedChallenges
(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    GKChallenge.loadReceivedChallenges(completionHandler: {challenges, error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        if(challenges != nil) {
            onSuccess(taskId, Unmanaged.passRetained(challenges! as NSArray).toOpaque());
        } else {
            onSuccess(taskId, nil);
        }
    });
}

@_cdecl("GKChallenge_GetChallengeType")
public func GKChallenge_GetChallengeType
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let challenge = Unmanaged<GKChallenge>.fromOpaque(pointer).takeUnretainedValue();
    
    if challenge is GKScoreChallenge {
        return 0;
    }
    
    if challenge is GKAchievementChallenge {
        return 1;
    }
    
    return -1;
}
