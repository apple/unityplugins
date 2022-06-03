//
//  GKLeaderboardSet.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKLeaderboardSet_Free")
public func GKLeaderboardSet_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKLeaderboardSet>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKLeaderboardSet_GetTitle")
public func GKLeaderboardSet_GetTitle
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let target = Unmanaged<GKLeaderboardSet>.fromOpaque(pointer).takeUnretainedValue();
    return target.title.toCharPCopy();
}

@_cdecl("GKLeaderboardSet_GetIdentifier")
public func GKLeaderboardSet_GetIdentifier
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let target = Unmanaged<GKLeaderboardSet>.fromOpaque(pointer).takeUnretainedValue();
    return target.identifier?.toCharPCopy();
}

@_cdecl("GKLeaderboardSet_GetGroupIdentifier")
public func GKLeaderboardSet_GetGroupIdentifier
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let target = Unmanaged<GKLeaderboardSet>.fromOpaque(pointer).takeUnretainedValue();
    return target.groupIdentifier?.toCharPCopy();
}

@_cdecl("GKLeaderboardSet_LoadLeaderboards")
public func GKLeaderboardSet_LoadLeaderboards
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKLeaderboardSet>.fromOpaque(pointer).takeUnretainedValue();
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        target.loadLeaderboards(handler: { leaderboards, error in
            if(error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
            
            if(leaderboards != nil) {
                onSuccess(taskId, Unmanaged.passRetained(leaderboards! as NSArray).toOpaque());
            } else {
                onSuccess(taskId, nil);
            }
        })
    } else {
        // TODO: Handle fallback?
        onSuccess(taskId, nil);
    };
}

@_cdecl("GKLeaderboardSet_LoadLeaderboardSets")
public func GKLeaderboardSet_LoadLeaderboardSets
(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    GKLeaderboardSet.loadLeaderboardSets(completionHandler: { sets, error  in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        if(sets != nil) {
            onSuccess(taskId, Unmanaged.passRetained(sets! as NSArray).toOpaque());
        } else {
            onSuccess(taskId, nil);
        }
    });
}

@_cdecl("GKLeaderboardSet_LoadImage")
public func GKLeaderboardSet_LoadImage
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onImageLoaded: @escaping SuccessTaskImageCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKLeaderboardSet>.fromOpaque(pointer).takeUnretainedValue();
    
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
