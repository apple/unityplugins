//
//  GKMatchmaker.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKMatchmaker_GetShared")
public func GKMatchmaker_GetShared
(
) -> UnsafeMutableRawPointer
{
    let matchmaker = GKMatchmaker.shared();
    return Unmanaged.passRetained(matchmaker).toOpaque();
}

@_cdecl("GKMatchmaker_MatchForInvite")
public func GKMatchmaker_MatchForInvite
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    invitePtr: UnsafeMutableRawPointer,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let matchmaker = Unmanaged<GKMatchmaker>.fromOpaque(pointer).takeUnretainedValue();
    let invite = Unmanaged<GKInvite>.fromOpaque(invitePtr).takeUnretainedValue();

    matchmaker.match(for: invite, completionHandler: { match, error in
        if (error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        let delegate = GKWMatchDelegate();
        match!.delegate = delegate;

        onSuccess(taskId, Unmanaged.passRetained(match!).toOpaque());
    });
}

@_cdecl("GKMatchmaker_FindMatch")
public func GKMatchmaker_FindMatch
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    matchRequestPtr: UnsafeMutableRawPointer,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let matchmaker = Unmanaged<GKMatchmaker>.fromOpaque(pointer).takeUnretainedValue();
    let matchRequest = Unmanaged<GKMatchRequest>.fromOpaque(matchRequestPtr).takeUnretainedValue();

    matchmaker.findMatch(for: matchRequest, withCompletionHandler: { match, error in
        if (error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        let delegate = GKWMatchDelegate();
        match!.delegate = delegate;

        onSuccess(taskId, Unmanaged.passRetained(match!).toOpaque());
    });
}

@_cdecl("GKMatchmaker_FinishMatchmaking")
public func GKMatchmaker_FinishMatchmaking
(
    gkMatchmakerPtr : UnsafeMutableRawPointer,
    gkMatchPtr : UnsafeMutableRawPointer
)
{
    let gkMatchmaker = Unmanaged<GKMatchmaker>.fromOpaque(gkMatchmakerPtr).takeUnretainedValue();
    let gkMatch = Unmanaged<GKMatch>.fromOpaque(gkMatchPtr).takeUnretainedValue();

    gkMatchmaker.finishMatchmaking(for: gkMatch);
}

@_cdecl("GKMatchmaker_Cancel")
public func GKMatchmaker_Cancel
(
    pointer: UnsafeMutableRawPointer
)
{
    let matchmaker = Unmanaged<GKMatchmaker>.fromOpaque(pointer).takeUnretainedValue();
    matchmaker.cancel();
}

@_cdecl("GKMatchmaker_FindPlayers")
public func GKMatchmaker_FindPlayers
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    matchRequestPtr: UnsafeMutableRawPointer,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let matchmaker = Unmanaged<GKMatchmaker>.fromOpaque(pointer).takeUnretainedValue();
    let matchRequest = Unmanaged<GKMatchRequest>.fromOpaque(matchRequestPtr).takeUnretainedValue();
    matchmaker.findPlayers(forHostedRequest: matchRequest, withCompletionHandler: { players, error in
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

@_cdecl("GKMatchMaker_FindMatchedPlayers")
public func GKMatchMaker_FindMatchedPlayers
(
    gkMatchmakerPtr: UnsafeMutableRawPointer,
    taskId: Int64,
    gkMatchRequestPtr: UnsafeMutableRawPointer,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    if #available(iOS 17.2, tvOS 17.2, macOS 14.2, visionOS 1.1, *) {
        let gkMatchMaker = Unmanaged<GKMatchmaker>.fromOpaque(gkMatchmakerPtr).takeUnretainedValue()
        let gkMatchRequest = Unmanaged<GKMatchRequest>.fromOpaque(gkMatchRequestPtr).takeUnretainedValue()

        gkMatchMaker.findMatchedPlayers(gkMatchRequest, withCompletionHandler: { gkMatchedPlayers, error in
            if (error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque())
                return;
            }

            if (gkMatchedPlayers != nil) {
                onSuccess(taskId, Unmanaged.passRetained(gkMatchedPlayers! as GKMatchedPlayers).toOpaque())
            } else {
                onSuccess(taskId, nil);
            }
        })
    } else {
        // API not available
        onError(taskId, Unmanaged.passRetained(NSError.init(domain: "GKMatchmaker", code: GKErrorCodeExtension.unsupportedOperationForOSVersion.rawValue, userInfo: nil)).toOpaque())
    }
}

@_cdecl("GKMatchmaker_AddPlayers")
public func GKMatchmaker_AddPlayers
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    matchPtr: UnsafeMutableRawPointer,
    matchRequestPtr: UnsafeMutableRawPointer,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    let matchmaker = Unmanaged<GKMatchmaker>.fromOpaque(pointer).takeUnretainedValue();
    let match = Unmanaged<GKMatch>.fromOpaque(matchPtr).takeUnretainedValue();
    let matchRequest = Unmanaged<GKMatchRequest>.fromOpaque(matchRequestPtr).takeUnretainedValue();
    
    matchmaker.addPlayers(to: match, matchRequest: matchRequest, completionHandler: { error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId);
    });
}

@_cdecl("GKMatchmaker_QueryActivity")
public func GKMatchmaker_QueryActivity
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskIntCallback,
    onError: @escaping NSErrorCallback
)
{
    let matchmaker = Unmanaged<GKMatchmaker>.fromOpaque(pointer).takeUnretainedValue();
    matchmaker.queryActivity(completionHandler: { numPlayers, error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId, numPlayers);
    });
}

