using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;
using Apple.GameKit.Leaderboards;
using UnityEngine;
using UnityEngine.Scripting;

namespace Apple.GameKit
{
    /// <summary>
    /// An object that allows players to view and manage their Game Center information from within your game.
    /// </summary>
    [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
    public class GKAccessPoint : NSObject
    {
        [Preserve]
        public GKAccessPoint(IntPtr pointer) : base(pointer)
        {
        }
        
        private static GKAccessPoint _shared;

        /// <summary>
        /// The shared access point object.
        /// </summary>
        public static GKAccessPoint Shared => _shared ??= PointerCast<GKAccessPoint>(Interop.GKAccessPoint_GetShared());

        /// <summary>
        /// The corner of the screen to display the access point.
        /// </summary>
        public GKAccessPointLocation Location
        {
            get => Interop.GKAccessPoint_GetLocation(Pointer);
            set => Interop.GKAccessPoint_SetLocation(Pointer, value);
        }
        
        /// <summary>
        /// observable property that contains the current frame needed to display the widget
        /// </summary>
        /// <symbol>c:objc(cs)GKAccessPoint(py)frameInScreenCoordinates</symbol>
        public Rect FrameInScreenCoordinates => Interop.GKAccessPoint_GetFrameInScreenCoordinates(Pointer).ToRect();
        
        /// <summary>
        /// The normalized frame of the access point in unit (0 -> 1) coordinates.
        /// </summary>
        #if !UNITY_VISIONOS
        public Rect FrameInUnitCoordinates => Interop.GKAccessPoint_GetFrameInUnitCoordinates(Pointer).ToRect();
        #endif
        
        /// <summary>
        /// A Boolean value that determines whether to display the access point.
        /// </summary>
        public bool IsActive
        {
            get => Interop.GKAccessPoint_GetIsActive(Pointer);
            set => Interop.GKAccessPoint_SetIsActive(Pointer, value);
        }

        /// <summary>
        /// A Boolean value that indicates whether the game is presenting the Game Center dashboard.
        /// </summary>
        public bool IsPresentingGameCenter => Interop.GKAccessPoint_GetIsPresentingGameCenter(Pointer);
        
        /// <summary>
        /// A Boolean value that indicates whether the access point is visible.
        /// </summary>
        public bool IsVisible => Interop.GKAccessPoint_GetIsVisible(Pointer);
        
        /// <summary>
        /// A Boolean value that indicates whether to display highlights for achievements and current ranks for leaderboards.
        /// </summary>
        /// <symbol>c:objc(cs)GKAccessPoint(py)showHighlights</symbol>
        [Deprecated("Deprecated", iOS: "26.0.0", macOS: "26.0.0", tvOS: "26.0.0", visionOS: "26.0.0")]
        public bool ShowHighlights
        {
            get => Interop.GKAccessPoint_GetShowHighlights(Pointer);
            set => Interop.GKAccessPoint_SetShowHighlights(Pointer, value);
        }

#if UNITY_TVOS
        /// <summary>
        /// A Boolean value that indicates whether the access point is in focus on tvOS.
        /// </summary>
        public bool IsFocused => Interop.GKAccessPoint_GetIsFocused(Pointer);
#endif

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnTriggerSuccess(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnTriggerError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }

        /// <summary>
        /// Displays the Game Center dashboard.
        /// </summary>
        public Task Trigger()
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKAccessPoint_Trigger(Pointer, taskId, OnTriggerSuccess, OnTriggerError);
            return tcs.Task;
        }

        /// <summary>
        /// Displays the Game Center dashboard in the specified state.
        /// </summary>
        public Task Trigger(GKGameCenterViewControllerState state)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKAccessPoint_TriggerWithState(Pointer, (long)state, taskId, OnTriggerSuccess, OnTriggerError);
            return tcs.Task;
        } 

