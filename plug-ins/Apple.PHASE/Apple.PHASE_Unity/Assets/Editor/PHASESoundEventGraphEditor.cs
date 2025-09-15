using Apple.PHASE;
using System;
using UnityEngine;
using XNode;
using XNodeEditor;

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
	
    public override Node CreateNode(Type type, Vector2 position)
    {
        Node newNode = base.CreateNode(type, position);
        
        // Only change the name if it is of type PHASEMixer
        if (typeof(Apple.PHASE.PHASEMixer).IsAssignableFrom(type))
        {
            NodeGraph graph = target as NodeGraph;
            Type testType;
            if (type == typeof(PHASEAmbientMixer))
            {
                testType = typeof(PHASEAmbientMixer);
            } 
            else if (type == typeof(PHASESpatialMixer))
            {
                testType = typeof(PHASESpatialMixer);
            }
            else
            {
                testType = typeof(PHASEChannelMixer);
            }

            var typeCount = 0;
            foreach (Node node in graph.nodes)
            {
                if (node.GetType() == testType)
                {
                    typeCount++;
                }
            }

            newNode.name = newNode.name + " " + typeCount.ToString();
        } 
        return newNode;
    }
}
