//
//  UiUtilities.swift
//  AppleCoreNative
//

import Foundation
import GameKit
import SwiftUI

class UiUtilities {
#if os(macOS)
    static func defaultWindow() -> NSWindow? {
        return NSApplication.shared.keyWindow;
    }

    static func rootViewController() -> NSViewController? {
        return defaultWindow()?.contentViewController;
    }

    static func presentViewController(viewController: NSViewController) {
        rootViewController()?.presentAsModalWindow(viewController);
    }

    static func presentViewController(viewController: any NSViewController & GKViewController) {
        GKDialogController.shared().parentWindow = defaultWindow();
        GKDialogController.shared().present(viewController);
    }

    static func dismissViewController(viewController: any NSViewController & GKViewController) {
        GKDialogController.shared().dismiss(viewController);
    }

#else
    static func defaultWindow() -> UIWindow? {
        guard let windowScene = UIApplication.shared.connectedScenes.first(where: { scene in
            return scene is UIWindowScene;
        }) as? UIWindowScene else {
            return nil;
        }
        if let window = windowScene.windows.first(where: { window in
            return window.rootViewController != nil;
        }) {
            return window;
        }
        return nil;
    }

    static func rootViewController() -> UIViewController? {
        return defaultWindow()?.rootViewController;
    }

    static func presentViewController(viewController: UIViewController) {
        rootViewController()?.present(viewController, animated: true);
    }

    static func dismissViewController(viewController: UINavigationController) {
        viewController.dismiss(animated: true);
    }
#endif
}
