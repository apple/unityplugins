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
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let target = Unmanaged<GKLeaderboard>.fromOpaque(pointer).takeUnretainedValue();
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        return target.baseLeaderboardID.toCharPCopy()
    } else {
        return target.identifier?.toCharPCopy();
    };
}

@_cdecl("GKLeaderboard_GetTitle")
public func GKLeaderboard_GetTitle
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let target = Unmanaged<GKLeaderboard>.fromOpaque(pointer).takeUnretainedValue();
    return target.title?.toCharPCopy();
}

@_cdecl("GKLeaderboard_GetType")
public func GKLeaderboard_GetType
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKLeaderboard>.fromOpaque(pointer).takeUnretainedValue();
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        return target.type.rawValue
    } else {
        return 0;
    };
}

@_cdecl("GKLeaderboard_GetGroupIdentifier")
public func GKLeaderboard_GetGroupIdentifier
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let target = Unmanaged<GKLeaderboard>.fromOpaque(pointer).takeUnretainedValue();
    return target.groupIdentifier?.toCharPCopy();
}

@_cdecl("GKLeaderboard_GetStartDate")
public func GKLeaderboard_GetStartDate
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKLeaderboard>.fromOpaque(pointer).takeUnretainedValue();
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        return Int(target.startDate?.timeIntervalSince1970 ?? 0)
    } else {
        return 0;
    };
}

@_cdecl("GKLeaderboard_GetNextStartDate")
public func GKLeaderboard_GetNextStartDate
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKLeaderboard>.fromOpaque(pointer).takeUnretainedValue();
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        return Int(target.nextStartDate?.timeIntervalSince1970 ?? 0)
    } else {
        return 0;
    };
}

@_cdecl("GKLeaderboard_GetDuration")
public func GKLeaderboard_GetDuration
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKLeaderboard>.fromOpaque(pointer).takeUnretainedValue();
    
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        return Int(target.duration);
    } else {
        return 0;
    };
}

@_cdecl("GKLeaderboard_LoadPreviousOccurrence")
public func GKLeaderboard_LoadPreviousOccurrence
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKLeaderboard>.fromOpaque(pointer).takeUnretainedValue();
    guard #available(iOS 14, tvOS 14, macOS 11.0, *) else {
        // TODO: Handle this fallback?
        onSuccess(taskId, nil);
    }

    target.loadPreviousOccurrence(completionHandler: { leaderboard, error in
        if (error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }

        onSuccess(taskId, leaderboard.map { Unmanaged.passRetained($0).toOpaque() });
    });
}

public typealias GKLeaderboardLoadEntriesCallback = @convention(c) (Int64, UnsafeMutableRawPointer?, UnsafeMutableRawPointer?, Int) -> Void;

@_cdecl("GKLeaderboard_LoadEntries")
public func GKLeaderboard_LoadEntries
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    playerScope: Int,
    timeScope: Int,
    rankMin: Int,
    rankMax: Int,
    onSuccess: @escaping GKLeaderboardLoadEntriesCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKLeaderboard>.fromOpaque(pointer).takeUnretainedValue();
    let gkRange = NSMakeRange(rankMin, (rankMax + 1) - rankMin);

    guard #available(iOS 14, tvOS 14, macOS 11.0, *) else {
        // TODO: Handle fallback?
        onSuccess(taskId, nil, nil, 0);
    }

    target.loadEntries(
       for: GKLeaderboard.PlayerScope(rawValue: playerScope)!,
       timeScope: GKLeaderboard.TimeScope(rawValue: timeScope)!,
       range: gkRange,
       completionHandler: { localPlayerEntry, entries, totalPlayerCount, error in
           if (error != nil) {
               onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
               return;
           }

           let localPtr = localPlayerEntry.map { Unmanaged.passRetained($0).toOpaque() }
           let entriesPtr = entries.map { Unmanaged.passRetained($0 as NSArray).toOpaque() }

           onSuccess(taskId, localPtr, entriesPtr, totalPlayerCount);
       });
}

