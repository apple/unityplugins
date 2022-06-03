//
//  GKLocalPlayerListener.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public let _localPlayerListener = GKWLocalPlayerListener();

public class GKWLocalPlayerListener : NSObject, GKLocalPlayerListener {
    public var InviteAccepted : InviteAcceptedCallback? = nil;
    
    public var ChallengeReceived: ChallengeReceivedCallback? = nil;
    public var ChallengeOtherPlayerAccepted: ChallengeOtherPlayerAcceptedCallback? = nil;
    public var ChallengeCompleted: ChallengeCompletedCallback? = nil;
    public var ChallengeOtherPlayerCompleted: ChallengeOtherPlayerCompletedCallback? = nil;
    
    public var ExchangeReceived : ExchangedReceivedCallback? = nil;
    public var ExchangeCanceled : ExchangeCanceledCallback? = nil;
    public var ExchangeCompleted: ExchangeCompletedCallback? = nil;
    
    public var MatchRequestedWithOtherPlayers : MatchRequestedWithOtherPlayersCallback? = nil;
    public var MatchEnded : MatchEndedCallback? = nil;
    public var TurnEventReceived : TurnEventReceivedCallback? = nil;
    public var PlayerWantsToQuit: PlayerWantsToQuitCallback? = nil;
}
