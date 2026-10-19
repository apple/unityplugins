# Apple Unity Plug-Ins


## Overview
The Apple Unity Plug-Ins expose a selection of Apple platform frameworks to Unity developers.

To get started with integration of these plug-ins into your Unity projects, run `python3 build.py` and use the `.tgz` packages in the `Build/` directory. For a more detailed walkthrough, please check out the [Quick Start Guide](Documentation/Quickstart.md), which also includes introductions to each plug-in.

| [Home](README.md) | [Feedback](Documentation/Feedback.md) | [Quick Start Guide](Documentation/Quickstart.md) | [Build Script](Documentation/BuildScript.md) |
| :---: | :---: | :---: | :---: |

| Plug-In | Description |
| :------ | :---------- |
| Apple.Core | Provides integrated build post process management and Editor UI.<br/>**Note:** Apple.Core is a dependency of all Apple Unity plug-ins.|
| Apple.Accessibility | Provides Apple's accessibility to Unity developers allowing adding supports to Apple's built-in assistive technologies such as VoiceOver.|
| Apple.BackgroundAssets | Exposes Apple’s Background Assets framework to Unity developers, enabling out-of-band delivery of asset packs from Apple or third-party servers. The plug-in also automatically configures generated Xcode projects with a downloader extension and everything else that’s necessary to use Background Assets. |
| Apple.CoreHaptics | Brings Apple's Core Haptics framework to Unity developers, enabling for customizable haptic patterns and in-depth playback control on supported devices. This Plug-In also includes UIKit's UIFeedbackGenerator API.|
| Apple.GameController | Exposes Apple's GameController framework to Unity developers allowing for rich controller features in macOS, iOS, and tvOS apps. |
| Apple.GameKit | Allows Unity developers to easily integrate GameKit features such as leaderboards, achievements, and match making. |
| Apple.PHASE | The PHASE plug-in allows Unity developers to take full advantage of Apple's new geometry and material aware spatial audio system. |
| Apple.SpatialController | Exposes Apple's AccessoryTracking and GameController frameworks to Unity developers on visionOS allowing for spatial controller features in visionOS apps. Requires visionOS 26.0 |
| Apple.StoreKit | Exposes Apple's StoreKit 2 framework to Unity developers, enabling in-app purchases, subscriptions, and transaction management with modern async/await patterns. |

## Minimum Supported OS Versions
| OS | Version |
| :- | :------ |
| iOS | 15.6 |
| macOS | 12.0 |
| tvOS | 15.6 |
| visionOS | 1.3 |
| watchOS | not supported |


## Debug Symbols

Each plug-in ships a `.dSYM` beside its native library, and the build step copies it into the archive when you build
for distribution. That is what makes plug-in frames appear as function names in crash reports rather than as raw
addresses, and it is why uploading to App Store Connect does not warn that a dSYM is missing for the framework.

Debug symbols never enter the app bundle. The `.ipa` you upload is the same size with or without them; they live in
the `.xcarchive` only, so the cost is local disk and upload time, not download size for players.

They are the larger part of each package for the same reason — the libraries themselves are stripped, so most of the
symbol information is in the dSYM rather than in the binary. If you do not want them, deleting the `.dSYM` folders
from `NativeLibraries~` after installing the package is enough; the build step copies whatever is there. Expect App
Store Connect to warn about the missing symbols if you do.

## Leaving Feedback
Leave an issue for the community or see the [Feedback](Documentation/Feedback.md) documentation for more information.
