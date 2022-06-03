using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.Core
{
    public class AppleCoreBuildStep : AppleBuildStep
    {
        public override string DisplayName => "Apple.Core";

        readonly Dictionary<BuildTarget, string> _libraryTable = new Dictionary<BuildTarget, string>
        {
            {BuildTarget.iOS, "AppleCoreNative.framework"},
            {BuildTarget.tvOS, "AppleCoreNative.framework"},
            {BuildTarget.StandaloneOSX, "AppleCoreNativeMac.bundle"}
        };

#if UNITY_EDITOR_OSX
        public override void OnFinalizePostProcess(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltProject)
        {
            Debug.Log($"AppleBuild: disabling Bitcode for framework and app targets.");
            var pbxProject = AppleBuild.GetPbxProject(buildTarget, pathToBuiltProject);
            var pbxProjectPath = AppleBuild.GetPbxProjectPath(buildTarget, pathToBuiltProject);

            if (pbxProject != null)
            {
                var targetGuid = (buildTarget == BuildTarget.StandaloneOSX) ? pbxProject.TargetGuidByName(Application.productName) : pbxProject.GetUnityMainTargetGuid();
                var frameworkGuid = pbxProject.GetUnityFrameworkTargetGuid();

                pbxProject.AddBuildProperty(frameworkGuid, "ENABLE_BITCODE", "false");
                pbxProject.AddBuildProperty(targetGuid, "ENABLE_BITCODE", "false");

                Debug.Log($"AppleBuild: Writing bitcode changes to PBXProject {pbxProjectPath}...");
                pbxProject.WriteToFile(pbxProjectPath);
            }
        }

        public override void OnProcessFrameworks(AppleBuildProfile _, BuildTarget buildTarget, string pathToBuiltTarget, PBXProject pbxProject)
        {
            if (_libraryTable.ContainsKey(buildTarget))
            {
                string libraryName = _libraryTable[buildTarget];
                string libraryPath = AppleFrameworkUtility.GetPluginLibraryPathForBuildTarget(libraryName, buildTarget);
                if (string.IsNullOrEmpty(libraryPath))
                {
                    Debug.Log($"Failed to locate path for library: {libraryName}");
                }
                else
                {
                    AppleFrameworkUtility.CopyAndEmbed(libraryPath, buildTarget, pathToBuiltTarget, pbxProject);
                }
            }
            else
            {
                Debug.Log($"Processing {DisplayName} frameworks for unsupported platform. Skipping.");
            }
        }
#endif
    }
}
