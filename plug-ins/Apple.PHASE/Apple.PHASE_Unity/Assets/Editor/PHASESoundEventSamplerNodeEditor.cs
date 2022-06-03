using Apple.PHASE;
using UnityEditor;
using System.IO;
using UnityEngine;
using UnityPickers;
using System.Linq;
using XNodeEditor;

[CustomNodeEditor(typeof(PHASESoundEventSamplerNode))]
public class PHASESoundEventSamplerNodeEditor : NodeEditor
{
    PHASESoundEventSamplerNode _node;

    public override void OnBodyGUI()
    {
        serializedObject.Update();
        _node = serializedObject.targetObject as PHASESoundEventSamplerNode;

        EditorGUIUtility.labelWidth = 110;

        NodeEditorGUILayout.PortField(_node.GetInputPort("ParentNode"));

        SerializedProperty isStreamingAsset = serializedObject.FindProperty("_isStreamingAsset");
        EditorGUILayout.PropertyField(isStreamingAsset);
        if (isStreamingAsset.boolValue)
        {
            SerializedProperty streamingAsset = serializedObject.FindProperty("_streamingAssetAudioClip");
            string fullPath = AssetDatabase.GetAssetPath(streamingAsset.objectReferenceValue);
            AssetPicker.PropertyField(EditorGUILayout.GetControlRect(), streamingAsset, streamingAsset.GetType().GetField("name"), new GUIContent("Audio Clip"), typeof(Object), he => AssetFilter(he.Path));

            if (streamingAsset.objectReferenceValue != null)
            {
                _node.SetSubDirectory(GetSubDirectory(Path.GetDirectoryName(fullPath)));
                _node.SetAssetName(Path.GetFileName(fullPath));
            }
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_audioClip"));
        }

        SerializedProperty calibrationMode = serializedObject.FindProperty("_calibrationMode");
        EditorGUILayout.PropertyField(calibrationMode);
        switch(calibrationMode.enumValueIndex)
        {
            // Calibration Mode None
            case 0:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_levelNone"));
                break;
            // Calibration Mode RelativeSpl
            case 1:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_levelRelativeSpl"));
                break;
            // Calibration Mode AbsoluteSpl
            case 2:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_levelAbsoluteSpl"));
                break;

        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_looping"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Mixer"));
        serializedObject.ApplyModifiedProperties();
    }

    private bool AssetFilter(string path)
    {
        return PHASESoundEventSamplerNode.m_validAudioExtensions.Any(path.EndsWith) && path.Contains("StreamingAssets");
    }

    private string GetSubDirectory(string path)
    {
        string[] segments = path.Split(Path.DirectorySeparatorChar);
        int i;
        for (i = 0; i < segments.Length; i++)
        {
            if (segments[i] == "StreamingAssets")
            {
                break;
            }
        }

        string subDirectory = "";
        for (int j = i + 1; j < segments.Length; j++)
        {
            subDirectory = Path.Combine(subDirectory, segments[j]);
        }

        return subDirectory;
    }
}
