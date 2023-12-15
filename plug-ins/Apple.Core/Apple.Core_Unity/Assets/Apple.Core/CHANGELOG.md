# CHANGELOG
All notable changes to this project will be documented in this file.

## [2.0.0] - 2023-11-09
### Added
- Add C# wrappers for `NSNull`, `NSNumber`, `NSObject`, `NSString`.
- Add C# wrapper for `NSMutableDictionary<,>`. See below for further details.
### Changed
- BREAKING CHANGES: There are a number of breaking changes to `NSArray` and `NSDictionary` to make them behave more like standard C# collections.
  - `NSArray<>` is now a generic class that implements `IReadOnlyList<>` and derives from `NSObject`.
  - `NSMutableArray<>` is now a generic class that implements `IList<>` and derives from `NSArray<>`. The `Init` factory method has been removed. New instances can simply be created by using `new`.
  - `NSDictionary<,>` is now a generic class that implements `IReadOnlyDictionary<,>` and derives from `NSObject`.
  - `NSMutableDictionary<,>` is a new generic class that implements `IDictionary<,>` and derives from `NSDictionary<,>`
  - Items added to these collections can be any of the following:
    - Primitive `System` types: `Boolean`, `Byte`, `SByte`, `Int16`, `UInt16`, `Int32`, `UInt32`, `Int64`, `UInt64`, `Single`, and `Double`
    - `System.String`
    - `NSObject` and subclasses.
  - Keys used with the dictionary collections can be any of the same types allowed for items except for `NSObject`. Any subclass of `NSObject` used as a key must implement the `NSCopying` protocol.
- BREAKING CHANGE: `NSError` now derives from `NSObject` instead of `System.Exception`.

## [1.0.5] - 2023-11-28
### Fixed
- Cleans up an issue with Unity 2022.1 and later in which Unity's Asset Database was unable to find frameworks or bundles.

## [1.0.4] - 2023-07-25
### Added
- API Availability checking
- Sample to highlight use of new API availability check feature.
- API Availability attributes to communicate API availability information to users and attach availability metadata to Apple Unity plug-in C# API

## [1.0.3] - 2022-12-16
### Changed
- Updated logging in AppleFrameworkUtility
- Updated AppleFrameworkUtility to search `/Libraries` along with `/Frameworks` for copied libraries

## [1.0.2] - 2022-11-29
### Changed
- Disabled bitcode generation in all native library project build targets.

## [1.0.1] - 2022-10-10
### Changed
- Updated package name to `com.apple.unityplugin.core`

## [1.0.0] - 2022-06-02
### Added
- Initial release.
