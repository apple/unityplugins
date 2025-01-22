//
//  GKChallengeDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias ChallengeReceivedCallback = @convention(c) (UnsafeMutablePointer<GKPlayer>, UnsafeMutablePointer<GKChallenge>) -> Void;
public typealias ChallengeOtherPlayerAcceptedCallback = @convention(c) (UnsafeMutablePointer<GKPlayer>, UnsafeMutablePointer<GKChallenge>) -> Void;
public typealias ChallengeCompletedCallback = @convention(c) (UnsafeMutablePointer<GKPlayer>, UnsafeMutablePointer<GKChallenge>, UnsafeMutablePointer<GKPlayer>) -> Void;
public typealias ChallengeOtherPlayerCompletedCallback = @convention(c) (UnsafeMutablePointer<GKPlayer>, UnsafeMutablePointer<GKChallenge>, UnsafeMutablePointer<GKPlayer>) -> Void;

extension GKWLocalPlayerListener : GKChallengeListener {

    
    public func player(_ player: GKPlayer, didReceive challenge: GKChallenge) {
        ChallengeReceived?(
            player.passRetainedUnsafeMutablePointer(),
            challenge.passRetainedUnsafeMutablePointer());
    }
    
    public func player(_ player: GKPlayer, wantsToPlay challenge: GKChallenge) {
        ChallengeOtherPlayerAccepted?(
            player.passRetainedUnsafeMutablePointer(),
            challenge.passRetainedUnsafeMutablePointer());
    }
    
    public func player(_ player: GKPlayer, didComplete challenge: GKChallenge, issuedByFriend friendPlayer: GKPlayer) {
        ChallengeCompleted?(
            player.passRetainedUnsafeMutablePointer(),
            challenge.passRetainedUnsafeMutablePointer(),
            friendPlayer.passRetainedUnsafeMutablePointer());
    }
    
    public func player(_ player: GKPlayer, issuedChallengeWasCompleted challenge: GKChallenge, byFriend friendPlayer: GKPlayer) {
        ChallengeOtherPlayerCompleted?(
            player.passRetainedUnsafeMutablePointer(),
            challenge.passRetainedUnsafeMutablePointer(),
            friendPlayer.passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKChallenge_SetChallengeCompletedCallback")
public func GKChallenge_SetChallengeCompletedCallback(callback : @escaping ChallengeCompletedCallback)
{
    _localPlayerListener.ChallengeCompleted = callback;
}

@_cdecl("GKChallenge_SetChallengeReceivedCallback")
public func GKChallenge_SetChallengeReceivedCallback(callback : @escaping ChallengeReceivedCallback)
{
    _localPlayerListener.ChallengeReceived = callback;
}

@_cdecl("GKChallenge_SetChallengeOtherPlayerAcceptedCallback")
public func GKChallenge_SetChallengeOtherPlayerAcceptedCallback(callback : @escaping ChallengeOtherPlayerAcceptedCallback)
{
    _localPlayerListener.ChallengeOtherPlayerAccepted = callback;
}

@_cdecl("GKChallenge_SetChallengeOtherPlayerCompletedCallback")
public func GKChallenge_SetChallengeOtherPlayerCompletedCallback(callback : @escaping ChallengeOtherPlayerCompletedCallback)
{
    _localPlayerListener.ChallengeOtherPlayerCompleted = callback;
}

