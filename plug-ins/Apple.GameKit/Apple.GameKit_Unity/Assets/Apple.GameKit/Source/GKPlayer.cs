using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace Apple.GameKit
{
    public delegate void SuccessTaskImageCallback(long taskId, IntPtr nsDataPtr);
    
    public class GKPlayer : NSObject
    {
        /// <summary>
        /// The size of a photo that Game Center loads.
        /// </summary>
        public enum PhotoSize : long
        {
            Small = 0,
            Normal = 1
        }

        [Preserve]
        internal GKPlayer(IntPtr pointer) : base(pointer) {}

        /// <summary>
        /// A unique identifier for a player of the game.
        /// </summary>
        public string GamePlayerId => Interop.GKPlayer_GetGamePlayerId(Pointer);

        /// <summary>
        /// A unique identifier for a player of the game.
        /// </summary>
        public string TeamPlayerId => Interop.GKPlayer_GetTeamPlayerId(Pointer);

        /// <summary>
        /// Returns a Boolean value depending on whether the game and Team ID for this player are persistent or unique to this game instance.
        /// </summary>
        public bool ScopedIdsArePersistent => Interop.GKPlayer_GetScopedIdsArePersistent(Pointer);

        /// <summary>
        /// A string the player chooses to identify themself to other players.
        /// </summary>
        public string Alias => Interop.GKPlayer_GetAlias(Pointer);

        /// <summary>
        /// A string to display for the player.
        /// </summary>
        public string DisplayName => Interop.GKPlayer_GetDisplayName(Pointer);

        /// <summary>
        /// A Boolean value that indicates whether the local player can send an invitation to the player.
        /// </summary>
        public bool IsInvitable => Interop.GKPlayer_GetIsInvitable(Pointer);

        /// <summary>
        /// A developer-created string that identifies a guest player.
        /// </summary>
        public string GuestIdentifier => Interop.GKPlayer_GetGuestIdentifier(Pointer);
        
        #region LoadPhoto
        /// <summary>
        /// Loads a photo of the player from Game Center.
        /// </summary>
        /// <param name="photoSize">A constant that determines the size of the photo to load./param>
        /// <returns></returns>
        public Task<Texture2D> LoadPhoto(PhotoSize photoSize)
        {
            var tcs = InteropTasks.Create<Texture2D>(out var taskId);
            Interop.GKPlayer_LoadPhoto(Pointer, taskId, photoSize, OnPhotoLoaded, OnPhotoError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskImageCallback))]
        private static void OnPhotoLoaded(long taskId, IntPtr nsDataPtr)
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
        private static void OnPhotoError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<Texture2D>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        /// <summary>
        /// Creates a guest player with the specified identifier.
        /// </summary>
        /// <param name="identifier">An identifier to use for the guest player.</param>
        /// <returns>A player object that represents the guest.</returns>
        public static GKPlayer AnonymousGuestPlayer(string identifier)
        {
            var pointer = Interop.GKPlayer_AnonymousGuestPlayer(identifier);
            return (pointer != IntPtr.Zero) ? new GKPlayer(pointer) : null;
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKPlayer_GetGamePlayerId(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKPlayer_GetTeamPlayerId(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKPlayer_GetScopedIdsArePersistent(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKPlayer_GetAlias(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKPlayer_GetDisplayName(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKPlayer_GetIsInvitable(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKPlayer_GetGuestIdentifier(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKPlayer_LoadPhoto(IntPtr pointer, long taskId, PhotoSize photoSize, SuccessTaskImageCallback onLoaded, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKPlayer_AnonymousGuestPlayer(string identifier);
        }
    }
}
