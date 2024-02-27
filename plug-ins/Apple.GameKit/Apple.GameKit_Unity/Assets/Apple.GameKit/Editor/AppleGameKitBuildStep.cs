using Apple.Core;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX))
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.GameKit.Editor
{
    public class AppleGameKitBuildStep : AppleBuildStep
    {
        public override string DisplayName => "Apple.GameKit";
        public override BuildTarget[] SupportedTargets => new BuildTarget[] {BuildTarget.iOS, BuildTarget.tvOS, BuildTarget.StandaloneOSX};

        [Tooltip("A message that tells the user why the app needs access to their Game Center friends list.")]
        public string FriendListUsageDescription;

#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX))
        public override void OnProcessEntitlements(AppleBuildProfile _, BuildTarget buildTarget, string _1, PlistDocument entitlements)
        {
            entitlements.root.SetBoolean("com.apple.developer.game-center", true);
        }

        public override void OnProcessInfoPlist(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltTarget, PlistDocument infoPlist)
        {
            if (!string.IsNullOrWhiteSpace(FriendListUsageDescription))
            {
                infoPlist.root.SetString("NSGKFriendListUsageDescription", FriendListUsageDescription);
            }
        }

        public override void OnProcessFrameworks(AppleBuildProfile _, BuildTarget buildTarget, string generatedProjectPath, PBXProject pbxProject)
        {
            if (Array.IndexOf(SupportedTargets, buildTarget) > -1)
            {
                AppleNativeLibraryUtility.ProcessWrapperLibrary(DisplayName, buildTarget, generatedProjectPath, pbxProject);
                AppleNativeLibraryUtility.AddPlatformFrameworkDependency("GameKit.framework", false, buildTarget, pbxProject);
            }
            else
            {
                Debug.LogWarning($"[{DisplayName}] No native library defined for Unity build target {buildTarget.ToString()}. Skipping.");
            }
        }
#endif
    }
}
