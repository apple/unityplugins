//
//  SKOverlay.swift
//  StoreKitWrapper
//
//  Copyright © 2026 Apple, Inc. All rights reserved.
//

import StoreKit

#if os(iOS) || os(visionOS)

public typealias SKOverlayCallback = @convention(c) () -> Void
public typealias SKOverlayErrorCallback = @convention(c) (UnsafeMutableRawPointer) -> Void

@available(iOS 14.0, visionOS 2.2, *)
public class SKOverlayDelegateHandler : NSObject, SKOverlayDelegate {
    var onWillStartPresentation: SKOverlayCallback?
    var onDidFinishPresentation: SKOverlayCallback?
    var onWillStartDismissal: SKOverlayCallback?
    var onDidFinishDismissal: SKOverlayCallback?
    var onDidFailToLoad: SKOverlayErrorCallback?

    public func storeOverlayWillStartPresentation(_ overlay: SKOverlay, transitionContext: SKOverlay.TransitionContext) {
        onWillStartPresentation?()
    }

    public func storeOverlayDidFinishPresentation(_ overlay: SKOverlay, transitionContext: SKOverlay.TransitionContext) {
        onDidFinishPresentation?()
    }

    public func storeOverlayWillStartDismissal(_ overlay: SKOverlay, transitionContext: SKOverlay.TransitionContext) {
        onWillStartDismissal?()
    }

    public func storeOverlayDidFinishDismissal(_ overlay: SKOverlay, transitionContext: SKOverlay.TransitionContext) {
        onDidFinishDismissal?()
    }

    public func storeOverlayDidFailToLoad(_ overlay: SKOverlay, error: Error) {
        onDidFailToLoad?((error as NSError).passRetainedUnsafeMutableRawPointer())
    }
}

/// Stored globally to keep the delegate and overlay alive while presented.
@available(iOS 14.0, visionOS 2.2, *)
private var currentDelegateHandler: SKOverlayDelegateHandler? = nil

@available(iOS 14.0, visionOS 2.2, *)
@_cdecl("SKOverlay_Present")
public func SKOverlay_Present(
    appIdentifier: char_p,
    position: Int,
    onWillStartPresentation: @escaping SKOverlayCallback,
    onDidFinishPresentation: @escaping SKOverlayCallback,
    onWillStartDismissal: @escaping SKOverlayCallback,
    onDidFinishDismissal: @escaping SKOverlayCallback,
    onDidFailToLoad: @escaping SKOverlayErrorCallback
)
{
    guard let scene = UiUtilities.defaultWindow()?.windowScene else {
        return
    }

    let handler = SKOverlayDelegateHandler()
    handler.onWillStartPresentation = onWillStartPresentation
    handler.onDidFinishPresentation = onDidFinishPresentation
    handler.onWillStartDismissal = onWillStartDismissal
    handler.onDidFinishDismissal = onDidFinishDismissal
    handler.onDidFailToLoad = onDidFailToLoad
    currentDelegateHandler = handler

    let config = SKOverlay.AppConfiguration(appIdentifier: appIdentifier.toString(), position: SKOverlay.Position(rawValue: position) ?? .bottom)
    let overlay = SKOverlay(configuration: config)
    overlay.delegate = handler
    overlay.present(in: scene)
}

@available(iOS 14.0, visionOS 2.2, *)
@_cdecl("SKOverlay_Dismiss")
public func SKOverlay_Dismiss()
{
    guard let scene = UiUtilities.defaultWindow()?.windowScene else {
        return
    }

    SKOverlay.dismiss(in: scene)
}

#endif
