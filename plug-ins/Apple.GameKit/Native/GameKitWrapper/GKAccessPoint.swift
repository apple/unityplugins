//
//  AccessPoint.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKAccessPoint_GetShared")
public func GKAccessPoint_GetShared
(
) -> UnsafeMutableRawPointer // GKAccessPoint
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        return GKAccessPoint.shared.passRetainedUnsafeMutableRawPointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKAccessPoint_Trigger")
public func GKAccessPoint_Trigger
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        if (!target.isPresentingGameCenter) {
            target.trigger(handler: {
                onSuccess(taskId);
            });
        } else {
            onError(taskId, NSError(code: GKErrorCodeExtension.gameCenterDashboardAlreadyShown).passRetainedUnsafeMutablePointer());
        }
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKAccessPoint_TriggerWithState")
public func GKAccessPoint_TriggerWithState
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    state: Int64, // GKGameCenterViewControllerState
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        if (!target.isPresentingGameCenter) {
            if let stateEnum = GKGameCenterViewControllerState(rawValue: Int(truncatingIfNeeded: state)) {
                target.trigger(
                    state: stateEnum,
                    handler: {
                        onSuccess(taskId);
                    });
            } else {
                onError(taskId, NSError(code: GKError.Code.invalidParameter).passRetainedUnsafeMutablePointer());
            }
        } else {
            onError(taskId, NSError(code: GKErrorCodeExtension.gameCenterDashboardAlreadyShown).passRetainedUnsafeMutablePointer());
        }
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKAccessPoint_TriggerWithAchievementID")
public func GKAccessPoint_TriggerWithAchievementID
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    achievementIDPtr: UnsafeMutablePointer<NSString>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 18.0, macOS 15.0, tvOS 18.0, visionOS 2.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        let achievementID = achievementIDPtr.takeUnretainedValue();

        if (!target.isPresentingGameCenter) {
            target.trigger(
                achievementID: achievementID as String,
                handler: {
                    onSuccess(taskId);
                });
        } else {
            onError(taskId, NSError(code: GKErrorCodeExtension.gameCenterDashboardAlreadyShown).passRetainedUnsafeMutablePointer());
        }
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKAccessPoint_TriggerWithLeaderboardSetID")
public func GKAccessPoint_TriggerWithLeaderboardSetID
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    leaderboardSetIDPtr: UnsafeMutablePointer<NSString>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 18.0, macOS 15.0, tvOS 18.0, visionOS 2.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        let leaderboardSetID = leaderboardSetIDPtr.takeUnretainedValue();

        if (!target.isPresentingGameCenter) {
            target.trigger(
                leaderboardSetID: leaderboardSetID as String,
                handler: {
                    onSuccess(taskId);
                });
        } else {
            onError(taskId, NSError(code: GKErrorCodeExtension.gameCenterDashboardAlreadyShown).passRetainedUnsafeMutablePointer());
        }
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKAccessPoint_TriggerWithLeaderboardID")
public func GKAccessPoint_TriggerWithLeaderboardID
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    leaderboardIDPtr: UnsafeMutablePointer<NSString>,
    playerScope: Int,
    timeScope: Int,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 18.0, macOS 15.0, tvOS 18.0, visionOS 2.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        let leaderboardID = leaderboardIDPtr.takeUnretainedValue();

        if (!target.isPresentingGameCenter) {
            target.trigger(
                leaderboardID: leaderboardID as String,
                playerScope: GKLeaderboard.PlayerScope.init(rawValue: playerScope)!,
                timeScope: GKLeaderboard.TimeScope.init(rawValue: timeScope)!,
                handler: {
                    onSuccess(taskId);
                });
        } else {
            onError(taskId, NSError(code: GKErrorCodeExtension.gameCenterDashboardAlreadyShown).passRetainedUnsafeMutablePointer());
        }
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKAccessPoint_TriggerWithPlayer")
public func GKAccessPoint_TriggerWithPlayer
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    playerPtr: UnsafeMutablePointer<GKPlayer>, // GKPlayer
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 18.0, macOS 15.0, tvOS 18.0, visionOS 2.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        let player = playerPtr.takeUnretainedValue();

        if (!target.isPresentingGameCenter) {
            target.trigger(
                player: player,
                handler: {
                    onSuccess(taskId);
                });
        } else {
            onError(taskId, NSError(code: GKErrorCodeExtension.gameCenterDashboardAlreadyShown).passRetainedUnsafeMutablePointer());
        }
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

