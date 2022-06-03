using UnityEngine;
using Apple.PHASE;
using XNodeEditor;
using UnityEditor;

[CustomNodeEditor(typeof(PHASESoundEventSwitchNode))]
public class PHASESoundEventSwitchNodeEditor : NodeEditor
{
    PHASESoundEventSwitchNode _node;

    public override void OnBodyGUI()
    {
        serializedObject.Update();
        _node = target as PHASESoundEventSwitchNode;

        EditorGUIUtility.labelWidth = 100;

        NodeEditorGUILayout.PortField(_node.GetInputPort("ParentNode"));

        for (var i = 0; i < _node.Entries.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                _node.RemoveEntry(i);
                i--;
            }
            else
            {
                EditorGUILayout.BeginVertical();
                var entry = serializedObject.FindProperty("Entries").GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(entry.FindPropertyRelative("SwitchValue"));
                NodeEditorGUILayout.PortField(new GUIContent("Child Node"), _node.GetPort(_node.Entries[i].PortName));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("+"))
        {
            _node.AddEntry();
        }

        EditorGUILayout.Space();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Parameter"));
    }
}
