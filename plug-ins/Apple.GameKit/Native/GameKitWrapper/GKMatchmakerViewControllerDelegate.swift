//
//  GKMatchmakerViewControllerDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias GKMatchmakerDidFindMatchCallback = @convention(c) (
    UnsafeMutablePointer<GKWMatchmakerViewControllerDelegate>,
    UnsafeMutablePointer<GKMatchmakerViewController>,
    UnsafeMutablePointer<GKMatch>) -> Void;

public typealias GKMatchmakerDidFindHostedPlayersCallback = @convention(c) (
    UnsafeMutablePointer<GKWMatchmakerViewControllerDelegate>,
    UnsafeMutablePointer<GKMatchmakerViewController>,
    UnsafeMutablePointer<NSArray>) -> Void; // NSArray<GKPlayer>

public typealias GKMatchmakerCanceledCallback = @convention(c) (
    UnsafeMutablePointer<GKWMatchmakerViewControllerDelegate>,
    UnsafeMutablePointer<GKMatchmakerViewController>) -> Void;

public typealias GKMatchmakerDidFailWithErrorCallback = @convention(c) (
    UnsafeMutablePointer<GKWMatchmakerViewControllerDelegate>,
    UnsafeMutablePointer<GKMatchmakerViewController>,
    UnsafeMutablePointer<NSError>) -> Void;

public typealias GKMatchmakerHostedPlayerDidAcceptCallback = @convention(c) (
    UnsafeMutablePointer<GKWMatchmakerViewControllerDelegate>,
    UnsafeMutablePointer<GKMatchmakerViewController>,
    UnsafeMutablePointer<GKPlayer>) -> Void;

public typealias GKMatchmakerGetMatchPropertiesForRecipientCallback = @convention(c) (
    UnsafeMutablePointer<GKWMatchmakerViewControllerDelegate>,
    UnsafeMutablePointer<GKMatchmakerViewController>,
    UnsafeMutablePointer<GKPlayer>,
    UnsafeMutableRawPointer) -> Void;

public class GKWMatchmakerViewControllerDelegate : NSObject, GKMatchmakerViewControllerDelegate {
    public var DidFindMatch : GKMatchmakerDidFindMatchCallback? = nil;
    public var DidFindHostedPlayers: GKMatchmakerDidFindHostedPlayersCallback? = nil;
    public var Canceled: GKMatchmakerCanceledCallback? = nil;
    public var DidFailWithError: GKMatchmakerDidFailWithErrorCallback? = nil;
    public var HostedPlayerDidAccept: GKMatchmakerHostedPlayerDidAcceptCallback? = nil;
    public var GetMatchPropertiesForRecipient: GKMatchmakerGetMatchPropertiesForRecipientCallback? = nil;

    public func matchmakerViewControllerWasCancelled(_ viewController: GKMatchmakerViewController) {
        GKMatchmakerViewController_Dismiss(viewController: viewController);

        Canceled?(
            self.passRetainedUnsafeMutablePointer(),
            viewController.passRetainedUnsafeMutablePointer());
    }

    public func matchmakerViewController(_ viewController: GKMatchmakerViewController, didFailWithError error: Error) {
        GKMatchmakerViewController_Dismiss(viewController: viewController);

        DidFailWithError?(self.passRetainedUnsafeMutablePointer(),
                          viewController.passRetainedUnsafeMutablePointer(),
                          (error as NSError).passRetainedUnsafeMutablePointer());
    }

    public func matchmakerViewController(_ viewController: GKMatchmakerViewController, didFind match: GKMatch) {
        GKMatchmakerViewController_Dismiss(viewController: viewController);
        
        DidFindMatch?(self.passRetainedUnsafeMutablePointer(),
                      viewController.passRetainedUnsafeMutablePointer(),
                      match.passRetainedUnsafeMutablePointer());
    }

    public func matchmakerViewController(_ viewController: GKMatchmakerViewController, hostedPlayerDidAccept player: GKPlayer) {
        HostedPlayerDidAccept?(
            self.passRetainedUnsafeMutablePointer(),
            viewController.passRetainedUnsafeMutablePointer(),
            player.passRetainedUnsafeMutablePointer());
    }
    
