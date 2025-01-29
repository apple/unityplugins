# CHANGELOG
All notable changes to this project will be documented in this file.

## [3.0.2] - 2025-01-29
- Remove references to some unused packages.

## [3.0.1] - 2024-12-10
- Add Apache 2.0 license file.

## [3.0.0] - 2023-09-05
### Added
- C# wrappers for the following GameKit APIs new in iOS/tvOS 18.0, macOS 15.0, and visionOS 2.0.
  - `GKGameCenterViewController.Init(leaderboardSetID)` and `.Init(player)`
  - `GKAccessPoint`
    - `.TriggerWithAchievementID(achievementID)`
    - `.TriggerWithLeaderboardSetID(leaderboardSetID)`
    - `.TriggerWithLeaderboardID(leaderboardID, playerScope, timeScope)`
    - `.TriggerWithPlayer(player)`
- C# wrappers for the following `GKLocalPlayer` methods:
  - `.LoadDefaultLeaderboardIdentifier()`
  - `.SetDefaultLeaderboardIdentifier(leaderboardIdentifier)`
- C# wrappers for the existing `GKSavedGame` family of APIs.
### Changed
  - Add C# events to `GKLocalPlayer` to handle authentication state changes.
    - `AuthenticateUpdate`
    - `AuthenticateError`
  - Improve precision of date, time, and duration properties of achievements, challenges, and leaderboards.
  - All of the `GKAccessPoint.Trigger*` methods are now asynchronous.

## [2.2.2] - 2023-04-23
### Updated
- Updating how Info.plist files are generated for native libraries.
  - Info.plist files are now each generated when libraries are built
  - Settings for the Info.plist file are configured in GameKit.xcconfig
  - Updated projet to generate XML (human readable) for debug and binary for release when generating Info.plist
  - Encoded version(s) in the xcconfig and generated Info.plist should align with the plug-in version reported in `package.json`

## [2.2.1] - 2023-04-01
- Fix some issues with how view controllers are handled on visionOS.

## [2.2.0] - 2023-02-23
### Added
- Support for visionOS

## [2.1.0] - 2023-02-15
### Updated
- Adopt Apple.Core 3.0.0

## [2.0.1] - 2024-02-15
### Added
- C# wrappers for the following GameKit APIs:
  - `GKLeaderboard.LoadEntriesForPlayers`
  - `GKMatchmaker.FinishMatchmaking`, `.Start/StopBrowsingForNearbyPlayers`, and `.Start/StopGroupActivity`
  - `GKMatchmakerViewController.AddPlayersToMatch`
  - `GKMatchRequest.RecipientResponse`, `.DefaultNumberOfPlayers`, and `.MaxPlayersAllowedForMatch`
  - `GKTurnBasedMatch.SetLocalizableMessageWithKey`
  - `GKAccessPoint.Trigger` (with state)
  - `GKAchievement.Init` (for player)
  - `GKAchievementDescription.IncompleteAchievementImage` and `.PlaceholderCompletedAchievementImage`
  - `GKLocalPlayer.LoadFriends` (with identifiers) and `.LoadFriendsAuthorizationStatus`
### Changed
  - Updated the GameKit sample app to demonstrate most of the newly wrapped APIs above.

## [2.0.0] - 2023-12-15
### Added
- Support new rule-based matchmaking APIs available in iOS/tvOS 17.2 and macOS 14.2.
- New `RarityPercent` property of `GKAchievementDescription`.
### Changed
- BREAKING CHANGE: Removed `NSArrayExtensions` since `NSArray<>` and `NSMutableArray<>` are now generic classes that no longer need these extensions.
- BREAKING CHANGE: Where appropriate, all `GK*` types now derive from `NSObject` rather than `InteropReference`.
- BREAKING CHANGE: `GameKitException` now contains `NSError` rather than deriving from it.
### Fixed
- Fixed a discrepency in the `GKErrorCode` enumeration.

## [1.0.4] - 2022-12-21
### Changed
- Post build script phase of native library project was failing to correctly copy libraries; updated to use Xcode's built in 'copy files' phase.
### Fixed
- Fixed an issue in which app would freeze/become unresponsible after viewing and closing leaderboards.

## [1.0.3] - 2022-11-29
### Changed
- Disabled bitcode generation in all native library project build targets.

## [1.0.2] - 2022-10-10
### Changed
- Update package name to `com.apple.unityplugin.gamekit`

## [1.0.1] - 2022-09-28
### Changed
- Preserve attribute added to GKAccessPoint constructor.
- Preserve attribute added to GKMatchMaker constructor.
### Fixed
- Unity code stripping no longer removes GKAccessPoint and GKMatchMaker constructors. This is necessary because these constructors are only ever called implicitly in the package source.
  - See [Unity Managed Code Stripping](https://docs.unity3d.com/Manual/ManagedCodeStripping.html) for more information.

## [1.0.0] - 2022-06-02
### Added
- Initial release.
