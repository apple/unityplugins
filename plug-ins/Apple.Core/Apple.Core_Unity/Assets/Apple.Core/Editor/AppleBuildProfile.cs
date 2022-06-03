using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Apple.Core
{
    [CreateAssetMenu(menuName = "Apple/Build/Apple Build Profile")]
    public class AppleBuildProfile : ScriptableObject
    {
        public const string BuildSettingsPath = "Assets/Apple.Core/Editor/";
        public const string DefaultAsset = "DefaultAppleBuildProfile.asset";
        public const string DefaultBuildSettingsAssetPath = BuildSettingsPath + DefaultAsset;

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
            if (!Directory.Exists(BuildSettingsPath))
            {
                Debug.Log($"Failed to locate path {BuildSettingsPath}. Creating");

                if (!Directory.Exists("Assets/Apple.Core/"))
                {
                    AssetDatabase.CreateFolder("Assets", "Apple.Core");
                }

                AssetDatabase.CreateFolder("Assets/Apple.Core", "Editor");
            }

            AppleBuildProfile defaultProfile = null;
            var profs = Array.Empty<Object>();
            if (File.Exists(DefaultBuildSettingsAssetPath))
            {
                profs = AssetDatabase.LoadAllAssetsAtPath(DefaultBuildSettingsAssetPath);
                defaultProfile = (AppleBuildProfile)AssetDatabase.LoadMainAssetAtPath(DefaultBuildSettingsAssetPath);
            }

            if (defaultProfile is null)
            {
                Debug.Log("Failed to find previous default build profile. Creating a new one.");
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
