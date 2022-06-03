using UnityEditor;
using UnityEngine;
using Apple.PHASE;
using XNodeEditor;

[CustomNodeEditor(typeof(PHASESpatialMixer))]
public class PHASESpatialMixerNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        float prevListenerInnerAngle = serializedObject.FindProperty("_listenerDirectivityInnerAngle").floatValue;
        float prevListenerOuterAngle = serializedObject.FindProperty("_listenerDirectivityOuterAngle").floatValue;
        float prevSourceInnerAngle = serializedObject.FindProperty("_sourceDirectivityInnerAngle").floatValue;
        float prevSourceOuterAngle = serializedObject.FindProperty("_sourceDirectivityOuterAngle").floatValue;

        serializedObject.Update();
        EditorGUIUtility.labelWidth = 150;

        var node = target as PHASESpatialMixer;
        NodeEditorGUILayout.PortField(node.GetInputPort("ParentNode"));

        EditorGUILayout.LabelField("Spatial Mixer Properties");
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_directPathModeler"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_earlyReflectionModeler"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_lateReverbModeler"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_cullDistance"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Listener Directivity Properties");
        EditorGUILayout.Space();
        SerializedProperty listenerPreset = serializedObject.FindProperty("_listenerDirectivityPreset");
        EditorGUILayout.PropertyField(listenerPreset, new GUIContent("Preset"));
        if (listenerPreset.enumValueIndex == (int)Helpers.DirectivityPreset.Cone)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_listenerDirectivityInnerAngle"), new GUIContent("Inner Angle"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_listenerDirectivityOuterAngle"), new GUIContent("Outer Angle"));
            float innerAngle = serializedObject.FindProperty("_listenerDirectivityInnerAngle").floatValue;
            float outerAngle = serializedObject.FindProperty("_listenerDirectivityOuterAngle").floatValue;
            // Inner Angle must be less than outer angle.
            if (prevListenerInnerAngle != innerAngle && innerAngle > outerAngle)
            {
                serializedObject.FindProperty("_listenerDirectivityOuterAngle").floatValue = innerAngle;
            }
            else if (prevListenerOuterAngle != outerAngle && outerAngle < innerAngle)
            {
                serializedObject.FindProperty("_listenerDirectivityInnerAngle").floatValue = outerAngle;
            }
        }
        // Only display sharpness if it is not a cone directivity pattern.
        else if (listenerPreset.enumValueIndex != 0)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_listenerDirectivitySharpness"), new GUIContent("Sharpness"));
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Source Directivity Properties");
        EditorGUILayout.Space();
        SerializedProperty sourcePreset = serializedObject.FindProperty("_sourceDirectivityPreset");
        EditorGUILayout.PropertyField(sourcePreset, new GUIContent("Preset"));
        if (sourcePreset.enumValueIndex == (int)Helpers.DirectivityPreset.Cone)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_sourceDirectivityInnerAngle"), new GUIContent("Inner Angle"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_sourceDirectivityOuterAngle"), new GUIContent("Outer Angle"));

            // Inner Angle must be less than outer angle
            float innerAngle = serializedObject.FindProperty("_sourceDirectivityInnerAngle").floatValue;
            float outerAngle = serializedObject.FindProperty("_sourceDirectivityOuterAngle").floatValue;
            if (prevSourceInnerAngle != innerAngle && innerAngle > outerAngle)
            {
                serializedObject.FindProperty("_sourceDirectivityOuterAngle").floatValue = innerAngle;
            }
            else if (prevSourceOuterAngle != outerAngle && outerAngle < innerAngle)
            {
                serializedObject.FindProperty("_sourceDirectivityInnerAngle").floatValue = outerAngle;
            }
        }
        // Only display sharpness if it is not a cone directivity pattern.
        else if (sourcePreset.enumValueIndex != 0)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_sourceDirectivitySharpness"), new GUIContent("Sharpness"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
