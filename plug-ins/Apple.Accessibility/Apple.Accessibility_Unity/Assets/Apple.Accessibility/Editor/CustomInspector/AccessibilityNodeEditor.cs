using UnityEngine;
using UnityEditor;

namespace Apple.Accessibility.Editor
{
    /// <summary>
    /// The custom inspector for a AccessibilityNode component.
    /// </summary>
    [CustomEditor(typeof(AccessibilityNode)), CanEditMultipleObjects]
    public class AccessibilityNodeEditor : UnityEditor.Editor
    {
        bool _isAccessibilityElement = true;
        bool _userProvidedIsAccessibilityElement;
        bool _userProvidedAccessibilityTraits;
        bool _userProvidedAccessibilityLabel;
        bool _userProvidedAccessibilityValue;
        bool _userProvidedAccessibilityHint;
        bool _userProvidedAccessibilityIdentifier;
        bool _userProvidedAccessibilityViewIsModal;

        public void OnEnable()
        {
            _userProvidedAccessibilityTraits = serializedObject.FindProperty("_userAccessibilityTraitsProvided").boolValue;
            _userProvidedAccessibilityLabel = serializedObject.FindProperty("_userAccessibilityLabelProvided").boolValue;
            _userProvidedAccessibilityValue = serializedObject.FindProperty("_userAccessibilityValueProvided").boolValue;
            _userProvidedAccessibilityHint = serializedObject.FindProperty("_userAccessibilityHintProvided").boolValue;
            _userProvidedIsAccessibilityElement = serializedObject.FindProperty("_userIsAccessibilityElementProvided").boolValue;
            _userProvidedAccessibilityIdentifier = serializedObject.FindProperty("_userAccessibilityIdentifierProvided").boolValue;
            _userProvidedAccessibilityViewIsModal = serializedObject.FindProperty("_userAccessibilityViewIsModalProvided").boolValue;
        }

        public override void OnInspectorGUI()
        {
            if (_userProvidedIsAccessibilityElement)
            {
                _isAccessibilityElement = serializedObject.FindProperty("m_userIsAccessibilityElement").boolValue;
            }
            else
            {
                _isAccessibilityElement = false;
            }
            bool oldIsAxElement = serializedObject.FindProperty("m_userIsAccessibilityElement").boolValue;

            _isAccessibilityElement = EditorGUILayout.BeginToggleGroup("Is Accessibility Element", serializedObject.FindProperty("m_userIsAccessibilityElement").boolValue);
            if (_isAccessibilityElement != oldIsAxElement)
            {
                serializedObject.FindProperty("_userIsAccessibilityElementProvided").boolValue = true;
                serializedObject.ApplyModifiedProperties();
            }
            if (_isAccessibilityElement && _userProvidedAccessibilityViewIsModal)
            {
                serializedObject.FindProperty("m_userAccessibilityViewIsModal").boolValue = false;
            }
            serializedObject.FindProperty("m_userIsAccessibilityElement").boolValue = _isAccessibilityElement;

            
            EditorGUILayout.BeginHorizontal();
            _userProvidedAccessibilityTraits = EditorGUILayout.BeginToggleGroup("Traits", _userProvidedAccessibilityTraits);
            serializedObject.FindProperty("_userAccessibilityTraitsProvided").boolValue = _userProvidedAccessibilityTraits;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_userAccessibilityTraits"), new GUIContent("", "The combination of accessibility traits that best characterizes the accessibility element."), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _userProvidedAccessibilityLabel = EditorGUILayout.BeginToggleGroup("Label", _userProvidedAccessibilityLabel);
            serializedObject.FindProperty("_userAccessibilityLabelProvided").boolValue = _userProvidedAccessibilityLabel;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_userAccessibilityLabel"), new GUIContent("", "A succinct label in a localized string that identifies the accessibility element."), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _userProvidedAccessibilityValue = EditorGUILayout.BeginToggleGroup("Value", _userProvidedAccessibilityValue);
            serializedObject.FindProperty("_userAccessibilityValueProvided").boolValue = _userProvidedAccessibilityValue;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_userAccessibilityValue"), new GUIContent("", "A localized string that contains the value of the accessibility element."), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _userProvidedAccessibilityHint = EditorGUILayout.BeginToggleGroup("Hint", _userProvidedAccessibilityHint);
            serializedObject.FindProperty("_userAccessibilityHintProvided").boolValue = _userProvidedAccessibilityHint;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_userAccessibilityHint"), new GUIContent("", "A localized string that contains a brief description of the result of performing an action on the accessibility element."), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _userProvidedAccessibilityIdentifier = EditorGUILayout.BeginToggleGroup("Identifier", _userProvidedAccessibilityIdentifier);
            serializedObject.FindProperty("_userAccessibilityIdentifierProvided").boolValue = _userProvidedAccessibilityIdentifier;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_userAccessibilityIdentifier"), new GUIContent("", "A string that identifies the element."), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndToggleGroup();

            if (!_isAccessibilityElement)
            {
                EditorGUILayout.BeginHorizontal();
                _userProvidedAccessibilityViewIsModal = EditorGUILayout.BeginToggleGroup("Is Modal", _userProvidedAccessibilityViewIsModal);
                serializedObject.FindProperty("_userAccessibilityViewIsModalProvided").boolValue = _userProvidedAccessibilityViewIsModal && !_isAccessibilityElement;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_userAccessibilityViewIsModal"), new GUIContent("", "A Boolean value that indicates whether VoiceOver ignores the accessibility elements within views that are siblings of the element."), GUILayout.ExpandWidth(true));
                EditorGUILayout.EndToggleGroup();
                EditorGUILayout.EndHorizontal();
                serializedObject.ApplyModifiedProperties();
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}
