using XNodeEditor;
using UnityEditor;
using Apple.PHASE;
using UnityEngine;

[CustomNodeEditor(typeof(PHASESoundEventMappedParameter))]
public class PHASESoundEventMappedParameterNodeEditor : NodeEditor
{
    PHASESoundEventMappedParameter _node;

    public override void OnBodyGUI()
    {
        serializedObject.Update();
        _node = target as PHASESoundEventMappedParameter;

        EditorGUIUtility.labelWidth = 100;

        NodeEditorGUILayout.PortField(_node.GetInputPort("ParentNode"));

        for (var i = 0; i < _node.EnvelopeSegments.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                _node.EnvelopeSegments.RemoveAt(i);
                i--;
            }
            else
            {
                EditorGUILayout.BeginVertical();
                var entry = serializedObject.FindProperty("EnvelopeSegments").GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(entry.FindPropertyRelative("X"));
                EditorGUILayout.PropertyField(entry.FindPropertyRelative("Y"));
                EditorGUILayout.PropertyField(entry.FindPropertyRelative("CurveType"));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("+"))
        {
            _node.EnvelopeSegments.Add(new PHASESoundEventMappedParameter.Segment());
        }

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Parameter"));
        serializedObject.ApplyModifiedProperties();
    }
}