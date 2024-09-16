//
//  GKLocalPlayer.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

// Cache the most recent authenticateHandler callback result so it can be sent
// back to the client on subsequent calls to set the authenticateHandler.
// Strictly speaking, the authenticateHandler should only be set once per process
// lifetime. However, when running an app repeatedly in the Unity Editor, this
// native plug-in isn't reloaded between runs so the previously set handler persists.
// We improve the developer experience in this case by returning the previously
// cached result to simulate the process being torn down and restarted on every run.
var _mostRecentAuthenticatePlayer : GKLocalPlayer? = nil;
var _mostRecentAuthenticateError : NSError? = nil;

var _onAuthenticate : SuccessTaskPtrCallback? = nil;
var _onAuthenticateError : NSErrorTaskCallback? = nil;

// Collection of pending auth requests.
var _pendingAuthTasks = Array<Int64>();

@_cdecl("GKLocalPlayer_SetAuthenticateHandler")
public func GKLocalPlayer_SetAuthenticateHandler
(
    taskId: Int64,
    onAuthenticate: @escaping SuccessTaskPtrCallback,
    onAuthenticateError: @escaping NSErrorTaskCallback
)
{
    _onAuthenticate = onAuthenticate;
    _onAuthenticateError = onAuthenticateError;

    // The authenticateHandler can't be set more than once per process lifetime.
    if (GKLocalPlayer.local.authenticateHandler == nil) {
        _pendingAuthTasks.append(taskId);
        GKLocalPlayer.local.authenticateHandler = { gcAuthVC, error in

            // Always show the viewController if provided.
            if let gcAuthVC {
                UiUtilities.presentViewController(viewController: gcAuthVC);
            }

            if let error = error as? NSError {
                _mostRecentAuthenticateError = error;
                _mostRecentAuthenticatePlayer = nil;

                if (_onAuthenticateError != nil) {
                    if (_pendingAuthTasks.isEmpty) {
                        // There are no waiting tasks, but we still need to call the handler in case an event is hooked up.
                        // Just use a bogus (unused) task id.
                        _onAuthenticateError!(0, _mostRecentAuthenticateError!.passRetainedUnsafeMutablePointer());
                    } else {
                        for tid in _pendingAuthTasks {
                            _onAuthenticateError!(tid, _mostRecentAuthenticateError!.passRetainedUnsafeMutablePointer());
                        }
                    }
                }
            } else {
                _mostRecentAuthenticatePlayer = GKLocalPlayer.local;
                _mostRecentAuthenticateError = nil;

                GKLocalPlayer.local.unregisterAllListeners();
                GKLocalPlayer.local.register(_localPlayerListener);

                if (_onAuthenticate != nil) {
                    if (_pendingAuthTasks.isEmpty) {
                        // There are no waiting tasks, but we still need to call the handler in case an event is hooked up.
                        // Just use a bogus (unused) task id.
                        _onAuthenticate!(0, _mostRecentAuthenticatePlayer!.passRetainedUnsafeMutablePointer());
                    } else {
                        for tid in _pendingAuthTasks {
                            _onAuthenticate!(tid, _mostRecentAuthenticatePlayer!.passRetainedUnsafeMutablePointer());
                        }
                    }
                }
            }

            _pendingAuthTasks.removeAll();
        }
    } else {
        // Just return the most recent result, if there is one.
        if (_mostRecentAuthenticatePlayer != nil) {
            if (_onAuthenticate != nil) {
                _onAuthenticate!(taskId, _mostRecentAuthenticatePlayer!.passRetainedUnsafeMutablePointer());
            }
        } else if (_mostRecentAuthenticateError != nil) {
            if (_onAuthenticateError != nil) {
                _onAuthenticateError!(taskId, _mostRecentAuthenticateError!.passRetainedUnsafeMutablePointer());
            }
        } else {
            // There is no previous result so wait until we get one.
            _pendingAuthTasks.append(taskId);
        }
    }
}

