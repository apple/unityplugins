using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.Accessibility.Demo
{
    internal class CanvasController : MonoBehaviour
    {

        public Button button = null;
        public Toggle toggle1 = null;
        public Toggle toggle2 = null;

        private bool controlsEnabled = true;

        void UpdateEnabledState()
        {
            controlsEnabled = !controlsEnabled;
            if (toggle1 != null)
            {
                toggle1.enabled = controlsEnabled;
                toggle1.interactable = controlsEnabled;
            }
            if (toggle2 != null)
            {
                toggle2.enabled = controlsEnabled;
                toggle2.interactable = controlsEnabled;
            }
            if (button != null)
            {
                button.GetComponentInChildren<Text>().text = controlsEnabled ? "Disable Controls" : "Enable Controls";
            }
        }

        public void ToggleControls()
        {
            UpdateEnabledState();
        }
    }
}
