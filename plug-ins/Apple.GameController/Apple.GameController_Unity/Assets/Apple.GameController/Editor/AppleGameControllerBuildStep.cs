using Apple.Core;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.GameController.Editor
{
    public class AppleGameControllerBuildStep : AppleBuildStep
    {
        public bool GCSupportsControllerUserInteraction => true;
        public bool SupportsMicroGamePad => true;
        public bool SupportsExtendedGamePad => true;

        public override string DisplayName => "Apple.GameController";
        public override BuildTarget[] SupportedTargets => new BuildTarget[] {BuildTarget.iOS, BuildTarget.tvOS, BuildTarget.StandaloneOSX, BuildTarget.VisionOS};
#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
        public override void OnProcessInfoPlist(AppleBuildProfile _, BuildTarget buildTarget, string pathToBuiltTarget, PlistDocument infoPlist)
        {
            // Notify that we support controllers
            infoPlist.root.SetBoolean("GCSupportsControllerUserInteraction", GCSupportsControllerUserInteraction);

            if (!GCSupportsControllerUserInteraction)
                return;

            // Support device profiles
            PlistElementArray supportedControllers = null;

            if (buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.StandaloneOSX || buildTarget == BuildTarget.VisionOS)
            {
                supportedControllers = infoPlist.root.CreateArray("GCSupportedGameControllers");
            }
            else if (buildTarget == BuildTarget.tvOS)
            {
                supportedControllers = infoPlist.root["GCSupportedGameControllers"].AsArray();
            }

            if (SupportsMicroGamePad)
            {
                var newController = new PlistElementDict();
                newController.SetString("ProfileName", "MicroGamepad");
                supportedControllers.values.Add(newController);
            }

            if (SupportsExtendedGamePad)
            {
                var newController = new PlistElementDict();
                newController.SetString("ProfileName", "ExtendedGamepad");
                supportedControllers.values.Add(newController);
            }
        }

        public override void OnProcessFrameworks(AppleBuildProfile _, BuildTarget buildTarget, string generatedProjectPath, PBXProject pbxProject)
        {
            if (Array.IndexOf(SupportedTargets, buildTarget) > -1)
            {
                AppleNativeLibraryUtility.ProcessWrapperLibrary(DisplayName, buildTarget, generatedProjectPath, pbxProject);
                AppleNativeLibraryUtility.AddPlatformFrameworkDependency("GameController.framework", false, buildTarget, pbxProject);
            }
            else
            {
                Debug.LogWarning($"[{DisplayName}] No native library defined for Unity build target {buildTarget.ToString()}. Skipping.");
            }
        }
#endif // (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
    }
}
