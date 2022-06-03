using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// A peer-to-peer network between a group of players that sign into Game Center.
    /// </summary>
    public class GKMatch : InteropReference
    {
        /// <summary>
        /// The mechanism used to transmit data to other peers.
        /// </summary>
        public enum GKSendDataMode : long
        {
            /// <summary>
            /// Sends data continuously until the recipients successfully receive it or the connection times out.
            /// </summary>
            Reliable = 0,
            /// <summary>
            /// Sends data once even if an error occurs.
            /// </summary>
            Unreliable = 1
        }

        #region Delegate
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKMatch_GetDelegate(IntPtr matchPointer);

        private GKMatchDelegate _delegate;
        
        /// <summary>
        /// The delegate that handles communication between players in a match.
        /// </summary>
        public GKMatchDelegate Delegate
        {
            get
            {
                if (_delegate == null)
                {
                    _delegate = PointerCast<GKMatchDelegate>(GKMatch_GetDelegate(Pointer));
                }

                return _delegate;
            }
        }

        #endregion

        #region Init & Dispose
        internal GKMatch(IntPtr pointer) : base(pointer)
        {

        }
        
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatch_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                // Dispose any delegates...
                Delegate?.Dispose();
                
                // Free the match...
                GKMatch_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        #endregion
        
        #region ExpectedPlayerCount
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKMatch_GetExpectedPlayerCount(IntPtr pointer);

        /// <summary>
        /// The remaining number of players invited but not yet connected to the match.
        /// </summary>
        public long ExpectedPlayerCount
        {
            get => GKMatch_GetExpectedPlayerCount(Pointer);
        }
        #endregion
        
        #region Players
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKMatch_GetPlayers(IntPtr pointer);

        /// <summary>
        /// The players that join the match.
        /// </summary>
        public NSArray<GKPlayer> Players => PointerCast<NSArrayGKPlayer>(GKMatch_GetPlayers(Pointer));

        #endregion
        
        #region Send
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKMatch_SendTo(IntPtr pointer, IntPtr data, int dataLength, IntPtr players, GKSendDataMode sendDataMode);
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKMatch_SendToAll(IntPtr pointer, IntPtr data, int dataLength, GKSendDataMode sendDataMode);

        /// <summary>
        /// Transmits data to one or more players connected to the match.
        /// </summary>
        /// <param name="data">The bytes to send.</param>
        /// <param name="players">The players who receive the data.</param>
        /// <param name="sendDataMode">The mechanism used to send the data.</param>
        public void Send(byte[] data, NSArray<GKPlayer> players, GKSendDataMode sendDataMode)
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var errorPointer = GKMatch_SendTo(Pointer, handle.AddrOfPinnedObject(), data.Length, players.Pointer, sendDataMode);
            handle.Free();
            
            if(errorPointer != IntPtr.Zero)
                throw new GameKitException(errorPointer);
        }
        
        /// <summary>
        /// Transmits data to all players connected to the match.
        /// </summary>
        /// <param name="data">The bytes to send.</param>
        /// <param name="sendDataMode">The mechanism used to send the data.</param>
        public void Send(byte[] data, GKSendDataMode sendDataMode)
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var errorPointer = GKMatch_SendToAll(Pointer, handle.AddrOfPinnedObject(), data.Length, sendDataMode);
            handle.Free();
            
            if(errorPointer != IntPtr.Zero)
                throw new GameKitException(errorPointer);
        }
        #endregion
        
        #region Disconnect
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatch_Disconnect(IntPtr pointer);

        /// <summary>
        /// Disconnects the local player from the match.
        /// </summary>
        public void Disconnect()
        {
            GKMatch_Disconnect(Pointer);
        }
        #endregion
        
        #region ChooseBestHostingPlayer
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatch_ChooseBestHostingPlayer(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Determines the best player in the game to act as the server for a client-server topology.
        /// </summary>
        /// <returns>The player with the best estimated network performance, or nil if GameKit couldn't determine the best host.</returns>
        public Task<GKPlayer> ChooseBestHostingPlayer()
        {
            var tcs = InteropTasks.Create<GKPlayer>(out var taskId);
            GKMatch_ChooseBestHostingPlayer(Pointer, taskId, OnChooseBestHostingPlayerSuccess, OnChooseBestHostingPlayerError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnChooseBestHostingPlayerSuccess(long taskId, IntPtr playerPtr)
        {
            var player = playerPtr != IntPtr.Zero ? new GKPlayer(playerPtr) : null;
            InteropTasks.TrySetResultAndRemove(taskId, player);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnChooseBestHostingPlayerError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<GKPlayer>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region VoiceChat
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKMatch_VoiceChat(IntPtr pointer, string channel);
        
        /// <summary>
        /// Joins the local player to a voice channel.
        /// </summary>
        /// <param name="channel">The name of the channel to join.</param>
        /// <returns>A voice chat object for the channel, or nil if an error occurs or parental controls restrict the player from joining a voice chat.</returns>
        public GKVoiceChat VoiceChat(string channel)
        {
            var pointer = GKMatch_VoiceChat(Pointer, channel);
            return pointer != IntPtr.Zero ? new GKVoiceChat(pointer) : null;
        }
        #endregion
        
        #region Rematch
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatch_Rematch(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Creates a new match with the players from an existing match.
        /// </summary>
        /// <returns>A new match, or nil sif an error occurs.</returns>
        public Task<GKMatch> Rematch()
        {
            var tcs = InteropTasks.Create<GKMatch>(out var taskId);
            GKMatch_Rematch(Pointer, taskId, OnRematch, OnRematchError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnRematch(long taskId, IntPtr matchPtr)
        {
            var match = matchPtr != IntPtr.Zero ? new GKMatch(matchPtr) : null;
            InteropTasks.TrySetResultAndRemove(taskId, match);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnRematchError(long taskId, IntPtr errorPtr)
        {
            InteropTasks.TrySetExceptionAndRemove<GKMatch>(taskId, new GameKitException(errorPtr));
        }
        #endregion
    }
}