//
//  GKMatch.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKMatch_Free")
public func GKMatch_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _  = Unmanaged<GKMatch>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKMatch_GetExpectedPlayerCount")
public func GKMatch_GetExpectedPlayerCount
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKMatch>.fromOpaque(pointer).takeUnretainedValue();
    return target.expectedPlayerCount;
}

@_cdecl("GKMatch_GetPlayers")
public func GKMatch_GetPlayers
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKMatch>.fromOpaque(pointer).takeUnretainedValue();
    return Unmanaged.passRetained(target.players as NSArray).toOpaque();
}

@_cdecl("GKMatch_SendToAll")
public func GKMatch_SendToAll
(
    pointer: UnsafeMutableRawPointer,
    data: InteropStructArray,
    sendMode: Int
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKMatch>.fromOpaque(pointer).takeUnretainedValue();
    
    do {
        try target.sendData(toAllPlayers: data.toData(), with: GKMatch.SendDataMode.init(rawValue: sendMode)!);
        return nil;
    } catch {
        return Unmanaged.passRetained(error as NSError).toOpaque();
    }
}

@_cdecl("GKMatch_SendTo")
public func GKMatch_SendTo
(
    pointer: UnsafeMutableRawPointer,
    data: InteropStructArray,
    players: UnsafeMutableRawPointer,
    sendMode: Int
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKMatch>.fromOpaque(pointer).takeUnretainedValue();
    let players = Unmanaged<NSArray>.fromOpaque(players).takeUnretainedValue() as! [GKPlayer];
    
    do {
        try target.send(data.toData(), to: players, dataMode: GKMatch.SendDataMode.init(rawValue: sendMode)!);
        return nil;
    } catch {
        return Unmanaged.passRetained(error as NSError).toOpaque();
    }
}

@_cdecl("GKMatch_Disconnect")
public func GKMatch_Disconnect
(
    pointer: UnsafeMutableRawPointer
)
{
    let target = Unmanaged<GKMatch>.fromOpaque(pointer).takeUnretainedValue();
    target.disconnect();
    
    _currentGKMatchDelegate = nil;
}

@_cdecl("GKMatch_ChooseBestHostingPlayer")
public func GKMatch_ChooseBestHostingPlayer
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKMatch>.fromOpaque(pointer).takeUnretainedValue();
    target.chooseBestHostingPlayer(completionHandler: { player in
        if(player != nil) {
            onSuccess(taskId, Unmanaged.passRetained(player!).toOpaque());
        } else {
            onSuccess(taskId, nil);
        }
    });
}

@_cdecl("GKMatch_VoiceChat")
public func GKMatch_VoiceChat
(
    pointer: UnsafeMutableRawPointer,
    channel:  char_p
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKMatch>.fromOpaque(pointer).takeUnretainedValue();
    let chat = target.voiceChat(withName: channel.toString());
    
    if(chat != nil) {
        return Unmanaged.passRetained(chat!).toOpaque();
    } else {
        return nil;
    }
}

@_cdecl("GKMatch_Rematch")
public func GKMatch_Rematch
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKMatch>.fromOpaque(pointer).takeUnretainedValue();
    target.rematch(completionHandler: {match, error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        if(match != nil) {
            onSuccess(taskId, Unmanaged.passRetained(match!).toOpaque());
        } else {
            onSuccess(taskId, nil);
        }
    });
}

public var _currentGKMatchDelegate : GKWMatchDelegate? = nil;

@_cdecl("GKMatch_GetDelegate")
public func GKMatch_GetDelegate
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKMatch>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.delegate == nil) {
        _currentGKMatchDelegate = GKWMatchDelegate();
        _currentGKMatchDelegate!.Match = target;
        
        target.delegate = _currentGKMatchDelegate;
    }
    
    return Unmanaged<GKWMatchDelegate>.passRetained(target.delegate! as! GKWMatchDelegate).toOpaque();
}
