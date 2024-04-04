using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;
using UnityEngine.Scripting;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// An object that creates matches with other players without presenting an interface to the players.
    /// </summary>
    public class GKMatchmaker : NSObject
    {
        private static readonly InteropWeakMap<GKMatchmaker> _instanceMap = new InteropWeakMap<GKMatchmaker>();

        [Preserve]
        internal GKMatchmaker(IntPtr pointer) : base(pointer)
        {
            _instanceMap.Add(this);
        }

        protected override void OnDispose(bool isDisposing)
        {
            _instanceMap.Remove(this);
            base.OnDispose(isDisposing);
        }

        private static GKMatchmaker _shared;
        
        /// <summary>
        /// Returns the singleton matchmaker instance.
        /// </summary>
        public static GKMatchmaker Shared => _shared ??= PointerCast<GKMatchmaker>(Interop.GKMatchmaker_GetShared());
        
        #region FindMatch

        /// <summary>
        /// Initiates a request to find players for a peer-to-peer match.
        /// </summary>
        /// <param name="matchRequest">The configuration for the match.</param>
        /// <returns></returns>
        public Task<GKMatch> FindMatch(GKMatchRequest matchRequest)
        {
            var tcs = InteropTasks.Create<GKMatch>(out var taskId);
            Interop.GKMatchmaker_FindMatch(Pointer, taskId, matchRequest.Pointer, OnFindMatchSuccess, OnFindMatchError);
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

        /// <summary>
        /// Creates a match from an invitation that the local player accepts.
        /// </summary>
        /// <param name="invite">The invitation that the local player accepts.</param>
        /// <returns></returns>
        public Task<GKMatch> Match(GKInvite invite)
        {
            var tcs = InteropTasks.Create<GKMatch>(out var taskId);
            Interop.GKMatchmaker_MatchForInvite(Pointer, taskId, invite.Pointer, OnInviteMatchSuccess, OnInviteMatchError);
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

        #region FinishMatchmaking
        public void FinishMatchmaking(GKMatch match)
        {
            Interop.GKMatchmaker_FinishMatchmaking(Pointer, match.Pointer);
        }
        #endregion
        
        #region FindPlayers

        /// <summary>
        /// Initiates a request to find players for a hosted match.
        /// </summary>
        /// <param name="matchRequest">The configuration for the match.</param>
        /// <returns>An NSArray of the players that join the match. If unsuccessful, this parameter is nil.</returns>
        public Task<NSArray<GKPlayer>> FindPlayers(GKMatchRequest matchRequest)
        {
            var tcs = InteropTasks.Create<NSArray<GKPlayer>>(out var taskId);
            Interop.GKMatchmaker_FindPlayers(Pointer, taskId, matchRequest.Pointer, OnFindPlayers, OnFindPlayersError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnFindPlayers(long taskId, IntPtr playersArrayPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKPlayer>>(playersArrayPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnFindPlayersError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKPlayer>>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        #region FindMatchedPlayers

        /// <summary>
        /// Initiates a request to find players for a hosted match that uses matchmaking rules.
        /// </summary>
        /// <param name="matchRequest">The configuration for the match.</param>
        /// <returns>The players that join the match, including their properties that matchmaking rules uses. If unsuccessful, this parameter is null.</returns>
        [Introduced(iOS: "17.2", macOS: "14.2", tvOS: "17.2", visionOS: "1.1")]
        public Task<GKMatchedPlayers> FindMatchedPlayers(GKMatchRequest matchRequest)
        {
            var tcs = InteropTasks.Create<GKMatchedPlayers>(out var taskId);
            Interop.GKMatchMaker_FindMatchedPlayers(Pointer, taskId, matchRequest.Pointer, OnFindMatchedPlayersSuccess, OnFindMatchedPlayersError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnFindMatchedPlayersSuccess(long taskId, IntPtr gkMatchedPlayersPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<GKMatchedPlayers>(gkMatchedPlayersPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnFindMatchedPlayersError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKMatchedPlayers>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        #region AddPlayers

        /// <summary>
        /// Invites additional players to an existing match.
        /// </summary>
        /// <param name="match">The match to which GameKit adds the players.</param>
        /// <param name="matchRequest">The configuration for the match.</param>
        /// <returns></returns>
        public Task AddPlayers(GKMatch match, GKMatchRequest matchRequest)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKMatchmaker_AddPlayers(Pointer, taskId, match.Pointer, matchRequest.Pointer, OnAddPlayers, OnAddPlayersError);
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

        /// <summary>
        /// Finds the number of players, across player groups, who recently requested a match.
        /// </summary>
        /// <returns>The number of match requests for all player groups during the previous 60 seconds.</returns>
        public Task<long> QueryActivity()
        {
            var tcs = InteropTasks.Create<long>(out var taskId);
            Interop.GKMatchmaker_QueryActivity(Pointer, taskId, OnQueryActivity, OnQueryActivityError);
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

        #region QueryQueueActivity

        /// <summary>
        /// Finds the number of players in a specific queue who recently requested a match.
        /// </summary>
        /// <param name="queueName">
        /// The name of the queue that Game Center places the match requests in, which it uses for finding players when using matchmaking rules. This uniform type identifier (UTI) contains only alphanumeric characters (A-Z, a-z, 0-9), hyphens (-), or periods (.).
        /// The string should be in reverse-DNS format and queue names are case sensitive.
        /// </param>
        /// <returns>The number of match requests in the queue during the previous 60 seconds.</returns>
        [Introduced(iOS: "17.2", macOS: "14.2", tvOS: "17.2", visionOS: "1.1")]
        public Task<long> QueryQueueActivity(string queueName)
        {
            var tcs = InteropTasks.Create<long>(out var taskId);
            Interop.GKMatchmaker_QueryQueueActivity(Pointer, taskId, queueName, OnQueryQueueActivitySuccess, OnQueryQueueActivityError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<long>))]
        private static void OnQueryQueueActivitySuccess(long taskId, long activity)
        {
            InteropTasks.TrySetResultAndRemove(taskId, activity);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnQueryQueueActivityError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<long>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        #region QueryPlayerGroupActivity

        /// <summary>
        /// Finds the number of players in a player group who recently requested a match.
        /// </summary>
        /// <param name="playerGroupId">A number that uniquely identifies a subset of players in your game.</param>
        /// <returns>The number of match requests for the player group during the previous 60 seconds.</returns>
        public Task<long> QueryPlayerGroupActivity(long playerGroupId)
        {
            var tcs = InteropTasks.Create<long>(out var taskId);
            Interop.GKMatchmaker_QueryPlayerGroupActivity(Pointer, taskId, playerGroupId, OnQueryPlayerGroupActivity, OnQueryPlayerGroupActivityError);
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

        /// <summary>
        /// Cancels a matchmaking request.
        /// </summary>
        public void Cancel() => Interop.GKMatchmaker_Cancel(Pointer);
        
        /// <summary>
        /// Cancels a pending invitation to another player.
        /// </summary>
        /// <param name="player"></param>
        public void CancelPendingInvite(GKPlayer player) => Interop.GKMatchmaker_CancelPendingInvite(Pointer, player.Pointer);

        /// <summary>
        /// Finds nearby players through Bluetooth or WiFi on the same subnet.
        /// Fires the NearbyPlayerReachable event whenever the reachability of a nearby player changes.
        /// </summary>
        [Introduced(iOS: "8.0", macOS: "10.10", tvOS: "9.0")]
        public void StartBrowsingForNearbyPlayers()
        {
            Interop.GKMatchmaker_StartBrowsingForNearbyPlayers(Pointer, OnNearbyPlayerReachable);
        }

        /// <summary>
        /// Stops finding nearby players.
        /// </summary>
        [Introduced(iOS: "6.0", macOS: "10.9", tvOS: "9.0")]
        public void StopBrowsingForNearbyPlayers()
        {
            Interop.GKMatchmaker_StopBrowsingForNearbyPlayers(Pointer);
        }

        /// <summary>
        /// Event fired whenever the reachability of a nearby player changes while browsing for nearby players.
        /// </summary>
        public event NearbyPlayerReachableHandler NearbyPlayerReachable;

        public delegate void NearbyPlayerReachableHandler(GKPlayer player, bool isReachable);
        internal delegate void InternalNearbyPlayerReachableHandler(IntPtr gkMatchmakerPtr, IntPtr gkPlayerPtr, bool isReachable);

        [MonoPInvokeCallback(typeof(InternalNearbyPlayerReachableHandler))]
        private static void OnNearbyPlayerReachable(IntPtr gkMatchmakerPtr, IntPtr gkPlayerPtr, bool isReachable)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                if (_instanceMap.TryGet(gkMatchmakerPtr, out var gkMatchmaker))
                {
                    var gkPlayer = PointerCast<GKPlayer>(gkPlayerPtr);
                    gkMatchmaker.NearbyPlayerReachable?.Invoke(gkPlayer, isReachable);
                }
            });
        }

#if !UNITY_TVOS
        /// <summary>
        /// Begins a SharePlay activity for your game when a FaceTime call is active.
        /// Fires the PlayerJoiningGroupActivity event whenever a player requests to join the group activity.
        /// </summary>
        [Introduced(iOS: "16.2", macOS: "13.1")]
        public void StartGroupActivity()
        {
            Interop.GKMatchmaker_StartGroupActivity(Pointer, OnPlayerJoiningGroupActivity);
        }

        /// <summary>
        /// Ends a SharePlay activity for the entire group, which the local player activates.
        /// </summary>
        [Introduced(iOS: "16.2", macOS: "13.1")]
        public void StopGroupActivity()
        {
            Interop.GKMatchmaker_StopGroupActivity(Pointer);
        }

        /// <summary>
        /// Event fired whenever a player requests to join the group activity while the group is active.
        /// </summary>
        public event PlayerJoiningGroupActivityHandler PlayerJoiningGroupActivity;

        public delegate void PlayerJoiningGroupActivityHandler(GKPlayer player);
        internal delegate void InternalPlayerJoiningGroupActivityHandler(IntPtr gkMatchmakerPtr, IntPtr gkPlayerPtr);

        [MonoPInvokeCallback(typeof(InternalPlayerJoiningGroupActivityHandler))]
        private static void OnPlayerJoiningGroupActivity(IntPtr gkMatchmakerPtr, IntPtr gkPlayerPtr)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                if (_instanceMap.TryGet(gkMatchmakerPtr, out var gkMatchmaker))
                {
                    var gkPlayer = PointerCast<GKPlayer>(gkPlayerPtr);
                    gkMatchmaker.PlayerJoiningGroupActivity?.Invoke(gkPlayer);
                }
            });
        }
#endif // !UNITY_TVOS

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKMatchmaker_GetShared();
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmaker_FindMatch(IntPtr pointer, long taskId, IntPtr matchRequest, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmaker_MatchForInvite(IntPtr pointer, long taskId, IntPtr invite, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmaker_FinishMatchmaking(IntPtr gkMatchmakerPtr, IntPtr gkMatchPtr);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmaker_FindPlayers(IntPtr pointer, long taskId, IntPtr request, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchMaker_FindMatchedPlayers(IntPtr gkMatchmakerPtr, long taskId, IntPtr gkMatchRequestPtr, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmaker_AddPlayers(IntPtr pointer, long taskId, IntPtr match, IntPtr matchRequest, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmaker_QueryActivity(IntPtr pointer, long taskId, SuccessTaskCallback<long> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmaker_QueryQueueActivity(IntPtr pointer, long taskId, string queueName, SuccessTaskCallback<long> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmaker_QueryPlayerGroupActivity(IntPtr pointer, long taskId, long playerGroupId, SuccessTaskCallback<long> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmaker_Cancel(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmaker_CancelPendingInvite(IntPtr pointer, IntPtr player);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmaker_StartBrowsingForNearbyPlayers(IntPtr gkMatchmakerPtr, InternalNearbyPlayerReachableHandler nearbyPlayerReachableHandler);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmaker_StopBrowsingForNearbyPlayers(IntPtr gkMatchmakerPtr);
#if !UNITY_TVOS
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmaker_StartGroupActivity(IntPtr gkMatchmakerPtr, InternalPlayerJoiningGroupActivityHandler playerJoiningGroupActivityHandler);
#endif            
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmaker_StopGroupActivity(IntPtr gkMatchmakerPtr);
        }
    }
}
