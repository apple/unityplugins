#! /usr/bin/env python3
# Requirements: Xcode, Xcode Command Line tools, npm, python3
import argparse, pathlib

import scripts.python.upi_utility as utility
import scripts.python.upi_unity_native_plugin_manager as plugin_manager
import scripts.python.upi_toolchain as toolchain

from datetime import datetime
from pathlib import Path

from scripts.python.upi_cli_argument_options import PluginID, PlatformID, ConfigID, BuildActionID, CleanActionID, CodeSignActionID
from scripts.python.upi_build_context import BuildContext
from scripts.python.upi_utility import PromptColor, Printer

# Set a script version to track evolution
build_script_version = "2.2.2"

#---------------------
# Create Build Context

# Create context and configure default paths
CTX = BuildContext(Path().resolve(__file__))

# ------------------------
# Handle command line args

argument_parser = argparse.ArgumentParser(description="Builds all native libraries, packages plug-ins, and moves packages to build folder.")
argument_parser.add_argument("-p", "--plugin-list", dest="plugin_list", nargs='*', default=[PluginID.ALL], help=f"Selects the plug-ins to process. Possible values are: {PluginID.ACCESSIBILITY}, {PluginID.CORE}, {PluginID.CORE_HAPTICS}, {PluginID.GAME_CONTROLLER}, {PluginID.GAME_KIT}, {PluginID.PHASE}, or {PluginID.ALL}. Default is: {PluginID.ALL}")
argument_parser.add_argument("-m", "--platforms", dest="platform_list", nargs='*', default=[PlatformID.ALL], help=f"Selects the desired platforms to target when building native libraries. Possible values are: {PlatformID.IOS}, {PlatformID.IOS_SIMULATOR}, {PlatformID.MACOS}, {PlatformID.TVOS}, {PlatformID.TVOS_SIMULATOR}, {PlatformID.VISIONOS}, {PlatformID.VISIONOS_SIMULATOR}, {PlatformID.SIMULATORS}, {PlatformID.DEVICES} or {PlatformID.ALL}. Default is: {PlatformID.ALL}")
argument_parser.add_argument("-b", "--build-action", dest="build_actions", nargs='*', default=[BuildActionID.BUILD, BuildActionID.PACK], help=f"Sets the build actions for the selected plug-ins. Possible values are: {BuildActionID.BUILD}, {BuildActionID.PACK}, {BuildActionID.NONE} or {BuildActionID.ALL}. Defaults are: {BuildActionID.BUILD}, {BuildActionID.PACK}")
argument_parser.add_argument("-bc","--build-config", dest="build_config", default=ConfigID.ALL, help=f"Sets the build configuration to compile. Possible values are: {ConfigID.RELEASE}, {ConfigID.DEBUG}, or {ConfigID.ALL} which builds all other configs. Default is: {ConfigID.ALL}")
argument_parser.add_argument("-c", "--codesign-identity", dest="codesign_identity", default=str(), help=f"Signs compiled native libraries with provided code signing identity hash or prompts the user to select from a list of identities on the system when {CodeSignActionID.PROMPT} is passed.")
argument_parser.add_argument("-u", "--unity-installation-root", dest="unity_installation_root", default="", help="Root path to search for Unity installations when building tests. Note: performs a full recursive search of the given directory.")
argument_parser.add_argument("-o", "--output-path", dest="output_path", default=CTX.build_output_path, help=f"Build result path for final packages. Default: {CTX.build_output_path}")
argument_parser.add_argument("-k", "--clean-action", dest="clean_actions", nargs='*', default=[CleanActionID.NONE], help=f"Sets the clean actions for the selected plug-ins. Possible values are: {CleanActionID.NATIVE}, {CleanActionID.PACKAGES}, {CleanActionID.TESTS}, {CleanActionID.NONE}, or {CleanActionID.ALL}. Defaults to no clean action.")
argument_parser.add_argument("-f", "--force", dest="force_clean", action="store_true", help="Setting this option will not prompt user on file deletion during clean operations.")
argument_parser.add_argument("-t", "--test", dest="build_tests", action="store_true", help="Builds Unity tests for each plug-in.")
argument_parser.add_argument("-to", "--test-output-path", dest="test_output_path", default=CTX.test_build_root, help=f"Output path for test build results. Default: {CTX.test_build_root}")
argument_parser.add_argument("-nc", "--no-color", dest="no_color", action="store_true", help="Use no color in the terminal output. Default: terminal output is colorized.")