#if os(iOS) || os(macOS)
@_cdecl("GKAccessPoint_TriggerForPlayTogether")
public func GKAccessPoint_TriggerForPlayTogether
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 26.0, macOS 26.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        if (!target.isPresentingGameCenter) {
            target.triggerForPlayTogether(handler: {
                onSuccess(taskId);
            });
        } else {
            onError(taskId, NSError(code: GKErrorCodeExtension.gameCenterDashboardAlreadyShown).passRetainedUnsafeMutablePointer());
        }
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}
#endif

#if os(iOS) || os(macOS)
@_cdecl("GKAccessPoint_TriggerForChallenges")
public func GKAccessPoint_TriggerForChallenges
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 26.0, macOS 26.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        if (!target.isPresentingGameCenter) {
            target.triggerForChallenges(handler: {
                onSuccess(taskId);
            });
        } else {
            onError(taskId, NSError(code: GKErrorCodeExtension.gameCenterDashboardAlreadyShown).passRetainedUnsafeMutablePointer());
        }
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}
#endif

#if os(iOS) || os(macOS)
@_cdecl("GKAccessPoint_TriggerWithChallengeDefinitionID")
public func GKAccessPoint_TriggerWithChallengeDefinitionID
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    challengeDefinitionIDPtr: UnsafeMutablePointer<NSString>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 26.0, macOS 26.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        let challengeDefinitionID = challengeDefinitionIDPtr.takeUnretainedValue() as String;
        if (!target.isPresentingGameCenter) {
            target.trigger(
                challengeDefinitionID: challengeDefinitionID,
                handler: {
                    onSuccess(taskId);
                });
        } else {
            onError(taskId, NSError(code: GKErrorCodeExtension.gameCenterDashboardAlreadyShown).passRetainedUnsafeMutablePointer());
        }
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}
#endif

#if os(iOS) || os(macOS)
@_cdecl("GKAccessPoint_TriggerWithGameActivity")
public func GKAccessPoint_TriggerWithGameActivity
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    gameActivityPointer: UnsafeMutableRawPointer, // GKGameActivity
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    
    guard #available(iOS 26.0, macOS 26.0, *) else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
        return;
    }
    
    let target: GKAccessPoint = pointer.takeUnretainedValue();
    let gameActivity = gameActivityPointer.takeUnretainedValue() as GKGameActivity;
    if (!target.isPresentingGameCenter) {
        target.trigger(
            gameActivity: gameActivity,
            handler: {
                onSuccess(taskId);
            });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.gameCenterDashboardAlreadyShown).passRetainedUnsafeMutablePointer());
    }
}

#endif

#if os(iOS) || os(macOS)
@_cdecl("GKAccessPoint_TriggerWithGameActivityDefinitionID")
public func GKAccessPoint_TriggerWithGameActivityDefinitionID
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    gameActivityDefinitionIDPtr: UnsafeMutablePointer<NSString>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 26.0, macOS 26.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        let gameActivityDefinitionID = gameActivityDefinitionIDPtr.takeUnretainedValue() as String;
        if (!target.isPresentingGameCenter) {
            target.trigger(
                gameActivityDefinitionID: gameActivityDefinitionID,
                handler: {
                    onSuccess(taskId);
                });
        } else {
            onError(taskId, NSError(code: GKErrorCodeExtension.gameCenterDashboardAlreadyShown).passRetainedUnsafeMutablePointer());
        }
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}
#endif

