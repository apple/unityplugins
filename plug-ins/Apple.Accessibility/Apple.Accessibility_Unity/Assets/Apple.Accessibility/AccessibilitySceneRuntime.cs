using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Apple.Accessibility
{
    internal static class AccessibilitySceneRuntime
    {
        #region Native Bridge

        private static bool __registeredBeforeSplashScreen = false;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void BeforeSplashScreen()
        {
            if (!__registeredBeforeSplashScreen)
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                SceneManager.activeSceneChanged += SceneChanged;
#endif
                __registeredBeforeSplashScreen = true;
            }
        }

        private static bool __registeredAfterSceneLoad = false;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            if (!__registeredAfterSceneLoad)
            {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
                _UnityAX_PostUnityViewChanged();
#endif
                __registeredAfterSceneLoad = true;
            }
        }

        #endregion

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void _UnityAX_PostUnityViewChanged();
#endif

        static void SceneChanged(Scene pre, Scene post)
        {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
            _UnityAX_PostUnityViewChanged();
#endif
        }

    }
}
