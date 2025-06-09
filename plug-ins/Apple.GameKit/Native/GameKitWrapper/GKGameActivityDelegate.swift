//
//  GKGameActivityDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2025 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias WantsToPlayActivityCompletionHandler = @Sendable (Bool) -> Void;

public typealias WantsToPlayActivityCallback = @convention(c) (
    UnsafeMutablePointer<GKPlayer>,
    UnsafeMutableRawPointer /*GKGameActivity*/,
    UnsafeMutableRawPointer /*WantsToPlayActivityCompletionHandlerContainer*/) -> Void;

class WantsToPlayActivityCompletionHandlerContainer : NSObject {
    public var completionHandler : WantsToPlayActivityCompletionHandler? = nil;
}

extension GKWLocalPlayerListener : GKGameActivityListener {

    @available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *)
    public func player(_ player: GKPlayer, wantsToPlay activity: GKGameActivity, completionHandler completion: @escaping WantsToPlayActivityCompletionHandler) {
        guard let wantsToPlayActivity = WantsToPlayActivity else {
            completion(false);
            return;
        }

        let container = WantsToPlayActivityCompletionHandlerContainer();
        container.completionHandler = completion;

        wantsToPlayActivity(
            player.passRetainedUnsafeMutablePointer(),
            activity.passRetainedUnsafeMutableRawPointer(),
            container.passRetainedUnsafeMutableRawPointer());
    }
}

@_cdecl("GKGameActivity_SetWantsToPlayCallback")
public func GKGameActivity_SetWantsToPlayCallback(callback : @escaping WantsToPlayActivityCallback) {
    if #available(iOS 19.0, macOS 16.0, tvOS 19.0, visionOS 3.0, *) {
        _localPlayerListener.WantsToPlayActivity = callback;
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKGameActivity_WantsToPlayCallbackCompletionHandler_Invoke")
public func GKGameActivity_WantsToPlayCallbackCompletionHandler_Invoke
(
    completionHandlerContainerPtr: UnsafeMutableRawPointer, // WantsToPlayActivityCompletionHandlerContainer
    result: Bool
)
{
    let completionHandlerContainer: WantsToPlayActivityCompletionHandlerContainer = completionHandlerContainerPtr.takeUnretainedValue();
    completionHandlerContainer.completionHandler?(result);
}
