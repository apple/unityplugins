using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;
using Apple.GameKit.Players;

namespace Apple.GameKit
{
    public class GKLocalPlayer : GKPlayer
    {
        #region Delegates
        public delegate void SavedGameConflictingHandler(GKPlayer player, NSArray<GKSavedGame> conflicts);
        private delegate void InteropSavedGameConflictingHandler(IntPtr player, IntPtr conflicts);

        public delegate void SavedGameModifiedHandler(GKPlayer player, GKSavedGame modified);
        private delegate void InteropSavedGameModifiedHandler(IntPtr player, IntPtr modified);
        #endregion
        
        #region Static Events
        /// <summary>
        /// Chooses the correct game data from the saved games that contain conflicts.
        /// </summary>
        public static event SavedGameConflictingHandler SavedGameConflicting;

        /// <summary>
        /// Handles when data changes in a saved game file.
        /// </summary>
        public static event SavedGameModifiedHandler SavedGameModified;
        #endregion
        
        #region Static Event Registration
        static GKLocalPlayer()
        {
            if (Availability.Available(RuntimeOperatingSystem.macOS, 10, 10) ||
                Availability.Available(RuntimeOperatingSystem.iOS, 8, 0))
            {
                Interop.GKSavedGame_SetSavedGameConflictingCallback(OnSavedGameConflicting);
                Interop.GKSavedGame_SetSavedGameModifiedCallback(OnSavedGameModified);
            }
        }

