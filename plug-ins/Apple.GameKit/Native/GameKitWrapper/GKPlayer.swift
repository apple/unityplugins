//
//  GKPlayer.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKPlayer_GetGamePlayerId")
public func GKPlayer_GetGamePlayerId
(
    pointer : UnsafeMutableRawPointer
) -> char_p
{
    let player = Unmanaged<GKPlayer>.fromOpaque(pointer).takeUnretainedValue();
    return player.gamePlayerID.toCharPCopy();
}

@_cdecl("GKPlayer_GetTeamPlayerId")
public func GKPlayer_GetTeamPlayerId
(
    pointer : UnsafeMutableRawPointer
) -> char_p
{
    let player = Unmanaged<GKPlayer>.fromOpaque(pointer).takeUnretainedValue();
    return player.teamPlayerID.toCharPCopy();
}

@_cdecl("GKPlayer_GetScopedIDsArePersistent")
public func GKPlayer_GetScopedIDsArePersistent
(
    pointer : UnsafeMutableRawPointer
) -> Bool
{
    let player = Unmanaged<GKPlayer>.fromOpaque(pointer).takeUnretainedValue();
    return player.scopedIDsArePersistent();
}

@_cdecl("GKPlayer_GetAlias")
public func GKPlayer_GetAlias
(
    pointer : UnsafeMutableRawPointer
) -> char_p
{
    let player = Unmanaged<GKPlayer>.fromOpaque(pointer).takeUnretainedValue();
    return player.alias.toCharPCopy();
}

@_cdecl("GKPlayer_GetDisplayName")
public func GKPlayer_GetDisplayName
(
    pointer : UnsafeMutableRawPointer
) -> char_p
{
    let player = Unmanaged<GKPlayer>.fromOpaque(pointer).takeUnretainedValue();
    return player.displayName.toCharPCopy();
}

@_cdecl("GKPlayer_GetIsInvitable")
public func GKPlayer_GetIsInvitable
(
    pointer : UnsafeMutableRawPointer
) -> Bool
{
    let player = Unmanaged<GKPlayer>.fromOpaque(pointer).takeUnretainedValue();
    
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        return player.isInvitable
    } else {
        return false;
    };
}

@_cdecl("GKPlayer_GetScopedIdsArePersistent")
public func GKPlayer_GetScopedIdsArePersistent
(
    pointer : UnsafeMutableRawPointer
) -> Bool
{
    let player = Unmanaged<GKPlayer>.fromOpaque(pointer).takeUnretainedValue();
    return player.scopedIDsArePersistent();
}

@_cdecl("GKPlayer_GetGuestIdentifier")
public func GKPlayer_GetGuestIdentifier
(
    pointer : UnsafeMutableRawPointer
) -> char_p?
{
    let player = Unmanaged<GKPlayer>.fromOpaque(pointer).takeUnretainedValue();
    return player.guestIdentifier?.toCharPCopy();
}

@_cdecl("GKPlayer_AnonymousGuestPlayer")
public func GKPlayer_AnonymousGuestPlayer
(
    identifier: char_p
) -> UnsafeMutableRawPointer
{
    let player = GKPlayer.anonymousGuestPlayer(withIdentifier: identifier.toString());
    return Unmanaged.passRetained(player).toOpaque();
}


@_cdecl("GKPlayer_LoadPhoto")
public func GKPlayer_LoadPhoto
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    photoSize: Int,
    onImageLoaded: @escaping SuccessTaskImageCallback,
    onError: @escaping NSErrorCallback
)
{
    let player = Unmanaged<GKPlayer>.fromOpaque(pointer).takeUnretainedValue();

    player.loadPhoto(for: GKPlayer.PhotoSize.init(rawValue: photoSize)!, withCompletionHandler: { (image, error) in
            if(error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
        
            let data = image!.pngData()!;
            onImageLoaded(
                taskId,
                Int32(image!.size.width),
                Int32(image!.size.height),
                data.toUCharP(),
                Int32(data.count));
    });
}
