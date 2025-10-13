//
//  GKTurnBasedMatch.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKTurnBasedMatch_GetParticipants")
public func GKTurnBasedMatch_GetParticipants
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>
) -> UnsafeMutablePointer<NSArray> // NSArray<GKTurnBasedParticipant>
{
    let target = pointer.takeUnretainedValue();
    return (target.participants as NSArray).passRetainedUnsafeMutablePointer();
}

@_cdecl("GKTurnBasedMatch_GetCurrentParticipant")
public func GKTurnBasedMatch_GetCurrentParticipant
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>
) -> UnsafeMutablePointer<GKTurnBasedParticipant>?
{
    let target = pointer.takeUnretainedValue();
    return target.currentParticipant?.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKTurnBasedMatch_GetMatchData")
public func GKTurnBasedMatch_GetMatchData
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>
) -> UnsafeMutablePointer<NSData>?
{
    let target = pointer.takeUnretainedValue();
    return (target.matchData as? NSData)?.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKTurnBasedMatch_GetMatchDataMaximumSize")
public func GKTurnBasedMatch_GetMatchDataMaximumSize
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>
) -> Int
{
    let target = pointer.takeUnretainedValue();
    return target.matchDataMaximumSize;
}

@_cdecl("GKTurnBasedMatch_GetMessage")
public func GKTurnBasedMatch_GetMessage
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>
) -> char_p?
{
    let target = pointer.takeUnretainedValue();
    return target.message?.toCharPCopy();
}

@_cdecl("GKTurnBasedMatch_GetMatchID")
public func GKTurnBasedMatch_GetMatchID
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>
) -> char_p
{
    let target = pointer.takeUnretainedValue();
    return target.matchID.toCharPCopy();
}

@_cdecl("GKTurnBasedMatch_GetCreationDate")
public func GKTurnBasedMatch_GetCreationDate
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>
) -> TimeInterval // aka Double
{
    let target = pointer.takeUnretainedValue();
    return target.creationDate.timeIntervalSince1970;
}

@_cdecl("GKTurnBasedMatch_GetStatus")
public func GKTurnBasedMatch_GetStatus
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>
) -> Int
{
    let target = pointer.takeUnretainedValue();
    return target.status.rawValue;
}

