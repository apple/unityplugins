#! /usr/bin/env python3
# Requirements: python3

from pathlib import Path
from scripts.upi_utility import Printer
from scripts.upi_cli_argument_options import PlatformID

SIMULATOR_PLATFORMS = [PlatformID.IOS, PlatformID.TVOS]

# Common context data for building the plug-ins.
class BuildContext:
    def __init__(self, root_path) -> None:
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
        self.simulator_build = False
        self.build_tests = False
        self.build_config = None
        self.codesign_hash = ""
        
        # Output formatting
        self.printer : Printer = None

    # Helper method creates an xcodebuild command for each target platform
    # Returns as a dictionary mapping a supported platform string to a list of strings ready to pass to subprocess.run()
    def GenerateXcodeBuildCommands(self) -> dict[str, list[str]]:
        build_commands = dict()
        for platform, enabled in self.platforms.items():
            if enabled:
                destination = f"generic/platform={platform} Simulator" if self.simulator_build and platform in SIMULATOR_PLATFORMS else f"generic/platform={platform}"

                command = ["xcodebuild", "-scheme", f"{platform} - {self.build_config}", "-destination", f"{destination}", "clean", "build"]
                build_commands[platform] = command
                
        return build_commands
