//
//  GKMatchmakerViewController.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKMatchmakerViewController_InitWithInvite")
public func GKMatchmakerViewController_InitWithInvite
(
    pointer: UnsafeMutablePointer<GKInvite>
) -> UnsafeMutablePointer<GKMatchmakerViewController>
{
    let invite = pointer.takeUnretainedValue();
    let target = GKMatchmakerViewController.init(invite: invite);

    return target!.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKMatchmakerViewController_InitWithMatchRequest")
public func GKMatchmakerViewController_InitWithMatchRequest
(
    pointer: UnsafeMutablePointer<GKMatchRequest>
) -> UnsafeMutablePointer<GKMatchmakerViewController>
{
    let request = pointer.takeUnretainedValue();
    let target = GKMatchmakerViewController.init(matchRequest: request);
    
    return target!.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKMatchmakerViewController_GetMatchRequest")
public func GKMatchmakerViewController_GetMatchRequest
(
    pointer: UnsafeMutablePointer<GKMatchmakerViewController>
) -> UnsafeMutablePointer<GKMatchRequest>
{
    let target = pointer.takeUnretainedValue();
    return target.matchRequest.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKMatchmakerViewController_GetCanStartWithMinimumPlayers")
public func GKMatchmakerViewController_GetCanStartWithMinimumPlayers
(
    pointer: UnsafeMutablePointer<GKMatchmakerViewController>
) -> Bool
{
    if #available(iOS 15, tvOS 15, macOS 12.0, *) {
        let target = pointer.takeUnretainedValue();
        return target.canStartWithMinimumPlayers
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKMatchmakerViewController_SetCanStartWithMinimumPlayers")
public func GKMatchmakerViewController_SetCanStartWithMinimumPlayers
(
    pointer: UnsafeMutablePointer<GKMatchmakerViewController>,
    value: Bool
)
{
    if #available(iOS 15, tvOS 15, macOS 12.0, *) {
        let target = pointer.takeUnretainedValue();
        target.canStartWithMinimumPlayers = value
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKMatchmakerViewController_GetMatchmakingMode")
public func GKMatchmakerViewController_GetMatchmakingMode
(
    pointer: UnsafeMutablePointer<GKMatchmakerViewController>
) -> Int
{
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let target = pointer.takeUnretainedValue();
        return target.matchmakingMode.rawValue;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKMatchmakerViewController_SetMatchmakingMode")
public func GKMatchmakerViewController_SetMatchmakingMode
(
    pointer: UnsafeMutablePointer<GKMatchmakerViewController>,
    value: Int
)
{
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let target = pointer.takeUnretainedValue();
        target.matchmakingMode = GKMatchmakingMode.init(rawValue: value)!;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKMatchmakerViewController_GetIsHosted")
public func GKMatchmakerViewController_GetIsHosted
(
    pointer: UnsafeMutablePointer<GKMatchmakerViewController>
) -> Bool
{
    let target = pointer.takeUnretainedValue();
    return target.isHosted;
}

@_cdecl("GKMatchmakerViewController_SetIsHosted")
public func GKMatchmakerViewController_SetIsHosted
(
    pointer: UnsafeMutablePointer<GKMatchmakerViewController>,
    value: Bool
)
{
    let target = pointer.takeUnretainedValue();
    target.isHosted = value;
}

@_cdecl("GKMatchmakerViewController_SetHostedPlayerDidConnect")
public func GKMatchmakerViewController_SetHostedPlayerDidConnect
(
    gkMatchmakerViewControllerPtr: UnsafeMutablePointer<GKMatchmakerViewController>,
    gkPlayerPtr: UnsafeMutablePointer<GKPlayer>,
    didConnect: Bool
)
{
    let gkMatchmakerViewController = gkMatchmakerViewControllerPtr.takeUnretainedValue();
    let gkPlayer = gkPlayerPtr.takeUnretainedValue();
    gkMatchmakerViewController.setHostedPlayer(gkPlayer, didConnect:didConnect);
}

@_cdecl("GKMatchmakerViewController_Present")
public func GKMatchmakerViewController_Present
(
    pointer: UnsafeMutablePointer<GKMatchmakerViewController>
)
{
    let target = pointer.takeUnretainedValue();
    UiUtilities.presentViewController(viewController: target)
}

public func GKMatchmakerViewController_Dismiss
(
    viewController: GKMatchmakerViewController
)
{
    UiUtilities.dismissViewController(viewController: viewController)
    _activeRealtimeMatchmakerDelegate = nil;
}

public var _activeRealtimeMatchmakerDelegate : GKWMatchmakerViewControllerDelegate? = nil;

@_cdecl("GKMatchmakerViewController_GetMatchmakerDelegate")
public func GKMatchmakerViewController_GetMatchmakerDelegate
(
    pointer: UnsafeMutablePointer<GKMatchmakerViewController>
) -> UnsafeMutablePointer<GKWMatchmakerViewControllerDelegate>?
{
    let target = pointer.takeUnretainedValue();

    if (target.matchmakerDelegate == nil) {
        _activeRealtimeMatchmakerDelegate = GKWMatchmakerViewControllerDelegate();
        target.matchmakerDelegate = _activeRealtimeMatchmakerDelegate;
    }

    return _activeRealtimeMatchmakerDelegate!.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKMatchmakerViewController_AddPlayersToMatch")
public func GKMatchmakerViewController_AddPlayersToMatch
(
    gkMatchmakerViewControllerPtr: UnsafeMutablePointer<GKMatchmakerViewController>,
    gkMatchPtr: UnsafeMutablePointer<GKMatch>
)
{
    let gkMatchmakerViewController = gkMatchmakerViewControllerPtr.takeUnretainedValue();
    let gkMatch = gkMatchPtr.takeUnretainedValue();

    gkMatchmakerViewController.addPlayers(to: gkMatch);
}
