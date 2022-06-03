//
//  GKTurnBasedMatch.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKTurnBasedMatch_Free")
public func GKTurnBasedMatch_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).autorelease();
}

@_cdecl("GKTurnBasedMatch_GetParticipants")
public func GKTurnBasedMatch_GetParticipants
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    return Unmanaged.passRetained(target.participants as NSArray).toOpaque();
}

@_cdecl("GKTurnBasedMatch_GetCurrentParticipant")
public func GKTurnBasedMatch_GetCurrentParticipant
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.currentParticipant != nil) {
        return Unmanaged.passRetained(target.currentParticipant!).toOpaque();
    }
    return nil;
}

@_cdecl("GKTurnBasedMatch_GetMatchData")
public func GKTurnBasedMatch_GetMatchData
(
    pointer: UnsafeMutableRawPointer
) -> InteropStructArray
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.matchData != nil) {
        return InteropStructArray(pointer: target.matchData!.toUCharP(), length: Int32(target.matchData!.count));
    }
    
    return InteropStructArray();
}

@_cdecl("GKTurnBasedMatch_GetMatchDataMaximumSize")
public func GKTurnBasedMatch_GetMatchDataMaximumSize
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    return target.matchDataMaximumSize;
}

@_cdecl("GKTurnBasedMatch_GetMessage")
public func GKTurnBasedMatch_GetMessage
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    return target.message?.toCharPCopy();
}

@_cdecl("GKTurnBasedMatch_GetMatchID")
public func GKTurnBasedMatch_GetMatchID
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    return target.matchID.toCharPCopy();
}

@_cdecl("GKTurnBasedMatch_GetCreationDate")
public func GKTurnBasedMatch_GetCreationDate
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    return Int(target.creationDate.timeIntervalSince1970);
}

@_cdecl("GKTurnBasedMatch_GetStatus")
public func GKTurnBasedMatch_GetStatus
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    return target.status.rawValue;
}

public typealias SuccessTaskInteropStructArrayCallback = @convention(c) (Int64, InteropStructArray) -> Void;

@_cdecl("GKTurnBasedMatch_LoadMatchData")
public func GKTurnBasedMatch_LoadMatchData
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskInteropStructArrayCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    target.loadMatchData(completionHandler: { data, error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        if(data != nil) {
            onSuccess(taskId, InteropStructArray(pointer: data!.toUCharP(), length: Int32(data!.count)));
        } else {
            onSuccess(taskId, InteropStructArray());
        }
    });
}

