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
    pointer: UnsafeMutablePointer<GKScoreChallenge>
) -> UnsafeMutablePointer<GKScore>?
{
    let challenge = pointer.takeUnretainedValue();
    return challenge.score?.passRetainedUnsafeMutablePointer();
}
