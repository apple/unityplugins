using Apple.Core;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.PHASE.Editor
{
    public class PHASEBuildStep : AppleBuildStep
    {
        public override string DisplayName => "Apple.PHASE";
        public override BuildTarget[] SupportedTargets => new BuildTarget[] {BuildTarget.iOS, BuildTarget.tvOS, BuildTarget.StandaloneOSX, BuildTarget.VisionOS};
        
#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
        public override void OnBeginPostProcess(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.tvOS)
            {
                var projectPath = AppleBuild.GetPbxProjectPath(buildTarget, pathToBuiltProject);
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

        public override void OnProcessFrameworks(AppleBuildProfile _, BuildTarget buildTarget, string generatedProjectPath, PBXProject pbxProject)
        {
            if (Array.IndexOf(SupportedTargets, buildTarget) > -1)
            {
                AppleNativeLibraryUtility.ProcessWrapperLibrary(DisplayName, buildTarget, generatedProjectPath, pbxProject);

                if (buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.tvOS || buildTarget == BuildTarget.VisionOS)
                {
                    pbxProject.AddFrameworkToProject(pbxProject.GetUnityFrameworkTargetGuid(), "PHASE.framework", false);
                    pbxProject.AddFrameworkToProject(pbxProject.GetUnityFrameworkTargetGuid(), "ModelIO.framework", false);
                    pbxProject.AddFrameworkToProject(pbxProject.GetUnityFrameworkTargetGuid(), "AVFoundation.framework", false);
                }
                else
                {
                    AppleNativeLibraryUtility.AddPlatformFrameworkDependency("PHASE.framework", false, buildTarget, pbxProject);
                    AppleNativeLibraryUtility.AddPlatformFrameworkDependency("ModelIO.framework", false, buildTarget, pbxProject);
                }
            }
            else
            {
                Debug.LogWarning($"[{DisplayName}] No native library defined for Unity build target {buildTarget.ToString()}. Skipping.");
            }
        }
#endif // (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
    }
}
