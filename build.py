#! /usr/bin/env python3
# Requirements: Xcode, Xcode Command Line tools, npm, python3
import argparse, os, pathlib, shutil, subprocess
from datetime import datetime
import scripts.upi_utility as utility
import scripts.upi_unity as unity

# ---------------------------------------
# Plug-in Identifiers (-p, --plugin-list)

# Selection identifiers for which plug-ins to perform build actions upon. Build script will ignore actions for unselected plug-ins.
plugin_id_accessibility = "Accessibility"
plugin_id_apple_core = "Core"
plugin_id_core_haptics = "CoreHaptics"
plugin_id_game_controller = "GameController"
plugin_id_game_kit = "GameKit"
plugin_id_phase = "PHASE"

# (Default)
plugin_id_all = "all"

# --------------------------------------
# Platform Identifiers (-m, --platforms)

# Selection identifiers for which platforms target when building.
platform_id_ios = "iOS"
platform_id_macos = "macOS"
platform_id_tvos = "tvOS"

# (Default)
platform_id_all = "all"

# -----------------------------------------
# Platform config identifiers (-d, --debug)

platform_config_id_debug = "Debug"

# (Default) Set implicitly when the debug argument is not passed to the build script.
platform_config_id_release = "Release"

# ----------------------------------
# Build Actions (-b, --build-action)

# Builds each selected plug-in's native frameworks and moves them to associated Unity plug-in project folder hierarchy
build_action_native_build = "build"

# Runs 'npm pack' on each selected plug-in and saves resulting package to the current output_path (See option: --output-path)
build_action_pack = "pack"

# Skips all build actions. Used when only a clean action is desired.
build_action_none = "none"

# (Default) Performs all builds actions for each selected plug-in
build_action_all = "all"

# ------------------------------------
# Clean Actions (-k, --clean-action)

# Removes native libraries and associated .meta files from within the selected plug-in Unity projects (See option: --plugin-list)
clean_action_native = "native"

# Removes packages for the selected plug-ins in the current output_path (See option: --output-path)
clean_action_packages = "packages"

# Removes all output under test_output_path (See option: --test-output-path)
clean_action_tests = "tests"

# (Default) Skips all clean actions.
clean_action_none = "none"

# Performs all clean actions for the selected plug-ins
clean_action_all = "all"

#----------------
# Configure paths

build_script_path = pathlib.Path().resolve(__file__)
default_build_path = build_script_path.joinpath("Build")
top_level_plugin_path = build_script_path.joinpath("plug-ins")
default_test_build_path = build_script_path.joinpath("TestBuilds")
apple_core_library_path_table = {
    platform_id_ios : top_level_plugin_path.joinpath("Apple.Core/Apple.Core_Unity/Assets/Apple.Core/Plugins/iOS/AppleCoreNative.framework"),
    platform_id_macos : top_level_plugin_path.joinpath("Apple.Core/Apple.Core_Unity/Assets/Apple.Core/Plugins/macOS/AppleCoreNativeMac.bundle"),
    platform_id_tvos : top_level_plugin_path.joinpath("Apple.Core/Apple.Core_Unity/Assets/Apple.Core/Plugins/tvOS/AppleCoreNative.framework"),
}

# Script will search this path for instances of Unity.app to track their versions and executable paths
default_unity_install_root_path = pathlib.Path("/Applications/Unity")

# ------------------------
# Handle command line args

