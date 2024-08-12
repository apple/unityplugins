//
//  GKLocalPlayer.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKSavedGame_GetName")
public func GKSavedGame_GetName
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let savedGame = Unmanaged<GKSavedGame>.fromOpaque(pointer).takeUnretainedValue();
    return savedGame.name!.toCharPCopy();
}

@_cdecl("GKSavedGame_GetModificationDate")
public func GKSavedGame_GetModificationDate
(
    pointer: UnsafeMutableRawPointer
) -> Double
{
    let savedGame = Unmanaged<GKSavedGame>.fromOpaque(pointer).takeUnretainedValue();
    return savedGame.modificationDate!.timeIntervalSince1970;
}

@_cdecl("GKSavedGame_GetDeviceName")
public func GKSavedGame_GetDeviceName
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let savedGame = Unmanaged<GKSavedGame>.fromOpaque(pointer).takeUnretainedValue();
    return savedGame.deviceName!.toCharPCopy();
}

@_cdecl("GKSavedGame_LoadData")
public func GKSavedGame_LoadData
(
    gkSavedGame: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskInteropStructArrayCallback,
    onError: @escaping NSErrorCallback
)
{
    if #available(macOS 10.10, iOS 8.0, *) {
        let savedGame = Unmanaged<GKSavedGame>.fromOpaque(gkSavedGame).takeUnretainedValue();
        savedGame.loadData(completionHandler: { data, error in
            if (error != nil) {
                onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
                return;
            }
            
            if(data != nil) {
                onSuccess(taskId, InteropStructArray(pointer: data!.toUCharP(), length: Int32(data!.count)));
            } else {
                onSuccess(taskId, InteropStructArray());
            }
        })
    } else {
        let error = NSError.init(domain: "GKLocalPlayer", code: GKErrorCodeExtension.unsupportedOperationForOSVersion.rawValue, userInfo: nil);
        onError(taskId, Unmanaged.passRetained(error as NSError).toOpaque());
    };
}
