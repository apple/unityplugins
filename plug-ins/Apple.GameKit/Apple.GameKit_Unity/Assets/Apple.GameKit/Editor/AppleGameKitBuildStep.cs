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
            if (buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.tvOS || buildTarget == BuildTarget.StandaloneOSX)
            {
                const string libraryName = "GameKitWrapper";
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