argument_parser = argparse.ArgumentParser(description="Builds all native libraries, packages plug-ins, and moves packages to build folder.")
argument_parser.add_argument("-p", "--plugin-list", dest="plugin_list", nargs='*', default=[plugin_id_all], help=f"Selects the plug-ins to process. Possible values are: {plugin_id_accessibility}, {plugin_id_apple_core}, {plugin_id_core_haptics}, {plugin_id_game_controller}, {plugin_id_game_kit}, {plugin_id_phase}, or {plugin_id_all}. Default is: {plugin_id_all}")
argument_parser.add_argument("-m", "--platforms", dest="platform_list", nargs='*', default=[platform_id_all], help=f"Selects the desired platforms to target when building native libraries. Possible values are: {platform_id_ios}, {platform_id_macos}, {platform_id_tvos}, or {platform_id_all}. Default is: {platform_id_all}")
argument_parser.add_argument("-b", "--build-action", dest="build_actions", nargs='*', default=[build_action_native_build, build_action_pack], help=f"Sets the build actions for the selected plug-ins. Possible values are: {build_action_native_build}, {build_action_pack}, {build_action_none} or {build_action_all}. Defaults are: {build_action_native_build}, {build_action_pack}")
argument_parser.add_argument("-u", "--unity-installation-root", dest="unity_installation_root", default=default_unity_install_root_path, help="Root path to search for Unity installations. Note: performs a full recursive search of the given directory.")
argument_parser.add_argument("-d", "--debug", dest="debug", action="store_true", help=f"Compiles debug native libraries for the selected plug-ins.")
argument_parser.add_argument("-o", "--output-path", dest="output_path", default=default_build_path, help=f"Build result path for final packages. Default: {default_build_path}")
argument_parser.add_argument("-k", "--clean-action", dest="clean_actions", nargs='*', default=[clean_action_none], help=f"Sets the clean actions for the selected plug-ins. Possible values are: {clean_action_native}, {clean_action_packages}, {clean_action_tests}, {clean_action_none}, or {clean_action_all}. Defaults to no clean action.")
argument_parser.add_argument("-f", "--force", dest="force_clean", action="store_true", help="Setting this option will not prompt user on file deletion during clean operations.")
argument_parser.add_argument("-t", "--test", dest="build_tests", action="store_true", help="Builds Unity tests for each plug-in.")
argument_parser.add_argument("-to", "--test-output-path", dest="test_output_path", default=default_test_build_path, help=f"Output path for test build results. Default: {default_test_build_path}")

build_args = argument_parser.parse_args()

#-----------------
# Main entry point

if __name__ == '__main__':
    # Store the time of invocation for later use
    invocation_time = datetime.now()
    invocation_time_string = invocation_time.strftime("%Y-%m-%d_%H-%M-%S")

    utility.StatusMessage("***********************************"
                          "\nRunning Unity plug-in build script."
                          "\n\nCommand Line Option Summary"
                          "\n---------------------------"
                         f"\n              Build Actions: {' '.join(build_args.build_actions)}"
                         f"\n         Selected Platforms: {' '.join(build_args.platform_list)}"
                         f"\n               Build Config: {'Debug' if build_args.debug else 'Release'}"
                         f"\n        Package Output Path: {build_args.output_path}"
                         f"\n          Selected Plug-Ins: {' '.join(build_args.plugin_list)}"
                         f"\n                Build Tests: {'Yes' if build_args.build_tests else 'No'}"
                         f"\n           Test Output Path: {build_args.test_output_path}"
                         f"\n              Clean Actions: {' '.join(build_args.clean_actions)}"
                         f"\n                Force Clean: {'Yes' if build_args.force_clean else 'No'}"
                         f"\n    Unity Installation Root: {build_args.unity_installation_root}")

