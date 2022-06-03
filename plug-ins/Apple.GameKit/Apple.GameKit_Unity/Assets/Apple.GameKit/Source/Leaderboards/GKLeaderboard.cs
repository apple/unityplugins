using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;
using UnityEngine;

namespace Apple.GameKit.Leaderboards
{
    public class GKLeaderboard : InteropReference
    {
        /// <summary>
        /// Specifies whether a leaderboard is recurring.
        /// </summary>
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
        public class Entry : InteropReference
        {
            #region Init & Dispose
            [DllImport(InteropUtility.DLLName)]
            private static extern void GKLeaderboardEntry_Free(IntPtr pointer);
            internal Entry(IntPtr pointer) : base(pointer) {}

            protected override void OnDispose(bool isDisposing)
            {
                base.OnDispose(isDisposing);

                if (Pointer != IntPtr.Zero)
                {
                    GKLeaderboardEntry_Free(Pointer);
                    Pointer = IntPtr.Zero;
                }
            }

            #endregion
            
            #region Context
            [DllImport(InteropUtility.DLLName)]
            private static extern long GKLeaderboardEntry_GetContext(IntPtr pointer);

            /// <summary>
            /// An integer value that your game uses.
            /// </summary>
            public long Context
            {
                get => GKLeaderboardEntry_GetContext(Pointer);
            }
            #endregion
            
            #region Date
            [DllImport(InteropUtility.DLLName)]
            private static extern long GKLeaderboardEntry_GetDate(IntPtr pointer);

            /// <summary>
            /// The date and time when the player earns the score.
            /// </summary>
            public DateTimeOffset Date
            {
                get
                {
                    var time = GKLeaderboardEntry_GetDate(Pointer);
                    return DateTimeOffset.FromUnixTimeSeconds(time);
                }
            }
            #endregion
            
            #region FormattedScore
            [DllImport(InteropUtility.DLLName)]
            private static extern string GKLeaderboardEntry_GetFormattedScore(IntPtr pointer);

            /// <summary>
            /// The playerâ€™s score as a localized string.
            /// </summary>
            public string FormattedScore
            {
                get => GKLeaderboardEntry_GetFormattedScore(Pointer);
            }
            #endregion
            
            #region Player
            [DllImport(InteropUtility.DLLName)]
            private static extern IntPtr GKLeaderboardEntry_GetPlayer(IntPtr pointer);

            /// <summary>
            /// The player who earns the score.
            /// </summary>
            public GKPlayer Player
            {
                get
                {
                    var pointer = GKLeaderboardEntry_GetPlayer(Pointer);
                    
                    if(pointer != IntPtr.Zero)
                        return new GKPlayer(pointer);

                    return null;
                }
            }
            #endregion
            
            #region Rank
            [DllImport(InteropUtility.DLLName)]
            private static extern long GKLeaderboardEntry_GetRank(IntPtr pointer);

            /// <summary>
            /// The position of the score in the results of a leaderboard search.
            /// </summary>
            public long Rank
            {
                get => GKLeaderboardEntry_GetRank(Pointer);
            }
            #endregion
            
            #region Score
            [DllImport(InteropUtility.DLLName)]
            private static extern long GKLeaderboardEntry_GetScore(IntPtr pointer);

            /// <summary>
            /// The score that the player earns.
            /// </summary>
            public long Score
            {
                get => GKLeaderboardEntry_GetScore(Pointer);
            }
            #endregion
        }
        
        #region Init & Dispose
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKLeaderboard_Free(IntPtr pointer);
        
        internal GKLeaderboard(IntPtr pointer) : base(pointer) {}

