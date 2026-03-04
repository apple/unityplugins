//
//  GKSavedGame.swift
//  GameKitWrapper
//
//  Copyright © 2024 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit


#if !os(tvOS)
@_cdecl("GKSavedGame_GetName")
public func GKSavedGame_GetName
(
    gkSavedGamePtr: UnsafeMutablePointer<GKSavedGame>
) -> char_p?
{
    let savedGame = gkSavedGamePtr.takeUnretainedValue();
    return savedGame.name?.toCharPCopy();
}
#endif

#if !os(tvOS)
@_cdecl("GKSavedGame_GetModificationDate")
public func GKSavedGame_GetModificationDate
(
    gkSavedGamePtr: UnsafeMutablePointer<GKSavedGame>
) -> TimeInterval // aka Double
{
    let savedGame = gkSavedGamePtr.takeUnretainedValue();
    return savedGame.modificationDate?.timeIntervalSince1970 ?? 0.0;
}
#endif

#if !os(tvOS)
@_cdecl("GKSavedGame_GetDeviceName")
public func GKSaveGame_GetDeviceName
(
    gkSavedGamePtr: UnsafeMutablePointer<GKSavedGame>
) -> char_p?
{
    let savedGame = gkSavedGamePtr.takeUnretainedValue();
    return savedGame.deviceName?.toCharPCopy();
}
#endif

#if !os(tvOS)
@_cdecl("GKSavedGame_LoadData")
public func GKSavedGame_LoadData
(
    gkSavedGamePtr: UnsafeMutablePointer<GKSavedGame>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<NSData>,
    onError: @escaping NSErrorTaskCallback
)
{
    let savedGame = gkSavedGamePtr.takeUnretainedValue();
    savedGame.loadData(completionHandler: { data, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, (data as? NSData)?.passRetainedUnsafeMutablePointer());
    });
}
#endif
