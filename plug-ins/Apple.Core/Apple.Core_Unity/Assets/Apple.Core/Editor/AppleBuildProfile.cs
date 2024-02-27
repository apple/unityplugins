#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX))
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Apple.Core
{
    /// <summary>
    /// AppleBuildProfile is used to store build settings and meta data for the Apple Unity Plug-Ins.
    /// </summary>
    [CreateAssetMenu(menuName = "Apple/Build/Apple Build Profile")]
    public class AppleBuildProfile : ScriptableObject
    {
        public static string BuildSettingsPath => ApplePlugInEnvironment.ApplePlugInSupportEditorPath;
        public static string DefaultAsset => "DefaultAppleBuildProfile.asset";
        public static string DefaultBuildSettingsAssetPath => $"{BuildSettingsPath}/{DefaultAsset}";

        public Dictionary<string, AppleBuildStep> buildSteps = new Dictionary<string, AppleBuildStep>();

        public bool AutomateInfoPlist = true;
        public UnityEngine.Object DefaultInfoPlist;

        public string MinimumOSVersion_iOS = string.Empty;
        public string MinimumOSVersion_tvOS = string.Empty;
        public string MinimumOSVersion_macOS = string.Empty;

        public bool AppUsesNonExemptEncryption = false;

        public bool AutomateEntitlements = true;
        public UnityEngine.Object DefaultEntitlements;

        /// <summary>
        /// Accesses the default build profile, creating it if one isn't available.
        /// </summary>
        public static AppleBuildProfile DefaultProfile()
        {
            AppleBuildProfile defaultProfile = null;
            var profs = Array.Empty<Object>();
            if (File.Exists(DefaultBuildSettingsAssetPath))
            {
                profs = AssetDatabase.LoadAllAssetsAtPath(DefaultBuildSettingsAssetPath);
                defaultProfile = (AppleBuildProfile)AssetDatabase.LoadMainAssetAtPath(DefaultBuildSettingsAssetPath);
            }

            if (defaultProfile is null)
            {
                Debug.Log("[AppleBuildProfile] Creating Apple Unity Plug-Ins build setting asset.");
                defaultProfile = CreateInstance<AppleBuildProfile>();

                AssetDatabase.CreateAsset(defaultProfile, DefaultBuildSettingsAssetPath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                defaultProfile.ResolveBuildSteps();
                AssetDatabase.SaveAssets();

                AssetDatabase.SetMainObject(defaultProfile, DefaultBuildSettingsAssetPath);
                AssetDatabase.ImportAsset(DefaultBuildSettingsAssetPath);
            }
            else
            {
                foreach (var p in profs)
                {
                    if (p != defaultProfile && p is AppleBuildStep)
                    {
                        defaultProfile.buildSteps[p.name] = (AppleBuildStep)p;
                    }
                }
            }

            return defaultProfile;
        }

        /// <summary>
        /// Updates the currently known list of AppleBuildSteps
        /// </summary>
        public void ResolveBuildSteps()
        {
            var buildStepTypes = AppleBuildStep.ProjectAppleBuildStepTypes();

            // Add any newly added build steps
            foreach (var buildStepType in buildStepTypes)
            {
                if (!buildSteps.ContainsKey(buildStepType.Name))
                {
                    var buildStep = (AppleBuildStep)CreateInstance(buildStepType);
                    buildStep.name = buildStepType.Name;

                    buildSteps[buildStepType.Name] = buildStep;

                    AssetDatabase.AddObjectToAsset(buildStep, this);
                }
            }

            // Remove build steps that are no longer found
            var buildStepTypeNames = buildStepTypes.Select((t) => t.Name).ToArray();
            foreach (var entry in buildSteps)
            {
                if (!buildStepTypeNames.Contains(entry.Key))
                {
                    buildSteps.Remove(entry.Key);
                    DestroyImmediate(entry.Value, true);
                }
            }
        }
    }
}
#endif // (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX))
