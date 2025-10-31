#! /usr/bin/env python3
# Requirements: python3

# Plug-in Identifiers (-p, --plugin-list)
# Selection identifiers for which plug-ins to perform build actions upon. Build script will ignore actions for unselected plug-ins.
class PluginID:
    ACCESSIBILITY = "Accessibility"
    CORE = "Core"
    CORE_HAPTICS = "CoreHaptics"
    GAME_CONTROLLER = "GameController"
    SPATIAL_CONTROLLER = "SpatialController"
    GAME_KIT = "GameKit"
    PHASE = "PHASE"
    ALL = "all"

# Platform Identifiers (-m, --platforms)
# Selection identifiers for which platforms target when building.
class PlatformID:
    IOS = "iOS"
    TVOS = "tvOS"
    MACOS = "macOS"
    VISIONOS = "visionOS"
    IOS_SIMULATOR = "iPhoneSimulator"
    TVOS_SIMULATOR = "AppleTVSimulator"
    VISIONOS_SIMULATOR = "VisionSimulator"

    # Just simulator platforms: iOS-simulator, tvOS-simulator
    SIMULATORS = "simulators"

    # Just device platforms: iOS, tvOS, macOS
    DEVICES = "devices"

    # All supported devices and simulators - depends upon the platform SDKs installed on the machine running the script.
    ALL = "all"

# Platform config identifiers (-bc, --build-config)
#  Note: command argument '-d' (--debug) has been obsoleted.
class ConfigID:
    RELEASE = "Release"
    DEBUG = "Debug"

    # Builds both Debug and Release libraries.
    ALL = "all"

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

# Code sign options (-c, --codesign-identity)
class CodeSignActionID:
    # In general the -c flag takes a codesign identity hash as an argument, but when this argument is provided the script will enable the code signing identity selection workflow
    PROMPT = "prompt"
