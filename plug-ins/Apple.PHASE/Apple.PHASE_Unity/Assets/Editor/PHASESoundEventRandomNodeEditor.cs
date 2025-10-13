using Apple.PHASE;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(PHASESoundEventRandomNode))]
public class PHASESoundEventRandomNodeEditor : NodeEditor
{
    private PHASESoundEventRandomNode node;

    public override void OnBodyGUI()
    {
        node = target as PHASESoundEventRandomNode;

        serializedObject.Update();

        EditorGUIUtility.labelWidth = 150;
        NodeEditorGUILayout.PortField(node.GetInputPort("ParentNode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("UniqueSelectionQueueLength"));

        for (var i = 0; i < node.Entries.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                node.RemoveEntry(i);
                i--;
            }
            else
            {
                EditorGUILayout.BeginVertical();
                EditorGUIUtility.labelWidth = 75;
                var entry = serializedObject.FindProperty("Entries").GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(entry.FindPropertyRelative("Weight"));
                NodeEditorGUILayout.PortField(new GUIContent("Child Node"), node.GetPort(node.Entries[i].PortName));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("+"))
        {
            node.AddEntry();
        }
    }
}
