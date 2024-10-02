#! /usr/bin/env python3
# Requirements: python3

import os, shutil, json

import scripts.python.upi_utility as utility
import scripts.python.upi_toolchain as toolchain

from scripts.python.upi_cli_argument_options import ConfigID

from pathlib import Path
from collections.abc import Callable

from scripts.python.upi_build_context import BuildContext
from scripts.python.upi_utility import Printer

CTX : BuildContext = None

# Unity tracks Sim/Device separately from BuildTarget
class UnitySdkVariantID:
    DEVICE = "Device"
    SIMULATOR = "Simulator"

# Represents an individual Unity project at a given path. This path is equivalent to the path that would be opened by the Unity Editor.
class UnityProject:
    unknown_unity_project_version_string = "Unknown"

    def __init__(self) -> None:
        self.path : Path = None
        self.version : str = self.unknown_unity_project_version_string
        self.native_library_path : Path = None
        self.supported_platforms : dict[str, dict[str, str]] = dict() # {UnityPlatformString:{SDK_Variant: LibraryPath}, ...}
        self.test_assemblies : list[str] = list()
        self.editor_test_assemblies : list[str] = list()

# Tracks important paths and version information for a given Unity installation
class UnityInstallation:
    def __init__(self, app_path : Path, exe_path : Path, unity_version : str) -> None:
        self.app_path = app_path
        self.executable_path = exe_path
        self.version = unity_version

    # This method uses this Unity installation to open a passed project in batch mode and immediately close in order to do the following:
    #   - Create/Update .meta files within the project
    #   - If the project version doesn't match this installation's version, opening will attempt to update the project to match the installation's version
    def TouchProject(self, unity_project : UnityProject, logWithContext : Callable[[str, str], None] = lambda m, c: CTX.printer.MessageWithContext(m, c)) -> bool:
        unity_command = [f"{self.executable_path}", "-batchmode", "-nographics", "-projectPath", f"{unity_project.path}", "-quit"]
        
        logWithContext(f"Unity project path: ", f"{unity_project.path}")
        logWithContext(f"Unity touch command: ", f"{' '.join(unity_command)}")
        
        command_output = utility.RunCommand(unity_command)
        
        if command_output.returncode != 0:
            CTX.printer.WarningMessage(f"Updating Unity project completed with non-zero return code.\n\nSTDOUT:\n{command_output.stdout}")
            return False
        
        return True

# Simple object which contains both the Unity project reprensetation along with the path to the native Xcode project.
class NativeUnityPlugin:
    def __init__(self, root_path : Path, native_project_path : Path) -> None:
        self.plugin_root_path = root_path
        self.native_project_path = native_project_path
        self.unity_project = UnityProject()

