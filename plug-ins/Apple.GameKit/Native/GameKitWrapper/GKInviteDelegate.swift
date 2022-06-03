//
//  GKInviteDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias InviteAcceptedCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;

extension GKWLocalPlayerListener : GKInviteEventListener {
    
    public func player(_ player: GKPlayer, didAccept invite: GKInvite) {
        InviteAccepted?(
            Unmanaged.passRetained(player).toOpaque(),
            Unmanaged.passRetained(invite).toOpaque());
    }
}

@_cdecl("GKInvite_SetInviteAcceptedCallback")
public func GKInvite_SetInviteAcceptedCallback(callback : @escaping InviteAcceptedCallback) {
    _localPlayerListener.InviteAccepted = callback;
}
