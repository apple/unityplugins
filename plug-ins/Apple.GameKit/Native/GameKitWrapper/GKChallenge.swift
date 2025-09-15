//
//  GKChallenge.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKChallenge_GetIssuingPlayer")
public func GKChallenge_GetIssuingPlayer
(
    pointer: UnsafeMutablePointer<GKChallenge>
) -> UnsafeMutablePointer<GKPlayer>?
{
    let target = pointer.takeUnretainedValue();
    return target.issuingPlayer?.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKChallenge_GetReceivingPlayer")
public func GKChallenge_GetReceivingPlayer
(
    pointer: UnsafeMutablePointer<GKChallenge>
) -> UnsafeMutablePointer<GKPlayer>?
{
    let target = pointer.takeUnretainedValue();
    return target.receivingPlayer?.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKChallenge_GetMessage")
public func GKChallenge_GetMessage
(
    pointer: UnsafeMutablePointer<GKChallenge>
) -> char_p?
{
    let target = pointer.takeUnretainedValue();
    return target.message?.toCharPCopy();
}

@_cdecl("GKChallenge_GetState")
public func GKChallenge_GetState
(
    pointer: UnsafeMutablePointer<GKChallenge>
) -> Int
{
    let target = pointer.takeUnretainedValue();
    return target.state.rawValue;
}

@_cdecl("GKChallenge_GetIssueDate")
public func GKChallenge_GetIssueDate
(
    pointer: UnsafeMutablePointer<GKChallenge>
) -> TimeInterval // aka Double
{
    let target = pointer.takeUnretainedValue();
    return target.issueDate.timeIntervalSince1970;
}

@_cdecl("GKChallenge_GetCompletionDate")
public func GKChallenge_GetCompletionDate
(
    pointer: UnsafeMutablePointer<GKChallenge>
) -> TimeInterval // aka Double
{
    let target = pointer.takeUnretainedValue();
    return target.completionDate?.timeIntervalSince1970 ?? 0.0;
}

@_cdecl("GKChallenge_Decline")
public func GKChallenge_Decline
(
    pointer: UnsafeMutablePointer<GKChallenge>
)
{
    let target = pointer.takeUnretainedValue();
    target.decline();
}

@_cdecl("GKChallenge_LoadReceivedChallenges")
public func GKChallenge_LoadReceivedChallenges
(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<NSArray>, // NSArray<GKChallenge>
    onError: @escaping NSErrorTaskCallback
)
{
    GKChallenge.loadReceivedChallenges(completionHandler: {challenges, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, (challenges as? NSArray)?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKChallenge_GetChallengeType")
public func GKChallenge_GetChallengeType
(
    pointer: UnsafeMutablePointer<GKChallenge>
) -> Int
{
    let challenge = pointer.takeUnretainedValue();
    
    if challenge is GKScoreChallenge {
        return 0;
    }
    
    if challenge is GKAchievementChallenge {
        return 1;
    }
    
    return -1;
}
