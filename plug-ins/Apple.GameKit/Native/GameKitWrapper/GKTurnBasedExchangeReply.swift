//
//  GKTurnBasedExchangeReply.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKTurnBasedExchangeReply_GetData")
public func GKTurnBasedExchangeReply_GetData
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchangeReply>
) -> UnsafeMutablePointer<NSData>?
{
    let target = pointer.takeUnretainedValue();
    return (target.data as? NSData)?.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKTurnBasedExchangeReply_GetMessage")
public func GKTurnBasedExchangeReply_GetMessage
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchangeReply>
) -> char_p?
{
    let target = pointer.takeUnretainedValue();
    return target.message?.toCharPCopy();
}

@_cdecl("GKTurnBasedExchangeReply_GetRecipient")
public func GKTurnBasedExchangeReply_GetRecipient
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchangeReply>
) -> UnsafeMutablePointer<GKTurnBasedParticipant>
{
    let target = pointer.takeUnretainedValue();
    return target.recipient.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKTurnBasedExchangeReply_GetReplyDate")
public func GKTurnBasedExchangeReply_GetReplyDate
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchangeReply>
) -> TimeInterval // aka Double
{
    let target = pointer.takeUnretainedValue();
    return target.replyDate.timeIntervalSince1970;
}
