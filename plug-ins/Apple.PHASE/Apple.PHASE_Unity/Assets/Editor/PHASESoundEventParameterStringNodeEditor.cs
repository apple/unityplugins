using XNodeEditor;
using UnityEditor;
using Apple.PHASE;

[CustomNodeEditor(typeof(PHASESoundEventParameterString))]
public class PHASESoundEventParameterStringNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        serializedObject.Update();
        EditorGUIUtility.labelWidth = 100;
        var node = target as PHASESoundEventParameterString;
        NodeEditorGUILayout.PortField(node.GetInputPort("ParentNode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_parameterName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultValue"));
        serializedObject.ApplyModifiedProperties();
    }
}
