using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;
using UnityEngine;

namespace Apple.GameKit.Leaderboards
{
    public class GKLeaderboardSet : NSObject
    {
        internal GKLeaderboardSet(IntPtr pointer) : base(pointer) {}
        
        /// <summary>
        /// The localized title for the leaderboard set.
        /// </summary>
        public string Title => Interop.GKLeaderboardSet_GetTitle(Pointer);

        /// <summary>
        /// The identifier for the leaderboard set.
        /// </summary>
        public string Identifier => Interop.GKLeaderboardSet_GetIdentifier(Pointer);

        /// <summary>
        /// The identifier for the group that the leaderboard set belongs to.
        /// </summary>
        public string GroupIdentifier => Interop.GKLeaderboardSet_GetGroupIdentifier(Pointer);
        
        #region LoadLeaderboards

        /// <summary>
        /// Loads the leaderboards in the leaderboard set.
        /// </summary>
        /// <returns></returns>
        public Task<NSArray<GKLeaderboard>> LoadLeaderboards()
        {
            var tcs = InteropTasks.Create<NSArray<GKLeaderboard>>(out var taskId);
            Interop.GKLeaderboardSet_LoadLeaderboards(Pointer, taskId, OnLoadLeaderboards, OnLoadLeaderboardsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadLeaderboards(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKLeaderboard>>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadLeaderboardsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKLeaderboard>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region LoadLeaderboardSets

        /// <summary>
        /// Loads all of the leaderboard sets you configure for your game.
        /// </summary>
        /// <returns></returns>
        public static Task<NSArray<GKLeaderboardSet>> LoadLeaderboardSets()
        {
            var tcs = InteropTasks.Create<NSArray<GKLeaderboardSet>>(out var taskId);
            Interop.GKLeaderboardSet_LoadLeaderboardSets(taskId, OnLoadLeaderboardSets, OnLoadLeaderboardSetsError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadLeaderboardSets(long taskId, IntPtr pointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKLeaderboardSet>>(pointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadLeaderboardSetsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKLeaderboardSet>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region LoadImage
        
        /// <summary>
        /// Loads the image for the default leaderboard.
        /// </summary>
        /// <returns></returns>
        public Task<Texture2D> LoadImage()
        {
            var tcs = InteropTasks.Create<Texture2D>(out var taskId);
            Interop.GKLeaderboardSet_LoadImage(Pointer, taskId, OnLoadImage, OnLoadImageError);
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

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKLeaderboardSet_GetTitle(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKLeaderboardSet_GetIdentifier(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKLeaderboardSet_GetGroupIdentifier(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLeaderboardSet_LoadLeaderboards(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLeaderboardSet_LoadLeaderboardSets(long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLeaderboardSet_LoadImage(IntPtr pointer, long taskId, SuccessTaskImageCallback onSuccess, NSErrorTaskCallback onError);
        }
    }

}
