//
//  GKScoreChallenge.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKScoreChallenge_GetScore")
public func GKScoreChallenge_GetScore
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let challenge = Unmanaged<GKScoreChallenge>.fromOpaque(pointer).takeUnretainedValue();
    
    if(challenge.score != nil) {
        return Unmanaged.passRetained(challenge.score!).toOpaque();
    }
    
    return nil;
}
