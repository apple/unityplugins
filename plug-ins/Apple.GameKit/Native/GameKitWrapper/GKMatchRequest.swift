//
//  GKMatchRequest.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKMatchRequest_Init")
public func GKMatchRequest_Init() -> UnsafeMutablePointer<GKMatchRequest>
{
    let request = GKMatchRequest();
    return request.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKMatchRequest_SetMaxPlayers")
public func GKMatchRequest_SetMaxPlayers
(
    pointer: UnsafeMutablePointer<GKMatchRequest>,
    value: Int
)
{
    let target = pointer.takeUnretainedValue();
    target.maxPlayers = value;
}

@_cdecl("GKMatchRequest_GetMaxPlayers")
public func GKMatchRequest_GetMaxPlayers
(
    pointer: UnsafeMutablePointer<GKMatchRequest>
) -> Int
{
    let target = pointer.takeUnretainedValue();
    return target.maxPlayers;
}

@_cdecl("GKMatchRequest_SetMinPlayers")
public func GKMatchRequest_SetMinPlayers
(
    pointer: UnsafeMutablePointer<GKMatchRequest>,
    value: Int
)
{
    let target = pointer.takeUnretainedValue();
    target.minPlayers = value;
}

@_cdecl("GKMatchRequest_GetMinPlayers")
public func GKMatchRequest_GetMinPlayers
(
    pointer: UnsafeMutablePointer<GKMatchRequest>
) -> Int
{
    let target = pointer.takeUnretainedValue();
    return target.minPlayers;
}

@_cdecl("GKMatchRequest_GetPlayerGroup")
public func GKMatchRequest_GetPlayerGroup
(
    pointer: UnsafeMutablePointer<GKMatchRequest>
) -> Int
{
    let target = pointer.takeUnretainedValue();
    return target.playerGroup;
}

@_cdecl("GKMatchRequest_GetPlayerAttributes")
public func GKMatchRequest_GetPlayerAttributes
(
    pointer: UnsafeMutablePointer<GKMatchRequest>
) -> UInt32
{
    let target = pointer.takeUnretainedValue();
    return target.playerAttributes;
}

@_cdecl("GKMatchRequest_SetPlayerGroup")
public func GKMatchRequest_SetPlayerGroup
(
    pointer: UnsafeMutablePointer<GKMatchRequest>,
    value: Int
)
{
    let target = pointer.takeUnretainedValue();
    target.playerGroup = value;
}

@_cdecl("GKMatchRequest_SetPlayerAttributes")
public func GKMatchRequest_SetPlayerAttributes
(
    pointer: UnsafeMutablePointer<GKMatchRequest>,
    value: UInt32
)
{
    let target = pointer.takeUnretainedValue();
    target.playerAttributes = value;
}

@_cdecl("GKMatchRequest_GetInviteMessage")
public func GKMatchRequest_GetInviteMessage
(
    pointer: UnsafeMutablePointer<GKMatchRequest>
) -> char_p?
{
    let target = pointer.takeUnretainedValue();
    return target.inviteMessage?.toCharPCopy();
}

@_cdecl("GKMatchRequest_SetInviteMessage")
public func GKMatchRequest_SetInviteMessage
(
    pointer: UnsafeMutablePointer<GKMatchRequest>,
    value: char_p?
)
{
    let target = pointer.takeUnretainedValue();
    target.inviteMessage = value?.toString();
}

@_cdecl("GKMatchRequest_GetRecipients")
public func GKMatchRequest_GetRecipients
(
    pointer: UnsafeMutablePointer<GKMatchRequest>
) -> UnsafeMutablePointer<NSArray>? // NSArray<GKPlayer>
{
    let target = pointer.takeUnretainedValue();
    return (target.recipients as? NSArray)?.passRetainedUnsafeMutablePointer();
}
    
@_cdecl("GKMatchRequest_SetRecipients")
public func GKMatchRequest_SetRecipients
(
    pointer: UnsafeMutablePointer<GKMatchRequest>,
    value: UnsafeMutablePointer<NSArray>? // NSArray<GKPlayer>
)
{
    let target = pointer.takeUnretainedValue();
    target.recipients = value?.takeUnretainedValue() as? [GKPlayer];
}

@_cdecl("GKMatchRequest_GetQueueName")
public func GKMatchRequest_GetQueueName
(
    gkMatchRequestPtr: UnsafeMutablePointer<GKMatchRequest>
) -> char_p?
{
    if #available(iOS 17.2, tvOS 17.2, macOS 14.2, visionOS 1.1, *) {
        let gkMatchRequest = gkMatchRequestPtr.takeUnretainedValue();
        return gkMatchRequest.queueName?.toCharPCopy();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKMatchRequest_SetQueueName")
public func GKMatchRequest_SetQueueName
(
    gkMatchRequestPtr: UnsafeMutablePointer<GKMatchRequest>,
    value: char_p?
)
{
    if #available(iOS 17.2, tvOS 17.2, macOS 14.2, visionOS 1.1, *) {
        let gkMatchRequest = gkMatchRequestPtr.takeUnretainedValue();
        gkMatchRequest.queueName = value?.toString();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKMatchRequest_GetProperties")
public func GKMatchRequest_GetProperties
(
    gkMatchRequestPtr: UnsafeMutablePointer<GKMatchRequest>
) -> UnsafeMutablePointer<NSDictionary>? // NSDictionary<NSString, Any>
{
    if #available(iOS 17.2, tvOS 17.2, macOS 14.2, visionOS 1.1, *) {
        let gkMatchRequest = gkMatchRequestPtr.takeUnretainedValue();
        return (gkMatchRequest.properties as? NSDictionary)?.passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKMatchRequest_SetProperties")
public func GKMatchRequest_SetProperties
(
    gkMatchRequestPtr: UnsafeMutablePointer<GKMatchRequest>,
    nsDictionaryPtr: UnsafeMutablePointer<NSDictionary>? // NSDictionary<NSString, Any>
)
{
    if #available(iOS 17.2, tvOS 17.2, macOS 14.2, visionOS 1.1, *) {
        let gkMatchRequest = gkMatchRequestPtr.takeUnretainedValue();
        gkMatchRequest.properties = nsDictionaryPtr?.takeUnretainedValue() as? [String : Any];
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKMatchRequest_GetRecipientProperties")
public func GKMatchRequest_GetRecipientProperties
(
    gkMatchRequestPtr: UnsafeMutablePointer<GKMatchRequest>
) -> UnsafeMutablePointer<NSDictionary>? // NSDictionary<GKPlayer, NSDictionary<NSString, Any>>
{
    if #available(iOS 17.2, tvOS 17.2, macOS 14.2, visionOS 1.1, *) {
        let gkMatchRequest = gkMatchRequestPtr.takeUnretainedValue();
        return (gkMatchRequest.recipientProperties as? NSDictionary)?.passRetainedUnsafeMutablePointer();
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKMatchRequest_SetRecipientProperties")
public func GKMatchRequest_SetRecipientProperties
(
    gkMatchRequestPtr: UnsafeMutablePointer<GKMatchRequest>,
    nsDictionaryPtr: UnsafeMutablePointer<NSDictionary>? // NSDictionary<GKPlayer, NSDictionary<NSString, Any>>
)
{
    if #available(iOS 17.2, tvOS 17.2, macOS 14.2, visionOS 1.1, *) {
        let gkMatchRequest = gkMatchRequestPtr.takeUnretainedValue();
        gkMatchRequest.recipientProperties = nsDictionaryPtr?.takeUnretainedValue() as? [GKPlayer : [String : Any]]
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

public typealias GKMatchRequestRecipientResponseHandler = @convention(c) (
    UnsafeMutablePointer<GKMatchRequest>,
    UnsafeMutablePointer<GKPlayer>,
    Int /*GKInviteRecipientResponse*/) -> Void;

@_cdecl("GKMatchRequest_SetRecipientResponseHandler")
public func GKMatchRequest_SetRecipientResponseHandler
(
    gkMatchRequestPtr: UnsafeMutablePointer<GKMatchRequest>,
    recipientResponseHandler: GKMatchRequestRecipientResponseHandler? // optional func params are @escaping by default
)
{
    let gkMatchRequest = gkMatchRequestPtr.takeUnretainedValue();

    guard let recipientResponseHandler else {
        gkMatchRequest.recipientResponseHandler = nil;
        return;
    }

    gkMatchRequest.recipientResponseHandler = { gkPlayer, gkInviteRecipientResponse in
        recipientResponseHandler(
            gkMatchRequestPtr, // not retained as per notes in InteropWeakMap.cs.
            gkPlayer.passRetainedUnsafeMutablePointer(),
            gkInviteRecipientResponse.rawValue);
    }
}

@_cdecl("GKMatchRequest_GetDefaultNumberOfPlayers")
public func GKMatchRequest_GetDefaultNumberOfPlayers
(
    gkMatchRequestPtr: UnsafeMutablePointer<GKMatchRequest>
) -> Int
{
    let gkMatchRequest = gkMatchRequestPtr.takeUnretainedValue();
    return gkMatchRequest.defaultNumberOfPlayers;
}

@_cdecl("GKMatchRequest_SetDefaultNumberOfPlayers")
public func GKMatchRequest_SetDefaultNumberOfPlayers
(
    gkMatchRequestPtr: UnsafeMutablePointer<GKMatchRequest>,
    value: Int
)
{
    let gkMatchRequest = gkMatchRequestPtr.takeUnretainedValue();
    gkMatchRequest.defaultNumberOfPlayers = value;
}

@_cdecl("GKMatchRequest_GetMaxPlayersAllowedForMatchOfType")
public func GKMatchRequest_GetMaxPlayersAllowedForMatchOfType
(
    matchType: UInt // GKMatchType
) -> Int
{
    return GKMatchRequest.maxPlayersAllowedForMatch(of: GKMatchType(rawValue: matchType)!);
}
