//
//  GKGameActivity.swift
//  GameKitWrapper
//
//  Copyright © 2025 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKGameActivity_GetAchievementsAsArray")
public func GKGameActivity_GetAchievementsAsArray
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
) -> UnsafeMutablePointer<NSArray> // NSArray<GKAchievement>
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        // Repackage in NSArray since the C# layer doesn't wrap NSSet.
        return ((thisObj.achievements as NSSet).allObjects as NSArray).passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_GetActivityDefinition")
public func GKGameActivity_GetActivityDefinition
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
) -> UnsafeMutableRawPointer // GKGameActivityDefinition
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        return thisObj.activityDefinition.passRetainedUnsafeMutableRawPointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_CheckPendingGameActivityExistence")
public func GKGameActivity_CheckPendingGameActivityExistence
(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskBoolCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        GKGameActivity.checkPendingGameActivityExistence(completionHandler: { result in
            onSuccess(taskId, result);
        });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKGameActivity_GetCreationDate")
public func GKGameActivity_GetCreationDate
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
) -> TimeInterval // aka Double
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        return thisObj.creationDate.timeIntervalSince1970;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_GetDuration")
public func GKGameActivity_GetDuration
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
) -> TimeInterval // aka Double
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        return thisObj.duration;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_End")
public func GKGameActivity_End
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
)
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        thisObj.end();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_GetEndDate")
public func GKGameActivity_GetEndDate
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
) -> TimeInterval // aka Double
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        return thisObj.endDate?.timeIntervalSince1970 ?? 0.0;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_FindMatch")
public func GKGameActivity_FindMatch
(
    thisPtr: UnsafeMutableRawPointer, // GKGameActivity
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<GKMatch>,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        thisObj.findMatch(completionHandler: { match, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, match?.passRetainedUnsafeMutablePointer());
        });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKGameActivity_FindPlayersForHostedMatch")
public func GKGameActivity_FindPlayersForHostedMatch
(
    thisPtr: UnsafeMutableRawPointer, // GKGameActivity
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<NSArray>, // NSArray<GKPlayer>
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        thisObj.findPlayersForHostedMatch(completionHandler: { players, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, (players as? NSArray)?.passRetainedUnsafeMutablePointer());
        });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKGameActivity_GetProgressOnAchievement")
public func GKGameActivity_GetProgressOnAchievement
(
    thisPtr: UnsafeMutableRawPointer, // GKGameActivity
    achievementPtr: UnsafeMutablePointer<GKAchievement>
) -> Double
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        let achievement = achievementPtr.takeUnretainedValue();
        return thisObj.progress(on: achievement);
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_GetScoreOnLeaderboard")
public func GKGameActivity_GetScoreOnLeaderboard
(
    thisPtr: UnsafeMutableRawPointer, // GKGameActivity
    leaderboardPtr: UnsafeMutablePointer<GKLeaderboard>
) -> UnsafeMutableRawPointer? // GKLeaderboardScore
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        let leaderboard = leaderboardPtr.takeUnretainedValue();
        return thisObj.score(on: leaderboard)?.passRetainedUnsafeMutableRawPointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_GetIdentifier")
public func GKGameActivity_GetIdentifier
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
) -> char_p
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        return thisObj.identifier.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_InitWithDefinition")
public func GKGameActivity_InitWithDefinition
(
    activityDefinitionPtr: UnsafeMutableRawPointer // GKGameActivityDefinition
) -> UnsafeMutableRawPointer
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let activityDefinition: GKGameActivityDefinition = activityDefinitionPtr.takeUnretainedValue();
        return GKGameActivity.init(definition: activityDefinition).passRetainedUnsafeMutableRawPointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_IsValidPartyCode")
public func GKGameActivity_IsValidPartyCode
(
    partyCode: char_p
) -> Bool
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        return GKGameActivity.isValidPartyCode(partyCode.toString());
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_LastResumeDate")
public func GKGameActivity_LastResumeDate
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
) -> TimeInterval // aka Double
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        return thisObj.lastResumeDate?.timeIntervalSince1970 ?? 0.0;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_GetLeaderboardScoresAsArray")
public func GKGameActivity_GetLeaderboardScoresAsArray
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
) -> UnsafeMutablePointer<NSArray> // NSArray<GKLeaderboardScore>
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        // Repackage in NSArray since the C# layer doesn't wrap NSSet.
        return ((thisObj.leaderboardScores as NSSet).allObjects as NSArray).passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_MakeMatchRequest")
public func GKGameActivity_MakeMatchRequest
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
) -> UnsafeMutablePointer<GKMatchRequest>?
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        return thisObj.makeMatchRequest()?.passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_GetPartyCode")
public func GKGameActivity_GetPartyCode
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
) -> char_p?
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        return thisObj.partyCode?.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_GetPartyURL")
public func GKGameActivity_GetPartyURL
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
) -> char_p?
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        return thisObj.partyURL?.absoluteString.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_Pause")
public func GKGameActivity_Pause
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
)
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        thisObj.pause();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_GetProperties")
public func GKGameActivity_GetProperties
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
) -> UnsafeMutablePointer<NSDictionary> // NSDictionary<NSString, NSString>
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        return (thisObj.properties as NSDictionary).passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_SetProperties")
public func GKGameActivity_SetProperties
(
    thisPtr: UnsafeMutableRawPointer, // GKGameActivity
    dictionaryPtr: UnsafeMutablePointer<NSDictionary> // NSDictionary<NSString, NSString>
)
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        let dictionary = dictionaryPtr.takeUnretainedValue() as! [String: String];
        thisObj.properties = dictionary;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_RemoveAchievements")
