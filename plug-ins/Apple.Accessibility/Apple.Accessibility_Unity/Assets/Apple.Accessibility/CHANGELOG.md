# CHANGELOG
All notable changes to this project will be documented in this file.

## [1.1.1] - 2024-04-23
### Updated
- Updating how Info.plist files are generated for native libraries.
  - Info.plist files are now each generated when libraries are built
  - Settings for the Info.plist file are configured in Accessibility.xcconfig
  - Updated projet to generate XML (human readable) for debug and binary for release when generating Info.plist
  - Encoded version(s) in the xcconfig and generated Info.plist should align with the plug-in version reported in `package.json`

## [1.1.0] - 2024-02-11
### Updated
- Adopt Apple.Core 3.0.0

## [1.0.2] - 2022-11-29
### Changed
- Disabled bitcode generation in all native library project build targets.

## [1.0.1] - 2022-10-10
### Changed
- Updated package name to `com.apple.unityplugin.accessibility`

## [1.0.0] - 2022-06-02
### Added
- Initial release.