        protected override void OnDispose(bool isDisposing)
        {
            base.OnDispose(isDisposing);

            if (Pointer != IntPtr.Zero)
            {
                GKLeaderboard_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        #endregion
        
        #region BaseLeaderboardId
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKLeaderboard_GetBaseLeaderboardId(IntPtr pointer);

        /// <summary>
        /// The ID that Game Center uses to identify this leaderboard.
        /// </summary>
        public string BaseLeaderboardId
        {
            get => GKLeaderboard_GetBaseLeaderboardId(Pointer);
        }
        #endregion
        
        #region Title
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKLeaderboard_GetTitle(IntPtr pointer);

        /// <summary>
        /// The localized title for the leaderboard.
        /// </summary>
        public string Title
        {
            get => GKLeaderboard_GetTitle(Pointer);
        }
        #endregion
        
        #region Type
        [DllImport(InteropUtility.DLLName)]
        private static extern LeaderboardType GKLeaderboard_GetType(IntPtr pointer);

        /// <summary>
        /// The type of leaderboard, classic or recurring.
        /// </summary>
        public LeaderboardType Type
        {
            get => GKLeaderboard_GetType(Pointer);
        }
        #endregion
        
        #region Group Identifier
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKLeaderboard_GetGroupIdentifier(IntPtr pointer);

        /// <summary>
        /// The identifier for the group the leaderboard belongs to.
        /// </summary>
        public string GroupIdentifier
        {
            get => GKLeaderboard_GetGroupIdentifier(Pointer);
        }
        #endregion
        
        #region Start Date
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKLeaderboard_GetStartDate(IntPtr pointer);

        /// <summary>
        /// The date and time a recurring leaderboard occurrence starts accepting scores.
        /// </summary>
        public DateTimeOffset StartDate
        {
            get
            {
                var time = GKLeaderboard_GetStartDate(Pointer);
                return DateTimeOffset.FromUnixTimeSeconds(time);
            }
        }
        #endregion
        
        #region Next Start Date
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKLeaderboard_GetNextStartDate(IntPtr pointer);

        /// <summary>
        /// The date and time the next recurring leaderboard occurrence starts accepting scores.
        /// </summary>
        public DateTimeOffset NextStartDate
        {
            get
            {
                var time = GKLeaderboard_GetNextStartDate(Pointer);
                return DateTimeOffset.FromUnixTimeSeconds(time);
            }
        }
        #endregion
        
        #region Duration
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKLeaderboard_GetDuration(IntPtr pointer);

        /// <summary>
        /// The duration from the start date that a recurring leaderboard occurrence accepts scores.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                var time = GKLeaderboard_GetDuration(Pointer);
                return new TimeSpan(0, 0, (int)time);
            }
        }
        #endregion

        #region Load Leaderboards
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKLeaderboard_LoadLeaderboards(IntPtr idsNsArray, long taskId, SuccessTaskCallback<IntPtr> onCallback, NSErrorTaskCallback onError);

