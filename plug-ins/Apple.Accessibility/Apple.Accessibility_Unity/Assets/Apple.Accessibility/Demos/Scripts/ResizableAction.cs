using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.Accessibility.Demo
{
    internal class ResizableAction : MonoBehaviour
    {
        private iconSize _iconSize = iconSize.Medium;

        private enum iconSize
        {
            Small,
            Medium,
            Large
        }
        void Start()
        {
            gameObject.AddComponent<AccessibilityNode>();
            _adjustSize();
        }

        private void _adjustSize()
        {
            switch (_iconSize)
            {
                case iconSize.Small: gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 1.0f); break;
                case iconSize.Medium: gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f); break;
                case iconSize.Large: gameObject.GetComponent<RectTransform>().localScale = new Vector3(2.0f, 2.0f, 1.0f); break;

            }
        }

        public AccessibilityCustomAction[] accessibilityCustomActions()
        {
            AccessibilityCustomAction Small = new AccessibilityCustomAction("Small", makeSmall);
            AccessibilityCustomAction Medium = new AccessibilityCustomAction("Medium", makeMedium);
            AccessibilityCustomAction Large = new AccessibilityCustomAction("Large", makeLarge);

            return new AccessibilityCustomAction[] { Small, Medium, Large };
        }

        private bool makeSmall()
        {
            if (_iconSize == iconSize.Small)
            {
                return false;
            }
            AccessibilityNotification.PostAnnouncementNotification("changed size to small");
            _iconSize = iconSize.Small;
            _adjustSize();
            return true;
        }

        private bool makeMedium()
        {
            if (_iconSize == iconSize.Medium)
            {
                return false;
            }
            AccessibilityNotification.PostAnnouncementNotification("changed size to medium");
            _iconSize = iconSize.Medium;
            _adjustSize();
            return true;
        }

        private bool makeLarge()
        {
            if (_iconSize == iconSize.Large)
            {
                return false;
            }
            AccessibilityNotification.PostAnnouncementNotification("changed size to large");
            _iconSize = iconSize.Large;
            _adjustSize();
            return true;
        }
    }
}
