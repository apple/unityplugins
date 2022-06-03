using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Apple.Accessibility.UnitTests
{
    public class AccessibilityObject_TestSuite
    {
        AsyncOperation asyncLoad;

        [OneTimeSetUp]
        public void OneTimeSetupAccessibility()
        {
            asyncLoad = SceneManager.LoadSceneAsync("Assets/Apple.Accessibility/Tests/TestScene.unity");
        }

        private AccessibilityNode ButtonAXObject()
        {
            GameObject button = GameObject.Find("Button1");
            UnityEngine.Assertions.Assert.IsNotNull(button);

            AccessibilityNode axObject = button.GetComponent<AccessibilityNode>();
            UnityEngine.Assertions.Assert.IsNotNull(axObject);
            return axObject;
        }

        private AccessibilityNode SliderAXObject()
        {
            GameObject slider = GameObject.Find("Slider1");
            UnityEngine.Assertions.Assert.IsNotNull(slider);

            AccessibilityNode axObject = slider.GetComponent<AccessibilityNode>();
            UnityEngine.Assertions.Assert.IsNotNull(axObject);
            return axObject;
        }

        [UnityTest]
        public IEnumerator Test_EmptyTest()
        {
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }

        [UnityTest]
        public IEnumerator Test_AccessibilityLabel()
        {
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            AccessibilityNode axObject = ButtonAXObject();
            UnityEngine.Assertions.Assert.AreEqual(axObject.AccessibilityLabel, "Button");

            bool succeeded = AccessibilityTests.RuniOSSideUnitTestWithKeyPathExpectingStringResult(axObject.gameObject.GetInstanceID(), "accessibilityLabel", "Button");
            UnityEngine.Assertions.Assert.IsTrue(succeeded);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_AccessibilityValue()
        {
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            AccessibilityNode axObject = ButtonAXObject();
            UnityEngine.Assertions.Assert.AreEqual(axObject.AccessibilityValue, "ButtonValue");

            bool succeeded = AccessibilityTests.RuniOSSideUnitTestWithKeyPathExpectingStringResult(axObject.gameObject.GetInstanceID(), "accessibilityValue", "ButtonValue");
            UnityEngine.Assertions.Assert.IsTrue(succeeded);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_AccessibilityHint()
        {
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            AccessibilityNode axObject = ButtonAXObject();
            UnityEngine.Assertions.Assert.AreEqual(axObject.AccessibilityHint, "ButtonHint");

            bool succeeded = AccessibilityTests.RuniOSSideUnitTestWithKeyPathExpectingStringResult(axObject.gameObject.GetInstanceID(), "accessibilityHint", "ButtonHint");
            UnityEngine.Assertions.Assert.IsTrue(succeeded);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_AccessibleSlider()
        {
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            AccessibilityNode axObject = SliderAXObject();

            UnityEngine.Assertions.Assert.AreEqual(axObject.AccessibilityValue, "0.5");

            yield return null;
        }


#if HAS_TEST_REPLACEMEMENT_FOR_MAGIC_TAP

        [UnityTest]
        public IEnumerator Test_PerformMagicTap()
        {
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            int magicTapCount = 0;
            int expectedMagicTapCount = 0;

            //Succeeding tap
            AccessibilityNode axObject = ButtonAXObject();
            axObject.onAccessibilityPerformMagicTap = () =>
            {
                magicTapCount += 1;
                return true;
            };

            bool tapSucceeded = AccessibilityRuntime._UnityAX_AccessibilityPerformMagicTap(axObject.gameObject.GetInstanceID());
            expectedMagicTapCount += 1;
            UnityEngine.Assertions.Assert.AreEqual(magicTapCount, expectedMagicTapCount);
            UnityEngine.Assertions.Assert.IsTrue(tapSucceeded);



#if UNITY_IOS && !UNITY_EDITOR
            tapSucceeded = AccessibilityTests.RuniOSUnitTestWithName("_test_performMagicTapOnButton");
            expectedMagicTapCount += 1;
            UnityEngine.Assertions.Assert.AreEqual(magicTapCount, expectedMagicTapCount);
            UnityEngine.Assertions.Assert.IsTrue(tapSucceeded);
#endif

            //Failing tap
            axObject.onAccessibilityPerformMagicTap = () =>
            {
                magicTapCount += 1;
                return false;
            };

            tapSucceeded = AccessibilityRuntime._UnityAX_AccessibilityPerformMagicTap(axObject.gameObject.GetInstanceID());
            expectedMagicTapCount += 1;
            UnityEngine.Assertions.Assert.AreEqual(magicTapCount, expectedMagicTapCount);
            UnityEngine.Assertions.Assert.IsFalse(tapSucceeded);

#if UNITY_IOS && !UNITY_EDITOR
            tapSucceeded = AccessibilityTests.RuniOSUnitTestWithName("_test_performMagicTapOnButton");
            expectedMagicTapCount += 1;
            UnityEngine.Assertions.Assert.AreEqual(magicTapCount, expectedMagicTapCount);
            UnityEngine.Assertions.Assert.IsFalse(tapSucceeded);
#endif

            //Unimplemented tap
            axObject.onAccessibilityPerformMagicTap = null;

            tapSucceeded = AccessibilityRuntime._UnityAX_AccessibilityPerformMagicTap(axObject.gameObject.GetInstanceID());
            UnityEngine.Assertions.Assert.AreEqual(magicTapCount, expectedMagicTapCount);
            UnityEngine.Assertions.Assert.IsFalse(tapSucceeded);
#if UNITY_IOS && !UNITY_EDITOR
            tapSucceeded = AccessibilityTests.RuniOSUnitTestWithName("_test_performMagicTapOnButton");
            UnityEngine.Assertions.Assert.AreEqual(magicTapCount, expectedMagicTapCount);
            UnityEngine.Assertions.Assert.IsFalse(tapSucceeded);
#endif

            yield return null;
        }

#endif

#if HAS_TEST_REPLACEMEMENT_FOR_ESCAPE
        [UnityTest]
        public IEnumerator Test_PerformEscape()
        {
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            int EscapeCount = 0;
            int expectedEscapeCount = 0;

            //Succeeding tap
            AccessibilityNode axObject = ButtonAXObject();
            axObject.onAccessibilityPerformEscape = () =>
            {
                EscapeCount += 1;
                return true;
            };

            bool tapSucceeded = AccessibilityRuntime._UnityAX_AccessibilityPerformEscape(axObject.gameObject.GetInstanceID());
            expectedEscapeCount += 1;
            UnityEngine.Assertions.Assert.AreEqual(EscapeCount, expectedEscapeCount);
            UnityEngine.Assertions.Assert.IsTrue(tapSucceeded);



#if UNITY_IOS && !UNITY_EDITOR
            tapSucceeded = AccessibilityTests.RuniOSUnitTestWithName("_test_performEscapeOnButton");
            expectedEscapeCount += 1;
            UnityEngine.Assertions.Assert.AreEqual(EscapeCount, expectedEscapeCount);
            UnityEngine.Assertions.Assert.IsTrue(tapSucceeded);
#endif

            //Failing tap
            axObject.onAccessibilityPerformEscape = () =>
            {
                EscapeCount += 1;
                return false;
            };

            tapSucceeded = AccessibilityRuntime._UnityAX_AccessibilityPerformEscape(axObject.gameObject.GetInstanceID());
            expectedEscapeCount += 1;
            UnityEngine.Assertions.Assert.AreEqual(EscapeCount, expectedEscapeCount);
            UnityEngine.Assertions.Assert.IsFalse(tapSucceeded);

#if UNITY_IOS && !UNITY_EDITOR
            tapSucceeded = AccessibilityTests.RuniOSUnitTestWithName("_test_performEscapeOnButton");
            expectedEscapeCount += 1;
            UnityEngine.Assertions.Assert.AreEqual(EscapeCount, expectedEscapeCount);
            UnityEngine.Assertions.Assert.IsFalse(tapSucceeded);
#endif

            //Unimplemented tap
            axObject.onAccessibilityPerformEscape = null;

            tapSucceeded = AccessibilityRuntime._UnityAX_AccessibilityPerformEscape(axObject.gameObject.GetInstanceID());
            UnityEngine.Assertions.Assert.AreEqual(EscapeCount, expectedEscapeCount);
            UnityEngine.Assertions.Assert.IsFalse(tapSucceeded);
#if UNITY_IOS && !UNITY_EDITOR
            tapSucceeded = AccessibilityTests.RuniOSUnitTestWithName("_test_performEscapeOnButton");
            UnityEngine.Assertions.Assert.AreEqual(EscapeCount, expectedEscapeCount);
            UnityEngine.Assertions.Assert.IsFalse(tapSucceeded);
#endif

            yield return null;
        }
#endif


        [UnityTest]
        public IEnumerator Test_AccessibilityObjectIsRegistered()
        {
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            GameObject button = GameObject.Find("Button1");
            UnityEngine.Assertions.Assert.IsNotNull(button);

            AccessibilityNode axObject = button.GetComponent<AccessibilityNode>();
            UnityEngine.Assertions.Assert.IsNotNull(axObject);

            bool succeeded = AccessibilityTests.RuniOSUnitTestWithName("_test_anyElementsExist");
            UnityEngine.Assertions.Assert.IsTrue(succeeded);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Test_UnityViewRegistered()
        {
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            bool succeeded = AccessibilityTests.RuniOSUnitTestWithName("_test_unityViewRegistered");
            UnityEngine.Assertions.Assert.IsTrue(succeeded);

            yield return null;
        }


        [UnityTest]
        public IEnumerator Test_iOSSideTestsSmokeTest()
        {
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

#if UNITY_IOS && !UNITY_EDITOR
            bool succeeded = AccessibilityTests.RuniOSUnitTestWithName("test_SmokeTestSucceeded");
            UnityEngine.Assertions.Assert.IsTrue(succeeded);

            succeeded = AccessibilityTests.RuniOSUnitTestWithName("test_SmokeTestFailed");
            UnityEngine.Assertions.Assert.IsFalse(succeeded);

            succeeded = AccessibilityTests.RuniOSUnitTestWithName("test_SmokeTestUnimplemented");
            UnityEngine.Assertions.Assert.IsFalse(succeeded);
#else
            bool succeeded = AccessibilityTests.RuniOSUnitTestWithName("test_SmokeTestSucceeded");
            UnityEngine.Assertions.Assert.IsTrue(succeeded);
#endif

            yield return null;
        }
    }
}
