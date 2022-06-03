using UnityEditor;
using Apple.PHASE;
using XNodeEditor;

[CustomNodeEditor(typeof(PHASEMixer))]
public class PHASEMixerNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        EditorGUIUtility.labelWidth = 100;
        base.OnBodyGUI();
    }
}