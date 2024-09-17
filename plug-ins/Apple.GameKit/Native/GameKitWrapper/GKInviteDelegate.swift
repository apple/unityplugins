//
//  GKInviteDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias InviteAcceptedCallback = @convention(c) (UnsafeMutablePointer<GKPlayer>, UnsafeMutablePointer<GKInvite>) -> Void;

extension GKWLocalPlayerListener : GKInviteEventListener {
    
    public func player(_ player: GKPlayer, didAccept invite: GKInvite) {
        InviteAccepted?(
            player.passRetainedUnsafeMutablePointer(),
            invite.passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKInvite_SetInviteAcceptedCallback")
public func GKInvite_SetInviteAcceptedCallback(callback : @escaping InviteAcceptedCallback) {
    _localPlayerListener.InviteAccepted = callback;
}
