//
//  GKTurnBasedExchange.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKTurnBasedExchange_Free")
public func GKTurnBasedExchange_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKTurnBasedExchange>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKTurnBasedExchange_Cancel")
public func GKTurnBasedExchange_Cancel
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    localizableMessageKey: char_p,
    argumentsPtr: UnsafeMutableRawPointer,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedExchange>.fromOpaque(pointer).takeUnretainedValue();
    let arguments = Unmanaged<NSArray>.fromOpaque(argumentsPtr).takeUnretainedValue() as! [String];
    
    target.cancel(withLocalizableMessageKey: localizableMessageKey.toString(), arguments: arguments, completionHandler: { error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedExchange_GetCompletionDate")
public func GKTurnBasedExchange_GetCompletionDate
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKTurnBasedExchange>.fromOpaque(pointer).takeUnretainedValue();
    return Int(target.completionDate?.timeIntervalSince1970 ?? 0);
}

@_cdecl("GKTurnBasedExchange_GetSendDate")
public func GKTurnBasedExchange_GetSendDate
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKTurnBasedExchange>.fromOpaque(pointer).takeUnretainedValue();
    return Int(target.sendDate.timeIntervalSince1970);
}

@_cdecl("GKTurnBasedExchange_GetTimeoutDate")
public func GKTurnBasedExchange_GetTimeoutDate
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKTurnBasedExchange>.fromOpaque(pointer).takeUnretainedValue();
    return Int(target.timeoutDate?.timeIntervalSince1970 ?? 0);
}

@_cdecl("GKTurnBasedExchange_GetSender")
public func GKTurnBasedExchange_GetSender
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let target = Unmanaged<GKTurnBasedExchange>.fromOpaque(pointer).takeUnretainedValue();
    return Unmanaged.passRetained(target.sender).toOpaque();
}

@_cdecl("GKTurnBasedExchange_GetRecipients")
public func GKTurnBasedExchange_GetRecipients
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let target = Unmanaged<GKTurnBasedExchange>.fromOpaque(pointer).takeUnretainedValue();
    return Unmanaged.passRetained(target.recipients as NSArray).toOpaque();
}

@_cdecl("GKTurnBasedExchange_GetData")
public func GKTurnBasedExchange_GetData
(
    pointer: UnsafeMutableRawPointer
) -> InteropStructArray
{
    let target = Unmanaged<GKTurnBasedExchange>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.data != nil) {
        return InteropStructArray(pointer: target.data!.toUCharP(), length: Int32(target.data!.count));
    }
    
    return InteropStructArray();
}

@_cdecl("GKTurnBasedExchange_GetExchangeID")
public func GKTurnBasedExchange_GetExchangeID
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let target = Unmanaged<GKTurnBasedExchange>.fromOpaque(pointer).takeUnretainedValue();
    return target.exchangeID.toCharPCopy();
}

@_cdecl("GKTurnBasedExchange_GetMessage")
public func GKTurnBasedExchange_GetMessage
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let target = Unmanaged<GKTurnBasedExchange>.fromOpaque(pointer).takeUnretainedValue();
    return target.message?.toCharPCopy();
}

@_cdecl("GKTurnBasedExchange_GetReplies")
public func GKTurnBasedExchange_GetReplies
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKTurnBasedExchange>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.replies != nil) {
        return Unmanaged.passRetained(target.replies! as NSArray).toOpaque();
    }
    
    return nil;
}

@_cdecl("GKTurnBasedExchange_GetStatus")
public func GKTurnBasedExchange_GetStatus
(
    pointer: UnsafeMutableRawPointer
) -> Int8
{
    let target = Unmanaged<GKTurnBasedExchange>.fromOpaque(pointer).takeUnretainedValue();
    return target.status.rawValue;
}

@_cdecl("GKTurnBasedExchange_Reply")
public func GKTurnBasedExchange_Reply
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    localizableMessageKey: char_p,
    argumentsPtr: UnsafeMutableRawPointer,
    data: InteropStructArray,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedExchange>.fromOpaque(pointer).takeUnretainedValue();
    let arguments = Unmanaged<NSArray>.fromOpaque(argumentsPtr).takeUnretainedValue() as! [String];
    
    target.reply(withLocalizableMessageKey: localizableMessageKey.toString(), arguments: arguments, data: data.toData(), completionHandler: { error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId);
    });
}
