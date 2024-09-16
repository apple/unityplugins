//
//  GKLeaderboard.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKLeaderboard_GetBaseLeaderboardId")
public func GKLeaderboard_GetBaseLeaderboardId
(
    pointer: UnsafeMutablePointer<GKLeaderboard>
) -> char_p?
{
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let target = pointer.takeUnretainedValue();
        return target.baseLeaderboardID.toCharPCopy()
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    };
}

@_cdecl("GKLeaderboard_GetTitle")
public func GKLeaderboard_GetTitle
(
    pointer: UnsafeMutablePointer<GKLeaderboard>
) -> char_p?
{
    let target = pointer.takeUnretainedValue();
    return target.title?.toCharPCopy();
}

@_cdecl("GKLeaderboard_GetType")
public func GKLeaderboard_GetType
(
    pointer: UnsafeMutablePointer<GKLeaderboard>
) -> Int
{
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let target = pointer.takeUnretainedValue();
        return target.type.rawValue
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    };
}

@_cdecl("GKLeaderboard_GetGroupIdentifier")
public func GKLeaderboard_GetGroupIdentifier
(
    pointer: UnsafeMutablePointer<GKLeaderboard>
) -> char_p?
{
    let target = pointer.takeUnretainedValue();
    return target.groupIdentifier?.toCharPCopy();
}

@_cdecl("GKLeaderboard_GetStartDate")
public func GKLeaderboard_GetStartDate
(
    pointer: UnsafeMutablePointer<GKLeaderboard>
) -> TimeInterval // aka Double
{

    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let target = pointer.takeUnretainedValue();
        return target.startDate?.timeIntervalSince1970 ?? 0.0;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    };
}

@_cdecl("GKLeaderboard_GetNextStartDate")
public func GKLeaderboard_GetNextStartDate
(
    pointer: UnsafeMutablePointer<GKLeaderboard>
) -> TimeInterval // aka Double
{
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let target = pointer.takeUnretainedValue();
        return target.nextStartDate?.timeIntervalSince1970 ?? 0.0;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    };
}

@_cdecl("GKLeaderboard_GetDuration")
public func GKLeaderboard_GetDuration
(
    pointer: UnsafeMutablePointer<GKLeaderboard>
) -> Int
{
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let target = pointer.takeUnretainedValue();
        return Int(target.duration);
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    };
}

@_cdecl("GKLeaderboard_LoadPreviousOccurrence")
public func GKLeaderboard_LoadPreviousOccurrence
(
    pointer: UnsafeMutablePointer<GKLeaderboard>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let target = pointer.takeUnretainedValue();
        target.loadPreviousOccurrence(completionHandler: { leaderboard, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, leaderboard?.passRetainedUnsafeMutablePointer());
        });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

public typealias GKLeaderboardLoadEntriesCallback = @convention(c) (
    Int64,
    UnsafeMutableRawPointer? /*GKLeaderboard.Entry*/,
    UnsafeMutablePointer<NSArray>?,
    Int) -> Void;

@_cdecl("GKLeaderboard_LoadEntries")
public func GKLeaderboard_LoadEntries
(
    pointer: UnsafeMutablePointer<GKLeaderboard>,
    taskId: Int64,
    playerScope: Int,
    timeScope: Int,
    rankMin: Int,
    rankMax: Int,
    onSuccess: @escaping GKLeaderboardLoadEntriesCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let target = pointer.takeUnretainedValue();
        let gkRange = NSMakeRange(rankMin, (rankMax + 1) - rankMin);

        target.loadEntries(
            for: GKLeaderboard.PlayerScope(rawValue: playerScope)!,
            timeScope: GKLeaderboard.TimeScope(rawValue: timeScope)!,
            range: gkRange,
            completionHandler: { localPlayerEntry, entries, totalPlayerCount, error in
                if let error = error as? NSError {
                    onError(taskId, error.passRetainedUnsafeMutablePointer());
                    return;
                }

                let localPtr = localPlayerEntry?.passRetainedUnsafeMutableRawPointer();
                let entriesPtr = (entries as? NSArray)?.passRetainedUnsafeMutablePointer();

                onSuccess(taskId,
                          localPtr,
                          entriesPtr,
                          totalPlayerCount);
            });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

public typealias GKLeaderboardLoadEntriesForPlayersCallback = @convention(c) (
    Int64,
    UnsafeMutableRawPointer? /*GKLeaderboard.Entry*/,
    UnsafeMutablePointer<NSArray>?) -> Void;

@_cdecl("GKLeaderboard_LoadEntriesForPlayers")
public func GKLeaderboard_LoadEntriesForPlayers
(
    gkLeaderboardPtr: UnsafeMutablePointer<GKLeaderboard>,
    taskId: Int64,
    playersPtr: UnsafeMutablePointer<NSArray>, // NSArray<GKPlayer *> *
    timeScope: Int, // GKLeaderboardTimeScope
    onSuccess: @escaping GKLeaderboardLoadEntriesForPlayersCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let gkLeaderboard = gkLeaderboardPtr.takeUnretainedValue();
        let players = playersPtr.takeUnretainedValue() as! [GKPlayer];

        gkLeaderboard.loadEntries(
            for: players,
            timeScope: GKLeaderboard.TimeScope(rawValue: timeScope)!,
            completionHandler: { localPlayerEntry, entries, error in
                if let error = error as? NSError {
                    onError(taskId, error.passRetainedUnsafeMutablePointer());
                    return;
                }

                let localPtr = localPlayerEntry?.passRetainedUnsafeMutableRawPointer();
                let entriesPtr = (entries as? NSArray)?.passRetainedUnsafeMutablePointer();

                onSuccess(taskId,
                          localPtr,
                          entriesPtr);
            });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKLeaderboard_LoadImage")
public func GKLeaderboard_LoadImage
(
    pointer: UnsafeMutablePointer<GKLeaderboard>,
    taskId: Int64,
    onImageLoaded: @escaping SuccessTaskImageCallback,
    onError: @escaping NSErrorTaskCallback
)
{
#if !os(tvOS)
    let leaderboard = pointer.takeUnretainedValue();
    leaderboard.loadImage(completionHandler: { (image, error) in

        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        // Prior to iOS 14, loadImage can return nil if no image is set on App Store Connect.
        // At and after iOS 14, loadImage returns GKErrorCommunicationsFailure if no image is defined.
        let data = image?.pngData() as? NSData;
        onImageLoaded(taskId, data?.passRetainedUnsafeMutablePointer());
    });
#else
    onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
#endif
}

@_cdecl("GKLeaderboard_SubmitScore")
public func GKLeaderboard_SubmitScore
(
    pointer: UnsafeMutablePointer<GKLeaderboard>,
    taskId: Int64,
    score: Int,
    context: Int,
    player: UnsafeMutablePointer<GKPlayer>,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let player = player.takeUnretainedValue();
        let target = pointer.takeUnretainedValue();

        target.submitScore(score, context: context, player: player, completionHandler: { error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId);
        });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKLeaderboard_LoadLeaderboards")
public func GKLeaderboard_LoadLeaderboards
(
    pointer: UnsafeMutablePointer<NSArray>?, // NSArray<NSString>
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let ids = pointer?.takeUnretainedValue() as? [String];

        GKLeaderboard.loadLeaderboards(IDs: ids, completionHandler: { leaderboards, error in
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
