using UnityEditor;
using UnityEngine;
using XNodeEditor;
using Apple.PHASE;

[CustomNodeEditor(typeof(PHASESoundEventBlendNode))]
public class PHASESoundEventBlendNodeEditor : NodeEditor
{
    private PHASESoundEventBlendNode _node;

    private BlendMode _blendMode = BlendMode.AutoDistance;

    private enum BlendMode
    {
        Parameter,
        AutoDistance
    }

    public override void OnBodyGUI()
    {
        serializedObject.Update();
        _node = target as PHASESoundEventBlendNode;

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
                if (i != 0 && _node.Entries.Count > 1) // Do not display these for leftmost node.
                {
                    EditorGUILayout.PropertyField(entry.FindPropertyRelative("LowValue"));
                    EditorGUILayout.PropertyField(entry.FindPropertyRelative("FullGainAtLow"));
                }
                if (i != _node.Entries.Count - 1) // Do not display these for rightmost mode.
                {
                    EditorGUILayout.PropertyField(entry.FindPropertyRelative("FullGainAtHigh"));
                    EditorGUILayout.PropertyField(entry.FindPropertyRelative("HighValue"));
                }
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
        CreateBlendModeDropdown();
        if (_blendMode == BlendMode.AutoDistance)
        {
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Mixer"));
        }
        else
        {
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Parameter"));
        }
    }

    void CreateBlendModeDropdown()
    {
        _blendMode = _node.UseDistanceBlend ? BlendMode.AutoDistance : BlendMode.Parameter;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Blend Mode"));
        _blendMode = (BlendMode)EditorGUILayout.EnumPopup(_blendMode);
        _node.UseDistanceBlend = _blendMode == BlendMode.Parameter ? false : true;
        EditorGUILayout.EndHorizontal();
    }
}
