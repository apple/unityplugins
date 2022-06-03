using UnityEngine;
using UnityEngine.UI;

namespace Apple.Accessibility.Demo
{
    internal class AdjustableLogo : MonoBehaviour
    {
        RawImage rawImage;
        private float hue = 0.0f;

        void Start()
        {
            AccessibilityNode axObject = gameObject.GetComponent<AccessibilityNode>();
            if (axObject == null)
            {
                gameObject.AddComponent<AccessibilityNode>();
            }
            rawImage = gameObject.GetComponent<RawImage>();
        }

        public bool AccessibilityIncrement()
        {
            hue += 0.1f;
            if (hue > 1.0f)
            {
                hue -= 1.0f;
            }
            rawImage.color = Color.HSVToRGB(hue, 1.0f, 1.0f);
            return true;
        }

        public bool AccessibilityDecrement()
        {
            hue -= 0.1f;
            if (hue < 0.0f)
            {
                hue += 1.0f;
            }
            rawImage.color = Color.HSVToRGB(hue, 1.0f, 1.0f);
            return true;
        }

        public string AccessibilityValue()
        {
            return Mathf.Round(hue * 100.0f) + "%, hue";
        }
    }
}
