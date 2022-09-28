# CHANGELOG
All notable changes to this project will be documented in this file.

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