#region Validate input
    build_actions = {
        build_action_native_build: False,
        build_action_pack: False,
    }

    valid_build_action_found = False
    for action in build_args.build_actions:
        if action in build_actions:
            build_actions[action] = True
            valid_build_action_found = True
        elif action == build_action_all:
            for build_action_key in build_actions.keys():
                build_actions[build_action_key] = True
            valid_build_action_found = True
            break
        elif action == build_action_none:
            for build_action_key in build_actions.keys():
                build_actions[build_action_key] = False
            valid_build_action_found = True
            break
        else:
            utility.WarningMessage(f"Ignoring unknown build action '{action}'. Valid options are {build_action_native_build}, {build_action_pack}, {build_action_all} (Default), or {build_action_none}")
    
    if not valid_build_action_found:
        utility.WarningMessage(f"No valid build action passed to build script. Using default argument: {build_action_all}")
        for build_action_key in build_actions.keys():
            build_actions[build_action_key] = True
        
    selected_platforms = {
        platform_id_ios: False,
        platform_id_macos: False,
        platform_id_tvos: False,
    }

    valid_platform_found = False
    for platform_id in build_args.platform_list:
        if platform_id == platform_id_all:
            valid_platform_found = True
            for selected_platform_key in selected_platforms.keys():
                selected_platforms[selected_platform_key] = True
            break
        elif platform_id in selected_platforms:
            valid_platform_found = True  
            selected_platforms[platform_id] = True
        else:
            utility.WarningMessage(f"Ignoring unknown platform '{platform_id}'. Valid options are {platform_id_ios}, {platform_id_macos}, {platform_id_tvos}, or {platform_id_all} (Default)")

    if not valid_platform_found:
        utility.WarningMessage(f"No valid platform passed to build script. Using default argument: {platform_id_all}")
        for selected_platform_key in selected_platforms.keys():
            selected_platforms[selected_platform_key] = True
    
    selected_plugins = {
        plugin_id_accessibility: False,
        plugin_id_apple_core: False,
        plugin_id_core_haptics: False,
        plugin_id_game_controller: False,
        plugin_id_game_kit: False,
        plugin_id_phase: False
    }

    valid_plugin_found = False
    for plugin_id in build_args.plugin_list:
        if plugin_id in selected_plugins:
            selected_plugins[plugin_id] = True
            valid_plugin_found = True
        elif plugin_id == plugin_id_all:
            for selected_plugin_key in selected_plugins.keys():
                selected_plugins[selected_plugin_key] = True
            valid_plugin_found = True
            break
        else:
            utility.WarningMessage(f"Ignoring unknown plug-in '{plugin_id}'. Valid options are {plugin_id_accessibility}, {plugin_id_apple_core}, {plugin_id_core_haptics}, {plugin_id_game_controller}, {plugin_id_game_kit}, {plugin_id_phase}, or {plugin_id_all} (Default)")

    if not valid_plugin_found:
        utility.WarningMessage(f"No valid plug-in passed to build script. Using default argument: {plugin_id_all}")
        for selected_plugin_key in selected_plugins.keys():
            selected_plugins[selected_plugin_key] = True

    clean_actions = {
        clean_action_native: False,
        clean_action_packages: False,
        clean_action_tests: False
    }

    valid_clean_action_found = False
    for action in build_args.clean_actions:
        if action in clean_actions:
            clean_actions[action] = True
            valid_clean_action_found = True
        elif action == clean_action_all:
            for clean_action_key in clean_actions.keys():
                clean_actions[clean_action_key] = True
            valid_clean_action_found = True
            break
        elif action == clean_action_none:
            for clean_action_key in clean_actions.keys():
                clean_actions[clean_action_key] = False
            valid_clean_action_found = True
            break
        else:
            utility.WarningMessage(f"Ignoring unknown clean action '{action}'. Valid options are {clean_action_native}, {clean_action_packages}, {clean_action_tests}, {clean_action_all}, or {clean_action_none} (Default)")

    if not valid_clean_action_found:
        utility.WarningMessage(f"No valid clean action passed to build script. Using default argument: {clean_action_none}")
        for clean_action_key in clean_actions.keys():
            clean_actions[clean_action_key] = False
        

    build_tests = build_args.build_tests