build_args = argument_parser.parse_args()

# -----------------
# Prompt Formatting

prompt_theme = utility.PromptTheme()

# Set to false to disable all colors in your terminal emulator.
if build_args.no_color:
    prompt_theme_enable = False
else:
    prompt_theme_enable = True

# These colors control the colors that the script uses in your terminal emulator.
if prompt_theme_enable:
    # Control the color of standard messages
    prompt_theme.standard_output_color = PromptColor.NONE

    # Color for section headings in output
    prompt_theme.section_heading_color = PromptColor.BRIGHT_BLUE

    # Color used when the script reports status
    prompt_theme.status_color = PromptColor.GREEN

    # Color used when the script adds context, such as a file path or version number, to a message
    prompt_theme.context_color = PromptColor.MAGENTA

    # Error tags
    prompt_theme.error_bg_color = PromptColor.BG_RED
    prompt_theme.error_color = PromptColor.BRIGHT_WHITE

    # Warning tags
    prompt_theme.warning_bg_color = PromptColor.BG_BRIGHT_YELLOW
    prompt_theme.warning_color = PromptColor.BLACK

    # Info tags
    prompt_theme.info_bg_color = PromptColor.BG_BLACK
    prompt_theme.info_color = PromptColor.GREEN

    # Colors used when user input is prompted
    prompt_theme.user_input_bg_color = PromptColor.BG_BLUE
    prompt_theme.user_input_color = PromptColor.BRIGHT_WHITE

# This string represents a single level of indentation in the script output in your terminal emulator.
prompt_theme.indent_string = '  '

# Store prompt theme
CTX.printer = Printer(prompt_theme)


