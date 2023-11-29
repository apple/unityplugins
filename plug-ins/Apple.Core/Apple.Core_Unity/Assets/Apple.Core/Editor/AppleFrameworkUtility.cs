#if UNITY_EDITOR_OSX
using System;
using System.IO;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;

namespace Apple.Core
{
    public static class AppleFrameworkUtility
    {
        /// <summary>Looks up the correct path for a given plug-in library name and build target.</summary>
        /// <param name="libraryName">Library name should include it's extension, such as .bundle or .framework</param>
        /// <param name="buildTarget">A Unity BuildTarget enum value. Supported targets are iOS, tvOS, or macOS</param>
        /// <return>String representing the path within the project to the library. String.Empty if not found or unsupported platform.</return>
        public static string GetPluginLibraryPathForBuildTarget(string libraryName, BuildTarget buildTarget)
        {
            string platformString;
            switch (buildTarget)
            {
                case BuildTarget.iOS:
                    platformString = "iOS";
                    break;
                case BuildTarget.tvOS:
                    platformString = "tvOS";
                    break;
                case BuildTarget.StandaloneOSX:
                    platformString = "macOS";
                    break;
                default:
                    return string.Empty;
            }

#if UNITY_2022_1_OR_NEWER
            // The asset database in Unity 2022.1 and newer will no longer find libraries when the suffix, such as '.framework' or '.bundle', is included in the asset name.
            int suffix_index = libraryName.LastIndexOf(".");
            string libraryNameStem = suffix_index == -1 ? libraryName : libraryName.Substring(0, suffix_index);

            Debug.Log($"[AppleFrameworkUtility] Unity 2022.1 or newer in use: Input library name [{libraryName}], target asset name [{libraryNameStem}]");
            string[] results = AssetDatabase.FindAssets(libraryNameStem);
#else
            string[] results = AssetDatabase.FindAssets(libraryName);
#endif
            foreach (string currGUID in results)
            {
                string libraryPath = AssetDatabase.GUIDToAssetPath(currGUID);
                string[] folders = libraryPath.Split('/');
                if (Array.IndexOf(folders, platformString) > -1)
                {
                    return libraryPath;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Wrapper around pbxProject.AddFrameworkToProject. This helper
        /// automatically finds the correct targetGuid based on settings.
        /// </summary>
        /// <param name="framework"></param>
        /// <param name="weak"></param>
        /// <param name="buildTarget"></param>
        /// <param name="pbxProject"></param>
        public static void AddFrameworkToProject(string framework, bool weak, BuildTarget buildTarget, PBXProject pbxProject)
        {

            if (buildTarget == BuildTarget.StandaloneOSX
                && !AppleBuild.IsXcodeGeneratedMac())
                return;

            var projectTargetName = buildTarget == BuildTarget.StandaloneOSX ? Application.productName : "Unity-iPhone";
            var targetGuid = buildTarget == BuildTarget.StandaloneOSX ? pbxProject.TargetGuidByName(projectTargetName) : pbxProject.GetUnityMainTargetGuid();

            pbxProject.AddFrameworkToProject(targetGuid, framework, weak);
        }

        /// <summary>
        /// Wrapper around PbxProjectExtensions.AddFileToEmbeddedFrameworks. This
        /// helper automatically finds the correct paths and targetGuids.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="buildTarget"></param>
        /// <param name="pathToBuiltProject"></param>
        /// <param name="pbxProject"></param>
        public static void CopyAndEmbed(string source, BuildTarget buildTarget, string pathToBuiltProject, PBXProject pbxProject)
        {
            Debug.Log($"CopyAndEmbed - source:{source}, build target:{buildTarget}, pbxPath:{pathToBuiltProject}");

            var frameworkName = Path.GetFileName(source);

            if (pbxProject == null)
            {
                Debug.Log($"CopyAndEmbed no pbxproject file. Not embedding {frameworkName}");
                return;
            }

            string fileGuid;

            // First check if the framework was already copied over by Unity's build system
            string searchString = string.Empty;
            if (source.Contains("Assets/"))
            {
                searchString = "Assets/";
            }
            else if (source.Contains("Packages/"))
            {
                searchString = "Packages/";
            }

            // If it wasn't added, copy it now
            if (searchString == string.Empty)
            {
                string relativeTargetCopyName;
                if (buildTarget == BuildTarget.iOS || buildTarget == BuildTarget.tvOS)
                {
                    relativeTargetCopyName = "Frameworks";
                }
                else if (buildTarget == BuildTarget.StandaloneOSX)
                {
                    if (AppleBuild.IsXcodeGeneratedMac())
                    {
                        relativeTargetCopyName = $"{Application.productName}/Frameworks";
                    }
                    else
                    {
                        relativeTargetCopyName = "Contents/PlugIns";
                    }
                }
                else
                {
                    Debug.Log("CopyAndEmbed encountered an unsupported build target.");
                    return;
                }

                // Copy the actual framework over, delete existing & meta files
                var copyBinaryPath = $"{pathToBuiltProject}/{relativeTargetCopyName}/{Path.GetFileName(source)}";
                Debug.Log($"CopyAndEmbed putting source file {source} to destination {copyBinaryPath}");
                Copy(source, copyBinaryPath);
                fileGuid = pbxProject.AddFile(Path.GetFullPath(copyBinaryPath), $"Frameworks/{frameworkName}", PBXSourceTree.Source);
            }
            // If it was copied over, just find the GUID for the existing version
            else
            {
                var expectedInstallPath = source.Substring(source.LastIndexOf(searchString) + searchString.Length);
                Debug.Log($"CopyAndEmbed - Expected install path for {frameworkName}: {expectedInstallPath}");
                fileGuid = pbxProject.FindFileGuidByProjectPath(Path.Combine("Frameworks", expectedInstallPath));
                if (string.IsNullOrEmpty(fileGuid))
                {
                    fileGuid = pbxProject.FindFileGuidByProjectPath(Path.Combine("Libraries", expectedInstallPath));
                    if (string.IsNullOrEmpty(fileGuid))
                    {
                        Debug.LogError($"CopyAndEmbed expected to find an existing GUID for {frameworkName} at {expectedInstallPath} but could not be found.");
                        return;
                    }
                }
            }


            // Now embed the framework into the main target
            var projectTargetName = buildTarget == BuildTarget.StandaloneOSX ? Application.productName : "Unity-iPhone";
            var targetGuid = buildTarget == BuildTarget.StandaloneOSX ? pbxProject.TargetGuidByName(projectTargetName) : pbxProject.GetUnityMainTargetGuid();
            Debug.Log($"CopyAndEmbed embedding {frameworkName} into target {projectTargetName}");
            PBXProjectExtensions.AddFileToEmbedFrameworks(pbxProject, targetGuid, fileGuid);
        }

        /// <summary>
        /// Copies the path from source to destination and removes any .meta files from the destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void Copy(string source, string destination)
        {
            Debug.Log($"AppleFrameworkUtility: Copying {source} to {destination}");

            // Clean up any existing unity plugins or old from previous build...
            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }

            // Copy raw from source...
            FileUtil.CopyFileOrDirectory(source, destination);


            // Recursively cleanup meta files...
            RecursiveCleanupMetaFiles(new DirectoryInfo(destination));
        }

        /// <summary>
        /// Private recursive method to remove .meta files from a directory and all of it's sub directories.
        /// </summary>
        private static void RecursiveCleanupMetaFiles(DirectoryInfo directory)
        {
            var directories = directory.GetDirectories();
            var files = directory.GetFiles();

            foreach (var file in files)
            {
                // File is a Unity meta file, clean it up...
                if (file.Extension == ".meta")
                {
                    Debug.Log($"AppleFrameworkUtility: Cleaning up meta file ({file.FullName})");
                    FileUtil.DeleteFileOrDirectory(file.FullName);
                }
            }

            // Recurse...
            foreach (var subdirectory in directories)
            {
                RecursiveCleanupMetaFiles(subdirectory);
            }
        }
    }
}
#endif