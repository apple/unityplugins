using System.Runtime.InteropServices;

namespace Apple.Accessibility
{
    public static class AccessibilityNotification
    {
        /// <summary>
        /// A notification that an app posts when a new view appears that occupies a major portion of the screen.
        /// </summary>
        public static void PostScreenChangedNotification()
        {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
            _UnityAX_PostScreenChangedNotification();
#endif
        }

        /// <summary>
        /// A notification that an app posts when the layout of a screen changes.
        /// </summary>
        public static void PostLayoutChangedNotification()
        {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
            _UnityAX_PostLayoutChangedNotification();
#endif
        }

        /// <summary>
        /// A notification that an app posts when it needs to convey an announcement to the assistive app.
        /// Use this notification to provide accessibility information about events that don't update the app's UI, or that update the UI only briefly.
        /// </summary>
        /// <param name="announcement">An assistive app outputs the announcement string in the parameter.</param>
        public static void PostAnnouncementNotification(string announcement)
        {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
            _UnityAX_PostAnnouncementNotification(announcement);
#endif
        }

        /// <summary>
        /// A notification that an app posts when a scroll action completes.
        /// Use this notification to provide custom information about the contents of the screen after a user performs a VoiceOver scroll gesture. For example, a tab-based app might provide a string like Tab 3 of 5, or an app that displays information in pages might provide a string like Page 19 of 27.
        /// When an assistive app repeatedly receives the same scroll position string, it indicates to users that scrolling can't continue due to a border or boundary.
        /// </summary>
        /// <param name="scrollPosition">A description of the new scroll position. An assistive app outputs the description string in the parameter.</param>
        public static void PostPageScrolledNotification(string scrollPosition)
        {
#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
            _UnityAX_PostPageScrolledNotification(scrollPosition);
#endif
        }

        #region Private

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void _UnityAX_PostScreenChangedNotification();
        [DllImport("__Internal")] private static extern void _UnityAX_PostLayoutChangedNotification();
        [DllImport("__Internal")] private static extern void _UnityAX_PostAnnouncementNotification(string announcement);
        [DllImport("__Internal")] private static extern void _UnityAX_PostPageScrolledNotification(string pageScrolled);
#endif

        #endregion
    }
}