#endregion

    # Configure build paths for packages
    build_path = pathlib.Path(build_args.output_path)
    if clean_actions[clean_action_packages] and build_path.exists():
        utility.StatusMessage(f"\nCleaning packages.\nRemoving folder at path: {build_path}")
        utility.RemoveFolder(build_path, prompt= not build_args.force_clean)

    if build_actions[build_action_native_build]:
        if not build_path.exists():
            utility.StatusMessage(f"\nBuild output path not found at: {build_path}\nCreating.")
            build_path.mkdir()

    # Configure and optionally clean paths for test builds
    test_build_root_path = pathlib.Path(build_args.test_output_path)
    if clean_actions[clean_action_tests] and test_build_root_path.exists():
        utility.StatusMessage(f"\nClean tests option '{clean_action_tests}' set.")
        utility.RemoveFolder(test_build_root_path, prompt= not build_args.force_clean)
        for curr_plugin_path in top_level_plugin_path.iterdir():
            if not curr_plugin_path.is_dir():
                continue

            # As a repo standard, all plug-in Unity projects are the name of the plug-in folder with the string '_Unity' appended
            curr_unity_project_path = curr_plugin_path.joinpath(f"{curr_plugin_path.name}_Unity")
            curr_test_player_path = curr_unity_project_path.joinpath("TestPlayers")

            if curr_unity_project_path.is_dir() and curr_test_player_path.is_dir():
                utility.RemoveFolder(curr_test_player_path, prompt= not build_args.force_clean)

    test_build_path = None
    if build_tests:
        if not test_build_root_path.exists():
            utility.StatusMessage(f"\nTest build output root not found at: {test_build_root_path}\nCreating.")
            test_build_root_path.mkdir()

        # Each set of test builds for an invocation will store output in a newly time-stamped folder
        test_build_path = test_build_root_path.joinpath(f"TestBuild_{invocation_time_string}")
        test_build_path.mkdir()

    utility.StatusMessage("\nSetting up Unity.app paths"
                          "\n--------------------------")

    # Create an interface to the UnityManager object to track Unity installations and plug-in projects
    unity_manager = unity.UnityManager(build_args.unity_installation_root)
    unity_install_count = len(unity_manager.unity_installation_table)
    if (unity_install_count < 1):
        warning_string = f"Did not find any Unity installations under root: {build_args.unity_installation_root}"
                         
        if build_actions[build_action_native_build]:
            warning_string += "\nNative libraries will be compiled, but no .meta files will be generated."

        if build_actions[build_action_pack]:
            build_actions[build_action_pack] = False
            warning_string += "\nSkipping pack."

        if build_tests:
            build_tests = False
            warning_string += "\nSkipping test build."

        utility.WarningMessage(warning_string)
        
    # Sort plug-in build order so that Apple.Core always comes first
    plugin_path_list = list()
    for curr_plugin_path in top_level_plugin_path.iterdir():
        if not curr_plugin_path.is_dir():
            continue

        if curr_plugin_path.name == "Apple.Core":
            plugin_path_list.insert(0, curr_plugin_path)
        else:
            plugin_path_list.append(curr_plugin_path)


    # Cache for restore; script will be changing wd.
    # Note: Terminating calls to ErrorMessage should restore working_dir prior to termination.
    working_dir = os.getcwd()
    for curr_plugin_path in plugin_path_list:

        # By standard, all plug-ins begin with 'Apple.'
        curr_plugin_id = curr_plugin_path.name[len("Apple."):]
        if selected_plugins[curr_plugin_id] == False:
            continue

        plugin_build_label = f"\nProcessing plug-in: {curr_plugin_id}"
        plugin_build_label_footer = "-" * (len(plugin_build_label.rstrip()) - 1)
        utility.StatusMessage(f"{plugin_build_label}\n{plugin_build_label_footer}")

        # As a standard, all plug-in Unity project folders end with the string '_Unity'
        curr_unity_project = unity_manager.TrackUnityProjectAtPath(curr_plugin_path.joinpath(f"{curr_plugin_path.name}_Unity"))
        if curr_unity_project is None:
            continue

        if clean_actions[clean_action_native]:
            if curr_unity_project.native_lib_plugin_path is not None and curr_unity_project.native_lib_plugin_path.exists():
                utility.StatusMessage("\nCleaning native libraries.")
                utility.RemoveFolder(curr_unity_project.native_lib_plugin_path, contents_only=True, prompt= not build_args.force_clean)
            else:
                utility.StatusMessage("\nNo native libraries found.")
            
