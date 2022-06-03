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
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let challenge = Unmanaged<GKAchievementChallenge>.fromOpaque(pointer).takeUnretainedValue();
    
    if(challenge.achievement != nil) {
        return Unmanaged.passRetained(challenge.achievement!).toOpaque();
    }
    
    return nil;
}
