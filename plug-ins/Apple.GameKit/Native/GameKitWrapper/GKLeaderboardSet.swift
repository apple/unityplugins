//
//  GKLeaderboardSet.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKLeaderboardSet_GetTitle")
public func GKLeaderboardSet_GetTitle
(
    pointer: UnsafeMutablePointer<GKLeaderboardSet>
) -> char_p
{
    let target = pointer.takeUnretainedValue();
    return target.title.toCharPCopy();
}

@_cdecl("GKLeaderboardSet_GetIdentifier")
public func GKLeaderboardSet_GetIdentifier
(
    pointer: UnsafeMutablePointer<GKLeaderboardSet>
) -> char_p?
{
    let target = pointer.takeUnretainedValue();
    return target.identifier?.toCharPCopy();
}

@_cdecl("GKLeaderboardSet_GetGroupIdentifier")
public func GKLeaderboardSet_GetGroupIdentifier
(
    pointer: UnsafeMutablePointer<GKLeaderboardSet>
) -> char_p?
{
    let target = pointer.takeUnretainedValue();
    return target.groupIdentifier?.toCharPCopy();
}

@_cdecl("GKLeaderboardSet_LoadLeaderboards")
public func GKLeaderboardSet_LoadLeaderboards
(
    pointer: UnsafeMutablePointer<GKLeaderboardSet>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<NSArray>, // NSArray<GKLeaderboard>
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let target = pointer.takeUnretainedValue();
        target.loadLeaderboards(handler: { leaderboards, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, (leaderboards as? NSArray)?.passRetainedUnsafeMutablePointer());
        })
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    };
}

@_cdecl("GKLeaderboardSet_LoadLeaderboardSets")
public func GKLeaderboardSet_LoadLeaderboardSets
(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<NSArray>, // NSArray<GKLeaderboardSet>
    onError: @escaping NSErrorTaskCallback
)
{
    GKLeaderboardSet.loadLeaderboardSets(completionHandler: { sets, error  in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, (sets as? NSArray)?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKLeaderboardSet_LoadImage")
public func GKLeaderboardSet_LoadImage
(
    pointer: UnsafeMutablePointer<GKLeaderboardSet>,
    taskId: Int64,
    onImageLoaded: @escaping SuccessTaskImageCallback,
    onError: @escaping NSErrorTaskCallback
)
{
#if !os(tvOS)
    let target = pointer.takeUnretainedValue();
    target.loadImage(completionHandler: { (image, error) in

        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        // Prior to iOS 14, loadImage can return nil if no image is set on App Store Connect.
        // >= iOS 14, loadImage returns GKErrorCommunicationsFailure if no image is defined.
        let data = image?.pngData() as? NSData;
        onImageLoaded(taskId, data?.passRetainedUnsafeMutablePointer());
    });
#else
    onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
#endif
}
