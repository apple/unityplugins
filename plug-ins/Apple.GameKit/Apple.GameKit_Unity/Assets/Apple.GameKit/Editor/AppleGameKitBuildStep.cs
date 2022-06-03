using Apple.Core;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.GameKit.Editor
{
    public class AppleGameKitBuildStep : AppleBuildStep
    {
        public override string DisplayName => "GameKit";

        readonly Dictionary<BuildTarget, string> _libraryTable = new Dictionary<BuildTarget, string>
        {
            {BuildTarget.iOS, "GameKitWrapper.framework"},
            {BuildTarget.tvOS, "GameKitWrapper.framework"},
            {BuildTarget.StandaloneOSX, "GameKitWrapper.bundle"}
        };

#if UNITY_EDITOR_OSX
        public override void OnProcessEntitlements(AppleBuildProfile _, BuildTarget buildTarget, string _1, PlistDocument entitlements)
        {
            if(buildTarget == BuildTarget.StandaloneOSX)
            {
                entitlements.root.SetBoolean("com.apple.developer.game-center", true);
            }
        }

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
                    AppleFrameworkUtility.AddFrameworkToProject("GameKit.framework", false, buildTarget, pbxProject);
                }
            }
            else
            {
                Debug.Log($"Processing {this.DisplayName} frameworks for unsupported platform. Skipping.");
            }
        }
#endif
    }
}
