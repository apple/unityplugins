//
//  GKMatchmakerViewControllerDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias GKMatchmakerDidFindMatchCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias GKMatchmakerDidFindHostedPlayersCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias GKMatchmakerCanceledCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias GKMatchmakerDidFailWithErrorCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias GKMatchmakerHostedPlayerDidAcceptCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;

public class GKWMatchmakerViewControllerDelegate : NSObject, GKMatchmakerViewControllerDelegate {
    public var DidFindMatch : GKMatchmakerDidFindMatchCallback? = nil;
    public var DidFindHostedPlayers: GKMatchmakerDidFindHostedPlayersCallback? = nil;
    public var Canceled: GKMatchmakerCanceledCallback? = nil;
    public var DidFailWithError: GKMatchmakerDidFailWithErrorCallback? = nil;
    public var HostedPlayerDidAccept: GKMatchmakerHostedPlayerDidAcceptCallback? = nil;
    
    public func matchmakerViewControllerWasCancelled(_ viewController: GKMatchmakerViewController) {
        GKMatchmakerViewController_Dismiss(viewController: viewController);
        
        Canceled?(
            Unmanaged.passRetained(self).toOpaque(),
            Unmanaged.passRetained(viewController).toOpaque());
    }
    
    public func matchmakerViewController(_ viewController: GKMatchmakerViewController, didFailWithError error: Error) {
        GKMatchmakerViewController_Dismiss(viewController: viewController);
        
        DidFailWithError?(Unmanaged.passRetained(self).toOpaque(),
                          Unmanaged.passRetained(viewController).toOpaque(),
                          Unmanaged.passRetained(error as NSError).toOpaque());
    }
    
    public func matchmakerViewController(_ viewController: GKMatchmakerViewController, didFind match: GKMatch) {
        GKMatchmakerViewController_Dismiss(viewController: viewController);
        
        DidFindMatch?(Unmanaged.passRetained(self).toOpaque(),
                      Unmanaged.passRetained(viewController).toOpaque(),
                      Unmanaged.passRetained(match).toOpaque());
    }

    public func matchmakerViewController(_ viewController: GKMatchmakerViewController, hostedPlayerDidAccept player: GKPlayer) {
        HostedPlayerDidAccept?(
            Unmanaged.passRetained(self).toOpaque(),
            Unmanaged.passRetained(viewController).toOpaque(),
            Unmanaged.passRetained(player).toOpaque());
    }
    
    public func matchmakerViewController(_ viewController: GKMatchmakerViewController, didFindHostedPlayers players: [GKPlayer]) {
        DidFindHostedPlayers?(
            Unmanaged.passRetained(self).toOpaque(),
            Unmanaged.passRetained(viewController).toOpaque(),
            Unmanaged.passRetained(players as NSArray).toOpaque());
    }
}

@_cdecl("GKMatchmakerViewControllerDelegate_Free")
public func GKMatchmakerViewControllerDelegate_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKWMatchmakerViewControllerDelegate>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetDidFindMatchCallback")
public func GKMatchmakerViewControllerDelegate_SetDidFindMatchCallback
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchmakerDidFindMatchCallback
)
{
    let target = Unmanaged<GKWMatchmakerViewControllerDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.DidFindMatch = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetDidFindHostedPlayersCallback")
public func GKMatchmakerViewControllerDelegate_SetDidFindHostedPlayersCallback
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchmakerDidFindHostedPlayersCallback
)
{
    let target = Unmanaged<GKWMatchmakerViewControllerDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.DidFindHostedPlayers = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback")
public func GKMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchmakerCanceledCallback
)
{
    let target = Unmanaged<GKWMatchmakerViewControllerDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.Canceled = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback")
public func GKMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchmakerDidFailWithErrorCallback
)
{
    let target = Unmanaged<GKWMatchmakerViewControllerDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.DidFailWithError = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetHostedPlayerDidAccept")
public func GKMatchmakerViewControllerDelegate_SetHostedPlayerDidAccept
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchmakerHostedPlayerDidAcceptCallback
)
{
    let target = Unmanaged<GKWMatchmakerViewControllerDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.HostedPlayerDidAccept = callback;
}
