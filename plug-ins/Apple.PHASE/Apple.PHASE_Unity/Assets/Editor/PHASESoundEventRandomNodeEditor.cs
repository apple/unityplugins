using UnityEditor;
using UnityEngine;
using XNodeEditor;
using Apple.PHASE;

[CustomNodeEditor(typeof(PHASESoundEventRandomNode))]
public class PHASESoundEventRandomNodeEditor : NodeEditor
{
    private PHASESoundEventRandomNode node;

    public override void OnBodyGUI()
    {
        node = target as PHASESoundEventRandomNode;

        serializedObject.Update();

        NodeEditorGUILayout.PortField(node.GetInputPort("ParentNode"));

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
