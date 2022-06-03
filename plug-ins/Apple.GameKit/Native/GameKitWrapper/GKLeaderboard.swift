//
//  GKLeaderboard.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKLeaderboard_Free")
public func GKLeaderboard_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKLeaderboard>.fromOpaque(pointer).autorelease();
}

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
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        target.loadPreviousOccurrence(completionHandler: { leaderboard, error in
            if(error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
            
            if(leaderboard != nil) {
                onSuccess(taskId, Unmanaged.passRetained(leaderboard!).toOpaque());
            } else {
                onSuccess(taskId, nil);
            }
            
        })
    } else {
        // TODO: Handle this fallback?
        onSuccess(taskId, nil);
    };
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
    
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        target.loadEntries(
            for: GKLeaderboard.PlayerScope.init(rawValue: playerScope)!,
               timeScope: GKLeaderboard.TimeScope.init(rawValue: timeScope)!,
               range: gkRange,
               completionHandler: { localPlayerEntry, entries, totalPlayerCount, error in
                   if(error != nil) {
                       onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                       return;
                   }
                   
                   let localPtr = localPlayerEntry != nil ? Unmanaged.passRetained(localPlayerEntry!).toOpaque() : nil;
                   let entriesPtr = entries != nil ? Unmanaged.passRetained(entries! as NSArray).toOpaque() : nil;
                   
                   onSuccess(taskId, localPtr, entriesPtr, totalPlayerCount);
               })
    } else {
        // TODO: Handle fallback?
        onSuccess(taskId, nil, nil, 0);
    };
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
            if(error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
        
            let data = image!.pngData()!;
            onImageLoaded(
                taskId,
                Int32(image!.size.width),
                Int32(image!.size.height),
                data.toUCharP(),
                Int32(data.count));
    });
    #else
    let error = NSError(domain: "GameKit", code: -7, userInfo: nil);
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
    
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        target.submitScore(score, context: context, player: player, completionHandler: { error in
            if(error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
            
            onSuccess(taskId);
        })
    } else {
        // TODO: Do we want to suppor this fallback?
        onSuccess(taskId);
    };
}

@_cdecl("GKLeaderboard_LoadLeaderboards")
public func GKLeaderboard_LoadLeaderboards
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let ids = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue() as! [String];
    
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        GKLeaderboard.loadLeaderboards(IDs: ids, completionHandler: { leaderboards, error in
            if(error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
            
            if(leaderboards != nil) {
                onSuccess(taskId, Unmanaged.passRetained(leaderboards! as NSArray).toOpaque());
            } else {
                onSuccess(taskId, nil);
            }
        });
    } else {
        // TODO: Handle this fallback?
        onSuccess(taskId, nil);
    };
}
