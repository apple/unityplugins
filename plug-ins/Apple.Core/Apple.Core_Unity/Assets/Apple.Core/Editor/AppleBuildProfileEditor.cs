using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Apple.Core
{
    [CustomEditor(typeof(AppleBuildProfile))]
    public class AppleBuildProfileEditor : Editor
    {
        private const float VerticalUIPadding = 5.0f;
        private const float _minLabelWidth = 220f;
        private const float _buildStepLabelWidth = 250f;
        public const string BuildPlayerWindowType = "UnityEditor.BuildPlayerWindow,UnityEditor";

        private SerializedProperty _serializedAutomateInfoPlist;
        private SerializedProperty _serializedDefaultInfoPlist;
        private SerializedProperty _serializedNonExemptEncryption;
        private SerializedProperty _serializedMinimumOSVersion_iOS;
        private SerializedProperty _serializedMinimumOSVersion_tvOS;
        private SerializedProperty _serializedMinimumOSVersion_macOS;
        private SerializedProperty _serializedAutomateEntitlements;
        private SerializedProperty _serializedDefaultEntitlements;

        private static Dictionary<Editor, bool> _editorFoldouts = new Dictionary<Editor, bool>();
        private static Dictionary<ScriptableObject, Editor> _editors = new Dictionary<ScriptableObject, Editor>();

        class UIStrings
        {
            public const string UnityBuildConfigSectionLabelText = "Unity Build Configuration";
            public const string UnityActiveBuildTargetLabelText = "Current build target:";
            public const string UnityBuildSettingsButtonLabelText = "Unity Build Settings...";

            public const string AutomationSettingsSectionLabelText = "Automation Settings";

            public const string AutomateInfoPlistToggleLabelText = "Automate info.plist";
            public const string AutomateInfoPlistTooltip = "Automatically include the Info.plist into the resulting Xcode project.";

            public const string AppUsesNonExemptEncryptionToggleLabelText = "ITSAppUsesNonExemptEncryption";
            public const string NonExemptEncryptionTooltip = "Something special about this non exempetion...";

            public const string DefaultInfoPlistFieldLabelText = "Default Info.plist";
            public const string DefaultInfoPlistTooltip = "(Optional) An Info.plist file used to configure your Xcode app.";

            public const string DefaultMinimumMacOSVersionText = "10.15.0";
            public const string MinimumOSVersionFieldLabelText_iOS = "Minimum iOS Version";
            public const string MinimumOSVersionFieldLabelText_tvOS = "Minimum tvOS Version";
            public const string MinimumOSVersionFieldLabelText_macOS = "Minimum macOS Version";

            public const string AutomateEntitlementsToggleLabelText = "Automate Entitlements";
            public const string AutomateEntitlementsTooltip = "Automatically add an entitlements file to your Xcode project.";

            public const string DefaultEntitlementsFieldLabelText = "Default Entitlements";
            public const string DefaultEntitlementsTooltip = "(Optional) An Entitlements file to incorporate into your Xcode app.";

            public const string iOSBuildTargetName = "iOS";

            public const string tvOSBuildTargetName = "tvOS";

            public const string macOSBuildTargetName = "macOS";
        }

        public void OnEnable()
        {
            _serializedAutomateInfoPlist = serializedObject.FindProperty("AutomateInfoPlist");
            _serializedDefaultInfoPlist = serializedObject.FindProperty("DefaultInfoPlist");

            _serializedNonExemptEncryption = serializedObject.FindProperty("AppUsesNonExemptEncryption");

            _serializedMinimumOSVersion_iOS = serializedObject.FindProperty("MinimumOSVersion_iOS");
            _serializedMinimumOSVersion_macOS = serializedObject.FindProperty("MinimumOSVersion_macOS");
            _serializedMinimumOSVersion_tvOS = serializedObject.FindProperty("MinimumOSVersion_tvOS");

            _serializedAutomateEntitlements = serializedObject.FindProperty("AutomateEntitlements");
            _serializedDefaultEntitlements = serializedObject.FindProperty("DefaultEntitlements");
        }

        /// <summary>
        /// Called when drawing associated tab of the Project Settings window. See: AppleBuildSettingsProvider.cs
        /// Also called to draw within the Inspector window when a user selects an AppleBuildProfile asset
        /// </summary>
        public override void OnInspectorGUI()
        {
            var appleBuildProfile = target as AppleBuildProfile;

            serializedObject.Update();

            #region Draw Build Summary

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(UIStrings.UnityBuildConfigSectionLabelText, EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            string buildTargetName;
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.iOS:
                    buildTargetName = UIStrings.iOSBuildTargetName;
                    break;
                case BuildTarget.tvOS:
                    buildTargetName = UIStrings.tvOSBuildTargetName;
                    break;
                case BuildTarget.StandaloneOSX:
                    buildTargetName = UIStrings.macOSBuildTargetName;
                    break;
                default:
                    buildTargetName = string.Empty;
                    break;
            }


            if (_serializedMinimumOSVersion_iOS.stringValue == string.Empty)
            {
                _serializedMinimumOSVersion_iOS.stringValue = PlayerSettings.iOS.targetOSVersionString;
                serializedObject.ApplyModifiedProperties();
            }


            if (_serializedMinimumOSVersion_tvOS.stringValue == string.Empty)
            {
                _serializedMinimumOSVersion_tvOS.stringValue = PlayerSettings.tvOS.targetOSVersionString;
                serializedObject.ApplyModifiedProperties();
            }


            if (_serializedMinimumOSVersion_macOS.stringValue == string.Empty)
            {
                _serializedMinimumOSVersion_macOS.stringValue = UIStrings.DefaultMinimumMacOSVersionText;
                serializedObject.ApplyModifiedProperties();
            }

            GUILayout.Label($"{UIStrings.UnityActiveBuildTargetLabelText} {buildTargetName}");

            if (GUILayout.Button(UIStrings.UnityBuildSettingsButtonLabelText))
            {
                EditorWindow.GetWindow(Type.GetType(BuildPlayerWindowType));
            }

            GUILayout.EndVertical();

            #endregion // Draw Build Summary

            GUILayout.Space(VerticalUIPadding);

            #region Draw Build Profile Properties

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label(UIStrings.AutomationSettingsSectionLabelText, EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = _minLabelWidth;

            // Info Plist
            var automateInfoPlistLabel = new GUIContent(UIStrings.AutomateInfoPlistToggleLabelText, UIStrings.AutomateInfoPlistTooltip);
            EditorGUILayout.PropertyField(_serializedAutomateInfoPlist, automateInfoPlistLabel, GUILayout.MinWidth(_minLabelWidth));

            EditorGUI.indentLevel++;

            if (_serializedAutomateInfoPlist.boolValue)
            {
                var nonExemptEncryptionLabel = new GUIContent(UIStrings.AppUsesNonExemptEncryptionToggleLabelText, UIStrings.NonExemptEncryptionTooltip);
                EditorGUILayout.PropertyField(_serializedNonExemptEncryption, nonExemptEncryptionLabel, null);

                EditorGUI.BeginChangeCheck();
                var defaultInfoPlistLabel = new GUIContent(UIStrings.DefaultInfoPlistFieldLabelText, UIStrings.DefaultInfoPlistTooltip);
                EditorGUILayout.ObjectField(_serializedDefaultInfoPlist, typeof(UnityEngine.Object), defaultInfoPlistLabel, GUILayout.MinWidth(_minLabelWidth));
                if (EditorGUI.EndChangeCheck() && !(_serializedDefaultInfoPlist.objectReferenceValue is null))
                {
                    if (_serializedDefaultInfoPlist.objectReferenceValue != null)
                    {
                        string filePath = AssetDatabase.GetAssetPath(_serializedDefaultInfoPlist.objectReferenceValue.GetInstanceID());
                        if (!filePath.EndsWith(".plist"))
                        {
                            _serializedDefaultInfoPlist.objectReferenceValue = null;
                            Debug.LogError("The Info.plist field only supports files with a .plist extension.");
                        }
                    }
                }

                var minimumOSVersionLabel_iOS = new GUIContent(UIStrings.MinimumOSVersionFieldLabelText_iOS);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_serializedMinimumOSVersion_iOS, minimumOSVersionLabel_iOS, GUILayout.MinWidth(_minLabelWidth));
                if (EditorGUI.EndChangeCheck() && _serializedMinimumOSVersion_iOS.stringValue != string.Empty)
                {
                    PlayerSettings.iOS.targetOSVersionString = _serializedMinimumOSVersion_iOS.stringValue;
                }

                var minimumOSVersionLabel_tvOS = new GUIContent(UIStrings.MinimumOSVersionFieldLabelText_tvOS);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_serializedMinimumOSVersion_tvOS, minimumOSVersionLabel_tvOS, GUILayout.MinWidth(_minLabelWidth));
                if (EditorGUI.EndChangeCheck() && _serializedMinimumOSVersion_tvOS.stringValue != string.Empty)
                {
                    PlayerSettings.tvOS.targetOSVersionString = _serializedMinimumOSVersion_tvOS.stringValue;
                }

                var minimumOSVersionLabel_macOS = new GUIContent(UIStrings.MinimumOSVersionFieldLabelText_macOS);
                EditorGUILayout.PropertyField(_serializedMinimumOSVersion_macOS, minimumOSVersionLabel_macOS, GUILayout.MinWidth(_minLabelWidth));



            }

            EditorGUI.indentLevel--;

            // Entitlements

            var automateEntitlementsLabel = new GUIContent(UIStrings.AutomateEntitlementsToggleLabelText, UIStrings.AutomateEntitlementsTooltip);
            EditorGUILayout.PropertyField(_serializedAutomateEntitlements, automateEntitlementsLabel, GUILayout.MinWidth(_minLabelWidth));

            if (_serializedAutomateEntitlements.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                var defaultEntitlementsLabel = new GUIContent(UIStrings.DefaultEntitlementsFieldLabelText, UIStrings.DefaultEntitlementsTooltip);
                EditorGUILayout.ObjectField(_serializedDefaultEntitlements, typeof(UnityEngine.Object), defaultEntitlementsLabel, GUILayout.MinWidth(_minLabelWidth));
                if (EditorGUI.EndChangeCheck() && !(_serializedDefaultEntitlements.objectReferenceValue is null))
                {
                    string filePath = AssetDatabase.GetAssetPath(_serializedDefaultEntitlements.objectReferenceValue.GetInstanceID());
                    if (!filePath.EndsWith(".entitlements"))
                    {
                        _serializedDefaultEntitlements.objectReferenceValue = null;
                        Debug.LogError("The entitlements field only supports files with a .entitlements extension.");
                    }
                }
                EditorGUI.indentLevel--;
            }


            GUILayout.EndVertical();

            #endregion // Draw Build Profile Properties

            GUILayout.Space(VerticalUIPadding);

            #region Draw Apple Build Steps

            GUILayout.BeginVertical();


            List<string> buildStepNames = appleBuildProfile.buildSteps.Keys.ToList();
            buildStepNames.Sort();

            EditorGUIUtility.labelWidth = _buildStepLabelWidth;

            foreach (var name in buildStepNames)
            {
                var buildStep = appleBuildProfile.buildSteps[name];

                if (!_editors.ContainsKey(buildStep))
                {
                    _editors.Add(buildStep, CreateEditor(buildStep));
                }

                var currEditor = _editors[buildStep];

                if (!_editorFoldouts.ContainsKey(currEditor))
                {
                    _editorFoldouts.Add(currEditor, false);
                }

                var showFoldout = _editorFoldouts[currEditor];

                GUILayout.BeginHorizontal(EditorStyles.toolbar);

                var arrow = showFoldout ? EditorGUIUtility.IconContent("d_icon dropdown") : EditorGUIUtility.IconContent("d_forward");

                if (GUILayout.Button(arrow, EditorStyles.toolbarButton, GUILayout.Width(24)))
                {
                    showFoldout = !showFoldout;
                    _editorFoldouts[currEditor] = showFoldout;
                }

                GUILayout.Label(buildStep.DisplayName, EditorStyles.boldLabel);

                GUILayout.EndHorizontal();

                if (showFoldout)
                {
                    EditorGUI.indentLevel++;
                    GUILayout.BeginVertical(EditorStyles.helpBox);

                    currEditor.OnInspectorGUI();

                    GUILayout.EndVertical();
                    GUILayout.Space(VerticalUIPadding);
                    EditorGUI.indentLevel--;
                }
            }

            GUILayout.EndVertical();

            #endregion // Draw Apple Build Steps

            // Restore EditorGUIUtility's default label width
            EditorGUIUtility.labelWidth = 0f;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
