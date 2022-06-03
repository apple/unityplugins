using XNodeEditor;
using Apple.PHASE;
using System;

[CustomNodeGraphEditor(typeof(PHASESoundEventNodeGraph))]
public class PHASESoundEventGraphEditor : NodeGraphEditor
{
    public override string GetNodeMenuName(Type type)
    {

        if (typeof(PHASESoundEventNode).IsAssignableFrom(type))
        {
            return $"SoundEventNodes/{type.Name}";
        }
        else if (typeof(PHASEMixer).IsAssignableFrom(type))
        {
            return $"Mixers/{type.Name}";
        }
        else if (typeof(PHASESoundEventParameter).IsAssignableFrom(type))
        {
            return $"Parameters/{type.Name}";
        }
        else
        {
            return null;
        }
    }

    public override void OnGUI()
    {
        base.OnGUI();
        window.titleContent.text = "PHASE Sound Event Composer";
    }
}
