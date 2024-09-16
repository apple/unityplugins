//
//  GKTurnBasedExchange.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKTurnBasedExchange_Cancel")
public func GKTurnBasedExchange_Cancel
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchange>,
    taskId: Int64,
    localizableMessageKey: char_p,
    argumentsPtr: UnsafeMutablePointer<NSArray>, // NSArray<NSString>
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    let arguments = argumentsPtr.takeUnretainedValue() as! [String];
    
    target.cancel(withLocalizableMessageKey: localizableMessageKey.toString(), arguments: arguments, completionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }
        
        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedExchange_GetCompletionDate")
public func GKTurnBasedExchange_GetCompletionDate
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchange>
) -> TimeInterval // aka Double
{
    let target = pointer.takeUnretainedValue();
    return target.completionDate?.timeIntervalSince1970 ?? 0.0;
}

@_cdecl("GKTurnBasedExchange_GetSendDate")
public func GKTurnBasedExchange_GetSendDate
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchange>
) -> TimeInterval // aka Double
{
    let target = pointer.takeUnretainedValue();
    return target.sendDate.timeIntervalSince1970;
}

@_cdecl("GKTurnBasedExchange_GetTimeoutDate")
public func GKTurnBasedExchange_GetTimeoutDate
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchange>
) -> TimeInterval // aka Double
{
    let target = pointer.takeUnretainedValue();
    return target.timeoutDate?.timeIntervalSince1970 ?? 0.0;
}

@_cdecl("GKTurnBasedExchange_GetSender")
public func GKTurnBasedExchange_GetSender
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchange>
) -> UnsafeMutablePointer<GKTurnBasedParticipant>
{
    let target = pointer.takeUnretainedValue();
    return target.sender.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKTurnBasedExchange_GetRecipients")
public func GKTurnBasedExchange_GetRecipients
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchange>
) -> UnsafeMutablePointer<NSArray> // NSArray<GKTurnBasedParticipant>
{
    let target = pointer.takeUnretainedValue();
    return (target.recipients as NSArray).passRetainedUnsafeMutablePointer();
}

@_cdecl("GKTurnBasedExchange_GetData")
public func GKTurnBasedExchange_GetData
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchange>
) -> UnsafeMutablePointer<NSData>?
{
    let target = pointer.takeUnretainedValue();
    return (target.data as? NSData)?.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKTurnBasedExchange_GetExchangeID")
public func GKTurnBasedExchange_GetExchangeID
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchange>
) -> char_p
{
    let target = pointer.takeUnretainedValue();
    return target.exchangeID.toCharPCopy();
}

@_cdecl("GKTurnBasedExchange_GetMessage")
public func GKTurnBasedExchange_GetMessage
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchange>
) -> char_p?
{
    let target = pointer.takeUnretainedValue();
    return target.message?.toCharPCopy();
}

@_cdecl("GKTurnBasedExchange_GetReplies")
public func GKTurnBasedExchange_GetReplies
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchange>
) -> UnsafeMutablePointer<NSArray>? // NSArray<GKTurnBasedExchangeReply>
{
    let target = pointer.takeUnretainedValue();
    return (target.replies as? NSArray)?.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKTurnBasedExchange_GetStatus")
public func GKTurnBasedExchange_GetStatus
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchange>
) -> Int8
{
    let target = pointer.takeUnretainedValue();
    return target.status.rawValue;
}

@_cdecl("GKTurnBasedExchange_Reply")
public func GKTurnBasedExchange_Reply
(
    pointer: UnsafeMutablePointer<GKTurnBasedExchange>,
    taskId: Int64,
    localizableMessageKey: char_p,
    argumentsPtr: UnsafeMutablePointer<NSArray>, // NSArray<NSString>
    dataPtr: UnsafeMutablePointer<NSData>,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    let arguments = argumentsPtr.takeUnretainedValue() as! [String];
    
    target.reply(withLocalizableMessageKey: localizableMessageKey.toString(), arguments: arguments, data: dataPtr.takeUnretainedValue() as Data, completionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId);
    });
}
