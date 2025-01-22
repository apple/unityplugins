//
//  GKSavedGameDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2024 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

#if !os(tvOS)
public typealias SavedGameModifiedCallback = @convention(c) (UnsafeMutablePointer<GKPlayer>, UnsafeMutablePointer<GKSavedGame>) -> Void;
public typealias SavedGamesConflictingCallback = @convention(c) (UnsafeMutablePointer<GKPlayer>, UnsafeMutablePointer<NSArray> /*NSArray<GKSavedGame>*/) -> Void;
#endif

#if !os(tvOS)
extension GKWLocalPlayerListener : GKSavedGameListener {

    public func player(_ player: GKPlayer, didModifySavedGame savedGame: GKSavedGame) {
        SavedGameModified?(
            player.passRetainedUnsafeMutablePointer(),
            savedGame.passRetainedUnsafeMutablePointer());
    }

    public func player(_ player: GKPlayer, hasConflictingSavedGames savedGames: [GKSavedGame]) {
        SavedGamesConflicting?(
            player.passRetainedUnsafeMutablePointer(),
            (savedGames as NSArray).passRetainedUnsafeMutablePointer());
    }
}
#endif

#if !os(tvOS)
@_cdecl("GKSavedGame_SetSavedGameModifiedCallback")
public func GKSavedGame_SetSavedGameModifiedCallback(callback : @escaping SavedGameModifiedCallback)
{
    _localPlayerListener.SavedGameModified = callback;
}
#endif

#if !os(tvOS)
@_cdecl("GKSavedGame_SetSavedGamesConflictingCallback")
public func GKSavedGame_SetSavedGamesConflictingCallback(callback : @escaping SavedGamesConflictingCallback)
{
    _localPlayerListener.SavedGamesConflicting = callback;
}
#endif
