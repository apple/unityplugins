using UnityEngine;
using UnityEngine.UI;

namespace Apple.Accessibility.Demo
{
    public class AccessibilityTextSizeAdjustment : MonoBehaviour
    {
        Text axNode;
        int initialSize;

        void Awake()
        {
            axNode = GetComponent<Text>();
            initialSize = axNode.fontSize;
        }

        void Start()
        {
            AdjustTextSize();
        }

        private void OnEnable()
        {
            AccessibilitySettings.onPreferredContentSizeChanged += AdjustTextSize;
        }

        private void OnDisable()
        {
            AccessibilitySettings.onPreferredContentSizeChanged -= AdjustTextSize;
        }

        void AdjustTextSize()
        {
            var scale = AccessibilitySettings.PreferredContentSizeMultiplier;
            axNode.fontSize = (int)(scale * initialSize);
        }
    }
}
