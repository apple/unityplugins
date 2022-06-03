using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Apple.Core
{
    public class AppleBuildSettingsProvider : SettingsProvider
    {
        public const string             SettingsProviderPath     = "Project/Apple Build Settings";
        public static readonly string[] SettingsProviderKeywords = {"Apple", "build"};

        private AppleBuildProfile       currentBuildProfile;
        private AppleBuildProfileEditor profileEditor;

        /// <summary>
        /// Constructor for settings provider initializes base class
        /// </summary>
        public AppleBuildSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) { }

        /// <summary>
        /// Called when a user clicks on the custom element in the Project Settings window
        /// </summary>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // TODO: Handle multiple build profiles
            currentBuildProfile = AppleBuildProfile.DefaultProfile();
            profileEditor = (AppleBuildProfileEditor)Editor.CreateEditor(currentBuildProfile, typeof(AppleBuildProfileEditor));

            currentBuildProfile.ResolveBuildSteps();
        }

        /// <summary>
        /// Called when it's time to draw UI
        /// <summary>
        public override void OnGUI(string searchContext)
        {
            profileEditor.OnInspectorGUI();
        }

        /// <summary>
        // Registers the SettingsProvider
        /// <summary>
        [SettingsProvider]
        public static SettingsProvider Create()
        {
            var provider = new AppleBuildSettingsProvider(SettingsProviderPath, SettingsScope.Project)
            {
                keywords = SettingsProviderKeywords
            };

            return provider;
        }
    }
}
