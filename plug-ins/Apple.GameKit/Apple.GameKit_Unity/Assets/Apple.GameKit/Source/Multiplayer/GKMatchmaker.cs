using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// An object that creates matches with other players without presenting an interface to the players.
    /// </summary>
    public class GKMatchmaker : InteropReference
    {
        #region Init
        internal GKMatchmaker(IntPtr pointer) : base(pointer) {}
        #endregion
        
        #region Shared
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKMatchmaker_GetShared();

        private static GKMatchmaker _shared;
        
        /// <summary>
        /// Returns the singleton matchmaker instance.
        /// </summary>
        public static GKMatchmaker Shared
        {
            get
            {
                if(_shared == null)
                    _shared = PointerCast<GKMatchmaker>(GKMatchmaker_GetShared());
                
                return _shared;
            }
        }
        #endregion
        
        #region FindMatch
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmaker_FindMatch(IntPtr pointer, long taskId, IntPtr matchRequest, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Initiates a request to find players for a peer-to-peer match.
        /// </summary>
        /// <param name="matchRequest">The configuration for the match.</param>
        /// <returns></returns>
        public Task<GKMatch> FindMatch(GKMatchRequest matchRequest)
        {
            var tcs = InteropTasks.Create<GKMatch>(out var taskId);
            GKMatchmaker_FindMatch(Pointer, taskId, matchRequest.Pointer, OnFindMatchSuccess, OnFindMatchError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnFindMatchSuccess(long taskId, IntPtr matchPointer)
        {
            var match = matchPointer != IntPtr.Zero ? new GKMatch(matchPointer) : null; 
            InteropTasks.TrySetResultAndRemove(taskId, match);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnFindMatchError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKMatch>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region Match
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmaker_MatchForInvite(IntPtr pointer, long taskId, IntPtr invite, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Creates a match from an invitation that the local player accepts.
        /// </summary>
        /// <param name="invite">The invitation that the local player accepts.</param>
        /// <returns></returns>
        public Task<GKMatch> Match(GKInvite invite)
        {
            var tcs = InteropTasks.Create<GKMatch>(out var taskId);
            GKMatchmaker_MatchForInvite(Pointer, taskId, invite.Pointer, OnInviteMatchSuccess, OnInviteMatchError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnInviteMatchSuccess(long taskId, IntPtr matchPointer)
        {
            var match = matchPointer != IntPtr.Zero ? new GKMatch(matchPointer) : null;
            InteropTasks.TrySetResultAndRemove(taskId, match);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnInviteMatchError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKMatch>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region FindPlayers
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmaker_FindPlayers(IntPtr pointer, long taskId, IntPtr request, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Initiates a request to find players for a hosted match.
        /// </summary>
        /// <param name="matchRequest">The configuration for the match.</param>
        /// <returns>An NSArray of the players that join the match. If unsuccessful, this parameter is nil.</returns>
        public Task<NSArray<GKPlayer>> FindPlayers(GKMatchRequest matchRequest)
        {
            var tcs = InteropTasks.Create<NSArray<GKPlayer>>(out var taskId);
            GKMatchmaker_FindPlayers(Pointer, taskId, matchRequest.Pointer, OnFindPlayers, OnFindPlayersError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnFindPlayers(long taskId, IntPtr playersArrayPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArrayGKPlayer>(playersArrayPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnFindPlayersError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKPlayer>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region AddPlayers
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmaker_AddPlayers(IntPtr pointer, long taskId, IntPtr match, IntPtr matchRequest, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Invites additional players to an existing match.
        /// </summary>
        /// <param name="match">The match to which GameKit adds the players.</param>
        /// <param name="matchRequest">The configuration for the match.</param>
        /// <returns></returns>
        public Task AddPlayers(GKMatch match, GKMatchRequest matchRequest)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            GKMatchmaker_AddPlayers(Pointer, taskId, match.Pointer, matchRequest.Pointer, OnAddPlayers, OnAddPlayersError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnAddPlayers(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnAddPlayersError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region QueryActivity
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmaker_QueryActivity(IntPtr pointer, long taskId, SuccessTaskCallback<long> onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Finds the number of players, across player groups, who recently requested a match.
        /// </summary>
        /// <returns>The number of match requests for all player groups during the previous 60 seconds.</returns>
        public Task<long> QueryActivity()
        {
            var tcs = InteropTasks.Create<long>(out var taskId);
            GKMatchmaker_QueryActivity(Pointer, taskId, OnQueryActivity, OnQueryActivityError);
            return tcs.Task;
        }
        
        [MonoPInvokeCallback(typeof(SuccessTaskCallback<long>))]
        private static void OnQueryActivity(long taskId, long activity)
        {
            InteropTasks.TrySetResultAndRemove(taskId, activity);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnQueryActivityError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<long>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region QueryPlayerGroupActivity
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmaker_QueryPlayerGroupActivity(IntPtr pointer, long taskId, long playerGroupId, SuccessTaskCallback<long> onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Finds the number of players in a player group who recently requested a match.
        /// </summary>
        /// <param name="playerGroupId">A number that uniquely identifies a subset of players in your game.</param>
        /// <returns>The number of match requests for the player group during the previous 60 seconds.</returns>
        public Task<long> QueryPlayerGroupActivity(long playerGroupId)
        {
            var tcs = InteropTasks.Create<long>(out var taskId);
            GKMatchmaker_QueryPlayerGroupActivity(Pointer, taskId, playerGroupId, OnQueryPlayerGroupActivity, OnQueryPlayerGroupActivityError);
            return tcs.Task;
        }
        
        [MonoPInvokeCallback(typeof(SuccessTaskCallback<long>))]
        private static void OnQueryPlayerGroupActivity(long taskId, long activity)
        {
            InteropTasks.TrySetResultAndRemove(taskId, activity);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnQueryPlayerGroupActivityError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<long>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region Cancel
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmaker_Cancel(IntPtr pointer);

        /// <summary>
        /// Cancels a matchmaking request.
        /// </summary>
        public void Cancel()
        {
            GKMatchmaker_Cancel(Pointer);
        }
        #endregion
        
        #region CancelPendingInvite
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmaker_CancelPendingInvite(IntPtr pointer, IntPtr player);

        /// <summary>
        /// Cancels a pending invitation to another player.
        /// </summary>
        /// <param name="player"></param>
        public void CancelPendingInvite(GKPlayer player)
        {
            GKMatchmaker_CancelPendingInvite(Pointer, player.Pointer);
        }
        #endregion
    }
}