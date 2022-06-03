//
//  GKTurnBasedMatchmakerViewControllerDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias DidFindMatchCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias MatchmakingCanceledCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias DidFailWithErrorCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;

public class GKWTurnBasedMatchmakerViewControllerDelegate : NSObject, GKTurnBasedMatchmakerViewControllerDelegate {
    public var DidFindMatch : DidFindMatchCallback? = nil;
    public var MatchmakingCanceled: MatchmakingCanceledCallback? = nil;
    public var DidFailWithError: DidFailWithErrorCallback? = nil;
    
    public func turnBasedMatchmakerViewControllerWasCancelled(_ viewController: GKTurnBasedMatchmakerViewController) {
        GKTurnBasedMatchmakerViewController_Dismiss(viewController: viewController);
        
        MatchmakingCanceled?(
            Unmanaged.passRetained(self).toOpaque(),
            Unmanaged.passRetained(viewController).toOpaque());
    }
    
    public func turnBasedMatchmakerViewController(_ viewController: GKTurnBasedMatchmakerViewController, didFailWithError error: Error) {
        GKTurnBasedMatchmakerViewController_Dismiss(viewController: viewController);
        
        DidFailWithError?(
            Unmanaged.passRetained(self).toOpaque(),
            Unmanaged.passRetained(viewController).toOpaque(),
            Unmanaged.passRetained(error as NSError).toOpaque());
    }
}

@_cdecl("GKTurnBasedMatchmakerViewControllerDelegate_SetDidFindMatchCallback")
public func GKTurnBasedMatchmakerViewControllerDelegate_SetDidFindMatchCallback
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping DidFindMatchCallback
)
{
    let target = Unmanaged<GKWTurnBasedMatchmakerViewControllerDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.DidFindMatch = callback;
}

@_cdecl("GKTurnBasedMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback")
public func GKTurnBasedMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping DidFailWithErrorCallback
)
{
    let target = Unmanaged<GKWTurnBasedMatchmakerViewControllerDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.DidFailWithError = callback;
}

@_cdecl("GKTurnBasedMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback")
public func GKTurnBasedMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping MatchmakingCanceledCallback
)
{
    let target = Unmanaged<GKWTurnBasedMatchmakerViewControllerDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.MatchmakingCanceled = callback;
}