def Main():
    # Store the time of invocation for later use
    invocation_time = datetime.now()
    invocation_time_string = invocation_time.strftime("%Y-%m-%d_%H-%M-%S")

    print(f"\n{Printer.Bold('*'*80)}"
          f"\n\n{Printer.Bold('Unity Plug-In Build Script'):^80s}"
          f"\n\n{CTX.printer.Context(build_script_version):^80}"
          f"\n\n{Printer.Bold('*'*80)}")
    
    # Filter platform list for proxy values (device and simulator platforms)
    filtered_user_platforms = build_args.platform_list
    if (PlatformID.DEVICES in filtered_user_platforms):
        filtered_user_platforms[:] = [value for value in build_args.platform_list if value != PlatformID.DEVICES]
        filtered_user_platforms += BuildContext.DEVICE_PLATFORMS

    if (PlatformID.SIMULATORS in filtered_user_platforms):
        filtered_user_platforms[:] = [value for value in build_args.platform_list if value != PlatformID.SIMULATORS]
        filtered_user_platforms += BuildContext.SIMULATOR_PLATFORMS

    supported_platforms = toolchain.GetSupportedPlatformList()

    CTX.printer.SectionHeading("Command Line Option Summary")
    
    print(f"\n            Build Actions({Printer.Bold('-b')}): {CTX.printer.Context(' '.join(build_args.build_actions))}"
          f"\n       Selected Platforms({Printer.Bold('-m')}): {CTX.printer.Context(' '.join(filtered_user_platforms))} (Build System Support: {Printer.MultiDecorate(', '.join(supported_platforms), prompt_theme.info_bg_color, prompt_theme.info_color)})"
          f"\n            Build Config({Printer.Bold('-bc')}): {CTX.printer.Context(build_args.build_config)}"
          f"\n      Package Output Path({Printer.Bold('-o')}): {CTX.printer.Context(build_args.output_path)}"
          f"\n        Selected Plug-Ins({Printer.Bold('-p')}): {CTX.printer.Context(' '.join(build_args.plugin_list))}"
          f"\n            Clean Actions({Printer.Bold('-k')}): {CTX.printer.Context(' '.join(build_args.clean_actions))}"
          f"\n              Force Clean({Printer.Bold('-f')}): {CTX.printer.Context('Yes (-f set)' if build_args.force_clean else 'No (-f not set)')}"
          f"\n              Build Tests({Printer.Bold('-t')}): {CTX.printer.Context('Yes (-t set)' if build_args.build_tests else 'No (-t not set)')}"
          f"\n     Codesigning Identity({Printer.Bold('-c')}): {CTX.printer.Context(build_args.codesign_identity if len(build_args.codesign_identity) > 0 else 'None supplied.')}")
    
    if len(build_args.unity_installation_root) > 0:
        print(f"  Unity Installation Root({Printer.Bold('-u')}): {CTX.printer.Context(build_args.unity_installation_root)}")

    if build_args.build_tests:
        print(f"        Test Output Path({Printer.Bold('-to')}): {CTX.printer.Context(build_args.test_output_path)}")

    CTX.printer.SectionHeading("Validate Input")

    # -------------------------------------------------------------------------

    CTX.build_actions = {
        BuildActionID.BUILD: False,
        BuildActionID.PACK: False,
    }

    valid_build_action_found = False
    for action in build_args.build_actions:
        if action in CTX.build_actions:
            CTX.build_actions[action] = True
            valid_build_action_found = True
        elif action == BuildActionID.ALL:
            for build_action_key in CTX.build_actions.keys():
                CTX.build_actions[build_action_key] = True
            valid_build_action_found = True
            break
        elif action == BuildActionID.NONE:
            for build_action_key in CTX.build_actions.keys():
                CTX.build_actions[build_action_key] = False
            valid_build_action_found = True
            break
        else:
            CTX.printer.WarningMessage(f"Ignoring unknown build action '{action}'. Valid options are {BuildActionID.BUILD}, {BuildActionID.PACK}, {BuildActionID.ALL} (Default), or {BuildActionID.NONE}")
    
    if not valid_build_action_found:
        CTX.printer.WarningMessage(f"No valid build action passed to build script. Using default argument: {BuildActionID.ALL}")
        for build_action_key in CTX.build_actions.keys():
            CTX.build_actions[build_action_key] = True

    # -------------------------------------------------------------------------

    CTX.platforms = {
        PlatformID.IOS: False,
        PlatformID.IOS_SIMULATOR: False,
        PlatformID.TVOS: False,
        PlatformID.TVOS_SIMULATOR : False,
        PlatformID.MACOS: False,
        PlatformID.VISIONOS: False,
        PlatformID.VISIONOS_SIMULATOR: False
    }

    valid_platform_found = False
    for platform_id in filtered_user_platforms:
        if platform_id == PlatformID.ALL:
            CTX.printer.Message(f"Platform '{PlatformID.ALL}' selected to build for all supported platforms and overrides all other platform ({Printer.Bold('-m')}) arguments.")
            CTX.printer.InfoMessage(f"Unity lacks full support for {PlatformID.IOS_SIMULATOR} and {PlatformID.TVOS_SIMULATOR}; these platforms are skipped unless explicitly set as platform ({Printer.Bold('-m')}) arguments.")
            valid_platform_found = True
            for selected_platform_key in CTX.platforms:
                if selected_platform_key in supported_platforms:
                    if selected_platform_key != PlatformID.IOS_SIMULATOR and selected_platform_key != PlatformID.TVOS_SIMULATOR:
                        CTX.platforms[selected_platform_key] = True
            break
        elif platform_id in CTX.platforms:
            if platform_id in supported_platforms:
                valid_platform_found = True  
                CTX.platforms[platform_id] = True
            else:
                CTX.printer.WarningMessage(f"Valid platform '{platform_id}' selected, but no {platform_id} SDK is installed.\nPlease add the SDK:\n  {Printer.Bold('1.')} Open {Printer.Bold('Xcode')}\n  {Printer.Bold('2.')} Open {Printer.Bold('Settings')} (Xcode > Settings...) or (⌘ + ,) \n  {Printer.Bold('3.')} Go to the {Printer.Bold('Platforms')} tab\n  {Printer.Bold('4.')} Install the {platform_id} SDK.")
                if not utility.BooleanPrompt(CTX.printer, f"Would you like to continue building without {platform_id} support?"):
                    exit()
        else:
            CTX.printer.WarningMessage(f"Ignoring unknown platform '{platform_id}'. Valid options are {PlatformID.IOS}, {PlatformID.IOS_SIMULATOR}, {PlatformID.MACOS}, {PlatformID.TVOS}, {PlatformID.TVOS_SIMULATOR}, {PlatformID.VISIONOS}, {PlatformID.VISIONOS_SIMULATOR}, {PlatformID.SIMULATORS}, {PlatformID.DEVICES}, or {PlatformID.ALL} (default).")

    if not valid_platform_found:
        CTX.printer.WarningMessage(f"No valid platform passed to build script. Using default argument: {PlatformID.ALL}")
        for selected_platform_key in CTX.platforms:
            if selected_platform_key in supported_platforms:
                if selected_platform_key != PlatformID.IOS_SIMULATOR and selected_platform_key != PlatformID.TVOS_SIMULATOR:
                    CTX.platforms[selected_platform_key] = True

    if not CTX.platforms[PlatformID.MACOS]:
        CTX.printer.WarningMessage(f"User selected to skip building for {PlatformID.MACOS}. Play mode (the play button) in the Unity Editor will not be supported by the plug-ins.")
        CTX.platforms[PlatformID.MACOS] = utility.BooleanPrompt(CTX.printer, f"Would you like to enable build for {PlatformID.MACOS}?")

    # -------------------------------------------------------------------------
    
    CTX.build_configs = {
        ConfigID.RELEASE: False,
        ConfigID.DEBUG: False
    }

    if (build_args.build_config == ConfigID.ALL):
        CTX.build_configs[ConfigID.RELEASE] = True
        CTX.build_configs[ConfigID.DEBUG] = True
    else:
        if build_args.build_config in CTX.build_configs:
            CTX.printer.WarningMessage(f"Valid build config '{build_args.build_config}' selected, but it's recommended to use the default value of '{ConfigID.ALL}' to ensure that both Debug and Release libraries are available when building in the Editor.")
            CTX.printer.InfoMessage("This may be intentional and is valid, the Apple Unity plug-ins will fall back to use Debug or Release libraries depending upon which are available.")
            CTX.build_configs[build_args.build_config] = True
        else:
            CTX.printer.WarningMessage(f"Invalid build config \"{build_args.build_config}\" passed to build script. Using default argument: {ConfigID.ALL}")
            CTX.build_configs[ConfigID.RELEASE] = True
            CTX.build_configs[ConfigID.DEBUG] = True

    # -------------------------------------------------------------------------

    CTX.plugins = {
        PluginID.ACCESSIBILITY: False,
        PluginID.CORE: False,
        PluginID.CORE_HAPTICS: False,
        PluginID.GAME_CONTROLLER: False,
        PluginID.GAME_KIT: False,
        PluginID.PHASE: False
    }

    valid_plugin_found = False
    for plugin_id in build_args.plugin_list:
        if plugin_id in CTX.plugins:
            CTX.plugins[plugin_id] = True
            valid_plugin_found = True
        elif plugin_id == PluginID.ALL:
            for selected_plugin_key in CTX.plugins.keys():
                CTX.plugins[selected_plugin_key] = True
            valid_plugin_found = True
            break
        else:
            CTX.printer.WarningMessage(f"Ignoring unknown plug-in '{plugin_id}'. Valid options are {PluginID.ACCESSIBILITY}, {PluginID.CORE}, {PluginID.CORE_HAPTICS}, {PluginID.GAME_CONTROLLER}, {PluginID.GAME_KIT}, {PluginID.PHASE}, or {PluginID.ALL} (Default)")

    # -------------------------------------------------------------------------

    CTX.build_tests = build_args.build_tests

    # If user has opted to build tests, Apple.Core must also be selected as all plug-ins are dependent upon Apple.Core
    if CTX.build_tests and not CTX.plugins[PluginID.CORE]:
        CTX.printer.WarningMessage(f"Build Tests({Printer.Bold('-t')}) set to true, but Apple.Core has not been selected to process.")
        CTX.printer.InfoMessage("All plug-ins are dependent upon Apple.Core, so it must be built for tests build successfully.")
        CTX.printer.StatusMessage("Adding Apple.Core to selected plug-ins.", "\n")
        CTX.plugins[PluginID.CORE] = True

    if not valid_plugin_found:
        CTX.printer.WarningMessage(f"No valid plug-in passed to build script. Using default argument: {PluginID.ALL}")
        for selected_plugin_key in CTX.plugins.keys():
            CTX.plugins[selected_plugin_key] = True

    # -------------------------------------------------------------------------

    CTX.clean_actions = {
        CleanActionID.NATIVE: False,
        CleanActionID.PACKAGES: False,
        CleanActionID.TESTS: False
    }

    valid_clean_action_found = False
    for action in build_args.clean_actions:
        if action in CTX.clean_actions:
            CTX.clean_actions[action] = True
            valid_clean_action_found = True
        elif action == CleanActionID.ALL:
            for clean_action_key in CTX.clean_actions.keys():
                CTX.clean_actions[clean_action_key] = True
            valid_clean_action_found = True
            break
        elif action == CleanActionID.NONE:
            for clean_action_key in CTX.clean_actions.keys():
                CTX.clean_actions[clean_action_key] = False
            valid_clean_action_found = True
            break
        else:
            CTX.printer.WarningMessage(f"Ignoring unknown clean action '{action}'. Valid options are {CleanActionID.NATIVE}, {CleanActionID.PACKAGES}, {CleanActionID.TESTS}, {CleanActionID.ALL}, or {CleanActionID.NONE} (Default)")

    if not valid_clean_action_found:
        CTX.printer.WarningMessage(f"No valid clean action passed to build script. Using default argument: {CleanActionID.NONE}")
        for clean_action_key in CTX.clean_actions.keys():
            CTX.clean_actions[clean_action_key] = False

    # -------------------------------------------------------------------------

    if build_args.unity_installation_root != "" and CTX.build_tests == False:
        CTX.printer.WarningMessage("Unity installation root provided, but no tests being built. Argument ignored.")
    else:
        unity_install_root = Path(build_args.unity_installation_root)
        if unity_install_root != "" and unity_install_root.is_dir():
            CTX.unity_install_root = unity_install_root

    # -------------------------------------------------------------------------

    CTX.printer.SectionHeading("Configure Build Paths")

    # Configure build paths for packages
    CTX.build_path = pathlib.Path(build_args.output_path)

    if CTX.clean_actions[CleanActionID.PACKAGES] and CTX.build_path.exists():
        CTX.printer.StatusMessage("Cleaning packages.", "\n")
        CTX.printer.StatusMessageWithContext("Removing folder at path:",  f"{CTX.build_path}")
        utility.RemoveFolder(CTX.build_path, prompt= not build_args.force_clean, printer= CTX.printer)

    if CTX.build_actions[BuildActionID.BUILD] or CTX.build_actions[BuildActionID.PACK]:
        if not CTX.build_path.exists():
            CTX.printer.Message(f"Build output path not found.", "\n")
            CTX.printer.StatusMessageWithContext("Creating: ", f"{CTX.build_path}")
            CTX.build_path.mkdir()

    # Configure and optionally clean paths for test builds
    test_build_root_path = pathlib.Path(build_args.test_output_path)
    if CTX.clean_actions[CleanActionID.TESTS] and test_build_root_path.exists():
        CTX.printer.StatusMessage(f"Clean tests option '{CleanActionID.TESTS}' set.", "\n")
        utility.RemoveFolder(test_build_root_path, prompt= not build_args.force_clean, printer= CTX.printer)
        for curr_plugin_path in CTX.plugin_root.iterdir():
            if not curr_plugin_path.is_dir():
                continue

            # As a standard, all plug-in Unity projects are the name of the plug-in folder with the string '_Unity' appended
            curr_unity_project_path = curr_plugin_path.joinpath(f"{curr_plugin_path.name}_Unity")
            curr_test_player_path = curr_unity_project_path.joinpath("TestPlayers")

            if curr_unity_project_path.is_dir() and curr_test_player_path.is_dir():
                utility.RemoveFolder(curr_test_player_path, prompt= not build_args.force_clean, printer= CTX.printer)

    if CTX.build_tests:
        if not CTX.test_build_root.exists():
            CTX.printer.StatusMessage("Test build output root not found.", "\n")
            CTX.printer.StatusMessageWithContext("Creating: ", f"{CTX.test_build_root}")
            test_build_root_path.mkdir()

        # Each set of test builds for an invocation will store output in a newly time-stamped folder
        CTX.test_build_output_path = CTX.test_build_root.joinpath(f"TestBuild_{invocation_time_string}")
        CTX.test_build_output_path.mkdir()

    # -------------------------------------------------------------------------

    if CTX.build_actions[BuildActionID.BUILD]:
        CTX.printer.SectionHeading("Configure Native Library Build Options")

        xcode_version, xcode_build_number = toolchain.GetToolchainVersions()
        CTX.printer.MessageWithContext("Native library build using: ", f"Xcode {xcode_version} ({xcode_build_number})", "\n")
        CTX.printer.InfoMessage(f"If this is incorrect, please update your environment with {Printer.Bold('xcode-select')}. (Call \'{Printer.Bold('xcode-select -h')}\' from the command line for more info.)")

        if len(build_args.codesign_identity) > 0:
            if build_args.codesign_identity == CodeSignActionID.PROMPT:
                CTX.codesign_hash = toolchain.PromptForCodesignIdentity(CTX.printer)
            else:
                CTX.codesign_hash = build_args.codesign_identity

        CTX.printer.SectionHeading("Gather Unity Installation Info")

        unity_plugin_manager = plugin_manager.NativeUnityPluginManager(CTX)
        if (CTX.build_tests):
            unity_plugin_manager.ScanForUnityInstallations()

        CTX.printer.SectionHeading("Process Plug-Ins")

        # Sort plug-in build order so that Apple.Core always comes first
        plugin_path_list = list()
        for curr_plugin_path in CTX.plugin_root.iterdir():
            if not curr_plugin_path.is_dir():
                continue

            if curr_plugin_path.name == "Apple.Core":
                plugin_path_list.insert(0, curr_plugin_path)
            else:
                plugin_path_list.append(curr_plugin_path)

        for plugin_path in plugin_path_list:
            unity_plugin_manager.ProcessNativeUnityPlugin(plugin_path)

    if CTX.build_tests:
        CTX.printer.SectionHeading("Build Unity Tests")
        unity_plugin_manager.BuildTests()

    if CTX.build_actions[BuildActionID.PACK]:
        CTX.printer.SectionHeading("Create Plug-In Packages")
        unity_plugin_manager.GeneratePlugInPackages()

    CTX.printer.Message("Finished running Unity plug-in build script.", "\n")

# Entry point
if __name__ == '__main__':
    Main()
