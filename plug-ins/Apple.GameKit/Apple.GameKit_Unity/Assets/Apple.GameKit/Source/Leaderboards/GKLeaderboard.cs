using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;
using UnityEngine;

namespace Apple.GameKit.Leaderboards
{
    public class GKLeaderboard : NSObject
    {
        /// <summary>
        /// Specifies whether a leaderboard is recurring.
        /// </summary>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public enum LeaderboardType : long
        {
            Classic = 0,
            Recurring = 1
        }
        
        /// <summary>
        /// Specifies the type of players for filtering data.
        /// </summary>
        public enum PlayerScope : long
        {
            /// <summary>
            /// Loads data for all players of the game.
            /// </summary>
            Global = 0,
            /// <summary>
            /// Loads only data for friends of the local player.
            /// </summary>
            FriendsOnly = 1
        }
        
        /// <summary>
        /// Specifies the time period for filtering data.
        /// </summary>
        public enum TimeScope : long
        {
            /// <summary>
            /// Loads data for the past 24 hours.
            /// </summary>
            Today = 0,
            /// <summary>
            /// Loads data for the past week.
            /// </summary>
            Week = 1,
            /// <summary>
            /// Loads a player's best score
            /// </summary>
            AllTime = 2
        }

        /// <summary>
        /// Information about a single score by a player on a leaderboard.
        /// </summary>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public class Entry : NSObject
        {
            internal Entry(IntPtr pointer) : base(pointer) {}

            // This allows interop to work with the Objective-C type which has a different name: GKLeaderboardEntry.
            internal static string InteropTypeName => "GKLeaderboardEntry";

            /// <summary>
            /// An integer value that your game uses.
            /// </summary>
            public long Context => Interop.GKLeaderboardEntry_GetContext(Pointer);

            /// <summary>
            /// The date and time when the player earns the score.
            /// </summary>
            public DateTimeOffset Date => DateTimeOffsetExtensions.FromUnixTimeSeconds(Interop.GKLeaderboardEntry_GetDate(Pointer));
            
            /// <summary>
            /// The player's score as a localized string.
            /// </summary>
            public string FormattedScore => Interop.GKLeaderboardEntry_GetFormattedScore(Pointer);

            /// <summary>
            /// The player who earns the score.
            /// </summary>
            public GKPlayer Player => PointerCast<GKPlayer>(Interop.GKLeaderboardEntry_GetPlayer(Pointer));

            /// <summary>
            /// The position of the score in the results of a leaderboard search.
            /// </summary>
            public long Rank => Interop.GKLeaderboardEntry_GetRank(Pointer);
            
            /// <summary>
            /// The score that the player earns.
            /// </summary>
            public long Score => Interop.GKLeaderboardEntry_GetScore(Pointer);
        }

        internal GKLeaderboard(IntPtr pointer) : base(pointer) {}
        
        /// <summary>
        /// The ID that Game Center uses to identify this leaderboard.
        /// </summary>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public string BaseLeaderboardId => Interop.GKLeaderboard_GetBaseLeaderboardId(Pointer);
        
        /// <summary>
        /// The localized title for the leaderboard.
        /// </summary>
        public string Title => Interop.GKLeaderboard_GetTitle(Pointer);
        
        /// <summary>
        /// The type of leaderboard, classic or recurring.
        /// </summary>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public LeaderboardType Type => Interop.GKLeaderboard_GetType(Pointer);
        
        /// <summary>
        /// The identifier for the group the leaderboard belongs to.
        /// </summary>
        public string GroupIdentifier => Interop.GKLeaderboard_GetGroupIdentifier(Pointer);
        
        /// <summary>
        /// The date and time a recurring leaderboard occurrence starts accepting scores.
        /// </summary>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public DateTimeOffset StartDate => DateTimeOffsetExtensions.FromUnixTimeSeconds(Interop.GKLeaderboard_GetStartDate(Pointer));
        
        /// <summary>
        /// The date and time the next recurring leaderboard occurrence starts accepting scores.
        /// </summary>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public DateTimeOffset NextStartDate => DateTimeOffsetExtensions.FromUnixTimeSeconds(Interop.GKLeaderboard_GetNextStartDate(Pointer));
        
        /// <summary>
        /// The duration from the start date that a recurring leaderboard occurrence accepts scores.
        /// </summary>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public TimeSpan Duration => TimeSpan.FromSeconds(Interop.GKLeaderboard_GetDuration(Pointer));

        #region Load Leaderboards

