using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;
using Apple.GameKit.Leaderboards;
using UnityEngine;
using UnityEngine.Scripting;

namespace Apple.GameKit
{
    /// <symbol>c:objc(cs)GKChallengeDefinition</symbol>
    [Introduced(iOS: "26.0.0", macOS: "26.0.0", tvOS: "26.0.0", visionOS: "26.0.0")]
    public class GKChallengeDefinition : NSObject
    {
        [Preserve]
        internal GKChallengeDefinition(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// A more detailed description of the challenge definition.
        /// </summary>
        /// <symbol>c:objc(cs)GKChallengeDefinition(py)details</symbol>
        public string Details => Interop.GKChallengeDefinition_GetDetails(Pointer);

        /// <summary>
        /// The duration options for the challenge, like `1 day` or `1 week`.
        ///  - Note: If set, the amount of weeks is stored in the `weekOfYear` field.
        /// - Important: The actual duration of the challenge may be dynamically adjusted
        ///              in order to accommodate different factors like players' timezones.
        /// </summary>
        /// <symbol>c:objc(cs)GKChallengeDefinition(py)durationOptions</symbol>
        public NSArray<NSDateComponents> DurationOptions => PointerCast<NSArray<NSDateComponents>>(Interop.GKChallengeDefinition_GetDurationOptions(Pointer));

        /// <summary>
        /// The group identifier for the challenge definition, if one exists.
        /// </summary>
        /// <symbol>c:objc(cs)GKChallengeDefinition(py)groupIdentifier</symbol>
        public string GroupIdentifier => Interop.GKChallengeDefinition_GetGroupIdentifier(Pointer);

        /// <summary>
        /// Indicates if this definition has active challenges associated with it.
        /// </summary>
        /// <symbol>c:objc(cs)GKChallengeDefinition(im)hasActiveChallengesWithCompletionHandler:</symbol>
        public Task<bool> HasActiveChallenges()
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKChallengeDefinition_HasActiveChallenges(Pointer, taskId, OnHasActiveChallenges, OnHasActiveChallengesError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<bool>))]
        private static void OnHasActiveChallenges(long taskId, bool hasActiveChallenges)
        {
            InteropTasks.TrySetResultAndRemove(taskId, hasActiveChallenges);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnHasActiveChallengesError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }

        /// <summary>
        /// The developer defined identifier for a given challenge definition.
        /// </summary>
        /// <symbol>c:objc(cs)GKChallengeDefinition(py)identifier</symbol>
        public string Identifier => Interop.GKChallengeDefinition_GetIdentifier(Pointer);

        /// <summary>
        /// Indicates if a challenge can be attempted more than once.
        /// </summary>
        /// <symbol>c:objc(cs)GKChallengeDefinition(py)isRepeatable</symbol>
        public bool IsRepeatable => Interop.GKChallengeDefinition_GetIsRepeatable(Pointer);

        /// <summary>
        /// Scores submitted to this leaderboard will also be submitted as scores in this challenge.
        /// </summary>
        /// <symbol>c:objc(cs)GKChallengeDefinition(py)leaderboard</symbol>
        public GKLeaderboard Leaderboard => PointerCast<GKLeaderboard>(Interop.GKChallengeDefinition_GetLeaderboard(Pointer));

        /// <summary>
        /// Loads all the challenge definitions for the current game, returns an empty array if none exist.
        /// - Important: Archived challenge definitions are excluded.
        /// </summary>
        /// <symbol>c:objc(cs)GKChallengeDefinition(cm)loadChallengeDefinitionsWithCompletionHandler:</symbol>
        public static Task<NSArray<GKChallengeDefinition>> LoadChallengeDefinitions()
        {
            var tcs = InteropTasks.Create<NSArray<GKChallengeDefinition>>(out var taskId);
            Interop.GKChallengeDefinition_LoadChallengeDefinitions(taskId, OnLoadChallengeDefinitions, OnLoadChallengeDefinitionsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadChallengeDefinitions(long taskId, IntPtr nsArrayPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKChallengeDefinition>>(nsArrayPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadChallengeDefinitionsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKChallengeDefinition>>(taskId, new GameKitException(errorPointer));
        }

        /// <summary>
        /// Loads the image set on the challenge definition, which may be `nil` if none was set.
        /// </summary>
        /// <symbol>c:objc(cs)GKChallengeDefinition(im)loadImageWithCompletionHandler:</symbol>
        public Task<Texture2D> LoadImage()
        {
            var tcs = InteropTasks.Create<Texture2D>(out var taskId);
            Interop.GKChallengeDefinition_LoadImage(Pointer, taskId, OnLoadImage, OnLoadImageError);
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
        /// The release state of the challenge definition in App Store Connect.
        /// </summary>
        /// <symbol>c:objc(cs)GKChallengeDefinition(py)releaseState</symbol>
        public GKReleaseState ReleaseState => Interop.GKChallengeDefinition_GetReleaseState(Pointer);

        /// <summary>
        /// A short title for the challenge definition.
        /// </summary>
        /// <symbol>c:objc(cs)GKChallengeDefinition(py)title</symbol>
        public string Title => Interop.GKChallengeDefinition_GetTitle(Pointer);

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKChallengeDefinition_GetDetails(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKChallengeDefinition_GetDurationOptions(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKChallengeDefinition_GetGroupIdentifier(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKChallengeDefinition_HasActiveChallenges(IntPtr pointer, long taskId, SuccessTaskCallback<bool> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKChallengeDefinition_GetIdentifier(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKChallengeDefinition_GetIsRepeatable(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKChallengeDefinition_GetLeaderboard(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKChallengeDefinition_LoadChallengeDefinitions(long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKChallengeDefinition_LoadImage(IntPtr pointer, long taskId, SuccessTaskImageCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKReleaseState GKChallengeDefinition_GetReleaseState(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKChallengeDefinition_GetTitle(IntPtr pointer);
        }
    }
}
