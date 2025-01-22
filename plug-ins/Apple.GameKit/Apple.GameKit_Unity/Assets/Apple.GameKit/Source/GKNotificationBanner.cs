using System;
using System.Runtime.InteropServices;
using Apple.Core;

namespace Apple.GameKit
{
#if !UNITY_VISIONOS
    /// <summary>
    /// A notification banner that displays text to the player.
    /// </summary>
    [Deprecated("Use UNNotificationRequest or provide custom UI instead. This method will become a no-op in a future version of GameKit.", iOS: "17.0", macOS: "14.0", tvOS: "16.1", visionOS: "1.0")]
    public static class GKNotificationBanner
    {
        /// <summary>
        /// Displays a banner to the player.
        /// </summary>
        /// <param name="title">The title of the banner.</param>
        /// <param name="message">A secondary message to be displayed.</param>
        public static void Show(string title, string message) => Interop.GKNotificationBanner_Show(title, message);

        /// <summary>
        /// Displays a banner to the player for a specified period of time.
        /// </summary>
        /// <param name="title">The title of the banner.</param>
        /// <param name="message">A secondary message to be displayed.</param>
        /// <param name="duration">The amount of time that the banner should be displayed to the player.</param>
        public static void Show(string title, string message, TimeSpan duration) => Interop.GKNotificationBanner_ShowWithDuration(title, message, duration.TotalSeconds);

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKNotificationBanner_Show(string title, string message);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKNotificationBanner_ShowWithDuration(string title, string message, double duration);
        }
    }
#endif
}