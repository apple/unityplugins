using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;
using Apple.GameKit.Leaderboards;

namespace Apple.GameKit
{
    public class GKGameCenterViewController : InteropReference
    {
        #region Init & Dispose
        public GKGameCenterViewController(IntPtr pointer) : base(pointer)
        {
        }
        
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKGameCenterViewController_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                GKGameCenterViewController_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }
        #endregion
        
        #region Static Init
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKGameCenterViewController_InitWithState(GKGameCenterViewControllerState state);

        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKGameCenterViewController_InitWithLeaderboard(IntPtr leaderboard, GKLeaderboard.PlayerScope playerScope, GKLeaderboard.TimeScope timeScope);
        
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKGameCenterViewController_InitWithAchievement(IntPtr achievement);

        /// <summary>
        /// Returns a view controller that presents the specified Game Center content.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static GKGameCenterViewController Init(GKGameCenterViewControllerState state)
        {
            return PointerCast<GKGameCenterViewController>(GKGameCenterViewController_InitWithState(state));
        }

        /// <summary>
        /// Returns a view controller that presents a leaderboard with data from the specified players and time period.
        /// </summary>
        /// <param name="leaderboard"></param>
        /// <param name="playerScope"></param>
        /// <param name="timeScope"></param>
        /// <returns></returns>
        public static GKGameCenterViewController Init(GKLeaderboard leaderboard, GKLeaderboard.PlayerScope playerScope, GKLeaderboard.TimeScope timeScope)
        {
            return PointerCast<GKGameCenterViewController>(GKGameCenterViewController_InitWithLeaderboard(leaderboard.Pointer, playerScope, timeScope));
        }

        /// <summary>
        /// Returns a view controller that presents an achievement.
        /// </summary>
        /// <param name="achievement"></param>
        /// <returns></returns>
        public static GKGameCenterViewController Init(GKAchievement achievement)
        {
            return PointerCast<GKGameCenterViewController>(GKGameCenterViewController_InitWithAchievement(achievement.Pointer));
        }
        #endregion

        #region Present
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKGameCenterViewController_Present(IntPtr pointer, long taskId, SuccessTaskCallback onSuccess);

        /// <summary>
        /// Displays the view controller and awaits for the user to dismiss it.
        /// </summary>
        /// <returns>The task can be awaited to notify the client when the user has dismissed the view controller.</returns>
        public Task Present()
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            GKGameCenterViewController_Present(Pointer, taskId, OnPresent);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnPresent(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }
        #endregion
        
        /// <summary>
        /// The type of content for the view controller to present.
        /// </summary>
        public enum GKGameCenterViewControllerState : long
        {
            /// <summary>
            /// The view controller should present the default screen.
            /// </summary>
            Default = -1,
            /// <summary>
            /// The view controller should present leaderboard sets or leaderboards if there are no sets.
            /// </summary>
            Leaderboards = 0,
            /// <summary>
            /// The view controller should present a list of achievements.
            /// </summary>
            Achievements = 1,
            /// <summary>
            /// The view controller should present a list of challenges.
            /// </summary>
            Challenges = 2,
            /// <summary>
            /// The view controller should present the local player's profile.
            /// </summary>
            LocalPlayerProfile = 3,
            /// <summary>
            /// The view controller should present the dashboard.
            /// </summary>
            Dashboard = 4,
            /// <summary>
            /// The view controller should present the friends list.
            /// </summary>
            LocalPlayerFriendsList = 5
        }
    }
}