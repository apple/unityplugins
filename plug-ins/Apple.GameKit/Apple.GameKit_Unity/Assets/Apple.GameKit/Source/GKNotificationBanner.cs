using System;
using System.Runtime.InteropServices;

namespace Apple.GameKit
{
    /// <summary>
    /// A notification banner that displays text to the player.
    /// </summary>
    public static class GKNotificationBanner
    {
        #region Show
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKNotificationBanner_Show(string title, string message);
        
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKNotificationBanner_ShowWithDuration(string title, string message, double duration);

        /// <summary>
        /// Displays a banner to the player.
        /// </summary>
        /// <param name="title">The title of the banner.</param>
        /// <param name="message">A secondary message to be displayed.</param>
        public static void Show(string title, string message)
        {
            GKNotificationBanner_Show(title, message);
        }

        /// <summary>
        /// Displays a banner to the player for a specified period of time.
        /// </summary>
        /// <param name="title">The title of the banner.</param>
        /// <param name="message">A secondary message to be displayed.</param>
        /// <param name="duration">The amount of time that the banner should be displayed to the player.</param>
        public static void Show(string title, string message, TimeSpan duration)
        {
            GKNotificationBanner_ShowWithDuration(title, message, duration.TotalSeconds);
        }
        #endregion
    }
}