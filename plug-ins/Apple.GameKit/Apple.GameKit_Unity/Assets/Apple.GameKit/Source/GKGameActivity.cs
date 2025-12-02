#define IOS_19_BETA_1_WANTSTOPLAY_MAIN_THREAD_WORKAROUND

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;
using Apple.GameKit.Leaderboards;
using Apple.GameKit.Multiplayer;
using UnityEngine.Scripting;

namespace Apple.GameKit
{
    /// <summary>
    /// `GKGameActivity` represents a single instance of a game activity for the current game.
    /// </summary>
    /// <symbol>c:objc(cs)GKGameActivity</symbol>
    [Introduced(iOS: "26.0.0", macOS: "26.0.0", tvOS: "26.0.0", visionOS: "26.0.0")]
    public class GKGameActivity : NSObject
    {
        public delegate Task<bool> WantsToPlayHandler(GKPlayer player, GKGameActivity activity);
        private delegate void InteropWantsToPlayHandler(IntPtr player, IntPtr activity, IntPtr context);

        /// <summary>
        /// Called when a player intends to play for a specific game activity.
        /// </summary>
        /// <remarks>
        /// Register for this event before calling GKLocalPlayer.Authenticate() to avoid missing events.
        /// </remarks>
        public static event WantsToPlayHandler WantsToPlay;

        static GKGameActivity()
        {
            Interop.GKGameActivity_SetWantsToPlayCallback(OnWantsToPlay);
        }

#if IOS_19_BETA_1_WANTSTOPLAY_MAIN_THREAD_WORKAROUND
        // In iOS 19 beta 1, the wantsToPlay callback isn't dispatched on the main thread.
        // To work around this, the callback is 

        private static System.Threading.SynchronizationContext _mainThreadContext = null;
        private static int _mainThreadId = 0;

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void OnStartup()
        {
            _mainThreadContext = System.Threading.SynchronizationContext.Current;
            _mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        private static bool IsMainThread => _mainThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif // IOS_19_BETA_1_WANTSTOPLAY_MAIN_THREAD_WORKAROUND

        [MonoPInvokeCallback(typeof(InteropWantsToPlayHandler))]
        private static void OnWantsToPlay(IntPtr player, IntPtr activity, IntPtr context)
        {
#if IOS_19_BETA_1_WANTSTOPLAY_MAIN_THREAD_WORKAROUND
            _mainThreadContext.Post(_ =>
            {
#endif // IOS_19_BETA_1_WANTSTOPLAY_MAIN_THREAD_WORKAROUND

                InteropPInvokeExceptionHandler.CatchAndLog(async () =>
                {
                    bool result = false;
                    if (WantsToPlay != null)
                    {
                        result = await WantsToPlay.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKGameActivity>(activity));
                    }

                    Interop.GKGameActivity_WantsToPlayCallbackCompletionHandler_Invoke(context, result);
                });

#if IOS_19_BETA_1_WANTSTOPLAY_MAIN_THREAD_WORKAROUND
            }, null);
#endif // IOS_19_BETA_1_WANTSTOPLAY_MAIN_THREAD_WORKAROUND

        }

        [Preserve]
        internal GKGameActivity(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// All achievements that have been associated with this activity.
        /// 
        /// Progress of each achievement will be reported when the activity ends.
        /// </summary>
        /// <remarks>
        /// Results returned in NSArray rather than NSSet.
        /// </remarks>
        /// <symbol>c:objc(cs)GKGameActivity(py)achievements</symbol>
        public NSArray<GKAchievement> AchievementsAsArray => PointerCast<NSArray<GKAchievement>>(Interop.GKGameActivity_GetAchievementsAsArray(Pointer));

        /// <summary>
        /// The activity definition that this activity instance is based on.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(py)activityDefinition</symbol>
        public GKGameActivityDefinition ActivityDefinition => PointerCast<GKGameActivityDefinition>(Interop.GKGameActivity_GetActivityDefinition(Pointer));

        /// <summary>
        /// Checks whether there is a pending activity to handle for the current game.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(cm)checkPendingGameActivityExistenceWithCompletionHandler:</symbol>
        public static Task<bool> CheckPendingGameActivityExistence()
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKGameActivity_CheckPendingGameActivityExistence(taskId, OnCheckPendingGameActivityExistence, OnCheckPendingGameActivityExistenceError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<bool>))]
        private static void OnCheckPendingGameActivityExistence(long taskId, bool hasActiveChallenges)
        {
            InteropTasks.TrySetResultAndRemove(taskId, hasActiveChallenges);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnCheckPendingGameActivityExistenceError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }

        /// <summary>
        /// The date when the activity was created.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(py)creationDate</symbol>
        public DateTimeOffset CreationDate => DateTimeOffsetExtensions.FromUnixTimeSeconds(Interop.GKGameActivity_GetCreationDate(Pointer));

