#! /usr/bin/env python3
# Methods for interacting with local Unity installations
# Requirements: Unity, python3
from pathlib import Path
import subprocess
import scripts.upi_utility as utility

# Unity Manager class maintains collections of Unity.app installations and Unity projects
class UnityManager:        
    class UnityInstallation:
        def __init__(self, app_path, exe_path, unity_version):
            self.app_path = app_path
            self.executable_path = exe_path
            self.unity_version = unity_version

        # Updates Unity .meta files for the passed UnityProject
        # Returns False if installation version doesn't align with project version
        def TouchProject(self, unity_project):
            if unity_project.version != self.unity_version:
                return False

            unity_command = f"{self.executable_path} -batchmode -nographics -projectPath {unity_project.path} -quit"
            utility.StatusMessage(f"\nTouching Unity project at path: {unity_project.path}\nTouch Command: {unity_command}")
            command_output = subprocess.run(unity_command, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True, universal_newlines=True)
            if command_output.returncode != 0:
                if len(command_output.stdout) > 0:
                    utility.WarningMessage(f"Updating Unity project completed with non-zero return code.\n\nSTDOUT:\n{command_output.stdout}")
                else:
                    utility.WarningMessage(f"Updating Unity project completed with non-zero return code.")
            return True
            
    # Keyed by version
    unity_installation_table = dict()

    class UnityProject:
        unknown_unity_project_version_string = "Unknown"

        def __init__(self):
            self.name = ""
            self.path = ""
            self.native_lib_plugin_path = None
            self.version = self.unknown_unity_project_version_string
            self.supported_platforms = dict() # {PlatformString:LibraryPath, ...}
            self.test_assemblies = list()
            self.editor_test_assemblies = list()

    # Keyed by path
    unity_project_list = list()

    # Init will find and track all Unity.app installations found under 'unity_installation_root'
    # Note: Ensure this is called on a sensible root folder as recursively searching large folder hierarchies can be slow
    def __init__(self, unity_installation_root):
        utility.StatusMessage(f"\nScanning for Unity installations under path: {unity_installation_root}")

        app_paths = list(Path(unity_installation_root).glob('**/Unity.app'))
        if len(app_paths) < 1:
            utility.WarningMessage("No Unity installations found.")
            return

        for curr_app_path in app_paths:
            utility.StatusMessage(f"\nInspecting Unity.app at path: {curr_app_path}")

            exe_path = curr_app_path.joinpath("Contents/MacOS/Unity")
            if exe_path.exists():
                unity_version_cli_output = subprocess.run([exe_path, "-version"], capture_output=True)
                unity_version_string = unity_version_cli_output.stdout.decode("utf-8").rstrip("\n")
                if unity_version_string in self.unity_installation_table:
                    utility.StatusMessage(f"\nUnity {unity_version_string} already tracked.\n    Unity.app Path: {self.unity_installation_table[unity_version_string].app_path}")
                else:
                    self.unity_installation_table[unity_version_string] = self.UnityInstallation(curr_app_path, exe_path, unity_version_string)
                    utility.StatusMessage("Tracking new Unity.app installation:"
                        f"\n           Version: {unity_version_string}"
                        f"\n    Unity.app Path: {curr_app_path}"
                        f"\n   Executable Path: {exe_path}")
            else:
                utility.WarningMessage(f"Could not locate executable for Unity.app at {exe_path}")

    # Returns the tracked UnityInstallation object for the provided 'version_string'
    # Returns 'None' if no installations can be found with the provided version string
    def GetUnityInstallationForVersion(self, version_string):
        if version_string in self.unity_installation_table:
            return self.unity_installation_table[version_string]
        else:
            utility.WarningMessage(f"No Unity installation tracked with version: {version_string}")
            return None

    # Adds a project at provided path to the _unity_project_list
    # Given a project path, this method derives the following information:
    #   - Project Name
    #   - Project Path
    #   - Project Version
    #   - Supported Platforms
    #   - Test Assemblies (both Editor and Player)
    # Returns the tracked UnityProject object if successful, None otherwise.
    def TrackUnityProjectAtPath(self, project_path):
        utility.StatusMessage(f"\nScanning Unity project at path: {project_path}")

        if not project_path.is_dir():
            utility.ErrorMessage(f"Failed to locate folder at path: {project_path}")
            return None

        if any(project.path == project_path for project in self.unity_project_list):
            utility.StatusMessage(f"Already tracking Unity project at path: {project_path}")
            return None

        unity_project = self.UnityProject()
        unity_project.name = project_path.name
        unity_project.path = project_path

        # Determine project's associated Unity version
        utility.StatusMessage("\n\tScanning for project version.")
        project_version_file_path = project_path.joinpath("ProjectSettings/ProjectVersion.txt")
        if project_version_file_path.exists():
            project_version_file_contents = project_version_file_path.read_text().split()
            try:
                version_string_index = project_version_file_contents.index("m_EditorVersion:") + 1
                if len(project_version_file_contents) <= version_string_index:
                    utility.WarningMessage(f"Couldn't find editor version string in {project_version_file_path}")
                    unity_project.version = self.UnityProject.unknown_unity_project_version_string
                else:
                    unity_project.version = project_version_file_contents[version_string_index]
                    utility.StatusMessage(f"\tFound project version string: {unity_project.version}")
            except:
                utility.WarningMessage(f"Couldn't find editor version string in {project_version_file_path}")
                unity_project.version = self.UnityProject.unknown_unity_project_version_string
        else:
            utility.ErrorMessage(f"Couldn't find file at path: {project_version_file_path}\nFolder must contain 'ProjectSettings/ProjectVersion.txt' to be considered a Unity project.")
            return None

        # Determine path to /Plugins, Unity's native library root
        unity_plugins_paths = list(unity_project.path.joinpath("Assets").glob('**/Plugins'))
        if len(unity_plugins_paths) < 1:
            utility.StatusMessage(f"{unity_project.name} contains no native libraries.\nTreating project as a non-native plug-in project.")
        else:
            if len(unity_plugins_paths) > 1:
                utility.WarningMessage(f"{unity_project.name} has multiple 'Plugins' folders under /Assets. Using first found at {unity_plugins_paths[0]}")

            unity_project.native_lib_plugin_path = unity_plugins_paths[0]

        
        # Find project test assemblies
        utility.StatusMessage("\n\tScanning for Unity tests.")
        unity_tests_paths = list(project_path.joinpath("Assets").glob('**/Tests'))
        if len(unity_tests_paths) < 1:
            utility.WarningMessage(f"{unity_project.name} appears to have no supported tests. Cannot find a 'Tests' folder under /Assets.")

        for tests_path in unity_tests_paths:
            test_assembly_paths = list(tests_path.glob('**/*.asmdef'))
            for test_assembly_path in test_assembly_paths:
                if list(test_assembly_path.parts).count('Editor') > 0:
                    utility.StatusMessage(f"\tFound Editor test assembly: {test_assembly_path.stem}")
                    unity_project.editor_test_assemblies.append(test_assembly_path.stem)
                else:
                    utility.StatusMessage(f"\tFound test assembly: {test_assembly_path.stem}")
                    unity_project.test_assemblies.append(test_assembly_path.stem)

        self.unity_project_list.append(unity_project)
        utility.StatusMessage("\nUnity project scan complete.")
        return unity_project

    # Helper updates a project's dictionary of supported platforms by re-scanning the Unity project's /Plugins path for known platforms
    # Note: Platform support is dependent upon which native libraries have been built.
    def UpdateSupportedPlatformsForProject(self, unity_project):
        utility.StatusMessage(f"\nScanning for supported platforms with project at: {unity_project.path}")

        path_name_to_platform_table = {
            'iOS': 'iOS',
            'tvOS': 'tvOS',
            'macOS': 'StandaloneOSX'
        }

        unity_project.supported_platforms.clear()

        for platform_path in unity_project.native_lib_plugin_path.iterdir():
            if platform_path.is_dir():
                if platform_path.name in path_name_to_platform_table:
                    platform_name = path_name_to_platform_table[platform_path.name]
                    unity_project.supported_platforms[platform_name] = platform_path
                    utility.StatusMessage(f"\nFound supported platform: {platform_name}\n\tPlatform Path: {platform_path}")
                else:
                    utility.WarningMessage(f"Unknown platform {platform_path.name} found in Plugins folder at {platform_path.parent}")

        if len(unity_project.supported_platforms) < 1:
            utility.WarningMessage(f"No supported platforms found in {unity_project.native_lib_plugin_path}")

    # Helper to look for a tracked Unity project with a given path
    # Returns a UnityProject instance if found, None otherwise.
    def GetProjectWithPath(self, project_path):
        for curr_project in self.unity_project_list:
            if project_path == curr_project.path:
                return curr_project
        return None

