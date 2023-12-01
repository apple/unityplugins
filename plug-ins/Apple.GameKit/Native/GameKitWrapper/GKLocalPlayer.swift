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
    if(GKLocalPlayer.local.authenticateHandler != nil) {
        onSuccess(taskId, GKLocalPlayer_GetLocal());
        return;
    }
    
    GKLocalPlayer.local.authenticateHandler = { gcAuthVC, error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        // Always show the viewController if provided...
        if gcAuthVC != nil {
            #if os(iOS) || os(tvOS)
                let viewController = UIApplication.shared.windows.first!.rootViewController;
                viewController?.present(gcAuthVC!, animated: true)
            #else
                let viewController = NSApplication.shared.keyWindow?.contentViewController;
                viewController?.presentAsModalWindow(gcAuthVC!)
            #endif
        } else {
            GKLocalPlayer.local.register(_localPlayerListener);
            onSuccess(taskId, GKLocalPlayer_GetLocal());
        }
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
            if(error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
            
            if(friends != nil) {
                onSuccess(taskId, Unmanaged.passRetained(friends! as NSArray).toOpaque());
            } else {
                onSuccess(taskId, nil);
            }
        })
    } else {
        onSuccess(taskId, nil);
    };
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
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        if(friends != nil) {
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
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        if(friends != nil) {
            onSuccess(taskId, Unmanaged.passRetained(friends! as NSArray).toOpaque());
        } else {
            onSuccess(taskId, nil);
        }
    });
}

@_cdecl("GKLocalPlayer_LoadFriendsAuthorizationStatus")
public func GKLocalPlayer_LoadFriendsAuthorizationStatus
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskIntCallback,
    onError: @escaping NSErrorCallback
)
{
    let player = Unmanaged<GKLocalPlayer>.fromOpaque(pointer).takeUnretainedValue();
    if #available(iOS 14.5, tvOS 14.5, macOS 11.3, *) {
        player.loadFriendsAuthorizationStatus({ status, error in
            if(error != nil) {
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

public typealias SuccessTaskFetchItemsCallback = @convention(c) (Int64, UInt, uchar_p, Int32, uchar_p, Int32, uchar_p, Int32) -> Void;

@_cdecl("GKLocalPlayer_FetchItems")
public func GKLocalPlayer_FetchItems
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskFetchItemsCallback,
    onError: @escaping NSErrorCallback
)
{
    if #available(macOS 10.15.5, iOS 13.5, tvOS 13.5, *) {
        let player = Unmanaged<GKLocalPlayer>.fromOpaque(pointer).takeUnretainedValue();
        player.fetchItems(forIdentityVerificationSignature: { publicKeyUrl, signature, salt, timestamp, error in
            if(error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
            
            let publicKeyUrlData = publicKeyUrl!.absoluteString.data(using: .utf8)
            
            onSuccess(
                taskId,
                UInt(timestamp),
                publicKeyUrlData!.toUCharP(),
                Int32(publicKeyUrlData!.count),
                signature!.toUCharP(),
                Int32(signature!.count),
                salt!.toUCharP(),
                Int32(salt!.count)
            )
            
        })
    } else {
        let error = NSError.init(domain: "GKLocalPlayer", code: -7, userInfo: nil);
        onError(taskId, Unmanaged.passRetained(error as NSError).toOpaque());
    };
}
