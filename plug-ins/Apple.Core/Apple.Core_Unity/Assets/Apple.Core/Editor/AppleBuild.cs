#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Apple.Core
{
    public static class AppleBuild
    {   
        // Helper for logging during development.
        private static void LogDevelopmentMessage(string methodName, string message)
        {
            #if DEVELOPMENT_LOGGING_ENABLED
            Debug.Log($"[AppleBuild.{methodName}] {message}");
            #endif
        }

        /// <summary>
        /// Executes all post-build AppleBuildSteps as structured in the AppleBuildProfile.
        /// </summary>
        [PostProcessBuild(10)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string generatedProjectPath)
        {
            // TODO: Add management for multiple build profiles.
            var appleBuildProfile = AppleBuildProfile.DefaultProfile();

            #region Begin Post Process

            LogDevelopmentMessage("OnPostProcessBuild", "OnBeginPostProcess begin");
            LogDevelopmentMessage("OnPostProcessBuild", $"Found {appleBuildProfile.buildSteps.Count} build steps.");
            LogDevelopmentMessage("OnPostProcessBuild", $"Outputting to project at path {generatedProjectPath}");

            var processedBuildSteps = new Dictionary<string, bool>();
            foreach (var buildStep in appleBuildProfile.buildSteps)
            {
                if (buildStep.Value.IsEnabled)
                {
                    LogDevelopmentMessage("OnPostProcessBuild", $"OnBeginPostProcess for step: {buildStep.Key}");
                    buildStep.Value.OnBeginPostProcess(appleBuildProfile, buildTarget, generatedProjectPath);

                    processedBuildSteps[buildStep.Key] = true;
                }
                else
                {
                    LogDevelopmentMessage("OnPostProcessBuild", $"Build post process disabled for build step: {buildStep.Key}");
                    processedBuildSteps[buildStep.Key] = false;
                }
            }

            #endregion // Begin Post Process

            #region Process info.plist

            if (appleBuildProfile.AutomateInfoPlist)
            {
                LogDevelopmentMessage("OnPostProcessBuild", "OnProcessInfoPlist begin");

                var infoPlist = new PlistDocument();
                var infoPlistPath = GetInfoPlistPath(buildTarget, generatedProjectPath);
                infoPlist.ReadFromFile(infoPlistPath);

                // Required property which notifies Apple about 3rd party encryption...
                infoPlist.root.SetBoolean("ITSAppUsesNonExemptEncryption", appleBuildProfile.AppUsesNonExemptEncryption);

                string minOSVersionString = string.Empty;
                switch (buildTarget)
                {
                    case BuildTarget.iOS:
                        minOSVersionString = appleBuildProfile.MinimumOSVersion_iOS;
                        break;

                    case BuildTarget.tvOS:
                        minOSVersionString = appleBuildProfile.MinimumOSVersion_tvOS;
                        break;

                    case BuildTarget.StandaloneOSX:
                        minOSVersionString = appleBuildProfile.MinimumOSVersion_macOS;
                        break;

                    case BuildTarget.VisionOS:
                        minOSVersionString = appleBuildProfile.MinimumOSVersion_visionOS;
                        break;

                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(minOSVersionString))
                {
                    infoPlist.root.SetString("LSMinimumSystemVersion", minOSVersionString);
                }

                // Ensure we update keys with our default plist...
                if (appleBuildProfile.DefaultInfoPlist != null)
                {
                    var defaultPlist = new PlistDocument();
                    defaultPlist.ReadFromFile(AssetDatabase.GetAssetPath(appleBuildProfile.DefaultInfoPlist));

                    foreach (var pair in defaultPlist.root.values)
                    {
                        LogDevelopmentMessage("OnPostProcessBuild", $"Setting InfoPlist [{pair.Key}] to [{pair.Value}].");
                        infoPlist.root[pair.Key] = pair.Value;
                    }
                }

                foreach (var buildStep in appleBuildProfile.buildSteps)
                {
                    if (buildStep.Value.IsEnabled)
                    {
                        LogDevelopmentMessage("OnPostProcessBuild", $"OnProcessInfoPlist for step: {buildStep.Key}");
                        buildStep.Value.OnProcessInfoPlist(appleBuildProfile, buildTarget, generatedProjectPath, infoPlist);
                    }
                }

                infoPlist.WriteToFile(infoPlistPath);
            }

            #endregion // Process info.plist

            #region Process Entitlements

            var pbxProject = GetPbxProject(buildTarget, generatedProjectPath);
            var pbxProjectPath = GetPbxProjectPath(buildTarget, generatedProjectPath);

            if (appleBuildProfile.AutomateEntitlements)
            {
                LogDevelopmentMessage("OnPostProcessBuild", "OnProcessEntitlements begin");

                var entitlementsPath = GetEntitlementsPath(buildTarget, generatedProjectPath);
                var entitlements = new PlistDocument();

                if (File.Exists(entitlementsPath))
                {
                    entitlements.ReadFromFile(entitlementsPath);
                }
                else
                {
                    entitlements.Create();
                }

                // Ensure we update keys with our default entitlements
                if (appleBuildProfile.DefaultEntitlements != null)
                {
                    var defaultPlist = new PlistDocument();
                    defaultPlist.ReadFromFile(AssetDatabase.GetAssetPath(appleBuildProfile.DefaultEntitlements));

                    foreach (var pair in defaultPlist.root.values)
                    {
                        LogDevelopmentMessage("OnPostProcessBuild", $"Setting Entitlements [{pair.Key}] to [{pair.Value}].");
                        entitlements.root[pair.Key] = pair.Value;
                    }
                }

                foreach (var buildStep in appleBuildProfile.buildSteps)
                {
                    if (buildStep.Value.IsEnabled)
                    {
                        LogDevelopmentMessage("OnPostProcessBuild", $"OnProcessEntitlements for step: {buildStep.Key}");
                        buildStep.Value.OnProcessEntitlements(appleBuildProfile, buildTarget, generatedProjectPath, entitlements);
                    }
                }

                entitlements.WriteToFile(entitlementsPath);

                FixEntitlementFormatting(entitlementsPath);

                if (pbxProject != null)
                {
                    var entitlementsXCodePath = buildTarget == BuildTarget.StandaloneOSX ? $"{Application.productName}/{Application.productName}.entitlements" : $"{Application.productName}.entitlements";
                    var targetGuid = buildTarget == BuildTarget.StandaloneOSX ? pbxProject.TargetGuidByName(Application.productName) : pbxProject.GetUnityMainTargetGuid();
                    pbxProject.AddBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", entitlementsXCodePath);

                    LogDevelopmentMessage("OnPostProcessBuild", $"Writing changes to PBXProject {pbxProjectPath}");
                    pbxProject.WriteToFile(pbxProjectPath);
                }
            }

            #endregion // Process Entitlements

            #region Process Frameworks

            LogDevelopmentMessage("OnPostProcessBuild", $"OnProcessFrameworks begin");

            foreach (var buildStep in appleBuildProfile.buildSteps)
            {
                if (buildStep.Value.IsEnabled)
                {
                    LogDevelopmentMessage("OnPostProcessBuild", $"OnProcessFrameworks for step: {buildStep.Key}");
                    buildStep.Value.OnProcessFrameworks(appleBuildProfile, buildTarget, generatedProjectPath, pbxProject);
                }
            }

            if (pbxProject != null)
            {
                string projectRelativeNativeLibraryRoot = AppleNativeLibraryUtility.GetDestinationNativeLibraryFolderRoot(buildTarget);
                pbxProject.AddShellScriptBuildPhase(pbxProject.GetUnityMainTargetGuid(), "Embed Apple Plug-in Libraries", "/bin/sh", GenerateEmbedNativeLibraryShellScript(projectRelativeNativeLibraryRoot));
                pbxProject.WriteToFile(pbxProjectPath);
            }

            #endregion // Process Frameworks

            #region Finalize Post Process

            LogDevelopmentMessage("OnPostProcessBuild", "OnFinalizePostProcess begin");

            foreach (var buildStep in appleBuildProfile.buildSteps)
            {
                if (buildStep.Value.IsEnabled)
                {
                    LogDevelopmentMessage("OnPostProcessBuild", $"OnFinalizePostProcess for step: {buildStep.Key}");
                    buildStep.Value.OnFinalizePostProcess(appleBuildProfile, buildTarget, generatedProjectPath);
                }
            }
            #endregion // Finalize Post Process

            // TODO: (122199654) Enhance build summary
            string summaryMessage = "[Apple Unity Plug-ins] Build post process complete.\n"
            + $"Built for config: {(ApplePlugInEnvironment.IsDevelopmentBuild ? "Debug" : "Release")}\n"
            + "Processed the following:\n";

            foreach (var key in processedBuildSteps.Keys)
            {
                string currStatus = processedBuildSteps[key] ? "Enabled" : "Disabled";
                summaryMessage += $"- {key}: {currStatus}\n";
            }

            Debug.Log(summaryMessage);
        }

        /// <summary>
        /// Unity's plist modification often adds spaces which cause issue with formatting with the entitlements file.
        /// </summary>
        /// <param name="entitlementsPath"></param>
        private static void FixEntitlementFormatting(string entitlementsPath)
        {
            if (!File.Exists(entitlementsPath))
                return;

            LogDevelopmentMessage("FixEntitlementFormatting", "Fixing entitlements formatting begin");

            var contents = File.ReadAllText(entitlementsPath);

            // We replace any <tag /> with a space and remove the space...
            var matches = Regex.Matches(contents, "<(.*)\\s/>");

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    contents = contents.Replace(match.Groups[0].Value, $"<{match.Groups[1].Value}/>");
                }
            }

            File.WriteAllText(entitlementsPath, contents);
        }

        /// <summary>
        /// Returns the path to the info.plist for a given build target and project
        /// </summary>
        public static string GetInfoPlistPath(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget == BuildTarget.StandaloneOSX)
            {
                if (AppleNativeLibraryUtility.IsXcodeProjectGeneratedForMac)
                {
#if UNITY_2020_1_OR_NEWER
                    return $"{pathToBuiltProject}/{Application.productName}/Info.plist";
#else
                    return $"{Path.GetDirectoryName(pathToBuiltProject)}/{Application.productName}/Info.plist";
#endif
                }
                else
                {
                    return $"{pathToBuiltProject}/Contents/Info.plist";
                }
            }
            else
            {
                return $"{pathToBuiltProject}/Info.plist";
            }
        }

        /// <summary>
        /// Returns the path to the entitlements for a given build target and project
        /// </summary>
        public static string GetEntitlementsPath(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget == BuildTarget.StandaloneOSX)
            {
                if (AppleNativeLibraryUtility.IsXcodeProjectGeneratedForMac)
                {
#if UNITY_2020_1_OR_NEWER
                    return $"{pathToBuiltProject}/{Application.productName}/{Application.productName}.entitlements";
#else
                    return $"{Path.GetDirectoryName(pathToBuiltProject)}/{Application.productName}/{Application.productName}.entitlements";
#endif
                }
                else
                {
                    return $"{Path.GetDirectoryName(pathToBuiltProject)}/{Application.productName}.entitlements";
                }
            }
            else
            {
                return $"{pathToBuiltProject}/{Application.productName}.entitlements";
            }
        }

        /// <summary>
        /// Utility method for getting the .pbxproj path for a built project
        /// </summary>
        public static string GetPbxProjectPath(BuildTarget buildTarget, string pathToBuiltProject)
        {
            switch (buildTarget)
            {
                case BuildTarget.iOS:
                case BuildTarget.tvOS:
                    return $"{pathToBuiltProject}/Unity-iPhone.xcodeproj/project.pbxproj";
                case BuildTarget.VisionOS:
                    return $"{pathToBuiltProject}/Unity-VisionOS.xcodeproj/project.pbxproj";
                case BuildTarget.StandaloneOSX:
#if UNITY_2020_1_OR_NEWER
                    return $"{pathToBuiltProject}/{new DirectoryInfo(pathToBuiltProject).Name}.xcodeproj/project.pbxproj";
#else
                    return $"{pathToBuiltProject}/project.pbxproj";
#endif
                default:
                    return null;
            }
        }

        /// <summary>
        /// Utility method for getting a PBXProject object associated with the provided BuildTarget and built project path
        /// </summary>
        public static PBXProject GetPbxProject(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget == BuildTarget.StandaloneOSX && !AppleNativeLibraryUtility.IsXcodeProjectGeneratedForMac)
            {
                return null;
            }

            var proj = new PBXProject();
            proj.ReadFromFile(GetPbxProjectPath(buildTarget, pathToBuiltProject));

            return proj;
        }

        /// <summary>
        /// Allows all AppleBuildSteps in the project to optionally respond to updated exportPlistOptions when called from another build step
        /// </summary>
        public static void ProcessExportPlistOptions(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltProject, PlistDocument exportPlistOptions)
        {
            LogDevelopmentMessage("ProcessExportPlistOptions", "ProcessExportPlistOptions begin");

            foreach (var buildStep in appleBuildProfile.buildSteps)
            {
                if (buildStep.Value.IsEnabled)
                {
                    buildStep.Value.OnProcessExportPlistOptions(appleBuildProfile, buildTarget, pathToBuiltProject, exportPlistOptions);
                }
            }
        }

        /// <summary>
        /// Generates a shell script that is added to the trampoline project as a post-build 'Run Script' phase.
        /// This script embeds linked Apple native plug-in libraries into the final build product.
        /// </summary>
        /// <param name="projectRelativeLibraryPath">The target native library path defined relatively to</param>
        /// <returns></returns>
        private static string GenerateEmbedNativeLibraryShellScript(string projectRelativeNativeLibraryRoot)
        {
            string embedNativeLibraryShellScript = "#  Apple Unity Plug-in Sign & Embed libraries shell script\n"
            + "#  Copyright © 2024 Apple, Inc. All rights reserved.\n"
            + "#  This script is added to the generated Xcode project by the Apple.Core plug-in.\n"
            + "#  Please see AppleBuild.cs in the Apple.Core plug-in for more information.\n"
            + "dstFrameworkFolder=\"$BUILT_PRODUCTS_DIR/$FRAMEWORKS_FOLDER_PATH\"\n"
            + "dstBundleFolder=\"$BUILT_PRODUCTS_DIR/$PLUGINS_FOLDER_PATH\"\n"
            + $"APPLE_PLUGIN_LIBRARY_ROOT=\"$PROJECT_DIR/{projectRelativeNativeLibraryRoot}\"\n"
            + "if [ -d $APPLE_PLUGIN_LIBRARY_ROOT ]; then\n"
            + "    for folder in \"$APPLE_PLUGIN_LIBRARY_ROOT\"/*; do\n"
            + "        if [ -d \"$folder\" ]; then\n"
            + "            for item in \"$folder\"/*; do\n"
            + "                if [[ $item = *'.dSYM' ]]; then\n"
            + "                    continue\n"
            + "                elif [[ $item = *'.framework' ]]; then\n"
            + "                    filename=$(basename $item)\n"
            + "                    echo \"    Embedding Apple plug-in framework $filename\"\n"
            + "                    echo \"      Source: $item\"\n"
            + "                    echo \"      Destination: $dstFrameworkFolder/$filename\"\n"
            + "                    if [ ! -z \"$EXPANDED_CODE_SIGN_IDENTITY\" ]; then\n"
            + "                        echo \"      Code signing identity: $EXPANDED_CODE_SIGN_IDENTITY\"\n"
            + "                        codesign --force --sign $EXPANDED_CODE_SIGN_IDENTITY --timestamp\\=none --generate-entitlement-der $item\n"
            + "                    fi\n"
            + "                    ditto $item \"$dstFrameworkFolder/$filename\"\n"
            + "                    break\n"
            + "                elif [[ $item = *'.bundle' ]]; then\n"
            + "                    filename=$(basename $item)\n"
            + "                    echo \"    Embedding Apple plug-in bundle $filename\"\n"
            + "                    echo \"      Source: $item\"\n"
            + "                    echo \"      Destination: $dstBundleFolder/$filename\"\n"
            + "                    if [ ! -z \"$EXPANDED_CODE_SIGN_IDENTITY\" ]; then\n"
            + "                        echo \"      Code signing identity: $EXPANDED_CODE_SIGN_IDENTITY\"\n"
            + "                        codesign --force --sign $EXPANDED_CODE_SIGN_IDENTITY --timestamp\\=none --generate-entitlement-der $item\n"
            + "                    fi\n"
            + "                    ditto $item \"$dstBundleFolder/$filename\"\n"
            + "                    break\n"
            + "                fi\n"
            + "            done\n"
            + "        fi\n"
            + "    done\n"
            + "    echo \"Completed search of libraries in the Apple native plug-in folder root.\"\n"
            + "else\n"
            + "    echo \"No Apple plug-in library path found at $APPLE_PLUGIN_LIBRARY_ROOT\"\n"
            + "    echo \"Please build a Development Build in Unity for this script to log more information.\"\n"
            + "    exit 1\n"
            + "fi";

            string debugEmbedNativeLibraryShellScript = "#  Apple Unity Plug-in Sign & Embed libraries shell script\n"
            + "#  Copyright © 2024 Apple, Inc. All rights reserved.\n"
            + "#  This script is added to the generated Xcode project by the Apple.Core plug-in.\n"
            + "#  Please see AppleBuild.cs in the Apple.Core plug-in for more information.\n"
            + "echo \"Debug Apple Unity Plug-in Embed libraries shell script: enhanced output\"\n"
            + "echo \"***********************************************************************\"\n"
            + "dstFrameworkFolder=\"$BUILT_PRODUCTS_DIR/$FRAMEWORKS_FOLDER_PATH\"\n"
            + "dstBundleFolder=\"$BUILT_PRODUCTS_DIR/$PLUGINS_FOLDER_PATH\"\n"
            + "echo \"Embed framework destination folder: $dstFrameworkFolder\"\n"
            + "echo \"Embed bundle destination folder: $dstBundleFolder\"\n"
            + $"APPLE_PLUGIN_LIBRARY_ROOT=\"$PROJECT_DIR/{projectRelativeNativeLibraryRoot}\"\n"
            + "if [ -d $APPLE_PLUGIN_LIBRARY_ROOT ]; then\n"
            + "    echo \"Found Apple plug-in native library root: $APPLE_PLUGIN_LIBRARY_ROOT\"\n"
            + "    echo \"Iterating through contents.\"\n"
            + "    for folder in \"$APPLE_PLUGIN_LIBRARY_ROOT\"/*; do\n"
            + "        if [ -d \"$folder\" ]; then\n"
            + "            echo \"  Discovered potential plug-in library folder: $folder\"\n"
            + "            echo \"  Iterating through contents.\"\n"
            + "            for item in \"$folder\"/*; do\n"
            + "                echo \"    Found item: $item\"\n"
            + "                echo \"    Checking to see if it's a .framework, if so will embed in destination folder.\"\n"
            + "                if [[ $item = *'.dSYM' ]]; then\n"
            + "                    echo \"    Debug symbol (.dSYM) file found. Continuing.\"\n"
            + "                    continue\n"
            + "                elif [[ $item = *'.framework' ]]; then\n"
            + "                    filename=$(basename $item)\n"
            + "                    echo \"    Embedding Apple plug-in framework $filename\"\n"
            + "                    echo \"      Source: $item\"\n"
            + "                    echo \"      Destination: $dstFrameworkFolder/$filename\"\n"
            + "                    if [ ! -z \"$EXPANDED_CODE_SIGN_IDENTITY\" ]; then\n"
            + "                        echo \"      Code signing identity: $EXPANDED_CODE_SIGN_IDENTITY\"\n"
            + "                        codesign --force --sign $EXPANDED_CODE_SIGN_IDENTITY --timestamp\\=none --generate-entitlement-der $item\n"
            + "                    fi\n"
            + "                    ditto $item \"$dstFrameworkFolder/$filename\"\n"
            + "                    break\n"
            + "                elif [[ $item = *'.bundle' ]]; then\n"
            + "                    filename=$(basename $item)\n"
            + "                    echo \"    Embedding Apple plug-in bundle $filename\"\n"
            + "                    echo \"      Source: $item\"\n"
            + "                    echo \"      Destination: $dstBundleFolder/$filename\"\n"
            + "                    if [ ! -z \"$EXPANDED_CODE_SIGN_IDENTITY\" ]; then\n"
            + "                        echo \"      Code signing identity: $EXPANDED_CODE_SIGN_IDENTITY\"\n"
            + "                        codesign --force --sign $EXPANDED_CODE_SIGN_IDENTITY --timestamp\\=none --generate-entitlement-der $item\n"
            + "                    fi\n"
            + "                    ditto $item \"$dstBundleFolder/$filename\"\n"
            + "                    break\n"
            + "                fi\n"
            + "            done\n"
            + "        fi\n"
            + "    done\n"
            + "    echo \"Completed search of libraries in the Apple native plug-in folder root.\"\n"
            + "    echo \"If this processing completed with missing or unexpected libraries, please check the following:\"\n"
            + "    echo \" - Ensure your Unity project only includes the Apple Unity Plug-ins that are necessary for your project.\"\n"
            + "    echo \" - Examine Unity console output for build errors.\"\n"
            + "    echo \" - Check the output of the build script (build.py) if you used the script to build and pack the Apple Unity Plug-ins.\"\n"
            + "    echo \" - If you built the native libraries with Xcode or xcodebuild, please ensure that the libraries compiled successfully and were copied into the correct plug-in folders.\"\n"
            + "else\n"
            + "    echo \"No Apple plug-in library path found at $APPLE_PLUGIN_LIBRARY_ROOT\"\n"
            + "    echo \"Please check the Unity Editor build output and/or logs for issues and errors.\"\n"
            + "    exit 1\n"
            + "fi";

            return ApplePlugInEnvironment.IsDevelopmentBuild ? debugEmbedNativeLibraryShellScript : embedNativeLibraryShellScript;
        }
    }
}
#endif // (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
