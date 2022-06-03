using Apple.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.Core
{
    public class AppleBuildStep : ScriptableObject
    {
        public bool IsEnabled = true;
        
        public virtual string DisplayName => GetType().Name;

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

        /// <summary>
        /// Called at the beginning of performing build post process. This is invoked first for all steps.
        /// </summary>
        /// <param name="appleBuildProfile"></param>
        /// <param name="buildTarget"></param>
        /// <param name="pathToBuiltProject"></param>
        public virtual void OnBeginPostProcess(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltProject) { }

        /// <summary>
        /// Called when steps should modify the info plist.
        /// </summary>
        /// <param name="appleBuildProfile"></param>
        /// <param name="buildTarget"></param>
        /// <param name="pathToBuiltTarget"></param>
        /// <param name="infoPlist"></param>
        public virtual void OnProcessInfoPlist(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltTarget, PlistDocument infoPlist) { }

        /// <summary>
        /// Called when steps should modifiy entitlements.
        /// </summary>
        /// <param name="appleBuildProfile"></param>
        /// <param name="buildTarget"></param>
        /// <param name="pathToBuiltTarget"></param>
        /// <param name="entitlements"></param>
        public virtual void OnProcessEntitlements(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltTarget, PlistDocument entitlements) { }

        /// <summary>
        /// Called when steps should modify or copy frameworks over. PBXProject is supplied on all platforms; only mac if xcode generated.
        /// </summary>
        /// <param name="appleBuildProfile"></param>
        /// <param name="buildTarget"></param>
        /// <param name="pathToBuiltTarget"></param>
        /// <param name="pBXProject"></param>
        public virtual void OnProcessFrameworks(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltTarget, PBXProject pBXProject) { }

        /// <summary>
        /// Called whenever a step calls AppleBuild.ProcessExportPlistOptions; and is called to manipulate the generated exportPlistOptions.
        /// </summary>
        /// <param name="appleBuildProfile"></param>
        /// <param name="buildTarget"></param>
        /// <param name="exportPlistOptions"></param>
        public virtual void OnProcessExportPlistOptions(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltProject, PlistDocument exportPlistOptions) { }

        /// <summary>
        /// Called on all steps, in-order, as a final post process command. Any final signatures can be done here (if last step).
        /// </summary>
        /// <param name="appleBuildProfile"></param>
        /// <param name="buildTarget"></param>
        /// <param name="pathToBuiltProject"></param>
        public virtual void OnFinalizePostProcess(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltProject) { }
    }
}