        [MonoPInvokeCallback(typeof(InteropSavedGameConflictingHandler))]
        private static void OnSavedGameConflicting(IntPtr player, IntPtr conflicts)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => SavedGameConflicting?.Invoke(PointerCast<GKPlayer>(player), PointerCast<NSArray<GKSavedGame>>(conflicts)));
        }

        [MonoPInvokeCallback(typeof(InteropSavedGameModifiedHandler))]
        private static void OnSavedGameModified(IntPtr player, IntPtr modified)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => SavedGameModified?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKSavedGame>(modified)));
        }
        #endregion

        internal GKLocalPlayer(IntPtr pointer) : base(pointer) {}

        /// <summary>
        /// A Boolean value that indicates whether a local player has signed in to Game Center.
        /// </summary>
        public bool IsAuthenticated => Interop.GKLocalPlayer_GetIsAuthenticated(Pointer);

        /// <summary>
        /// A Boolean value that indicates whether the local player is underage.
        /// </summary>
        public bool IsUnderage => Interop.GKLocalPlayer_GetIsUnderage(Pointer);

        /// <summary>
        /// A Boolean value that indicates whether the player can join multiplayer games.
        /// </summary>
        public bool IsMultiplayerGamingRestricted => Interop.GKLocalPlayer_GetIsMultiplayerGamingRestricted(Pointer);

        /// <summary>
        /// A Boolean value that indicates whether the player can use personalized communication on the device.
        /// </summary>
        public bool IsPersonalizedCommunicationRestricted => Interop.GKLocalPlayer_GetIsPersonalizedCommunicationRestricted(Pointer);
        

        private static GKLocalPlayer _local;
        
        /// <summary>
        /// The shared instance of the local player.
        /// </summary>
        public static GKLocalPlayer Local => _local ??= new GKLocalPlayer(Interop.GKLocalPlayer_GetLocal());
        
        #region FetchItems

        /// <summary>
        /// Generates a signature so that a third-party server can authenticate the local player.
        /// </summary>
        /// <returns></returns>
        public Task<GKIdentityVerificationResponse> FetchItems()
        {
            var tcs = InteropTasks.Create<GKIdentityVerificationResponse>(out var taskId);
            Interop.GKLocalPlayer_FetchItems(Pointer, taskId, OnFetchItems, OnFetchItemsError);
            return tcs.Task;
        }

        internal delegate void InternalOnFetchItemsHandler(long taskId, string publicKeyUrl, IntPtr signaturePtr, IntPtr saltPtr, UInt64 timestamp);

        [MonoPInvokeCallback(typeof(InternalOnFetchItemsHandler))]
        private static void OnFetchItems(long taskId, string publicKeyUrl, IntPtr signaturePtr, IntPtr saltPtr, UInt64 timestamp)
        {
            try
            {
                var response = new GKIdentityVerificationResponse
                {
                    PublicKeyUrl = publicKeyUrl,
                    Signature = PointerCast<NSData>(signaturePtr),
                    Salt = PointerCast<NSData>(saltPtr),
                    Timestamp = timestamp
                };
                InteropTasks.TrySetResultAndRemove(taskId, response);
            }
            catch (Exception ex)
            {
                InteropTasks.TrySetExceptionAndRemove<GKIdentityVerificationResponse>(taskId, ex);
            }
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnFetchItemsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKIdentityVerificationResponse>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region Authenticate

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
            Interop.GKLocalPlayer_Authenticate(taskId, OnAuthenticate, OnAuthenticateError);
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
            Interop.GKLocalPlayer_LoadFriends(Pointer, taskId, OnLoadFriends, OnLoadFriendsError);
            return tcs.Task;
        }

        /// <summary>
        /// Loads the player’s friends list, scoped by the identifiers, if the player and their friends grant access.
        /// </summary>
        /// <param name="identifiers"></param>
        /// <returns>An NSArray of GKPlayer. The player's friends who also grant your game access to their friends.
        /// The local player and their friends authorization status must be GKFriendsAuthorizationStatus.authorized.
        /// The local player and their friends must use a version of your game with the same bundle ID.
        ///</returns>
        public Task<NSArray<GKPlayer>> LoadFriends(NSArray<NSString> identifiers)
        {
            var tcs = InteropTasks.Create<NSArray<GKPlayer>>(out var taskId);
            Interop.GKLocalPlayer_LoadFriendsWithIdentifiers(Pointer, taskId, identifiers.Pointer, OnLoadFriends, OnLoadFriendsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadFriends(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKPlayer>>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadFriendsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKPlayer>>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        #region LoadFriendsAuthorizationStatus

        /// <summary>
        /// Returns whether the player authorizes your game to access their friends list.
        /// </summary>
        public Task<GKFriendsAuthorizationStatus> LoadFriendsAuthorizationStatus()
        {
            var tcs = InteropTasks.Create<GKFriendsAuthorizationStatus>(out var taskId);
            Interop.GKLocalPlayer_LoadFriendsAuthorizationStatus(Pointer, taskId, OnLoadFriendsAuthorizationStatus, OnLoadFriendsAuthorizationStatusError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<GKFriendsAuthorizationStatus>))]
        private static void OnLoadFriendsAuthorizationStatus(long taskId, GKFriendsAuthorizationStatus authStatus)
        {
            InteropTasks.TrySetResultAndRemove(taskId, authStatus);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadFriendsAuthorizationStatusError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKFriendsAuthorizationStatus>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        #region LoadChallengableFriends

        /// <summary>
        /// Loads players to whom the local player can issue a challenge.
        /// </summary>
        /// <returns>
        /// An NSArray of GKPlayer. Players to whom the local player can issue a challenge. The local player can issue a challenge to a player with a friend level of FL1 or FL2.
        ///</returns>
        public Task<NSArray<GKPlayer>> LoadChallengeableFriends()
        {
            var tcs = InteropTasks.Create<NSArray<GKPlayer>>(out var taskId);
            Interop.GKLocalPlayer_LoadChallengableFriends(Pointer, taskId, OnLoadChallengableFriends, OnLoadChallengableFriendsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadChallengableFriends(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKPlayer>>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadChallengableFriendsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKPlayer>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region LoadRecentPlayers

        /// <summary>
        /// Loads players from the friends list or players that recently participated in a game with the local player.
        /// </summary>
        /// <returns>
        /// An NSArray of GKPlayer. Players from the friends list or players that recently participated in a game with the local player.
        ///</returns>
        public Task<NSArray<GKPlayer>> LoadRecentPlayers()
        {
            var tcs = InteropTasks.Create<NSArray<GKPlayer>>(out var taskId);
            Interop.GKLocalPlayer_LoadRecentPlayers(Pointer, taskId, OnLoadRecentPlayers, OnLoadRecentPlayersError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadRecentPlayers(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKPlayer>>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadRecentPlayersError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKPlayer>>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        #region SaveGameData

        /// <summary>
        /// Saves game data with the specified name.
        /// </summary>
        /// <returns>
        /// The saved game.
        ///</returns>
        public Task<GKSavedGame> SaveGameData(byte[] data, string name)
        {
            // Data...
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var interopData = new InteropData
            {
                DataPtr = handle.AddrOfPinnedObject(),
                DataLength = data.Length
            };

            var tcs = InteropTasks.Create<GKSavedGame>(out var taskId);
            Interop.GKLocalPlayer_SaveGameData(Pointer, taskId, interopData, name, OnSaveGameData, OnSaveGameDataError);

            handle.Free();
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnSaveGameData(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<GKSavedGame>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnSaveGameDataError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKSavedGame>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        #region FetchSavedGames

        /// <summary>
        /// Retrieves all available saved games.
        /// </summary>
        /// <returns>
        /// An array of saved games that GameKit fetches.
        ///</returns>
        public Task<NSArray<GKSavedGame>> FetchSavedGames()
        {
            var tcs = InteropTasks.Create<NSArray<GKSavedGame>>(out var taskId);
            Interop.GKLocalPlayer_FetchSavedGames(Pointer, taskId, OnFetchSavedGames, OnFetchSavedGamesError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnFetchSavedGames(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKSavedGame>>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnFetchSavedGamesError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKSavedGame>>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        #region ResolveConflictingSavedGames

        /// <summary>
        /// Replaces duplicate saved games that use the same filename with one file containing the specified game data.
        /// </summary>
        /// <returns>
        /// The resolved saved games that you include in conflictingSavedGames, and any other saved games GameKit
        /// finds with conflicts that you don’t include in conflictingSavedGames.
        ///
        /// For example, if there are five saved game files with the same filename, but only three are in conflictingSavedGames,
        /// this parameter contains the three saved games you provide and the two saved games GameKit finds.
        ///</returns>
        public Task<NSArray<GKSavedGame>> ResolveConflictingSavedGames(NSArray<GKSavedGame> conflictingSavedGames, byte[] data)
        {
            // Data...
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var interopData = new InteropData
            {
                DataPtr = handle.AddrOfPinnedObject(),
                DataLength = data.Length
            };

            var tcs = InteropTasks.Create<NSArray<GKSavedGame>>(out var taskId);
            Interop.GKLocalPlayer_ResolveConflictingSavedGames(Pointer, taskId, conflictingSavedGames.Pointer, interopData, OnResolveConflictingSavedGames, OnResolveConflictingSavedGamesError);

            handle.Free();
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnResolveConflictingSavedGames(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKSavedGame>>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnResolveConflictingSavedGamesError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKSavedGame>>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        #region DeleteSavedGames

        /// <summary>
        /// Deletes saved games with the specified filename.
        /// </summary>
        /// <returns></returns>
        public Task DeleteSavedGames(string name)
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKLocalPlayer_DeleteSavedGames(Pointer, taskId, name, OnDeleteSavedGames, OnDeleteSavedGamesError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnDeleteSavedGames(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnDeleteSavedGamesError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKSavedGame>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKLocalPlayer_GetIsAuthenticated(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKLocalPlayer_GetIsUnderage(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKLocalPlayer_GetIsMultiplayerGamingRestricted(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKLocalPlayer_GetIsPersonalizedCommunicationRestricted(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKLocalPlayer_GetLocal();
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_FetchItems(IntPtr pointer, long taskId, InternalOnFetchItemsHandler onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_Authenticate(long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_LoadFriends(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onCallback, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_LoadFriendsWithIdentifiers(IntPtr gkLocalPlayerPtr, long taskId, IntPtr identifiersPtr, SuccessTaskCallback<IntPtr> onCallback, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_LoadFriendsAuthorizationStatus(IntPtr pointer, long taskId, SuccessTaskCallback<GKFriendsAuthorizationStatus> onCallback, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_LoadChallengableFriends(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onCallback, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_LoadRecentPlayers(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onCallback, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKSavedGame_SetSavedGameConflictingCallback(InteropSavedGameConflictingHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKSavedGame_SetSavedGameModifiedCallback(InteropSavedGameModifiedHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_SaveGameData(IntPtr gkLocalPlayerPtr, long taskId, InteropData data, string name, SuccessTaskCallback<IntPtr> onCallback, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_FetchSavedGames(IntPtr gkLocalPlayerPtr, long taskId, SuccessTaskCallback<IntPtr> onCallback, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_ResolveConflictingSavedGames(IntPtr gkLocalPlayerPtr, long taskId, IntPtr savedGames, InteropData data, SuccessTaskCallback<IntPtr> onCallback, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_DeleteSavedGames(IntPtr gkLocalPlayerPtr, long taskId, string name, SuccessTaskCallback onCallback, NSErrorTaskCallback onError);
        }
    }
}
