using XNodeEditor;
using UnityEditor;
using Apple.PHASE;

[CustomNodeEditor(typeof(PHASESoundEventParameterDouble))]
public class PHASESoundEventParameterDoubleNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        serializedObject.Update();
        EditorGUIUtility.labelWidth = 100;
        var node = target as PHASESoundEventParameterDouble;
        NodeEditorGUILayout.PortField(node.GetInputPort("ParentNode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_parameterName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_defaultValue"));
        serializedObject.ApplyModifiedProperties();
    }
}