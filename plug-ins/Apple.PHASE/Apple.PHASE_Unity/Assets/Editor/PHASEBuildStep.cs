using Apple.Core;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

#if UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX)
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.PHASE.Editor
{
    public class PHASEBuildStep : AppleBuildStep
    {
        public override string DisplayName => "PHASE";

        readonly Dictionary<BuildTarget, string> _libraryTable = new Dictionary<BuildTarget, string>
        {
            {BuildTarget.iOS, "AudioPluginPHASE"},
            {BuildTarget.StandaloneOSX, "AudioPluginPHASE.bundle"},
            {BuildTarget.tvOS, "AudioPluginPHASE"}
        };

#if UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX)
        public override void OnProcessFrameworks(AppleBuildProfile _, BuildTarget buildTarget, string pathToBuiltTarget, PBXProject pbxProject)
        {
            if (_libraryTable.ContainsKey(buildTarget))
            {
                string libraryName = _libraryTable[buildTarget];
                string libraryPath = AppleFrameworkUtility.GetPluginLibraryPathForBuildTarget(libraryName, buildTarget);
                if (String.IsNullOrEmpty(libraryPath))
                {
                    Debug.Log($"Failed to locate path for library: {libraryName}");
                }
                else
                {
                    AppleFrameworkUtility.CopyAndEmbed(libraryPath, buildTarget, pathToBuiltTarget, pbxProject);
                    if (buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.tvOS)
                    {
                        pbxProject.AddFrameworkToProject(pbxProject.GetUnityFrameworkTargetGuid(), "PHASE.framework", false);
                        pbxProject.AddFrameworkToProject(pbxProject.GetUnityFrameworkTargetGuid(), "ModelIO.framework", false);
                        pbxProject.AddFrameworkToProject(pbxProject.GetUnityFrameworkTargetGuid(), "AVFoundation.framework", false);
                    }
                    else
                    {
                        AppleFrameworkUtility.AddFrameworkToProject("PHASE.framework", false, buildTarget, pbxProject);
                        AppleFrameworkUtility.AddFrameworkToProject("ModelIO.framework", false, buildTarget, pbxProject);
                    }
                }
            }
            else
            {
                Debug.Log($"Processing {this.DisplayName} frameworks for unsupported platform. Skipping.");
            }
        }

        public override void OnBeginPostProcess(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.tvOS)
            {
                var projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
                PBXProject project = new PBXProject();
                if (File.Exists(projectPath))
                {
                    project.ReadFromFile(projectPath);
                    string targetGuid = project.GetUnityMainTargetGuid();
                    project.AddBuildProperty(targetGuid, "DEAD_CODE_STRIPPING", "YES");
                    project.AddBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
                    project.WriteToFile(projectPath);
                }

                var xcSettingsPath = $"{pathToBuiltProject}/Unity-iPhone.xcodeproj/project.xcworkspace/xcshareddata/WorkspaceSettings.xcsettings";
                // Change the xcode project to use the new build system.
                var xcSettingsDoc = new PlistDocument();
                if (File.Exists(xcSettingsPath))
                {
                    xcSettingsDoc.ReadFromString(File.ReadAllText(xcSettingsPath));
                    var xcSettingsValues = xcSettingsDoc.root.values;
                    var buildSystemTypeKey = "BuildSystemType";
                    if (xcSettingsValues.ContainsKey(buildSystemTypeKey))
                    {
                        // Remove key/value pair <key>BuildSystemType</key><string>Original</string>.
                        xcSettingsValues.Remove(buildSystemTypeKey);
                    }
                    File.WriteAllText(xcSettingsPath, xcSettingsDoc.WriteToString());
                }
            }
        }
#endif
    }
}
