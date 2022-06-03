using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.iOS.Xcode;

/// <summary>
/// Custom asset processor ensures that compiled libraries are configured with the appropriate platform support settings
/// By default, debug messages for AppleNativePluginProcessor are only logged for development builds
/// To always force debug output set ForceLogging to 'true'
/// To process libraries in imported packages, set ProcessPackageLibraries to 'true' (This setting is usually only helpful during plug-in development)
/// </summary>
public class AppleNativePluginProcessor : AssetPostprocessor
{
    // TODO: Figure out if possible to expose as settings in Apple Build Settings pane of Project Settings window
    const bool ForceLogging = false;
    const bool ProcessPackageLibraries = false;

    const string BundleIDKey = "CFBundleIdentifier";
    const string ApplePlugInBundleIDPrefix = "com.apple-plugins.";

    // Helper to format debug log output
    static string Tabs(int i)
    {
        const int tabSpaces = 4;
        return new string(' ', tabSpaces * i);
    }

    /// <summary>
    /// Iterates through all assets, checking for files ending in .framework or .bundle
    /// If either are located, the associated Info.plist will then be interrogated for it's Bundle ID
    /// When the library's bundle ID begins with 'com.apple-plugins.':
    /// - Library is assumed to be an Apple plug-in library
    /// - PluginImporter settings will be updated to ensure correct platform support
    /// </summary>
    static void OnPostprocessAllAssets(string[] assetPaths, string[] _, string[] _1, string[] _2)
    {
        // To keep console messages to a minimum, output debug logs as a single Debug.Log() message summarazing the entire post-process step.
        LinkedList<string> logMessages = new LinkedList<string>();
    
        uint processedCount = 0;
        foreach (string currAssetPath in assetPaths)
        {
            if (!ProcessPackageLibraries && currAssetPath.StartsWith("Packages"))
                continue;

            if (currAssetPath.EndsWith(".framework") || currAssetPath.EndsWith(".bundle"))
            {
                ++processedCount;
                logMessages.AddLast($"\n{Tabs(1)}Found library at {currAssetPath}"
                                  + $"\n{Tabs(2)}Attempting to identify as an Apple plug-in library.");

                // Look up all the Info.plist files
                string[] infoPlistPaths = Directory.GetFiles(currAssetPath, "Info.plist", SearchOption.AllDirectories);

                if (infoPlistPaths.Length < 1)
                {
                    logMessages.AddLast($"{Tabs(2)}Unable to determine if located library is an Apple plug-in library:"
                                    + $"\n{Tabs(3)}Failed to locate Info.plist within: {currAssetPath}");
                    continue;
                }
                else if (infoPlistPaths.Length > 1)
                {
                    logMessages.AddLast($"{Tabs(2)}Found multiple Info.plist files within: {currAssetPath}"
                                    + $"\n{Tabs(3)}Reading first found at {infoPlistPaths[0]}");
                }

                PlistDocument infoPlist = new PlistDocument();

                // Attempts to read binary plists will throw an exception.
                try
                {
                    infoPlist.ReadFromFile(infoPlistPaths[0]);
                }
                catch (Exception e)
                {
                    
                    logMessages.AddLast($"{Tabs(2)}Unable to determine if located library is an Apple plug-in library:"
                                    + $"\n{Tabs(3)}Failed to read Info.plist at {infoPlistPaths[0]}"
                                    + $"\n{Tabs(3)}{e.GetType()} exception thrown with message: {e.Message}"
                                    + $"\n{Tabs(3)}**Note that Apple plug-in libraries must have an Info.plist encoded as XML!**");
                    continue;
                }

                // Check the Info.plist's bundle ID entry for ApplePlugInBundleIDPrefix
                bool isAppleLibrary = false;
                if (infoPlist.root.values.ContainsKey(BundleIDKey))
                {
                    // Call to AsString() will throw an exception if it fails.
                    try
                    {
                        string bundleID = infoPlist.root.values[BundleIDKey].AsString();
                        if (bundleID.StartsWith(ApplePlugInBundleIDPrefix))
                        {
                            logMessages.AddLast($"{Tabs(2)}Bundle ID '{bundleID}' found!");
                            isAppleLibrary = true;
                        }
                        else
                        {
                            logMessages.AddLast($"{Tabs(2)}Not an Apple plug-in library:"
                                            + $"\n{Tabs(3)}Expected a bundle ID beginning with '{ApplePlugInBundleIDPrefix}'"
                                            + $"\n{Tabs(3)}Found bundle ID '{bundleID}'");
                        }
                    }
                    catch (Exception e)
                    {
                        logMessages.AddLast($"{Tabs(2)}Unable to determine if library is an Apple plug-in library:"
                                        + $"\n{Tabs(3)}{e.GetType()} exception thrown with message: {e.Message}"
                                        + $"\n{Tabs(3)}Failed to read Info.plist entry for '{BundleIDKey}'");
                        continue;
                    }
                }

                // Configure Unity's PluginImporter with the correct settings based upon platform folder
                if (isAppleLibrary)
                {
                    string[] folders = currAssetPath.Split('/');
                    if (Array.IndexOf(folders, "iOS") > -1)
                    {
                        logMessages.AddLast($"{Tabs(2)}Updating Apple plug-in library metadata: Set compatible with iOS, incompatible with all other platforms.");
                        
                        PluginImporter importer = AssetImporter.GetAtPath(currAssetPath) as PluginImporter;
                        importer.SetCompatibleWithEditor(false);
                        importer.SetCompatibleWithAnyPlatform(false);
                        importer.SetCompatibleWithPlatform(BuildTarget.iOS, true);
                        importer.SetCompatibleWithPlatform(BuildTarget.tvOS, false);
                        importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX, false);
                    }
                    else if (Array.IndexOf(folders, "tvOS") > -1)
                    {
                        logMessages.AddLast($"{Tabs(2)}Updating Apple plug-in library metadata: Set compatible with tvOS, incompatible with all other platforms.");

                        PluginImporter importer = AssetImporter.GetAtPath(currAssetPath) as PluginImporter;
                        importer.SetCompatibleWithEditor(false);
                        importer.SetCompatibleWithAnyPlatform(false);
                        importer.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                        importer.SetCompatibleWithPlatform(BuildTarget.tvOS, true);
                        importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX, false);
                    }
                    else if (Array.IndexOf(folders, "macOS") > -1)
                    {
                        logMessages.AddLast($"{Tabs(2)}Updating Apple plug-in library metadata: Set compatible with macOS and Unity Editor, incompatible with all other platforms.");

                        PluginImporter importer = AssetImporter.GetAtPath(currAssetPath) as PluginImporter;
                        importer.SetCompatibleWithEditor(true);
                        importer.SetCompatibleWithAnyPlatform(false);
                        importer.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                        importer.SetCompatibleWithPlatform(BuildTarget.tvOS, false);
                        importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX, true);
                    }
                }
            }
        }

        // No need to log anything if no work was done.
        if (processedCount == 0)
            return;

        logMessages.AddFirst("[AppleNativePluginProcessor] Running asset post-process for Apple plug-in libraries.\nDebug log:");
        logMessages.AddLast("\n[AppleNativePluginProcessor] Finished asset post-process for Apple plug-in libraries.\n\n");

        if (Debug.isDebugBuild || ForceLogging)
        {
            // Output cached log messages as a single log.
            string debugOutput = String.Join("\n", logMessages);
            Debug.Log(debugOutput);
        }
        else
        {
            Debug.Log("[AppleNativePluginProcessor] Finished asset post-process for Apple plug-in libraries."
                + $"\n{Tabs(1)}Debug messages suppressed. For more information, please see: AppleNativePluginProcessor.cs");
        }
    }
}
