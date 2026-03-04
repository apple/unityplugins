using Apple.Core;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.SpatialController.Editor
{
    public class AppleSpatialControllerBuildStep : AppleBuildStep
    {
        public bool GCSupportsControllerUserInteraction => true;
        public bool SupportsSpatialGamePad => true;

        public override string DisplayName => "Apple.SpatialController";
        public override BuildTarget[] SupportedTargets => new BuildTarget[] { BuildTarget.VisionOS };
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

            if (SupportsSpatialGamePad)
            {
                var newController = new PlistElementDict();
                newController.SetString("ProfileName", "SpatialGamepad");
                supportedControllers.values.Add(newController);
            }
        }

        public override void OnProcessFrameworks(AppleBuildProfile _, BuildTarget buildTarget, string generatedProjectPath, PBXProject pbxProject)
        {
            if (Array.IndexOf(SupportedTargets, buildTarget) > -1)
            {
                AppleNativeLibraryUtility.ProcessWrapperLibrary(DisplayName, buildTarget, generatedProjectPath, pbxProject);
                AppleNativeLibraryUtility.AddPlatformFrameworkDependency("SpatialController.framework", false, buildTarget, pbxProject);
            }
            else
            {
                Debug.LogWarning($"[{DisplayName}] No native library defined for Unity build target {buildTarget.ToString()}. Skipping.");
            }
        }
#endif // (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
    }
}
