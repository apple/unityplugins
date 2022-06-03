using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.Accessibility.Demo
{
    internal class MagicTap : MonoBehaviour
    {

        public AccessibilityNode axObject = null;
        public Button backButton = null;

        void Start()
        {
            axObject.onAccessibilityPerformMagicTap = performMagicTap;
        }

        public bool performMagicTap()
        {
            backButton.onClick.Invoke();
            return true;
        }
    }
}
