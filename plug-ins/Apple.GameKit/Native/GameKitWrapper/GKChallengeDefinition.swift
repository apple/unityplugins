//
//  GKChallengeDefinition.swift
//  GameKitWrapper
//
//  Copyright © 2025 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKChallengeDefinition_GetDetails")
public func GKChallengeDefinition_GetDetails
(
    thisPtr: UnsafeMutableRawPointer // GKChallengeDefinition
) -> char_p?
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKChallengeDefinition = thisPtr.takeUnretainedValue();
        return thisObj.details?.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKChallengeDefinition_GetDurationOptions")
public func GKChallengeDefinition_GetDurationOptions
(
    thisPtr: UnsafeMutableRawPointer // GKChallengeDefinition
) -> UnsafeMutablePointer<NSArray>
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKChallengeDefinition = thisPtr.takeUnretainedValue();
        return (thisObj.durationOptions as NSArray).passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKChallengeDefinition_GetGroupIdentifier")
public func GKChallengeDefinition_GetGroupIdentifier
(
    thisPtr: UnsafeMutableRawPointer // GKChallengeDefinition
) -> char_p?
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKChallengeDefinition = thisPtr.takeUnretainedValue();
        return thisObj.groupIdentifier?.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKChallengeDefinition_HasActiveChallenges")
public func GKChallengeDefinition_HasActiveChallenges
(
    thisPtr: UnsafeMutableRawPointer, // GKChallengeDefinition
    taskId: Int64,
    onSuccess: @escaping SuccessTaskBoolCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKChallengeDefinition = thisPtr.takeUnretainedValue();
        thisObj.hasActiveChallenges(completionHandler: { hasActiveChallenges, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, hasActiveChallenges);
        });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKChallengeDefinition_GetIdentifier")
public func GKChallengeDefinition_GetIdentifier
(
    thisPtr: UnsafeMutableRawPointer // GKChallengeDefinition
) -> char_p
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKChallengeDefinition = thisPtr.takeUnretainedValue();
        return thisObj.identifier.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKChallengeDefinition_GetIsRepeatable")
public func GKChallengeDefinition_GetIsRepeatable
(
    thisPtr: UnsafeMutableRawPointer // GKChallengeDefinition
) -> Bool
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKChallengeDefinition = thisPtr.takeUnretainedValue();
        return thisObj.isRepeatable;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKChallengeDefinition_GetLeaderboard")
public func GKChallengeDefinition_GetLeaderboard
(
    thisPtr: UnsafeMutableRawPointer // GKChallengeDefinition
) -> UnsafeMutablePointer<GKLeaderboard>?
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKChallengeDefinition = thisPtr.takeUnretainedValue();
        return thisObj.leaderboard?.passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKChallengeDefinition_LoadChallengeDefinitions")
public func GKChallengeDefinition_LoadChallengeDefinitions(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<NSArray>, // NSArray<GKChallengeDefinition>
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        GKChallengeDefinition.loadChallengeDefinitions(completionHandler: { challengeDefinitions, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, (challengeDefinitions as? NSArray)?.passRetainedUnsafeMutablePointer());
        });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKChallengeDefinition_LoadImage")
public func GKChallengeDefinition_LoadImage
(
    thisPtr: UnsafeMutableRawPointer, // GKChallengeDefinition
    taskId: Int64,
    onSuccess: @escaping SuccessTaskImageCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKChallengeDefinition = thisPtr.takeUnretainedValue();
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

@_cdecl("GKChallengeDefinition_GetReleaseState")
public func GKChallengeDefinition_GetReleaseState
(
    thisPtr: UnsafeMutablePointer<GKAchievementDescription>
) -> UInt /* aka GKReleaseState */
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj = thisPtr.takeUnretainedValue();
        return thisObj.releaseState.rawValue;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKChallengeDefinition_GetTitle")
public func GKChallengeDefinition_GetTitle
(
    thisPtr: UnsafeMutableRawPointer // GKChallengeDefinition
) -> char_p
{
    if #available(iOS 26.0, macOS 26.0, tvOS 26.0, visionOS 26.0, *) {
        let thisObj: GKChallengeDefinition = thisPtr.takeUnretainedValue();
        return thisObj.title.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}
