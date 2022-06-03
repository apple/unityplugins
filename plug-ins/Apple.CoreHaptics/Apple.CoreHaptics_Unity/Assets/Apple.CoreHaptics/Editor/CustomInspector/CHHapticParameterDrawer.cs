using Apple.CoreHaptics;
using UnityEngine;
using UnityEditor;

/// <summary>
/// The custom property drawer for CHHapticDynamicParameter, used to configure
/// the custom inspector on CHHapticPlayer and CHOneShotPlayer components.
/// </summary>
[CustomPropertyDrawer(typeof(CHHapticParameter))]
public class CHHapticDynamicParameterDrawer : PropertyDrawer {
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label) {
        EditorGUI.BeginProperty(pos, label, prop);

        var serializedType = prop.FindPropertyRelative("ParameterID");
        var serializedValue = prop.FindPropertyRelative("ParameterValue");
        var serializedTime = prop.FindPropertyRelative("Time");

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
        }

        var deleted = false;
        var buttonRect = new Rect(pos.x + foldoutPos.width + dropdownPos.width + 10f, pos.y, pos.width - foldoutPos.width - dropdownPos.width - 10f, pos.height);

        if (GUI.Button(buttonRect, "\u232B")) // Other options: \u232B is <[x], \u2327 is [x], \u20E0 is circle w/ slash
        {
            prop.DeleteCommand();
            EditorUtility.SetDirty(prop.serializedObject.targetObject);
            deleted = true;
        }

        EditorGUILayout.EndHorizontal();

        if (!deleted) {
            var lines = 1;

            if (prop.isExpanded) {
                var timeRect = new Rect(pos.x, pos.y + lines * 1.25f * EditorGUIUtility.singleLineHeight, pos.width, pos.height);

                EditorGUI.PropertyField(timeRect, serializedTime, new GUIContent("Time (sec)"));
                // Pattern times must be positive
                if (serializedTime.floatValue < 0) {
                    serializedTime.floatValue = 0f;
                }
                lines += 1;

                var valueRect = new Rect(pos.x, pos.y + lines * 1.25f * EditorGUIUtility.singleLineHeight, pos.width, pos.height);
                EditorGUI.PropertyField(valueRect, serializedValue);

	            var minValue = CHHapticCapabilities.MinValueForDynamicParameter(serializedType.enumValueIndex);
                var maxValue = CHHapticCapabilities.MaxValueForDynamicParameter(serializedType.enumValueIndex);
                serializedValue.floatValue = Mathf.Clamp(serializedValue.floatValue, minValue, maxValue);
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
        return prop.isExpanded ? 3 * 1.25f * EditorGUIUtility.singleLineHeight : EditorGUIUtility.singleLineHeight;
    }
}
