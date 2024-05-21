using Apple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.Core
{
    public class AppleBuildStep : ScriptableObject
    {
        /// <summary>
        /// Toggles whether or not a plug-in is included in the build. Exposed to user via Editor UI in the Apple Build Settings section.
        /// </summary>
        public bool IsEnabled = true;
        
        /// <summary>
        /// Name used for display of a plug-in in Editor UI. This should be the same string used for the <c>displayName</c> field of a given plug-in's <c>package.json</c>
        /// </summary>
        public virtual string DisplayName => GetType().Name;

        /// <summary>
        /// Each build step may reference a plug-in with a set of per-BuiltTarget native libraries. 
        /// </summary>
        public virtual BuildTarget[] SupportedTargets => Array.Empty<BuildTarget>();

        /// <summary>
        /// Convenience property to determine if the plug-in has associated native libraries.
        /// </summary>
        public bool IsNativePlugIn => SupportedTargets.Length > 0;

        /// <summary>
        /// Returns an enumerable collection of all objects in the project which derive from AppleBuildStep
        /// </summary>
        public static IEnumerable<Type> ProjectAppleBuildStepTypes()
        {
            var appleBuildStepTypes = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                                    from type in assembly.GetTypes()
                                                    where typeof(AppleBuildStep).IsAssignableFrom(type) && type != typeof(AppleBuildStep)
                                                    select type;

            return appleBuildStepTypes;
        }
#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
        /// <summary>
        /// Called at the beginning of performing build post process. This is invoked first for all steps.
        /// </summary>
        /// <param name="appleBuildProfile"></param>
        /// <param name="buildTarget"></param>
        /// <param name="generatedProjectPath"></param>
        public virtual void OnBeginPostProcess(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string generatedProjectPath) { }

        /// <summary>
        /// Called when steps should modify the info plist.
        /// </summary>
        /// <param name="appleBuildProfile"></param>
        /// <param name="buildTarget"></param>
        /// <param name="generatedProjectPath"></param>
        /// <param name="infoPlist"></param>
        public virtual void OnProcessInfoPlist(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string generatedProjectPath, PlistDocument infoPlist) { }

        /// <summary>
        /// Called when steps should modifiy entitlements.
        /// </summary>
        /// <param name="appleBuildProfile"></param>
        /// <param name="buildTarget"></param>
        /// <param name="generatedProjectPath"></param>
        /// <param name="entitlements"></param>
        public virtual void OnProcessEntitlements(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string generatedProjectPath, PlistDocument entitlements) { }

        /// <summary>
        /// Called when steps should modify or copy frameworks over. PBXProject is supplied on all platforms; only mac if xcode generated.
        /// </summary>
        /// <param name="appleBuildProfile"></param>
        /// <param name="buildTarget"></param>
        /// <param name="generatedProjectPath"></param>
        /// <param name="pBXProject"></param>
        public virtual void OnProcessFrameworks(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string generatedProjectPath, PBXProject pBXProject) { }

        /// <summary>
        /// Called whenever a step calls AppleBuild.ProcessExportPlistOptions; and is called to manipulate the generated exportPlistOptions.
        /// </summary>
        /// <param name="appleBuildProfile"></param>
        /// <param name="buildTarget"></param>
        /// <param name="exportPlistOptions"></param>
        public virtual void OnProcessExportPlistOptions(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string generatedProjectPath, PlistDocument exportPlistOptions) { }

        /// <summary>
        /// Called on all steps, in-order, as a final post process command. Any final signatures can be done here (if last step).
        /// </summary>
        /// <param name="appleBuildProfile"></param>
        /// <param name="buildTarget"></param>
        /// <param name="generatedProjectPath"></param>
        public virtual void OnFinalizePostProcess(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string generatedProjectPath) { }
#endif // (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
    }
}
