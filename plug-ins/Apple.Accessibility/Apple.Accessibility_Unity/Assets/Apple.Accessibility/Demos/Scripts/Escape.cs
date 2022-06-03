using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.Accessibility.Demo
{
    internal class Escape : MonoBehaviour
    {
        public Button backButton = null;

        public bool AccessibilityPerformEscape()
        {
            backButton.onClick.Invoke();
            return true;
        }
    }
}
