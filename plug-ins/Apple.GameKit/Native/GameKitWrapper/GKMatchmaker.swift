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
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        let delegate = GKWMatchDelegate();
        match!.delegate = delegate;
        
        matchmaker.finishMatchmaking(for: match!);
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
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        let delegate = GKWMatchDelegate();
        match!.delegate = delegate;
        
        matchmaker.finishMatchmaking(for: match!);
        onSuccess(taskId, Unmanaged.passRetained(match!).toOpaque());
    });
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
