using System;
using System.Runtime.InteropServices;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// A voice channel that allows players to speak with each other in a multiplayer game.
    /// </summary>
    public class GKVoiceChat : NSObject
    {
        private static readonly InteropWeakMap<GKVoiceChat> _instanceMap = new InteropWeakMap<GKVoiceChat>();

        /// <summary>
        /// A method that handles when a player's voice chat changes state.
        /// </summary>
        public event PlayerVoiceChatStateDidChangeHandler PlayerVoiceChatStateDidChange;
        
        internal GKVoiceChat(IntPtr pointer) : base(pointer)
        {
            _instanceMap.Add(this);
            Interop.GKVoiceChat_PlayerVoiceChatStateDidChangeHandler(pointer, OnPlayerVoiceChatStateDidChange);
        }

        protected override void OnDispose(bool isDisposing)
        {
            _instanceMap.Remove(this);
            base.OnDispose(isDisposing);
        }
        
        /// <summary>
        /// Returns whether voice chat is available on the device.
        /// </summary>
        public static bool IsVoIPAllowed => Interop.GKVoiceChat_GetIsVoIPAllowed();

        /// <summary>
        /// Starts communication with other players in a channel.
        /// </summary>
        public void Start()
        {
            Interop.GKVoiceChat_Start(Pointer);
        }

        /// <summary>
        /// Ends communication with other players in a channe
        /// </summary>
        public void Stop()
        {
            Interop.GKVoiceChat_Stop(Pointer);
        }

        /// <summary>
        /// A Boolean value that indicates whether the channel is sampling the microphone.
        /// </summary>
        public bool IsActive
        {
            get => Interop.GKVoiceChat_GetIsActive(Pointer);
            set => Interop.GKVoiceChat_SetIsActive(Pointer, value);
        }
        
        #region PlayerVoiceChatStateDidChangeHandler
        public delegate void PlayerVoiceChatStateDidChangeHandler(GKPlayer player, PlayerState state);
        
        internal delegate void InternalPlayerVoiceChatStateDidChangeHandler(IntPtr pointer, IntPtr playerPtr, PlayerState state);

        [MonoPInvokeCallback(typeof(InternalPlayerVoiceChatStateDidChangeHandler))]
        private static void OnPlayerVoiceChatStateDidChange(IntPtr pointer, IntPtr playerPtr, PlayerState state)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                if (_instanceMap.TryGet(pointer, out var gkVoiceChat))
                {
                    var player = PointerCast<GKPlayer>(playerPtr);
                    gkVoiceChat.PlayerVoiceChatStateDidChange?.Invoke(player, state);
                }
            });
        }
        #endregion

        /// <summary>
        /// Mutes a player in the chat, including the local player.
        /// </summary>
        /// <param name="player">The player that GameKit mutes or unmutes.</param>
        /// <param name="isMuted">Determines whether to mute or unmute the player.</param>
        public void SetPlayer(GKPlayer player, bool isMuted)
        {
            Interop.GKVoiceChat_SetPlayer(Pointer, player.Pointer, isMuted);
        }

        /// <summary>
        /// The volume level for the channel.
        /// </summary>
        public float Volume
        {
            get => Interop.GKVoiceChat_GetVolume(Pointer);
            set => Interop.GKVoiceChat_SetVolume(Pointer, value);
        }

        /// <summary>
        /// The name of the voice chat channel.
        /// </summary>
        public string Name => Interop.GKVoiceChat_GetName(Pointer);

        /// <summary>
        /// The players connected to the channel.
        /// </summary>
        public NSArray<GKPlayer> Players => PointerCast<NSArray<GKPlayer>>(Interop.GKVoiceChat_GetPlayers(Pointer));
        
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

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKVoiceChat_GetIsVoIPAllowed();
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKVoiceChat_Start(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKVoiceChat_Stop(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKVoiceChat_GetIsActive(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKVoiceChat_SetIsActive(IntPtr pointer, bool value);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKVoiceChat_PlayerVoiceChatStateDidChangeHandler(IntPtr pointer, InternalPlayerVoiceChatStateDidChangeHandler onStateChanged);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKVoiceChat_SetPlayer(IntPtr pointer, IntPtr playerPtr, bool isMuted);
            [DllImport(InteropUtility.DLLName)]
            public static extern float GKVoiceChat_GetVolume(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKVoiceChat_SetVolume(IntPtr pointer, float value);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKVoiceChat_GetName(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKVoiceChat_GetPlayers(IntPtr pointer);
        }
    }
}
