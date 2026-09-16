using Apple.Core;
using System.IO;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Apple.BackgroundAssets {
	
	public class BuildStep: AppleBuildStep {
			
			public override string DisplayName => "Apple.BackgroundAssets";
			
			public override BuildTarget[] SupportedTargets => new BuildTarget[] {BuildTarget.iOS, BuildTarget.tvOS, BuildTarget.StandaloneOSX, BuildTarget.VisionOS};
			
			[Tooltip("Your app’s downloader extension’s bundle ID. It must have your main app’s bundle ID followed by a “.” character as a prefix.")]
			public string DownloaderExtensionBundleId;
			
			[Tooltip("The ID of the app group that your app and its downloader extension share.")]
			public string AppGroupId;
			
			[Tooltip("An object that provides information about where your asset packs are hosted and how large they are in aggregate.")]
			public HostingConfiguration HostingConfiguration;
			
			const string AppGroupIdKey = "BAAppGroupID";
			
			const string AppGroupsKey = "com.apple.security.application-groups";
			
			const string AppSandboxKey = "com.apple.security.app-sandbox";
			
			public void OnEnable() {
				if (Application.identifier != null) {
					this.DownloaderExtensionBundleId = Application.identifier + ".downloader";
					this.AppGroupId = "group." + Application.identifier;
				}
			}
			
			public override void OnProcessInfoPlist(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string generatedProjectPath, PlistDocument infoPlist) {
				infoPlist.root.SetString(AppGroupIdKey, this.AppGroupId);
				this.HostingConfiguration.OnProcessInfoPlist(infoPlist);
			}
			
			public override void OnProcessEntitlements(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string generatedProjectPath, PlistDocument entitlements) {
				PlistElementArray appGroups;
				if (entitlements.root.values.ContainsKey(AppGroupsKey)) {
					appGroups = entitlements.root[AppGroupsKey].AsArray();
				} else {
					appGroups = entitlements.root.CreateArray(AppGroupsKey);
				}
				bool appIsAlreadyInAppGroup = false;
				foreach (PlistElement appGroupElement in appGroups.values) {
					if (appGroupElement is PlistElementString appGroupStringElement && appGroupStringElement.value == this.AppGroupId) {
						appIsAlreadyInAppGroup = true;
						break;
					}
				}
				if (!appIsAlreadyInAppGroup) {
					appGroups.AddString(this.AppGroupId);
				}
			}
			
			public override void OnProcessFrameworks(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string generatedProjectPath, PBXProject pbxProject) {
				// Background Assets requires that an app include a downloader extension that handles background asset downloads. The following code automatically creates and configures a downloader-extension target in the generated Xcode project.
				string mainTargetGuid =
					buildTarget == BuildTarget.StandaloneOSX
					? pbxProject.TargetGuidByName(Application.productName)
					: pbxProject.GetUnityMainTargetGuid();
				string downloaderTargetGuid = pbxProject.TargetGuidByName("Downloader");
				if (downloaderTargetGuid == null) {
					downloaderTargetGuid = pbxProject.AddTarget("Downloader", "appex", "com.apple.product-type.app-extension");
					PlistDocument downloaderInfoPlist = new PlistDocument();
					downloaderInfoPlist.root.SetString("CFBundleDisplayName", "$(PRODUCT_NAME)");
					downloaderInfoPlist.root.SetString("CFBundleExecutable", "$(EXECUTABLE_NAME)");
					downloaderInfoPlist.root.SetString("CFBundleIdentifier", "$(PRODUCT_BUNDLE_IDENTIFIER)");
					downloaderInfoPlist.root.SetString("CFBundleName", "$(PRODUCT_NAME)");
					downloaderInfoPlist.root.SetString("CFBundleVersion", PlayerSettings.iOS.buildNumber);
					downloaderInfoPlist.root.SetString("CFBundleShortVersionString", PlayerSettings.bundleVersion);
					PlistElementDict downloaderExtensionAttributes = downloaderInfoPlist.root.CreateDict("EXAppExtensionAttributes");
					downloaderExtensionAttributes.SetString("EXExtensionPointIdentifier", "com.apple.background-asset-downloader-extension");
					string fullDownloaderDirectoryPath = Path.Combine(generatedProjectPath, "Downloader");
					if (!Directory.Exists(fullDownloaderDirectoryPath)) {
						Directory.CreateDirectory(fullDownloaderDirectoryPath);
					}
					string fullDownloaderInfoPlistPath = Path.Combine(fullDownloaderDirectoryPath, "Info.plist");
					downloaderInfoPlist.WriteToFile(fullDownloaderInfoPlistPath);
					pbxProject.AddBuildProperty(downloaderTargetGuid, "CODE_SIGN_ENTITLEMENTS", "Downloader/Downloader.entitlements");
					pbxProject.AddBuildProperty(downloaderTargetGuid, "CODE_SIGN_STYLE", "Automatic");
					pbxProject.AddBuildProperty(downloaderTargetGuid, "INFOPLIST_FILE", "Downloader/Info.plist");
					pbxProject.AddBuildProperty(downloaderTargetGuid, "PRODUCT_BUNDLE_IDENTIFIER", this.DownloaderExtensionBundleId);
					pbxProject.AddBuildProperty(downloaderTargetGuid, "PRODUCT_NAME", "Downloader");
					pbxProject.AddBuildProperty(downloaderTargetGuid, "SWIFT_VERSION", "6.0");
					switch (buildTarget) {
					case BuildTarget.iOS:
						pbxProject.AddBuildProperty(downloaderTargetGuid, "IPHONEOS_DEPLOYMENT_TARGET", PlayerSettings.iOS.targetOSVersionString);
						break;
					case BuildTarget.StandaloneOSX:
						pbxProject.AddBuildProperty(downloaderTargetGuid, "MACOSX_DEPLOYMENT_TARGET", PlayerSettings.macOS.targetOSVersion);
						break;
					case BuildTarget.tvOS:
						pbxProject.AddBuildProperty(downloaderTargetGuid, "TVOS_DEPLOYMENT_TARGET", PlayerSettings.tvOS.targetOSVersionString);
						break;
					case BuildTarget.VisionOS:
						pbxProject.AddBuildProperty(downloaderTargetGuid, "XROS_DEPLOYMENT_TARGET", PlayerSettings.VisionOS.targetOSVersionString);
						break;
					}
					pbxProject.AddFolderReference(fullDownloaderDirectoryPath, "Downloader");
					pbxProject.AddSourcesBuildPhase(downloaderTargetGuid);
					string fullBackgroundDownloadHandlerPath = Path.Combine(fullDownloaderDirectoryPath, "BackgroundDownloadHandler.swift");
					string backgroundDownloadHandlerCode = "";
					if (this.HostingConfiguration is AppleHostingConfiguration) {
						backgroundDownloadHandlerCode = @"
import ExtensionFoundation
import StoreKit

@main
struct BackgroundDownloadHandler: StoreDownloaderExtension { }
";
					} else if (this.HostingConfiguration is SelfHostingConfiguration) {
						backgroundDownloadHandlerCode += @"
import BackgroundAssets
import ExtensionFoundation

@main
struct BackgroundDownloadHandler: ManagedDownloaderExtension { }
";
					}
					File.WriteAllText(fullBackgroundDownloadHandlerPath, backgroundDownloadHandlerCode);
					string backgroundDownloadHandlerGuid = pbxProject.AddFile(fullBackgroundDownloadHandlerPath, "BackgroundDownloadHandler.swift");
					pbxProject.AddFileToBuild(downloaderTargetGuid, backgroundDownloadHandlerGuid);
					string embedExtensionKitExtensionsBuildPhaseGuid = pbxProject.AddCopyFilesBuildPhase(mainTargetGuid, "Embed ExtensionKit Extensions", "$(EXTENSIONS_FOLDER_PATH)", "16");
					string downloaderProductGuid = pbxProject.FindFileGuidByProjectPath("Products/Downloader.appex");
					pbxProject.AddFileToBuildSection(mainTargetGuid, embedExtensionKitExtensionsBuildPhaseGuid, downloaderProductGuid);
					AppleNativeLibraryUtility.ProcessWrapperLibrary(this.DisplayName, buildTarget, generatedProjectPath, pbxProject);
					AppleNativeLibraryUtility.AddPlatformFrameworkDependency("BackgroundAssets.framework", false, buildTarget, pbxProject);
				} else {
					string[] downloaderInfoPlistPaths = pbxProject.GetBuildPropertyForAnyConfig(downloaderTargetGuid, "INFOPLIST_FILE").Split(' ');
					foreach (string downloaderInfoPlistPath in downloaderInfoPlistPaths) {
						string fullDownloaderInfoPlistPath = Path.Combine(generatedProjectPath, downloaderInfoPlistPath);
						PlistDocument downloaderInfoPlist = new PlistDocument();
						downloaderInfoPlist.ReadFromFile(fullDownloaderInfoPlistPath);
						downloaderInfoPlist.root.SetString("CFBundleVersion", PlayerSettings.iOS.buildNumber);
						downloaderInfoPlist.root.SetString("CFBundleShortVersionString", PlayerSettings.bundleVersion);
						downloaderInfoPlist.WriteToFile(fullDownloaderInfoPlistPath);
					}
					pbxProject.SetBuildProperty(downloaderTargetGuid, "PRODUCT_BUNDLE_IDENTIFIER", this.DownloaderExtensionBundleId);
					switch (buildTarget) {
					case BuildTarget.iOS:
						pbxProject.SetBuildProperty(downloaderTargetGuid, "IPHONEOS_DEPLOYMENT_TARGET", PlayerSettings.iOS.targetOSVersionString);
						break;
					case BuildTarget.StandaloneOSX:
						pbxProject.SetBuildProperty(downloaderTargetGuid, "MACOSX_DEPLOYMENT_TARGET", PlayerSettings.macOS.targetOSVersion);
						break;
					case BuildTarget.tvOS:
						pbxProject.SetBuildProperty(downloaderTargetGuid, "TVOS_DEPLOYMENT_TARGET", PlayerSettings.tvOS.targetOSVersionString);
						break;
					case BuildTarget.VisionOS:
						pbxProject.SetBuildProperty(downloaderTargetGuid, "XROS_DEPLOYMENT_TARGET", PlayerSettings.VisionOS.targetOSVersionString);
						break;
					}
				}
				string downloaderEntitlementsPath = pbxProject.GetEntitlementFilePathForTarget(downloaderTargetGuid);
				string fullDownloaderEntitlementsPath = Path.Combine(generatedProjectPath, downloaderEntitlementsPath);
				PlistDocument downloaderEntitlements = new PlistDocument();
				if (File.Exists(fullDownloaderEntitlementsPath)) {
					downloaderEntitlements.ReadFromFile(fullDownloaderEntitlementsPath);
				}
				PlistElementArray downloaderAppGroups;
				if (downloaderEntitlements.root.values.ContainsKey(AppGroupsKey)) {
					downloaderAppGroups = downloaderEntitlements.root[AppGroupsKey].AsArray();
				} else {
					downloaderAppGroups = downloaderEntitlements.root.CreateArray(AppGroupsKey);
				}
				bool downloaderIsAlreadyInAppGroup = false;
				foreach (PlistElement appGroupElement in downloaderAppGroups.values) {
					if (appGroupElement is PlistElementString appGroupStringElement && appGroupStringElement.value == this.AppGroupId) {
						downloaderIsAlreadyInAppGroup = true;
						break;
					}
				}
				if (!downloaderIsAlreadyInAppGroup) {
					downloaderAppGroups.AddString(this.AppGroupId);
				}
				if (!downloaderEntitlements.root.values.ContainsKey(AppSandboxKey)) {
					downloaderEntitlements.root.SetBoolean(AppSandboxKey, true);
				}
				downloaderEntitlements.WriteToFile(fullDownloaderEntitlementsPath);
				
				// This is a hack to work around the fact that Unity’s property-list API doesn’t support integer values that are greater than the maximum 32-bit signed integer.
				string[] mainInfoPlistPaths = pbxProject.GetBuildPropertyForAnyConfig(mainTargetGuid, "INFOPLIST_FILE").Split(' ');
				foreach (string mainInfoPlistPath in mainInfoPlistPaths) {
					string fullMainInfoPlistPath = Path.Combine(generatedProjectPath, mainInfoPlistPath);
					if (!File.Exists(fullMainInfoPlistPath)) {
						continue;
					}
					string mainInfoPlistString = File.ReadAllText(fullMainInfoPlistPath);
					foreach ((string Old, string New) replacement in this.HostingConfiguration.InfoPlistReplacements) {
						mainInfoPlistString = mainInfoPlistString.Replace(replacement.Old, replacement.New);
					}
					File.WriteAllText(fullMainInfoPlistPath, mainInfoPlistString);
				}
			}
			
		}
	
}
