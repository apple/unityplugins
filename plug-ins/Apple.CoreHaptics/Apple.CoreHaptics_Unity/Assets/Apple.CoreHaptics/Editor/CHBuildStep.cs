using Apple.Core;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.CoreHaptics.Editor
{
	public class CHBuildStep : AppleBuildStep
	{
		public override string DisplayName => "Apple.CoreHaptics";
		public override BuildTarget[] SupportedTargets => new BuildTarget[] {BuildTarget.iOS, BuildTarget.tvOS, BuildTarget.StandaloneOSX, BuildTarget.VisionOS};

#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
		public override void OnProcessFrameworks(AppleBuildProfile _, BuildTarget buildTarget, string generatedProjectPath, PBXProject pbxProject)
        {
            if (Array.IndexOf(SupportedTargets, buildTarget) > -1)
            {
                AppleNativeLibraryUtility.ProcessWrapperLibrary(DisplayName, buildTarget, generatedProjectPath, pbxProject);
				AppleNativeLibraryUtility.AddPlatformFrameworkDependency("CoreHaptics.framework", isWeaklyLinked: false, buildTarget, pbxProject);
            }
            else
            {
                Debug.LogWarning($"[{DisplayName}] No native library defined for Unity build target {buildTarget.ToString()}. Skipping.");
            }
        }
#endif
	}
}
