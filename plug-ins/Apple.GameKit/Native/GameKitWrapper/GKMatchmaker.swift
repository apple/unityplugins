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
) -> UnsafeMutablePointer<GKMatchmaker>
{
    return GKMatchmaker.shared().passRetainedUnsafeMutablePointer();
}

@_cdecl("GKMatchmaker_MatchForInvite")
public func GKMatchmaker_MatchForInvite
(
    pointer: UnsafeMutablePointer<GKMatchmaker>,
    taskId: Int64,
    invitePtr: UnsafeMutablePointer<GKInvite>,
    onSuccess: @escaping SuccessTaskPtrCallback<GKMatch>,
    onError: @escaping NSErrorTaskCallback
)
{
    let matchmaker = pointer.takeUnretainedValue();
    let invite = invitePtr.takeUnretainedValue();

    matchmaker.match(for: invite, completionHandler: { match, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        let delegate = GKWMatchDelegate();
        match!.delegate = delegate;

        onSuccess(taskId, match!.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKMatchmaker_FindMatch")
public func GKMatchmaker_FindMatch
(
    pointer: UnsafeMutablePointer<GKMatchmaker>,
    taskId: Int64,
    matchRequestPtr: UnsafeMutablePointer<GKMatchRequest>,
    onSuccess: @escaping SuccessTaskPtrCallback<GKMatch>,
    onError: @escaping NSErrorTaskCallback
)
{
    let matchmaker = pointer.takeUnretainedValue();
    let matchRequest = matchRequestPtr.takeUnretainedValue();

    matchmaker.findMatch(for: matchRequest, withCompletionHandler: { match, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        let delegate = GKWMatchDelegate();
        match!.delegate = delegate;

        onSuccess(taskId, match!.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKMatchmaker_FinishMatchmaking")
public func GKMatchmaker_FinishMatchmaking
(
    gkMatchmakerPtr : UnsafeMutablePointer<GKMatchmaker>,
    gkMatchPtr : UnsafeMutablePointer<GKMatch>
)
{
    let gkMatchmaker = gkMatchmakerPtr.takeUnretainedValue();
    let gkMatch = gkMatchPtr.takeUnretainedValue();

    gkMatchmaker.finishMatchmaking(for: gkMatch);
}

@_cdecl("GKMatchmaker_Cancel")
public func GKMatchmaker_Cancel
(
    pointer: UnsafeMutablePointer<GKMatchmaker>
)
{
    let matchmaker = pointer.takeUnretainedValue();
    matchmaker.cancel();
}

@_cdecl("GKMatchmaker_FindPlayers")
public func GKMatchmaker_FindPlayers
(
    pointer: UnsafeMutablePointer<GKMatchmaker>,
    taskId: Int64,
    matchRequestPtr: UnsafeMutablePointer<GKMatchRequest>,
    onSuccess: @escaping SuccessTaskPtrCallback<NSArray>, // NSArray<GKPlayer>
    onError: @escaping NSErrorTaskCallback
)
{
    let matchmaker = pointer.takeUnretainedValue();
    let matchRequest = matchRequestPtr.takeUnretainedValue();
    matchmaker.findPlayers(forHostedRequest: matchRequest, withCompletionHandler: { players, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, (players as? NSArray)?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKMatchMaker_FindMatchedPlayers")
public func GKMatchMaker_FindMatchedPlayers
(
    gkMatchmakerPtr: UnsafeMutablePointer<GKMatchmaker>,
    taskId: Int64,
    gkMatchRequestPtr: UnsafeMutablePointer<GKMatchRequest>,
    onSuccess: @escaping SuccessTaskRawPtrCallback, // GKMatchedPlayers
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 17.2, tvOS 17.2, macOS 14.2, visionOS 1.1, *) {
        let gkMatchMaker = gkMatchmakerPtr.takeUnretainedValue()
        let gkMatchRequest = gkMatchRequestPtr.takeUnretainedValue()

        gkMatchMaker.findMatchedPlayers(gkMatchRequest, withCompletionHandler: { gkMatchedPlayers, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, gkMatchedPlayers?.passRetainedUnsafeMutablePointer());
        })
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKMatchmaker_AddPlayers")
public func GKMatchmaker_AddPlayers
(
    pointer: UnsafeMutablePointer<GKMatchmaker>,
    taskId: Int64,
    matchPtr: UnsafeMutablePointer<GKMatch>,
    matchRequestPtr: UnsafeMutablePointer<GKMatchRequest>,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let matchmaker = pointer.takeUnretainedValue();
    let match = matchPtr.takeUnretainedValue();
    let matchRequest = matchRequestPtr.takeUnretainedValue();
    
    matchmaker.addPlayers(to: match, matchRequest: matchRequest, completionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId);
    });
}

@_cdecl("GKMatchmaker_QueryActivity")
public func GKMatchmaker_QueryActivity
(
    pointer: UnsafeMutablePointer<GKMatchmaker>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskIntCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let matchmaker = pointer.takeUnretainedValue();
    matchmaker.queryActivity(completionHandler: { numPlayers, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, numPlayers);
    });
}

@_cdecl("GKMatchmaker_QueryQueueActivity")
public func GKMatchmaker_QueryQueueActivity
(
    gkMatchmakerPtr: UnsafeMutablePointer<GKMatchmaker>,
    taskId: Int64,
    queueName: char_p,
    onSuccess: @escaping SuccessTaskIntCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 17.2, tvOS 17.2, macOS 14.2, visionOS 1.1, *) {
        let gkMatchmaker = gkMatchmakerPtr.takeUnretainedValue();
        gkMatchmaker.queryQueueActivity(queueName.toString(), withCompletionHandler: { numPlayers, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, numPlayers);
        });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKMatchmaker_QueryPlayerGroupActivity")
public func GKMatchmaker_QueryPlayerGroupActivity
(
    pointer: UnsafeMutablePointer<GKMatchmaker>,
    taskId: Int64,
    groupId: Int,
    onSuccess: @escaping SuccessTaskIntCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let matchmaker = pointer.takeUnretainedValue();
    matchmaker.queryPlayerGroupActivity(groupId, withCompletionHandler: { numPlayers, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, numPlayers);
    });
}

@_cdecl("GKMatchmaker_CancelPendingInvite")
public func GKMatchmaker_CancelPendingInvite
(
    pointer: UnsafeMutablePointer<GKMatchmaker>,
    playerPtr: UnsafeMutablePointer<GKPlayer>
)
{
    let matchmaker = pointer.takeUnretainedValue();
    let player = playerPtr.takeUnretainedValue();

    matchmaker.cancelPendingInvite(to: player);
}

public typealias GKMatchmakerNearbyPlayerReachableHandler = @convention(c) (UnsafeMutablePointer<GKMatchmaker>, UnsafeMutablePointer<GKPlayer>, Bool /*reachable*/) -> Void;

@_cdecl("GKMatchmaker_StartBrowsingForNearbyPlayers")
public func GKMatchmaker_StartBrowsingForNearbyPlayers
(
    gkMatchmakerPtr: UnsafeMutablePointer<GKMatchmaker>,
    nearbyPlayerReachableHandler: @escaping GKMatchmakerNearbyPlayerReachableHandler
)
{
    if #available(iOS 8.0, tvOS 9.0, macOS 10.10, *) {
        let gkMatchmaker = gkMatchmakerPtr.takeUnretainedValue();
        gkMatchmaker.startBrowsingForNearbyPlayers(handler: { gkPlayer, isReachable in
            nearbyPlayerReachableHandler(
                gkMatchmakerPtr,    // not retained as per notes in InteropWeakMap.cs.
                gkPlayer.passRetainedUnsafeMutablePointer(),
                isReachable);
        });
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKMatchmaker_StopBrowsingForNearbyPlayers")
public func GKMatchmaker_StopBrowsingForNearbyPlayers
(
    gkMatchmakerPtr: UnsafeMutablePointer<GKMatchmaker>
)
{
    if #available(iOS 6.0, tvOS 9.0, macOS 10.9, *) {
        let gkMatchmaker = gkMatchmakerPtr.takeUnretainedValue();
        gkMatchmaker.stopBrowsingForNearbyPlayers();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

public typealias GKMatchmakerGroupActivityPlayerHandler = @convention(c) (UnsafeMutablePointer<GKMatchmaker>, UnsafeMutablePointer<GKPlayer>) -> Void;

@_cdecl("GKMatchmaker_StartGroupActivity")
public func GKMatchmaker_StartGroupActivity
(
    gkMatchmakerPtr: UnsafeMutablePointer<GKMatchmaker>,
    groupActivityPlayerHandler: @escaping GKMatchmakerGroupActivityPlayerHandler
)
{
#if !os(tvOS)
    if #available(iOS 16.2, macOS 13.1, *) {
        let gkMatchmaker = gkMatchmakerPtr.takeUnretainedValue();
        gkMatchmaker.startGroupActivity(playerHandler: { gkPlayer in
            groupActivityPlayerHandler(
                gkMatchmakerPtr,    // not retained as per notes in InteropWeakMap.cs.
                gkPlayer.passRetainedUnsafeMutablePointer());
        });
        return;
    }
#endif
    DefaultNSErrorHandler.throwApiUnavailableError();
}

@_cdecl("GKMatchmaker_StopGroupActivity")
public func GKMatchmaker_StopGroupActivity
(
    gkMatchmakerPtr: UnsafeMutablePointer<GKMatchmaker>
)
{
#if !os(tvOS)
    if #available(iOS 16.2, macOS 13.1, *) {
        let gkMatchmaker = gkMatchmakerPtr.takeUnretainedValue();
        gkMatchmaker.stopGroupActivity();
        return;
    }
#endif
    DefaultNSErrorHandler.throwApiUnavailableError();
}
