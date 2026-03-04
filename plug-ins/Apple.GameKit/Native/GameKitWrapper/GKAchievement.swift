//
//  GKAchievement.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKAchievement_Init")
public func GKAchievement_Init
(
    identifier: char_p
) -> UnsafeMutablePointer<GKAchievement>
{
    let achievement = GKAchievement.init(identifier: identifier.toString());
    return achievement.passRetainedUnsafeMutablePointer();
}

// Initialize the achievement for a specific player. Use to submit participant achievements when ending a turn-based match.
@_cdecl("GKAchievement_InitForPlayer")
public func GKAchievement_InitForPlayer
(
    identifier: char_p,
    gkPlayerPtr : UnsafeMutablePointer<GKPlayer>
) -> UnsafeMutablePointer<GKAchievement>
{
    let player = gkPlayerPtr.takeUnretainedValue();
    let achievement = GKAchievement.init(identifier: identifier.toString(), player: player);
    return achievement.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKAchievement_GetIdentifier")
public func GKAchievement_GetIdentifier
(
    pointer: UnsafeMutablePointer<GKAchievement>
) -> char_p
{
    let achievement = pointer.takeUnretainedValue();
    return achievement.identifier.toCharPCopy();
}

@_cdecl("GKAchievement_SetIdentifier")
public func GKAchievement_SetIdentifier
(
    pointer: UnsafeMutablePointer<GKAchievement>,
    value: char_p
)
{
    let achievement = pointer.takeUnretainedValue();
    achievement.identifier = value.toString();
}

@_cdecl("GKAchievement_GetPlayer")
public func GKAchievement_GetPlayer
(
    pointer: UnsafeMutablePointer<GKAchievement>
) -> UnsafeMutablePointer<GKPlayer>
{
    let achievement = pointer.takeUnretainedValue();
    return achievement.player.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKAchievement_GetPercentComplete")
public func GKAchievement_GetPercentComplete
(
    pointer: UnsafeMutablePointer<GKAchievement>
) -> Double
{
    let achievement = pointer.takeUnretainedValue();
    return achievement.percentComplete;
}

@_cdecl("GKAchievement_SetPercentComplete")
public func GKAchievement_SetPercentComplete
(
    pointer: UnsafeMutablePointer<GKAchievement>,
    value: Double
)
{
    let achievement = pointer.takeUnretainedValue();
    achievement.percentComplete = value;
}

@_cdecl("GKAchievement_GetIsCompleted")
public func GKAchievement_GetIsCompleted
(
    pointer: UnsafeMutablePointer<GKAchievement>
) -> Bool
{
    let achievement = pointer.takeUnretainedValue();
    return achievement.isCompleted;
}

@_cdecl("GKAchievement_GetLastReportedDate")
public func GKAchievement_GetLastReportedDate
(
    pointer: UnsafeMutablePointer<GKAchievement>
) -> TimeInterval // aka Double
{
    let achievement = pointer.takeUnretainedValue();
    return achievement.lastReportedDate.timeIntervalSince1970;
}

@_cdecl("GKAchievement_GetShowCompletionBanner")
public func GKAchievement_GetShowCompletionBanner
(
    pointer: UnsafeMutablePointer<GKAchievement>
) -> Bool
{
    let achievement = pointer.takeUnretainedValue();
    return achievement.showsCompletionBanner;
}

@_cdecl("GKAchievement_SetShowCompletionBanner")
public func GKAchievement_SetShowCompletionBanner
(
    pointer: UnsafeMutablePointer<GKAchievement>,
    value : Bool
)
{
    let achievement = pointer.takeUnretainedValue();
    achievement.showsCompletionBanner = value;
}

@_cdecl("GKAchievement_Report")
public func GKAchievement_Report
(
    taskId: Int64,
    pointer: UnsafeMutablePointer<NSArray>,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let array = pointer.takeUnretainedValue() as! [GKAchievement];
    GKAchievement.report(array, withCompletionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId);
    });
}

@_cdecl("GKAchievement_ResetAchievements")
public func GKAchievement_ResetAchievements
(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    GKAchievement.resetAchievements(completionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }
        
        onSuccess(taskId);
    });
}

@_cdecl("GKAchievement_LoadAchievements")
public func GKAchievement_LoadAchievements
(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<NSArray>, // NSArray<GKAchievement>
    onError: @escaping NSErrorTaskCallback
)
{
    GKAchievement.loadAchievements(completionHandler: {achievements, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, (achievements as? NSArray)?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKAchievement_SelectChallengeablePlayers")
public func GKAchievement_SelectChallengeablePlayers
(
    pointer: UnsafeMutablePointer<GKAchievement>,
    taskId: Int64,
    playersPtr: UnsafeMutablePointer<NSArray>, // NSArray<GKPlayer>
    onSuccess: @escaping SuccessTaskPtrCallback<NSArray>, // NSArray<GKPlayer>
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    let players = playersPtr.takeUnretainedValue() as! [GKPlayer];
    target.selectChallengeablePlayers(players, withCompletionHandler: { players, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }
        
        onSuccess(taskId, (players as? NSArray)?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKAchievement_ChallengeComposeController")
public func GKAchievement_ChallengeComposeController
(
    pointer: UnsafeMutablePointer<GKAchievement>,
    message: char_p,
    playersPtr: UnsafeMutablePointer<NSArray>
)
{
    let target = pointer.takeUnretainedValue();
    let players = playersPtr.takeUnretainedValue() as! [GKPlayer];

#if os(visionOS)
    // Avoid including deprecated version of the API in visionOS builds.
    target.challengeComposeController(withMessage: message.toString(), players: players, completion: nil)
#else
    if #available(iOS 17.0, tvOS 17.0, macOS 14.0, *) {
        target.challengeComposeController(withMessage: message.toString(), players: players, completion: nil)
    } else {
        // Explicitly set completionHandler parameter to avoid ambiguous call when building for visionOS
        target.challengeComposeController(withMessage: message.toString(), players: players)
    };
#endif
}
