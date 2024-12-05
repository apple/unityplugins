#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
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
using System.Threading;

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
        public static string visionOS => "visionOS";
        public static string iPhoneSimulator => "iPhoneSimulator";
        public static string AppleTVSimulator => "AppleTVSimulator";
        public static string VisionSimulator => "VisionSimulator";
        public static string Unknown => "Unknown";

        public static string[] ValidPlatforms { get; private set; }

        static ApplePlatformID()
        {
            ValidPlatforms = new string[] {ApplePlatformID.iOS, ApplePlatformID.macOS, ApplePlatformID.tvOS, ApplePlatformID.iPhoneSimulator, ApplePlatformID.AppleTVSimulator, ApplePlatformID.visionOS, ApplePlatformID.VisionSimulator};
        } 
    }

    [InitializeOnLoad]
    public class ApplePlugInEnvironment : AssetPostprocessor
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
        /// Time, in seconds, that the package manager list request will wait before failing the list request.
        /// </summary>
        /// <remarks>
        /// The Apple Unity Plug-Ins infrastructure will fail if attempts to access the list of packages fails, so it may be necessary to investigate what would cause timeouts or even to extend the timeout duration.
        /// </remarks>
        private static int _packageManagerBatchModeListRequestTimeout => 5;

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
        /// Static constructor used by Unity for early initialization of the <c>ApplePlugInEnvironment</c>.
        /// </summary>
        static ApplePlugInEnvironment()
        {
            _appleUnityPackages = new Dictionary<string, AppleUnityPackage>();
            _packageManagerListRequest = Client.List();
            Events.registeringPackages += OnPackageManagerRegistrationUpdate;

            _trackedAppleConfig = GetAppleBuildConfig();
            _trackedApplePlatform = GetApplePlatformID(EditorUserBuildSettings.activeBuildTarget);

            _updateState = UpdateState.Initializing;

            if (!Application.isBatchMode)
            {
                EditorApplication.update += OnEditorUpdate;
            }
        }

        /// <summary>
        /// Called when Package Manager List requests have succeded, this method updates internal data structures which track Apple Unity Plug-In packages and their associated native libraries.
        /// </summary>
        /// <param name="syncPlayModeLibraries">When true, macOS support libraries for each Apple Unity Plug-In will be copied to <c>ApplePlugInSupportPlayModeSupportPath</c></param>
        private static void OnPackageManagerListSuccess(bool syncPlayModeLibraries)
        {
            // Need to handle the special case of libraries being within this project, so postpone logging results.
            AddPackagesFromCollection(_packageManagerListRequest.Result, logPackagesAfterUpdate: false);

            // If this is one of the development Apple plug-in Unity projects, it needs to be handled as a special case because 
            //  it isn't loaded/managed by the package manager; all of the assets are local under Assets/
            // All Apple plug-ins will have an AppleBuildStep implementation, so check for build steps that haven't been added already
            foreach (var buildStep in _defaultProfile.buildSteps.Values)
            {
                if (!_appleUnityPackages.ContainsKey(buildStep.DisplayName))
                {
                    _appleUnityPackages[buildStep.DisplayName] = buildStep.IsNativePlugIn ? new AppleUnityPackage("Local Project", buildStep.DisplayName, Application.dataPath) : new AppleUnityPackage("Local Project", buildStep.DisplayName);
                }
            }

            LogLibrarySummary();

            if (syncPlayModeLibraries)
            {
                SyncronizePlayModeSupportLibraries();
            }
            
            ValidateLibraries();
        }

        /// <summary>
        /// Handles all initialization which requires asset operations.
        /// </summary>
        /// <param name="importedAssets">Array of paths to imported assets.</param>
        /// <param name="deletedAssets">Array of paths to deleted assets.</param>
        /// <param name="movedAssets">Array of paths to moved assets.</param>
        /// <param name="movedFromAssetPaths">Array of original paths for moved assets.</param>
        /// <param name="didDomainReload">Boolean set to true if there has been a domain reload.</param>
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            if (!didDomainReload)
            {
                return;
            }

            // Apple Unity plug-ins settings are stored in a non-volatile asset, the default build profile, at ApplePlugInSupportEditorPath within the project
            if (!Directory.Exists(ApplePlugInSupportRootPath))
            {
                Debug.Log($"[Apple Unity Plug-ins] Creating support folder: {ApplePlugInSupportRootPath}");
                AssetDatabase.CreateFolder("Assets", ApplePlugInSupportFolderName);
            }

            if (!Directory.Exists(ApplePlugInSupportEditorPath))
            {
                Debug.Log($"[Apple Unity Plug-ins] Creating support folder: {ApplePlugInSupportEditorPath}");
                AssetDatabase.CreateFolder(ApplePlugInSupportRootPath, "Editor");
            }

            _defaultProfile = AppleBuildProfile.DefaultProfile();
            _defaultProfile.ResolveBuildSteps();

            // When running in the Editor, initialization of the tracked Apple Unity plug-in packages is driven through interaction with the Package Manager scriting API,
            //  which may take several update cycles to complete. Lacking a standard update loop, a while-loop will burn cycles until the list request has completed.
            if (Application.isBatchMode)
            {
                DateTime startTime = DateTime.Now;

                // Unfortunately, Unity will throw an exception if this isn't called on the main thread. Stuck with blocking behavior for now.
                while (!_packageManagerListRequest.IsCompleted)
                {
                    if ((DateTime.Now - startTime).Seconds > _packageManagerBatchModeListRequestTimeout)
                    {
                        string errorMessage = $"[Apple Unity Plug-Ins] Package manager list request exceeded {_packageManagerBatchModeListRequestTimeout} seconds.\n"
                        + "  This may be the result of the size of the project, network latency, or issues with the package cache.\n"
                        + "  Apple.Core build processing will not function correctly if this fails. Please investigate and retry.";
                        
                        Debug.LogError(errorMessage);
                        return;
                    }

                    // Prevent being blocked by tight loop.
                    Thread.Sleep(100);
                }

                if (_packageManagerListRequest.Status == StatusCode.Success)
                {
                    // No need to sync play mode support libraries in batch mode. These are used just for Play Mode within the Editor.
                    OnPackageManagerListSuccess(syncPlayModeLibraries: false);
                }
                else
                {
                    Debug.LogError($"[Apple Unity Plug-Ins] Failed query to the package manager for list of packages with status: {_packageManagerListRequest.Status}");
                }
            }
            else
            {
                if (!Directory.Exists(ApplePlugInSupportPlayModeSupportPath))
                {
                    Debug.Log($"[Apple Unity Plug-ins] Running in Editor, creating support folder: {ApplePlugInSupportPlayModeSupportPath}");
                    AssetDatabase.CreateFolder(ApplePlugInSupportEditorPath, "PlayModeSupport");
                }   
            }
        }

        /// <summary>
        /// As soon as the Unity Editor creates the <c>ApplePluginEnvironment</c>, this event handler will be added to initialize the <c>_appleUnityPackages</c> dictionary.
        /// Once initialization has occured, this method will handle runtime validation for available libraries against selected Unity Editor settings.
        /// </summary>
        /// <remarks>
        /// Note that this method does not apply when Unity is run in batch mode.
        /// Please see the section under 'Batch mode arguments' at: https://docs.unity3d.com/Manual/EditorCommandLineArguments.html
        /// </remarks>
        private static void OnEditorUpdate()
        {
            switch (_updateState)
            {
                case UpdateState.Initializing:
                    if (_packageManagerListRequest.IsCompleted && _packageManagerListRequest.Status == StatusCode.Success)
                    {
                        OnPackageManagerListSuccess(syncPlayModeLibraries: true);

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
                AppleBuildStep buildStep = _defaultProfile.FindBuildStep(applePackage.DisplayName);
                if (buildStep != null && buildStep.IsNativePlugIn)
                {
                    if (Array.IndexOf(buildStep.SupportedTargets, GetUnityBuildTarget(_trackedApplePlatform)) > -1)
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
                    else if (buildStep.IsEnabled)
                    {
                        string warningMessage = $"[Apple Unity Plug-Ins] Targeting unsupported platform '{_trackedApplePlatform}' for Apple plug-in package {applePackage.DisplayName}.\n"
                        + $"  To continue building for the current platform, please disable {applePackage.DisplayName} in the Apple Build Settings window.";

                        Debug.LogWarning(warningMessage);
                    }
                }
            }
        }

        /// <summary>
        /// Helper gets the current <c>AppleConfigID</c> based upon Editor development build settings.
        /// </summary>
        /// <returns>A tuple which contains an <c>AppleConfigID</c> string representing the principal config as well as a fallback</returns>
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

                case BuildTarget.VisionOS:
                    switch (PlayerSettings.VisionOS.sdkVersion)
                    {
                        case VisionOSSdkVersion.Device: return ApplePlatformID.visionOS;
                        case VisionOSSdkVersion.Simulator: return ApplePlatformID.VisionSimulator;
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

            if (applePlatformId == ApplePlatformID.visionOS || applePlatformId == ApplePlatformID.VisionSimulator)
            {
                return BuildTarget.VisionOS;
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
            // Ensure collection of build steps is current; package names will be validated against build step names.
            _defaultProfile.ResolveBuildSteps();

            bool packagesAdded = false;
            foreach (var unityPackage in packageCollection)
            {
                AppleBuildStep buildStep = _defaultProfile.FindBuildStep(unityPackage.displayName);

                // Apple packages with native libraries will always have a build step defined for handling those libraries, so validate here.
                if (buildStep != null && buildStep.IsNativePlugIn && buildStep.DisplayName == unityPackage.displayName && unityPackage.author.name == AppleUnityPackageAuthorName && !_appleUnityPackages.ContainsKey(unityPackage.displayName))
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
                // If there's no build step or the build step isn't associated with a native plug-in track the library-free (C# only) package.
                else if (unityPackage.name.StartsWith(AppleUnityPackageNamePrefix) && unityPackage.author.name == AppleUnityPackageAuthorName && !_appleUnityPackages.ContainsKey(unityPackage.displayName))
                {
                    AppleUnityPackage applePackage = new AppleUnityPackage(unityPackage.name, unityPackage.displayName);
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
            
            string summary = $"[Apple Unity Plug-ins] Synchronizing plug-in libraries for Editor Play Mode support to <b>{ApplePlugInSupportPlayModeSupportPath}</b>\nLibraries copied:";
            bool librariesCopied = false;

            // Copy current
            foreach (AppleUnityPackage applePackage in _appleUnityPackages.Values)
            {
                if (applePackage.PlayModeSupportLibrary.IsValid)
                {
                    librariesCopied = true;
                    AppleNativeLibrary pmsLibrary = applePackage.PlayModeSupportLibrary;
                    summary += $"\n  <b>{pmsLibrary.FileName}</b>";
                    FileUtil.CopyFileOrDirectory(pmsLibrary.FullPath, $"{UnityProjectPath}/{ApplePlugInSupportPlayModeSupportPath}/{pmsLibrary.FileName}");
                }
            }

            if (librariesCopied)
            {
                Debug.Log(summary);
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Outputs a nicely formatted summary of the currently tracked libraries to Unity's Debug logging.
        /// </summary>
        private static void LogLibrarySummary()
        {
            string summary = "[Apple Unity Plug-ins] Apple native plug-ins updated.\nTracking the following plug-in packages and native libraries:\n";
            bool librariesFound = false;
            foreach (AppleUnityPackage package in _appleUnityPackages.Values)
            {
                if (package.IsNativePackage)
                {
                    summary += $"\n<b>{package.DisplayName}</b> [{package.Name}]:\n  Package Source Path: {package.SourcePath}\n";
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
                else
                {
                    summary += $"\n<b>{package.DisplayName}</b> [{package.Name}]:\n  Non-native (C# Script or asset only) plug-in/extension.\n  <b>No libraries to list.</b>\n";
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
        /// When building Xcode projects for macOS, Unity puts everything but the project under an additional folder "/{Application.productName}" - this script will respect this folder hierarchy.<br/>
        /// Output paths will of the following form: <br/>
        ///   iOS/tvOS/visionOS: <c>[XCODE_PROJECT_DIR]/ApplePluginLibraries/[PLUGIN_NAME]/ApplePluginLibrary.suffix</c><br/>
        ///               macOS: <c>[XCODE_PROJECT_DIR]/[Application.productName]/ApplePluginLibraries/[PLUGIN_NAME]/ApplePluginLibrary.suffix</c>
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
            //  3. Build an Xcode project which targets iOS/tvOS/visionOS: [XCODE_PROJECT_DIR]/ApplePluginLibraries/[PLUGIN_NAME]/
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
#endif // #if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
