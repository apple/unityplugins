//
//  GKMatchDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias GKMatchDataReceivedCallback = @convention(c) (UnsafeMutablePointer<GKWMatchDelegate>, UnsafeMutablePointer<NSData>, UnsafeMutablePointer<GKPlayer>) -> Void;
public typealias GKMatchDataReceivedForPlayerCallback = @convention(c) (UnsafeMutablePointer<GKWMatchDelegate>, UnsafeMutablePointer<NSData>, UnsafeMutablePointer<GKPlayer>, UnsafeMutablePointer<GKPlayer>) -> Void;
public typealias GKMatchPlayerConnectionDidChangeCallback = @convention(c) (UnsafeMutablePointer<GKWMatchDelegate>, UnsafeMutablePointer<GKPlayer>, GKPlayerConnectionState) -> Void;
public typealias GKMatchShouldReinviteDisconnectedPlayerCallback = @convention(c) (UnsafeMutablePointer<GKWMatchDelegate>, UnsafeMutablePointer<GKPlayer>) -> Bool;
public typealias GKMatchDidFailWithErrorCallback = @convention(c) (UnsafeMutablePointer<GKWMatchDelegate>, UnsafeMutablePointer<NSError>) -> Void;

public class GKWMatchDelegate : NSObject, GKMatchDelegate {
    public var Match : GKMatch? = nil;

    public var DataReceivedHandler : GKMatchDataReceivedCallback? = nil;
    public var DataReceivedForPlayerHandler : GKMatchDataReceivedForPlayerCallback? = nil;
    public var PlayerConnectionDidChangeHandler: GKMatchPlayerConnectionDidChangeCallback? = nil;
    public var ShouldReinviteDisconnectedPlayerHandler: GKMatchShouldReinviteDisconnectedPlayerCallback? = nil;
    public var DidFailWithErrorHandler: GKMatchDidFailWithErrorCallback? = nil;

    public func match(_ match: GKMatch, didReceive data: Data, fromRemotePlayer player: GKPlayer) {
        DataReceivedHandler?(
            self.passRetainedUnsafeMutablePointer(),
            (data as NSData).passRetainedUnsafeMutablePointer(),
            player.passRetainedUnsafeMutablePointer());
    }

    public func match(_ match: GKMatch, didReceive data: Data, forRecipient recipient: GKPlayer, fromRemotePlayer player: GKPlayer) {
        DataReceivedForPlayerHandler?(
            self.passRetainedUnsafeMutablePointer(),
            (data as NSData).passRetainedUnsafeMutablePointer(),
            recipient.passRetainedUnsafeMutablePointer(),
            player.passRetainedUnsafeMutablePointer());
    }

    public func match(_ match: GKMatch, player: GKPlayer, didChange state: GKPlayerConnectionState) {
        PlayerConnectionDidChangeHandler?(
            self.passRetainedUnsafeMutablePointer(),
            player.passRetainedUnsafeMutablePointer(),
            state);
    }

    public func match(_ match: GKMatch, shouldReinviteDisconnectedPlayer player: GKPlayer) -> Bool {
        return ShouldReinviteDisconnectedPlayerHandler?(
            self.passRetainedUnsafeMutablePointer(),
            player.passRetainedUnsafeMutablePointer()) ?? false;
    }

    public func match(_ match: GKMatch, didFailWithError error: Error?) {
        DidFailWithErrorHandler?(
            self.passRetainedUnsafeMutablePointer(),
            (error! as NSError).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKMatchDelegate_SetDataReceived")
public func GKMatchDelegate_SetDataReceived
(
    pointer: UnsafeMutablePointer<GKWMatchDelegate>,
    callback: @escaping GKMatchDataReceivedCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.DataReceivedHandler = callback;
}

@_cdecl("GKMatchDelegate_SetDataReceivedForPlayer")
public func GKMatchDelegate_SetDataReceivedForPlayer
(
    pointer: UnsafeMutablePointer<GKWMatchDelegate>,
    callback: @escaping GKMatchDataReceivedForPlayerCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.DataReceivedForPlayerHandler = callback;
}

@_cdecl("GKMatchDelegate_SetPlayerConnectedDidChange")
public func GKMatchDelegate_SetPlayerConnectedDidChange
(
    pointer: UnsafeMutablePointer<GKWMatchDelegate>,
    callback: @escaping GKMatchPlayerConnectionDidChangeCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.PlayerConnectionDidChangeHandler = callback;
}

@_cdecl("GKMatchDelegate_SetDidFailWithError")
public func GKMatchDelegate_SetDidFailWithError
(
    pointer: UnsafeMutablePointer<GKWMatchDelegate>,
    callback: @escaping GKMatchDidFailWithErrorCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.DidFailWithErrorHandler = callback;
}

@_cdecl("GKMatchDelegate_SetShouldReinviteDisconnectedPlayer")
public func GKMatchDelegate_SetShouldReinviteDisconnectedPlayer
(
    pointer: UnsafeMutablePointer<GKWMatchDelegate>,
    callback: @escaping GKMatchShouldReinviteDisconnectedPlayerCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.ShouldReinviteDisconnectedPlayerHandler = callback;
}