public typealias GKLeaderboardLoadEntriesForPlayersCallback = @convention(c) (Int64, UnsafeMutableRawPointer?, UnsafeMutableRawPointer?) -> Void;

@_cdecl("GKLeaderboard_LoadEntriesForPlayers")
public func GKLeaderboard_LoadEntriesForPlayers
(
    gkLeaderboardPtr: UnsafeMutableRawPointer,
    taskId: Int64,
    playersPtr: UnsafeMutableRawPointer, // NSArray<GKPlayer *> *
    timeScope: Int, // GKLeaderboardTimeScope
    onSuccess: @escaping GKLeaderboardLoadEntriesForPlayersCallback,
    onError: @escaping NSErrorCallback
)
{
    let gkLeaderboard = Unmanaged<GKLeaderboard>.fromOpaque(gkLeaderboardPtr).takeUnretainedValue();
    let players = Unmanaged<NSArray>.fromOpaque(playersPtr).takeUnretainedValue() as! [GKPlayer];

    guard #available(iOS 14, tvOS 14, macOS 11.0, *) else {
        // TODO: Handle fallback?
        onSuccess(taskId, nil, nil);
    }

    gkLeaderboard.loadEntries(
        for: players,
        timeScope: GKLeaderboard.TimeScope(rawValue: timeScope)!,
        completionHandler: { localPlayerEntry, entries, error in
            if (error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }

            let localPtr = localPlayerEntry.map { Unmanaged.passRetained($0).toOpaque() }
            let entriesPtr = entries.map { Unmanaged.passRetained($0 as NSArray).toOpaque() }

            onSuccess(taskId, localPtr, entriesPtr);
        });
}

@_cdecl("GKLeaderboard_LoadImage")
public func GKLeaderboard_LoadImage
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onImageLoaded: @escaping SuccessTaskImageCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKLeaderboard>.fromOpaque(pointer).takeUnretainedValue();
    #if !os(tvOS)
    target.loadImage(completionHandler: { (image, error) in
        if (error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }

        let data = image!.pngData()!;
        onImageLoaded(
            taskId,
            Unmanaged.passRetained(data as NSData).toOpaque());
    });
    #else
    let error = NSError(domain: "GameKit", code: GKErrorCodeExtension.unsupportedOperationForOSVersion.rawValue, userInfo: nil);
    onError(taskId, Unmanaged.passRetained(error).toOpaque());
    #endif
}

@_cdecl("GKLeaderboard_SubmitScore")
public func GKLeaderboard_SubmitScore
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    score: Int,
    context: Int,
    player: UnsafeMutableRawPointer,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    let player = Unmanaged<GKPlayer>.fromOpaque(player).takeUnretainedValue();
    let target = Unmanaged<GKLeaderboard>.fromOpaque(pointer).takeUnretainedValue();

    guard #available(iOS 14, tvOS 14, macOS 11.0, *) else {
        // TODO: Do we want to suppor this fallback?
        onSuccess(taskId);
    }

    target.submitScore(score, context: context, player: player, completionHandler: { error in
        if (error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }

        onSuccess(taskId);
    });
}

@_cdecl("GKLeaderboard_LoadLeaderboards")
public func GKLeaderboard_LoadLeaderboards
(
    pointer: UnsafeMutableRawPointer?,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let ids = pointer.map { Unmanaged<NSArray>.fromOpaque($0).takeUnretainedValue() as! [String] };

    guard #available(iOS 14, tvOS 14, macOS 11.0, *) else {
        // TODO: Handle this fallback?
        onSuccess(taskId, nil);
    }

    GKLeaderboard.loadLeaderboards(IDs: ids, completionHandler: { leaderboards, error in
        if (error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }

        onSuccess(taskId, leaderboards.map { Unmanaged.passRetained($0 as NSArray).toOpaque() });
    });
}
