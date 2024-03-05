#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX))
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Apple.Core
{
    public static class AppleConfigID
    {
        public static string Release => "Release";
        public static string Debug => "Debug";
    }

    public static class ApplePlatformID
    {
        public static string iOS => "iOS";
        public static string macOS => "macOS";
        public static string tvOS => "tvOS";
        public static string iPhoneSimulator => "iPhoneSimulator";
        public static string AppleTVSimulator => "AppleTVSimulator";
        public static string Unknown => "Unknown";

        public static string[] ValidPlatforms { get; private set; }

        static ApplePlatformID()
        {
            ValidPlatforms = new string[] {ApplePlatformID.iOS, ApplePlatformID.macOS, ApplePlatformID.tvOS, ApplePlatformID.iPhoneSimulator, ApplePlatformID.AppleTVSimulator};
        } 
    }

    [InitializeOnLoad]
    public static class ApplePlugInEnvironment
    {
        /// <summary>
        /// Name of the folder that the Apple Unity Plug-Ins will use for storing permanent data assets and helper objects
        /// </summary>
        public static string ApplePlugInSupportFolderName => "Apple Plug-In Support";
            
        /// <summary>
        /// Unity project path for assets, such as build settings, necessary to support Apple Unity Plug-Ins
        /// </summary>
        public static string ApplePlugInSupportRootPath => $"Assets/{ApplePlugInSupportFolderName}";

        /// <summary>
        /// Folder for Apple Plug-In assets meant to enable/improve/enhance Editor support
        /// </summary>
        public static string ApplePlugInSupportEditorPath => $"{ApplePlugInSupportRootPath}/Editor";

        /// <summary>
        /// The plug-in system will move necessary macOS bundles to this location to enable Play mode support within the Editor.
        /// </summary>
        public static string ApplePlugInSupportPlayModeSupportPath => $"{ApplePlugInSupportEditorPath}/PlayModeSupport";

        /// <summary>
        /// Utility property provides the containing Unity project path.
        /// </summary>
        public static string UnityProjectPath => Directory.GetParent(Application.dataPath).ToString();

        /// <summary>
        /// Access the build profile used by the Apple Unity Plug-In environment.
        /// </summary>
        public static AppleBuildProfile DefaultBuildProfile => _defaultProfile;

        /// <summary>
        /// WA: Under some combinations of build target and built setting, Unity will always return true for 'Debug.isDebugBuild' and does not set DEVELOPMENT_BUILD prior to this object's instantiation.
        /// </summary>
        public static bool IsDevelopmentBuild => EditorUserBuildSettings.development;

        /// <summary>
        /// Used to validate packages as Apple Plug-In packages, all Apple plug-in names (see package.json for any Apple plug-in) begin with this string.
        /// </summary>
        public static string AppleUnityPackageNamePrefix => "com.apple.unityplugin";

        /// <summary>
        /// Use to validate packages as Apple Plug-In packages, all Apple plug-in author names (see package.json for any Apple plug-in) are exactly this string.
        /// </summary>
        public static string AppleUnityPackageAuthorName => "Apple, Inc";

        /// <summary>
        /// For saving/restoring non-volatile configuration settings for a given project
        /// </summary>
        private static AppleBuildProfile _defaultProfile;

        /// <summary>
        /// Used for interactions with the Unity Package Manager API
        /// </summary>
        private static ListRequest _packageManagerListRequest;

        /// <summary>
        /// Collection of all known Apple Unity Plug-In packages
        /// </summary>
        private static Dictionary<string, AppleUnityPackage> _appleUnityPackages;

        /// <summary>
        /// Setting up a simple state machine for monitoring and validating configuration settings.
        /// </summary>
        private enum UpdateState
        {
            Initializing,
            Updating
        }

        /// <summary>
        /// Tracks current state
        /// </summary>
        private static UpdateState _updateState;

        /// <summary>
        /// State tracking for selected build config
        /// </summary>
        /// <remarks>
        /// TODO: (122670822)
        /// </remarks>
        private static (string Principal, string Fallback) _trackedAppleConfig;

        /// <summary>
        /// State tracking for select platform
        /// </summary>
        private static string _trackedApplePlatform;

        /// <summary>
        /// Static constructor used by Unity for initialization of the ApplePlugInEnvironment.
        /// </summary>
        static ApplePlugInEnvironment()
        {
            // Ensure that the necessary Apple Unity Plug-In support folders exist and let user know if any have been created.
            string createFolderMessage = "[Apple Unity Plug-ins] Creating support folders:\n";
            bool foldersCreated = false;

            if (!Directory.Exists(ApplePlugInSupportRootPath))
            {
                createFolderMessage += $"  Created {ApplePlugInSupportRootPath}\n";
                foldersCreated = true;
                AssetDatabase.CreateFolder("Assets", ApplePlugInSupportFolderName);
            }

            if (!Directory.Exists(ApplePlugInSupportEditorPath))
            {
                createFolderMessage += $"  Created {ApplePlugInSupportEditorPath}\n";
                foldersCreated = true;
                AssetDatabase.CreateFolder(ApplePlugInSupportRootPath, "Editor");
            }

            if (!Directory.Exists(ApplePlugInSupportPlayModeSupportPath))
            {
                createFolderMessage += $"  Created {ApplePlugInSupportPlayModeSupportPath}\n";
                foldersCreated = true;
                AssetDatabase.CreateFolder(ApplePlugInSupportEditorPath, "PlayModeSupport");
            }

            if (foldersCreated)
            {
                Debug.Log(createFolderMessage);
            }

            _defaultProfile = AppleBuildProfile.DefaultProfile();
            _defaultProfile.ResolveBuildSteps();

            // Initialize collection of packages
            _appleUnityPackages = new Dictionary<string, AppleUnityPackage>();
            _packageManagerListRequest = Client.List();

            // Initialize state tracking
            _updateState = UpdateState.Initializing;
            _trackedAppleConfig = GetAppleBuildConfig();
            _trackedApplePlatform = GetApplePlatformID(EditorUserBuildSettings.activeBuildTarget);

            EditorApplication.update += OnEditorUpdate;
            Events.registeringPackages += OnPackageManagerRegistrationUpdate;
        }

        /// <summary>
        /// As soon as the Unity Editor creates the ApplePluginEnvironment, this event handler will be added to initialize the _appleUnityPackages dictionary.
        /// </summary>
        private static void OnEditorUpdate()
        {
            switch (_updateState)
            {
                case UpdateState.Initializing:
                    if (_packageManagerListRequest.IsCompleted && _packageManagerListRequest.Status == StatusCode.Success)
                    {
                        // Need to handle a the special case of libraries being within this project, so postpone logging results.
                        AddPackagesFromCollection(_packageManagerListRequest.Result, false);

                        // If this is one of the development Apple plug-in Unity projects, it needs to be handled as a special case because 
                        //  it isn't loaded/managed by the package manager; all of the assets are local under Assets/
                        // All Apple plug-ins will have an AppleBuildStep implementation, so check for build steps that haven't been added already
                        foreach (var buildStepEntry in _defaultProfile.buildSteps)
                        {
                            if (buildStepEntry.Value.SupportedTargets.Length > 0 && !_appleUnityPackages.ContainsKey(buildStepEntry.Value.DisplayName))
                            {
                                _appleUnityPackages[buildStepEntry.Value.DisplayName] = new AppleUnityPackage("Local Project", buildStepEntry.Value.DisplayName, Application.dataPath);
                            }
                        }

                        LogLibrarySummary();
                        SyncronizePlayModeSupportLibraries();
                        ValidateLibraries();

                        _updateState = UpdateState.Updating;
                    }
                    break;
                
                case UpdateState.Updating:
                    var currentConfig = GetAppleBuildConfig();
                    string currentPlatformID = GetApplePlatformID(EditorUserBuildSettings.activeBuildTarget);

                    bool trackedStateUpdated = false;
                    if (_trackedAppleConfig.Principal != currentConfig.Principal)
                    {
                        _trackedAppleConfig = currentConfig;
                        trackedStateUpdated = true;
                    }

                    if (_trackedApplePlatform != currentPlatformID)
                    {
                        _trackedApplePlatform = currentPlatformID;
                        trackedStateUpdated = true;
                    }

                    if (trackedStateUpdated)
                    {
                        ValidateLibraries();
                    }
                    break;
            }
        }

        /// <summary>
        /// Prompts the user if a library is missing for the current config and platform given the set of registered Apple plug-in packages.
        /// </summary>
        private static void ValidateLibraries()
        {
            foreach (var applePackage in _appleUnityPackages.Values)
            {
                bool isCurrentPlatformSupported = false;
                bool isBuildStepEnabled = false;
                foreach(var buildStep in _defaultProfile.buildSteps.Values)
                {
                    if (buildStep.DisplayName == applePackage.DisplayName)
                    {
                        isBuildStepEnabled = buildStep.IsEnabled;
                        BuildTarget unityBuildTarget = GetUnityBuildTarget(_trackedApplePlatform);
                        if (Array.IndexOf(buildStep.SupportedTargets, unityBuildTarget) > -1)
                        {
                            isCurrentPlatformSupported = true;
                            break;
                        }
                    }
                }

                if (isCurrentPlatformSupported)
                {
                    AppleNativeLibrary currLibrary = GetLibrary(applePackage.DisplayName, _trackedAppleConfig.Principal, _trackedApplePlatform);
                    if (!currLibrary.IsValid)
                    {
                        string warningMessage = $"[Apple Unity Plug-Ins] Missing {_trackedAppleConfig.Principal} {applePackage.DisplayName} native library for {_trackedApplePlatform}\n"
                        + $"  {_trackedAppleConfig.Fallback} {applePackage.DisplayName} native library for {_trackedApplePlatform} will be used as a fallback.\n"
                        + $"  To generate the {_trackedAppleConfig.Principal} native library for {applePackage.DisplayName}, try re-building the {applePackage.DisplayName} plug-in with the following command line (assuming the working directory is the Apple Unity Plug-In project root folder):\n\n"
                        + $"  <b><color=orange>$> python3 ./build.py -p {applePackage.ShortName}</color></b>\n";

                        Debug.LogWarning(warningMessage);
                    }
                }
                else if (isBuildStepEnabled)
                {
                    string warningMessage = $"[Apple Unity Plug-Ins] Targeting unsupported platform '{_trackedApplePlatform}' for Apple plug-in package {applePackage.DisplayName}.\n"
                    + $"  To continue building for the current platform, please disable {applePackage.DisplayName} in the Apple Build Settings window.";

                    Debug.LogWarning(warningMessage);
                }
            }
        }

        /// <summary>
        /// Gelper gets the current AppleConfigID based upon Editor development build settings.
        /// </summary>
        /// <returns>A tuple which contains an AppleConfigID string representing the principal config as well as a fallback</returns>
        public static (string Principal, string Fallback) GetAppleBuildConfig()
        {
            return IsDevelopmentBuild ? (AppleConfigID.Debug, AppleConfigID.Release) : (AppleConfigID.Release, AppleConfigID.Debug);
        }

        /// <summary>
        /// Determines the Apple platform from the combination of Unity BuildTarget and SDK version.
        /// </summary>
        /// <remarks>
        /// The current Unity SDK is derived from the Unity Editor's Player Settings window.
        /// </remarks>
        /// <param name="unityBuildTarget">The current Unity build target to get the Apple platform for.</param>
        /// <returns>A string representing the selected ApplePlatformID</returns>
        public static string GetApplePlatformID(BuildTarget unityBuildTarget)
        {
            switch (unityBuildTarget)
            {
                case BuildTarget.iOS:
                    switch (PlayerSettings.iOS.sdkVersion)
                    {
                        case iOSSdkVersion.DeviceSDK: return ApplePlatformID.iOS;
                        case iOSSdkVersion.SimulatorSDK: return ApplePlatformID.iPhoneSimulator;
                        default: return ApplePlatformID.Unknown;
                    }

                case BuildTarget.tvOS:
                    switch (PlayerSettings.tvOS.sdkVersion)
                    {
                        case tvOSSdkVersion.Device: return ApplePlatformID.tvOS;
                        case tvOSSdkVersion.Simulator: return ApplePlatformID.AppleTVSimulator;
                        default: return ApplePlatformID.Unknown;
                    }

                case BuildTarget.StandaloneOSX: return ApplePlatformID.macOS;

                default: return ApplePlatformID.Unknown;
            }
        }

        /// <summary>
        /// Gets the associated Unity BuildTarget for a given <c>ApplePlatformID</c> string.
        /// </summary>
        /// <remarks>
        /// Although it's unlikely that it should ever happen, this method will default to assuming a <c>BuildTarget.StandaloneOSX</c> when <c>ApplePlatformID.Unknown</c> is provided as an argument.
        /// </remarks>
        /// <param name="applePlatformId">The <c>ApplePlatformID</c> string to use as a look-up</param>
        /// <returns>The associated Unity <c>BuildTarget</c>.</returns>
        public static BuildTarget GetUnityBuildTarget(string applePlatformId)
        {
            if (applePlatformId == ApplePlatformID.iOS || applePlatformId == ApplePlatformID.iPhoneSimulator)
            {
                return BuildTarget.iOS;
            }

            if (applePlatformId == ApplePlatformID.tvOS || applePlatformId == ApplePlatformID.AppleTVSimulator)
            {
                return BuildTarget.tvOS;
            }

            if (applePlatformId == ApplePlatformID.macOS)
            {
                return BuildTarget.StandaloneOSX;
            }

            Debug.LogWarning($"[Apple Unity Plug-ins] (GetUnityBuildTarget) Encountered Unknown Apple platform. Assuming macOS.");
            return BuildTarget.StandaloneOSX;
        }

        /// <summary>
        /// Retrieves an AppleNativeLibrary for the given plug-in package built with a given AppleConfigID targeting a specific ApplePlatformID.
        /// </summary>
        /// <param name="pluginDisplayName">The displayName of the Apple plug-in package, corresponding to the package's <c>displayName</c> entry in it's <c>package.json</c> file.</param>
        /// <param name="appleBuildConfig">The AppleConfigID used to look up the AppleNativeLibrary.</param>
        /// <param name="applePlatform">The ApplePlatformID used to look up the AppleNativeLibrary</param>
        /// <returns>The desired AppleNativeLibrary, or an Invalid AppleNativeLibrary if none is found.</returns>
        public static AppleNativeLibrary GetLibrary(string packageDisplayName, string appleBuildConfig, string applePlatform)
        {
            if (_appleUnityPackages.ContainsKey(packageDisplayName))
            {
                return _appleUnityPackages[packageDisplayName].GetLibrary(appleBuildConfig, applePlatform);
            }

            return AppleNativeLibrary.Invalid;
        }

        /// <summary>
        /// Unity package manager API retrives package updates via iterable collections of PackageInfo structs; this helper will handle updating the the environment's representation of each Apple package.
        /// </summary>
        /// <param name="packageCollection">An iterable collection of PackageInfo structs</param>
        private static void AddPackagesFromCollection(IEnumerable<UnityEditor.PackageManager.PackageInfo> packageCollection, bool logPackagesAfterUpdate = true)
        {
            bool packagesAdded = false;
            foreach (var unityPackage in packageCollection)
            {
                if (unityPackage.name.StartsWith(AppleUnityPackageNamePrefix) && unityPackage.author.name == AppleUnityPackageAuthorName && !_appleUnityPackages.ContainsKey(unityPackage.displayName))
                {
                    AppleUnityPackage applePackage = new AppleUnityPackage(unityPackage.name, unityPackage.displayName, unityPackage.resolvedPath);
                    if (!applePackage.PlayModeSupportLibrary.IsValid)
                    {
                        string warningMessage = $"[Apple Unity Plug-ins] Unable to locate a macOS native library for {applePackage.DisplayName}\n"
                        + $"  Play mode support for {applePackage.DisplayName} in the Unity Editor won't function without this library.\n"
                        + "  If this is unexpected, please ensure that your build script invocation includes macOS platform support: (build.py -m macOS ...)";
                        Debug.LogWarning(warningMessage);
                    }
                    _appleUnityPackages[applePackage.DisplayName] = applePackage;
                    packagesAdded = true;
                }
            }

            if (packagesAdded && logPackagesAfterUpdate)
            {
                LogLibrarySummary();
            }
        }

        /// <summary>
        /// This handler is called when the Unity package manager is updating registered plug-ins. This occurs when a user adds/removes a package from their project.
        /// </summary>
        /// <param name="eventArgs">Contains the collections of added, removed, and updated packages.</param>
        private static void OnPackageManagerRegistrationUpdate(PackageRegistrationEventArgs eventArgs)
        {
            AddPackagesFromCollection(eventArgs.added);

            int packagesRemoved = 0;
            foreach (var removedPackage in eventArgs.removed)
            {
                if(_appleUnityPackages.Remove(removedPackage.displayName))
                {
                    ++packagesRemoved;
                }
            }

            if (packagesRemoved > 0)
            {
                LogLibrarySummary();
            }
            
            SyncronizePlayModeSupportLibraries();
        }

        /// <summary>
        /// Helper ensures that the current set of PlayModeSupport libraries are aligned with tracked Apple Packages
        /// </summary>
        private static void SyncronizePlayModeSupportLibraries()
        {
            // Remove existing libraries
            string[] pmsPath = { ApplePlugInSupportPlayModeSupportPath };
            foreach (var libraryAsset in AssetDatabase.FindAssets("", pmsPath))
            {
                string path = AssetDatabase.GUIDToAssetPath(libraryAsset);
                AssetDatabase.DeleteAsset(path);
            }
            
            // Copy current
            foreach (AppleUnityPackage applePackage in _appleUnityPackages.Values)
            {
                if (applePackage.PlayModeSupportLibrary.IsValid)
                {
                    AppleNativeLibrary pmsLibrary = applePackage.PlayModeSupportLibrary;
                    FileUtil.CopyFileOrDirectory(pmsLibrary.FullPath, $"{UnityProjectPath}/{ApplePlugInSupportPlayModeSupportPath}/{pmsLibrary.FileName}");
                }
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Outputs a nicely formatted summary of the currently tracked libraries to Unity's Debug logging.
        /// </summary>
        private static void LogLibrarySummary()
        {
            string summary = "[Apple Unity Plug-ins] Apple native plug-ins updated.\nTracking the following plug-in packages and native libraries:\n\n";
            bool librariesFound = false;
            foreach (AppleUnityPackage package in _appleUnityPackages.Values)
            {
                summary += $"<b>{package.DisplayName}</b> [{package.Name}]:\n  Package Source Path: {package.SourcePath}\n";
                var debugLibraries = package.GetLibraries(AppleConfigID.Debug);
                if (debugLibraries.Length > 0)
                {
                    summary += "  Debug Libraries (file name - platform):\n";
                    foreach (var debugLibrary in debugLibraries)
                    {
                        summary += $"    {debugLibrary.FileName} - {debugLibrary.Platform}\n";
                    }
                    librariesFound = true;
                }

                var releaseLibraries = package.GetLibraries(AppleConfigID.Release);
                if (releaseLibraries.Length > 0)
                {
                    summary += "  Release Libraries (file name - platform):\n";
                    foreach (var releaseLibrary in releaseLibraries)
                    {
                        summary += $"    {releaseLibrary.FileName} - {releaseLibrary.Platform}\n";
                        if (releaseLibrary.DebugSymbolsFileName != string.Empty)
                        {
                            summary += $"    {releaseLibrary.DebugSymbolsFileName} - {releaseLibrary.Platform}\n";
                        }
                    }
                    librariesFound = true;
                }
            }

            if (librariesFound)
            {
                Debug.Log(summary);
            }
            else
            {
                string errorMessage = "[Apple Unity Plug-ins] No Apple native plug-in libraries found.\n"
                + "  Were the libraries built with the build script (build.py), xcodebuild, or Xcode?\n"
                + "  Was any output generated from the build script/tool?\n"
                + "  Were there any errors or issues?\n"
                + "  Please check to ensure that libraries (.a, .framework, or .bundle files) were created and saved to each plug-in's \"NativeLibraries~\" folder.";

                Debug.LogError(errorMessage);
            }
        }
    }

    /// <summary>
    /// Class offers an interface and utility methods for managing Apple platform native libraries.
    /// </summary>
    public static class AppleNativeLibraryUtility
    {
        public static string SourceNativeLibraryFolderName => "NativeLibraries~";
        public static string DestinationNativeLibraryFolderName => "ApplePluginLibraries";
        public static string AppleUnityPackageIdentifier => "com.apple.unityplugin";
        public static bool IsXcodeProjectGeneratedForMac => EditorUserBuildSettings.GetPlatformSettings("OSXUniversal", "CreateXcodeProject") == "true";

        /// <summary>
        /// Gets the native library root folder for the generated Xcode project.
        /// </summary>
        /// <remarks>
        /// When building Xcode projects for macOS, Unity puts everything but the project under an additional folder "/{Application.productName}" - we'll honor this folder hierarchy.
        /// Output paths will of the following form:
        ///     iOS/tvOS: <c>[XCODE_PROJECT_DIR]/ApplePluginLibraries/[PLUGIN_NAME]/ApplePluginLibrary.suffix</c>
        ///        macOS: <c>[XCODE_PROJECT_DIR]/[Application.productName]/ApplePluginLibraries/[PLUGIN_NAME]/ApplePluginLibrary.suffix</c>
        /// </remarks>
        /// <param name="unityBuildTarget">Current Unity BuildTarget</param>
        /// <returns>A string representing the path to the destination libraries, relative to the generated Xcode project's PROJECT_DIR folder.</returns>
        public static string GetDestinationNativeLibraryFolderRoot(BuildTarget unityBuildTarget)
        {
            return (unityBuildTarget == BuildTarget.StandaloneOSX) ? $"{Application.productName}/{DestinationNativeLibraryFolderName}" : DestinationNativeLibraryFolderName;
        }
        
        /// <summary>
        /// Use this method to add a standard platform framework dependency, such as UIKit.framework, for your plug-in
        /// </summary>
        /// <param name="framework">String representing the name of the platform framework to add. Example: <c>"UIKit.framework"</c></param>
        /// <param name="isWeaklyLinked">Bool determines if the framework you would like to add should be weakly linked.</param>
        /// <param name="unityBuildTarget">The currently configured Unity BuildTarget./param>
        /// <param name="project">An instance of Unity's PBXProject for interfacing with the generated Xcode project</param>
        public static void AddPlatformFrameworkDependency(string framework, bool isWeaklyLinked, BuildTarget unityBuildTarget, PBXProject project)
        {
            if (unityBuildTarget == BuildTarget.StandaloneOSX)
            {
                if (!IsXcodeProjectGeneratedForMac)
                {
                    return;
                }

                project.AddFrameworkToProject(project.TargetGuidByName(Application.productName), framework, isWeaklyLinked);
            }
            else
            {
                project.AddFrameworkToProject(project.GetUnityMainTargetGuid(), framework, isWeaklyLinked);
            }            
        }

        /// <summary>
        /// Handles selecting, copying, embedding, and linking each plug-in's native libraries into the projects generated by Unity.
        /// </summary>
        /// <param name="pluginDisplayName">The plug-in's DisplayName as defined in it's <c>package.json</c> file</param>
        /// <param name="unityBuildTarget">The currently configured Unity BuildTarget. This is used to look up the correct native library.</param>
        /// <param name="projectPath">The generated Xcode project path.</param>
        /// <param name="project">An instance of Unity's PBXProject for interfacing with the generated Xcode project at <c>projectPath</c></param>
        public static void ProcessWrapperLibrary(string pluginDisplayName, BuildTarget unityBuildTarget, string projectPath, PBXProject project)
        {
            string platform = ApplePlugInEnvironment.GetApplePlatformID(unityBuildTarget);
            if (platform == ApplePlatformID.Unknown)
            {
                string errorMessage = $"[Apple.Core.AppleNativeLibraryUtility] processing libraries for an unknown build target: {unityBuildTarget}\n"
                + "  This may be due to a configuration issue. This processor should only run for Apple platforms.";

                Debug.LogError(errorMessage);
                return;
            }

            var configuration = ApplePlugInEnvironment.GetAppleBuildConfig();

            AppleNativeLibrary appleLibrary = ApplePlugInEnvironment.GetLibrary(pluginDisplayName, configuration.Principal, platform);
            if (!appleLibrary.IsValid)
            {
                appleLibrary = ApplePlugInEnvironment.GetLibrary(pluginDisplayName, configuration.Fallback, platform);

                if (!appleLibrary.IsValid)
                {
                    string errorMessage = $"[Apple.Core.AppleNativeLibraryUtility] Cannot locate a {configuration.Principal} or {configuration.Fallback} {pluginDisplayName} native library for {platform}.\n"
                    + $"  Please ensure that the build invocation (build.py, xcodebuild, or Xcode) compiled cleanly and that the build was configured to support {configuration.Principal} on {platform}.";

                    Debug.LogError(errorMessage);
                    return;
                }

                string warningMessage = $"[Apple.Core.AppleNativeLibraryUtility] Cannot locate a {configuration.Principal} {platform} native library for {pluginDisplayName}.\n"
                + $"  Falling back on the {configuration.Fallback} {platform} library.\n"
                + $"  If this is an error, ensure that the build invocation (build.py, xcodebuild, or Xcode) compiled cleanly and that the build was configured to support {configuration.Principal} on {platform}.";
                
                Debug.LogWarning(warningMessage);
            }

            // Track if Unity is building a Mac app directly rather than through an intermediate (trampoline) Xcode project.
            bool isBuildingMacApp = (unityBuildTarget == BuildTarget.StandaloneOSX) && !IsXcodeProjectGeneratedForMac;
            if (!isBuildingMacApp && (project == null))
            {
                Debug.LogError($"[Apple.Core.AppleNativeLibraryUtility] Generated Xcode project is null!\n  Expected project path: {projectPath}");
                return;
            }
            
            // Building for Mac requires bundles for two reasons:
            // 1. When Unity builds a Mac app directly, there's no opportunity to link libraries of other types
            // 2. When packaging the plug-ins, npm strips symlinks which are vital to the structure of macOS frameworks.
            if ((unityBuildTarget == BuildTarget.StandaloneOSX) && (appleLibrary.Type != AppleNativeLibrary.LibraryType.Bundle))
            {
                string errorMessage =  $"[Apple.Core.AppleNativeLibraryUtility] Attempting to build a Mac app with an unsupported library type: {appleLibrary.Type.ToString()}\n"
                + "  Building for macOS requires that native libraries are packaged as bundles.\n"
                + "  Unless you are developing your own native library based plug-in you should not see this error.\n"
                + "  All Apple Unity plug-ins are already configured to create bundles when building for macOS.\n"
                + "  Please check your config; this can occur if folders or libraries have been moved to unexpected locations.";

                Debug.LogError(errorMessage);
                return;
            }

            // Destination paths are different depending upon the following scenarios:
            //  1. Building a Mac app directly: [OUTPUT_APP_PATH]/Contents/PlugIns
            //  2. Build an Xcode project which targets macOS: [XCODE_PROJECT_DIR]/[Application.productName]/ApplePluginLibraries/[PLUGIN_NAME]/
            //  3. Build an Xcode project which targets iOS/tvOS: [XCODE_PROJECT_DIR]/ApplePluginLibraries/[PLUGIN_NAME]/
            //
            // In the first (1.) case, no linking is necessary. Each library .bundle must be copied to the correct location in the containing .app bundle
            //  The path used in this case corresponds to the Xcode build setting PLUGINS_FOLDER_PATH
            //  Please see the following for more information: https://developer.apple.com/documentation/bundleresources/placing_content_in_a_bundle#
            // In the second (2.) and third (3.) cases library locations are not strictly enforced; but the Xcode project must be updated with the correct linker settings.
            string dstFolderPath = isBuildingMacApp ? $"{projectPath}/Contents/PlugIns" : $"{projectPath}/{GetDestinationNativeLibraryFolderRoot(unityBuildTarget)}/{pluginDisplayName}";

            if (!Directory.Exists(dstFolderPath))
            {
                Directory.CreateDirectory(dstFolderPath);
            }

            // Libraries can have arbitrary names, but static libraries require the "lib" prefix to link
            //  At least ensure that library name has the correct prefix, but this could fail with a library called library.a, for example
            string libPrefix = "lib";
            string dstLibraryPath = ((appleLibrary.Type == AppleNativeLibrary.LibraryType.Static) && !appleLibrary.FileName.StartsWith(libPrefix)) ? $"{dstFolderPath}/{libPrefix}{appleLibrary.FileName}" : $"{dstFolderPath}/{appleLibrary.FileName}";
            string dstDebugSymbolPath = $"{dstFolderPath}/{appleLibrary.DebugSymbolsFileName}";

            // Delete existing library and dSYM
            if (Directory.Exists(dstLibraryPath) || File.Exists(dstLibraryPath))
            {
                FileUtil.DeleteFileOrDirectory(dstLibraryPath);
            }

            if (appleLibrary.DebugSymbolsFileName != string.Empty && Directory.Exists(dstDebugSymbolPath))
            {
                FileUtil.DeleteFileOrDirectory(dstDebugSymbolPath);
            }

            // Copy library and, optionally, .dSYM
            FileUtil.CopyFileOrDirectory(appleLibrary.FullPath, dstLibraryPath);

            if (appleLibrary.DebugSymbolsFileName != string.Empty && Directory.Exists(appleLibrary.FullDebugSymbolsPath))
            {
                FileUtil.CopyFileOrDirectory(appleLibrary.FullDebugSymbolsPath, dstDebugSymbolPath);
            }
    
            // Since building apps directly for Mac requires bundles, no linking is necessary. In fact, there's no project to update with linking information.
            if (!isBuildingMacApp)
            {
                // Configure Unity trampoline project target linker settings.
                string relativeDestinationNativeLibraryRootPath = GetDestinationNativeLibraryFolderRoot(unityBuildTarget);
                if(!UpdateUnityProjectLinkerConfig(project.GetUnityMainTargetGuid(), relativeDestinationNativeLibraryRootPath, pluginDisplayName, project, appleLibrary))
                {
                    Debug.LogError($"[Apple.Core.AppleNativeLibraryUtility] Issue encoured while linking {appleLibrary.FileName} with Unity's main target in the generated Xcode project.\n  Skipping further steps.\n  Please investigate prior errors.");
                    return;
                }

                if (unityBuildTarget != BuildTarget.StandaloneOSX)
                {
                    UpdateUnityProjectLinkerConfig(project.GetUnityFrameworkTargetGuid(), relativeDestinationNativeLibraryRootPath, pluginDisplayName, project, appleLibrary);
                }
            }
        }

        /// <summary>
        /// Helper updates Unity trampoline project's build settings with linker settings.
        /// </summary>
        /// <param name="unityTargetGuid">The Unity target's GUID to apply the build settings to.</param>
        /// <param name="projectRelativeNativeLibraryRoot">Path to the native library root under the project (PROJECT_DIR) folder.</param>
        /// <param name="pluginName">The name of the plug-in adding the library, ex: Apple.Core</param>
        /// <param name="PBXProject">Unity's interface to the Unity-generated trampoline project.</param>
        /// <param name="AppleNativeLibrary">The native library to be linked.</param>
        private static bool UpdateUnityProjectLinkerConfig(string unityTargetGuid, string projectRelativeNativeLibraryRoot, string pluginName, PBXProject project, AppleNativeLibrary appleLibrary)
        {
            // When configuring Xcode to build against a framework, such as Example.framework, an associated flag removing the suffix is necessary: "-framework Example"
            string shortLibraryName = appleLibrary.FileName[..appleLibrary.FileName.LastIndexOf('.')];
            switch (appleLibrary.Type)
            {
                case AppleNativeLibrary.LibraryType.Framework:
                    project.AddBuildProperty(unityTargetGuid, "FRAMEWORK_SEARCH_PATHS", $"$(PROJECT_DIR)/{projectRelativeNativeLibraryRoot}/{pluginName}");
                    project.AddBuildProperty(unityTargetGuid, "OTHER_LDFLAGS", $"-L$(PROJECT_DIR)/{projectRelativeNativeLibraryRoot}/{pluginName}");

                    // Unity's AddBuildProperty() API filters duplicate build properties; therefore it's not possible to add multiple instances of the "-framework" flag directly.
                    //  This prevents this script from adding sets of flags of the form "-framework PluginA -framework PluginB" and similar to OTHER_LDFLAGS
                    //  As a WA, the script will introduce a unique build setting per library which itself resolves to the required flag "-framework."
                    string pluginFrameworkBuildSettingKey = $"APPLE_{shortLibraryName.ToUpper()}_FW_FLAG";
                    project.AddBuildProperty(unityTargetGuid, pluginFrameworkBuildSettingKey, "-framework");

                    // Now OTHER_LDFLAGS can reference the unique token as an indirect for the "-framework" flag.
                    project.AddBuildProperty(unityTargetGuid, "OTHER_LDFLAGS", $"$({pluginFrameworkBuildSettingKey})");
                    project.AddBuildProperty(unityTargetGuid, "OTHER_LDFLAGS", $"{shortLibraryName}");
                    break;

                case AppleNativeLibrary.LibraryType.Static:
                    project.AddBuildProperty(unityTargetGuid, "OTHER_LDFLAGS", $"-L$(PROJECT_DIR)/{projectRelativeNativeLibraryRoot}/{pluginName}");
                    project.AddBuildProperty(unityTargetGuid, "OTHER_LDFLAGS", $"-l{shortLibraryName}");
                    break;

                case AppleNativeLibrary.LibraryType.Bundle:
                    string warningMessage = "[Apple.Core.AppleNativeLibraryUtility] attempting to link a bundle.\n"
                    + "  Libraries packaged as .bundle files aren't processed by the linker.\n"
                    + "  They should only be used for builds targeting macOS.\n"
                    +"   Given an app called Sample.app, they should be copied to: Sample.app/Contents/PlugIns";
                
                    Debug.LogWarning(warningMessage);
                    break;

                case AppleNativeLibrary.LibraryType.Unknown:
                    Debug.LogError("[Apple.Core.AppleNativeLibraryUtility] Attempting to link an unknown library type.");
                    return false;
            }
            return true;
        }
    }
}
#endif // #if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX))
