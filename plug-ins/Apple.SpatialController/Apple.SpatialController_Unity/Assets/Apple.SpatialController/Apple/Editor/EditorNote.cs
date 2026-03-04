using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Note))]
public class EditorNote : Editor
{
    private SerializedProperty notesProperty;
    private SerializedProperty isEditableProperty;
    private Vector2 scrollPosition;

    private void OnEnable()
    {
        notesProperty = serializedObject.FindProperty("notes");
        isEditableProperty = serializedObject.FindProperty("isEditable");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Note", EditorStyles.boldLabel);
        
        string currentText = notesProperty.stringValue;
        bool isEditable = isEditableProperty.boolValue;
        
        
        if (isEditable)
        {
            // Editable mode - show text area
            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
            textAreaStyle.wordWrap = true;

            // Calculate height based on content
            float textHeight = textAreaStyle.CalcHeight(new GUIContent(currentText), EditorGUIUtility.currentViewWidth - 40);
            float minHeight = 60f;
            float maxHeight = 200f;
            float finalHeight = Mathf.Clamp(textHeight + 10f, minHeight, maxHeight);

            // If content exceeds max height, use scroll view
            if (textHeight > maxHeight - 10f)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(maxHeight));
                string newNotes = EditorGUILayout.TextArea(currentText, textAreaStyle, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();

                if (newNotes != notesProperty.stringValue)
                {
                    notesProperty.stringValue = newNotes;
                }
            }
            else
            {
                // Normal expanding text area
                string newNotes = EditorGUILayout.TextArea(currentText, textAreaStyle, GUILayout.Height(finalHeight));

                if (newNotes != notesProperty.stringValue)
                {
                    notesProperty.stringValue = newNotes;
                }
            }
        }
        else
        {
            // Locked mode - show as paragraph label
            if (!string.IsNullOrEmpty(currentText))
            {
                GUIStyle paragraphStyle = new GUIStyle(EditorStyles.label);
                paragraphStyle.wordWrap = true;
                paragraphStyle.richText = true;
                paragraphStyle.padding = new RectOffset(10, 10, 10, 10);
                paragraphStyle.normal.background = MakeBackgroundTexture(2, 2, new Color(0.3f, 0.3f, 0.3f, 0.2f));

                // Calculate height for the paragraph
                float paragraphHeight = paragraphStyle.CalcHeight(new GUIContent(currentText), EditorGUIUtility.currentViewWidth - 40);
                float maxParagraphHeight = 200f;

                if (paragraphHeight > maxParagraphHeight)
                {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(maxParagraphHeight));
                    EditorGUILayout.LabelField(currentText, paragraphStyle, GUILayout.ExpandHeight(true));
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    EditorGUILayout.LabelField(currentText, paragraphStyle, GUILayout.Height(paragraphHeight));
                }
            }
            else
            {
                EditorGUILayout.LabelField("(No notes)", EditorStyles.centeredGreyMiniLabel);
            }
        }
        
        EditorGUILayout.LabelField($"Characters: {notesProperty.stringValue.Length}", EditorStyles.miniLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        
        GUI.enabled = isEditable;
        if (GUILayout.Button("Clear Notes", GUILayout.Width(100)))
        {
            if (EditorUtility.DisplayDialog("Clear Notes", "Are you sure you want to clear all notes?", "Yes", "No"))
            {
                notesProperty.stringValue = "";
            }
        }
        GUI.enabled = true;
        
        if (GUILayout.Button("Copy to Clipboard", GUILayout.Width(120)))
        {
            EditorGUIUtility.systemCopyBuffer = notesProperty.stringValue;
        }
        
        if (GUILayout.Button(isEditable ? "🔓" : "🔒", GUILayout.Width(30)))
        {
            isEditableProperty.boolValue = !isEditable;
        }
        
        EditorGUILayout.EndHorizontal();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    // Helper method to create a background texture for the paragraph style
    private Texture2D MakeBackgroundTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        
        Texture2D backgroundTexture = new Texture2D(width, height);
        backgroundTexture.SetPixels(pixels);
        backgroundTexture.Apply();
        
        return backgroundTexture;
    }
}
