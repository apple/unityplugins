//
//  GKTurnBasedMatchDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias ExchangeCanceledCallback = @convention(c) (
    UnsafeMutablePointer<GKPlayer>,
    UnsafeMutablePointer<GKTurnBasedExchange>,
    UnsafeMutablePointer<GKTurnBasedMatch>) -> Void;

public typealias ExchangeCompletedCallback = @convention(c) (
    UnsafeMutablePointer<GKPlayer>,
    UnsafeMutablePointer<NSArray>,  // NSArray<GKTurnBasedExchangeReply>
    UnsafeMutablePointer<GKTurnBasedMatch>) -> Void;

public typealias ExchangedReceivedCallback = @convention(c) (
    UnsafeMutablePointer<GKPlayer>,
    UnsafeMutablePointer<GKTurnBasedExchange>,
    UnsafeMutablePointer<GKTurnBasedMatch>) -> Void;

public typealias MatchRequestedWithOtherPlayersCallback = @convention(c) (
    UnsafeMutablePointer<GKPlayer>,
    UnsafeMutablePointer<NSArray>) -> Void; // NSArray<GKPlayer>

public typealias MatchEndedCallback = @convention(c) (
    UnsafeMutablePointer<GKPlayer>,
    UnsafeMutablePointer<GKTurnBasedMatch>) -> Void;

public typealias TurnEventReceivedCallback = @convention(c) (
    UnsafeMutablePointer<GKPlayer>,
    UnsafeMutablePointer<GKTurnBasedMatch>,
    Bool) -> Void;

public typealias PlayerWantsToQuitCallback = @convention(c) (
    UnsafeMutablePointer<GKPlayer>,
    UnsafeMutablePointer<GKTurnBasedMatch>) -> Void;

extension GKWLocalPlayerListener : GKTurnBasedEventListener {
    
    public func player(_ player: GKPlayer, receivedExchangeRequest exchange: GKTurnBasedExchange, for match: GKTurnBasedMatch) {
        ExchangeReceived?( // ExchangedReceivedCallback
            player.passRetainedUnsafeMutablePointer(),
            exchange.passRetainedUnsafeMutablePointer(),
            match.passRetainedUnsafeMutablePointer());
    }

    public func player(_ player: GKPlayer, receivedExchangeCancellation exchange: GKTurnBasedExchange, for match: GKTurnBasedMatch) {
        ExchangeCanceled?( // ExchangeCanceledCallback
            player.passRetainedUnsafeMutablePointer(),
            exchange.passRetainedUnsafeMutablePointer(),
            match.passRetainedUnsafeMutablePointer());
    }

    public func player(_ player: GKPlayer, receivedExchangeReplies replies: [GKTurnBasedExchangeReply], forCompletedExchange exchange: GKTurnBasedExchange, for match: GKTurnBasedMatch) {
        ExchangeCompleted?( // ExchangeCompletedCallback
            player.passRetainedUnsafeMutablePointer(),
            (replies as NSArray).passRetainedUnsafeMutablePointer(),
            match.passRetainedUnsafeMutablePointer());
    }

    public func player(_ player: GKPlayer, didRequestMatchWithRecipients recipientPlayers: [GKPlayer]) {
        MatchRequestedWithOtherPlayers?( // MatchRequestedWithOtherPlayersCallback
            player.passRetainedUnsafeMutablePointer(),
            (recipientPlayers as NSArray).passRetainedUnsafeMutablePointer());
    }

    public func player(_ player: GKPlayer, matchEnded match: GKTurnBasedMatch) {
        MatchEnded?( // MatchEndedCallback
            player.passRetainedUnsafeMutablePointer(),
            match.passRetainedUnsafeMutablePointer());
    }

    public func player(_ player: GKPlayer, receivedTurnEventFor match: GKTurnBasedMatch, didBecomeActive: Bool) {
        if (_presentingTurnBasedMatchmakerViewController != nil) {
            // Match found...
            if let delegate = _presentingTurnBasedMatchmakerViewController!.turnBasedMatchmakerDelegate as? GKWTurnBasedMatchmakerViewControllerDelegate {
                delegate.DidFindMatch?( // DidFindMatchCallback
                    delegate.passRetainedUnsafeMutablePointer(),
                    _presentingTurnBasedMatchmakerViewController!.passRetainedUnsafeMutablePointer(),
                    match.passRetainedUnsafeMutablePointer());
            }

            // Dismiss...
            GKTurnBasedMatchmakerViewController_Dismiss(viewController: _presentingTurnBasedMatchmakerViewController!);
        }

        TurnEventReceived?( // TurnEventReceivedCallback
            player.passRetainedUnsafeMutablePointer(),
            match.passRetainedUnsafeMutablePointer(),
            didBecomeActive);
    }

    public func player(_ player: GKPlayer, wantsToQuitMatch match: GKTurnBasedMatch) {
        PlayerWantsToQuit?( // PlayerWantsToQuitCallback
            player.passRetainedUnsafeMutablePointer(),
            match.passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKTurnBasedMatch_SetExchangeReceivedCallback")
public func GKTurnBasedMatch_SetExchangeReceivedCallback(callback : @escaping ExchangedReceivedCallback)
{
    _localPlayerListener.ExchangeReceived = callback;
}

@_cdecl("GKTurnBasedMatch_SetExchangeCanceledCallback")
public func GKTurnBasedMatch_SetExchangeCanceledCallback(callback : @escaping ExchangeCanceledCallback)
{
    _localPlayerListener.ExchangeCanceled = callback;
}

@_cdecl("GKTurnBasedMatch_SetExchangeCompletedCallback")
public func GKTurnBasedMatch_SetExchangeCompletedCallback(callback : @escaping ExchangeCompletedCallback)
{
    _localPlayerListener.ExchangeCompleted = callback;
}

@_cdecl("GKTurnBasedMatch_SetMatchRequestedWithOtherPlayersCallback")
public func GKTurnBasedMatch_SetMatchRequestedWithOtherPlayersCallback(callback : @escaping MatchRequestedWithOtherPlayersCallback)
{
    _localPlayerListener.MatchRequestedWithOtherPlayers = callback;
}

@_cdecl("GKTurnBasedMatch_SetMatchEndedCallback")
public func GKTurnBasedMatch_SetMatchEndedCallback(callback : @escaping MatchEndedCallback)
{
    _localPlayerListener.MatchEnded = callback;
}

@_cdecl("GKTurnBasedMatch_SetTurnEventReceivedCallback")
public func GKTurnBasedMatch_SetTurnEventReceivedCallback(callback : @escaping TurnEventReceivedCallback)
{
    _localPlayerListener.TurnEventReceived = callback;
}

@_cdecl("GKTurnBasedMatch_SetPlayerWantsToQuitMatchCallback")
public func GKTurnBasedMatch_SetPlayerWantsToQuitMatchCallback(callback : @escaping PlayerWantsToQuitCallback)
{
    _localPlayerListener.PlayerWantsToQuit = callback;
}
