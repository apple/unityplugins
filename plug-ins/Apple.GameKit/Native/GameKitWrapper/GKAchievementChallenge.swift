//
//  GKAchievementChallenge.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKAchievementChallenge_GetAchievement")
public func GKAchievementChallenge_GetAchievement
(
    pointer: UnsafeMutablePointer<GKAchievementChallenge>
) -> UnsafeMutablePointer<GKAchievement>?
{
    let challenge = pointer.takeUnretainedValue();
    return challenge.achievement?.passRetainedUnsafeMutablePointer();
}
