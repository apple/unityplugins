using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit
{
    /// <summary>
    /// An achievement you can award a player as they make progress toward and reach a goal in your game.
    /// </summary>
    public class GKAchievement : NSObject
    {
        internal GKAchievement(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// The identifier for the achievement that you enter in App Store Connect.
        /// </summary>
        public string Identifier
        {
            get => Interop.GKAchievement_GetIdentifier(Pointer);
            set => Interop.GKAchievement_SetIdentifier(Pointer, value);
        }

        /// <summary>
        /// The player who earned the achievement.
        /// </summary>
        public GKPlayer Player => PointerCast<GKPlayer>(Interop.GKAchievement_GetPlayer(Pointer));
        
        /// <summary>
        /// A percentage value that states how far the player has progressed on the achievement.
        /// </summary>
        public float PercentComplete
        {
            get => Interop.GKAchievement_GetPercentComplete(Pointer);
            set => Interop.GKAchievement_SetPercentComplete(Pointer, value);
        }

        /// <summary>
        /// A Boolean value that states whether the player has completed the achievement.
        /// </summary>
        public bool IsCompleted => Interop.GKAchievement_GetIsCompleted(Pointer);

        /// <summary>
        /// The last time your game reported progress on the achievement for the player.
        /// </summary>
        public DateTimeOffset LastReportedDate => DateTimeOffset.FromUnixTimeSeconds((long)Interop.GKAchievement_GetLastReportedDate(Pointer));
        
        /// <summary>
        /// A Boolean value that indicates whether GameKit displays a banner when the player completes the achievement.
        /// </summary>
        public bool ShowCompletionBanner
        {
            get => Interop.GKAchievement_GetShowCompletionBanner(Pointer);
            set => Interop.GKAchievement_SetShowCompletionBanner(Pointer, value);
        }

        #region Report

        /// <summary>
        /// Reports the progress of players toward one or more achievements to Game Center.
        /// </summary>
        /// <param name="achievements">The achievements that you're reporting to Game Center.</param>
        /// <returns></returns>
        public static Task Report(params GKAchievement[] achievements)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);

            var mutable = new NSMutableArray<GKAchievement>();

            foreach (var achievement in achievements)
            {
                mutable.Add(achievement);
            }
            
            Interop.GKAchievement_Report(taskId, mutable.Pointer, OnReportSuccess, OnReportError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnReportSuccess(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnReportError(long taskId, IntPtr errorPtr)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPtr));
        }
        #endregion
        
        #region ResetAchievements

        /// <summary>
        /// Resets the percentage completed for all of the player's achievements.
        /// </summary>
        /// <returns></returns>
        public static Task ResetAchievements()
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKAchievement_ResetAchievements(taskId, OnResetAchievements, OnResetAchievementsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnResetAchievements(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnResetAchievementsError(long taskId, IntPtr errorPtr)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPtr));
        }
        #endregion
        
        #region LoadAchievements

        /// <summary>
        /// Loads the achievements that you previously reported the player making progress toward.
        /// </summary>
        /// <returns></returns>
        public static Task<NSArray<GKAchievement>> LoadAchievements()
        {
            var tcs = InteropTasks.Create<NSArray<GKAchievement>>(out var taskId);
            Interop.GKAchievement_LoadAchievements(taskId, OnLoadAchievements, OnLoadAchievementsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadAchievements(long taskId, IntPtr nsArrayPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKAchievement>>(nsArrayPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadAchievementsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKAchievement>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region SelectChallengeablePlayers

        /// <summary>
        /// Finds the subset of players who can earn an achievement.
        /// </summary>
        /// <param name="players">A list of players that GameKit uses to find players who are eligible to earn the achievement.</param>
        /// <returns>The players in the players parameter who are able to earn the achievement. If an error occurs, this parameter may be non-nil, containing achievement information GameKit is able to fetch before the error.</returns>
        public Task<NSArray<GKPlayer>> SelectChallengeablePlayers(GKPlayer[] players)
        {
            // Mutable players...
            var mutablePlayers = new NSMutableArray<GKPlayer>();
            if(players != null)
                foreach(var player in players)
                    mutablePlayers.Add(player);
            
            var tcs = InteropTasks.Create<NSArray<GKPlayer>>(out var taskId);
            Interop.GKAchievement_SelectChallengeablePlayers(Pointer, taskId, mutablePlayers.Pointer, OnSelectChallengeablePlayers, OnSelectChallengeablePlayersError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnSelectChallengeablePlayers(long taskId, IntPtr playersPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKPlayer>>(playersPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnSelectChallengeablePlayersError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKPlayer>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        /// <summary>
        /// Provides a view controller that you present to the player to issue an achievement challenge.
        /// </summary>
        /// <param name="message">The challenge message which the player can edit before GameKit sends it to other players.</param>
        /// <param name="players">The players that the challenge should be sent to.</param>
        public void ChallengeComposeController(string message, GKPlayer[] players)
        {
            // Mutable players...
            var mutablePlayers = new NSMutableArray<GKPlayer>();
            if(players != null)
                foreach(var player in players)
                    mutablePlayers.Add(player);
            
            Interop.GKAchievement_ChallengeComposeController(Pointer, message, mutablePlayers.Pointer);
        }
        
        /// <summary>
        /// Initializes an achievement for the local player.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>GKAchievement</returns>
        public static GKAchievement Init(string identifier)
        {
            return PointerCast<GKAchievement>(Interop.GKAchievement_Init(identifier));
        }

        /// <summary>
        /// Initialize the achievement for a specific player. Use to submit participant achievements when ending a turn-based match.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="player"></param>
        /// <returns>GKAchievement</returns>
        public static GKAchievement Init(string identifier, GKPlayer player)
        {
            return PointerCast<GKAchievement>(Interop.GKAchievement_InitForPlayer(identifier, player));
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKAchievement_GetIdentifier(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAchievement_SetIdentifier(IntPtr pointer, string value);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKAchievement_GetPlayer(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern float GKAchievement_GetPercentComplete(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAchievement_SetPercentComplete(IntPtr pointer, float value);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKAchievement_GetIsCompleted(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKAchievement_GetLastReportedDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKAchievement_GetShowCompletionBanner(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAchievement_SetShowCompletionBanner(IntPtr pointer, bool value);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAchievement_Report(long taskId, IntPtr array, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAchievement_ResetAchievements(long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAchievement_LoadAchievements(long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAchievement_SelectChallengeablePlayers(IntPtr pointer, long taskId, IntPtr players, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAchievement_ChallengeComposeController(IntPtr pointer, string message, IntPtr players);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKAchievement_Init(string identifier);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKAchievement_InitForPlayer(string identifier, GKPlayer player);
        }
    }
}
