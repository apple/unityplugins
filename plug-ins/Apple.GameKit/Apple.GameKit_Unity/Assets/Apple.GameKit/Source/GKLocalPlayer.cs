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
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public bool IsPersonalizedCommunicationRestricted => Interop.GKLocalPlayer_GetIsPersonalizedCommunicationRestricted(Pointer);
        

        private static GKLocalPlayer _local;
        
        /// <summary>
        /// The shared instance of the local player.
        /// </summary>
        public static GKLocalPlayer Local => _local ??= new GKLocalPlayer(Interop.GKLocalPlayer_GetLocal());
        
        #region FetchItemsForIdentityVerificationSignature

        /// <summary>
        /// Generates a signature so that a third-party server can authenticate the local player.
        /// </summary>
        /// <returns></returns>
        [Introduced(iOS: "13.5", macOS: "10.15.5", tvOS: "13.4.8", visionOS: "1.0")]
        public Task<GKIdentityVerificationResponse> FetchItemsForIdentityVerificationSignature()
        {
            var tcs = InteropTasks.Create<GKIdentityVerificationResponse>(out var taskId);
            Interop.GKLocalPlayer_FetchItemsForIdentityVerificationSignature(Pointer, taskId, OnFetchItems, OnFetchItemsError);
            return tcs.Task;
        }
        public Task<GKIdentityVerificationResponse> FetchItems() => FetchItemsForIdentityVerificationSignature();

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

        // Events triggered by callbacks for authentication success or failure.

        /// <summary>
        /// Authentication success event.
        /// </summary>
        /// <param name="localPlayer"></param>
        public delegate void AuthenticateHandler(GKLocalPlayer localPlayer);
        public static event AuthenticateHandler AuthenticateUpdate;

        /// <summary>
        /// Authentication error event.
        /// </summary>
        /// <param name="error"></param>
        public delegate void AuthenticateErrorHandler(NSError error);
        public static event AuthenticateErrorHandler AuthenticateError;

        /// <summary>
        /// Assigns a handler that GameKit calls while authenticating the local player. The task will
        /// return when the user has been authenticated or failed via a GameKitException. Only one
        /// handler may be assigned. Subsequent calls will simply return the most recent result.
        /// Otherwise use the static GKLocalPlayer.Local property.
        /// </summary>
        /// <returns></returns>
        public static Task<GKLocalPlayer> Authenticate()
        {
            var tcs = InteropTasks.Create<GKLocalPlayer>(out var taskId);
            Interop.GKLocalPlayer_SetAuthenticateHandler(taskId, OnAuthenticate, OnAuthenticateError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnAuthenticate(long taskId, IntPtr pointer)
        {
            _local = PointerCast<GKLocalPlayer>(pointer);
            InteropTasks.TrySetResultAndRemove(taskId, _local);

            InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                AuthenticateUpdate?.Invoke(_local);
            });
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnAuthenticateError(long taskId, IntPtr errorPointer)
        {
            var ex = new GameKitException(errorPointer);
            InteropTasks.TrySetExceptionAndRemove<GKLocalPlayer>(taskId, ex);

            InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                AuthenticateError?.Invoke(ex.NSError);
            });
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
        [Introduced(iOS: "14.5", macOS: "11.3", tvOS: "14.5", visionOS: "1.0")]
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
        [Introduced(iOS: "14.5", macOS: "11.3", tvOS: "14.5", visionOS: "1.0")]
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
        [Introduced(iOS: "14.5", macOS: "11.3", tvOS: "14.5", visionOS: "1.0")]
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

        #region DefaultLeaderboardIdentifier

        /// <summary>
        /// Loads the identifier for the local player’s default leaderboard.
        /// </summary>
        /// <returns>
        /// The leaderboard ID for the local player’s default leaderboard that you set in App Store Connect.
        /// </returns>
        /// <symbol>c:objc(cs)GKLocalPlayer(im)loadDefaultLeaderboardIdentifierWithCompletionHandler:</symbol>
        public Task<string> LoadDefaultLeaderboardIdentifier()
        {
            var tcs = InteropTasks.Create<string>(out var taskId);
            Interop.GKLocalPlayer_LoadDefaultLeaderboardIdentifier(Pointer, taskId, OnLoadDefaultLeaderboardIdentifier, OnLoadDefaultLeaderboardIdentifierError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadDefaultLeaderboardIdentifier(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, (pointer != default) ? PointerCast<NSString>(pointer).ToString() : null);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadDefaultLeaderboardIdentifierError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<string>(taskId, new GameKitException(errorPointer));
        }

        /// <summary>
        /// Sets the local player’s default leaderboard.
        /// </summary>
        /// <param name="leaderboardIdentifier">The identifier of the leaderboard.</param>
        /// <symbol>c:objc(cs)GKLocalPlayer(im)setDefaultLeaderboardIdentifier:completionHandler:</symbol>
        public Task SetDefaultLeaderboardIdentifier(string leaderboardIdentifier)
        {
            var tcs = InteropTasks.Create<bool /*dummy*/>(out var taskId);
            var nsString = new NSString(leaderboardIdentifier ?? string.Empty);
            Interop.GKLocalPlayer_SetDefaultLeaderboardIdentifier(Pointer, taskId, nsString.Pointer, OnSetDefaultLeaderboardIdentifier, OnSetDefaultLeaderboardIdentifierError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnSetDefaultLeaderboardIdentifier(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnSetDefaultLeaderboardIdentifierError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<string>(taskId, new GameKitException(errorPointer));
        }

        #endregion

#if !UNITY_TVOS
        /// <summary>
        /// Saves game data with the specified name.
        /// </summary>
        /// <param name="data">An object that contains the saved game data.</param>
        /// <param name="name">A unique filename for the saved game data.</param>
        /// <returns>The saved game.</returns>
        /// <symbol>c:objc(cs)GKLocalPlayer(im)saveGameData:withName:completionHandler:</symbol>
        [Unavailable(RuntimeOperatingSystem.tvOS)]
        public Task<GKSavedGame> SaveGameData(NSData data, string name)
        {
            var tcs = InteropTasks.Create<GKSavedGame>(out var taskId);
            Interop.GKLocalPlayer_SaveGameData(Pointer, taskId, data.Pointer, name, OnSaveGameData, OnSaveGameDataError);
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

        /// <summary>
        /// Retrieves all available saved games.
        /// </summary>
        /// <returns>An array of saved games that GameKit fetches.</returns>
        /// <symbol>c:objc(cs)GKLocalPlayer(im)fetchSavedGamesWithCompletionHandler:</symbol>
        [Unavailable(RuntimeOperatingSystem.tvOS)]
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

        /// <summary>
        /// Replaces duplicate saved games that use the same filename with one file containing the specified game data.
        /// </summary>
        /// <param name="conflictingSavedGames">The saved games that contain the conflicts.</param>
        /// <param name="data">The correct game data to save.</param>
        /// <returns>
        /// The resolved saved games that you include in <paramref name="conflictingSavedGames"/>, and any other saved games GameKit finds with conflicts that you don’t include in <paramref name="conflictingSavedGames"/>.
        /// </returns>
        /// <symbol>c:objc(cs)GKLocalPlayer(im)resolveConflictingSavedGames:withData:completionHandler:</symbol>
        [Unavailable(RuntimeOperatingSystem.tvOS)]
        public Task<NSArray<GKSavedGame>> ResolveConflictingSavedGames(NSArray<GKSavedGame> conflictingSavedGames, NSData data)
        {
            var tcs = InteropTasks.Create<NSArray<GKSavedGame>>(out var taskId);
            Interop.GKLocalPlayer_ResolveConflictingSavedGames(Pointer, taskId, conflictingSavedGames.Pointer, data.Pointer, OnResolveConflictingSavedGames, OnResolveConflictingSavedGamesError);
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

        /// <summary>
        /// Deletes saved games with the specified filename.
        /// </summary>
        /// <param name="name">A string that identifies the saved game data to delete.</param>
        /// <symbol>c:objc(cs)GKLocalPlayer(im)deleteSavedGamesWithName:completionHandler:</symbol>
        [Unavailable(RuntimeOperatingSystem.tvOS)]
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
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }
#endif // !UNITY_TVOS

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
            public static extern void GKLocalPlayer_FetchItemsForIdentityVerificationSignature(IntPtr pointer, long taskId, InternalOnFetchItemsHandler onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_SetAuthenticateHandler(long taskId, SuccessTaskCallback<IntPtr> onAuthenticate, NSErrorTaskCallback onAuthenticateError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_LoadFriends(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_LoadFriendsWithIdentifiers(IntPtr gkLocalPlayerPtr, long taskId, IntPtr identifiersPtr, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_LoadFriendsAuthorizationStatus(IntPtr pointer, long taskId, SuccessTaskCallback<GKFriendsAuthorizationStatus> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_LoadChallengableFriends(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_LoadRecentPlayers(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_LoadDefaultLeaderboardIdentifier(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_SetDefaultLeaderboardIdentifier(IntPtr pointer, long taskId, IntPtr identifierPtr, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
#if !UNITY_TVOS
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_SaveGameData(IntPtr pointer, long taskId, IntPtr nsDataPtr, string name, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_FetchSavedGames(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_ResolveConflictingSavedGames(IntPtr pointer, long taskId, IntPtr conflictingSavedGamesPtr, IntPtr nsDataPtr, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLocalPlayer_DeleteSavedGames(IntPtr pointer, long taskId, string name, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
#endif // !UNITY_TVOS
        }
    }
}