@_cdecl("GKLocalPlayer_GetLocal")
public func GKLocalPlayer_GetLocal
(
) -> UnsafeMutablePointer<GKLocalPlayer>
{
    return GKLocalPlayer.local.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKLocalPlayer_GetIsAuthenticated")
public func GKLocalPlayer_GetIsAuthenticated
(
    pointer: UnsafeMutablePointer<GKLocalPlayer>
) -> Bool
{
    let player = pointer.takeUnretainedValue();
    return player.isAuthenticated;
}

@_cdecl("GKLocalPlayer_GetIsUnderage")
public func GKLocalPlayer_GetIsUnderage
(
    pointer: UnsafeMutablePointer<GKLocalPlayer>
) -> Bool
{
    let player = pointer.takeUnretainedValue();
    return player.isUnderage;
}

@_cdecl("GKLocalPlayer_GetIsMultiplayerGamingRestricted")
public func GKLocalPlayer_GetIsMultiplayerGamingRestricted
(
    pointer: UnsafeMutablePointer<GKLocalPlayer>
) -> Bool
{
    let player = pointer.takeUnretainedValue();
    return player.isMultiplayerGamingRestricted;
}

@_cdecl("GKLocalPlayer_GetIsPersonalizedCommunicationRestricted")
public func GKLocalPlayer_GetIsPersonalizedCommunicationRestricted
(
    pointer: UnsafeMutablePointer<GKLocalPlayer>
) -> Bool
{
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let player = pointer.takeUnretainedValue();
        return player.isPersonalizedCommunicationRestricted
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    };
}

@_cdecl("GKLocalPlayer_LoadFriends")
public func GKLocalPlayer_LoadFriends
(
    pointer: UnsafeMutablePointer<GKLocalPlayer>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let player = pointer.takeUnretainedValue();
    if #available(iOS 14.5, tvOS 14.5, macOS 11.3, *) {
        player.loadFriends({ friends, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, (friends as? NSArray)?.passRetainedUnsafeMutablePointer());
        })
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    };
}

@_cdecl("GKLocalPlayer_LoadFriendsWithIdentifiers")
public func GKLocalPlayer_LoadFriendsWithIdentifiers
(
    gkLocalPlayerPtr: UnsafeMutablePointer<GKLocalPlayer>,
    taskId: Int64,
    identifiersPtr: UnsafeMutablePointer<NSArray>, // NSArray<NSString *> *
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 14.5, tvOS 14.5, macOS 11.3, *) {
        let player = gkLocalPlayerPtr.takeUnretainedValue();
        let identifiers = identifiersPtr.takeUnretainedValue() as! [String];

        player.loadFriends(identifiedBy: identifiers, completionHandler: { friends, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, (friends as? NSArray)?.passRetainedUnsafeMutablePointer());
        });
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    }
}

