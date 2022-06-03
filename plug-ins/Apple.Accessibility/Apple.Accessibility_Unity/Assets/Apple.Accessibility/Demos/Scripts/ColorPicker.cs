using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Apple.Accessibility.Demo
{
    internal class ColorPicker : MonoBehaviour, IAccessibilityCustomActions
    {
        void Start()
        {
            gameObject.AddComponent<AccessibilityNode>(); //or add this via the gui based editor
        }

        public AccessibilityCustomAction[] AccessibilityCustomActions()
        {
            AccessibilityCustomAction red = new AccessibilityCustomAction("red", selectRed);
            AccessibilityCustomAction blue = new AccessibilityCustomAction("blue", selectBlue);
            AccessibilityCustomAction green = new AccessibilityCustomAction("green", selectGreen);

            return new AccessibilityCustomAction[] { red, green, blue };
        }

        private bool selectRed()
        {
            setColor(Color.red);
            AccessibilityNotification.PostAnnouncementNotification("changed color to red");
            return true;
        }

        private bool selectBlue()
        {
            setColor(Color.blue);
            AccessibilityNotification.PostAnnouncementNotification("changed color to blue");
            return true;
        }

        private bool selectGreen()
        {
            setColor(Color.green);
            AccessibilityNotification.PostAnnouncementNotification("changed color to green");
            return true;
        }


        private Button button()
        {
            Button b = gameObject.GetComponent<Button>();
            return b;
        }

        private void setColor(Color c)
        {
            Button b = button();
            ColorBlock cb = b.colors;
            cb.normalColor = c;
            cb.selectedColor = c;
            b.colors = cb;
        }
    }
}
