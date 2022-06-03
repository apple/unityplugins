using UnityEngine;
using UnityEngine.UI;

namespace Apple.Accessibility.Demo
{
    internal class AccessibilitySettingsWatcher : MonoBehaviour
    {
        public Text voText = null;
        public Text invertText = null;
        public RawImage image = null;
        void Start()
        {
            _updateSettingsText();
           
        }

        void OnEnable()
        {
            AccessibilitySettings.onIsVoiceOverRunningChanged += _settingChanged;
            AccessibilitySettings.onIsInvertColorsEnabledChanged += _settingChanged;
        }
        void OnDisable()
        {
            AccessibilitySettings.onIsVoiceOverRunningChanged -= _settingChanged;
            AccessibilitySettings.onIsInvertColorsEnabledChanged -= _settingChanged;
        }

        void _settingChanged()
        {
            _updateSettingsText();
        }

        void _updateSettingsText()
        {
            bool voEnabled = AccessibilitySettings.IsVoiceOverRunning;
            bool invertEnabled = AccessibilitySettings.IsInvertColorsEnabled;
            voText.text = voEnabled ? "VoiceOver: ON" : "VoiceOver: OFF";
            invertText.text = invertEnabled ? "Invert Colors: ON" : "Invert Colors: OFF";
            float hue = 29f / 360f;

            if (!invertEnabled)
            {
                hue += 0.5f;
            }
            image.color = Color.HSVToRGB(hue, 1.0f, 1.0f);
        }
    }
}
