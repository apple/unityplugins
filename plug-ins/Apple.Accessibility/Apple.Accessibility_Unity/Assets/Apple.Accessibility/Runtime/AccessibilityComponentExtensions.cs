using System.Linq;
using UnityEngine;

namespace Apple.Accessibility
{
    internal class AccessibilityComponentExtensions
    {
        internal static AccessibilityTrait AccessibilityTraitsForComponent(UnityEngine.UI.Toggle toggle, GameObject go)
        {
            if (toggle.isOn)
            {
                if ( toggle.isActiveAndEnabled )
                {
                    return AccessibilityTrait.Selected;
                }
                return AccessibilityTrait.Selected | AccessibilityTrait.NotEnabled;
            }
            if (toggle.isActiveAndEnabled)
            {
                return AccessibilityTrait.None;
            }
            return AccessibilityTrait.NotEnabled;
        }

        internal static string AccessibilityLabelForComponent(UnityEngine.UI.Toggle toggle, GameObject go)
        {
            return toggle.GetComponentInChildren<UnityEngine.UI.Text>()?.text;
        }

        internal static AccessibilityTrait AccessibilityTraitsForComponent(UnityEngine.UI.Slider slider, GameObject go)
        {
            if (slider.isActiveAndEnabled)
            {
                return AccessibilityTrait.Adjustable;
            }
            return AccessibilityTrait.Adjustable | AccessibilityTrait.NotEnabled;
        }

        internal static string AccessibilityValueForComponent(UnityEngine.UI.Slider slider, GameObject go)
        {
            return slider.value.ToString();
        }

        internal static AccessibilityTrait AccessibilityTraitsForComponent(UnityEngine.UI.Button button, GameObject go)
        {
            if (button.isActiveAndEnabled)
            {
                return AccessibilityTrait.Button;
            }
            return AccessibilityTrait.Button | AccessibilityTrait.NotEnabled;
        }

        internal static string AccessibilityLabelForComponent(UnityEngine.UI.Button button, GameObject go)
        {
            return button.GetComponentInChildren< UnityEngine.UI.Text>()?.text;
        }

        internal static AccessibilityTrait AccessibilityTraitsForComponent(UnityEngine.UI.InputField inputField, GameObject go)
        {
            if (inputField.isActiveAndEnabled)
            {
                return AccessibilityTrait.StaticText;
            }
            return AccessibilityTrait.StaticText | AccessibilityTrait.NotEnabled;
        }

        internal static string AccessibilityValueForComponent(UnityEngine.UI.InputField inputField, GameObject go)
        {
            var textValue = inputField.text;
            if (string.IsNullOrEmpty(textValue))
            {
                if (inputField.placeholder is UnityEngine.UI.Text)
                {
                    return (inputField.placeholder as UnityEngine.UI.Text).text;
                }
            }

            return textValue;
        }

        internal static AccessibilityTrait AccessibilityTraitsForComponent(UnityEngine.UI.Text text, GameObject go)
        {
            if (text.isActiveAndEnabled)
            {
                return AccessibilityTrait.StaticText;
            }
            else
            {
                return AccessibilityTrait.StaticText | AccessibilityTrait.NotEnabled;
            }
        }

        internal static string AccessibilityLabelForComponent(UnityEngine.UI.Text text, GameObject go)
        {
            return text.text;
        }

        internal static Rect AccessibilityFrameForComponent(SpriteRenderer render, GameObject go)
        {
            var screenPos = Camera.main.WorldToScreenPoint(go.transform.position);

            var width = render.sprite.textureRect.width * go.transform.localScale.x;
            var height = render.sprite.textureRect.height * go.transform.localScale.y;
            var x = screenPos.x - width / 2.0f;
            var y = screenPos.y + height / 2.0f;

            return new Rect
            {
                x = x,
                y = Screen.height - y,
                width = width,
                height = height
            };
        }

        internal static Rect AccessibilityFrameForComponent(RectTransform rectTransform, GameObject go)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            Vector3[] screenCorners = new Vector3[4];
            for (var i = 0; i < corners.Length; i++)
            {
                var screenPoint = RectTransformUtility.WorldToScreenPoint(null, corners[i]);
                screenCorners[i] = screenPoint;
            }
            float maxX = screenCorners.Max(corner => corner.x);
            float minX = screenCorners.Min(corner => corner.x);
            float maxY = screenCorners.Max(corner => corner.y);
            float minY = screenCorners.Min(corner => corner.y);

            return new Rect(minX, Screen.height - maxY, maxX - minX, maxY - minY);
        }
    }
}