#region Build Native Libraries
        if build_actions[build_action_native_build]:
            # As a standard, all plug-in native library Xcode projects are kept in /Native within the current plug-in folder.
            native_project_path = curr_plugin_path.joinpath("Native")

            if not native_project_path.exists():
                utility.StatusMessage(f"\n No native library project path exists at: {native_project_path}\nSkipping native library build step.")
                continue
                
            os.chdir(native_project_path)

            for curr_platform_key in selected_platforms.keys():
                if selected_platforms[curr_platform_key] is False:
                    continue

                build_scheme = f"{curr_platform_key} - {platform_config_id_debug if build_args.debug else platform_config_id_release}"
                build_command = f"xcodebuild -scheme \"{build_scheme}\" -destination \"generic/platform={curr_platform_key}\" build -quiet"
                utility.StatusMessage(f"\nBuilding native libraries.\nBuild command: {build_command}")

                build_command_output = subprocess.run(build_command, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True, universal_newlines=True)
                if build_command_output.returncode != 0:
                    utility.WarningMessage(f"Native library build command completed with non-zero return code.\n\nSTDOUT:\n{build_command_output.stdout}")

        
            # After native libraries have been built, the Unity project should be updated to track supported platforms
            unity_manager.UpdateSupportedPlatformsForProject(curr_unity_project)

            # The native plug-in projects are configured to copy their build results to the appropriate locations in each Unity plug-in folder
            # Changing Unity project hierarchy requires Unity to touch the project and update associated .meta files
            # Creation of new libraries also require the correct platform settings written to each .meta file.
            #     (See: plug-ins/Apple.Core/Apple.Core_Unity/Assets/Apple.Core/Editor/AppleNativePluginProcessor.cs for more detail)
            # Touching the plug-in projects after libraries have been compiled and copied will apply these updates.
            unity_installation = unity_manager.GetUnityInstallationForVersion(curr_unity_project.version)
            if unity_installation:
                unity_installation.TouchProject(curr_unity_project)
            else:
                utility.WarningMessage(f"No matching Unity installation with version [{curr_unity_project.version}] found for {curr_unity_project.name}. Unity meta files will not be updated for plug-in!")
#endregion

#region Pack
        if build_actions[build_action_pack]:
            utility.StatusMessage(f"\nPacking plug-in: {curr_unity_project.name}")

            os.chdir(curr_unity_project.path)

            # Not all Unity projects keep their package.json file in the same location, so get all the paths to any package.json under the current folder hierarchy
            # TODO: This will break if there's more than one package.json in the folder tree - with the exception of those in PackageCache, which are filtered.
            package_json_file_paths = list(curr_unity_project.path.glob('**/package.json'))

            # Ignore anything in the current project's package cache
            target_package_json_path = pathlib.Path()
            for curr_package_json_path in package_json_file_paths:
                if str(curr_package_json_path).find("PackageCache") != -1:
                    continue
                else:
                    target_package_json_path = curr_package_json_path
                    break

            # If /Demos exists in same folder, rename to Demos~ folder as needed
            curr_demo_path = target_package_json_path.parent.joinpath("Demos")
            curr_demo_meta_path = target_package_json_path.parent.joinpath("Demos.meta")
            dest_demo_path = target_package_json_path.parent.joinpath("Demos~")
            dest_demo_meta_path = target_package_json_path.parent.joinpath("../Demos.meta")

            if curr_demo_path.exists():
                subprocess.run(["mv", curr_demo_path, dest_demo_path])
                subprocess.run(["mv", curr_demo_meta_path, dest_demo_meta_path])

            pack_command = f"npm pack {target_package_json_path.parent} --pack-destination {build_path}"
            utility.StatusMessage(f"\nPacking project using: {target_package_json_path}\nPack command: {pack_command}")
            pack_command_output = subprocess.run(pack_command, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True, universal_newlines=True)
            if pack_command_output.returncode != 0:
                utility.WarningMessage(f"Pack command completed with non-zero return code.\n\nSTDOUT:\n{pack_command_output.stdout}")
            else:
                utility.StatusMessage(f"Pack command completed.")

            if dest_demo_path.exists():
                subprocess.run(["mv", dest_demo_path, curr_demo_path])
                subprocess.run(["mv", dest_demo_meta_path, curr_demo_meta_path])
