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
) -> UnsafeMutableRawPointer
{
    let achievement = GKAchievement.init(identifier: identifier.toString());
    return Unmanaged.passRetained(achievement).toOpaque();
}

@_cdecl("GKAchievement_Free")
public func GKAchievement_Free
(
    pointer : UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKAchievement>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKAchievement_GetIdentifier")
public func GKAchievement_GetIdentifier
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let achievement = Unmanaged<GKAchievement>.fromOpaque(pointer).takeUnretainedValue();
    return achievement.identifier.toCharPCopy();
}

@_cdecl("GKAchievement_SetIdentifier")
public func GKAchievement_SetIdentifier
(
    pointer: UnsafeMutableRawPointer,
    value: char_p
)
{
    let achievement = Unmanaged<GKAchievement>.fromOpaque(pointer).takeUnretainedValue();
    achievement.identifier = value.toString();
}

@_cdecl("GKAchievement_GetPlayer")
public func GKAchievement_GetPlayer
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let achievement = Unmanaged<GKAchievement>.fromOpaque(pointer).takeUnretainedValue();
    return Unmanaged.passRetained(achievement.player).toOpaque();
}

@_cdecl("GKAchievement_GetPercentComplete")
public func GKAchievement_GetPercentComplete
(
    pointer: UnsafeMutableRawPointer
) -> Float
{
    let achievement = Unmanaged<GKAchievement>.fromOpaque(pointer).takeUnretainedValue();
    return Float(achievement.percentComplete);
}

@_cdecl("GKAchievement_SetPercentComplete")
public func GKAchievement_SetPercentComplete
(
    pointer: UnsafeMutableRawPointer,
    value: Float
)
{
    let achievement = Unmanaged<GKAchievement>.fromOpaque(pointer).takeUnretainedValue();
    achievement.percentComplete = Double(value);
}

@_cdecl("GKAchievement_GetIsCompleted")
public func GKAchievement_GetIsCompleted
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let achievement = Unmanaged<GKAchievement>.fromOpaque(pointer).takeUnretainedValue();
    return achievement.isCompleted;
}

@_cdecl("GKAchievement_GetLastReportedDate")
public func GKAchievement_GetLastReportedDate
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let achievement = Unmanaged<GKAchievement>.fromOpaque(pointer).takeUnretainedValue();
    return Int(achievement.lastReportedDate.timeIntervalSince1970);
}

@_cdecl("GKAchievement_GetShowCompletionBanner")
public func GKAchievement_GetShowCompletionBanner
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let achievement = Unmanaged<GKAchievement>.fromOpaque(pointer).takeUnretainedValue();
    return achievement.showsCompletionBanner;
}

@_cdecl("GKAchievement_SetShowCompletionBanner")
public func GKAchievement_SetShowCompletionBanner
(
    pointer: UnsafeMutableRawPointer,
    value : Bool
)
{
    let achievement = Unmanaged<GKAchievement>.fromOpaque(pointer).takeUnretainedValue();
    achievement.showsCompletionBanner = value;
}

@_cdecl("GKAchievement_Report")
public func GKAchievement_Report
(
    taskId: Int64,
    pointer: UnsafeMutableRawPointer,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue() as! [GKAchievement];
    GKAchievement.report(array, withCompletionHandler: {error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
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
    onError: @escaping NSErrorCallback
)
{
    GKAchievement.resetAchievements(completionHandler: { error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId);
    });
}

@_cdecl("GKAchievement_LoadAchievements")
public func GKAchievement_LoadAchievements
(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    GKAchievement.loadAchievements(completionHandler: {achievements, error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        if(achievements != nil) {
            onSuccess(taskId, Unmanaged.passRetained(achievements! as NSArray).toOpaque());
        } else {
            onSuccess(taskId, nil);
        }
    });
}

@_cdecl("GKAchievement_SelectChallengeablePlayers")
public func GKAchievement_SelectChallengeablePlayers
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    playersPtr: UnsafeMutableRawPointer,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKAchievement>.fromOpaque(pointer).takeUnretainedValue();
    let players = Unmanaged<NSArray>.fromOpaque(playersPtr).takeUnretainedValue() as! [GKPlayer];
    target.selectChallengeablePlayers(players, withCompletionHandler: { players, error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        if(players != nil) {
            onSuccess(taskId, Unmanaged.passRetained(players! as NSArray).toOpaque());
        } else {
            onSuccess(taskId, nil);
        }
    });
}

@_cdecl("GKAchievement_ChallengeComposeController")
public func GKAchievement_ChallengeComposeController
(
    pointer: UnsafeMutableRawPointer,
    message: char_p,
    playersPtr: UnsafeMutableRawPointer
)
{
    let target = Unmanaged<GKAchievement>.fromOpaque(pointer).takeUnretainedValue();
    let players = Unmanaged<NSArray>.fromOpaque(playersPtr).takeUnretainedValue() as! [GKPlayer];
    target.challengeComposeController(withMessage: message.toString(), players: players);
}
