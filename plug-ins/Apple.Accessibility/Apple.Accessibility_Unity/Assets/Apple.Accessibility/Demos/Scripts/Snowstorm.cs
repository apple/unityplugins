using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Apple.Accessibility.Demo
{
    internal class Snowstorm : MonoBehaviour
    {
        public ParticleSystem snowParticleSystem = null;
      
        void Start()
        {
            AccessibilityNode axObject = gameObject.GetComponent<AccessibilityNode>();
            axObject.onAccessibilityPerformMagicTap = magicTap;
        }

        bool magicTap()
        {
            if (snowParticleSystem.isPaused)
            {
                snowParticleSystem.Play();
                AccessibilityNotification.PostAnnouncementNotification("Snowstorm starting");
            }
            else
            {
                snowParticleSystem.Pause();
                AccessibilityNotification.PostAnnouncementNotification("Snowstorm stopped");
            }

            return true;
        }
    }
}