#endregion

#region Build Tests
        if build_tests:
            utility.StatusMessage(f"\nBuilding Unity tests for plug-in: {curr_unity_project.name}")

            # All plug-ins are dependent upon Apple.Core, so emit an error if Apple.Core libraries are missing
            if curr_plugin_id != plugin_id_apple_core:
                for selected_platform_id in selected_platforms.keys():
                    curr_apple_core_lib_path = apple_core_library_path_table[selected_platform_id]
                    if selected_platforms[selected_platform_id] is True and not curr_apple_core_lib_path.exists():
                            os.chdir(working_dir)
                            utility.ErrorMessage(f"Missing Apple.Core library for platform {selected_platform_id} at path: {curr_apple_core_lib_path}\nApple.Core plug-in must be built (-p Core ...)", True)

            unity_installation = unity_manager.GetUnityInstallationForVersion(curr_unity_project.version)
            if unity_installation is None:
                utility.WarningMessage(f"No matching Unity installation for project version {curr_unity_project.version}. Skipping test build.")
                continue

            unity_exe = unity_installation.executable_path
            if unity_exe is None:
                utility.WarningMessage(f"Failed to find Unity executable for installation: {curr_unity_project.version}. Skipping test build.")
                continue
            
            if len(curr_unity_project.test_assemblies) < 1:
                utility.WarningMessage(f"{curr_unity_project.name}: No test assemblies found. Skipping test build.")
                continue

            if len(curr_unity_project.supported_platforms) < 1:
                utility.WarningMessage(f"{curr_unity_project.name}: No supported test platforms found. Skipping test build.")
                continue

            # Unity command line args consume the test assembly list as a single semicolon-delimited string
            curr_test_assembly_string = ';'.join(curr_unity_project.test_assemblies)

            for curr_platform in  curr_unity_project.supported_platforms.keys():
                curr_test_build_identifier = f"{curr_unity_project.name}_{curr_unity_project.version}_{curr_platform}"
                curr_test_build_path = test_build_path.joinpath(curr_test_build_identifier)
                if not curr_test_build_path.is_dir():
                    curr_test_build_path.mkdir()

                curr_unity_log_path = curr_test_build_path.joinpath(f"{curr_test_build_identifier}_build.log")

                curr_unity_build_command = f"{unity_exe} " \
                                    "-runTests " \
                                    "-batchmode " \
                                    "-forgetProjectPath " \
                                    f"-projectPath {curr_unity_project.path} " \
                                    f"-testPlatform {curr_platform} " \
                                    f"-assemblyNames {curr_test_assembly_string} " \
                                    f"-logFile {curr_unity_log_path}"

                utility.StatusMessage(f"\nBuilding {curr_platform} tests with build command:\n{curr_unity_build_command}")

                curr_unity_build_command_output = subprocess.run(curr_unity_build_command, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True, universal_newlines=True)
                if curr_unity_build_command_output.returncode != 0:
                    if len(curr_unity_build_command_output.stdout) > 0:
                        utility.WarningMessage(f"Build command completed with non-zero return code.\n\nSTDOUT:\n{curr_unity_build_command_output.stdout}")
                    else:
                        utility.WarningMessage(f"Build command completed with non-zero return code.\nUnity had no output to stdout or stderr.\nCheck Unity log for details: {curr_unity_log_path}")

                curr_temp_path = curr_unity_project.path.joinpath("TestPlayers")
                if not curr_temp_path.is_dir():
                    utility.ErrorMessage(f"No test build output found!\nExpected output path: {curr_temp_path}\nSee Unity build log: {curr_unity_log_path}")
                    continue

                subprocess.run(["cp", "-R", curr_temp_path, curr_test_build_path])
                shutil.rmtree(curr_temp_path)
#endregion

    os.chdir(working_dir)
    utility.StatusMessage("\nFinished running Unity plug-in build script.")
