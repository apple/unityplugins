#! /usr/bin/env python3
# Requirements: python3

# Plug-in Identifiers (-p, --plugin-list)
# Selection identifiers for which plug-ins to perform build actions upon. Build script will ignore actions for unselected plug-ins.
class PluginID:
    ACCESSIBILITY = "Accessibility"
    CORE = "Core"
    CORE_HAPTICS = "CoreHaptics"
    GAME_CONTROLLER = "GameController"
    GAME_KIT = "GameKit"
    PHASE = "PHASE"
    ALL = "all"

# Platform Identifiers (-m, --platforms)
# Selection identifiers for which platforms target when building.
class PlatformID:
    IOS = "iOS"
    MACOS = "macOS"
    TVOS = "tvOS"
    ALL = "all"

# Platform config identifiers (-d, --debug)
class ConfigID:
    RELEASE = "Release"
    DEBUG = "Debug"

# Build Actions (-b, --build-action)
class BuildActionID:
    # Builds each selected plug-in's native frameworks and moves them to associated Unity plug-in project folder hierarchy
    BUILD = "build"

    # Runs 'npm pack' on each selected plug-in and saves resulting package to the current output_path (See option: --output-path)
    PACK = "pack"

    # Skips all build actions. Used when only a clean action is desired.
    NONE = "none"

    # Performs all builds actions for each selected plug-in
    ALL = "all"

# Clean Actions (-k, --clean-action)
class CleanActionID:
    # Removes native libraries and associated .meta files from within the selected plug-in Unity projects (See option: --plugin-list)
    NATIVE = "native"

    # Removes packages for the selected plug-ins in the current output_path (See option: --output-path)
    PACKAGES = "packages"

    # Removes all output under test_output_path (See option: --test-output-path)
    TESTS = "tests"

    # Skips all clean actions.
    NONE = "none"

    # Performs all clean actions for the selected plug-ins
    ALL = "all"
