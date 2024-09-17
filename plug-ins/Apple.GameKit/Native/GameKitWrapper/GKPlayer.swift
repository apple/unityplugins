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
    pointer : UnsafeMutablePointer<GKPlayer>
) -> char_p
{
    let player = pointer.takeUnretainedValue();
    return player.gamePlayerID.toCharPCopy();
}

@_cdecl("GKPlayer_GetTeamPlayerId")
public func GKPlayer_GetTeamPlayerId
(
    pointer : UnsafeMutablePointer<GKPlayer>
) -> char_p
{
    let player = pointer.takeUnretainedValue();
    return player.teamPlayerID.toCharPCopy();
}

@_cdecl("GKPlayer_GetScopedIDsArePersistent")
public func GKPlayer_GetScopedIDsArePersistent
(
    pointer : UnsafeMutablePointer<GKPlayer>
) -> Bool
{
    let player = pointer.takeUnretainedValue();
    return player.scopedIDsArePersistent();
}

@_cdecl("GKPlayer_GetAlias")
public func GKPlayer_GetAlias
(
    pointer : UnsafeMutablePointer<GKPlayer>
) -> char_p
{
    let player = pointer.takeUnretainedValue();
    return player.alias.toCharPCopy();
}

@_cdecl("GKPlayer_GetDisplayName")
public func GKPlayer_GetDisplayName
(
    pointer : UnsafeMutablePointer<GKPlayer>
) -> char_p
{
    let player = pointer.takeUnretainedValue();
    return player.displayName.toCharPCopy();
}

@_cdecl("GKPlayer_GetIsInvitable")
public func GKPlayer_GetIsInvitable
(
    pointer : UnsafeMutablePointer<GKPlayer>
) -> Bool
{
    if #available(iOS 14, tvOS 14, macOS 11.0, *) {
        let player = pointer.takeUnretainedValue();
        return player.isInvitable
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKPlayer_GetScopedIdsArePersistent")
public func GKPlayer_GetScopedIdsArePersistent
(
    pointer : UnsafeMutablePointer<GKPlayer>
) -> Bool
{
    let player = pointer.takeUnretainedValue();
    return player.scopedIDsArePersistent();
}

@_cdecl("GKPlayer_GetGuestIdentifier")
public func GKPlayer_GetGuestIdentifier
(
    pointer : UnsafeMutablePointer<GKPlayer>
) -> char_p?
{
    let player = pointer.takeUnretainedValue();
    return player.guestIdentifier?.toCharPCopy();
}

@_cdecl("GKPlayer_AnonymousGuestPlayer")
public func GKPlayer_AnonymousGuestPlayer
(
    identifier: char_p
) -> UnsafeMutablePointer<GKPlayer>
{
    let player = GKPlayer.anonymousGuestPlayer(withIdentifier: identifier.toString());
    return player.passRetainedUnsafeMutablePointer();
}


@_cdecl("GKPlayer_LoadPhoto")
public func GKPlayer_LoadPhoto
(
    pointer: UnsafeMutablePointer<GKPlayer>,
    taskId: Int64,
    photoSize: Int,
    onImageLoaded: @escaping SuccessTaskImageCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let player = pointer.takeUnretainedValue();

    player.loadPhoto(for: GKPlayer.PhotoSize.init(rawValue: photoSize)!, withCompletionHandler: { (image, error) in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        // GKPlayer loadPhoto is supposed to return GKErrorPlayerPhotoFailure if the image can't be loaded.
        let data = image?.pngData() as? NSData;
        onImageLoaded(taskId, data?.passRetainedUnsafeMutablePointer());
    });
}
