using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;
using UnityEngine;

namespace Apple.GameKit
{
    /// <summary>
    /// An object containing the text and artwork used to present an achievement to a player.
    /// </summary>
    public class GKAchievementDescription : InteropReference
    {
        #region Init & Dispose
        internal GKAchievementDescription(IntPtr pointer) : base(pointer)
        {
        }
        
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAchievementDescription_Free(IntPtr pointer);
        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                GKAchievementDescription_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }
        #endregion
        
        #region Identifier
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKAchievementDescription_GetIdentifier(IntPtr pointer);

        /// <summary>
        /// The string you enter in App Store Connect that uniquely identifies the achievement.
        /// </summary>
        public string Identifier
        {
            get => GKAchievementDescription_GetIdentifier(Pointer);
        }
        #endregion
        
        #region Title
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKAchievementDescription_GetTitle(IntPtr pointer);

        /// <summary>
        /// A localized title for the achievement
        /// </summary>
        public string Title
        {
            get => GKAchievementDescription_GetTitle(Pointer);
        }
        #endregion
        
        #region UnachievedDescription
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKAchievementDescription_GetUnachievedDescription(IntPtr pointer);

        /// <summary>
        /// A localized description of the achievement that you display when the player hasn't completed the achievement.
        /// </summary>
        public string UnachievedDescription
        {
            get => GKAchievementDescription_GetUnachievedDescription(Pointer);
        }
        #endregion
        
        #region AchievedDescription
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKAchievementDescription_GetAchievedDescription(IntPtr pointer);

        /// <summary>
        /// A localized description of the achievement that you display when the player completes the achievement.
        /// </summary>
        public string AchievedDescription
        {
            get => GKAchievementDescription_GetAchievedDescription(Pointer);
        }
        #endregion
        
        #region GroupIdentifier
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKAchievementDescription_GetGroupIdentifier(IntPtr pointer);

        /// <summary>
        /// The identifier for the group that the achievement description is part of.
        /// </summary>
        public string GroupIdentifier
        {
            get => GKAchievementDescription_GetGroupIdentifier(Pointer);
        }
        #endregion
        
        #region MaximumPoints
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKAchievementDescription_GetMaximumPoints(IntPtr pointer);

        /// <summary>
        /// The number of points that the player earns when completing the achievement.
        /// </summary>
        public long MaximumPoints
        {
            get => GKAchievementDescription_GetMaximumPoints(Pointer);
        }
        #endregion
        
        #region IsHidden
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKAchievementDescription_GetIsHidden(IntPtr pointer);

        /// <summary>
        /// A Boolean value that states whether the achievement is initially visible to players.
        /// </summary>
        public bool IsHidden
        {
            get => GKAchievementDescription_GetIsHidden(Pointer);
        }
        #endregion
        
        #region IsReplayable
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKAchievementDescription_GetIsReplayable(IntPtr pointer);

        /// <summary>
        /// A Boolean value that states whether the player can earn the achievement multiple times.
        /// </summary>
        public bool IsReplayable
        {
            get => GKAchievementDescription_GetIsReplayable(Pointer);
        }
        #endregion
        
        #region LoadAchievementDescriptions
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAchievementDescription_LoadAchievementDescriptions(long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Downloads the localized descriptions of achievements from Game Center.
        /// </summary>
        /// <returns>The GKAchievementDescription objects that contain the localized text for the achievements in your game. If an error occurs, this parameter may be non-nil, containing the descriptions that GameKit is able to download before the error.</returns>
        public static Task<NSArray<GKAchievementDescription>> LoadAchievementDescriptions()
        {
            var tcs = InteropTasks.Create<NSArray<GKAchievementDescription>>(out var taskId);
            GKAchievementDescription_LoadAchievementDescriptions(taskId, OnLoadAchievementDescriptions, OnLoadAchievementDescriptionsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadAchievementDescriptions(long taskId, IntPtr nsArrayPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, (NSArray<GKAchievementDescription>)PointerCast<NSArrayGKAchievementDescription>(nsArrayPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadAchievementDescriptionsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKAchievementDescription>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region LoadImage
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAchievementDescription_LoadImage(IntPtr pointer, SuccessTaskImageCallback onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Loads the image to display when the player completes the achievement.
        /// </summary>
        /// <returns>The image that represents the completed achievement.</returns>
        public Task<Texture2D> LoadImage()
        {
            var tcs = InteropTasks.Create<Texture2D>(out var taskId);
            GKAchievementDescription_LoadImage(Pointer, OnLoadImage, OnLoadImageError);
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