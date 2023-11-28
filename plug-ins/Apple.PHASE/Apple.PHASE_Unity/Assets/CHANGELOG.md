# CHANGELOG
All notable changes to this project will be documented in this file.

## [1.0.4] - 2023-06-08
### Added
- Ability to get/set listener global gain level in editor and programatically at runtime.
- Ability to get/set source gain level in editor and programatically at runtime.
- Ability to get/set mixer node gain level in Sound Event composer and programatically at runtime.

## [1.0.3] - 2023-03-07
### Changed
- Updated README to include info on Samples.

### Fixed
- Cone directivity orientation.
- Set undefined fields in Prefabs.
- Ensures Sound Events are registered before playback.
- Resolve testDestroyListenerDuringPlayback unit test failure.
 
## [1.0.2] - 2022-11-29
### Changed
- Disabled bitcode generation in all native library project build targets.

## [1.0.1] - 2022-10-10
### Changed
- Updated package name to `com.apple.unityplugin.phase`.

## [1.0.0] - 2022-06-02
### Added
- Initial release.
