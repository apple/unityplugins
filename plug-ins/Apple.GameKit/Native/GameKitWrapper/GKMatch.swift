//
//  GKMatch.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKMatch_GetExpectedPlayerCount")
public func GKMatch_GetExpectedPlayerCount
(
    pointer: UnsafeMutablePointer<GKMatch>
) -> Int
{
    let target = pointer.takeUnretainedValue();
    return target.expectedPlayerCount;
}

@_cdecl("GKMatch_GetPlayers")
public func GKMatch_GetPlayers
(
    pointer: UnsafeMutablePointer<GKMatch>
) -> UnsafeMutablePointer<NSArray> // NSArray<GKPlayer *> *
{
    let target = pointer.takeUnretainedValue();
    return (target.players as NSArray).passRetainedUnsafeMutablePointer()
}

@_cdecl("GKMatch_GetProperties")
public func GKMatch_GetProperties
(
    gkMatchPtr: UnsafeMutablePointer<GKMatch>
) -> UnsafeMutablePointer<NSDictionary>? // NSDictionary<String, Any>?
{
    // GKMatchProperties is not exposed to Swift from Objective-C.
    // In Swift, it's merely a dictionary of strings to objects.
    if #available(iOS 17.2, tvOS 17.2, macOS 14.2, visionOS 1.1, *) {
        let gkMatch = gkMatchPtr.takeUnretainedValue();
        return (gkMatch.properties as? NSDictionary)?.passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKMatch_GetPlayerProperties")
public func GKMatch_GetPlayerProperties
(
    gkMatchPtr: UnsafeMutablePointer<GKMatch>
) -> UnsafeMutablePointer<NSDictionary>? // NSDictionary<GKPlayer, NSDictionary<String, Any>>?
{
    if #available(iOS 17.2, tvOS 17.2, macOS 14.2, visionOS 1.1, *) {
        let gkMatch = gkMatchPtr.takeUnretainedValue();
        return (gkMatch.playerProperties as? NSDictionary)?.passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKMatch_SendToAll")
public func GKMatch_SendToAll
(
    pointer: UnsafeMutablePointer<GKMatch>,
    dataPtr: UnsafeMutablePointer<NSData>,
    sendMode: Int
) -> UnsafeMutablePointer<NSError>?
{
    let target = pointer.takeUnretainedValue();
    do {
        try target.sendData(toAllPlayers: dataPtr.takeUnretainedValue() as Data, with: GKMatch.SendDataMode(rawValue: sendMode)!);
        return nil;
    } catch {
        return (error as NSError).passRetainedUnsafeMutablePointer();
    }
}

@_cdecl("GKMatch_SendTo")
public func GKMatch_SendTo
(
    pointer: UnsafeMutablePointer<GKMatch>,
    dataPtr: UnsafeMutablePointer<NSData>,
    players: UnsafeMutablePointer<NSArray>, // NSArray<GKPlayer>
    sendMode: Int
) -> UnsafeMutablePointer<NSError>?
{
    let target = pointer.takeUnretainedValue();
    let players = players.takeUnretainedValue() as! [GKPlayer];
    
    do {
        try target.send(dataPtr.takeUnretainedValue() as Data, to: players, dataMode: GKMatch.SendDataMode(rawValue: sendMode)!);
        return nil;
    } catch {
        return (error as NSError).passRetainedUnsafeMutablePointer();
    }
}

@_cdecl("GKMatch_Disconnect")
public func GKMatch_Disconnect
(
    pointer: UnsafeMutablePointer<GKMatch>
)
{
    let target = pointer.takeUnretainedValue();
    target.disconnect();

    _currentGKMatchDelegate = nil;
}

@_cdecl("GKMatch_ChooseBestHostingPlayer")
public func GKMatch_ChooseBestHostingPlayer
(
    pointer: UnsafeMutablePointer<GKMatch>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.chooseBestHostingPlayer(completionHandler: { player in
        onSuccess(taskId, player?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKMatch_VoiceChat")
public func GKMatch_VoiceChat
(
    pointer: UnsafeMutablePointer<GKMatch>,
    channel:  char_p
) -> UnsafeMutablePointer<GKVoiceChat>?
{
    let target = pointer.takeUnretainedValue();
    let chat = target.voiceChat(withName: channel.toString());
    return chat?.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKMatch_Rematch")
public func GKMatch_Rematch
(
    pointer: UnsafeMutablePointer<GKMatch>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.rematch(completionHandler: {match, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, match?.passRetainedUnsafeMutablePointer());
    });
}

public var _currentGKMatchDelegate : GKWMatchDelegate? = nil;

@_cdecl("GKMatch_GetDelegate")
public func GKMatch_GetDelegate
(
    pointer: UnsafeMutablePointer<GKMatch>
) -> UnsafeMutablePointer<GKWMatchDelegate>?
{
    let target = pointer.takeUnretainedValue();

    if (target.delegate == nil) {
        _currentGKMatchDelegate = GKWMatchDelegate();
        _currentGKMatchDelegate!.Match = target;

        target.delegate = _currentGKMatchDelegate;
    }

    return (target.delegate! as! GKWMatchDelegate).passRetainedUnsafeMutablePointer();
}
