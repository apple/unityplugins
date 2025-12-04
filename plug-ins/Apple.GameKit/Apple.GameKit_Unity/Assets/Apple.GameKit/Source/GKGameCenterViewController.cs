using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;
using Apple.GameKit.Leaderboards;
using UnityEngine.Scripting;

namespace Apple.GameKit
{
    [Deprecated("Deprecated", iOS: "26.0.0", macOS: "26.0.0", tvOS: "26.0.0", visionOS: "26.0.0")]
    public class GKGameCenterViewController : NSObject
    {
        [Preserve]
        public GKGameCenterViewController(IntPtr pointer) : base(pointer)
        {
        }
        
        /// <summary>
        /// Returns a view controller that presents the specified Game Center content.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public static GKGameCenterViewController InitWithState(GKGameCenterViewControllerState state)
        {
            return PointerCast<GKGameCenterViewController>(Interop.GKGameCenterViewController_InitWithState(state));
        }
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public static GKGameCenterViewController Init(GKGameCenterViewControllerState state)
        {
            return InitWithState(state);
        }

        /// <summary>
        /// Creates a view controller that presents a leaderboard with data for the specified players.
        /// </summary>
        /// <param name="leaderboard"></param>
        /// <param name="playerScope"></param>
        /// <returns></returns>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public static GKGameCenterViewController InitWithLeaderboard(GKLeaderboard leaderboard, GKLeaderboard.PlayerScope playerScope)
        {
            return PointerCast<GKGameCenterViewController>(Interop.GKGameCenterViewController_InitWithLeaderboard(leaderboard.Pointer, playerScope));
        }
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public static GKGameCenterViewController Init(GKLeaderboard leaderboard, GKLeaderboard.PlayerScope playerScope)
        {
            return InitWithLeaderboard(leaderboard, playerScope);
        }

        /// <summary>
        /// Returns a view controller that presents a leaderboard with data from the specified players and time period.
        /// </summary>
        /// <param name="leaderboard"></param>
        /// <param name="playerScope"></param>
        /// <param name="timeScope"></param>
        /// <returns></returns>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public static GKGameCenterViewController InitWithLeaderboard(GKLeaderboard leaderboard, GKLeaderboard.PlayerScope playerScope, GKLeaderboard.TimeScope timeScope)
        {
            return PointerCast<GKGameCenterViewController>(Interop.GKGameCenterViewController_InitWithLeaderboardID(new NSString(leaderboard.BaseLeaderboardId).Pointer, playerScope, timeScope));
        }
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public static GKGameCenterViewController Init(GKLeaderboard leaderboard, GKLeaderboard.PlayerScope playerScope, GKLeaderboard.TimeScope timeScope)
        {
            return InitWithLeaderboard(leaderboard, playerScope, timeScope);
        }

        /// <summary>
        /// Returns a view controller that presents a leaderboard with data from the specified players and time period.
        /// </summary>
        /// <param name="leaderboardID"></param>
        /// <param name="playerScope"></param>
        /// <param name="timeScope"></param>
        /// <returns></returns>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public static GKGameCenterViewController InitWithLeaderboardID(NSString leaderboardID, GKLeaderboard.PlayerScope playerScope, GKLeaderboard.TimeScope timeScope)
        {
            return PointerCast<GKGameCenterViewController>(Interop.GKGameCenterViewController_InitWithLeaderboardID(leaderboardID.Pointer, playerScope, timeScope));
        }

        /// <summary>
        /// Returns a view controller that presents an achievement.
        /// </summary>
        /// <param name="achievement"></param>
        /// <returns></returns>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public static GKGameCenterViewController InitWithAchievement(GKAchievement achievement)
        {
            return PointerCast<GKGameCenterViewController>(Interop.GKGameCenterViewController_InitWithAchievementID(new NSString(achievement.Identifier).Pointer));
        }
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public static GKGameCenterViewController Init(GKAchievement achievement)
        {
            return InitWithAchievement(achievement);
        }

        /// <summary>
        /// Returns a view controller that presents an achievement.
        /// </summary>
        /// <param name="achievementID"></param>
        /// <returns></returns>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public static GKGameCenterViewController InitWithAchievementID(NSString achievementID)
        {
            return PointerCast<GKGameCenterViewController>(Interop.GKGameCenterViewController_InitWithAchievementID(achievementID.Pointer));
        }

        /// <summary>
        /// Returns a view controller that presents a leaderboard set.
        /// </summary>
        /// <param name="leaderboardSetID">The identifier for the leaderboard set.</param>
        /// <returns></returns>
        [Introduced(iOS: "18.0", macOS: "15.0", tvOS: "18.0", visionOS: "2.0")]
        public static GKGameCenterViewController InitWithLeaderboardSetID(NSString leaderboardSetID)
        {
            return PointerCast<GKGameCenterViewController>(Interop.GKGameCenterViewController_InitWithLeaderboardSetID(leaderboardSetID.Pointer));
        }

        /// <summary>
        /// Returns a view controller that presents a player’s Game Center profile.
        /// </summary>
        /// <param name="player">Returns a view controller that presents a player’s Game Center profile.</param>
        /// <returns></returns>
        [Introduced(iOS: "18.0", macOS: "15.0", tvOS: "18.0", visionOS: "2.0")]
        public static GKGameCenterViewController InitWithPlayer(GKPlayer player)
        {
            return PointerCast<GKGameCenterViewController>(Interop.GKGameCenterViewController_InitWithPlayer(player.Pointer));
        }
        [Introduced(iOS: "18.0", macOS: "15.0", tvOS: "18.0", visionOS: "2.0")]
        public static GKGameCenterViewController Init(GKPlayer player)
        {
            return InitWithPlayer(player);
        }

        #region Present

        /// <summary>
        /// Displays the view controller and awaits for the user to dismiss it.
        /// </summary>
        /// <returns>The task can be awaited to notify the client when the user has dismissed the view controller.</returns>
        public Task Present()
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKGameCenterViewController_Present(Pointer, taskId, OnPresent);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnPresent(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }
        #endregion
        
        // BREAKING CHANGE: GKGameCenterViewControllerState has been moved out of class scope.

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameCenterViewController_InitWithState(GKGameCenterViewControllerState state);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameCenterViewController_InitWithLeaderboard(IntPtr leaderboard, GKLeaderboard.PlayerScope playerScope);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameCenterViewController_InitWithLeaderboardID(IntPtr leaderboardID, GKLeaderboard.PlayerScope playerScope, GKLeaderboard.TimeScope timeScope);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameCenterViewController_InitWithAchievementID(IntPtr achievementID);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameCenterViewController_InitWithLeaderboardSetID(IntPtr leaderboardSetID); // NSString
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameCenterViewController_InitWithPlayer(IntPtr gkPlayer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameCenterViewController_Present(IntPtr pointer, long taskId, SuccessTaskCallback onSuccess);
        }
    }
}