@_cdecl("GKTurnBasedMatch_LoadMatchData")
public func GKTurnBasedMatch_LoadMatchData
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<NSData>,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.loadMatchData(completionHandler: { data, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, (data as? NSData)?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKTurnBasedMatch_SaveCurrentTurn")
public func GKTurnBasedMatch_SaveCurrentTurn
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>,
    taskId: Int64,
    dataPtr: UnsafeMutablePointer<NSData>,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.saveCurrentTurn(withMatch: dataPtr.takeUnretainedValue() as Data, completionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_EndTurn")
public func GKTurnBasedMatch_EndTurn
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>,
    taskId: Int64,
    participantsPtr: UnsafeMutablePointer<NSArray>, // NSArray<GKTurnBasedParticipant>
    timeout: Double,
    dataPtr: UnsafeMutablePointer<NSData>,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    let participants = participantsPtr.takeUnretainedValue() as! [GKTurnBasedParticipant];
    
    target.endTurn(withNextParticipants: participants, turnTimeout: TimeInterval(timeout), match: dataPtr.takeUnretainedValue() as Data, completionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_ParticipantQuitInTurn")
public func GKTurnBasedMatch_ParticipantQuitInTurn
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>,
    taskId: Int64,
    outcome: Int,
    participantsPtr: UnsafeMutablePointer<NSArray>, // NSArray<GKTurnBasedParticipant>
    timeout: Double,
    dataPtr: UnsafeMutablePointer<NSData>,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    let participants = participantsPtr.takeUnretainedValue() as! [GKTurnBasedParticipant];
    
    target.participantQuitInTurn(with: GKTurnBasedMatch.Outcome(rawValue: outcome)!, nextParticipants: participants, turnTimeout: TimeInterval(timeout), match: dataPtr.takeUnretainedValue() as Data, completionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_ParticipantQuitOutOfTurn")
public func GKTurnBasedMatch_ParticipantQuitOutOfTurn
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>,
    taskId: Int64,
    outcome: Int,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    
    target.participantQuitOutOfTurn(with: GKTurnBasedMatch.Outcome(rawValue: outcome)!, withCompletionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_EndMatchInTurn")
public func GKTurnBasedMatch_EndMatchInTurn
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>,
    taskId: Int64,
    dataPtr: UnsafeMutablePointer<NSData>,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();

    target.endMatchInTurn(withMatch: dataPtr.takeUnretainedValue() as Data, completionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_Remove")
public func GKTurnBasedMatch_Remove
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.remove(completionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_SaveMergedMatch")
public func GKTurnBasedMatch_SaveMergedMatch
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>,
    taskId: Int64,
    dataPtr: UnsafeMutablePointer<NSData>,
    exchangesPtr: UnsafeMutablePointer<NSArray>, // NSArray<GKTurnBasedExchange>
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    let exchanges = exchangesPtr.takeUnretainedValue() as! [GKTurnBasedExchange];

    target.saveMergedMatch(dataPtr.takeUnretainedValue() as Data, withResolvedExchanges: exchanges, completionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_SendExchange")
public func GKTurnBasedMatch_SendExchange
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>,
    taskId: Int64,
    participantsPtr: UnsafeMutablePointer<NSArray>, // NSArray<GKTurnBasedParticipant>
    dataPtr: UnsafeMutablePointer<NSData>,
    localizableMessageKey: char_p,
    argumentsPtr: UnsafeMutablePointer<NSArray>, // NSArray<NSString>
    timeout: Double,
    onSuccess: @escaping SuccessTaskPtrCallback<GKTurnBasedExchange>,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    let participants = participantsPtr.takeUnretainedValue() as! [GKTurnBasedParticipant];
    let arguments = argumentsPtr.takeUnretainedValue() as! [String];
 
    target.sendExchange(to: participants, data: dataPtr.takeUnretainedValue() as Data, localizableMessageKey: localizableMessageKey.toString(), arguments: arguments, timeout: TimeInterval(timeout), completionHandler: { exchange, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, exchange?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKTurnBasedMatch_GKExchangeTimeoutDefault")
public func GKTurnBasedMatch_GKExchangeTimeoutDefault
(
) -> Double
{
    return GKExchangeTimeoutDefault;
}

@_cdecl("GKTurnBasedMatch_GKExchangeTimeoutNone")
public func GKTurnBasedMatch_GKExchangeTimeoutNone
(
) -> Double
{
    return GKExchangeTimeoutNone;
}

@_cdecl("GKTurnBasedMatch_GetTurnTimeoutDefault")
public func GKTurnBasedMatch_GetTurnTimeoutDefault
(
) -> Double
{
    return GKTurnTimeoutDefault;
}

@_cdecl("GKTurnBasedMatch_GetTurnTimeoutNone")
public func GKTurnBasedMatch_GetTurnTimeoutNone
(
) -> Double
{
    return GKTurnTimeoutNone;
}

@_cdecl("GKTurnBasedMatch_GetActiveExchanges")
public func GKTurnBasedMatch_GetActiveExchanges
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>

) -> UnsafeMutablePointer<NSArray>? // NSArray<GKTurnBasedExchange>
{
    let target = pointer.takeUnretainedValue();
    return (target.activeExchanges as? NSArray)?.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKTurnBasedMatch_GetCompletedExchanges")
public func GKTurnBasedMatch_GetCompletedExchanges
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>

) -> UnsafeMutablePointer<NSArray>? // NSArray<GKTurnBasedExchange>
{
    let target = pointer.takeUnretainedValue();
    return (target.completedExchanges as? NSArray)?.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKTurnBasedMatch_GetExchanges")
public func GKTurnBasedMatch_GetExchanges
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>

) -> UnsafeMutablePointer<NSArray>? // NSArray<GKTurnBasedExchange>
{
    let target = pointer.takeUnretainedValue();
    return (target.exchanges as? NSArray)?.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKTurnBasedMatch_GetExchangeDataMaximumSize")
public func GKTurnBasedMatch_GetExchangeDataMaximumSize
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>

) -> Int
{
    let target = pointer.takeUnretainedValue();
    return target.exchangeDataMaximumSize;
}

@_cdecl("GKTurnBasedMatch_GetExchangeMaxInitiatedExchangesPerPlayer")
public func GKTurnBasedMatch_GetExchangeMaxInitiatedExchangesPerPlayer
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>

) -> Int
{
    let target = pointer.takeUnretainedValue();
    return target.exchangeMaxInitiatedExchangesPerPlayer;
}

@_cdecl("GKTurnBasedMatch_SendReminder")
public func GKTurnBasedMatch_SendReminder
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>,
    taskId: Int64,
    participantsPtr: UnsafeMutablePointer<NSArray>, // NSArray<GKTurnBasedParticipant>
    localizableMessageKey: char_p,
    argumentsPtr: UnsafeMutablePointer<NSArray>, // NSArray<NSString>
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    let participants = participantsPtr.takeUnretainedValue() as! [GKTurnBasedParticipant];
    let arguments = argumentsPtr.takeUnretainedValue() as! [String];
    
    target.sendReminder(to: participants, localizableMessageKey: localizableMessageKey.toString(), arguments: arguments, completionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_SetLocalizableMessageWithKey")
public func GKTurnBasedMatch_SetLocalizableMessageWithKey
(
    gkTurnBasedMatchPtr: UnsafeMutablePointer<GKTurnBasedMatch>,
    key: char_p,
    argumentsPtr: UnsafeMutablePointer<NSArray>? // NSArray<NSString>
)
{
    let gkTurnBasedMatch = gkTurnBasedMatchPtr.takeUnretainedValue();
    let arguments = argumentsPtr?.takeUnretainedValue() as? [String];

    gkTurnBasedMatch.setLocalizableMessageWithKey(key.toString(), arguments: arguments);
}

@_cdecl("GKTurnBasedMatch_LoadMatches")
public func GKTurnBasedMatch_LoadMatches
(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<NSArray>, // NSArray<GKTurnBasedMatch>
    onError: @escaping NSErrorTaskCallback
)
{
    GKTurnBasedMatch.loadMatches(completionHandler: { matches, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }
        onSuccess(taskId, (matches as? NSArray)?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKTurnBasedMatch_Load")
public func GKTurnBasedMatch_Load
(
    taskId: Int64,
    matchId: char_p,
    onSuccess: @escaping SuccessTaskPtrCallback<GKTurnBasedMatch>,
    onError: @escaping NSErrorTaskCallback
)
{
    GKTurnBasedMatch.load(withID: matchId.toString(), withCompletionHandler: { match, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, match?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKTurnBasedMatch_Find")
public func GKTurnBasedMatch_Find
(
    taskId: Int64,
    matchRequestPtr: UnsafeMutablePointer<GKMatchRequest>,
    onSuccess: @escaping SuccessTaskPtrCallback<GKTurnBasedMatch>,
    onError: @escaping NSErrorTaskCallback
)
{
    let request = matchRequestPtr.takeUnretainedValue();

    GKTurnBasedMatch.find(for: request, withCompletionHandler: { match, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, match?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKTurnBasedMatch_AcceptInvite")
public func GKTurnBasedMatch_AcceptInvite
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<GKTurnBasedMatch>,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.acceptInvite(completionHandler: { match, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, match?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKTurnBasedMatch_DeclineInvite")
public func GKTurnBasedMatch_DeclineInvite
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.declineInvite(completionHandler: { error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_Rematch")
public func GKTurnBasedMatch_Rematch
(
    pointer: UnsafeMutablePointer<GKTurnBasedMatch>,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback<GKTurnBasedMatch>,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.rematch(completionHandler: { match, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, match?.passRetainedUnsafeMutablePointer());
    });
}
