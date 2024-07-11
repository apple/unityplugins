# CHANGELOG
All notable changes to this project will be documented in this file.

## [3.1.3] - 2024-04-23
### Updated
- Updating how Info.plist files are generated for native libraries.
  - Info.plist files are now each generated when libraries are built
  - Settings for the Info.plist file are configured in Core.xcconfig
  - Updated projet to generate XML (human readable) for debug and binary for release when generating Info.plist
  - Encoded version(s) in the xcconfig and generated Info.plist should align with the plug-in version reported in `package.json`

## [3.1.2] - 2024-04-11
### Added
- *Embed Apple Plug-In Libraries* script injected into the Unity-generated Xcode project will now sign native libraries using the same codesign identity configured for that Xcode project.
  - This simplifies the codesign workflow substantially, now libraries won't need to be signed when running the build script.

## [3.1.1] - 2024-04-08
### Added
- Adding support for tracking of Apple Unity plug-ins without native libraries.

## [3.1.0] - 2024-02-23
### Added
Support for visionOS

## [3.0.0] - 2024-02-15
### Added
- Support for iPhone simulator and AppleTV Simulator
- Release builds now generate .dSYM files for enhanced debugging abilities
- Conditional compilation enabled when building for non-Apple platforms from the Mac Editor
### Changed
- Adopts custom native library processing, sidestepping Unity's asset processing for libraries for increased flexibility and feature support
- Updated folder hierarchy for plug-in library layout
- Updated plug-in asset importer for simulator and device library support
- Updated build post-processor to select correct library variants based upon Unity player settings
- Updated library copy with additional checks to ensure correct libraries are copied
- Updated native Xcode project to use an xcconfig file to determine where to copy libraries after compile based upon variant (device or simulator)

## [2.0.1] - 2024-02-15
### Added
- Add C# wrapper for `NSData`.
- Add Texture2D extension methods for loading images from `NSData` objects.
### Changed
- Improve equality checks between `NSObject` derived objects by using the native `IsEquals` method.
- Add helper methods to `NSString` C# wrapper for interoperating with `NSData`.
- Improve handling of exceptions thrown during P/Invoke callbacks.

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
