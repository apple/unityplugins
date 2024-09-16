//
//  GKGameCenterViewController.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

var _currentPresentingGameCenterDelegate : GameKitUIDelegateHandler? = nil;

@_cdecl("GKGameCenterViewController_InitWithState")
public func GKGameCenterViewController_InitWithState
(
    state: Int
) -> UnsafeMutablePointer<GKGameCenterViewController>
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target = GKGameCenterViewController.init(state: GKGameCenterViewControllerState.init(rawValue: state)!);
        return target.passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameCenterViewController_InitWithLeaderboardID")
public func GKGameCenterViewController_InitWithLeaderboardID
(
    leaderboardIDPtr: UnsafeMutablePointer<NSString>,
    playerScope: Int,
    timeScope: Int
) -> UnsafeMutablePointer<GKGameCenterViewController>
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let leaderboardID = leaderboardIDPtr.takeUnretainedValue();
        let target = GKGameCenterViewController.init(
            leaderboardID: leaderboardID as String,
            playerScope: GKLeaderboard.PlayerScope.init(rawValue: playerScope)!,
            timeScope: GKLeaderboard.TimeScope.init(rawValue: timeScope)!);

        return target.passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameCenterViewController_InitWithLeaderboard")
public func GKGameCenterViewController_InitWithLeaderboard
(
    leaderboardPtr: UnsafeMutablePointer<GKLeaderboard>,
    playerScope: Int
) -> UnsafeMutablePointer<GKGameCenterViewController>
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let leaderboard = leaderboardPtr.takeUnretainedValue();
        let target = GKGameCenterViewController.init(
            leaderboard: leaderboard,
            playerScope: GKLeaderboard.PlayerScope.init(rawValue: playerScope)!);

        return target.passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameCenterViewController_InitWithAchievementID")
public func GKGameCenterViewController_InitWithAchievementID
(
    achievementIDPtr: UnsafeMutablePointer<NSString>
) -> UnsafeMutablePointer<GKGameCenterViewController>
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let achievementID = achievementIDPtr.takeUnretainedValue();
        let target = GKGameCenterViewController.init(achievementID: achievementID as String);

        return target.passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameCenterViewController_InitWithLeaderboardSetID")
public func GKGameCenterViewController_InitWithLeaderboardSetID
(
    leaderboardSetIDPtr: UnsafeMutablePointer<NSString>
) -> UnsafeMutablePointer<GKGameCenterViewController>
{
    if #available(iOS 18.0, macOS 15.0, tvOS 18.0, visionOS 2.0, *) {
        let leaderboardSetID = leaderboardSetIDPtr.takeUnretainedValue();
        let target = GKGameCenterViewController.init(leaderboardSetID: leaderboardSetID as String);

        return target.passRetainedUnsafeMutablePointer();
    } else {
        // API unsupported on this platform
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}
@_cdecl("GKGameCenterViewController_InitWithPlayer")
public func GKGameCenterViewController_InitWithPlayer
(
    gkPlayerPtr: UnsafeMutablePointer<GKPlayer>
) -> UnsafeMutablePointer<GKGameCenterViewController>
{
    if #available(iOS 18.0, macOS 15.0, tvOS 18.0, visionOS 2.0, *) {
        let gkPlayer = gkPlayerPtr.takeUnretainedValue();
        let target = GKGameCenterViewController.init(player: gkPlayer);

        return target.passRetainedUnsafeMutablePointer();
    } else {
        // API unsupported on this platform
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}


@_cdecl("GKGameCenterViewController_Present")
public func GKGameCenterViewController_Present
(
    pointer: UnsafeMutablePointer<GKGameCenterViewController>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    _currentPresentingGameCenterDelegate = GameKitUIDelegateHandler(taskId: taskId, onSuccess: onSuccess);
    target.gameCenterDelegate = _currentPresentingGameCenterDelegate;

    UiUtilities.presentViewController(viewController: target)
}
