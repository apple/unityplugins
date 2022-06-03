using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;
using Apple.GameKit.Players;
using UnityEngine;

namespace Apple.GameKit
{
    public delegate void SuccessTaskImageCallback(long taskId, int width, int height, IntPtr data, int dataLength);
    
    public class GKPlayer : InteropReference
    {
        /// <summary>
        /// The size of a photo that Game Center loads.
        /// </summary>
        public enum PhotoSize : long
        {
            Small = 0,
            Normal = 1
        }
        
        #region Init & Dispose
        internal GKPlayer(IntPtr pointer) : base(pointer) {}
        #endregion
        
        #region GamePlayerId
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKPlayer_GetGamePlayerId(IntPtr pointer);

        /// <summary>
        /// A unique identifier for a player of the game.
        /// </summary>
        public string GamePlayerId
        {
            get => GKPlayer_GetGamePlayerId(Pointer);
        }
        #endregion
        
        #region TeamPlayerId
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKPlayer_GetTeamPlayerId(IntPtr pointer);

        /// <summary>
        /// A unique identifier for a player of the game.
        /// </summary>
        public string TeamPlayerId
        {
            get => GKPlayer_GetTeamPlayerId(Pointer);
        }
        #endregion
        
        #region ScopedIdsArePersistent
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKPlayer_GetScopedIdsArePersistent(IntPtr pointer);

        /// <summary>
        /// Returns a Boolean value depending on whether the game and Team ID for this player are persistent or unique to this game instance.
        /// </summary>
        public bool ScopedIdsArePersistent
        {
            get => GKPlayer_GetScopedIdsArePersistent(Pointer);
        }
        #endregion
        
        #region Alias
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKPlayer_GetAlias(IntPtr pointer);

        /// <summary>
        /// A string the player chooses to identify themself to other players.
        /// </summary>
        public string Alias
        {
            get => GKPlayer_GetAlias(Pointer);
        }
        #endregion

        #region DisplayName
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKPlayer_GetDisplayName(IntPtr pointer);

        /// <summary>
        /// A string to display for the player.
        /// </summary>
        public string DisplayName
        {
            get => GKPlayer_GetDisplayName(Pointer);
        }
        #endregion
        
        #region IsInvitable
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKPlayer_GetIsInvitable(IntPtr pointer);

        /// <summary>
        /// A Boolean value that indicates whether the local player can send an invitation to the player.
        /// </summary>
        public bool IsInvitable
        {
            get => GKPlayer_GetIsInvitable(Pointer);
        }
        #endregion
        
        #region GuestIdentifier
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKPlayer_GetGuestIdentifier(IntPtr pointer);

        /// <summary>
        /// A developer-created string that identifies a guest player.
        /// </summary>
        public string GuestIdentifier
        {
            get => GKPlayer_GetGuestIdentifier(Pointer);
        }
        #endregion
        
        #region LoadPhoto
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKPlayer_LoadPhoto(IntPtr pointer, long taskId, PhotoSize photoSize, SuccessTaskImageCallback onLoaded, NSErrorTaskCallback onError);

        /// <summary>
        /// Loads a photo of the player from Game Center.
        /// </summary>
        /// <param name="photoSize">A constant that determines the size of the photo to load./param>
        /// <returns></returns>
        public Task<Texture2D> LoadPhoto(PhotoSize photoSize)
        {
            var tcs = InteropTasks.Create<Texture2D>(out var taskId);
            GKPlayer_LoadPhoto(Pointer, taskId, photoSize, OnPhotoLoaded, OnPhotoError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskImageCallback))]
        private static void OnPhotoLoaded(long taskId, int width, int height, IntPtr data, int dataLength)
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
        private static void OnPhotoError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<Texture2D>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region AnonymousGuestPlayer
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKPlayer_AnonymousGuestPlayer(string identifier);

        /// <summary>
        /// Creates a guest player with the specified identifier.
        /// </summary>
        /// <param name="identifier">An identifier to use for the guest player.</param>
        /// <returns>A player object that represents the guest.</returns>
        public static GKPlayer AnonymousGuestPlayer(string identifier)
        {
            var pointer = GKPlayer_AnonymousGuestPlayer(identifier);
            
            if(pointer != IntPtr.Zero)
                return new GKPlayer(pointer);

            return null;
        }
        #endregion
    }
}