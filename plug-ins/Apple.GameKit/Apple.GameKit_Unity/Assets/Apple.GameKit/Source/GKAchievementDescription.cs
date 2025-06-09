using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;
using UnityEngine;

namespace Apple.GameKit
{
    /// <summary>
    /// An object containing the text and artwork used to present an achievement to a player.
    /// </summary>
    public class GKAchievementDescription : NSObject
    {
        internal GKAchievementDescription(IntPtr pointer) : base(pointer)
        {
        }
        
        /// <summary>
        /// The string you enter in App Store Connect that uniquely identifies the achievement.
        /// </summary>
        public string Identifier => Interop.GKAchievementDescription_GetIdentifier(Pointer);
        
        /// <summary>
        /// A localized title for the achievement
        /// </summary>
        public string Title => Interop.GKAchievementDescription_GetTitle(Pointer);
        
        /// <summary>
        /// A localized description of the achievement that you display when the player hasn't completed the achievement.
        /// </summary>
        public string UnachievedDescription => Interop.GKAchievementDescription_GetUnachievedDescription(Pointer);
        
        /// <summary>
        /// A localized description of the achievement that you display when the player completes the achievement.
        /// </summary>
        public string AchievedDescription => Interop.GKAchievementDescription_GetAchievedDescription(Pointer);
        
        /// <summary>
        /// The identifier for the group that the achievement description is part of.
        /// </summary>
        public string GroupIdentifier => Interop.GKAchievementDescription_GetGroupIdentifier(Pointer);
        
        /// <summary>
        /// The number of points that the player earns when completing the achievement.
        /// </summary>
        public long MaximumPoints => Interop.GKAchievementDescription_GetMaximumPoints(Pointer);
        
        /// <summary>
        /// A Boolean value that states whether the achievement is initially visible to players.
        /// </summary>
        public bool IsHidden => Interop.GKAchievementDescription_GetIsHidden(Pointer);
        
        /// <summary>
        /// A Boolean value that states whether the player can earn the achievement multiple times.
        /// </summary>
        public bool IsReplayable => Interop.GKAchievementDescription_GetIsReplayable(Pointer);

        /// <summary>
        /// If present, the rarity of the achievement expressed as a percentage of players that earned it. Null if not enough data is available to compute it.
        /// </summary>
        [Introduced(iOS: "17", macOS: "14", tvOS: "17")]
        public double RarityPercent => Interop.GKAchievementDescription_GetRarityPercent(Pointer);

        /// <summary>
        /// The identifier of the game activity associated with this achievement, as configured by the developer in App Store Connect.
        /// </summary>
        /// <symbol>c:objc(cs)GKAchievementDescription(py)activityIdentifier</symbol>
        [Introduced(iOS: "19.0.0", macOS: "16.0.0", tvOS: "19.0.0", visionOS: "3.0.0")]
        public string ActivityIdentifier => Interop.GKAchievementDescription_GetActivityIdentifier(Pointer);

        /// <summary>
        /// The properties when associating this achievement with a game activity, as configured by the developer in App Store Connect.
        /// </summary>
        /// <symbol>c:objc(cs)GKAchievementDescription(py)activityProperties</symbol>
        [Introduced(iOS: "19.0.0", macOS: "16.0.0", tvOS: "19.0.0", visionOS: "3.0.0")]
        public NSDictionary<NSString, NSString> ActivityProperties => PointerCast<NSDictionary<NSString, NSString>>(Interop.GKAchievementDescription_GetActivityProperties(Pointer));

        /// <summary>
        /// The release state of the achievement in App Store Connect.
        /// </summary>
        /// <symbol>c:objc(cs)GKAchievementDescription(py)releaseState</symbol>
        [Introduced(iOS: "18.4.0", macOS: "15.4.0", tvOS: "18.4.0", visionOS: "2.4.0")]
        public GKReleaseState ReleaseState => Interop.GKAchievementDescription_GetReleaseState(Pointer);

        #region LoadAchievementDescriptions

        /// <summary>
        /// Downloads the localized descriptions of achievements from Game Center.
        /// </summary>
        /// <returns>The GKAchievementDescription objects that contain the localized text for the achievements in your game. If an error occurs, this parameter may be non-nil, containing the descriptions that GameKit is able to download before the error.</returns>
        public static Task<NSArray<GKAchievementDescription>> LoadAchievementDescriptions()
        {
            var tcs = InteropTasks.Create<NSArray<GKAchievementDescription>>(out var taskId);
            Interop.GKAchievementDescription_LoadAchievementDescriptions(taskId, OnLoadAchievementDescriptions, OnLoadAchievementDescriptionsError);
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
        #endregion
        
        #region LoadImage

        /// <summary>
        /// Loads the image to display when the player completes the achievement.
        /// </summary>
        /// <returns>The image that represents the completed achievement.</returns>
        public Task<Texture2D> LoadImage()
        {
            var tcs = InteropTasks.Create<Texture2D>(out var taskId);
            Interop.GKAchievementDescription_LoadImage(Pointer, taskId, OnLoadImage, OnLoadImageError);
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

        /// <summary>
        /// A common image that you can display when the player hasn’t completed the achievement.
        /// </summary>
        /// <remarks>
        /// Note: Customization of this symbol image is not supported yet in Unity.
        /// </remarks>
        public static Texture2D IncompleteAchievementImage => Texture2DExtensions.CreateFromNSDataPtr(Interop.GKAchievementDescription_GetIncompleteAchievementImage());

        /// <summary>
        /// A placeholder image that you can display when the player completes the achievement.
        /// </summary>
        /// <remarks>
        /// Note: Customization of this symbol image is not supported yet in Unity.
        /// </remarks>
        public static Texture2D PlaceholderCompletedAchievementImage => Texture2DExtensions.CreateFromNSDataPtr(Interop.GKAchievementDescription_GetPlaceholderCompletedAchievementImage());

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKAchievementDescription_GetIdentifier(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKAchievementDescription_GetTitle(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKAchievementDescription_GetUnachievedDescription(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKAchievementDescription_GetAchievedDescription(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKAchievementDescription_GetGroupIdentifier(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern long GKAchievementDescription_GetMaximumPoints(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKAchievementDescription_GetIsHidden(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKAchievementDescription_GetIsReplayable(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKAchievementDescription_GetRarityPercent(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAchievementDescription_LoadAchievementDescriptions(long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAchievementDescription_LoadImage(IntPtr pointer, long taskId, SuccessTaskImageCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKAchievementDescription_GetIncompleteAchievementImage();
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKAchievementDescription_GetPlaceholderCompletedAchievementImage();
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKAchievementDescription_GetActivityIdentifier(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKAchievementDescription_GetActivityProperties(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKReleaseState GKAchievementDescription_GetReleaseState(IntPtr pointer);
        }
    }
}