@_cdecl("GKTurnBasedMatch_SaveCurrentTurn")
public func GKTurnBasedMatch_SaveCurrentTurn
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    data: InteropStructArray,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    target.saveCurrentTurn(withMatch: data.toData(), completionHandler: { error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_EndTurn")
public func GKTurnBasedMatch_EndTurn
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    participantsPtr: UnsafeMutableRawPointer,
    timeout: Double,
    data: InteropStructArray,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    let participants = Unmanaged<NSArray>.fromOpaque(participantsPtr).takeUnretainedValue() as! [GKTurnBasedParticipant];
    
    target.endTurn(withNextParticipants: participants, turnTimeout: TimeInterval(timeout), match: data.toData(), completionHandler: { error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_ParticipantQuitInTurn")
public func GKTurnBasedMatch_ParticipantQuitInTurn
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    outcome: Int,
    participantsPtr: UnsafeMutableRawPointer,
    timeout: Double,
    data: InteropStructArray,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    let participants = Unmanaged<NSArray>.fromOpaque(participantsPtr).takeUnretainedValue() as! [GKTurnBasedParticipant];
    
    target.participantQuitInTurn(with: GKTurnBasedMatch.Outcome(rawValue: outcome)!, nextParticipants: participants, turnTimeout: TimeInterval(timeout), match: data.toData(), completionHandler: { error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_ParticipantQuitOutOfTurn")
public func GKTurnBasedMatch_ParticipantQuitOutOfTurn
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    outcome: Int,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    
    target.participantQuitOutOfTurn(with: GKTurnBasedMatch.Outcome(rawValue: outcome)!, withCompletionHandler: { error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_EndMatchInTurn")
public func GKTurnBasedMatch_EndMatchInTurn
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    data: InteropStructArray,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();

    target.endMatchInTurn(withMatch: data.toData(), completionHandler: { error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_Remove")
public func GKTurnBasedMatch_Remove
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    target.remove(completionHandler: { error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_SaveMergedMatch")
public func GKTurnBasedMatch_SaveMergedMatch
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    data: InteropStructArray,
    exchangesPtr: UnsafeMutableRawPointer,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    let exchanges = Unmanaged<NSArray>.fromOpaque(exchangesPtr).takeUnretainedValue() as! [GKTurnBasedExchange];

    target.saveMergedMatch(data.toData(), withResolvedExchanges: exchanges, completionHandler: { error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_SendExchange")
public func GKTurnBasedMatch_SendExchange
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    participantsPtr: UnsafeMutableRawPointer,
    data: InteropStructArray,
    localizableMessageKey: char_p,
    argumentsPtr: UnsafeMutableRawPointer,
    timeout: Double,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    let participants = Unmanaged<NSArray>.fromOpaque(participantsPtr).takeUnretainedValue() as! [GKTurnBasedParticipant];
    let arguments = Unmanaged<NSArray>.fromOpaque(argumentsPtr).takeUnretainedValue() as! [String];
 
    target.sendExchange(to: participants, data: data.toData(), localizableMessageKey: localizableMessageKey.toString(), arguments: arguments, timeout: TimeInterval(timeout), completionHandler: { exchange, error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        if(exchange != nil) {
            onSuccess(taskId, Unmanaged.passRetained(exchange!).toOpaque());
        } else {
            onSuccess(taskId, nil);
        }
        
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
    pointer: UnsafeMutableRawPointer
    
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.activeExchanges != nil) {
        return Unmanaged.passRetained(target.activeExchanges! as NSArray).toOpaque();
    }
    
    return nil;
}

@_cdecl("GKTurnBasedMatch_GetCompletedExchanges")
public func GKTurnBasedMatch_GetCompletedExchanges
(
    pointer: UnsafeMutableRawPointer
    
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.completedExchanges != nil) {
        return Unmanaged.passRetained(target.completedExchanges! as NSArray).toOpaque();
    }
    
    return nil;
}

@_cdecl("GKTurnBasedMatch_GetExchanges")
public func GKTurnBasedMatch_GetExchanges
(
    pointer: UnsafeMutableRawPointer
    
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    
    if(target.exchanges != nil) {
        return Unmanaged.passRetained(target.exchanges! as NSArray).toOpaque();
    }
    
    return nil;
}

@_cdecl("GKTurnBasedMatch_GetExchangeDataMaximumSize")
public func GKTurnBasedMatch_GetExchangeDataMaximumSize
(
    pointer: UnsafeMutableRawPointer
    
) -> Int
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    return target.exchangeDataMaximumSize;
}

@_cdecl("GKTurnBasedMatch_GetExchangeMaxInitiatedExchangesPerPlayer")
public func GKTurnBasedMatch_GetExchangeMaxInitiatedExchangesPerPlayer
(
    pointer: UnsafeMutableRawPointer
    
) -> Int
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    return target.exchangeMaxInitiatedExchangesPerPlayer;
}

@_cdecl("GKTurnBasedMatch_SendReminder")
public func GKTurnBasedMatch_SendReminder
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    participantsPtr: UnsafeMutableRawPointer,
    localizableMessageKey: char_p,
    argumentsPtr: UnsafeMutableRawPointer,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    let participants = Unmanaged<NSArray>.fromOpaque(participantsPtr).takeUnretainedValue() as! [GKTurnBasedParticipant];
    let arguments = Unmanaged<NSArray>.fromOpaque(argumentsPtr).takeUnretainedValue() as! [String];
    
    target.sendReminder(to: participants, localizableMessageKey: localizableMessageKey.toString(), arguments: arguments, completionHandler: { error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_LoadMatches")
public func GKTurnBasedMatch_LoadMatches
(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    GKTurnBasedMatch.loadMatches(completionHandler: { matches, error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        if(matches != nil) {
            onSuccess(taskId, Unmanaged.passRetained(matches! as NSArray).toOpaque());
        } else {
            onSuccess(taskId, nil);
        }
    });
}

@_cdecl("GKTurnBasedMatch_Load")
public func GKTurnBasedMatch_Load
(
    taskId: Int64,
    matchId: char_p,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    GKTurnBasedMatch.load(withID: matchId.toString(), withCompletionHandler: { match, error in
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

@_cdecl("GKTurnBasedMatch_Find")
public func GKTurnBasedMatch_Find
(
    taskId: Int64,
    matchRequestPtr: UnsafeMutableRawPointer,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let request = Unmanaged<GKMatchRequest>.fromOpaque(matchRequestPtr).takeUnretainedValue();
    
    GKTurnBasedMatch.find(for: request, withCompletionHandler: { match, error in
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

@_cdecl("GKTurnBasedMatch_AcceptInvite")
public func GKTurnBasedMatch_AcceptInvite
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    target.acceptInvite(completionHandler: { match, error in
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

@_cdecl("GKTurnBasedMatch_DeclineInvite")
public func GKTurnBasedMatch_DeclineInvite
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    target.declineInvite(completionHandler: { error in
        if(error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        onSuccess(taskId);
    });
}

@_cdecl("GKTurnBasedMatch_Rematch")
public func GKTurnBasedMatch_Rematch
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKTurnBasedMatch>.fromOpaque(pointer).takeUnretainedValue();
    target.rematch(completionHandler: { match, error in
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