        /// <summary>
        /// Loads leaderboards for the specified leaderboard IDs that Game Center uses.
        /// </summary>
        /// <param name="identifiers">The leaderboards that match the IDs or null for all leaderboards.</param>
        /// <returns></returns>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public static Task<NSArray<GKLeaderboard>> LoadLeaderboards(params string[] identifiers)
        {
            var tcs = InteropTasks.Create<NSArray<GKLeaderboard>>(out var taskId);
            
            // Prepare identifiers array...
            NSMutableArray<NSString> ids = null;
            if (identifiers != null && identifiers.Length > 0)
            {
                ids = new NSMutableArray<NSString>(identifiers.Select(id => new NSString(id)));
            }
            
            Interop.GKLeaderboard_LoadLeaderboards(ids?.Pointer ?? IntPtr.Zero, taskId, OnLoadLeaderboards, OnLoadLeaderboardsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadLeaderboards(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKLeaderboard>>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadLeaderboardsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKLeaderboard>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region Submit Score

        /// <summary>
        /// Submits a score to the leaderboard.
        /// </summary>
        /// <param name="score">The score that the player earns.</param>
        /// <param name="context">An long value that your game uses.</param>
        /// <param name="player">The player who earns the score.</param>
        /// <returns></returns>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public Task SubmitScore(long score, long context, GKPlayer player)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKLeaderboard_SubmitScore(Pointer, taskId, score, context, player.Pointer, OnSubmitScoreSuccess, OnSubmitScoreError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnSubmitScoreSuccess(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnSubmitScoreError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        #region LoadPreviousOccurrence

        /// <summary>
        /// Loads the previous recurring leaderboard occurrence that the player submits a score to.
        /// </summary>
        /// <returns>The previous occurrence of this leaderboard that the player submits a score to, or the most recent occurrence if GameKit can't find the previous one.</returns>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public Task<GKLeaderboard> LoadPreviousOccurrence()
        {
            var tcs = InteropTasks.Create<GKLeaderboard>(out var taskId);
            Interop.GKLeaderboard_LoadPreviousOccurrence(Pointer, taskId, OnLoadPrevious, OnLoadPreviousError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadPrevious(long taskId, IntPtr pointer)
        {
            var leaderboard = pointer != IntPtr.Zero ? new GKLeaderboard(pointer) : null;
            InteropTasks.TrySetResultAndRemove(taskId, leaderboard);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadPreviousError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKLeaderboard>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        #region LoadEntries

        private delegate void GKLeaderboardLoadEntriesHandler(long taskId, IntPtr localEntry, IntPtr entries, int totalPlayerCount);

        /// <summary>
        /// Returns the scores for the local player and other players for the specified type of player, time period, and ranks.
        /// </summary>
        /// <param name="playerScope">Specifies whether to get scores from friends or all players.</param>
        /// <param name="timeScope">Specifies the time period for the scores. This parameter is applicable to nonrecurring leaderboards only. For recurring leaderboards, pass GKLeaderboard.TimeScope.allTime for this parameter.</param>
        /// <param name="rankMin">Specifies the range of ranks to use for getting the scores. The minimum rank is 1.</param>
        /// <param name="rankMax">Specifies the range of ranks to use for getting the scores. The total number of entries requested must not exceed 100.</param>
        /// <returns></returns>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public Task<GKLeaderboardLoadEntriesResponse> LoadEntries(PlayerScope playerScope, TimeScope timeScope, long rankMin, long rankMax)
        {
            var tcs = InteropTasks.Create<GKLeaderboardLoadEntriesResponse>(out var taskId);
            Interop.GKLeaderboard_LoadEntries(Pointer, taskId, playerScope, timeScope, rankMin, rankMax, OnLoadEntries, OnLoadEntriesError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(GKLeaderboardLoadEntriesHandler))]
        private static void OnLoadEntries(long taskId, IntPtr localEntry, IntPtr entries, int totalPlayerCount)
        {
            try
            {
                var response = new GKLeaderboardLoadEntriesResponse
                {
                    LocalPlayerEntry = PointerCast<GKLeaderboard.Entry>(localEntry), 
                    Entries = PointerCast<NSArray<GKLeaderboard.Entry>>(entries), 
                    TotalPlayerCount = totalPlayerCount
                };

                InteropTasks.TrySetResultAndRemove(taskId, response);
            }
            catch (Exception ex)
            {
                InteropTasks.TrySetExceptionAndRemove<GKLeaderboardLoadEntriesResponse>(taskId, ex);
            }
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadEntriesError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKLeaderboardLoadEntriesResponse>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        #region LoadEntriesForPlayers

        private delegate void GKLeaderboardLoadEntriesForPlayersHandler(long taskId, IntPtr localEntry, IntPtr entries);

        /// <summary>
        /// Returns the scores for the local player and other players for the specified time period.
        /// </summary>
        /// <param name="players">The players whose scores this method returns.</param>
        /// <param name="timeScope">
        /// Specifies the time period for the scores. This parameter is applicable to nonrecurring leaderboards only.
        /// For recurring leaderboards, pass GKLeaderboardTimeScopeAllTime for this parameter.
        /// </param>
        /// <returns></returns>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public Task<GKLeaderboardLoadEntriesForPlayersResponse> LoadEntriesForPlayers(NSArray<GKPlayer> players, TimeScope timeScope)
        {
            var tcs = InteropTasks.Create<GKLeaderboardLoadEntriesForPlayersResponse>(out var taskId);

            Interop.GKLeaderboard_LoadEntriesForPlayers(Pointer, taskId, players.Pointer, timeScope, OnLoadEntriesForPlayers, OnLoadEntriesForPlayersError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(GKLeaderboardLoadEntriesForPlayersHandler))]
        private static void OnLoadEntriesForPlayers(long taskId, IntPtr localEntry, IntPtr entries)
        {
            try
            {
                var response = new GKLeaderboardLoadEntriesForPlayersResponse
                {
                    LocalPlayerEntry = PointerCast<GKLeaderboard.Entry>(localEntry), 
                    Entries = PointerCast<NSArray<GKLeaderboard.Entry>>(entries), 
                };

                InteropTasks.TrySetResultAndRemove(taskId, response);
            }
            catch (Exception ex)
            {
                InteropTasks.TrySetExceptionAndRemove<GKLeaderboardLoadEntriesForPlayersResponse>(taskId, ex);
            }
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadEntriesForPlayersError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKLeaderboardLoadEntriesForPlayersResponse>(taskId, new GameKitException(errorPointer));
        }

        #endregion

#if !UNITY_TVOS
        #region LoadImage
        
        /// <summary>
        /// Loads the image for the default leaderboard.
        /// </summary>
        /// <returns></returns>
        public Task<Texture2D> LoadImage()
        {
            var tcs = InteropTasks.Create<Texture2D>(out var taskId);
            Interop.GKLeaderboard_LoadImage(Pointer, taskId, OnLoadImage, OnLoadImageError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskImageCallback))]
        private static void OnLoadImage(long taskId, IntPtr nsDataPtr)
        {
            try
            {
                InteropTasks.TrySetResultAndRemove(taskId, Texture2DExtensions.CreateFromNSDataPtr(nsDataPtr));
            }
            catch (Exception ex)
            {
                InteropTasks.TrySetExceptionAndRemove<Texture2D>(taskId, ex);
            }
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadImageError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<Texture2D>(taskId, new GameKitException(errorPointer));
        }
        #endregion
#endif

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern long GKLeaderboardEntry_GetContext(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKLeaderboardEntry_GetDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKLeaderboardEntry_GetFormattedScore(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKLeaderboardEntry_GetPlayer(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern long GKLeaderboardEntry_GetRank(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern long GKLeaderboardEntry_GetScore(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string GKLeaderboard_GetBaseLeaderboardId(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKLeaderboard_GetTitle(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern LeaderboardType GKLeaderboard_GetType(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKLeaderboard_GetGroupIdentifier(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKLeaderboard_GetStartDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKLeaderboard_GetNextStartDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern long GKLeaderboard_GetDuration(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLeaderboard_LoadLeaderboards(IntPtr idsNsArray, long taskId, SuccessTaskCallback<IntPtr> onCallback, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLeaderboard_SubmitScore(IntPtr pointer, long taskId, long score, long context, IntPtr player, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLeaderboard_LoadPreviousOccurrence(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onCallback, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLeaderboard_LoadEntries(IntPtr pointer, long taskId, PlayerScope playerScope, TimeScope timeScope, long rankMin, long rankMax, GKLeaderboardLoadEntriesHandler onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLeaderboard_LoadEntriesForPlayers(IntPtr gkLeaderboardPtr, long taskId, IntPtr players, TimeScope timeScope, GKLeaderboardLoadEntriesForPlayersHandler onSuccess, NSErrorTaskCallback onError);
#if !UNITY_TVOS
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLeaderboard_LoadImage(IntPtr pointer, long taskId, SuccessTaskImageCallback onSuccess, NSErrorTaskCallback onError);
#endif            
        }
    }
}

