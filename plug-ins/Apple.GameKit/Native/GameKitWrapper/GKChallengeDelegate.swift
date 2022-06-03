//
//  GKChallengeDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias ChallengeReceivedCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias ChallengeOtherPlayerAcceptedCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias ChallengeCompletedCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias ChallengeOtherPlayerCompletedCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;

extension GKWLocalPlayerListener : GKChallengeListener {

    
    public func player(_ player: GKPlayer, didReceive challenge: GKChallenge) {
        ChallengeReceived?(
            Unmanaged.passRetained(player).toOpaque(),
            Unmanaged.passRetained(challenge).toOpaque());
    }
    
    public func player(_ player: GKPlayer, wantsToPlay challenge: GKChallenge) {
        ChallengeOtherPlayerAccepted?(
            Unmanaged.passRetained(player).toOpaque(),
            Unmanaged.passRetained(challenge).toOpaque());
    }
    
    public func player(_ player: GKPlayer, didComplete challenge: GKChallenge, issuedByFriend friendPlayer: GKPlayer) {
        ChallengeCompleted?(
            Unmanaged.passRetained(player).toOpaque(),
            Unmanaged.passRetained(challenge).toOpaque(),
            Unmanaged.passRetained(friendPlayer).toOpaque());
    }
    
    public func player(_ player: GKPlayer, issuedChallengeWasCompleted challenge: GKChallenge, byFriend friendPlayer: GKPlayer) {
        ChallengeOtherPlayerCompleted?(
            Unmanaged.passRetained(player).toOpaque(),
            Unmanaged.passRetained(challenge).toOpaque(),
            Unmanaged.passRetained(friendPlayer).toOpaque());
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

