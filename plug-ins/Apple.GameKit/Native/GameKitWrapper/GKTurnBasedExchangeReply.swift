//
//  GKTurnBasedExchangeReply.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKTurnBasedExchangeReply_Free")
public func GKTurnBasedExchangeReply_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKTurnBasedExchangeReply>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKTurnBasedExchangeReply_GetData")
public func GKTurnBasedExchangeReply_GetData
(
    pointer: UnsafeMutableRawPointer
) -> InteropStructArray
{
    let target = Unmanaged<GKTurnBasedExchangeReply>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.data != nil) {
        return InteropStructArray(pointer: target.data!.toUCharP(), length: Int32(target.data!.count));
    }
    
    return InteropStructArray();
}

@_cdecl("GKTurnBasedExchangeReply_GetMessage")
public func GKTurnBasedExchangeReply_GetMessage
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let target = Unmanaged<GKTurnBasedExchangeReply>.fromOpaque(pointer).takeUnretainedValue();
    return target.message?.toCharPCopy();
}

@_cdecl("GKTurnBasedExchangeReply_GetRecipient")
public func GKTurnBasedExchangeReply_GetRecipient
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let target = Unmanaged<GKTurnBasedExchangeReply>.fromOpaque(pointer).takeUnretainedValue();
    return Unmanaged.passRetained(target.recipient).toOpaque();
}

@_cdecl("GKTurnBasedExchangeReply_GetReplyDate")
public func GKTurnBasedExchangeReply_GetReplyDate
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKTurnBasedExchangeReply>.fromOpaque(pointer).takeUnretainedValue();
    return Int(target.replyDate.timeIntervalSince1970);
}
