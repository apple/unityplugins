using Apple.CoreHaptics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityPickers;
using System.Linq;

/// <summary>
/// The custom inspector to represent CHHapticEvents on CHHapticPlayer and
/// CHOneShotPlayer components.
/// </summary>
[CustomPropertyDrawer(typeof(CHHapticEvent), false)]
public class CHHapticEventDrawer : PropertyDrawer {
    #region Strings

    private const string _timeLabel = "Time (sec)";
    private const string _durationLabel = "Duration (sec)";
    private const string _customAudioLabel = "Waveform";
    private const string _paramsLabel = "Parameters";
    private const string _customWaveFilePathError = "Wave files for AudioCustom events must be in the StreamingAssets directory";
    private const string _customWaveFileFormatError = "The Waveform field is for audio files only.";

    #endregion Strings

    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label) {
        EditorGUI.BeginProperty(pos, label, prop);

        var serializedType = prop.FindPropertyRelative("EventType");
        var serializedTime = prop.FindPropertyRelative("Time");
        var serializedDuration = prop.FindPropertyRelative("EventDuration");
        var serializedWave = prop.FindPropertyRelative("EventWaveform");
        var serializedWavePath = prop.FindPropertyRelative("EventWaveformPath");
        var serializedParameters = prop.FindPropertyRelative("EventParameters");

        #region Expandable popup as item label

        EditorGUILayout.BeginHorizontal();

        var prevIdx = serializedType.enumValueIndex;
        var typeStrings = serializedType.enumDisplayNames;

        // Make the arrow clickable, and give a little tolerance
        var foldoutPos = new Rect(pos.x, pos.y, 5, pos.height);
        prop.isExpanded = EditorGUI.Foldout(foldoutPos, prop.isExpanded, "", true);

        // Move the dropdown off of the arrow
        var dropdownPos = new Rect(pos.x + foldoutPos.width, pos.y, pos.width - 40f, pos.height);
        var idx = EditorGUI.Popup(dropdownPos, "", prevIdx, typeStrings);

        if (prevIdx != idx) {
            serializedType.enumValueIndex = idx;
            serializedParameters.ClearArray();
        }

        var deleted = false;
        var buttonRect = new Rect(pos.x + foldoutPos.width + dropdownPos.width + 10f, pos.y, pos.width - foldoutPos.width - dropdownPos.width - 10f, pos.height);
        
        var deleteButtonContent = new GUIContent("\u232B");

        if (GUI.Button(buttonRect, deleteButtonContent)) {
            prop.DeleteCommand();
            EditorUtility.SetDirty(prop.serializedObject.targetObject);
            deleted = true;
        }

        EditorGUILayout.EndHorizontal();

        #endregion Expandable popup

        if (!deleted) {
            #region Event data

            var lines = 1;

            var eventType = (CHHapticEventType)serializedType.intValue;

            if (prop.isExpanded) {
                EditorGUIUtility.labelWidth = 90f;

                var timeRect = new Rect(pos.x, pos.y + lines * 1.25f * EditorGUIUtility.singleLineHeight, pos.width, pos.height);
                EditorGUI.PropertyField(timeRect, serializedTime, new GUIContent(_timeLabel));
                lines += 1;

                // Pattern times must be positive
                serializedTime.floatValue = Mathf.Max(0f, serializedTime.floatValue);

                if (eventType != CHHapticEventType.HapticTransient) {
                    var durRect = new Rect(pos.x, pos.y + lines * 1.25f * EditorGUIUtility.singleLineHeight, pos.width, pos.height);
                    EditorGUI.PropertyField(durRect, serializedDuration, new GUIContent(_durationLabel));
                    lines += 1;
                    // Duration cannot be negative
                    if (serializedDuration.floatValue < 0f) {
                        serializedDuration.floatValue = 0f;
                    }
                }

                if (eventType == CHHapticEventType.AudioCustom) {
                    var waveRect = new Rect(pos.x, pos.y + lines * 1.25f * EditorGUIUtility.singleLineHeight, pos.width, pos.height);

                    AssetPicker.PropertyField(waveRect, serializedWave, fieldInfo, new GUIContent(_customAudioLabel), typeof(UnityEngine.Object),
                        he => he.Path.Contains("StreamingAssets")
                        && CHHapticAudioCustomEvent.SupportedAudioExtensions.Any(he.Path.EndsWith));

                    if (!(serializedWave.objectReferenceValue is null)) {
                        var wavePath = AssetDatabase.GetAssetPath(serializedWave.objectReferenceValue);
                        if (!(wavePath is null))
                        {
	                        if (!CHHapticAudioCustomEvent.SupportedAudioExtensions.Any(wavePath.EndsWith))
	                        {
		                        Debug.LogError(_customWaveFileFormatError);
		                        serializedWave.objectReferenceValue = null;
	                        }
	                        else if (!wavePath.Contains("StreamingAssets"))
	                        {
		                        Debug.LogError(_customWaveFilePathError);
		                        serializedWave.objectReferenceValue = null;
	                        }
	                        else
	                        {
		                        var parts = wavePath.Split(new[] {"StreamingAssets/"},
			                        StringSplitOptions.RemoveEmptyEntries);
		                        wavePath = parts[parts.Length - 1];

		                        serializedWavePath.stringValue = wavePath;
	                        }
                        }
                    }
                    else {
                        serializedWavePath.stringValue = "";
                    }

                    lines += 1;
                }

                GUILayout.BeginHorizontal();
                var paramsRect = new Rect(pos.x, pos.y + lines * 1.25f * EditorGUIUtility.singleLineHeight + .5f, 110f, pos.height);
                if (serializedParameters.arraySize < 1) {
                    EditorGUI.LabelField(paramsRect, new GUIContent(_paramsLabel));
                }
                else {
                    paramsRect.x += 10f;
                    serializedParameters.isExpanded = EditorGUI.Foldout(paramsRect, serializedParameters.isExpanded, new GUIContent(_paramsLabel), true);
                }

                var validEnumValues = ValidParameters(serializedType);
                var enumMap = new Dictionary<string, int>();
                var enumNames = Enum.GetNames(typeof(CHHapticEventParameterID));
                for (var i = 0; i < enumNames.Length; i++) {
                    enumMap[enumNames[i]] = i;
                }

                if (serializedParameters.arraySize < validEnumValues.Count) {
                    var newParamButtonRect = new Rect(pos.x + 10f + paramsRect.width, pos.y + lines * 1.25f * EditorGUIUtility.singleLineHeight + .5f, 30f, pos.height);
                    if (GUI.Button(newParamButtonRect, EditorGUIUtility.FindTexture("d_Toolbar Plus"))) {
                        var newParamIdIdx = enumMap[GetUnselectedParameter(serializedParameters, validEnumValues)];

                        serializedParameters.arraySize += 1;

                        var newParam = serializedParameters.GetArrayElementAtIndex(serializedParameters.arraySize - 1);
                        var newParamValue = newParam.FindPropertyRelative("ParameterValue");
                        var newParamId = newParam.FindPropertyRelative("ParameterID");

                        newParamId.enumValueIndex = newParamIdIdx;
                        newParamValue.floatValue = CHHapticCapabilities.DefaultValueForEventParameter(newParamIdIdx, (int)eventType);

                        serializedParameters.isExpanded = true;
                    }
                }

                lines += 1;
                GUILayout.EndHorizontal();

                RenderParameters(prop, validEnumValues, enumMap, pos, lines);
            }

            #endregion Event data
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
        var serializedType = prop.FindPropertyRelative("EventType");
        var eventType = (CHHapticEventType)serializedType.intValue;

        // All events contain the title element, and a Time element
        var lines = 2;

        // No duration for Transient, all others have it
        if (eventType != CHHapticEventType.HapticTransient) {
            lines += 1;
        }

        // Extra line for Waveform
        if (eventType == CHHapticEventType.AudioCustom) {
            lines += 1;
        }

        // Line for the "Parameters" label
        lines += 1;

        // Add a line for each Parameter if it's expanded
        var serializedParameters = prop.FindPropertyRelative("EventParameters");
        if (serializedParameters.isExpanded) {
            lines += serializedParameters.arraySize;
        }

        return prop.isExpanded ? lines * 1.25f * EditorGUIUtility.singleLineHeight : EditorGUIUtility.singleLineHeight;
    }

    /// <summary>
    /// Get the list of CHHapticEventParameters that are compatible with this
    /// particular EventType
    /// </summary>
    /// <param name="eventType">
    /// The event's type, e.g. HapticTransient, AudioContinuous, etc
    /// </param>
    /// <returns>
    /// A list of the valid parameters' names.
    /// </returns>
    private static List<string> ValidParameters(SerializedProperty eventType) {
        var parameterNames = Enum.GetNames(typeof(CHHapticEventParameterID));

        var validParameters = new List<string>(parameterNames.Length);

        var typeName = eventType.enumNames[eventType.enumValueIndex];

        foreach (var s in parameterNames) {
            // Haptic parameters with haptic events, audio parameters with audio events
            if (!(typeName is null) && (typeName.Contains("Haptic") && s.Contains("Haptic") || typeName.Contains("Audio") && s.Contains("Audio"))) {
                validParameters.Add(s);
            }

            // ADSR params are valid for all events that aren't "HapticTransient"
            if (!(typeName is null) && !typeName.Contains("Transient") && !s.Contains("Haptic") && !s.Contains("Audio")) {
                validParameters.Add(s);
            }
        }

        return validParameters;
    }

    /// <summary>
    /// Manages the CHHapticEventParameter section for each CHHapticEvent.
    /// </summary>
    /// <param name="prop">The serialized CHHapticEvent</param>
    /// <param name="validEnumValues">A list of valid values</param>
    /// <param name="enumMap">A map from Parameter name to the integer enum value.</param>
    /// <param name="pos">The position of this serialized property</param>
    /// <param name="lines">The number of lines previously rendered.</param>
    /// <returns>The number of lines rendered including the parameters.</returns>
    private static void RenderParameters(SerializedProperty prop, IReadOnlyCollection<string> validEnumValues,
	    IReadOnlyDictionary<string, int> enumMap, Rect pos, int lines) {
        var serializedParameters = prop.FindPropertyRelative("EventParameters");
        var serializedEventType = prop.FindPropertyRelative("EventType");

        var selectedParams = new List<string>();

        for (var i = 0; i < serializedParameters.arraySize; i++) {
            var param = serializedParameters.GetArrayElementAtIndex(i);
            var paramId = param.FindPropertyRelative("ParameterID");

            var prevChoiceIdx = paramId.enumValueIndex;
            if (prevChoiceIdx > -1) {
                var previousParamId = paramId.enumNames[prevChoiceIdx];
                selectedParams.Add(previousParamId);
            }
        }

        if (!serializedParameters.isExpanded || serializedParameters.arraySize <= 0)
        {
	        return;
        }
        
        serializedParameters.arraySize = Mathf.Min(serializedParameters.arraySize, validEnumValues.Count);

        for (var i = 0; i < serializedParameters.arraySize; i++) {
	        var rowHeight = pos.y + lines * 1.25f * EditorGUIUtility.singleLineHeight + .5f;

	        var param = serializedParameters.GetArrayElementAtIndex(i);
	        var paramId = param.FindPropertyRelative("ParameterID");
	        var paramValue = param.FindPropertyRelative("ParameterValue");

	        var paramDeleteButtonRect = new Rect(pos.x, rowHeight, 20f, pos.height);
	        var paramIdRect = new Rect(pos.x + paramDeleteButtonRect.width, rowHeight, 120f, pos.height);
	        var paramValueRect = new Rect(paramIdRect.x + paramIdRect.width + 10f, rowHeight, pos.width - paramIdRect.width - paramDeleteButtonRect.width, pos.height);

	        GUILayout.BeginHorizontal();

	        if (GUI.Button(paramDeleteButtonRect, EditorGUIUtility.FindTexture("d_Toolbar Minus"))) {
		        serializedParameters.DeleteArrayElementAtIndex(i);

		        // Exit the loop. The parameters will be re-rendered on the next GUI refresh
		        EditorUtility.SetDirty(prop.serializedObject.targetObject);
		        break;
	        }

	        // Get the user's new choice for this row's parameter
	        var prevChoiceIdx = paramId.enumValueIndex;
	        var previousParamId = prevChoiceIdx > -1 ? paramId.enumNames[prevChoiceIdx] : "";

	        // The list of SELECTABLE choices includes any unselected parameters as well as this row's previously selected parameter
	        var selectableParams = validEnumValues.Where(p => previousParamId != "" && p == previousParamId || !selectedParams.Contains(p)).ToList();

	        var choice = ParameterIdPopup(paramIdRect, paramId, selectableParams, enumMap);

	        if (choice == "Sustained") {
		        paramValue.floatValue = EditorGUI.Popup(paramValueRect, (int)paramValue.floatValue, new[] { "0", "1" });
	        }
	        else
	        {
		        var maxValue = CHHapticCapabilities.MaxValueForEventParameter(paramId.enumValueIndex, serializedEventType.enumValueIndex);
		        var minValue = CHHapticCapabilities.MinValueForEventParameter(paramId.enumValueIndex, serializedEventType.enumValueIndex);
		        paramValue.floatValue = Mathf.Clamp(EditorGUI.FloatField(paramValueRect, paramValue.floatValue), minValue, maxValue);
	        }

	        GUILayout.EndHorizontal();

	        // Update the list of previously selected parameters so the following popups render correctly
	        if (choice != previousParamId)
	        {
		        selectedParams.Remove(previousParamId);
		        selectedParams.Add(choice);

		        EditorUtility.SetDirty(prop.serializedObject.targetObject);
	        }

	        lines += 1;
        }
    }

    /// <summary>
    /// Show the dropdown for the list of available parameters that a developer
    /// can select for a given CHHapticEventParameter. The available parameters
    /// are filtered by EventType as well as any previously-selected parameters,
    /// such that no parameter can be selected twice for a given event.
    /// </summary>
    /// <param name="rect">The position of the popup.</param>
    /// <param name="paramId">The currently selected parameter.</param>
    /// <param name="validValues">A list of parameter names to show</param>
    /// <param name="enumMap">The master map from parameter name to enum value.</param>
    /// <returns>The name of the parameter selected by the user.</returns>
    private static string ParameterIdPopup(Rect rect, SerializedProperty paramId, List<string> validValues, IReadOnlyDictionary<string, int> enumMap)
    {
        // Get the name of the param that should be marked as selected
        var previousChosenParam = paramId.enumNames[Math.Max(paramId.enumValueIndex, 0)];

        // Get the index of this choice relative to the values the user will see in this popup
        var chosenIdx = validValues.IndexOf(previousChosenParam);

        // Get the user's updated chosen index for the available values
        chosenIdx = EditorGUI.Popup(rect, chosenIdx, validValues.ToArray());

        // Get the string name of the above choice
        var choice = validValues[Math.Max(chosenIdx, 0)];

        // Set the parameterId to the master enum's corresponding index
        paramId.enumValueIndex = enumMap[choice];

        return choice;
    }

    /// <summary>
    /// Finds the next available unselected CHHapticEventParameter for this
    /// CHHapticEvent to avoid duplicate parameters.
    /// </summary>
    /// <param name="serializedParameters">
    /// All of the serialized CHHapticEventParameters for this CHHapticEvent
    /// </param>
    /// <param name="validEnumValues">
    /// The valid CHHapticEventParameter names given this CHHapticEvent's type.
    /// </param>
    /// <returns>The name of the available CHHapticEventParameter</returns>
    private static string GetUnselectedParameter(SerializedProperty serializedParameters, IEnumerable<string> validEnumValues) {
        var unselectedParams = new List<string>(validEnumValues);

        for (var i = 0; i < serializedParameters.arraySize; i++) {
            var param = serializedParameters.GetArrayElementAtIndex(i);
            var paramId = param.FindPropertyRelative("ParameterID");

            var prevChoiceIdx = paramId.enumValueIndex;
            if (prevChoiceIdx > -1) {
                var previousParamId = paramId.enumNames[prevChoiceIdx];
                unselectedParams.Remove(previousParamId);
            }
        }

        return unselectedParams[0];
    }
}