    public func matchmakerViewController(_ viewController: GKMatchmakerViewController, didFindHostedPlayers players: [GKPlayer]) {
        GKMatchmakerViewController_Dismiss(viewController: viewController);

        DidFindHostedPlayers?(
            self.passRetainedUnsafeMutablePointer(),
            viewController.passRetainedUnsafeMutablePointer(),
            (players as NSArray).passRetainedUnsafeMutablePointer());
    }

    // This API passes a closure to C# which must be called on completion.
    // We box the closure in AnyObject that we marshal to the C# side which C# will pass back on completion.
    // Then we unbox the closure and call it from the Swift side.
    // See: https://stackoverflow.com/questions/65860970/swift-pass-escaping-closure-to-c-api-callback
    public func matchmakerViewController(_ viewController: GKMatchmakerViewController, getMatchPropertiesForRecipient recipient: GKPlayer, withCompletionHandler completionHandler: @escaping ([String : Any]) -> Void) {
        let completionHandlerWrapper = completionHandler as AnyObject
        GetMatchPropertiesForRecipient?(
            self.passRetainedUnsafeMutablePointer(),
            viewController.passRetainedUnsafeMutablePointer(),
            recipient.passRetainedUnsafeMutablePointer(),
            Unmanaged.passRetained(completionHandlerWrapper).toOpaque());
    }
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetDidFindMatchCallback")
public func GKMatchmakerViewControllerDelegate_SetDidFindMatchCallback
(
    pointer: UnsafeMutablePointer<GKWMatchmakerViewControllerDelegate>,
    callback: @escaping GKMatchmakerDidFindMatchCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.DidFindMatch = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetDidFindHostedPlayersCallback")
public func GKMatchmakerViewControllerDelegate_SetDidFindHostedPlayersCallback
(
    pointer: UnsafeMutablePointer<GKWMatchmakerViewControllerDelegate>,
    callback: @escaping GKMatchmakerDidFindHostedPlayersCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.DidFindHostedPlayers = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback")
public func GKMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback
(
    pointer: UnsafeMutablePointer<GKWMatchmakerViewControllerDelegate>,
    callback: @escaping GKMatchmakerCanceledCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.Canceled = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback")
public func GKMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback
(
    pointer: UnsafeMutablePointer<GKWMatchmakerViewControllerDelegate>,
    callback: @escaping GKMatchmakerDidFailWithErrorCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.DidFailWithError = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetHostedPlayerDidAccept")
public func GKMatchmakerViewControllerDelegate_SetHostedPlayerDidAccept
(
    pointer: UnsafeMutablePointer<GKWMatchmakerViewControllerDelegate>,
    callback: @escaping GKMatchmakerHostedPlayerDidAcceptCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.HostedPlayerDidAccept = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetGetMatchPropertiesForRecipientCallback")
public func GKMatchmakerViewControllerDelegate_SetGetMatchPropertiesForRecipientCallback
(
    pointer: UnsafeMutablePointer<GKWMatchmakerViewControllerDelegate>,
    callback: @escaping GKMatchmakerGetMatchPropertiesForRecipientCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.GetMatchPropertiesForRecipient = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_CallGetMatchPropertiesForRecipientCompletionHandler")
public func GKMatchmakerViewControllerDelegate_CallGetMatchPropertiesForRecipientCompletionHandler
(
    completionHandlerWrapperPtr: UnsafeMutableRawPointer,
    nsDictionaryPtr: UnsafeMutablePointer<NSDictionary>
)
{
    let completionHandlerWrapper = Unmanaged<AnyObject>.fromOpaque(completionHandlerWrapperPtr).takeUnretainedValue()
    let completionHandler = completionHandlerWrapper as! ([String : Any]) -> Void

    let nsDictionary = nsDictionaryPtr.takeUnretainedValue();
    completionHandler(nsDictionary as! [String : Any])
}
