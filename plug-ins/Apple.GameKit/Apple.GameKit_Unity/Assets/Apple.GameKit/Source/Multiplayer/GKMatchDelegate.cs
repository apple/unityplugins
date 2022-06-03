using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// An object that receives connection status and data transmitted in a multiplayer game.
    /// </summary>
    public class GKMatchDelegate : InteropReference
    {
        #region Delegates
        /// <summary>
        /// Processes the data sent from one player to another.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="forRecipient"></param>
        /// <param name="fromPlayer"></param>
        public delegate void DataReceivedForPlayerHandler(byte[] data, GKPlayer forRecipient, GKPlayer fromPlayer);
        private delegate void InteropDataReceivedForPlayerHandler(IntPtr pointer, IntPtr data, int dataLength, IntPtr forRecipientPtr, IntPtr fromPlayerPtr);

        /// <summary>
        /// Processes the data sent from another player to the local player.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fromPlayer"></param>
        public delegate void DataReceivedHandler(byte[] data, GKPlayer fromPlayer);
        internal delegate void InteropDataReceivedHandler(IntPtr pointer, IntPtr data, int dataLength, IntPtr fromPlayerPtr);

        /// <summary>
        /// Handles when players connect or disconnect from a match.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="connectionState"></param>
        public delegate void PlayerConnectionDidChangeHandler(GKPlayer player, GKPlayerConnectionState connectionState);
        private delegate void InteropPlayerConnectionDidChangeHandler(IntPtr pointer, IntPtr playerPtr, GKPlayerConnectionState connectionState);
        
        /// <summary>
        /// Handles the local player's connection errors to a match.
        /// </summary>
        /// <param name="exception"></param>
        public delegate void DidFailWithErrorHandler(GameKitException exception);
        private delegate void InteropDidFailWithErrorHandler(IntPtr pointer, IntPtr errorPointer);

        /// <summary>
        /// Determines whether the local player should reinvite another player who disconnected from a two-player match.
        /// </summary>
        /// <param name="player"></param>
        public delegate bool ShouldReinviteDisconnectedPlayerHandler(GKPlayer player);
        internal delegate bool InteropShouldReinviteDisconnectedPlayerHandler(IntPtr pointer, IntPtr playerPtr);
        #endregion

        /// <summary>
        /// Processes the data sent from one player to another.
        /// </summary>
        public event DataReceivedForPlayerHandler DataReceivedForPlayer;
        /// <summary>
        /// Processes the data sent from another player to the local player.
        /// </summary>
        public event DataReceivedHandler DataReceived;
        /// <summary>
        /// Handles when players connect or disconnect from a match.
        /// </summary>
        public event PlayerConnectionDidChangeHandler PlayerConnectionChanged;
        /// <summary>
        /// Handles the local player's connection errors to a match.
        /// </summary>
        public event DidFailWithErrorHandler DidFailWithError;
        /// <summary>
        /// Determines whether the local player should reinvite another player who disconnected from a two-player match.
        /// </summary>
        public event ShouldReinviteDisconnectedPlayerHandler ShouldReinviteDisconnectedPlayer;
        
        private static readonly Dictionary<IntPtr, GKMatchDelegate> _delegates = new Dictionary<IntPtr, GKMatchDelegate>();

        #region Init & Dispose
        internal GKMatchDelegate(IntPtr pointer) : base(pointer)
        {
            _delegates[Pointer] = this;
            
            GKMatchDelegate_SetDataReceived(Pointer, OnDataReceived);
            GKMatchDelegate_SetDataReceivedForPlayer(Pointer, OnDataReceivedForPlayer);
            GKMatchDelegate_SetDidFailWithError(Pointer, OnDidFailWithError);
            GKMatchDelegate_SetPlayerConnectedDidChange(Pointer, OnPlayerConnectionDidChange);
            GKMatchDelegate_SetShouldReinviteDisconnectedPlayer(Pointer, OnShouldReinviteDisconnectedPlayer);
        }
        
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchDelegate_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                GKMatchDelegate_Free(Pointer);
                _delegates.Remove(Pointer);
                Pointer = IntPtr.Zero;
            }
        }
        #endregion
        
        #region Callback Handlers
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchDelegate_SetDataReceivedForPlayer(IntPtr pointer, InteropDataReceivedForPlayerHandler handler);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchDelegate_SetDataReceived(IntPtr pointer, InteropDataReceivedHandler handler);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchDelegate_SetPlayerConnectedDidChange(IntPtr pointer, InteropPlayerConnectionDidChangeHandler handler);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchDelegate_SetDidFailWithError(IntPtr pointer, InteropDidFailWithErrorHandler handler);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchDelegate_SetShouldReinviteDisconnectedPlayer(IntPtr pointer, InteropShouldReinviteDisconnectedPlayerHandler handler);

        [MonoPInvokeCallback(typeof(InteropDataReceivedForPlayerHandler))]
        private static void OnDataReceivedForPlayer(IntPtr pointer, IntPtr dataPtr, int dataLength, IntPtr forRecipientPtr, IntPtr fromPlayerPtr)
        {
            if (!_delegates.TryGetValue(pointer, out var matchDelegate))
                return;
            
            var data = new byte[dataLength];
            Marshal.Copy(dataPtr, data, 0, dataLength);

            var recipient = forRecipientPtr != IntPtr.Zero ? new GKPlayer(forRecipientPtr) : null;
            var from = fromPlayerPtr != IntPtr.Zero ? new GKPlayer(fromPlayerPtr) : null;
            
            matchDelegate.DataReceivedForPlayer?.Invoke(data, recipient, from);
        }
        
        [MonoPInvokeCallback(typeof(InteropDataReceivedHandler))]
        private static void OnDataReceived(IntPtr pointer, IntPtr dataPtr, int dataLength, IntPtr fromPlayerPtr)
        {
            if (!_delegates.TryGetValue(pointer, out var matchDelegate))
                return;
            
            var data = new byte[dataLength];
            Marshal.Copy(dataPtr, data, 0, dataLength);
            
            var from = fromPlayerPtr != IntPtr.Zero ? new GKPlayer(fromPlayerPtr) : null;
            matchDelegate.DataReceived?.Invoke(data, from);
        }
        
        [MonoPInvokeCallback(typeof(InteropPlayerConnectionDidChangeHandler))]
        private static void OnPlayerConnectionDidChange(IntPtr pointer, IntPtr playerPtr, GKPlayerConnectionState state)
        {
            if (!_delegates.TryGetValue(pointer, out var matchDelegate))
                return;
            
            var player = playerPtr != IntPtr.Zero ? new GKPlayer(playerPtr) : null;
            matchDelegate.PlayerConnectionChanged?.Invoke(player, state);
        }
        
        [MonoPInvokeCallback(typeof(InteropDidFailWithErrorHandler))]
        private static void OnDidFailWithError(IntPtr pointer, IntPtr errorPtr)
        {
            if (!_delegates.TryGetValue(pointer, out var matchDelegate))
                return;
            
            matchDelegate.DidFailWithError?.Invoke(new GameKitException(errorPtr));
        }
        
        [MonoPInvokeCallback(typeof(InteropShouldReinviteDisconnectedPlayerHandler))]
        private static bool OnShouldReinviteDisconnectedPlayer(IntPtr pointer, IntPtr playerPtr)
        {
            if (!_delegates.TryGetValue(pointer, out var matchDelegate))
                return false;
            
            var player = playerPtr != IntPtr.Zero ? new GKPlayer(playerPtr) : null;
            return matchDelegate.ShouldReinviteDisconnectedPlayer?.Invoke(player) ?? false;
        }
        #endregion

        /// <summary>
        /// The possible states of a connection to a match.
        /// </summary>
        public enum GKPlayerConnectionState : long
        {
            /// <summary>
            /// An undetermined state in which the player can't receive data.
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// A state in which a player connects to the match and can receive data.
            /// </summary>
            Connected = 1,
            /// <summary>
            /// A state in which a player disconnects from the match and canâ€™t receive data.
            /// </summary>
            Disconnected = 2
        }
    }
}