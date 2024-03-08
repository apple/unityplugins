using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX))
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.Core
{
    public class AppleCoreBuildStep : AppleBuildStep
    {
        public override string DisplayName => "Apple.Core";
        public override BuildTarget[] SupportedTargets => new BuildTarget[] {BuildTarget.iOS, BuildTarget.tvOS, BuildTarget.StandaloneOSX};

#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX))
        public override void OnFinalizePostProcess(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string generatedProjectPath)
        {
            var pbxProject = AppleBuild.GetPbxProject(buildTarget, generatedProjectPath);
            if (pbxProject != null)
            {
                pbxProject.AddBuildProperty(pbxProject.GetUnityMainTargetGuid(), "ENABLE_BITCODE", "NO");

                if (buildTarget != BuildTarget.StandaloneOSX)
                {
                    pbxProject.AddBuildProperty(pbxProject.GetUnityFrameworkTargetGuid(), "ENABLE_BITCODE", "NO");
                }   
            }
        }

        public override void OnProcessFrameworks(AppleBuildProfile _, BuildTarget buildTarget, string generatedProjectPath, PBXProject pbxProject)
        {
            if (Array.IndexOf(SupportedTargets, buildTarget) > -1)
            {
                AppleNativeLibraryUtility.ProcessWrapperLibrary(DisplayName, buildTarget, generatedProjectPath, pbxProject);
            }
            else
            {
                Debug.LogWarning($"[{DisplayName}] No native library defined for Unity build target {buildTarget.ToString()}. Skipping.");
            }
        }
#endif
    }
}
