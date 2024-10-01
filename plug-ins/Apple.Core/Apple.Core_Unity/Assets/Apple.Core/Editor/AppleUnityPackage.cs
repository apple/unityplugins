#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Apple.Core
{
    /// <summary>
    /// Simple representation of an Apple plug-in package, used to store look-up information for a given plug-in's libraries.
    /// </summary>
    public class AppleUnityPackage
    {
        /// <summary>
        /// This constructor will interrogate the file system in order to fill out a table of associated native libraries for the package.
        /// </summary>
        /// <param name="name">Matches the 'name' field of the associated package's <c>package.json</c> file. Should be of the form <c>com.apple.unityplugin.XXX</c>.</param>
        /// <param name="displayName">Matches the 'displayName' field of the associated package's <c>package.json</c> file</param>
        /// <param name="packageSystemPath">The local file system path to the plug-in as provided by the Unity package manager.</param>
        public AppleUnityPackage(string name, string displayName, string packageSystemPath)
        {
            Name = name;
            DisplayName = displayName;
            PlayModeSupportLibrary = AppleNativeLibrary.Invalid;
            SourcePath = packageSystemPath;
            IsNativePackage = true;

            _nativeLibraryCollection = new Dictionary<string, Dictionary<string, AppleNativeLibrary>>();

            if (Directory.Exists(packageSystemPath))
            {
                string[] nativeLibraryRootPaths = Directory.GetDirectories(packageSystemPath, AppleNativeLibraryUtility.SourceNativeLibraryFolderName, SearchOption.AllDirectories);
                if (nativeLibraryRootPaths.Length == 1)
                {
                    // Verfied that the provided path has a "NativeLibraries~" folder
                    string[] configPaths = Directory.GetDirectories(nativeLibraryRootPaths[0]);
                    if (configPaths.Length > 0)
                    {
                        foreach (string currConfigPath in configPaths)
                        {
                            string configFolderName = new DirectoryInfo(currConfigPath).Name;
                            if (configFolderName == AppleConfigID.Release || configFolderName == AppleConfigID.Debug)
                            {
                                // Verified a valid config, create an inner dictionary for this config
                                _nativeLibraryCollection[configFolderName] = new Dictionary<string, AppleNativeLibrary>();
                                string[] platformPaths = Directory.GetDirectories(currConfigPath);
                                if (platformPaths.Length > 0)
                                {
                                    foreach (string currPlatformPath in platformPaths)
                                    {
                                        string platformId = new DirectoryInfo(currPlatformPath).Name;
                                        if (Array.IndexOf(ApplePlatformID.ValidPlatforms, platformId) >= 0)
                                        {
                                            // Valid platform folder found, try to read necessary data to fill out an AppleNativeLibrary object.
                                            string libraryName = string.Empty;
                                            string dSymName = string.Empty;
                                            string libraryPath = string.Empty;

                                            string[] entryPaths = Directory.GetFileSystemEntries(currPlatformPath);
                                            foreach (string path in entryPaths)
                                            {
                                                if (path.EndsWith(".framework") || path.EndsWith(".bundle"))
                                                {
                                                    var dirInfo = new DirectoryInfo(path);
                                                    libraryName = dirInfo.Name;
                                                    libraryPath = dirInfo.Parent.FullName;
                                                }
                                                else if (path.EndsWith(".a"))
                                                {
                                                    var fileInfo = new FileInfo(path);
                                                    libraryName = fileInfo.Name;
                                                    libraryPath = fileInfo.Directory.FullName;
                                                }
                                                else if (path.EndsWith(".dSYM"))
                                                {
                                                    dSymName = new FileInfo(path).Name;
                                                }
                                            }

                                            if (libraryName != string.Empty && libraryPath != string.Empty)
                                            {
                                                _nativeLibraryCollection[configFolderName][platformId] = new AppleNativeLibrary(libraryName, dSymName, platformId, libraryPath);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Try to find a valid native library for Play mode support in the Editor
                    PlayModeSupportLibrary = GetLibrary(AppleConfigID.Release, ApplePlatformID.macOS);
                    if (!PlayModeSupportLibrary.IsValid)
                    {
                        PlayModeSupportLibrary = GetLibrary(AppleConfigID.Debug, ApplePlatformID.macOS);
                    }
                }
            }
        }

        /// <summary>
        /// Create a package with default values to act as a representation of a package without native libraries.
        /// </summary>
        /// <param name="name">Matches the 'name' field of the associated package's <c>package.json</c> file. Should be of the form <c>com.apple.unityplugin.XXX</c>.</param>
        /// <param name="displayName">Matches the 'displayName' field of the associated package's <c>package.json</c> file</param>
        public AppleUnityPackage(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
            IsNativePackage = false;

            PlayModeSupportLibrary = AppleNativeLibrary.Invalid;
            SourcePath = string.Empty;
            _nativeLibraryCollection = new Dictionary<string, Dictionary<string, AppleNativeLibrary>>();
        }

        /// <summary>
        /// Matches the 'name' field of the associated package's <c>package.json</c> file.
        /// </summary>
        /// <remarks>
        ///  Should be of the form: <c>com.apple.unityplugin.XXX</c>
        /// </remarks>
        public string Name { get; private set; }

        /// <summary>
        /// Matches the 'displayName' field of the associated package's <c>package.json</c> file and, by standard, is of the form "Apple.ShortName"
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Standard prefix for the display names of all Apple Unity Packages.
        /// </summary>
        public string ApplePackageDisplayNamePrefix => "Apple.";

        /// <summary>
        /// A helper property which fetches the DisplayName without the "Apple." prefix.
        /// </summary>
        public string ShortName => DisplayName[ApplePackageDisplayNamePrefix.Length..];
        
        /// <summary>
        /// The filename of the library used for the Unity Editor's Play mode. This property returns string.Empty when no file is available.
        /// </summary>
        public AppleNativeLibrary PlayModeSupportLibrary { get; private set; }

        /// <summary>
        /// Records the source of the package in the local file system.
        /// </summary>
        public string SourcePath { get; private set; }

        /// <summary>
        /// True when this package references native libraries.
        /// </summary>
        public bool IsNativePackage { get; private set; }
    
        /// <summary>
        /// Helper will get an AppleNativeLibrary for the provided config and platform combination, returning an invalid AppleNativeLibrary if none exists.
        /// </summary>
        /// <param name="appleConfig">The AppleConfigID of the desired library.</param>
        /// <param name="applePlatform">The ApplePlatformID of the desired library.</param>
        /// <returns></returns>
        public AppleNativeLibrary GetLibrary(string appleConfig, string applePlatform)
        {
            if (_nativeLibraryCollection.ContainsKey(appleConfig) && _nativeLibraryCollection[appleConfig].ContainsKey(applePlatform))
            {
                return _nativeLibraryCollection[appleConfig][applePlatform];
            }
            else return AppleNativeLibrary.Invalid;
        }

        /// <summary>
        /// Returns an array of all libraries for a given config. Returns all libraries for all tracked configs if appleConfig is unrecognized or string.Empty
        /// </summary>
        /// <param name="appleConfig">The desired AppleConfigID for the set of libraries to gather. Pass string.Empty for all tracked libraries.</param>
        /// <returns>The flattened array of AppleNativeLibrary elements for this package.</returns>
        public AppleNativeLibrary[] GetLibraries(string appleConfig = "")
        {
            if (!IsNativePackage)
            {
                return Array.Empty<AppleNativeLibrary>();
            }

            List<AppleNativeLibrary> libraries = new List<AppleNativeLibrary>();
            if (appleConfig == AppleConfigID.Debug || appleConfig == AppleConfigID.Release)
            {
                if (_nativeLibraryCollection.ContainsKey(appleConfig))
                {
                    var libraryTable = _nativeLibraryCollection[appleConfig];
                    foreach (var entry in libraryTable)
                    {
                        libraries.Add(entry.Value);
                    }

                    return libraries.ToArray();
                }
            }
            else
            {
                foreach (var libraryTable in _nativeLibraryCollection)
                {
                    foreach (var entry in libraryTable.Value)
                    {
                        libraries.Add(entry.Value);
                    }
                }

                return libraries.ToArray();
            }

            return Array.Empty<AppleNativeLibrary>();
        }

        /// <summary>
        /// Maps a collection of platforms to associated AppleNativeLibrary objects and a collection of configs to the collection of platforms.
        /// </summary>
        private Dictionary<string, Dictionary<string, AppleNativeLibrary>> _nativeLibraryCollection;
    }
}
#endif // (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
