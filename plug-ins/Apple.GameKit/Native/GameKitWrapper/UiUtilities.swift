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
        // Find the "best" default window (main, key, or other) that also has a contentViewController.
        // Otherwise, find the "best" one of those that doesn't have a contentViewController.
        if let window = NSApplication.shared.mainWindow,
           let _ = window.contentViewController {
            return window;
        } else if let window = NSApplication.shared.keyWindow,
                  let _ = window.contentViewController {
            return window;
        } else if let window = NSApplication.shared.windows.first(where: { window in
            return window.contentViewController != nil;
        }) {
            return window;
        } else if let window = NSApplication.shared.mainWindow {
            return window;
        } else if let window = NSApplication.shared.keyWindow {
            return window;
        } else {
            return NSApplication.shared.windows.first;
        }
    }

    static func rootViewController() -> NSViewController? {
        return defaultWindow()?.contentViewController;
    }

    static func presentViewController(viewController: NSViewController) {
        guard let window = defaultWindow() else {
            return;
        }

        var vc = window.contentViewController;
        if (vc == nil)
        {
            let childWindow = NSWindow(contentViewController: viewController);
            window.addChildWindow(childWindow, ordered: NSWindow.OrderingMode.above);
            vc = childWindow.contentViewController;
        }

        vc?.presentAsModalWindow(viewController);
    }

    static func presentViewController(viewController: any NSViewController & GKViewController) {
        GKDialogController.shared().parentWindow = defaultWindow();
        GKDialogController.shared().present(viewController);
    }

    static func dismissViewController(viewController: NSViewController) {
        rootViewController()?.dismiss(viewController);
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

    static func dismissViewController(viewController: UIViewController) {
        viewController.dismiss(animated: true);
    }
#endif
}