#if os(iOS) || os(macOS)
@_cdecl("GKAccessPoint_TriggerForFriending")
public func GKAccessPoint_TriggerForFriending
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 26.0, macOS 26.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        if (!target.isPresentingGameCenter) {
            target.triggerForFriending(handler: {
                onSuccess(taskId);
            });
        } else {
            onError(taskId, NSError(code: GKErrorCodeExtension.gameCenterDashboardAlreadyShown).passRetainedUnsafeMutablePointer());
        }
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}
#endif

@_cdecl("GKAccessPoint_GetLocation")
public func GKAccessPoint_GetLocation
(
    pointer: UnsafeMutableRawPointer // GKAccessPoint
) -> Int
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        return target.location.rawValue;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKAccessPoint_SetLocation")
public func GKAccessPoint_SetLocation
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    value: Int
)
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        target.location = GKAccessPoint.Location.init(rawValue: value)!;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKAccessPoint_GetShowHighlights")
public func GKAccessPoint_GetShowHighlights
(
    pointer: UnsafeMutableRawPointer // GKAccessPoint
) -> Bool
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        return target.showHighlights;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKAccessPoint_SetShowHighlights")
public func GKAccessPoint_SetShowHighlights
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    value: Bool
)
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        target.showHighlights = value;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKAccessPoint_GetIsPresentingGameCenter")
public func GKAccessPoint_GetIsPresentingGameCenter
(
    pointer: UnsafeMutableRawPointer // GKAccessPoint
) -> Bool
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        return target.isPresentingGameCenter;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

#if os(tvOS)
@_cdecl("GKAccessPoint_GetIsFocused")
public func GKAccessPoint_GetIsFocused
(
    pointer: UnsafeMutableRawPointer // GKAccessPoint
) -> Bool
{
    if #available(tvOS 14.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        return target.isFocused;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}
#endif

@_cdecl("GKAccessPoint_GetIsVisible")
public func GKAccessPoint_GetIsVisible
(
    pointer: UnsafeMutableRawPointer // GKAccessPoint
) -> Bool
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        return target.isVisible;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKAccessPoint_GetIsActive")
public func GKAccessPoint_GetIsActive
(
    pointer: UnsafeMutableRawPointer // GKAccessPoint
) -> Bool
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        return target.isActive;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKAccessPoint_SetIsActive")
public func GKAccessPoint_SetIsActive
(
    pointer: UnsafeMutableRawPointer, // GKAccessPoint
    value: Bool
)
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();

        if value {
            target.parentWindow = UiUtilities.defaultWindow()
        }

        target.isActive = value;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKAccessPoint_GetFrameInScreenCoordinates")
public func GKAccessPoint_GetFrameInScreenCoordinates
(
    pointer: UnsafeMutableRawPointer // GKAccessPoint
) -> GKWAccessPointFrameInScreenCoordinates
{
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, visionOS 1.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        let rect = target.frameInScreenCoordinates;

        return GKWAccessPointFrameInScreenCoordinates(
            x: Float(rect.minX),
            y: Float(rect.minY),
            width: Float(rect.width),
            height: Float(rect.height));
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKAccessPoint_GetFrameInUnitCoordinates")
public func GKAccessPoint_GetFrameInUnitCoordinates
(
    pointer: UnsafeMutableRawPointer // GKAccessPoint
) -> GKWAccessPointFrameInScreenCoordinates
{
#if os(visionOS)
    // Not available on visionOS because UIScreen is not available.
    DefaultNSErrorHandler.throwApiUnavailableError();
#else
    if #available(iOS 14.0, macOS 11.0, tvOS 14.0, *) {
        let target: GKAccessPoint = pointer.takeUnretainedValue();
        let rect = target.frameInScreenCoordinates;

#if os(macOS)
        let screenSize = NSScreen.main!.frame;
#else
        let screenSize = UIScreen.main.bounds;
#endif

        return GKWAccessPointFrameInScreenCoordinates(
            x: Float(rect.minX / screenSize.width),
            y: Float(rect.minY / screenSize.height),
            width: Float(rect.width / screenSize.width),
            height: Float(rect.height / screenSize.height));
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
#endif
}