        /// <summary>
        /// Displays the Game Center dashboard for the specified achievement.
        /// </summary>
        /// <param name="achievementID"></param>
        [Introduced(iOS: "18.0", macOS: "15.0", tvOS: "18.0", visionOS: "2.0")]
        public Task TriggerWithAchievementID(NSString achievementID)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKAccessPoint_TriggerWithAchievementID(Pointer, achievementID.Pointer, taskId, OnTriggerSuccess, OnTriggerError);
            return tcs.Task;
        }

        /// <summary>
        /// Displays the Game Center dashboard for the specified leaderboard set.
        /// </summary>
        /// <param name="leaderboardSetID"></param>
        [Introduced(iOS: "18.0", macOS: "15.0", tvOS: "18.0", visionOS: "2.0")]
        public Task TriggerWithLeaderboardSetID(NSString leaderboardSetID)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKAccessPoint_TriggerWithLeaderboardSetID(Pointer, leaderboardSetID.Pointer, taskId, OnTriggerSuccess, OnTriggerError);
            return tcs.Task;
        }

        /// <summary>
        /// Displays the Game Center dashboard for the specified leaderboard with the specified player and time scope.
        /// </summary>
        /// <param name="leaderboardID"></param>
        /// <param name="playerScope"></param>
        /// <param name="timeScope"></param>
        [Introduced(iOS: "18.0", macOS: "15.0", tvOS: "18.0", visionOS: "2.0")]
        public Task TriggerWithLeaderboardID(NSString leaderboardID, GKLeaderboard.PlayerScope playerScope, GKLeaderboard.TimeScope timeScope)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKAccessPoint_TriggerWithLeaderboardID(Pointer, leaderboardID.Pointer, playerScope, timeScope, taskId, OnTriggerSuccess, OnTriggerError);
            return tcs.Task;
        }

        /// <summary>
        /// Displays the Game Center dashboard for the specified player.
        /// </summary>
        /// <param name="player"></param>
        [Introduced(iOS: "18.0", macOS: "15.0", tvOS: "18.0", visionOS: "2.0")]
        public Task TriggerWithPlayer(GKPlayer player)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKAccessPoint_TriggerWithPlayer(Pointer, player.Pointer, taskId, OnTriggerSuccess, OnTriggerError);
            return tcs.Task;
        }

#if UNITY_IOS || UNITY_STANDALONE_OSX
        /// <summary>
        /// Brings up the view that allows players to engage each other via activities and challenges.
        /// </summary>
        /// <symbol>c:objc(cs)GKAccessPoint(im)triggerAccessPointForPlayTogetherWithHandler:</symbol>
        [Unavailable(RuntimeOperatingSystem.tvOS, RuntimeOperatingSystem.visionOS)]
        [Introduced(iOS: "26.0.0", macOS: "26.0.0")]
        public Task TriggerForPlayTogether()
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKAccessPoint_TriggerForPlayTogether(Pointer, taskId, OnTriggerSuccess, OnTriggerError);
            return tcs.Task;
        }
#endif

#if UNITY_IOS || UNITY_STANDALONE_OSX
        /// <summary>
        /// Brings up the view that allows players to engage each other via challenges.
        /// </summary>
        /// <symbol>c:objc(cs)GKAccessPoint(im)triggerAccessPointForChallengesWithHandler:</symbol>
        [Unavailable(RuntimeOperatingSystem.tvOS, RuntimeOperatingSystem.visionOS)]
        [Introduced(iOS: "26.0.0", macOS: "26.0.0")]
        public Task TriggerForChallenges()
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKAccessPoint_TriggerForChallenges(Pointer, taskId, OnTriggerSuccess, OnTriggerError);
            return tcs.Task;
        }
#endif

#if UNITY_IOS || UNITY_STANDALONE_OSX
        /// <summary>
        /// Brings up the challenge creation view for the provided definition.
        /// </summary>
        /// <symbol>c:objc(cs)GKAccessPoint(im)triggerAccessPointWithChallengeDefinitionID:handler:</symbol>
        [Unavailable(RuntimeOperatingSystem.tvOS, RuntimeOperatingSystem.visionOS)]
        [Introduced(iOS: "26.0.0", macOS: "26.0.0")]
        public Task TriggerWithChallengeDefinitionID(NSString challengeDefinitionID)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKAccessPoint_TriggerWithChallengeDefinitionID(Pointer, challengeDefinitionID.Pointer, taskId, OnTriggerSuccess, OnTriggerError);
            return tcs.Task;
        }
