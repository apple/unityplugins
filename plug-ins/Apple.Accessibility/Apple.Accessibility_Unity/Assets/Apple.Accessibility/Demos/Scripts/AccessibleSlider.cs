using UnityEngine;

namespace Apple.Accessibility.Demo
{
    internal class AccessibleSlider : MonoBehaviour
    {
        public bool AccessibilityIncrement()
        {
            if (!isActiveAndEnabled)
            {
                return false;
            }
            UnityEngine.UI.Slider slider = gameObject.GetComponent<UnityEngine.UI.Slider>();

            float delta = (slider.maxValue - slider.minValue) / 10.0f;
            float target = slider.value + delta;

            slider.value = Mathf.Clamp(target, slider.minValue, slider.maxValue);
            return true;
        }

        public bool AccessibilityDecrement()
        {
            if (!isActiveAndEnabled)
            {
                return false;
            }

            UnityEngine.UI.Slider slider = gameObject.GetComponent<UnityEngine.UI.Slider>();

            float delta = (slider.maxValue - slider.minValue) / 10.0f;
            float target = slider.value - delta;

            slider.value = Mathf.Clamp(target, slider.minValue, slider.maxValue);
            return true;
        }
    }
}
