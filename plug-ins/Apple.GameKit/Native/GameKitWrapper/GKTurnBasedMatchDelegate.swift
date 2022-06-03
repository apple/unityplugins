//
//  GKTurnBasedMatchDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias ExchangeCanceledCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias ExchangeCompletedCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias ExchangedReceivedCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;

public typealias MatchRequestedWithOtherPlayersCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias MatchEndedCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias TurnEventReceivedCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, Bool) -> Void;
public typealias PlayerWantsToQuitCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;

extension GKWLocalPlayerListener : GKTurnBasedEventListener {
    
    public func player(_ player: GKPlayer, receivedExchangeRequest exchange: GKTurnBasedExchange, for match: GKTurnBasedMatch) {
        ExchangeReceived?(
            Unmanaged.passRetained(player).toOpaque(),
            Unmanaged.passRetained(exchange).toOpaque(),
            Unmanaged.passRetained(match).toOpaque());
    }
    
    public func player(_ player: GKPlayer, receivedExchangeCancellation exchange: GKTurnBasedExchange, for match: GKTurnBasedMatch) {
        ExchangeCanceled?(
            Unmanaged.passRetained(player).toOpaque(),
            Unmanaged.passRetained(exchange).toOpaque(),
            Unmanaged.passRetained(match).toOpaque());
    }
    
   public func player(_ player: GKPlayer, receivedExchangeReplies replies: [GKTurnBasedExchangeReply], forCompletedExchange exchange: GKTurnBasedExchange, for match: GKTurnBasedMatch) {
       ExchangeCompleted?(
        Unmanaged.passRetained(player).toOpaque(),
        Unmanaged.passRetained(replies as NSArray).toOpaque(),
        Unmanaged.passRetained(match).toOpaque());
    }
    
    public func player(_ player: GKPlayer, didRequestMatchWithRecipients recipientPlayers: [GKPlayer]) {
        MatchRequestedWithOtherPlayers?(
            Unmanaged.passRetained(player).toOpaque(),
            Unmanaged.passRetained(recipientPlayers as NSArray).toOpaque());
    }
    
    public func player(_ player: GKPlayer, matchEnded match: GKTurnBasedMatch) {
        MatchEnded?(
            Unmanaged.passRetained(player).toOpaque(),
            Unmanaged.passRetained(match).toOpaque());
    }
    
    public func player(_ player: GKPlayer, receivedTurnEventFor match: GKTurnBasedMatch, didBecomeActive: Bool) {
        if(_presentingTurnBasedMatchmakerViewController != nil) {
            // Match found...
            let delegate = _presentingTurnBasedMatchmakerViewController!.turnBasedMatchmakerDelegate as? GKWTurnBasedMatchmakerViewControllerDelegate;
            
            if(delegate != nil) {
                delegate!.DidFindMatch?(
                        Unmanaged.passRetained(delegate!).toOpaque(),
                        Unmanaged.passRetained(_presentingTurnBasedMatchmakerViewController!).toOpaque(),
                        Unmanaged.passRetained(match).toOpaque());
            }
            
            // Dismiss...
            GKTurnBasedMatchmakerViewController_Dismiss(viewController: _presentingTurnBasedMatchmakerViewController!);
        }
        
        TurnEventReceived?(
            Unmanaged.passRetained(player).toOpaque(),
            Unmanaged.passRetained(match).toOpaque(),
            didBecomeActive);
    }
    
    public func player(_ player: GKPlayer, wantsToQuitMatch match: GKTurnBasedMatch) {
        PlayerWantsToQuit?(
            Unmanaged.passRetained(player).toOpaque(),
            Unmanaged.passRetained(match).toOpaque());
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
