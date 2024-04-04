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
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let matchRequest = Unmanaged<GKMatchRequest>.fromOpaque(pointer).takeUnretainedValue();
    let instance = GKTurnBasedMatchmakerViewController.init(matchRequest: matchRequest);
    
    return Unmanaged.passRetained(instance).toOpaque();
}

@_cdecl("GKTurnBasedMatchmakerViewController_GetShowExistingMatches")
public func GKTurnBasedMatchmakerViewController_GetShowExistingMatches
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let target = Unmanaged<GKTurnBasedMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
    return target.showExistingMatches;
}

@_cdecl("GKTurnBasedMatchmakerViewController_SetShowExistingMatches")
public func GKTurnBasedMatchmakerViewController_SetShowExistingMatches
(
    pointer: UnsafeMutableRawPointer,
    value: Bool
)
{
    let target = Unmanaged<GKTurnBasedMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
    target.showExistingMatches = value;
}

@_cdecl("GKTurnBasedMatchmakerViewController_GetMatchmakingMode")
public func GKTurnBasedMatchmakerViewController_GetMatchmakingMode
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKTurnBasedMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
    
    if #available(iOS 15, tvOS 15, macOS 12, *) {
        return target.matchmakingMode.rawValue
    } else {
        return 0;
        
    };
}

@_cdecl("GKTurnBasedMatchmakerViewController_SetMatchmakingMode")
public func GKTurnBasedMatchmakerViewController_SetMatchmakingMode
(
    pointer: UnsafeMutableRawPointer,
    value: Int
)
{
    let target = Unmanaged<GKTurnBasedMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
    
    if #available(iOS 15, tvOS 15, macOS 12, *) {
        target.matchmakingMode = GKMatchmakingMode.init(rawValue: value)!;
    }
}

public var _activeTurnBasedMatchmakerDelegate : GKWTurnBasedMatchmakerViewControllerDelegate? = nil;

@_cdecl("GKTurnBasedMatchmakerViewController_GetMatchmakerDelegate")
public func GKTurnBasedMatchmakerViewController_GetMatchmakerDelegate
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let target = Unmanaged<GKTurnBasedMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.turnBasedMatchmakerDelegate == nil) {
        _activeTurnBasedMatchmakerDelegate = GKWTurnBasedMatchmakerViewControllerDelegate();
        target.turnBasedMatchmakerDelegate = _activeTurnBasedMatchmakerDelegate;
    }
    
    return Unmanaged.passRetained(target.turnBasedMatchmakerDelegate!).toOpaque();
}

public var _presentingTurnBasedMatchmakerViewController : GKTurnBasedMatchmakerViewController? = nil;

@_cdecl("GKTurnBasedMatchmakerViewController_Present")
public func GKTurnBasedMatchmakerViewController_Present
(
    pointer: UnsafeMutableRawPointer
)
{
    let target = Unmanaged<GKTurnBasedMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
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