# Native Unity plug-in manager class maintains collections of Unity.app installations and relevant information for each native plug-in.
class NativeUnityPluginManager:
    def __init__(self, build_context : BuildContext) -> None:
        self.unity_installation_table : dict[str, UnityInstallation] = dict()
        self.native_unity_plugin_table : dict[str, NativeUnityPlugin] = dict()

        global CTX
        CTX = build_context
    
    # Find and track all Unity.app installations found under 'unity_installation_root'
    # Note: Ensure this is called on a sensible root folder as recursively searching large folder hierarchies can be slow
    def ScanForUnityInstallations(self) -> None:
        CTX.printer.StatusMessageWithContext("Scanning for Unity installations under path: ", f"{CTX.unity_install_root}", "\n")

        app_paths = list(CTX.unity_install_root.glob('**/Unity.app'))
        if len(app_paths) < 1:
            CTX.printer.WarningMessage("No Unity installations found. Consider updating the Unity installation root path.")
            return

        for curr_app_path in app_paths:
            CTX.printer.StatusMessageWithContext("Inspecting Unity.app at path: ", curr_app_path, "\n")

            exe_path = curr_app_path.joinpath("Contents/MacOS/Unity")
            if exe_path.exists():
                unity_version_cli_output = utility.RunCommand([exe_path, "-version"])
                unity_version_string = unity_version_cli_output.stdout.rstrip("\n")
                if unity_version_string in self.unity_installation_table:
                    CTX.printer.MessageWithContext("Already tracking Unity.app with version: ", unity_version_string, f"\n{CTX.printer.Indent(1)}")
                    CTX.printer.MessageWithContext("Unity.app Path: ", f"{self.unity_installation_table[unity_version_string].app_path}", f"\n{CTX.printer.Indent(1)}")
                else:
                    self.unity_installation_table[unity_version_string] = UnityInstallation(curr_app_path, exe_path, unity_version_string)
                    CTX.printer.MessageWithContext("Tracked Unity.app installation:")
                    CTX.printer.MessageWithContext("Version: ", unity_version_string, CTX.printer.Indent(1))
                    CTX.printer.MessageWithContext("Unity.app Path: ", curr_app_path, CTX.printer.Indent(1))
                    CTX.printer.MessageWithContext("Executable Path: ", exe_path, CTX.printer.Indent(1))
            else:
                CTX.printer.WarningMessage(f"Could not locate executable for Unity.app at {exe_path}")

    # Returns a list of tracked Unity installation versions
    def GetUnityInstallationList(self) -> list[str]:
        return list(self.unity_installation_table.keys())

    # Returns the tracked UnityInstallation object for the provided 'version_string'
    # Returns 'None' if no installations can be found with the provided version string
    def GetUnityInstallation(self, version : str) -> UnityInstallation:
        return self.unity_installation_table[version] if version in self.unity_installation_table else None
        
    def GetNativeUnityPlugin(self, plugin_id : str) -> NativeUnityPlugin:
        return self.native_unity_plugin_table[plugin_id] if plugin_id in self.native_unity_plugin_table else None

    # Scans the provided plug-in path, optionally builds native libraries for each plug-in, and tracks relevant information for the plug-in's Unity and Xcode projects.
    def ProcessNativeUnityPlugin(self, plugin_path : Path) -> None:
        CTX.printer.StatusMessageWithContext("Scanning native plug-in subfolder: ", plugin_path.name, "\n")
        CTX.printer.MessageWithContext("Plug-in path: ", plugin_path, f"{CTX.printer.Indent(1)}")

        # Get plug-in id, which will also be used as a key for future look-up
        # By standard, all plug-ins begin with 'Apple.'
        plugin_id = plugin_path.name[len("Apple."):]
        if plugin_id in self.native_unity_plugin_table:
            CTX.printer.StatusMessageWithContext("Already tracking plug-in at path", plugin_path)
            return
        
        # Skip if not needed
        if not CTX.plugins[plugin_id]:
            CTX.printer.MessageWithContext("User omitted from list of plug-ins. Skipping: ", plugin_id, CTX.printer.Indent(1))
            return

        # As a standard, all plug-in Unity project folders are the containing folder name with the string '_Unity' appended.
        #   Example: The Apple.Core plug-in Unity project is assumed to be at the path '/plugin_path/Apple.Core_Unity/'
        unity_project_path = plugin_path.joinpath(f"{plugin_path.name}_Unity")
        if not unity_project_path.is_dir():
            CTX.printer.ErrorMessage(f"Failed to locate expected Unity project folder at path: {unity_project_path}")
            return

        # As a standard, all native library Xcode projects are in the /Native subfolder in each plug-in folder.
        native_project_path = plugin_path.joinpath("Native")
        if not native_project_path.is_dir():
            CTX.printer.ErrorMessage(f"Failed to locate expected native Xcode project folder at path: {native_project_path}")
            return

        native_plugin = NativeUnityPlugin(plugin_path, native_project_path)
        native_plugin.unity_project.path = unity_project_path

        # Determine project's associated Unity version
        project_version_file_path = unity_project_path.joinpath("ProjectSettings/ProjectVersion.txt")
        if project_version_file_path.exists():
            project_version_file_contents = project_version_file_path.read_text().split()
            try:
                version_string_index = project_version_file_contents.index("m_EditorVersion:") + 1
                if len(project_version_file_contents) <= version_string_index:
                    CTX.printer.WarningMessage(f"Couldn't find editor version string in {project_version_file_path}")
                    native_plugin.unity_project.version = UnityProject.unknown_unity_project_version_string
                else:
                    native_plugin.unity_project.version = project_version_file_contents[version_string_index]
                    CTX.printer.MessageWithContext("Project version: ", native_plugin.unity_project.version, CTX.printer.Indent(1))
            except:
                CTX.printer.WarningMessage(f"Couldn't find editor version string in {project_version_file_path}")
                native_plugin.unity_project.version = self.UnityProject.unknown_unity_project_version_string
        else:
            CTX.printer.ErrorMessage(f"Couldn't find file at path: {project_version_file_path}")
            CTX.printer.InfoMessage("Unity project must contain '/ProjectSettings/ProjectVersion.txt' to be considered a Unity project.")
            return

        # Find project test assemblies
        unity_tests_paths = list(unity_project_path.joinpath("Assets").glob('**/Tests'))
        if len(unity_tests_paths) < 1:
            CTX.printer.WarningMessage(f"{plugin_id} appears to have no supported tests. Cannot find a 'Tests' folder under /Assets.")

        for tests_path in unity_tests_paths:
            test_assembly_paths = list(tests_path.glob('**/*.asmdef'))
            for test_assembly_path in test_assembly_paths:
                if list(test_assembly_path.parts).count('Editor') > 0:
                    CTX.printer.MessageWithContext("Editor test assembly: ", test_assembly_path.stem, CTX.printer.Indent(1))
                    native_plugin.unity_project.editor_test_assemblies.append(test_assembly_path.stem)
                else:
                    CTX.printer.MessageWithContext("Test assembly: ", test_assembly_path.stem, CTX.printer.Indent(1))
                    native_plugin.unity_project.test_assemblies.append(test_assembly_path.stem)

        # cache working dir and cd shell to native project folder; xcodebuild must be invoked from within folder containing the .xcodeproj
        working_dir = os.getcwd()
        os.chdir(native_project_path)

        # Build
        # TODO: (Jared) Interrogate build machine for SDKs
        build_commands = CTX.GenerateXcodeBuildCommands()

        for platform, command_set in build_commands.items():
            for config, command in command_set.items():
                CTX.printer.StatusMessageWithContext(f"Building {config} {plugin_id} native libraries for platform: ", platform, f"\n{CTX.printer.Indent(1)}")
                CTX.printer.MessageWithContext("Build command: ", f"{' '.join(command)}", CTX.printer.Indent(2))

                build_command_output = utility.RunCommand(command)
                if build_command_output.returncode != 0:
                    CTX.printer.WarningMessage("Native library build command completed with non-zero return code")
                    CTX.printer.MessageWithContext("Command output:", f"\n{build_command_output.stdout}")
                    
                    if not utility.BooleanPrompt(CTX.printer, "Would you like to continue the build process?"):
                        os.chdir(working_dir)
                        return

        # Determine path to /NativeLibraries~, Each project should write all built libraries to this folder.
        unity_plugins_paths = list(native_plugin.unity_project.path.joinpath("Assets").glob('**/NativeLibraries~'))

        if len(unity_plugins_paths) < 1:
            CTX.printer.ErrorMessage(f"Cannot locate the \"/NativeLibraries~\" folder for {plugin_id}.\nThis may be due to a build failure. Skipping remaining steps.")
            return
        else:
            if len(unity_plugins_paths) > 1:
                CTX.printer.ErrorMessage(f"{plugin_id} has multiple 'NativeLibraries~' folders under /Assets.\nPlease remove both and try building again.\nSkipping remaining steps.")
                return

            native_plugin.unity_project.native_library_path = unity_plugins_paths[0]

        # Build completed -> cd shell to working_dir
        os.chdir(working_dir)

        # Determine supported Unity platforms (see: https://docs.unity3d.com/ScriptReference/BuildTarget.html for relevant Apple platform target names)
        CTX.printer.StatusMessage("Scanning for supported platforms.", f"\n{CTX.printer.Indent(1)}")
        unity_platform_name_table = {"iOS":("iOS", UnitySdkVariantID.DEVICE),
                                     "iPhoneSimulator":("iOS", UnitySdkVariantID.SIMULATOR),
                                     "tvOS":("tvOS", UnitySdkVariantID.DEVICE),
                                     "AppleTVSimulator":("tvOS",UnitySdkVariantID.SIMULATOR),
                                     "macOS":("StandaloneOSX", UnitySdkVariantID.DEVICE),
                                     "visionOS":("VisionOS", UnitySdkVariantID.DEVICE),
                                     "VisionSimulator":("VisionOS", UnitySdkVariantID.SIMULATOR)}
        
        native_plugin.unity_project.supported_platforms.clear()
        
        for build_config_path in native_plugin.unity_project.native_library_path.iterdir():
            if build_config_path.is_dir() and build_config_path.name in [ConfigID.RELEASE, ConfigID.DEBUG]:
                for native_platform_path in build_config_path.iterdir():
                    if native_platform_path.is_dir() and native_platform_path.name in unity_platform_name_table:
                        unity_platform_name, unity_platform_variant = unity_platform_name_table[native_platform_path.name]
                        
                        if unity_platform_name not in native_plugin.unity_project.supported_platforms.keys():
                            native_plugin.unity_project.supported_platforms[unity_platform_name] = dict()

                        native_plugin.unity_project.supported_platforms[unity_platform_name][unity_platform_variant] = native_platform_path

                        CTX.printer.MessageWithContext("Found supported Unity platform: ", unity_platform_name, CTX.printer.Indent(2))
                        CTX.printer.MessageWithContext("              Platform variant: ", unity_platform_variant, CTX.printer.Indent(2))
                        CTX.printer.MessageWithContext("                  Build Config: ", build_config_path.name, CTX.printer.Indent(2))
                        CTX.printer.MessageWithContext("Platform path: ", native_platform_path, CTX.printer.Indent(3))

                        if len(CTX.codesign_hash) > 0:
                            CTX.printer.StatusMessageWithContext("Attempting to sign native library with identity: ", CTX.codesign_hash, f"{CTX.printer.Indent(3)}")
                            for item in native_platform_path.iterdir():
                                if item.suffix == '.bundle' or item.suffix == '.framework':
                                    if len(CTX.codesign_hash) > 0:
                                        toolchain.Codesign(CTX.printer, item, CTX.codesign_hash, logWithContext= lambda m, c: CTX.printer.MessageWithContext(m, c, CTX.printer.Indent(4)))
                                elif item.suffix == '.a':
                                    CTX.printer.MessageWithContext("Skipping static library: ", f"{item}", CTX.printer.Indent(4))
                        else:
                            CTX.printer.Message("User chose to skip codesign.", CTX.printer.Indent(3))

                        Printer.Newline()
                    else:
                        CTX.printer.WarningMessage(f"Unknown platform {native_platform_path.name} found in Plugins folder at {native_platform_path.parent}\n")
            else:
                # Don't warn about files - only considering folders
                if build_config_path.is_dir():
                    CTX.printer.WarningMessage(f"Unknown platform variant {build_config_path.name} found in Plugins folder at {build_config_path.parent}")

        if len(native_plugin.unity_project.supported_platforms) < 1:
            CTX.printer.WarningMessage(f"No supported platforms found in: {Printer.Decorate(f'{native_plugin.unity_project.native_library_path}', CTX.printer.theme.context_color)}")
            CTX.printer.WarningMessage(f"Subsequent processing steps for {Printer.Decorate(f'{plugin_id}', CTX.printer.theme.context_color)} will be skipped.", "")
            Printer.Newline()
        else:
            self.native_unity_plugin_table[plugin_id] = native_plugin

        CTX.printer.StatusMessage("Completed supported platform scan.", f"{CTX.printer.Indent(1)}")

    # Build tests for each plug-in
    def BuildTests(self) -> None:
        self.ValidateProjectVersions()

        for plugin_id, native_plugin in self.native_unity_plugin_table.items():
            CTX.printer.StatusMessageWithContext(f"\nBuilding Unity tests for plug-in: ", f"{plugin_id}", CTX.printer.Indent(1))

            unity_installation =  self.GetUnityInstallation(native_plugin.unity_project.version)
            if unity_installation is None:
                CTX.printer.WarningMessage(f"No matching Unity installation for project version {native_plugin.unity_project.version}. Skipping test build.")

            unity_exe = unity_installation.executable_path
            if unity_exe is None:
                CTX.printer.WarningMessage(f"Failed to find Unity executable for installation: {native_plugin.unity_project.version}. Skipping test build.")
            
            if len(native_plugin.unity_project.test_assemblies) < 1:
                CTX.printer.WarningMessage(f"{plugin_id}: No test assemblies found. Skipping test build.")

            if len(native_plugin.unity_project.supported_platforms) < 1:
                CTX.printer.WarningMessage(f"{plugin_id}: No supported test platforms found. Skipping test build.")

            # Unity command line args consume the test assembly list as a single semicolon-delimited string
            curr_test_assembly_string = ';'.join(native_plugin.unity_project.test_assemblies)

            for curr_platform, supported_variants in  native_plugin.unity_project.supported_platforms.items():
                for curr_variant in supported_variants.keys():
                    curr_test_build_identifier = f"{plugin_id}_{native_plugin.unity_project.version}_{curr_platform}_{curr_variant}"
                    curr_test_build_path = CTX.test_build_output_path.joinpath(curr_test_build_identifier)
                    if not curr_test_build_path.is_dir():
                        curr_test_build_path.mkdir()

                    curr_unity_log_path = curr_test_build_path.joinpath(f"{curr_test_build_identifier}_build.log")

                    curr_unity_build_command = [f"{unity_exe}",
                                                "-runTests",
                                                "-batchmode",
                                                "-forgetProjectPath",
                                                f"-projectPath {native_plugin.unity_project.path}",
                                                f"-testPlatform {curr_platform}",
                                                f"-assemblyNames {curr_test_assembly_string}",
                                                f"-logFile {curr_unity_log_path}"]

                    CTX.printer.StatusMessage(f"Building {curr_platform}_{curr_variant} tests.", f"\n{CTX.printer.Indent(2)}")
                    CTX.printer.MessageWithContext("Build command: ", f"{' '.join(curr_unity_build_command)}", CTX.printer.Indent(3))

                    curr_unity_build_command_output = utility.RunCommand(curr_unity_build_command)
                    if curr_unity_build_command_output.returncode != 0:
                        if len(curr_unity_build_command_output.stdout) > 0:
                            CTX.printer.WarningMessage(f"Build command completed with non-zero return code.\n\nSTDOUT:\n{curr_unity_build_command_output.stdout}")
                        else:
                            CTX.printer.WarningMessage(f"Build command completed with non-zero return code.\nUnity had no output to stdout or stderr.\nCheck Unity log for details: {curr_unity_log_path}")

                    curr_temp_path = native_plugin.unity_project.path.joinpath("TestPlayers")
                    if not curr_temp_path.is_dir():
                        CTX.printer.ErrorMessage(f"No test build output found!")
                        CTX.printer.MessageWithContext("Expected output path: ", f"{curr_temp_path}")
                        CTX.printer.MessageWithContext("See Unity build log: ", f"{curr_unity_log_path}")
                        continue

                    CTX.printer.RunCommand(["cp", "-R", curr_temp_path, curr_test_build_path])
                    shutil.rmtree(curr_temp_path)

    # Validates that a matching Unity installation has been found for each of the processed plug-ins.
    # Returns a dictionary mapping a plug-in identifier to a UnityProject for each plug-in where no matching installation of Unity was found.
    def ValidateProjectVersions(self):
        supported_plugins = dict()
        unsupported_plugins = dict()

        for plugin_id, plugin in self.native_unity_plugin_table.items():
            if plugin.unity_project.version in self.unity_installation_table:
                CTX.printer.MessageWithContext("Found supported Unity installation for plug-in: ", plugin_id, CTX.printer.Indent(1))
                supported_plugins[plugin_id] = (self.native_unity_plugin_table[plugin_id])
            else:
                CTX.printer.MessageWithContext(f"Missing supported Unity installation for {plugin_id}: ", plugin.unity_project.version, CTX.printer.Indent(1))
                unsupported_plugins[plugin_id] = (self.native_unity_plugin_table[plugin_id])
        
        # Touch each plug-in's Unity project with the appropriate Unity Editor version to update .meta files for newly compiled native libraries.
        CTX.printer.StatusMessage("Touching Unity plug-in projects:", f"\n{CTX.printer.Indent(1)}")
        for target_plugin_id, target_native_plugin in supported_plugins.items():
            target_unity_version = self.GetUnityInstallation(target_native_plugin.unity_project.version)
            CTX.printer.MessageWithContext("Plug-in: ", target_plugin_id, CTX.printer.Indent(2))
            target_unity_version.TouchProject(target_native_plugin.unity_project, lambda m, c: CTX.printer.MessageWithContext(m, c, CTX.printer.Indent(2)))
            
            Printer.Newline()

        # Optionally upgrade plug-in Unity projects for which no matching Unity Editor installation was located.
        unsupported_plugin_count = len(unsupported_plugins)
        if unsupported_plugin_count > 0:
            CTX.printer.Message(f"Found {unsupported_plugin_count} plug-in(s) with no corresponding Unity installation.", CTX.printer.Indent(2))
            CTX.printer.InfoMessage(f"\n{CTX.printer.Indent(1)}This script can attempt to upgrade plug-in projects with an existing Unity installation."
                                    f"\n{CTX.printer.Indent(2)}* If this operation fails, it may require manually opening the associated project within Unity or reverting your local repository."
                                    f"\n{CTX.printer.Indent(2)}* If you do not upgrade with this script, build may succeed but .meta files will not be generated and the plug-ins {Printer.Bold('*WILL NOT WORK*')}"
                                    f"\n{CTX.printer.Indent(2)}* Skipping this step means that you will either need to install a matching version of Unity or manually upgrade desired plug-in projects.", "\n")
            
            if utility.BooleanPrompt(CTX.printer, "Would you like the script to attempt project upgrade?"):
                installed_unity_versions = self.GetUnityInstallationList()
                target_unity_version = None
                
                if len(installed_unity_versions) != 0:
                    if len(installed_unity_versions) == 1:
                        target_unity_version = installed_unity_versions[0]
                        CTX.printer.MessageWithContext("Found one Unity installation, version: ", target_unity_version)
                    else:
                        target_unity_version = utility.SelectionPrompt(CTX.printer, "Please select the version of Unity to use for upgrade:", installed_unity_versions)
                    
                    CTX.printer.StatusMessageWithContext("Attempting to automatically upgrade the following plug-in projects: ", f"{' '.join(unsupported_plugins.keys())}", f"\n{CTX.printer.Indent(2)}")
                    CTX.printer.MessageWithContext("Upgrade with Unity version: ", target_unity_version, CTX.printer.Indent(3))
                    
                    target_unity_installation = self.GetUnityInstallation(target_unity_version)

                    for target_plugin_id, target_native_plugin in unsupported_plugins.items():
                        CTX.printer.StatusMessageWithContext("Upgrading: ", target_plugin_id, f"\n{CTX.printer.Indent(3)}")
                        target_unity_installation.TouchProject(target_native_plugin.unity_project, lambda m, c: CTX.printer.MessageWithContext(m, c, CTX.printer.Indent(4)))
                else:
                    CTX.printer.ErrorMessage("No Unity installations are being tracked. Please check your Unity installation root path or install the Unity Editor.")
    
    # Packs plug-ins with npm and moves the resulting package to the currently configured build output folder.
    def GeneratePlugInPackages(self) -> None:
        # Cache to return; npm should be invoked from the folder containing the associated package.json
        working_dir = os.getcwd()
        for  plugin_id, native_plugin in self.native_unity_plugin_table.items():
            CTX.printer.StatusMessageWithContext("Packing plug-in: ", f"{plugin_id}", "\n")
            
            os.chdir(native_plugin.unity_project.path)

            # Not all Unity projects keep their package.json file in the same location, so get all the paths to any package.json under the current folder hierarchy
            # TODO: This will break if there's more than one package.json in the folder tree - with the exception of those in PackageCache, which are filtered.
            package_json_file_paths = list(native_plugin.unity_project.path.glob('**/package.json'))

            # Ignore anything in the current project's package cache
            target_package_json_path = Path()
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
                utility.RunCommand(["mv", curr_demo_path, dest_demo_path])
                utility.RunCommand(["mv", curr_demo_meta_path, dest_demo_meta_path])

            # get the package name and version
            package_json_file = open(target_package_json_path)
            package_json_data = json.load(package_json_file)
            tgz_filename = f"{package_json_data['name']}" "-" f"{package_json_data['version']}" ".tgz"
            package_json_file.close()

            # using npm:
            # pack_command = ["npm", "pack", f"{target_package_json_path.parent}", "--pack-destination", f"{CTX.build_output_path}"]

            # using tar:
            pack_command = ["tar", "--auto-compress", "--create", "--file", f"{CTX.build_output_path.joinpath(tgz_filename)}", "--directory", f"{target_package_json_path.parent}", "-s", "/./package/", "." ]

            CTX.printer.MessageWithContext("Project package.json path: ", f"{target_package_json_path}", CTX.printer.Indent(1))
            CTX.printer.MessageWithContext("Pack command: ", f"{(' '.join(pack_command))}", CTX.printer.Indent(1))
            
            pack_command_output = utility.RunCommand(pack_command)
            
            if pack_command_output.returncode != 0:
                CTX.printer.WarningMessage(f"Pack command completed with non-zero return code.\n\nSTDOUT:\n{pack_command_output.stdout}")
            else:
                CTX.printer.StatusMessage(f"Pack completed.")

            if dest_demo_path.exists():
                utility.RunCommand(["mv", dest_demo_path, curr_demo_path])
                utility.RunCommand(["mv", dest_demo_meta_path, curr_demo_meta_path])

        os.chdir(working_dir)
