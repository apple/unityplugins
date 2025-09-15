using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;
using Apple.GameKit.Leaderboards;
using UnityEngine;

namespace Apple.GameKit
{
    /// <symbol>c:objc(cs)GKGameActivityDefinition</symbol>
    [Introduced(iOS: "26.0.0", macOS: "26.0.0", tvOS: "26.0.0", visionOS: "26.0.0")]
    public class GKGameActivityDefinition : NSObject
    {
        internal GKGameActivityDefinition(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// Default properties defined by the developer for this type of game activity.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(py)defaultProperties</symbol>
        public NSDictionary<NSString, NSString> DefaultProperties => PointerCast<NSDictionary<NSString, NSString>>(Interop.GKGameActivityDefinition_GetDefaultProperties(Pointer));

        /// <summary>
        /// A more detailed description of the game activity.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(py)details</symbol>
        public string Details => Interop.GKGameActivityDefinition_Details(Pointer);

        /// <summary>
        /// A fallback URL that can be used to construct a game-specific URL for players to share or join, if the joining device does not support the default URL.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(py)fallbackURL</symbol>
        public Uri FallbackURL
        {
            get
            {
                var urlString = Interop.GKGameActivityDefinition_GetFallbackURL(Pointer);
                return urlString != null ? new Uri(urlString) : null;
            }
        }

        /// <summary>
        /// The group identifier for the activity, if one exists.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(py)groupIdentifier</symbol>
        public string GroupIdentifier => Interop.GKGameActivityDefinition_GetGroupIdentifier(Pointer);

        /// <summary>
        /// The developer defined identifier for a given game activity.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(py)identifier</symbol>
        public string Identifier => Interop.GKGameActivityDefinition_GetIdentifier(Pointer);

        /// <summary>
        /// Loads all associated achievements that have defined deep links to this game activity definition.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(im)loadAchievementDescriptionsWithCompletionHandler:</symbol>
        public Task<NSArray<GKAchievementDescription>> LoadAchievementDescriptions()
        {
            var tcs = InteropTasks.Create<NSArray<GKAchievementDescription>>(out var taskId);
            Interop.GKGameActivityDefinition_LoadAchievementDescriptions(Pointer, taskId, OnLoadAchievementDescriptions, OnLoadAchievementDescriptionsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadAchievementDescriptions(long taskId, IntPtr nsArrayPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKAchievementDescription>>(nsArrayPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadAchievementDescriptionsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKAchievementDescription>>(taskId, new GameKitException(errorPointer));
        }

        /// <summary>
        /// Loads all the game activity definitions for the current game.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(cm)loadGameActivityDefinitionsWithCompletionHandler:</symbol>
        public static Task<NSArray<GKGameActivityDefinition>> LoadGameActivityDefinitions()
        {
            var tcs = InteropTasks.Create<NSArray<GKGameActivityDefinition>>(out var taskId);
            Interop.GKGameActivityDefinition_LoadGameActivityDefinitions(taskId, OnLoadGameActivityDefinitions, OnLoadGameActivityDefinitionsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadGameActivityDefinitions(long taskId, IntPtr nsArrayPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKGameActivityDefinition>>(nsArrayPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadGameActivityDefinitionsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKGameActivityDefinition>>(taskId, new GameKitException(errorPointer));
        }

        /// <summary>
        /// Loads game activity definitions with the supplied App Store Connect identifiers.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(cm)loadGameActivityDefinitionsWithIDs:completionHandler:</symbol>
        public static Task<NSArray<GKGameActivityDefinition>> LoadGameActivityDefinitionsWithIDs(NSArray<NSString> activityDefinitionIDs)
        {
            var tcs = InteropTasks.Create<NSArray<GKGameActivityDefinition>>(out var taskId);
            Interop.GKGameActivityDefinition_LoadGameActivityDefinitionsWithIDs(taskId, activityDefinitionIDs.Pointer, OnLoadGameActivityDefinitionsWithIDs, OnLoadGameActivityDefinitionsWithIDsError);
            return tcs.Task;
        }
        public static Task<NSArray<GKGameActivityDefinition>> LoadGameActivityDefinitionsWithIDs(params string[] activityDefinitionIDs) =>
            LoadGameActivityDefinitionsWithIDs(new NSMutableArray<NSString>(activityDefinitionIDs.Select(id => new NSString(id))));

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadGameActivityDefinitionsWithIDs(long taskId, IntPtr nsArrayPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKGameActivityDefinition>>(nsArrayPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadGameActivityDefinitionsWithIDsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKGameActivityDefinition>>(taskId, new GameKitException(errorPointer));
        }

        /// <summary>
        /// Asynchronously load the image. Error will be nil on success.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(im)loadImageWithCompletionHandler:</symbol>
        public Task<Texture2D> LoadImage()
        {
            var tcs = InteropTasks.Create<Texture2D>(out var taskId);
            Interop.GKGameActivityDefinition_LoadImage(Pointer, taskId, OnLoadImage, OnLoadImageError);
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

        /// <summary>
        /// Loads all associated leaderboards that have defined deep links to this game activity definition.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(im)loadLeaderboardsWithCompletionHandler:</symbol>
        public Task<NSArray<GKLeaderboard>> LoadLeaderboards()
        {
            var tcs = InteropTasks.Create<NSArray<GKLeaderboard>>(out var taskId);
            Interop.GKGameActivityDefinition_LoadLeaderboards(Pointer, taskId, OnLoadLeaderboards, OnLoadLeaderboardsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadLeaderboards(long taskId, IntPtr nsArrayPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKLeaderboard>>(nsArrayPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadLeaderboardsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKLeaderboard>>(taskId, new GameKitException(errorPointer));
        }

        /// <summary>
        /// The maximum number of participants that can join the activity. Returns nil when no maximum is set (unlimited players) or when player range is undefined. When not nil, the value is always greater than or equal to `minPlayers`.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(py)maxPlayers</symbol>
        public int? MaxPlayers
        {
            get
            {
                var nsNumber = PointerCast<NSNumber>(Interop.GKGameActivityDefinition_GetMaxPlayers(Pointer));
                return nsNumber?.IntValue;
            }
        }

        /// <summary>
        /// The minimum number of participants that can join the activity.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(py)minPlayers</symbol>
        public int? MinPlayers
        {
            get
            {
                var nsNumber = PointerCast<NSNumber>(Interop.GKGameActivityDefinition_GetMinPlayers(Pointer));
                return nsNumber?.IntValue;
            }
        }

        /// <summary>
        /// The play style of the game activity.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(py)playStyle</symbol>
        public GKGameActivityPlayStyle PlayStyle => Interop.GKGameActivityDefinition_GetPlayStyle(Pointer);

        /// <summary>
        /// The release state of the game activity definition in App Store Connect.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(py)releaseState</symbol>
        public GKReleaseState ReleaseState => Interop.GKGameActivityDefinition_GetReleaseState(Pointer);

        /// <summary>
        /// Whether the activity can be joined by others via a party code.
        /// - SeeAlso: ``-[GKGameActivityListener player:wantsToPlayGameActivity:completionHandler:]`` where you can receive and handle game activities that players want to play in a party with friends.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(py)supportsPartyCode</symbol>
        public bool SupportsPartyCode => Interop.GKGameActivityDefinition_GetSupportsPartyCode(Pointer);

        /// <summary>
        /// True if the activity supports an unlimited number of players. False if maxPlayers is set to a defined limit or if no player range is provided.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(py)supportsUnlimitedPlayers</symbol>
        public bool SupportsUnlimitedPlayers => Interop.GKGameActivityDefinition_GetSupportsUnlimitedPlayers(Pointer);

        /// <summary>
        /// A short title for the game activity.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivityDefinition(py)title</symbol>
        public string Title => Interop.GKGameActivityDefinition_GetTitle(Pointer);

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameActivityDefinition_GetDefaultProperties(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKGameActivityDefinition_Details(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKGameActivityDefinition_GetFallbackURL(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKGameActivityDefinition_GetGroupIdentifier(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKGameActivityDefinition_GetIdentifier(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivityDefinition_LoadAchievementDescriptions(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivityDefinition_LoadGameActivityDefinitions(long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivityDefinition_LoadGameActivityDefinitionsWithIDs(long taskId, IntPtr activityDefinitionIDsPtr, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivityDefinition_LoadImage(IntPtr pointer, long taskId, SuccessTaskImageCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivityDefinition_LoadLeaderboards(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameActivityDefinition_GetMaxPlayers(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameActivityDefinition_GetMinPlayers(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKGameActivityPlayStyle GKGameActivityDefinition_GetPlayStyle(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKReleaseState GKGameActivityDefinition_GetReleaseState(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKGameActivityDefinition_GetSupportsPartyCode(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKGameActivityDefinition_GetSupportsUnlimitedPlayers(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKGameActivityDefinition_GetTitle(IntPtr pointer);
        }
    }
}
