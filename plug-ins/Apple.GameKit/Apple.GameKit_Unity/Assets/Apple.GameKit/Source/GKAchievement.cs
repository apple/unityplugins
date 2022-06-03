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
    public class GKAchievement : InteropReference
    {
        #region Init & Dispose
        internal GKAchievement(IntPtr pointer) : base(pointer)
        {
        }
        
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAchievement_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                GKAchievement_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }
        #endregion
        
        #region Identifier
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKAchievement_GetIdentifier(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAchievement_SetIdentifier(IntPtr pointer, string value);

        /// <summary>
        /// The identifier for the achievement that you enter in App Store Connect.
        /// </summary>
        public string Identifier
        {
            get => GKAchievement_GetIdentifier(Pointer);
            set => GKAchievement_SetIdentifier(Pointer, value);
        }
        #endregion
        
        #region Player
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKAchievement_GetPlayer(IntPtr pointer);

        /// <summary>
        /// The player who earned the achievement.
        /// </summary>
        public GKPlayer Player => PointerCast<GKPlayer>(GKAchievement_GetPlayer(Pointer));

        #endregion
        
        #region PercentComplete
        [DllImport(InteropUtility.DLLName)]
        private static extern float GKAchievement_GetPercentComplete(IntPtr pointer);

        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAchievement_SetPercentComplete(IntPtr pointer, float value);

        /// <summary>
        /// A percentage value that states how far the player has progressed on the achievement.
        /// </summary>
        public float PercentComplete
        {
            get => GKAchievement_GetPercentComplete(Pointer);
            set => GKAchievement_SetPercentComplete(Pointer, value);
        }
        #endregion
        
        #region IsCompleted
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKAchievement_GetIsCompleted(IntPtr pointer);

        /// <summary>
        /// A Boolean value that states whether the player has completed the achievement.
        /// </summary>
        public bool IsCompleted
        {
            get => GKAchievement_GetIsCompleted(Pointer);
        }
        #endregion
        
        #region LastReportedDate
        [DllImport(InteropUtility.DLLName)]
        private static extern double GKAchievement_GetLastReportedDate(IntPtr pointer);

        /// <summary>
        /// The last time your game reported progress on the achievement for the player.
        /// </summary>
        public DateTimeOffset LastReportedDate
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds((long)GKAchievement_GetLastReportedDate(Pointer));
            }
        }
        #endregion
        
        #region ShowCompletionBanner
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKAchievement_GetShowCompletionBanner(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAchievement_SetShowCompletionBanner(IntPtr pointer, bool value);

        /// <summary>
        /// A Boolean value that indicates whether GameKit displays a banner when the player completes the achievement.
        /// </summary>
        public bool ShowCompletionBanner
        {
            get => GKAchievement_GetShowCompletionBanner(Pointer);
            set => GKAchievement_SetShowCompletionBanner(Pointer, value);
        }

        #endregion
        
        #region Report
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAchievement_Report(long taskId, IntPtr array, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Reports the progress of players toward one or more achievements to Game Center.
        /// </summary>
        /// <param name="achievements">The achievements that you're reporting to Game Center.</param>
        /// <returns></returns>
        public static Task Report(params GKAchievement[] achievements)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);

            var mutable = NSMutableArrayFactory.Init<NSMutableArrayGKAchievement, GKAchievement>();

            foreach (var achievement in achievements)
            {
                mutable.Add(achievement);
            }
            
            GKAchievement_Report(taskId, mutable.Pointer, OnReportSuccess, OnReportError);
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
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAchievement_ResetAchievements(long taskId, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Resets the percentage completed for all of the player's achievements.
        /// </summary>
        /// <returns></returns>
        public static Task ResetAchievements()
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            GKAchievement_ResetAchievements(taskId, OnResetAchievements, OnResetAchievementsError);
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
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAchievement_LoadAchievements(long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Loads the achievements that you previously reported the player making progress toward.
        /// </summary>
        /// <returns></returns>
        public static Task<NSArray<GKAchievement>> LoadAchievements()
        {
            var tcs = InteropTasks.Create<NSArray<GKAchievement>>(out var taskId);
            GKAchievement_LoadAchievements(taskId, OnLoadAchievements, OnLoadAchievementsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadAchievements(long taskId, IntPtr nsArrayPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, (NSArray<GKAchievement>) PointerCast<NSArrayGKAchievement>(nsArrayPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadAchievementsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKAchievement>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region SelectChallengeablePlayers
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAchievement_SelectChallengeablePlayers(IntPtr pointer, long taskId, IntPtr players, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Finds the subset of players who can earn an achievement.
        /// </summary>
        /// <param name="players">A list of players that GameKit uses to find players who are eligible to earn the achievement.</param>
        /// <returns>The players in the players parameter who are able to earn the achievement. If an error occurs, this parameter may be non-nil, containing achievement information GameKit is able to fetch before the error.</returns>
        public Task<NSArray<GKPlayer>> SelectChallengeablePlayers(GKPlayer[] players)
        {
            // Mutable players...
            var mutablePlayers = NSMutableArrayFactory.Init<NSMutableArrayGKPlayer, GKPlayer>();
            if(players != null)
                foreach(var player in players)
                    mutablePlayers.Add(player);
            
            var tcs = InteropTasks.Create<NSArray<GKPlayer>>(out var taskId);
            GKAchievement_SelectChallengeablePlayers(Pointer, taskId, mutablePlayers.Pointer, OnSelectChallengeablePlayers, OnSelectChallengeablePlayersError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnSelectChallengeablePlayers(long taskId, IntPtr playersPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, (NSArray<GKPlayer>) PointerCast<NSArrayGKPlayer>(playersPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnSelectChallengeablePlayersError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKPlayer>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region ChallengeComposeController
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAchievement_ChallengeComposeController(IntPtr pointer, string message, IntPtr players);

        /// <summary>
        /// Provides a view controller that you present to the player to issue an achievement challenge.
        /// </summary>
        /// <param name="message">The challenge message which the player can edit before GameKit sends it to other players.</param>
        /// <param name="players">The players that the challenge should be sent to.</param>
        public void ChallengeComposeController(string message, GKPlayer[] players)
        {
            // Mutable players...
            var mutablePlayers = NSMutableArrayFactory.Init<NSMutableArrayGKPlayer, GKPlayer>();
            if(players != null)
                foreach(var player in players)
                    mutablePlayers.Add(player);
            
            GKAchievement_ChallengeComposeController(Pointer, message, mutablePlayers.Pointer);
        }
        #endregion
        
        #region Static Init
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKAchievement_Init(string identifier);

        /// <summary>
        /// Initializes an achievement for the local player.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static GKAchievement Init(string identifier)
        {
            return PointerCast<GKAchievement>(GKAchievement_Init(identifier));
        }
        #endregion
    }
}