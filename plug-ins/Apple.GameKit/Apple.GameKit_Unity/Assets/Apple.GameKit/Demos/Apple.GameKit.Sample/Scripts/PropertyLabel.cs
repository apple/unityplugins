using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    public class PropertyLabel : MonoBehaviour
    {
        public PropertyLabel Instantiate(GameObject parent, string labelString = null)
        {
            var propertyLabel = Instantiate(this.gameObject, parent.transform, worldPositionStays: false).GetComponent<PropertyLabel>();
            propertyLabel.LabelString = labelString ?? string.Empty;
            return propertyLabel;
        }

        [SerializeField] private Text _labelText = default;

        public string LabelString
        {
            get => _labelText.text;
            set => _labelText.text = value;
        }
    }
}