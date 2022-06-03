using Apple.CoreHaptics;
using UnityEngine;
using UnityEditor;

/// <summary>
/// The custom property drawer for CHHapticParameterCurves, used to configure
/// the custom inspector on CHHapticPlayer and CHOneShotPlayer components.
/// </summary>
[CustomPropertyDrawer(typeof(CHHapticParameterCurve))]
public class CHHapticParameterCurveDrawer : PropertyDrawer
{
	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
	{
		EditorGUI.BeginProperty(pos, label, prop);

		var serializedType = prop.FindPropertyRelative("ParameterID");
		var serializedTime = prop.FindPropertyRelative("Time");
		var serializedControlPoints = prop.FindPropertyRelative("ParameterCurveControlPoints");

		EditorGUILayout.BeginHorizontal();

		var prevIdx = serializedType.enumValueIndex;
		var typeStrings = serializedType.enumDisplayNames;

		// Make the arrow clickable, and give a little tolerance
		var foldoutPos = new Rect(pos.x, pos.y, 5, pos.height);
		prop.isExpanded = EditorGUI.Foldout(foldoutPos, prop.isExpanded, "", true);

		// Move the dropdown off of the arrow
		var dropdownPos = new Rect(pos.x + foldoutPos.width, pos.y, pos.width - 40f, pos.height);
		var idx = EditorGUI.Popup(dropdownPos, "", prevIdx, typeStrings);

		if (prevIdx != idx)
		{
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

		if (!deleted)
		{
			var lines = 1;

			if (prop.isExpanded)
			{
				var timeRect = new Rect(pos.x, pos.y + lines * 1.25f * EditorGUIUtility.singleLineHeight, pos.width, pos.height);

				EditorGUI.PropertyField(timeRect, serializedTime, new GUIContent("Time (sec)"));
				// Pattern times must be positive
				serializedTime.floatValue = Mathf.Max(0f, serializedTime.floatValue);

				lines += 1;

				GUILayout.BeginHorizontal();
				var cpRect = new Rect(pos.x, pos.y + lines * 1.25f * EditorGUIUtility.singleLineHeight + .5f, 110f, pos.height);
				if (serializedControlPoints.arraySize < 1)
				{
					EditorGUI.LabelField(cpRect, new GUIContent("Control Points"));
				}
				else
				{
					cpRect.x += 10f;
					serializedControlPoints.isExpanded = EditorGUI.Foldout(cpRect, serializedControlPoints.isExpanded, new GUIContent("Control Points"), true);
				}

				if (serializedControlPoints.arraySize < 16)
				{
					var newParamButtonRect = new Rect(pos.x + 10f + cpRect.width, pos.y + lines * 1.25f * EditorGUIUtility.singleLineHeight + .5f, 30f, pos.height);
					if (GUI.Button(newParamButtonRect, EditorGUIUtility.FindTexture("d_Toolbar Plus")))
					{
						serializedControlPoints.arraySize += 1;
						var newPoint = serializedControlPoints.GetArrayElementAtIndex(serializedControlPoints.arraySize - 1);
						var newTime = newPoint.FindPropertyRelative("Time");
						var newValue = newPoint.FindPropertyRelative("ParameterValue");
						if (serializedControlPoints.arraySize > 1)
						{
							var val = serializedControlPoints.GetArrayElementAtIndex(serializedControlPoints.arraySize - 2).FindPropertyRelative("Time").floatValue + 0.1f;
							newTime.floatValue = (float)System.Math.Round(val, 2);
						}
						else
						{
							newTime.floatValue = 0f;
						}
						newValue.floatValue = 0f;

						serializedControlPoints.isExpanded = true;
					}
				}

				lines += 1;

				GUILayout.EndHorizontal();

				serializedControlPoints.arraySize = Mathf.Min(serializedControlPoints.arraySize, 16);

				if (serializedControlPoints.isExpanded && serializedControlPoints.arraySize > 0)
				{
					for (var i = 0; i < serializedControlPoints.arraySize; i++)
					{
						var serializedPoint = serializedControlPoints.GetArrayElementAtIndex(i);
						var serializedValue = serializedPoint.FindPropertyRelative("ParameterValue");
						var serializedPointTime = serializedPoint.FindPropertyRelative("Time");

						EditorGUILayout.BeginHorizontal();

						var rowHeight = pos.y + lines * 1.25f * EditorGUIUtility.singleLineHeight;
						var cpDeleteButtonRect = new Rect(pos.x, rowHeight, 20f, pos.height);

						if (GUI.Button(cpDeleteButtonRect, EditorGUIUtility.FindTexture("d_Toolbar Minus")))
						{
							serializedControlPoints.DeleteArrayElementAtIndex(i);

							// Exit the loop. The control points will be re-rendered on the next GUI refresh
							EditorUtility.SetDirty(prop.serializedObject.targetObject);
							break;
						}

						var originalLabelWidth = EditorGUIUtility.labelWidth;

						var cpTimeRect = new Rect(cpDeleteButtonRect.x + cpDeleteButtonRect.width + 5f, rowHeight, (pos.width - cpDeleteButtonRect.width) / 2f, pos.height);
						var cpValueRect = new Rect(cpTimeRect.x + cpTimeRect.width + 10f, rowHeight, (pos.width - cpDeleteButtonRect.width) / 2f, pos.height);

						EditorGUIUtility.labelWidth = 40f;
						EditorGUI.PropertyField(cpTimeRect, serializedPointTime, new GUIContent("Time"));
						serializedPointTime.floatValue = Mathf.Max(0f, serializedPointTime.floatValue);

						EditorGUI.PropertyField(cpValueRect, serializedValue, new GUIContent("Value"));
						EditorGUIUtility.labelWidth = originalLabelWidth;

						float maxParamValue = CHHapticCapabilities.MaxValueForDynamicParameter(serializedType.enumValueIndex);
						float minParamValue = CHHapticCapabilities.MinValueForDynamicParameter(serializedType.enumValueIndex);
						serializedValue.floatValue = Mathf.Clamp(serializedValue.floatValue, minParamValue, maxParamValue);

						EditorGUILayout.EndHorizontal();

						lines += 1;
					}
				}

			}
		}

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
	{
		var lines = 1;

		if (prop.isExpanded)
		{
			// One line for the time field, and one for the control points array label
			lines += 2;

			var serializedControlPoints = prop.FindPropertyRelative("ParameterCurveControlPoints");
			lines += serializedControlPoints.isExpanded ? serializedControlPoints.arraySize : 0;
		}
		return lines * 1.25f * EditorGUIUtility.singleLineHeight;
	}
}
