//
//  GKSavedGameDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias SavedGameConflictingCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias SavedGameModifiedCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;

extension GKWLocalPlayerListener : GKSavedGameListener {
    
    public func player(_ player: GKPlayer, hasConflictingSavedGames conflicts: [GKSavedGame]) {
        SavedGameConflicting?(
            Unmanaged.passRetained(player).toOpaque(),
            Unmanaged.passRetained(conflicts as NSArray).toOpaque());
    }
    
    public func player(_ player: GKPlayer, didModifySavedGame modified: GKSavedGame) {
        SavedGameModified?(
            Unmanaged.passRetained(player).toOpaque(),
            Unmanaged.passRetained(modified).toOpaque());
    }
}

@_cdecl("GKSavedGame_SetSavedGameConflictingCallback")
public func GKSavedGame_SetSavedGameConflictingCallback(callback : @escaping SavedGameConflictingCallback)
{
    _localPlayerListener.SavedGameConflicting = callback;
}

@_cdecl("GKSavedGame_SetSavedGameModifiedCallback")
public func GKSavedGame_SetSavedGameModifiedCallback(callback : @escaping SavedGameModifiedCallback)
{
    _localPlayerListener.SavedGameModified = callback;
}
