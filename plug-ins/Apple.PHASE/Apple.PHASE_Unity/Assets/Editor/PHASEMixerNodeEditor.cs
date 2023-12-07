using Apple.PHASE;
using UnityEditor;
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