#endif

#if UNITY_IOS || UNITY_STANDALONE_OSX
        /// <summary>
        /// Brings up the game activity play together flow for the provided definition.
        /// </summary>
        /// <symbol>c:objc(cs)GKAccessPoint(im)triggerAccessPointWithGameActivityDefinitionID:handler:</symbol>
        [Unavailable(RuntimeOperatingSystem.tvOS, RuntimeOperatingSystem.visionOS)]
        [Introduced(iOS: "26.0.0", macOS: "26.0.0")]
        public Task TriggerWithGameActivityDefinitionID(NSString gameActivityDefinitionID)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKAccessPoint_TriggerWithGameActivityDefinitionID(Pointer, gameActivityDefinitionID.Pointer, taskId, OnTriggerSuccess, OnTriggerError);
            return tcs.Task;
        }
#endif

#if UNITY_IOS || UNITY_STANDALONE_OSX
        /// <summary>
        /// Brings up the invite friends view.
        /// </summary>
        /// <symbol>c:objc(cs)GKAccessPoint(im)triggerAccessPointForFriendingWithHandler:</symbol>
        [Unavailable(RuntimeOperatingSystem.tvOS, RuntimeOperatingSystem.visionOS)]
        [Introduced(iOS: "26.0.0", macOS: "26.0.0")]
        public Task TriggerForFriending()
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKAccessPoint_TriggerForFriending(Pointer, taskId, OnTriggerSuccess, OnTriggerError);
            return tcs.Task;
        }
#endif

        /// <summary>
        /// Specifies the corner of the screen to display the access point.
        /// </summary>
        public enum GKAccessPointLocation : long
        {
            /// <summary>
            /// The upper-left corner of the screen.
            /// </summary>
            TopLeading = 0,
            /// <summary>
            /// The upper-right corner of the screen.
            /// </summary>
            TopTrailing = 1,
            /// <summary>
            /// The lower-left corner of the screen.
            /// </summary>
            BottomLeading = 2,
            /// <summary>
            /// The lower-right corner of the screen.
            /// </summary>
            BottomTrailing = 3
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKAccessPoint_GetShared();
            [DllImport(InteropUtility.DLLName)]
            public static extern GKAccessPointLocation GKAccessPoint_GetLocation(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_SetLocation(IntPtr pointer, GKAccessPointLocation location);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKAccessPointFrameInScreenCoordinates GKAccessPoint_GetFrameInScreenCoordinates(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKAccessPointFrameInScreenCoordinates GKAccessPoint_GetFrameInUnitCoordinates(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKAccessPoint_GetIsActive(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_SetIsActive(IntPtr pointer, bool isActive);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKAccessPoint_GetIsPresentingGameCenter(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKAccessPoint_GetIsVisible(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKAccessPoint_GetShowHighlights(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_SetShowHighlights(IntPtr pointer, bool isActive);

#if UNITY_TVOS
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKAccessPoint_GetIsFocused(IntPtr pointer);
#endif

            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_Trigger(IntPtr pointer, long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_TriggerWithState(IntPtr pointer, long state, long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_TriggerWithAchievementID(IntPtr pointer, IntPtr achievementIDPtr, long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_TriggerWithLeaderboardSetID(IntPtr pointer, IntPtr leaderboardSetIDPtr, long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_TriggerWithLeaderboardID(IntPtr pointer, IntPtr leaderboardIDPtr, GKLeaderboard.PlayerScope playerScope, GKLeaderboard.TimeScope timeScope, long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_TriggerWithPlayer(IntPtr pointer, IntPtr gkPlayerPtr, long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);

#if UNITY_IOS || UNITY_STANDALONE_OSX
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_TriggerForPlayTogether(IntPtr pointer, long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_TriggerForChallenges(IntPtr pointer, long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_TriggerWithChallengeDefinitionID(IntPtr pointer, IntPtr challengeDefinitionIDPtr, long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_TriggerWithGameActivityDefinitionID(IntPtr pointer, IntPtr gameActivityDefinitionIDPtr, long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_TriggerForFriending(IntPtr pointer, long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
#endif
        }

    }
}