        /// <summary>
        /// Loads leaderboards for the specified leaderboard IDs that Game Center uses.
        /// </summary>
        /// <param name="identifiers">The leaderboards that match the IDs.</param>
        /// <returns></returns>
        public static Task<NSArray<GKLeaderboard>> LoadLeaderboards(params string[] identifiers)
        {
            var tcs = InteropTasks.Create<NSArray<GKLeaderboard>>(out var taskId);
            
            // Prepare identifiers array...
            var ids = NSMutableArrayFactory.Init<NSMutableArrayString, string>();
            foreach (var identifier in identifiers)
            {
                ids.Add(identifier);
            }
            
            GKLeaderboard_LoadLeaderboards(ids.Pointer, taskId, OnLoadLeaderboards, OnLoadLoaderboardsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadLeaderboards(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, (NSArray<GKLeaderboard>)PointerCast<NSArrayGKLeaderboard>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadLoaderboardsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKLeaderboard>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region Submit Score
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKLeaderboard_SubmitScore(IntPtr pointer, long taskId, long score, long context, IntPtr player, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Submits a score to the leaderboard.
        /// </summary>
        /// <param name="score">The score that the player earns.</param>
        /// <param name="context">An long value that your game uses.</param>
        /// <param name="player">The player who earns the score.</param>
        /// <returns></returns>
        public Task SubmitScore(long score, long context, GKPlayer player)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            GKLeaderboard_SubmitScore(Pointer, taskId, score, context, player.Pointer, OnSubmitScoreSuccess, OnSubmitScoreError);
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
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKLeaderboard_LoadPreviousOccurrence(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onCallback, NSErrorTaskCallback onError);

        /// <summary>
        /// Loads the previous recurring leaderboard occurrence that the player submits a score to.
        /// </summary>
        /// <returns>The previous occurrence of this leaderboard that the player submits a score to, or the most recent occurrence if GameKit can't find the previous one.</returns>
        public Task<GKLeaderboard> LoadPreviousOccurrence()
        {
            var tcs = InteropTasks.Create<GKLeaderboard>(out var taskId);
            GKLeaderboard_LoadPreviousOccurrence(Pointer, taskId, OnLoadPrevious, OnLoadPreviousError);
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
        
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKLeaderboard_LoadEntries(IntPtr pointer, long taskId, PlayerScope playerScope, TimeScope timeScope, long rankMin, long rankMax, GKLeaderboardLoadEntriesHandler onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Returns the scores for the local player and other players for the specified type of player, time period, and ranks.
        /// </summary>
        /// <param name="playerScope">Specifies whether to get scores from friends or all players.</param>
        /// <param name="timeScope">Specifies the time period for the scores. This parameter is applicable to nonrecurring leaderboards only. For recurring leaderboards, pass GKLeaderboard.TimeScope.allTime for this parameter.</param>
        /// <param name="rankMin">Specifies the range of ranks to use for getting the scores. The minimum rank is 1 and the maximum rank is 100.</param>
        /// <param name="rankMax">Specifies the range of ranks to use for getting the scores. The minimum rank is 1 and the maximum rank is 100.</param>
        /// <returns></returns>
        public Task<GKLeaderboardLoadEntriesResponse> LoadEntries(PlayerScope playerScope, TimeScope timeScope, long rankMin, long rankMax)
        {
            var tcs = InteropTasks.Create<GKLeaderboardLoadEntriesResponse>(out var taskId);
            
            // Ensure ranks fall within min & max...
            rankMin = (long)Mathf.Clamp(rankMin, 1, 100);
            rankMax = (long)Mathf.Clamp(rankMax, 1, 100);
            
            GKLeaderboard_LoadEntries(Pointer, taskId, playerScope, timeScope, rankMin, rankMax, OnLoadEntries, OnLoadEntriesError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(GKLeaderboardLoadEntriesHandler))]
        private static void OnLoadEntries(long taskId, IntPtr localEntry, IntPtr entries, int totalPlayerCount)
        {
            var response = new GKLeaderboardLoadEntriesResponse
            {
                LocalPlayerEntry = PointerCast<GKLeaderboard.Entry>(localEntry), 
                Entries = PointerCast<NSMutableArrayGKLeaderboardEntry>(entries), 
                TotalPlayerCount = totalPlayerCount
            };

            InteropTasks.TrySetResultAndRemove(taskId, response);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadEntriesError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKLeaderboardLoadEntriesResponse>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region LoadImage
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKLeaderboard_LoadImage(IntPtr pointer, long taskId, SuccessTaskImageCallback onSuccess, NSErrorTaskCallback onError);
        
        /// <summary>
        /// Loads the image for the default leaderboard.
        /// </summary>
        /// <returns></returns>
        public Task<Texture2D> LoadImage()
        {
            var tcs = InteropTasks.Create<Texture2D>(out var taskId);
            GKLeaderboard_LoadImage(Pointer, taskId, OnLoadImage, OnLoadImageError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskImageCallback))]
        private static void OnLoadImage(long taskId, int width, int height, IntPtr data, int dataLength)
        {
            Texture2D texture = null;

            if (dataLength > 0)
            {
                var image = new byte[dataLength];
                Marshal.Copy(data, image, 0, dataLength);

                texture = new Texture2D(width, height);
                texture.LoadImage(image);
            }

            InteropTasks.TrySetResultAndRemove(taskId, texture);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadImageError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<Texture2D>(taskId, new GameKitException(errorPointer));
        }
        #endregion
    }

}