        /// <summary>
        /// Total time elapsed while in active state.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(py)duration</symbol>
        public TimeSpan Duration => TimeSpan.FromSeconds(Interop.GKGameActivity_GetDuration(Pointer));

        /// <summary>
        /// Ends the game activity if it is not already ended.
        /// 
        /// This will report all associated achievements and submit scores to leaderboards.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)end</symbol>
        public void End() => Interop.GKGameActivity_End(Pointer);

        /// <summary>
        /// The date when the activity was officially ended.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(py)endDate</symbol>
        public DateTimeOffset EndDate => DateTimeOffsetExtensions.FromUnixTimeSeconds(Interop.GKGameActivity_GetEndDate(Pointer));

        /// <summary>
        /// Use information from the activity to find matches for the local player.
        /// 
        /// GameKit will create a classic match making request with the activity's party code and other information, and return the match object in the completion handler or any error that occurred.
        /// Error occurs if this activity doesn't support party code, or has unsupported range of players, which is used to be configured as match request's minPlayers and maxPlayers.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)findMatchWithCompletionHandler:</symbol>
        public Task<GKMatch> FindMatch()
        {
            var tcs = InteropTasks.Create<GKMatch>(out var taskId);
            Interop.GKGameActivity_FindMatch(Pointer, taskId, OnFindMatch, OnFindMatchError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnFindMatch(long taskId, IntPtr matchPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<GKMatch>(matchPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnFindMatchError(long taskId, IntPtr errorPtr)
        {
            InteropTasks.TrySetExceptionAndRemove<GKMatch>(taskId, new GameKitException(errorPtr));
        }

        /// <summary>
        /// Use information from the activity to find server hosted players for the local player.
        /// 
        /// GameKit will create a classic server hosted match making request with the activity's party code and other information, and return the players in the completion handler or any error that occurred.
        /// Error occurs if this activity doesn't support party code, or has unsupported range of players, which is used to be configured as match request's minPlayers and maxPlayers.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)findPlayersForHostedMatchWithCompletionHandler:</symbol>
        public Task<NSArray<GKPlayer>> FindPlayersForHostedMatch()
        {
            var tcs = InteropTasks.Create<NSArray<GKPlayer>>(out var taskId);
            Interop.GKGameActivity_FindPlayersForHostedMatch(Pointer, taskId, OnFindPlayersForHostedMatch, OnFindPlayersForHostedMatchError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnFindPlayersForHostedMatch(long taskId, IntPtr nsArrayPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKPlayer>>(nsArrayPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnFindPlayersForHostedMatchError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKPlayer>>(taskId, new GameKitException(errorPointer));
        }

        /// <summary>
        /// Get the achievement progress from a specific achievement of the local player if previously set.
        /// 
        /// Returns 0 if the achievement has not been set in the current activity.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)getProgressOnAchievement:</symbol>
        public double GetProgressOnAchievement(GKAchievement achievement) => Interop.GKGameActivity_GetProgressOnAchievement(Pointer, achievement.Pointer);

        /// <summary>
        /// Get the leaderboard score from a specific leaderboard of the local player if previously set.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)getScoreOnLeaderboard:</symbol>
        public GKLeaderboardScore GetScoreOnLeaderboard(GKLeaderboard leaderboard) => PointerCast<GKLeaderboardScore>(Interop.GKGameActivity_GetScoreOnLeaderboard(Pointer, leaderboard.Pointer));

        /// <summary>
        /// The identifier of this activity instance.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(py)identifier</symbol>
        public string Identifier => Interop.GKGameActivity_GetIdentifier(Pointer);

        /// <summary>
        /// Initializes a game activity with definition.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)initWithDefinition:</symbol>
        public static GKGameActivity Init(GKGameActivityDefinition activityDefinition) => PointerCast<GKGameActivity>(Interop.GKGameActivity_InitWithDefinition(activityDefinition.Pointer));

        /// <summary>
        /// Checks whether a party code is in valid format.
        /// 
        /// Party code should be two parts of strings with the same length (2-6) connected with a dash, and the code can be either pure digits (0-9), or both parts are uppercased characters from `validPartyCodeAlphabet`.
        /// - SeeAlso: `validPartyCodeAlphabet` for allowed characters.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(cm)isValidPartyCode:</symbol>
        public static bool IsValidPartyCode(string partyCode) => Interop.GKGameActivity_IsValidPartyCode(partyCode);

        /// <summary>
        /// The date when the activity was last resumed.
        /// 
        /// - If the activity was first started, this will be the same as the start date.
        /// - If the activity was paused and resumed, this will be the date when the activity was resumed.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(py)lastResumeDate</symbol>
        public DateTimeOffset LastResumeDate => DateTimeOffsetExtensions.FromUnixTimeSeconds(Interop.GKGameActivity_LastResumeDate(Pointer));

        /// <summary>
        /// All leaderboard scores that have been associated with this activity.
        /// 
        /// Scores will be submitted to the leaderboards when the activity ends.
        /// </summary>
        /// <remarks>
        /// Results returned in NSArray rather than NSSet.
        /// </remarks>
        /// <symbol>c:objc(cs)GKGameActivity(py)leaderboardScores</symbol>
        public NSArray<GKLeaderboardScore> LeaderboardScoresAsArray => PointerCast<NSArray<GKLeaderboardScore>>(Interop.GKGameActivity_GetLeaderboardScoresAsArray(Pointer));

        /// <summary>
        /// Makes a `GKMatchRequest` object with information from the activity, which can be used to find matches for the local player.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)makeMatchRequest</symbol>
        public GKMatchRequest MakeMatchRequest() => PointerCast<GKMatchRequest>(Interop.GKGameActivity_MakeMatchRequest(Pointer));

        /// <summary>
        /// If the game supports party code, this is the party code that can be shared among players to join the party.
        /// 
        /// If the game does not support party code, this value will be nil.
        /// - SeeAlso: ``-[GKGameActivity startWithDefinition:partyCode:completionHandler:]`` for creating a game activity with a custom party code.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(py)partyCode</symbol>
        public string PartyCode => Interop.GKGameActivity_GetPartyCode(Pointer);

        /// <summary>
        /// If the game supports party code, this is the URL that can be shared among players to join the party.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(py)partyURL</symbol>
        public Uri PartyURL
        {
            get
            {
                var urlString = Interop.GKGameActivity_GetPartyURL(Pointer);
                return urlString != null ? new Uri(urlString) : null;
            }
        }

        /// <summary>
        /// Pauses the game activity if it is not already paused.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)pause</symbol>
        public void Pause() => Interop.GKGameActivity_Pause(Pointer);

        /// <summary>
        /// Properties that contain additional information about the activity.
        /// 
        /// This takes precedence over the `defaultProperties` on the `activityDefinition`.
        /// 
        /// 1. This dictionary is initialized with the default properties from the activity definition and deep linked properties if any.
        /// 2. If deep linking contains the same key as the default properties, the deep linked value will override the default value.
        /// 3. The properties can be updated at runtime.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(py)properties</symbol>
        public NSDictionary<NSString, NSString> Properties
        {
            get => PointerCast<NSDictionary<NSString, NSString>>(Interop.GKGameActivity_GetProperties(Pointer));
            set => Interop.GKGameActivity_SetProperties(Pointer, value?.Pointer ?? IntPtr.Zero);
        }

        /// <summary>
        /// Removes all achievements if exist.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)removeAchievements:</symbol>
        public void RemoveAchievements(NSArray<GKAchievement> achievements) =>
            Interop.GKGameActivity_RemoveAchievements(Pointer, achievements.Pointer);
        public void RemoveAchievements(params GKAchievement[] achievements) =>
            RemoveAchievements(new NSMutableArray<GKAchievement>(achievements)); 

        /// <summary>
        /// Removes all scores from leaderboards for a player if exist.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)removeScoresFromLeaderboards:</symbol>
        public void RemoveScoresFromLeaderboards(NSArray<GKLeaderboard> leaderboards) =>
            Interop.GKGameActivity_RemoveScoresFromLeaderboards(Pointer, leaderboards.Pointer);
        public void RemoveScoresFromLeaderboards(params GKLeaderboard[] leaderboards) =>
            RemoveScoresFromLeaderboards(new NSMutableArray<GKLeaderboard>(leaderboards));

        /// <summary>
        /// Resumes the game activity if it was paused.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)resume</symbol>
        public void Resume() => Interop.GKGameActivity_Resume(Pointer);

        /// <summary>
        /// Convenience method to set a progress to 100% for an achievement for a player.
        /// 
        /// Achievement completion will be reported when the activity ends.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)setAchievementCompleted:</symbol>
        public void SetAchievementCompleted(GKAchievement achievement) => Interop.GKGameActivity_SetAchievementCompleted(Pointer, achievement.Pointer);

        /// <summary>
        /// Set a progress for an achievement for a player.
        /// 
        /// Achievement progress will be reported when the activity ends.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)setProgressOnAchievement:toPercentComplete:</symbol>
        public void SetProgressOnAchievement(GKAchievement achievement, double percentComplete) =>
            Interop.GKGameActivity_SetProgressOnAchievement(Pointer, achievement.Pointer, percentComplete);

        /// <summary>
        /// Set a score of a leaderboard for a player.
        /// 
        /// The score will be submitted to the leaderboard when the activity ends.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)setScoreOnLeaderboard:toScore:</symbol>
        public void SetScoreOnLeaderboard(GKLeaderboard leaderboard, long score) =>
            Interop.GKGameActivity_SetScoreOnLoaderboard(Pointer, leaderboard.Pointer, score);

        /// <summary>
        /// Set a score of a leaderboard with a context for a player.
        /// 
        /// The score will be submitted to the leaderboard when the activity ends.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)setScoreOnLeaderboard:toScore:context:</symbol>
        public void SetScoreOnLeaderboard(GKLeaderboard leaderboard, long score, ulong context) =>
            Interop.GKGameActivity_SetScoreOnLoaderboardWithContext(Pointer, leaderboard.Pointer, score, context);

        /// <summary>
        /// Starts the game activity if it is not already started.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(im)start</symbol>
        public void Start() => Interop.GKGameActivity_Start(Pointer);

        /// <summary>
        /// The date when the activity was initially started.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(py)startDate</symbol>
        public DateTimeOffset StartDate => DateTimeOffsetExtensions.FromUnixTimeSeconds(Interop.GKGameActivity_GetStartDate(Pointer));

        /// <summary>
        /// Initializes and starts a game activity with definition.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(cm)startWithDefinition:error:</symbol>
        public static GKGameActivity Start(GKGameActivityDefinition activityDefinition) =>
            PointerCast<GKGameActivity>(Interop.GKGameActivity_StartWithDefinition(activityDefinition.Pointer));

        /// <summary>
        /// Creates and starts a new game activity with a custom party code.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(cm)startWithDefinition:partyCode:error:</symbol>
        public static GKGameActivity Start(GKGameActivityDefinition activityDefinition, string partyCode) =>
            PointerCast<GKGameActivity>(Interop.GKGameActivity_StartWithDefinitionAndPartyCode(activityDefinition.Pointer, partyCode));

        /// <summary>
        /// The state of the game activity.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(py)state</symbol>
        public GKGameActivityState State => Interop.GKGameActivity_GetState(Pointer);

        /// <summary>
        /// Allowed characters for the party code to be used to share this activity.
        /// </summary>
        /// <symbol>c:objc(cs)GKGameActivity(cpy)validPartyCodeAlphabet</symbol>
        public static NSArray<NSString> ValidPartyCodeAlphabet => PointerCast<NSArray<NSString>>(Interop.GKGameActivity_GetValidPartyCodeAlphabet());

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_SetWantsToPlayCallback(InteropWantsToPlayHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_WantsToPlayCallbackCompletionHandler_Invoke(IntPtr context, bool result);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameActivity_GetAchievementsAsArray(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameActivity_GetActivityDefinition(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_CheckPendingGameActivityExistence(long taskId, SuccessTaskCallback<bool> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKGameActivity_GetCreationDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKGameActivity_GetDuration(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_End(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKGameActivity_GetEndDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_FindMatch(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_FindPlayersForHostedMatch(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKGameActivity_GetProgressOnAchievement(IntPtr pointer, IntPtr achievementPtr);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameActivity_GetScoreOnLeaderboard(IntPtr pointer, IntPtr leaderboardPtr);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKGameActivity_GetIdentifier(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameActivity_InitWithDefinition(IntPtr activityDefinitionPtr);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKGameActivity_IsValidPartyCode(string partyCode);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKGameActivity_LastResumeDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameActivity_GetLeaderboardScoresAsArray(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameActivity_MakeMatchRequest(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKGameActivity_GetPartyCode(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKGameActivity_GetPartyURL(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_Pause(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameActivity_GetProperties(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_SetProperties(IntPtr pointer, IntPtr dictionaryPtr);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_RemoveAchievements(IntPtr pointer, IntPtr nsArrayAchievementsPtr);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_RemoveScoresFromLeaderboards(IntPtr pointer, IntPtr nsArrayLeaderboardsPtr);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_Resume(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_SetAchievementCompleted(IntPtr pointer, IntPtr achievementPtr);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_SetProgressOnAchievement(IntPtr pointer, IntPtr achievementPtr, double  percentComplete);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_SetScoreOnLoaderboard(IntPtr pointer, IntPtr leaderboardPtr, long score);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_SetScoreOnLoaderboardWithContext(IntPtr pointer, IntPtr leaderboardPtr, long score, ulong context);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKGameActivity_Start(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKGameActivity_GetStartDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameActivity_StartWithDefinition(IntPtr activityDefinitionPtr);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameActivity_StartWithDefinitionAndPartyCode(IntPtr activityDefinitionPtr, string partyCode);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKGameActivityState GKGameActivity_GetState(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKGameActivity_GetValidPartyCodeAlphabet();
        }
    }
}
