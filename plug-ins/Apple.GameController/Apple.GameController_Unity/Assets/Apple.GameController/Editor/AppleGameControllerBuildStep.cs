using Apple.Core;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.GameController.Editor
{
    public class AppleGameControllerBuildStep : AppleBuildStep
    {
        public override string DisplayName => "GameController";

        public bool GCSupportsControllerUserInteraction = true;
        public bool SupportsMicroGamePad = true;
        public bool SupportsExtendedGamePad = true;

#if UNITY_EDITOR_OSX
        public override void OnProcessInfoPlist(AppleBuildProfile _, BuildTarget buildTarget, string pathToBuiltTarget, PlistDocument infoPlist)
        {
            // Notify that we support controllers...
            infoPlist.root.SetBoolean("GCSupportsControllerUserInteraction", GCSupportsControllerUserInteraction);

            if (!GCSupportsControllerUserInteraction)
                return;

            // Support device profiles...
            PlistElementArray supportedControllers = null;

            if (buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.StandaloneOSX)
            {
                supportedControllers = infoPlist.root.CreateArray("GCSupportedGameControllers");
            }
            else if (buildTarget == BuildTarget.tvOS)
            {
                supportedControllers = infoPlist.root["GCSupportedGameControllers"].AsArray();
            }

            // MicroGamePad...
            if (SupportsMicroGamePad)
            {
                var newController = new PlistElementDict();
                newController.SetString("ProfileName", "MicroGamepad");
                supportedControllers.values.Add(newController);
            }

            // ExtendedGamepad...
            if (SupportsExtendedGamePad)
            {
                var newController = new PlistElementDict();
                newController.SetString("ProfileName", "ExtendedGamepad");
                supportedControllers.values.Add(newController);
            }
        }

        public override void OnProcessFrameworks(AppleBuildProfile _, BuildTarget buildTarget, string pathToBuiltTarget, PBXProject pbxProject)
        {
            if (buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.tvOS || buildTarget == BuildTarget.StandaloneOSX)
            {
                const string libraryName = "GameControllerWrapper";
                string libraryPath = AppleFrameworkUtility.GetPluginLibraryPathForBuildTarget(libraryName, buildTarget);
                if (String.IsNullOrEmpty(libraryPath))
                {
                    Debug.Log($"Failed to locate path for library: {libraryName}");
                }
                else
                {
                    AppleFrameworkUtility.CopyAndEmbed(libraryPath, buildTarget, pathToBuiltTarget, pbxProject);
                    AppleFrameworkUtility.AddFrameworkToProject("GameController.framework", false, buildTarget, pbxProject);
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
