using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;
using Apple.GameKit.Players;

namespace Apple.GameKit
{
    public class GKLocalPlayer : GKPlayer
    {
        #region Init & Dispose
        internal GKLocalPlayer(IntPtr pointer) : base(pointer) {}
        #endregion
        
        #region IsAuthenticated
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKLocalPlayer_GetIsAuthenticated(IntPtr pointer);

        /// <summary>
        /// A Boolean value that indicates whether a local player has signed in to Game Center.
        /// </summary>
        public bool IsAuthenticated
        {
            get => GKLocalPlayer_GetIsAuthenticated(Pointer);
        }
        #endregion
        
        #region IsUnderage
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKLocalPlayer_GetIsUnderage(IntPtr pointer);

        /// <summary>
        /// A Boolean value that indicates whether the local player is underage.
        /// </summary>
        public bool IsUnderage
        {
            get => GKLocalPlayer_GetIsUnderage(Pointer);
        }
        #endregion
        
        #region IsMultiplayerGamingRestricted
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKLocalPlayer_GetIsMultiplayerGamingRestricted(IntPtr pointer);

        /// <summary>
        /// A Boolean value that indicates whether the player can join multiplayer games.
        /// </summary>
        public bool IsMultiplayerGamingRestricted
        {
            get => GKLocalPlayer_GetIsMultiplayerGamingRestricted(Pointer);
        }
        #endregion
        
        #region IsPersonalizedCommunicationRestricted
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKLocalPlayer_GetIsPersonalizedCommunicationRestricted(IntPtr pointer);

        /// <summary>
        /// A Boolean value that indicates whether the player can use personalized communication on the device.
        /// </summary>
        public bool IsPersonalizedCommunicationRestricted
        {
            get => GKLocalPlayer_GetIsPersonalizedCommunicationRestricted(Pointer);
        }
        #endregion
        
        #region Local
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKLocalPlayer_GetLocal();

        private static GKLocalPlayer _local;
        
        /// <summary>
        /// The shared instance of the local player.
        /// </summary>
        public static GKLocalPlayer Local
        {
            get
            {
                if (_local == null)
                {
                    var pointer = GKLocalPlayer_GetLocal();
                    _local = new GKLocalPlayer(pointer);
                }

                return _local;
            }
        }
        #endregion
        
        #region FetchItems
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKLocalPlayer_FetchItems(IntPtr pointer, long taskId, SuccessTaskCallback<GKIdentityVerificationResponse> onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Generates a signature so that a third-party server can authenticate the local player.
        /// </summary>
        /// <returns></returns>
        public Task<GKIdentityVerificationResponse> FetchItems()
        {
            var tcs = InteropTasks.Create<GKIdentityVerificationResponse>(out var taskId);
            GKLocalPlayer_FetchItems(Pointer, taskId, OnFetchItems, OnFetchItemsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<GKIdentityVerificationResponse>))]
        private static void OnFetchItems(long taskId, GKIdentityVerificationResponse response)
        {
            InteropTasks.TrySetResultAndRemove(taskId, response);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnFetchItemsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKIdentityVerificationResponse>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region Authenticate
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKLocalPlayer_Authenticate(long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Assigns a handler that GameKit calls while authenticating the local player. The task will
        /// return when the user has been authenticated or failed via a GameKitException. Only one
        /// handler may be assigned, and as such, you should only call this once; otherwise use
        /// the static GKLocalPlayer.Local property.
        /// </summary>
        /// <returns></returns>
        public static Task<GKLocalPlayer> Authenticate()
        {
            var tcs = InteropTasks.Create<GKLocalPlayer>(out var taskId);
            GKLocalPlayer_Authenticate(taskId, OnAuthenticate, OnAuthenticateError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnAuthenticate(long taskId, IntPtr pointer)
        {
            _local = new GKLocalPlayer(pointer);
            InteropTasks.TrySetResultAndRemove(taskId, _local);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnAuthenticateError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKLocalPlayer>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region LoadFriends
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKLocalPlayer_LoadFriends(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onCallback, NSErrorTaskCallback onError);

        /// <summary>
        /// Loads the local player's friends list if the local player and their friends grant access.
        /// </summary>
        /// <returns>An NSArray of GKPlayer. The player's friends who also grant your game access to their friends.
        /// The local player and their friends authorization status must be GKFriendsAuthorizationStatus.authorized.
        /// The local player and their friends must use a version of your game with the same bundle ID.
        ///</returns>
        public Task<NSArray<GKPlayer>> LoadFriends()
        {
            var tcs = InteropTasks.Create<NSArray<GKPlayer>>(out var taskId);
            GKLocalPlayer_LoadFriends(Pointer, taskId, OnLoadFriends, OnLoadFriendsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadFriends(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, (NSArray<GKPlayer>)PointerCast<NSArrayGKPlayer>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadFriendsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKPlayer>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region LoadChallengableFriends
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKLocalPlayer_LoadChallengableFriends(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onCallback, NSErrorTaskCallback onError);

        /// <summary>
        /// Loads players to whom the local player can issue a challenge.
        /// </summary>
        /// <returns>
        /// An NSArray of GKPlayer. Players to whom the local player can issue a challenge. The local player can issue a challenge to a player with a friend level of FL1 or FL2.
        ///</returns>
        public Task<NSArray<GKPlayer>> LoadChallengeableFriends()
        {
            var tcs = InteropTasks.Create<NSArray<GKPlayer>>(out var taskId);
            GKLocalPlayer_LoadChallengableFriends(Pointer, taskId, OnLoadChallengableFriends, OnLoadChallengableFriendsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadChallengableFriends(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, (NSArray<GKPlayer>)PointerCast<NSArrayGKPlayer>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadChallengableFriendsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKPlayer>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region LoadRecentPlayers
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKLocalPlayer_LoadRecentPlayers(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onCallback, NSErrorTaskCallback onError);

        /// <summary>
        /// Loads players from the friends list or players that recently participated in a game with the local player.
        /// </summary>
        /// <returns>
        /// An NSArray of GKPlayer. Players from the friends list or players that recently participated in a game with the local player.
        ///</returns>
        public Task<NSArray<GKPlayer>> LoadRecentPlayers()
        {
            var tcs = InteropTasks.Create<NSArray<GKPlayer>>(out var taskId);
            GKLocalPlayer_LoadRecentPlayers(Pointer, taskId, OnLoadRecentPlayers, OnLoadRecentPlayersError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadRecentPlayers(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, (NSArray<GKPlayer>)PointerCast<NSArrayGKPlayer>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadRecentPlayersError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKPlayer>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
    }
}