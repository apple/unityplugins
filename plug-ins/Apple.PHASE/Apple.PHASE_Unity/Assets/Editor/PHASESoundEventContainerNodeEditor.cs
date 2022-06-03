using UnityEngine;
using Apple.PHASE;
using XNodeEditor;
using UnityEditor;

[CustomNodeEditor(typeof(PHASESoundEventContainerNode))]
public class PHASESoundEventContainerNodeEditor : NodeEditor
{
    PHASESoundEventContainerNode _node;

    public override void OnBodyGUI()
    {
        _node = target as PHASESoundEventContainerNode;
        NodeEditorGUILayout.PortField(_node.GetInputPort("ParentNode"));

        for (var i = 0; i < _node.Nodes.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                _node.RemoveEntry(i);
                i--;
            }
            else
            {
                NodeEditorGUILayout.PortField(new GUIContent("Child Node"), _node.GetPort(_node.Nodes[i]));
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("+"))
        {
            _node.AddEntry();
        }
    }
}