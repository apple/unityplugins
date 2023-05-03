using Apple.Core;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.CloudKit.Editor {
	public class AppleCloudKitBuildStep : AppleBuildStep {
		public override string DisplayName => "CloudKit";

		readonly Dictionary<BuildTarget, string> _libraryTable = new Dictionary<BuildTarget, string>
		{
			{BuildTarget.iOS, "CloudKitWrapper.framework"},
			{BuildTarget.tvOS, "CloudKitWrapper.framework"},
			{BuildTarget.StandaloneOSX, "CloudKitWrapper.bundle"}
		};

#if UNITY_EDITOR_OSX
		public override void OnProcessEntitlements(AppleBuildProfile _, BuildTarget buildTarget, string _1, PlistDocument entitlements) {
			if (buildTarget == BuildTarget.StandaloneOSX || buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.tvOS) {
				entitlements.root.CreateArray("com.apple.developer.icloud-container-identifiers");
				entitlements.root.SetString("com.apple.developer.ubiquity-kvstore-identifier", "$(TeamIdentifierPrefix)$(CFBundleIdentifier)");
			}
		}

		public override void OnProcessFrameworks(AppleBuildProfile _, BuildTarget buildTarget, string pathToBuiltTarget, PBXProject pbxProject) {
			if (_libraryTable.ContainsKey(buildTarget)) {
				string libraryName = _libraryTable[buildTarget];
				string libraryPath = AppleFrameworkUtility.GetPluginLibraryPathForBuildTarget(libraryName, buildTarget);
				if (String.IsNullOrEmpty(libraryPath)) {
					Debug.Log($"Failed to locate path for library: {libraryName}");
				} else {
					AppleFrameworkUtility.CopyAndEmbed(libraryPath, buildTarget, pathToBuiltTarget, pbxProject);
					AppleFrameworkUtility.AddFrameworkToProject("CloudKit.framework", false, buildTarget, pbxProject);
				}
			} else {
				Debug.Log($"Processing {this.DisplayName} frameworks for unsupported platform. Skipping.");
			}
		}
#endif
	}
}
