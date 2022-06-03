using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Apple.Accessibility.UnitTests
{
    public class AccessibilitySettings_TestSuite
    {
        
        int _voiceOverOnChangedReceivedCount = 0;
        bool _voiceOverOnChangedValue = false;
        [UnityTest]
        public IEnumerator Test_UIAccessibilityIsVoiceOverRunningNotification()
        {
            UnityEngine.Assertions.Assert.IsFalse(_voiceOverOnChangedValue, "VoiceOver shouldn't have received a onChanged value");
            UnityEngine.Assertions.Assert.IsFalse(AccessibilitySettings.IsVoiceOverRunning);

            AccessibilitySettings.onIsVoiceOverRunningChanged += onUIAccessibilityIsVoiceOverRunningChanged;

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
            AccessibilityTests.PostFeatureEnabledNotification("UIAccessibilityVoiceOverTouchStatusChanged", true);
            new WaitForSeconds(0.1f);

            UnityEngine.Assertions.Assert.AreEqual(_voiceOverOnChangedReceivedCount, 1);
#else 
            AccessibilitySettings.onIsVoiceOverRunningChanged.Invoke();

            new WaitForSeconds(0.1f);

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
            UnityEngine.Assertions.Assert.IsTrue(_voiceOverOnChangedValue);
            UnityEngine.Assertions.Assert.IsTrue(AccessibilitySettings.IsVoiceOverRunning);
#endif
            UnityEngine.Assertions.Assert.AreEqual(_voiceOverOnChangedReceivedCount, 1);

            AccessibilitySettings.onIsVoiceOverRunningChanged.Invoke();

            new WaitForSeconds(0.1f);
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
            UnityEngine.Assertions.Assert.IsFalse(_voiceOverOnChangedValue);
            UnityEngine.Assertions.Assert.IsFalse(AccessibilitySettings.IsVoiceOverRunning);
#endif
            UnityEngine.Assertions.Assert.AreEqual(_voiceOverOnChangedReceivedCount, 2);
#endif

            yield return null;
        }

        void onUIAccessibilityIsVoiceOverRunningChanged()
        {
            _voiceOverOnChangedReceivedCount += 1;
            _voiceOverOnChangedValue = AccessibilitySettings.IsVoiceOverRunning;
        }

    }
}
