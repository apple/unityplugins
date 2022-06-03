using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Apple.CoreHaptics
{
	[CustomEditor(typeof(AHAPAsset))]
	public class CHAhapAssetEditor : UnityEditor.Editor
	{
		private ReorderableList _reorderableEvents;
		private ReorderableList _reorderableParameters;
		private ReorderableList _reorderableParamCurves;

		private SerializedProperty _serializedEvents;
		private SerializedProperty _serializedDynamicParameters;
		private SerializedProperty _serializedParameterCurves;

		//private static Dictionary<object, bool> _foldoutStates = new Dictionary<object, bool>();
		private Vector2 _scrollView;

		private const string _eventsLabel = "Events";
		private const string _parametersLabel = "Parameters";
		private const string _addDynamicParamBtnTxt = "Add a Dynamic Parameter";
		private const string _parameterCurveLabel = "ParameterCurves";
		private const string _addParamCurveBtnTxt = "Add a Parameter Curve";

		private const string _resetButton = "Reset";
		private const string _importButton = "Import";
		private const string _exportButton = "Export";

		private const string _exportToAhapBtnTxt = "Export to AHAP";
		private const string _exportDefaultFilename = "CoreHapticsPattern";
		private const string _exportHelperTxt = "Please enter a file name for your AHAP.";

		public void OnEnable()
		{
			_serializedEvents = serializedObject.FindProperty("Events");
			_serializedDynamicParameters = serializedObject.FindProperty("Parameters");
			_serializedParameterCurves = serializedObject.FindProperty("ParameterCurves");

			_reorderableEvents = CreateList(serializedObject, _serializedEvents, _eventsLabel, "EventType", typeof(CHHapticEvent), typeof(CHHapticEventType));
			_reorderableParameters = CreateList(serializedObject, _serializedDynamicParameters, _parametersLabel, "ParameterID", typeof(CHHapticParameter), typeof(CHHapticDynamicParameterID));
			_reorderableParamCurves = CreateList(serializedObject, _serializedParameterCurves, _parameterCurveLabel, "ParameterID", typeof(CHHapticParameterCurve), typeof(CHHapticDynamicParameterID));
		}

		protected override void OnHeaderGUI()
		{
			base.OnHeaderGUI();

			var ahapAsset = target as AHAPAsset;
			OnDrawToolbar(ahapAsset);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			_scrollView = GUILayout.BeginScrollView(_scrollView, false, false);

			if (_serializedEvents.arraySize > 0)
			{
				_reorderableEvents.DoLayoutList();
			}
			else
			{
				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField(_eventsLabel, GUILayout.MaxWidth(80f));

				if (GUILayout.Button(EditorGUIUtility.FindTexture("d_Toolbar Plus"), GUILayout.MaxWidth(40f)))
				{
					// Add an item to the serialized event array
					_serializedEvents.arraySize += 1;

					// Make it a transient event
					var newSerialEvent = _serializedEvents.GetArrayElementAtIndex(_serializedEvents.arraySize - 1);
					var newSerialEventType = newSerialEvent.FindPropertyRelative("EventType");
					newSerialEventType.enumValueIndex = (int)CHHapticEventType.HapticTransient;

					// Force it to be expanded
					newSerialEvent.isExpanded = true;

					serializedObject.ApplyModifiedProperties();
				}

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space();

			if (_serializedDynamicParameters.arraySize > 0)
			{
				_reorderableParameters.DoLayoutList();
			}
			else
			{
				if (GUILayout.Button(_addDynamicParamBtnTxt, GUILayout.MaxWidth(160f)))
				{
					_serializedDynamicParameters.arraySize += 1;

					// Force the newly created Dynamic Parameter to be expanded
					var newDynamicParam = _serializedDynamicParameters.GetArrayElementAtIndex(_serializedDynamicParameters.arraySize - 1);
					newDynamicParam.isExpanded = true;
					// Set the value of the new param to the CHCapabilities default
					newDynamicParam.FindPropertyRelative("ParameterValue").floatValue =
						CHHapticCapabilities.DefaultValueForDynamicParameter(newDynamicParam.FindPropertyRelative("ParameterID").enumValueIndex);

					serializedObject.ApplyModifiedProperties();
				}
			}

			EditorGUILayout.Space();

			if (_serializedParameterCurves.arraySize > 0)
			{
				_reorderableParamCurves.DoLayoutList();
			}
			else
			{
				if (GUILayout.Button(_addParamCurveBtnTxt, GUILayout.MaxWidth(160f)))
				{
					_serializedParameterCurves.arraySize += 1;

					// Force the newly created Parameter Curve to be expanded
					var newParamCurve = _serializedParameterCurves.GetArrayElementAtIndex(_serializedParameterCurves.arraySize - 1);
					newParamCurve.isExpanded = true;

					serializedObject.ApplyModifiedProperties();
				}
			}
			GUILayout.EndScrollView();
			serializedObject.ApplyModifiedProperties();
		}

		private static void OnDrawToolbar(AHAPAsset ahapAsset)
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(_importButton, EditorStyles.toolbarButton))
			{
				var selectedPath = EditorUtility.OpenFilePanel("Import AHAP Json", Application.streamingAssetsPath, "ahap");

				if (!string.IsNullOrEmpty(selectedPath))
				{
					var ahap = CHSerializer.Deserialize(File.ReadAllText(selectedPath));

					// Clear and import...
					ahapAsset.Events.Clear();
					ahapAsset.Parameters.Clear();
					ahapAsset.ParameterCurves.Clear();

					foreach (var entry in ahap.Pattern)
					{
						if (!(entry.Event is null))
						{
							switch (entry.Event.EventType)
							{
								case CHHapticEventType.AudioContinuous:
									ahapAsset.Events.Add(new CHHapticAudioContinuousEvent(entry.Event));
									break;
								case CHHapticEventType.AudioCustom:
									ahapAsset.Events.Add(new CHHapticAudioCustomEvent(entry.Event));
									break;
								case CHHapticEventType.HapticContinuous:
									ahapAsset.Events.Add(new CHHapticContinuousEvent(entry.Event));
									break;
								case CHHapticEventType.HapticTransient:
									ahapAsset.Events.Add(new CHHapticTransientEvent(entry.Event));
									break;
								default:
									Debug.LogWarning($"Encountered unknown event type: {entry.Event.EventType}");
									break;
							}
						}
						else if (!(entry.Parameter is null))
						{
							ahapAsset.Parameters.Add(entry.Parameter);
						}
						else if (!(entry.ParameterCurve is null))
						{
							ahapAsset.ParameterCurves.Add(entry.ParameterCurve);
						}
					}
				}
			}

			if (GUILayout.Button(_exportButton, EditorStyles.toolbarButton))
			{
				var selectedPath = EditorUtility.SaveFilePanelInProject(_exportToAhapBtnTxt, _exportDefaultFilename, "ahap", _exportHelperTxt);

				if (!string.IsNullOrEmpty(selectedPath))
				{
					var serialized = CHSerializer.Serialize(ahapAsset.GetPattern());
					File.WriteAllText(selectedPath, serialized);
					AssetDatabase.ImportAsset(selectedPath, ImportAssetOptions.ForceSynchronousImport);

					Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(serialized);
				}
			}

			if (GUILayout.Button(_resetButton, EditorStyles.toolbarButton))
			{
				if (EditorUtility.DisplayDialog("Reset AHAP", "Are you sure you want to reset this AHAP asset?", "Reset", "Cancel"))
				{
					ahapAsset.Events.Clear();

					ahapAsset.Events.Add(new CHHapticTransientEvent());

					ahapAsset.Parameters.Clear();
					ahapAsset.ParameterCurves.Clear();
				}
			}
			GUILayout.EndHorizontal();
		}

		#region Reorderable List Implementation

		/// <summary>
		/// Creates a ReorderableList, implementing the ReorderableList interface
		/// </summary>
		/// <remarks>
		/// Adapted from code found at:
		/// https://gist.github.com/JesseHamburger/3d17f3892e671a4ae17873ec4e8a0926
		/// </remarks>
		/// <param name="obj">The serialized HapticPlayer component</param>
		/// <param name="prop">The serialized property</param>
		/// <param name="label">Label for the list, e.g. "Parameter Curves"</param>
		/// <param name="elementLabelPropertyName">The sub-property to search for, e.g. "DynamicParameters"</param>
		/// <param name="itemType">The type of the sub-property, e.g. CHHapticEvent</param>
		/// <param name="enumType">The corresponding enum, e.g. CHHapticEventType or CHHapticDynamicParameterId</param>
		/// <returns>A reorderable list that is draggable, can be added to, and removed from.</returns>
		private ReorderableList CreateList(SerializedObject obj, SerializedProperty prop, string label, string elementLabelPropertyName, Type itemType, Type enumType)
		{
			const int padding = 10;

			var l = new ReorderableList(obj, prop, true, true, true, false)
			{
				drawHeaderCallback = rect =>
				{
					EditorGUI.LabelField(rect, label);
				}
			};

			l.drawElementCallback = (rect, idx, active, focused) =>
			{
				if (idx < l.count)
				{
					var element = l.serializedProperty.GetArrayElementAtIndex(idx);

					if (!(element is null))
					{
						var types = element.FindPropertyRelative(elementLabelPropertyName).enumDisplayNames;
						var stringValue = types[element.FindPropertyRelative(elementLabelPropertyName).enumValueIndex];

						// Strip off "Control" for the Dynamic Parameter and Parameter Curve ParameterIDs
						var parts = stringValue.Split(new[] { " Control" },
							StringSplitOptions.RemoveEmptyEntries);
						stringValue = parts[0];

						EditorGUI.PropertyField(new Rect(rect.x += padding, rect.y,
							EditorGUIUtility.currentViewWidth - padding - 75, EditorGUIUtility.singleLineHeight),
							element, new GUIContent(stringValue), true);
					}
				}
			};

			l.elementHeightCallback = idx =>
			{
				if (idx < l.count)
				{
					var element = l.serializedProperty.GetArrayElementAtIndex(idx);

					if (!(element is null))
					{
						var propertyHeight = EditorGUI.GetPropertyHeight(element, true);
						var spacing = EditorGUIUtility.singleLineHeight / 2;
						return spacing + propertyHeight;
					}
				}
				return 0;
			};

			l.drawElementBackgroundCallback = (rect, idx, active, focused) =>
			{
				if (idx < l.count)
				{
					var element = l.serializedProperty.GetArrayElementAtIndex(idx);

					if (!(element is null))
					{
						rect.height = EditorGUI.GetPropertyHeight(element, true);
					}
				}
				rect.height = 0;
			};

			l.onRemoveCallback = li =>
			{
				prop.DeleteArrayElementAtIndex(li.index);
				serializedObject.ApplyModifiedProperties();
			};

			l.onAddDropdownCallback = (rect, li) =>
			{
				var menu = new GenericMenu();
				foreach (int t in Enum.GetValues(enumType))
				{
					GUIContent option;
					if (itemType == typeof(CHHapticEvent))
					{
						option = new GUIContent(((CHHapticEventType)t).GetDescription());
					}
					else if (itemType == typeof(CHHapticParameter) || itemType == typeof(CHHapticParameterCurve))
					{
						option = new GUIContent(((CHHapticDynamicParameterID)t).GetDescription());
					}
					else
					{
						option = new GUIContent(Enum.GetName(enumType, t));
					}


					menu.AddItem(option, false, () =>
					{
						serializedObject.Update();

						if (itemType == typeof(CHHapticEvent))
						{
							prop.arraySize += 1;
							var newSerialEvent = prop.GetArrayElementAtIndex(prop.arraySize - 1);
							var newSerialEventType = newSerialEvent.FindPropertyRelative("EventType");

							switch ((CHHapticEventType)t)
							{
								case CHHapticEventType.HapticTransient:
									newSerialEventType.enumValueIndex = (int)CHHapticEventType.HapticTransient;
									break;
								case CHHapticEventType.HapticContinuous:
									newSerialEventType.enumValueIndex = (int)CHHapticEventType.HapticContinuous;
									newSerialEvent.FindPropertyRelative("EventDuration").floatValue = 1f;
									break;
								case CHHapticEventType.AudioContinuous:
									newSerialEventType.enumValueIndex = (int)CHHapticEventType.AudioContinuous;
									newSerialEvent.FindPropertyRelative("EventDuration").floatValue = 1f;
									break;
								case CHHapticEventType.AudioCustom:
									newSerialEventType.enumValueIndex = (int)CHHapticEventType.AudioCustom;
									newSerialEvent.FindPropertyRelative("EventDuration").floatValue = 0f;
									newSerialEvent.FindPropertyRelative("EventWaveform").objectReferenceValue = null;
									break;
								default:
									Debug.LogWarning("Unknown event type.");
									break;
							}

							newSerialEvent.FindPropertyRelative("Time").floatValue = 0f;
							newSerialEvent.FindPropertyRelative("EventParameters").ClearArray();
						}
						else if (itemType == typeof(CHHapticParameter))
						{
							_serializedDynamicParameters.arraySize += 1;
							var newDynamicParam = _serializedDynamicParameters.GetArrayElementAtIndex(_serializedDynamicParameters.arraySize - 1);
							newDynamicParam.FindPropertyRelative("ParameterID").enumValueIndex = t;
							newDynamicParam.FindPropertyRelative("ParameterValue").floatValue = CHHapticCapabilities.DefaultValueForDynamicParameter(t);
						}
						else if (itemType == typeof(CHHapticParameterCurve))
						{
							_serializedParameterCurves.arraySize += 1;

							// Force the newly created Parameter Curve to be expanded
							var newParamCurve = _serializedParameterCurves.GetArrayElementAtIndex(_serializedParameterCurves.arraySize - 1);
							newParamCurve.FindPropertyRelative("ParameterID").enumValueIndex = t;
						}
						serializedObject.ApplyModifiedProperties();

						// Force the newly added element to be expanded
						serializedObject.Update();
						l.serializedProperty.GetArrayElementAtIndex(l.serializedProperty.arraySize - 1).isExpanded = true;
						serializedObject.ApplyModifiedProperties();
					});
				}

				menu.ShowAsContext();
			};

			return l;
		}
	}

	#endregion Reorderable list
}
