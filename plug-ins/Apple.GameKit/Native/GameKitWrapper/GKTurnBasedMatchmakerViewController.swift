//
//  GKTurnBasedMatchmakerViewController.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKTurnBasedMatchmakerViewController_InitWithMatchRequest")
public func GKTurnBasedMatchmakerViewController_InitWithMatchRequest
(
    pointer: UnsafeMutablePointer<GKMatchRequest>
) -> UnsafeMutablePointer<GKTurnBasedMatchmakerViewController>
{
    let matchRequest = pointer.takeUnretainedValue();
    let instance = GKTurnBasedMatchmakerViewController.init(matchRequest: matchRequest);
    
    return instance.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKTurnBasedMatchmakerViewController_GetShowExistingMatches")
public func GKTurnBasedMatchmakerViewController_GetShowExistingMatches
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatchmakerViewController>
) -> Bool
{
    let target = pointer.takeUnretainedValue();
    return target.showExistingMatches;
}

@_cdecl("GKTurnBasedMatchmakerViewController_SetShowExistingMatches")
public func GKTurnBasedMatchmakerViewController_SetShowExistingMatches
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatchmakerViewController>,
    value: Bool
)
{
    let target = pointer.takeUnretainedValue();
    target.showExistingMatches = value;
}

@_cdecl("GKTurnBasedMatchmakerViewController_GetMatchmakingMode")
public func GKTurnBasedMatchmakerViewController_GetMatchmakingMode
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatchmakerViewController>
) -> Int
{
    if #available(iOS 15, tvOS 15, macOS 12, *) {
        let target = pointer.takeUnretainedValue();
        return target.matchmakingMode.rawValue
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKTurnBasedMatchmakerViewController_SetMatchmakingMode")
public func GKTurnBasedMatchmakerViewController_SetMatchmakingMode
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatchmakerViewController>,
    value: Int
)
{
    if #available(iOS 15, tvOS 15, macOS 12, *) {
        let target = pointer.takeUnretainedValue();
        target.matchmakingMode = GKMatchmakingMode.init(rawValue: value)!;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

public var _activeTurnBasedMatchmakerDelegate : GKWTurnBasedMatchmakerViewControllerDelegate? = nil;

@_cdecl("GKTurnBasedMatchmakerViewController_GetMatchmakerDelegate")
public func GKTurnBasedMatchmakerViewController_GetMatchmakerDelegate
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatchmakerViewController>
) -> UnsafeMutablePointer<GKWTurnBasedMatchmakerViewControllerDelegate>
{
    let target = pointer.takeUnretainedValue();

    if (target.turnBasedMatchmakerDelegate == nil) {
        _activeTurnBasedMatchmakerDelegate = GKWTurnBasedMatchmakerViewControllerDelegate();
        target.turnBasedMatchmakerDelegate = _activeTurnBasedMatchmakerDelegate;
    }

    return _activeTurnBasedMatchmakerDelegate!.passRetainedUnsafeMutablePointer();
}

public var _presentingTurnBasedMatchmakerViewController : GKTurnBasedMatchmakerViewController? = nil;

@_cdecl("GKTurnBasedMatchmakerViewController_Present")
public func GKTurnBasedMatchmakerViewController_Present
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatchmakerViewController>
)
{
    let target = pointer.takeUnretainedValue();
    _presentingTurnBasedMatchmakerViewController = target;
    UiUtilities.presentViewController(viewController: target)
}

@_cdecl("GKTurnBasedMatchmakerViewController_Dismiss")
public func GKTurnBasedMatchmakerViewController_Dismiss
(
    viewController: GKTurnBasedMatchmakerViewController
)
{
    UiUtilities.dismissViewController(viewController: viewController)
    _presentingTurnBasedMatchmakerViewController = nil;
    _activeTurnBasedMatchmakerDelegate = nil;
}
