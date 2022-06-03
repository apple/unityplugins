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
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let invite = Unmanaged<GKInvite>.fromOpaque(pointer).takeUnretainedValue();
    let target = GKMatchmakerViewController.init(invite: invite);
    
    return Unmanaged.passRetained(target!).toOpaque();
}

@_cdecl("GKMatchmakerViewController_InitWithMatchRequest")
public func GKMatchmakerViewController_InitWithMatchRequest
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let request = Unmanaged<GKMatchRequest>.fromOpaque(pointer).takeUnretainedValue();
    let target = GKMatchmakerViewController.init(matchRequest: request);
    
    return Unmanaged.passRetained(target!).toOpaque();
}

@_cdecl("GKMatchmakerViewController_Free")
public func GKMatchmakerViewController_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKMatchmakerViewController>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKMatchmakerViewController_GetMatchRequest")
public func GKMatchmakerViewController_GetMatchRequest
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let target = Unmanaged<GKMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
    return Unmanaged.passRetained(target.matchRequest).toOpaque();
}

@_cdecl("GKMatchmakerViewController_GetCanStartWithMinimumPlayers")
public func GKMatchmakerViewController_GetCanStartWithMinimumPlayers
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let target = Unmanaged<GKMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
    if #available(iOS 15, tvOS 15, macOS 12.0, *) {
        return target.canStartWithMinimumPlayers
    } else {
        return false;
    };
}

@_cdecl("GKMatchmakerViewController_SetCanStartWithMinimumPlayers")
public func GKMatchmakerViewController_SetCanStartWithMinimumPlayers
(
    pointer: UnsafeMutableRawPointer,
    value: Bool
)
{
    let target = Unmanaged<GKMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
    if #available(iOS 15, tvOS 15, macOS 12.0, *) {
        target.canStartWithMinimumPlayers = value
    }
}

@_cdecl("GKMatchmakerViewController_GetMatchmakingMode")
public func GKMatchmakerViewController_GetMatchmakingMode
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
    return target.matchmakingMode.rawValue;
}

@_cdecl("GKMatchmakerViewController_SetMatchmakingMode")
public func GKMatchmakerViewController_SetMatchmakingMode
(
    pointer: UnsafeMutableRawPointer,
    value: Int
)
{
    let target = Unmanaged<GKMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
    target.matchmakingMode = GKMatchmakingMode.init(rawValue: value)!;
}

@_cdecl("GKMatchmakerViewController_GetIsHosted")
public func GKMatchmakerViewController_GetIsHosted
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let target = Unmanaged<GKMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
    return target.isHosted;
}

@_cdecl("GKMatchmakerViewController_SetIsHosted")
public func GKMatchmakerViewController_SetIsHosted
(
    pointer: UnsafeMutableRawPointer,
    value: Bool
)
{
    let target = Unmanaged<GKMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
    target.isHosted = value;
}

@_cdecl("GKMatchmakerViewController_Present")
public func GKMatchmakerViewController_Present
(
    pointer: UnsafeMutableRawPointer
)
{
    let target = Unmanaged<GKMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
    
#if os(iOS) || os(tvOS)
    let viewController = UIApplication.shared.windows.first!.rootViewController;
    viewController?.present(target, animated: true);
#else
    GKDialogController.shared().parentWindow = NSApplication.shared.keyWindow;
    GKDialogController.shared().present(target);
#endif
}

public func GKMatchmakerViewController_Dismiss
(
    viewController: GKMatchmakerViewController
)
{
    #if os(iOS) || os(tvOS)
        viewController.dismiss(animated: true);
    #else
        GKDialogController.shared().dismiss(viewController);
    #endif
    
    _activeRealtimeMatchmakerDelegate = nil;
}

public var _activeRealtimeMatchmakerDelegate : GKWMatchmakerViewControllerDelegate? = nil;

@_cdecl("GKMatchmakerViewController_GetMatchmakerDelegate")
public func GKMatchmakerViewController_GetMatchmakerDelegate
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKMatchmakerViewController>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.matchmakerDelegate == nil) {
        _activeRealtimeMatchmakerDelegate = GKWMatchmakerViewControllerDelegate();
        target.matchmakerDelegate = _activeRealtimeMatchmakerDelegate;
    }
    
    return Unmanaged.passRetained(target.matchmakerDelegate!).toOpaque();
}