@_cdecl("GKLocalPlayer_LoadChallengableFriends")
public func GKLocalPlayer_LoadChallengableFriends
(
    pointer: UnsafeMutablePointer<GKLocalPlayer>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let player = pointer.takeUnretainedValue();
    player.loadChallengableFriends(completionHandler: { friends, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, (friends as? NSArray)?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKLocalPlayer_LoadRecentPlayers")
public func GKLocalPlayer_LoadRecentPlayers
(
    pointer: UnsafeMutablePointer<GKLocalPlayer>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let player = pointer.takeUnretainedValue();
    player.loadRecentPlayers(completionHandler: { friends, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, (friends as? NSArray)?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKLocalPlayer_LoadFriendsAuthorizationStatus")
public func GKLocalPlayer_LoadFriendsAuthorizationStatus
(
    gkLocalPlayerPtr: UnsafeMutablePointer<GKLocalPlayer>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskIntCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let player = gkLocalPlayerPtr.takeUnretainedValue();
    if #available(iOS 14.5, tvOS 14.5, macOS 11.3, *) {
        player.loadFriendsAuthorizationStatus({ status, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId, status.rawValue);
        })
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    };
}

public typealias SuccessTaskFetchItemsCallback = @convention(c) (
    Int64 /*taskId*/,
    char_p /*publicKeyUrl*/,
    UnsafeMutablePointer<NSData> /*signatureData*/,
    UnsafeMutablePointer<NSData> /*saltData*/,
    UInt64 /*timestamp*/) -> Void;

@_cdecl("GKLocalPlayer_FetchItemsForIdentityVerificationSignature")
public func GKLocalPlayer_FetchItemsForIdentityVerificationSignature
(
    gkLocalPlayerPtr: UnsafeMutablePointer<GKLocalPlayer>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskFetchItemsCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(macOS 10.15.5, iOS 13.5, tvOS 13.5, *) {
        let player = gkLocalPlayerPtr.takeUnretainedValue();
        player.fetchItems(forIdentityVerificationSignature: { publicKeyUrl, signature, salt, timestamp, error in
            if let error = error as? NSError {
                onError(taskId, error.passRetainedUnsafeMutablePointer());
                return;
            }

            onSuccess(taskId,
                      publicKeyUrl!.absoluteString.toCharPCopy(),
                      (signature! as NSData).passRetainedUnsafeMutablePointer(),
                      (salt! as NSData).passRetainedUnsafeMutablePointer(),
                      UInt64(timestamp));
        })
    } else {
        onError(taskId, NSError(code: GKErrorCodeExtension.unsupportedOperationForOSVersion).passRetainedUnsafeMutablePointer());
    };
}

@_cdecl("GKLocalPlayer_LoadDefaultLeaderboardIdentifier")
public func GKLocalPlayer_LoadDefaultLeaderboardIdentifier
(
    gkLocalPlayerPtr: UnsafeMutablePointer<GKLocalPlayer>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let player = gkLocalPlayerPtr.takeUnretainedValue();
    player.loadDefaultLeaderboardIdentifier(completionHandler: { identifier, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, Unmanaged.passRetained(identifier! as NSString).toOpaque());
    });
}

@_cdecl("GKLocalPlayer_SetDefaultLeaderboardIdentifier")
public func GKLocalPlayer_SetDefaultLeaderboardIdentifier
(
    gkLocalPlayerPtr: UnsafeMutablePointer<GKLocalPlayer>,
    taskId: Int64,
    identifierPtr : UnsafeMutablePointer<NSString>,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let player = gkLocalPlayerPtr.takeUnretainedValue();
    let identifier = identifierPtr.takeUnretainedValue();
    player.setDefaultLeaderboardIdentifier(identifier as String, completionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId);
    });
}

#if !os(tvOS)
@_cdecl("GKLocalPlayer_SaveGameData")
public func GKLocalPlayer_SaveGameData
(
    gkLocalPlayerPtr: UnsafeMutablePointer<GKLocalPlayer>,
    taskId: Int64,
    nsDataPtr: UnsafeMutablePointer<NSData>,
    name: char_p,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let player = gkLocalPlayerPtr.takeUnretainedValue();
    let data = nsDataPtr.takeUnretainedValue();
    player.saveGameData(data as Data, withName: name.toString(), completionHandler: { savedGame, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, savedGame?.passRetainedUnsafeMutablePointer());
    });
}
#endif

#if !os(tvOS)
@_cdecl("GKLocalPlayer_FetchSavedGames")
public func GKLocalPlayer_FetchSavedGames
(
    gkLocalPlayerPtr: UnsafeMutablePointer<GKLocalPlayer>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let player = gkLocalPlayerPtr.takeUnretainedValue();
    player.fetchSavedGames(completionHandler: { savedGames, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, (savedGames as? NSArray)?.passRetainedUnsafeMutablePointer());
    });
}
#endif

#if !os(tvOS)
@_cdecl("GKLocalPlayer_ResolveConflictingSavedGames")
public func GKLocalPlayer_ResolveConflictingSavedGames
(
    gkLocalPlayerPtr: UnsafeMutablePointer<GKLocalPlayer>,
    taskId: Int64,
    conflictingSavedGamesPtr: UnsafeMutablePointer<NSArray>, // NSArray<GKSavedGame>
    nsDataPtr: UnsafeMutablePointer<NSData>,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let player = gkLocalPlayerPtr.takeUnretainedValue();
    let conflictingSavedGames = conflictingSavedGamesPtr.takeUnretainedValue() as! [GKSavedGame];
    let data = nsDataPtr.takeUnretainedValue() as Data;
    player.resolveConflictingSavedGames(conflictingSavedGames, with: data, completionHandler: { savedGames, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, (savedGames as? NSArray)?.passRetainedUnsafeMutablePointer());
    });
}
#endif

#if !os(tvOS)
@_cdecl("GKLocalPlayer_DeleteSavedGames")
public func GKLocalPlayer_DeleteSavedGames
(
    gkLocalPlayerPtr: UnsafeMutablePointer<GKLocalPlayer>,
    taskId: Int64,
    name: char_p,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let player = gkLocalPlayerPtr.takeUnretainedValue();
    player.deleteSavedGames(withName: name.toString(), completionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId);
    });
}
#endif
