#! /usr/bin/env python3
# Requirements: python3

from pathlib import Path
from scripts.python.upi_utility import Printer
from scripts.python.upi_cli_argument_options import PlatformID

# --
class BuildInfo:
    def __init__(self, platform_root : str, build_destination : str) -> None:
        self.platform_root = platform_root
        self.build_destination = build_destination

BUILD_INFO_TABLE = {
    PlatformID.IOS : BuildInfo("iOS", "generic/platform=iOS"),
    PlatformID.IOS_SIMULATOR : BuildInfo("iOS", "generic/platform=iOS Simulator"),
    PlatformID.TVOS : BuildInfo("tvOS", "generic/platform=tvOS"),
    PlatformID.TVOS_SIMULATOR : BuildInfo("tvOS", "generic/platform=tvOS Simulator"),
    PlatformID.MACOS : BuildInfo("macOS", "generic/platform=macOS"),
    PlatformID.VISIONOS : BuildInfo("visionOS", "generic/platform=visionOS"),
    PlatformID.VISIONOS_SIMULATOR : BuildInfo("visionOS", "generic/platform=visionOS Simulator")
}

# Common context data for building the plug-ins.
class BuildContext:
    SIMULATOR_PLATFORMS = [PlatformID.IOS_SIMULATOR, PlatformID.TVOS_SIMULATOR, PlatformID.VISIONOS_SIMULATOR]
    DEVICE_PLATFORMS = [PlatformID.IOS, PlatformID.TVOS, PlatformID.MACOS, PlatformID.VISIONOS]

    def __init__(self, root_path : Path) -> None:
        # Required Paths
        self.script_root = root_path
        self.build_output_path = root_path.joinpath("Build")
        self.plugin_root = root_path.joinpath("plug-ins")
        self.test_build_root = root_path.joinpath("TestBuilds")
        self.unity_install_root = Path("/Applications/Unity")

        # Build options
        self.build_actions : dict[str, bool] = dict()
        self.clean_actions : dict[str, bool] = dict()
        self.platforms : dict[str, bool] = dict()
        self.plugins : dict[str, bool] = dict()
        self.build_tests = False
        self.build_configs : dict[str, bool] = dict()
        self.codesign_hash = ""
        
        # Output formatting
        self.printer : Printer = None

    # Helper method creates an xcodebuild command for each target platform
    # Returns as a dictionary mapping a supported platform string to a list of strings ready to pass to subprocess.run()
    def GenerateXcodeBuildCommands(self) -> dict[str, dict[str, list[str]]]:        
        build_commands = dict()
        for platform, platform_enabled in self.platforms.items():
            if platform_enabled:
                build_commands[platform] = dict()
                currBuildInfo = BUILD_INFO_TABLE[platform]
                for config, config_enabled in self.build_configs.items():
                    if config_enabled:
                        command = ["xcodebuild", "-scheme", f"{currBuildInfo.platform_root} - {config}", "-destination", f"{currBuildInfo.build_destination}", "clean", "build"]
                        build_commands[platform][config] = command
        return build_commands
