# CHANGELOG
All notable changes to this project will be documented in this file.

## [1.0.0] - 2026-06-10
- Initial release. Exposes Apple's Background Assets framework to Unity developers, enabling
  out-of-band delivery of asset packs from the App Store or from a self-hosted server via
  `AssetPackManager`, `AssetPack`, and `AssetPackManifest`.
- The build step automatically configures the generated Xcode project with a Background Assets
  downloader extension and the accompanying hosting configuration.
