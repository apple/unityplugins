//
//  GKMatchDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias GKMatchDataReceivedCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, Int32, UnsafeMutableRawPointer) -> Void;
public typealias GKMatchDataReceivedForPlayerCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, Int32, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias GKMatchPlayerConnectionDidChangeCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, GKPlayerConnectionState) -> Void;
public typealias GKMatchShouldReinviteDisconnectedPlayerCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Bool;
public typealias GKMatchDidFailWithErrorhandler = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;

public class GKWMatchDelegate : NSObject, GKMatchDelegate {
    public var Match : GKMatch? = nil;
    
    public var DataReceivedHandler : GKMatchDataReceivedCallback? = nil;
    public var DataReceivedForPlayerHandler : GKMatchDataReceivedForPlayerCallback? = nil;
    public var PlayerConnectionDidChangeHandler: GKMatchPlayerConnectionDidChangeCallback? = nil;
    public var ShouldReinviteDisconnectedPlayerHandler: GKMatchShouldReinviteDisconnectedPlayerCallback? = nil;
    public var DidFailWithErrorHandler: GKMatchDidFailWithErrorhandler? = nil;
    
    public func match(_ match: GKMatch, didReceive data: Data, fromRemotePlayer player: GKPlayer) {
        print(player);
        DataReceivedHandler?(
            Unmanaged.passRetained(self).toOpaque(),
            data.toUCharP(),
            Int32(data.count),
            Unmanaged.passRetained(player).toOpaque());
    }
    
    public func match(_ match: GKMatch, didReceive data: Data, forRecipient recipient: GKPlayer, fromRemotePlayer player: GKPlayer) {
        DataReceivedForPlayerHandler?(
            Unmanaged.passRetained(self).toOpaque(),
            data.toUCharP(),
            Int32(data.count),
            Unmanaged.passRetained(recipient).toOpaque(),
            Unmanaged.passRetained(player).toOpaque());
    }
    
    public func match(_ match: GKMatch, player: GKPlayer, didChange state: GKPlayerConnectionState) {
        PlayerConnectionDidChangeHandler?(
            Unmanaged.passRetained(self).toOpaque(),
            Unmanaged.passRetained(player).toOpaque(),
            state);
    }
    
    public func match(_ match: GKMatch, shouldReinviteDisconnectedPlayer player: GKPlayer) -> Bool {
        return ShouldReinviteDisconnectedPlayerHandler?(
            Unmanaged.passRetained(self).toOpaque(),
            Unmanaged.passRetained(player).toOpaque()) ?? false;
    }
    
    public func match(_ match: GKMatch, didFailWithError error: Error?) {
        DidFailWithErrorHandler?(Unmanaged.passRetained(self).toOpaque(), Unmanaged.passRetained(error! as NSError).toOpaque());
    }
}

@_cdecl("GKMatchDelegate_Free")
public func GKMatchDelegate_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKWMatchDelegate>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKMatchDelegate_SetDataReceived")
public func GKMatchDelegate_SetDataReceived
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchDataReceivedCallback
)
{
    let target = Unmanaged<GKWMatchDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.DataReceivedHandler = callback;
}

@_cdecl("GKMatchDelegate_SetDataReceivedForPlayer")
public func GKMatchDelegate_SetDataReceivedForPlayer
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchDataReceivedForPlayerCallback
)
{
    let target = Unmanaged<GKWMatchDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.DataReceivedForPlayerHandler = callback;
}

@_cdecl("GKMatchDelegate_SetPlayerConnectedDidChange")
public func GKMatchDelegate_SetPlayerConnectedDidChange
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchPlayerConnectionDidChangeCallback
)
{
    let target = Unmanaged<GKWMatchDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.PlayerConnectionDidChangeHandler = callback;
}

@_cdecl("GKMatchDelegate_SetDidFailWithError")
public func GKMatchDelegate_SetDidFailWithError
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchPlayerConnectionDidChangeCallback
)
{
    let target = Unmanaged<GKWMatchDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.PlayerConnectionDidChangeHandler = callback;
}

@_cdecl("GKMatchDelegate_SetShouldReinviteDisconnectedPlayer")
public func GKMatchDelegate_SetShouldReinviteDisconnectedPlayer
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchShouldReinviteDisconnectedPlayerCallback
)
{
    let target = Unmanaged<GKWMatchDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.ShouldReinviteDisconnectedPlayerHandler = callback;
}
