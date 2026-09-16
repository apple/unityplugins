# CHANGELOG
All notable changes to this project will be documented in this file.

## [1.3.2] - 2026-09-16
### Changed
- Updated the asset picker and Ricochet sample for APIs deprecated in Unity 6 (`HierarchyProperty`,
  `Object.GetInstanceID()`, `Rigidbody2D.velocity`, `Resolution.refreshRate`), while still building
  against the Unity 2022.3 floor.
- Updated the native project for Xcode 27 and raised its deployment targets to iOS 15.0, tvOS 15.0,
  and macOS 12.0, matching the minimum OS versions the plug-ins already document as supported.
- Now requires Apple.Core 3.3.0.

### Fixed
- Added `[MarshalAs(UnmanagedType.I1)]` to `bool` P/Invoke declarations so `bool` results marshal
  correctly.
- Fixed `UnityPickers.AssetPickerDrawer` exceptions when `assetType` or `fieldInfo` is null. Thanks
  to [@ilterbilguven](https://github.com/ilterbilguven)
  ([#47](https://github.com/apple/unityplugins/pull/47)).
- Fixed plug-in builds when the source root path contains a space character.

## [1.3.1] - 2025-01-29
- Remove references to some unused packages.

## [1.3.0] - 2025-01-13
### Changed
- Pattern serialization supports systems that use commas for decimals

## [1.2.3] - 2024-12-10
- Add Apache 2.0 license file.
- Remove out-of-date documentation file.

## [1.2.2] - 2024-09-05
### Changed
- Cleaned up default project code sign settings.

## [1.2.1] - 2024-04-23
### Updated
- Updating how Info.plist files are generated for native libraries.
  - Info.plist files are now each generated when libraries are built
  - Settings for the Info.plist file are configured in CoreHaptics.xcconfig
  - Updated project to generate XML (human readable) for debug and binary for release when generating Info.plist
  - Encoded version(s) in the xcconfig and generated Info.plist should align with the plug-in version reported in `package.json`

## [1.2.0] - 2024-02-23
### Added
- Support for visionOS

## [1.1.0] - 2024-02-11
### Updated
- Adopt Apple.Core 3.0.0

## [1.0.2] - 2022-11-29
### Changed
- Disabled bitcode generation in all native library project build targets.

## [1.0.1] - 2022-10-10
### Changed
- Updated package name to `com.apple.unityplugin.corehaptics`

## [1.0.0] - 2022-06-02
### Added
- Initial release.
