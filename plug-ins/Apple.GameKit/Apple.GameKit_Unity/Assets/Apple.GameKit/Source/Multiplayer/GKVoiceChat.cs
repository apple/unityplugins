using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// A voice channel that allows players to speak with each other in a multiplayer game.
    /// </summary>
    public class GKVoiceChat : InteropReference
    {
        private static readonly Dictionary<IntPtr, GKVoiceChat> _gkVoiceChats = new Dictionary<IntPtr, GKVoiceChat>();

        /// <summary>
        /// A method that handles when a player's voice chat changes state.
        /// </summary>
        public event PlayerVoiceChatStateDidChangeHandler PlayerVoiceChatStateDidChange;
        
        #region Init & Dispose
        internal GKVoiceChat(IntPtr pointer) : base(pointer)
        {
            _gkVoiceChats.Add(pointer, this);
            GKVoiceChat_PlayerVoiceChatStateDidChangeHandler(Pointer, OnPlayerVoiceChatStateDidChange);
        }

        [DllImport(InteropUtility.DLLName)]
        private static extern void GKVoiceChat_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                GKVoiceChat_Free(Pointer);
                _gkVoiceChats.Remove(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        #endregion
        
        #region IsVoIPAllowed
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKVoiceChat_GetIsVoIPAllowed();

        /// <summary>
        /// Returns whether voice chat is available on the device.
        /// </summary>
        public static bool IsVoIPAllowed
        {
            get => GKVoiceChat_GetIsVoIPAllowed();
        }
        #endregion
        
        #region Start
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKVoiceChat_Start(IntPtr pointer);

        /// <summary>
        /// Starts communication with other players in a channel.
        /// </summary>
        public void Start()
        {
            GKVoiceChat_Start(Pointer);
        }
        #endregion
        
        #region Stop
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKVoiceChat_Stop(IntPtr pointer);

        /// <summary>
        /// Ends communication with other players in a channe
        /// </summary>
        public void Stop()
        {
            GKVoiceChat_Stop(Pointer);
        }
        #endregion
        
        #region IsActive
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKVoiceChat_GetIsActive(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKVoiceChat_SetIsActive(IntPtr pointer, bool value);

        /// <summary>
        /// A Boolean value that indicates whether the channel is sampling the microphone.
        /// </summary>
        public bool IsActive
        {
            get => GKVoiceChat_GetIsActive(Pointer);
            set => GKVoiceChat_SetIsActive(Pointer, value);
        }
        #endregion
        
        #region PlayerVoiceChatStateDidChangeHandler
        public delegate void PlayerVoiceChatStateDidChangeHandler(GKPlayer player, PlayerState state);
        
        internal delegate void InternalPlayerVoiceChatStateDidChangeHandler(IntPtr pointer, IntPtr playerPtr, PlayerState state);

        [DllImport(InteropUtility.DLLName)]
        private static extern void GKVoiceChat_PlayerVoiceChatStateDidChangeHandler(IntPtr pointer, InternalPlayerVoiceChatStateDidChangeHandler onStateChanged);

        [MonoPInvokeCallback(typeof(InternalPlayerVoiceChatStateDidChangeHandler))]
        private static void OnPlayerVoiceChatStateDidChange(IntPtr pointer, IntPtr playerPtr, PlayerState state)
        {
            if (!_gkVoiceChats.TryGetValue(pointer, out var voiceChat))
                return;

            var player = playerPtr != IntPtr.Zero ? new GKPlayer(playerPtr) : null;
            voiceChat.PlayerVoiceChatStateDidChange?.Invoke(player, state);
        }
        #endregion
        
        #region SetPlayer
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKVoiceChat_SetPlayer(IntPtr pointer, IntPtr playerPtr, bool isMuted);

        /// <summary>
        /// Mutes a player in the chat, including the local player.
        /// </summary>
        /// <param name="player">The player that GameKit mutes or unmutes.</param>
        /// <param name="isMuted">Determines whether to mute or unmute the player.</param>
        public void SetPlayer(GKPlayer player, bool isMuted)
        {
            GKVoiceChat_SetPlayer(Pointer, player.Pointer, isMuted);
        }
        #endregion
        
        #region Volume
        [DllImport(InteropUtility.DLLName)]
        private static extern float GKVoiceChat_GetVolume(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKVoiceChat_SetVolume(IntPtr pointer, float value);

        /// <summary>
        /// The volume level for the channel.
        /// </summary>
        public float Volume
        {
            get => GKVoiceChat_GetVolume(Pointer);
            set => GKVoiceChat_SetVolume(Pointer, value);
        }
        #endregion

        #region Name
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKVoiceChat_GetName(IntPtr pointer);

        /// <summary>
        /// The name of the voice chat channel.
        /// </summary>
        public string Name
        {
            get => GKVoiceChat_GetName(Pointer);
        }
        #endregion
        
        #region Players
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKVoiceChat_GetPlayers(IntPtr pointer);

        /// <summary>
        /// The players connected to the channel.
        /// </summary>
        public NSArray<GKPlayer> Players => PointerCast<NSArrayGKPlayer>(GKVoiceChat_GetPlayers(Pointer));

        #endregion
        
        /// <summary>
        /// The state of a player in a voice chat.
        /// </summary>
        public enum PlayerState : long
        {
            /// <summary>
            /// The state when the player connects to the channel.
            /// </summary>
            Connected = 0,
            /// <summary>
            /// The state when the player left the channel.
            /// </summary>
            Disconnected = 1,
            /// <summary>
            /// The state when the player speaks.
            /// </summary>
            Speaking = 2,
            /// <summary>
            /// The state when the player isn't speaking.
            /// </summary>
            Silent = 3,
            /// <summary>
            /// The state when the player is connecting to the channel, but isn't connected yet.
            /// </summary>
            Connecting = 4
        }
    }
}