public func GKGameActivity_RemoveAchievements
(
    thisPtr: UnsafeMutableRawPointer, // GKGameActivity
    arrayPtr: UnsafeMutablePointer<NSArray> // NSArray<GKAchievement>
)
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        let achievements = arrayPtr.takeUnretainedValue() as! [GKAchievement];
        thisObj.removeAchievements(achievements);
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_RemoveScoresFromLeaderboards")
public func GKGameActivity_RemoveScoresFromLeaderboards
(
    thisPtr: UnsafeMutableRawPointer, // GKGameActivity
    arrayPtr: UnsafeMutablePointer<NSArray> // NSArray<GKLeaderboard>
)
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        let leaderboards = arrayPtr.takeUnretainedValue() as! [GKLeaderboard];
        thisObj.removeScores(from: leaderboards);
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_Resume")
public func GKGameActivity_Resume
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
)
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        thisObj.resume();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_SetAchievementCompleted")
public func GKGameActivity_SetAchievementCompleted
(
    thisPtr: UnsafeMutableRawPointer, // GKGameActivity
    achievementPtr: UnsafeMutablePointer<GKAchievement>
)
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        let achievement = achievementPtr.takeUnretainedValue();
        thisObj.setAchievementCompleted(achievement);
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_SetProgressOnAchievement")
public func GKGameActivity_SetProgressOnAchievement
(
    thisPtr: UnsafeMutableRawPointer, // GKGameActivity
    achievementPtr: UnsafeMutablePointer<GKAchievement>,
    percentComplete: Double
)
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        let achievement = achievementPtr.takeUnretainedValue();
        thisObj.setProgress(on: achievement, to: percentComplete)
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_SetScoreOnLoaderboard")
public func GKGameActivity_SetScoreOnLoaderboard
(
    thisPtr: UnsafeMutableRawPointer, // GKGameActivity
    leaderboardPtr: UnsafeMutablePointer<GKLeaderboard>,
    score: Int
)
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        let leaderboard = leaderboardPtr.takeUnretainedValue();
        thisObj.setScore(on: leaderboard, to: score);
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_SetScoreOnLoaderboardWithContext")
public func GKGameActivity_SetScoreOnLoaderboardWithContext
(
    thisPtr: UnsafeMutableRawPointer, // GKGameActivity
    leaderboardPtr: UnsafeMutablePointer<GKLeaderboard>,
    score: Int,
    context: UInt
)
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        let leaderboard = leaderboardPtr.takeUnretainedValue();
        thisObj.setScore(on: leaderboard, to: score, context: Int(context))
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_Start")
public func GKGameActivity_Start
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
)
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        thisObj.start();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_GetStartDate")
public func GKGameActivity_GetStartDate
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
) -> TimeInterval // aka Double
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        return thisObj.startDate?.timeIntervalSince1970 ?? 0.0;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_StartWithDefinition")
public func GKGameActivity_StartWithDefinition
(
    activityDefinitionPtr: UnsafeMutableRawPointer // GKGameActivityDefinition
) -> UnsafeMutableRawPointer? // GKGameActivity
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let activityDefinition: GKGameActivityDefinition = activityDefinitionPtr.takeUnretainedValue();
        do {
            let activity = try GKGameActivity.start(definition: activityDefinition);
            return activity.passRetainedUnsafeMutableRawPointer();
        } catch {
            DefaultNSErrorHandler.throwError(error as NSError);
        }
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_StartWithDefinitionAndPartyCode")
public func GKGameActivity_StartWithDefinitionAndPartyCode
(
    activityDefinitionPtr: UnsafeMutableRawPointer, // GKGameActivityDefinition
    partyCode: char_p
) -> UnsafeMutableRawPointer? // GKGameActivity
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let activityDefinition: GKGameActivityDefinition = activityDefinitionPtr.takeUnretainedValue();
        do {
            let activity = try GKGameActivity.start(definition: activityDefinition, partyCode: partyCode.toString());
            return activity.passRetainedUnsafeMutableRawPointer();
        } catch {
            DefaultNSErrorHandler.throwError(error as NSError);
        }
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_GetState")
public func GKGameActivity_GetState
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivity
) -> UInt // aka GKGameActivity.State
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        let thisObj: GKGameActivity = thisPtr.takeUnretainedValue();
        return thisObj.state.rawValue;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_GetValidPartyCodeAlphabet")
public func GKGameActivity_GetValidPartyCodeAlphabet
(
) -> UnsafeMutablePointer<NSArray> // NSArray<NSString>
{
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        return (GKGameActivity.validPartyCodeAlphabet as NSArray).passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}
