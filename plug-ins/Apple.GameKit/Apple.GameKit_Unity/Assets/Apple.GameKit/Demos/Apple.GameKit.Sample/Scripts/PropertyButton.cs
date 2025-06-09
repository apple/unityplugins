using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class PropertyButton : MonoBehaviour
    {
        public PropertyButton Instantiate(GameObject parent, string propertyName = null, string propertyValue = null)
        {
            var propertyButton = Instantiate(this.gameObject, parent.transform, worldPositionStays: false).GetComponent<PropertyButton>();
            propertyButton.PropertyName = propertyName ?? string.Empty;
            propertyButton.PropertyValue = propertyValue ?? string.Empty;
            return propertyButton;
        }

        [SerializeField] private Text _propertyNameText = default;

        public string PropertyName
        {
            get => _propertyNameText.text;
            set => _propertyNameText.text = value;
        }

        [SerializeField] private Text _propertyValueText = default;

        public string PropertyValue
        {
            get => _propertyValueText.text;
            set => _propertyValueText.text = value;
        }

        public void CopyTextToClipboard()
        {
            TextEditor textEditor = new TextEditor
            {
                text = PropertyValue
            };
            textEditor.SelectAll();
            textEditor.Copy();
        }
    }
}