# CHANGELOG
All notable changes to this project will be documented in this file.

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
