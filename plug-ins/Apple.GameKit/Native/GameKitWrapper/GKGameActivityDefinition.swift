//
//  GKGameActivityDefinition.swift
//  GameKitWrapper
//
//  Copyright © 2025 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKGameActivityDefinition_GetDefaultProperties")
public func GKGameActivityDefinition_GetDefaultProperties
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivityDefinition
) -> UnsafeMutablePointer<NSDictionary> // NSDictionary<NSString, NSString>
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        return (thisObj.defaultProperties as NSDictionary).passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivityDefinition_Details")
public func GKGameActivityDefinition_Details
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivityDefinition
) -> char_p?
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        return thisObj.details?.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivityDefinition_GetFallbackURL")
public func GKGameActivityDefinition_GetFallbackURL
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivityDefinition
) -> char_p?
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        return thisObj.fallbackURL?.absoluteString.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivityDefinition_GetGroupIdentifier")
public func GKGameActivityDefinition_GetGroupIdentifier
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivityDefinition
) -> char_p?
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        return thisObj.groupIdentifier?.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivityDefinition_GetIdentifier")
public func GKGameActivityDefinition_GetIdentifier
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivityDefinition
) -> char_p
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        return thisObj.identifier.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivityDefinition_LoadAchievementDescriptions")
public func GKGameActivityDefinition_LoadAchievementDescriptions
(
    thisPtr: UnsafeMutableRawPointer, // GKGameActivityDefinition
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<NSArray>, // NSArray<GKAchievementDescription>
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        thisObj.loadAchievementDescriptions(completionHandler: { achievementDescriptions, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, (achievementDescriptions as? NSArray)?.passRetainedUnsafeMutablePointer());
        });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKGameActivityDefinition_LoadGameActivityDefinitions")
public func GKGameActivityDefinition_LoadGameActivityDefinitions
(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<NSArray>, // NSArray<GKGameActivityDefinition>
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        GKGameActivityDefinition.loadGameActivityDefinitions(completionHandler: { activityDefinitions, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, (activityDefinitions as? NSArray)?.passRetainedUnsafeMutablePointer());
        });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKGameActivityDefinition_LoadGameActivityDefinitionsWithIDs")
public func GKGameActivityDefinition_LoadGameActivityDefinitionsWithIDs
(
    taskId: Int64,
    activityDefinitionIDsPtr: UnsafeMutablePointer<NSArray>, // NSArray<NSString>
    onSuccess: @escaping SuccessTaskPtrCallback<NSArray>, // NSArray<GKGameActivityDefinition>
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let activityDefinitionIDs = activityDefinitionIDsPtr.takeUnretainedValue() as! [String];
        GKGameActivityDefinition.loadGameActivityDefinitions(IDs: activityDefinitionIDs, completionHandler: { activityDefinitions, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, (activityDefinitions as? NSArray)?.passRetainedUnsafeMutablePointer());
        });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKGameActivityDefinition_LoadImage")
public func GKGameActivityDefinition_LoadImage
(
    thisPtr: UnsafeMutableRawPointer, // GKGameActivityDefinition
    taskId: Int64,
    onSuccess: @escaping SuccessTaskImageCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        thisObj.loadImage(completionHandler: { image, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            let data = image?.pngData() as? NSData;
            onSuccess(taskId, data?.passRetainedUnsafeMutablePointer());
        });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKGameActivityDefinition_LoadLeaderboards")
public func GKGameActivityDefinition_LoadLeaderboards
(
    thisPtr: UnsafeMutableRawPointer, // GKGameActivityDefinition
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<NSArray>, // NSArray<GKLeaderboard>
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        thisObj.loadLeaderboards(completionHandler: { leaderboards, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, (leaderboards as? NSArray)?.passRetainedUnsafeMutablePointer());
        });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKGameActivityDefinition_GetMaxPlayers")
public func GKGameActivityDefinition_GetMaxPlayers
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivityDefinition
) -> UnsafeMutablePointer<NSNumber>?
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        return thisObj.__maxPlayers?.passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivityDefinition_GetMinPlayers")
public func GKGameActivityDefinition_GetMinPlayers
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivityDefinition
) -> UnsafeMutablePointer<NSNumber>?
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        return thisObj.__minPlayers?.passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivityDefinition_GetPlayStyle")
public func GKGameActivityDefinition_GetPlayStyle
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivityDefinition
) -> Int // aka GKGameActivityPlayStyle
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        return thisObj.playStyle.rawValue;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivityDefinition_GetReleaseState")
public func GKGameActivityDefinition_GetReleaseState
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivityDefinition
) -> UInt // aka GKReleaseState
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        return thisObj.releaseState.rawValue;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivityDefinition_GetSupportsPartyCode")
public func GKGameActivityDefinition_GetSupportsPartyCode
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivityDefinition
) -> Bool
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        return thisObj.supportsPartyCode;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivityDefinition_GetSupportsUnlimitedPlayers")
public func GKGameActivityDefinition_GetSupportsUnlimitedPlayers
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivityDefinition
) -> Bool
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        return thisObj.supportsUnlimitedPlayers;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivityDefinition_GetTitle")
public func GKGameActivityDefinition_GetTitle
(
    thisPtr: UnsafeMutableRawPointer // GKGameActivityDefinition
) -> char_p
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKGameActivityDefinition = thisPtr.takeUnretainedValue();
        return thisObj.title.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}
