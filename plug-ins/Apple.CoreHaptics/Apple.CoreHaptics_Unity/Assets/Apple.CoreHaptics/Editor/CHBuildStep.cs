using Apple.Core;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX)
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.CoreHaptics.Editor
{
	public class CHBuildStep : AppleBuildStep
	{
		public override string DisplayName => "CoreHaptics";

		readonly Dictionary<BuildTarget, string> _libraryTable = new Dictionary<BuildTarget, string>
		{
			{BuildTarget.iOS, "CoreHapticsWrapper.framework"},
			{BuildTarget.tvOS, "CoreHapticsWrapper.framework"},
			{BuildTarget.StandaloneOSX, "CoreHapticsWrapper.bundle"}
		};

#if UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX)
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
					AppleFrameworkUtility.AddFrameworkToProject("CoreHaptics.framework", false, buildTarget, pbxProject);
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