@_cdecl("GKMatchmaker_QueryQueueActivity")
public func GKMatchmaker_QueryQueueActivity
(
    gkMatchmakerPtr: UnsafeMutableRawPointer,
    taskId: Int64,
    queueName: char_p,
    onSuccess: @escaping SuccessTaskIntCallback,
    onError: @escaping NSErrorCallback
)
{
    if #available(iOS 17.2, tvOS 17.2, macOS 14.2, visionOS 1.1, *) {
        let gkMatchmaker = Unmanaged<GKMatchmaker>.fromOpaque(gkMatchmakerPtr).takeUnretainedValue();
        gkMatchmaker.queryQueueActivity(queueName.toString(), withCompletionHandler: { numPlayers, error in
            if (error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }

            onSuccess(taskId, numPlayers);
        });
    } else {
        // API not available
        onError(taskId, Unmanaged.passRetained(NSError.init(domain: "GKMatchmaker", code: GKErrorCodeExtension.unsupportedOperationForOSVersion.rawValue, userInfo: nil)).toOpaque())
    }
}

@_cdecl("GKMatchmaker_QueryPlayerGroupActivity")
public func GKMatchmaker_QueryPlayerGroupActivity
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    groupId: Int,
    onSuccess: @escaping SuccessTaskIntCallback,
    onError: @escaping NSErrorCallback
)
{
    let matchmaker = Unmanaged<GKMatchmaker>.fromOpaque(pointer).takeUnretainedValue();
    matchmaker.queryPlayerGroupActivity(groupId, withCompletionHandler: { numPlayers, error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId, numPlayers);
    });
}

@_cdecl("GKMatchmaker_CancelPendingInvite")
public func GKMatchmaker_CancelPendingInvite
(
    pointer: UnsafeMutableRawPointer,
    playerId: UnsafeMutableRawPointer
)
{
    let matchmaker = Unmanaged<GKMatchmaker>.fromOpaque(pointer).takeUnretainedValue();
    let player = Unmanaged<GKPlayer>.fromOpaque(playerId).takeUnretainedValue();
    
    matchmaker.cancelPendingInvite(to: player);
}

public typealias GKMatchmakerNearbyPlayerReachableHandler = @convention(c) (UnsafeMutableRawPointer /*GKMatchmaker*/, UnsafeMutableRawPointer /*GKPlayer*/, Bool /*reachable*/) -> Void;

@_cdecl("GKMatchmaker_StartBrowsingForNearbyPlayers")
public func GKMatchmaker_StartBrowsingForNearbyPlayers
(
    gkMatchmakerPtr: UnsafeMutableRawPointer,
    nearbyPlayerReachableHandler: @escaping GKMatchmakerNearbyPlayerReachableHandler
)
{
    if #available(iOS 8.0, tvOS 9.0, macOS 10.10, *) {
        let gkMatchmaker = Unmanaged<GKMatchmaker>.fromOpaque(gkMatchmakerPtr).takeUnretainedValue();
        gkMatchmaker.startBrowsingForNearbyPlayers(handler: { gkPlayer, isReachable in
            nearbyPlayerReachableHandler(
                gkMatchmakerPtr,    // not retained as per notes in InteropWeakMap.cs.
                Unmanaged.passRetained(gkPlayer).toOpaque(),
                isReachable);
        });
    }
}

@_cdecl("GKMatchmaker_StopBrowsingForNearbyPlayers")
public func GKMatchmaker_StopBrowsingForNearbyPlayers
(
    gkMatchmakerPtr: UnsafeMutableRawPointer
)
{
    if #available(iOS 6.0, tvOS 9.0, macOS 10.9, *) {
        let gkMatchmaker = Unmanaged<GKMatchmaker>.fromOpaque(gkMatchmakerPtr).takeUnretainedValue();
        gkMatchmaker.stopBrowsingForNearbyPlayers();
    }
}

public typealias GKMatchmakerGroupActivityPlayerHandler = @convention(c) (UnsafeMutableRawPointer /*GKMatchmaker*/, UnsafeMutableRawPointer /*GKPlayer*/) -> Void;

@_cdecl("GKMatchmaker_StartGroupActivity")
public func GKMatchmaker_StartGroupActivity
(
    gkMatchmakerPtr: UnsafeMutableRawPointer,
    groupActivityPlayerHandler: @escaping GKMatchmakerGroupActivityPlayerHandler
)
{
    #if !os(tvOS)
    if #available(iOS 16.2, macOS 13.1, *) {
        let gkMatchmaker = Unmanaged<GKMatchmaker>.fromOpaque(gkMatchmakerPtr).takeUnretainedValue();
        gkMatchmaker.startGroupActivity(playerHandler: { gkPlayer in
            groupActivityPlayerHandler(
                gkMatchmakerPtr,    // not retained as per notes in InteropWeakMap.cs.
                Unmanaged.passRetained(gkPlayer).toOpaque());
        });
    }
    #endif
}

@_cdecl("GKMatchmaker_StopGroupActivity")
public func GKMatchmaker_StopGroupActivity
(
    gkMatchmakerPtr: UnsafeMutableRawPointer
)
{
    #if !os(tvOS)
    if #available(iOS 16.2, macOS 13.1, *) {
        let gkMatchmaker = Unmanaged<GKMatchmaker>.fromOpaque(gkMatchmakerPtr).takeUnretainedValue();
        gkMatchmaker.stopGroupActivity();
    }
    #endif
}
