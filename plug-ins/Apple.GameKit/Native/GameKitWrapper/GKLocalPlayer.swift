//
//  GKLocalPlayer.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKLocalPlayer_Authenticate")
public func GKLocalPlayer_Authenticate
(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    if (GKLocalPlayer.local.authenticateHandler != nil) {
        onSuccess(taskId, GKLocalPlayer_GetLocal());
        return;
    }

    GKLocalPlayer.local.authenticateHandler = { gcAuthVC, error in
        // Always show the viewController if provided...
        if let gcAuthVC = gcAuthVC {
            UiUtilities.presentViewController(viewController: gcAuthVC);
        }

        if let error = error {
            onError(taskId, Unmanaged.passRetained(error as NSError).toOpaque());
            return;
        }

        GKLocalPlayer.local.register(_localPlayerListener);
        onSuccess(taskId, GKLocalPlayer_GetLocal());
    };
}

@_cdecl("GKLocalPlayer_GetLocal")
public func GKLocalPlayer_GetLocal
(
) -> UnsafeMutableRawPointer
{
    let player = GKLocalPlayer.local;
    return Unmanaged.passRetained(player).toOpaque();
}

@_cdecl("GKLocalPlayer_GetIsAuthenticated")
public func GKLocalPlayer_GetIsAuthenticated
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let player = Unmanaged<GKLocalPlayer>.fromOpaque(pointer).takeUnretainedValue();
    return player.isAuthenticated;
}

@_cdecl("GKLocalPlayer_GetIsUnderage")
public func GKLocalPlayer_GetIsUnderage
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let player = Unmanaged<GKLocalPlayer>.fromOpaque(pointer).takeUnretainedValue();
    return player.isUnderage;
}

@_cdecl("GKLocalPlayer_GetIsMultiplayerGamingRestricted")
public func GKLocalPlayer_GetIsMultiplayerGamingRestricted
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let player = Unmanaged<GKLocalPlayer>.fromOpaque(pointer).takeUnretainedValue();
    return player.isMultiplayerGamingRestricted;
}

@_cdecl("GKLocalPlayer_GetIsPersonalizedCommunicationRestricted")
public func GKLocalPlayer_GetIsPersonalizedCommunicationRestricted
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let player = Unmanaged<GKLocalPlayer>.fromOpaque(pointer).takeUnretainedValue();
    
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        return player.isPersonalizedCommunicationRestricted
    } else {
        return false;
    };
}

@_cdecl("GKLocalPlayer_LoadFriends")
public func GKLocalPlayer_LoadFriends
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let player = Unmanaged<GKLocalPlayer>.fromOpaque(pointer).takeUnretainedValue();
    if #available(iOS 14.5, tvOS 14.5, macOS 11.3, *) {
        player.loadFriends({ friends, error in
            if (error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
            
            if (friends != nil) {
                onSuccess(taskId, Unmanaged.passRetained(friends! as NSArray).toOpaque());
            } else {
                onSuccess(taskId, nil);
            }
        })
    } else {
        onSuccess(taskId, nil);
    };
}

@_cdecl("GKLocalPlayer_LoadFriendsWithIdentifiers")
public func GKLocalPlayer_LoadFriendsWithIdentifiers
(
    gkLocalPlayerPtr: UnsafeMutableRawPointer,
    taskId: Int64,
    identifiersPtr: UnsafeMutableRawPointer, // NSArray<NSString *> *
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let player = Unmanaged<GKLocalPlayer>.fromOpaque(gkLocalPlayerPtr).takeUnretainedValue();
    let identifiers = Unmanaged<NSArray>.fromOpaque(identifiersPtr).takeUnretainedValue() as! [String];
    guard #available(iOS 14.5, tvOS 14.5, macOS 11.3, *) else {
        // Return a nil friends list if the API isn't available.
        onSuccess(taskId, nil);
        return;
    }

    player.loadFriends(identifiedBy: identifiers, completionHandler: { friends, error in
        if (error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }

        onSuccess(taskId, friends.map { Unmanaged.passRetained($0 as NSArray).toOpaque() });
    });
}

@_cdecl("GKLocalPlayer_LoadChallengableFriends")
public func GKLocalPlayer_LoadChallengableFriends
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let player = Unmanaged<GKLocalPlayer>.fromOpaque(pointer).takeUnretainedValue();
    player.loadChallengableFriends(completionHandler: { friends, error in
        if (error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        if (friends != nil) {
            onSuccess(taskId, Unmanaged.passRetained(friends! as NSArray).toOpaque());
        } else {
            onSuccess(taskId, nil);
        }
    });
}

@_cdecl("GKLocalPlayer_LoadRecentPlayers")
public func GKLocalPlayer_LoadRecentPlayers
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let player = Unmanaged<GKLocalPlayer>.fromOpaque(pointer).takeUnretainedValue();
    player.loadRecentPlayers(completionHandler: { friends, error in
        if (error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        if (friends != nil) {
            onSuccess(taskId, Unmanaged.passRetained(friends! as NSArray).toOpaque());
        } else {
            onSuccess(taskId, nil);
        }
    });
}

@_cdecl("GKLocalPlayer_LoadFriendsAuthorizationStatus")
public func GKLocalPlayer_LoadFriendsAuthorizationStatus
(
    gkLocalPlayerPtr: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskIntCallback,
    onError: @escaping NSErrorCallback
)
{
    let player = Unmanaged<GKLocalPlayer>.fromOpaque(gkLocalPlayerPtr).takeUnretainedValue();
    if #available(iOS 14.5, tvOS 14.5, macOS 11.3, *) {
        player.loadFriendsAuthorizationStatus({ status, error in
            if (error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
            
            onSuccess(taskId, status.rawValue);
        })
    } else {
        // We pass 0 = 'NotDetermined' on unsupported platforms...
        onSuccess(taskId, 0);
    };
}

