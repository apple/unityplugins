//
//  GKMatchmakerViewControllerDelegate.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public typealias GKMatchmakerDidFindMatchCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias GKMatchmakerDidFindHostedPlayersCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias GKMatchmakerCanceledCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias GKMatchmakerDidFailWithErrorCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias GKMatchmakerHostedPlayerDidAcceptCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;
public typealias GKMatchmakerGetMatchPropertiesForRecipientCallback = @convention(c) (UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer, UnsafeMutableRawPointer) -> Void;

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
            Unmanaged.passRetained(self).toOpaque(),
            Unmanaged.passRetained(viewController).toOpaque());
    }
    
    public func matchmakerViewController(_ viewController: GKMatchmakerViewController, didFailWithError error: Error) {
        GKMatchmakerViewController_Dismiss(viewController: viewController);

        DidFailWithError?(Unmanaged.passRetained(self).toOpaque(),
                          Unmanaged.passRetained(viewController).toOpaque(),
                          Unmanaged.passRetained(error as NSError).toOpaque());
    }
    
    public func matchmakerViewController(_ viewController: GKMatchmakerViewController, didFind match: GKMatch) {
        GKMatchmakerViewController_Dismiss(viewController: viewController);
        
        DidFindMatch?(Unmanaged.passRetained(self).toOpaque(),
                      Unmanaged.passRetained(viewController).toOpaque(),
                      Unmanaged.passRetained(match).toOpaque());
    }

    public func matchmakerViewController(_ viewController: GKMatchmakerViewController, hostedPlayerDidAccept player: GKPlayer) {
        HostedPlayerDidAccept?(
            Unmanaged.passRetained(self).toOpaque(),
            Unmanaged.passRetained(viewController).toOpaque(),
            Unmanaged.passRetained(player).toOpaque());
    }
    
    public func matchmakerViewController(_ viewController: GKMatchmakerViewController, didFindHostedPlayers players: [GKPlayer]) {
        GKMatchmakerViewController_Dismiss(viewController: viewController);

        DidFindHostedPlayers?(
            Unmanaged.passRetained(self).toOpaque(),
            Unmanaged.passRetained(viewController).toOpaque(),
            Unmanaged.passRetained(players as NSArray).toOpaque());
    }

    // This API passes a closure to C# which must be called on completion.
    // We box the closure in AnyObject that we marshal to the C# side which C# will pass back on completion.
    // Then we unbox the closure and call it from the Swift side.
    // See: https://stackoverflow.com/questions/65860970/swift-pass-escaping-closure-to-c-api-callback
    public func matchmakerViewController(_ viewController: GKMatchmakerViewController, getMatchPropertiesForRecipient recipient: GKPlayer, withCompletionHandler completionHandler: @escaping ([String : Any]) -> Void) {
        let completionHandlerWrapper = completionHandler as AnyObject
        GetMatchPropertiesForRecipient?(
            Unmanaged.passRetained(self).toOpaque(),
            Unmanaged.passRetained(viewController).toOpaque(),
            Unmanaged.passRetained(recipient).toOpaque(),
            Unmanaged.passRetained(completionHandlerWrapper).toOpaque());
    }
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetDidFindMatchCallback")
public func GKMatchmakerViewControllerDelegate_SetDidFindMatchCallback
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchmakerDidFindMatchCallback
)
{
    let target = Unmanaged<GKWMatchmakerViewControllerDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.DidFindMatch = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetDidFindHostedPlayersCallback")
public func GKMatchmakerViewControllerDelegate_SetDidFindHostedPlayersCallback
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchmakerDidFindHostedPlayersCallback
)
{
    let target = Unmanaged<GKWMatchmakerViewControllerDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.DidFindHostedPlayers = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback")
public func GKMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchmakerCanceledCallback
)
{
    let target = Unmanaged<GKWMatchmakerViewControllerDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.Canceled = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback")
public func GKMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchmakerDidFailWithErrorCallback
)
{
    let target = Unmanaged<GKWMatchmakerViewControllerDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.DidFailWithError = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetHostedPlayerDidAccept")
public func GKMatchmakerViewControllerDelegate_SetHostedPlayerDidAccept
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchmakerHostedPlayerDidAcceptCallback
)
{
    let target = Unmanaged<GKWMatchmakerViewControllerDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.HostedPlayerDidAccept = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_SetGetMatchPropertiesForRecipientCallback")
public func GKMatchmakerViewControllerDelegate_SetGetMatchPropertiesForRecipientCallback
(
    pointer: UnsafeMutableRawPointer,
    callback: @escaping GKMatchmakerGetMatchPropertiesForRecipientCallback
)
{
    let target = Unmanaged<GKWMatchmakerViewControllerDelegate>.fromOpaque(pointer).takeUnretainedValue();
    target.GetMatchPropertiesForRecipient = callback;
}

@_cdecl("GKMatchmakerViewControllerDelegate_CallGetMatchPropertiesForRecipientCompletionHandler")
public func GKMatchmakerViewControllerDelegate_CallGetMatchPropertiesForRecipientCompletionHandler
(
    completionHandlerWrapperPtr: UnsafeMutableRawPointer,
    nsDictionaryPtr: UnsafeMutableRawPointer
)
{
    let completionHandlerWrapper = Unmanaged<AnyObject>.fromOpaque(completionHandlerWrapperPtr).takeUnretainedValue()
    let completionHandler = completionHandlerWrapper as! ([String : Any]) -> Void

    let nsDictionary = Unmanaged<NSDictionary>.fromOpaque(nsDictionaryPtr).takeUnretainedValue();
    completionHandler(nsDictionary as! [String : Any])
}
