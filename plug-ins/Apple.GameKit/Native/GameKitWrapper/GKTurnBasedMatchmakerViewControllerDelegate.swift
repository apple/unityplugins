//
//  GKTurnBasedMatchmakerViewControllerDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias DidFindMatchCallback = @convention(c) (
    UnsafeMutablePointer<GKWTurnBasedMatchmakerViewControllerDelegate>,
    UnsafeMutablePointer<GKTurnBasedMatchmakerViewController>,
    UnsafeMutablePointer<GKTurnBasedMatch>) -> Void;

public typealias MatchmakingCanceledCallback = @convention(c) (
    UnsafeMutablePointer<GKWTurnBasedMatchmakerViewControllerDelegate>,
    UnsafeMutablePointer<GKTurnBasedMatchmakerViewController>) -> Void;

public typealias DidFailWithErrorCallback = @convention(c) (
    UnsafeMutablePointer<GKWTurnBasedMatchmakerViewControllerDelegate>,
    UnsafeMutablePointer<GKTurnBasedMatchmakerViewController>,
    UnsafeMutablePointer<NSError>) -> Void;

public class GKWTurnBasedMatchmakerViewControllerDelegate : NSObject, GKTurnBasedMatchmakerViewControllerDelegate {
    public var DidFindMatch : DidFindMatchCallback? = nil;
    public var MatchmakingCanceled: MatchmakingCanceledCallback? = nil;
    public var DidFailWithError: DidFailWithErrorCallback? = nil;
    
    public func turnBasedMatchmakerViewControllerWasCancelled(_ viewController: GKTurnBasedMatchmakerViewController) {
        GKTurnBasedMatchmakerViewController_Dismiss(viewController: viewController);

        MatchmakingCanceled?( // MatchmakingCanceledCallback
            self.passRetainedUnsafeMutablePointer(),
            viewController.passRetainedUnsafeMutablePointer());
    }
    
    public func turnBasedMatchmakerViewController(_ viewController: GKTurnBasedMatchmakerViewController, didFailWithError error: Error) {
        GKTurnBasedMatchmakerViewController_Dismiss(viewController: viewController);
        
        DidFailWithError?( // DidFailWithErrorCallback
            self.passRetainedUnsafeMutablePointer(),
            viewController.passRetainedUnsafeMutablePointer(),
            (error as NSError).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKTurnBasedMatchmakerViewControllerDelegate_SetDidFindMatchCallback")
public func GKTurnBasedMatchmakerViewControllerDelegate_SetDidFindMatchCallback
(
    pointer: UnsafeMutablePointer<GKWTurnBasedMatchmakerViewControllerDelegate>,
    callback: @escaping DidFindMatchCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.DidFindMatch = callback;
}

@_cdecl("GKTurnBasedMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback")
public func GKTurnBasedMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback
(
    pointer: UnsafeMutablePointer<GKWTurnBasedMatchmakerViewControllerDelegate>,
    callback: @escaping DidFailWithErrorCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.DidFailWithError = callback;
}

@_cdecl("GKTurnBasedMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback")
public func GKTurnBasedMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback
(
    pointer: UnsafeMutablePointer<GKWTurnBasedMatchmakerViewControllerDelegate>,
    callback: @escaping MatchmakingCanceledCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.MatchmakingCanceled = callback;
}