public typealias SuccessTaskFetchItemsCallback = @convention(c) (
    Int64 /*taskId*/,
    char_p /*publicKeyUrl*/,
    UnsafeMutableRawPointer /*signatureData*/,
    UnsafeMutableRawPointer /*saltData*/,
    UInt64 /*timestamp*/) -> Void;

@_cdecl("GKLocalPlayer_FetchItems")
public func GKLocalPlayer_FetchItems
(
    gkLocalPlayerPtr: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskFetchItemsCallback,
    onError: @escaping NSErrorCallback
)
{
    if #available(macOS 10.15.5, iOS 13.5, tvOS 13.5, *) {
        let player = Unmanaged<GKLocalPlayer>.fromOpaque(gkLocalPlayerPtr).takeUnretainedValue();
        player.fetchItems(forIdentityVerificationSignature: { publicKeyUrl, signature, salt, timestamp, error in
            if (error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }

            onSuccess(taskId,
                      publicKeyUrl!.absoluteString.toCharPCopy(),
                      Unmanaged.passRetained(signature! as NSData).toOpaque(),
                      Unmanaged.passRetained(salt! as NSData).toOpaque(),
                      UInt64(timestamp));
        })
    } else {
        let error = NSError.init(domain: "GKLocalPlayer", code: GKErrorCodeExtension.unsupportedOperationForOSVersion.rawValue, userInfo: nil);
        onError(taskId, Unmanaged.passRetained(error as NSError).toOpaque());
    };
}

@_cdecl("GKLocalPlayer_SaveGameData")
public func GKLocalPlayer_SaveGameData
(
    gkLocalPlayerPtr: UnsafeMutableRawPointer,
    taskId: Int64,
    data: InteropStructArray,
    name: char_p,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    if #available(macOS 10.10, iOS 8.0, *) {
        let player = Unmanaged<GKLocalPlayer>.fromOpaque(gkLocalPlayerPtr).takeUnretainedValue();
        player.saveGameData(_: data.toData(), withName: name.toString(), completionHandler: { savedGame, error in
            if (error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
            
            onSuccess(taskId, Unmanaged.passRetained(savedGame! as GKSavedGame).toOpaque());
        })
    }
    else {
        let error = NSError.init(domain: "GKLocalPlayer", code: GKErrorCodeExtension.unsupportedOperationForOSVersion.rawValue, userInfo: nil);
        onError(taskId, Unmanaged.passRetained(error as NSError).toOpaque());
    }
}

@_cdecl("GKLocalPlayer_FetchSavedGames")
public func GKLocalPlayer_FetchSavedGames
(
    gkLocalPlayerPtr: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    if #available(macOS 10.10, iOS 8.0, *) {
        let player = Unmanaged<GKLocalPlayer>.fromOpaque(gkLocalPlayerPtr).takeUnretainedValue();
        player.fetchSavedGames(completionHandler: { savedGames, error in
            if (error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
            
            if (savedGames != nil) {
                onSuccess(taskId, Unmanaged.passRetained(savedGames! as NSArray).toOpaque());
            } else {
                onSuccess(taskId, nil);
            }
        })
    }
    else {
        let error = NSError.init(domain: "GKLocalPlayer", code: GKErrorCodeExtension.unsupportedOperationForOSVersion.rawValue, userInfo: nil);
        onError(taskId, Unmanaged.passRetained(error as NSError).toOpaque());
    }
}

@_cdecl("GKLocalPlayer_ResolveConflictingSavedGames")
public func GKLocalPlayer_ResolveConflictingSavedGames
(
    gkLocalPlayerPtr: UnsafeMutableRawPointer,
    taskId: Int64,
    _ gkConflictingSavedGames: UnsafeMutableRawPointer,
    data: InteropStructArray,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    if #available(macOS 10.10, iOS 8.0, *) {
        let player = Unmanaged<GKLocalPlayer>.fromOpaque(gkLocalPlayerPtr).takeUnretainedValue();
        let conflictingSavedGames = Unmanaged<NSArray>.fromOpaque(gkConflictingSavedGames).takeUnretainedValue() as! [GKSavedGame];
        player.resolveConflictingSavedGames(_: conflictingSavedGames, with: data.toData(), completionHandler: { savedGames, error in
            if (error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
            
            if (savedGames != nil) {
                onSuccess(taskId, Unmanaged.passRetained(savedGames! as NSArray).toOpaque());
            } else {
                onSuccess(taskId, nil);
            }
        })
    }
    else {
        let error = NSError.init(domain: "GKLocalPlayer", code: GKErrorCodeExtension.unsupportedOperationForOSVersion.rawValue, userInfo: nil);
        onError(taskId, Unmanaged.passRetained(error as NSError).toOpaque());
    }
}

@_cdecl("GKLocalPlayer_DeleteSavedGames")
public func GKLocalPlayer_DeleteSavedGames
(
    gkLocalPlayerPtr: UnsafeMutableRawPointer,
    taskId: Int64,
    name: char_p,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    if #available(macOS 10.10, iOS 8.0, *) {
        let player = Unmanaged<GKLocalPlayer>.fromOpaque(gkLocalPlayerPtr).takeUnretainedValue();
        player.deleteSavedGames(withName: name.toString(), completionHandler: { error in
            if (error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
            
            onSuccess(taskId);
        })
    }
    else {
        let error = NSError.init(domain: "GKLocalPlayer", code: GKErrorCodeExtension.unsupportedOperationForOSVersion.rawValue, userInfo: nil);
        onError(taskId, Unmanaged.passRetained(error as NSError).toOpaque());
    }
}
