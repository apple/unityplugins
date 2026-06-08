using Apple.Core;
using System;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.StoreKit.Editor
{
    public class AppleStoreKitBuildStep : AppleBuildStep
    {
        public override string DisplayName => "Apple.StoreKit";
        public override BuildTarget[] SupportedTargets => new BuildTarget[] {BuildTarget.iOS, BuildTarget.tvOS, BuildTarget.StandaloneOSX, BuildTarget.VisionOS};

#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
        public override void OnProcessEntitlements(AppleBuildProfile _, BuildTarget buildTarget, string _1, PlistDocument entitlements)
        {
            // StoreKit doesn't require specific entitlements by default
            // Add any required entitlements here if needed
        }

        public override void OnProcessInfoPlist(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltTarget, PlistDocument infoPlist)
        {
            // Add any required Info.plist entries here if needed
        }

        public override void OnProcessFrameworks(AppleBuildProfile _, BuildTarget buildTarget, string generatedProjectPath, PBXProject pbxProject)
        {
            if (Array.IndexOf(SupportedTargets, buildTarget) > -1)
            {
                AppleNativeLibraryUtility.ProcessWrapperLibrary(DisplayName, buildTarget, generatedProjectPath, pbxProject);
                AppleNativeLibraryUtility.AddPlatformFrameworkDependency("StoreKit.framework", false, buildTarget, pbxProject);
            }
            else
            {
                Debug.LogWarning($"[{DisplayName}] No native library defined for Unity build target {buildTarget.ToString()}. Skipping.");
            }
        }
#endif
    }
}
