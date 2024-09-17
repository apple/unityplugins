using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    using GKMatchProperties = NSDictionary<NSString, NSObject>;

    /// <summary>
    /// A peer-to-peer network between a group of players that sign into Game Center.
    /// </summary>
    public class GKMatch : NSObject
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

        private GKMatchDelegate _delegate;

        /// <summary>
        /// The delegate that handles communication between players in a match.
        /// </summary>
        public GKMatchDelegate Delegate => _delegate ??= PointerCast<GKMatchDelegate>(Interop.GKMatch_GetDelegate(Pointer));

        internal GKMatch(IntPtr pointer) : base(pointer)
        {
        }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                // Dispose any delegates...
                Delegate?.Dispose();
            }

            base.OnDispose(isDisposing);
        }

        /// <summary>
        /// The remaining number of players invited but not yet connected to the match.
        /// </summary>
        public long ExpectedPlayerCount => Interop.GKMatch_GetExpectedPlayerCount(Pointer);

        /// <summary>
        /// The players that join the match.
        /// </summary>
        public NSArray<GKPlayer> Players => PointerCast<NSArray<GKPlayer>>(Interop.GKMatch_GetPlayers(Pointer));

        /// <summary>
        /// The local player's properties that matchmaking rules used to find the players with some additions.
        /// </summary>
        [Introduced(iOS: "17.2", macOS: "14.2", tvOS: "17.2", visionOS: "1.1")]
        public GKMatchProperties Properties => PointerCast<GKMatchProperties>(Interop.GKMatch_GetProperties(Pointer));

        /// <summary>
        /// The properties for other players that matchmaking rules uses to find players, with some additions.
        /// </summary>
        [Introduced(iOS: "17.2", macOS: "14.2", tvOS: "17.2", visionOS: "1.1")]
        public NSDictionary<GKPlayer, GKMatchProperties> PlayerProperties => PointerCast<NSDictionary<GKPlayer, GKMatchProperties>>(Interop.GKMatch_GetPlayerProperties(Pointer));

        /// <summary>
        /// Transmits data to one or more players connected to the match.
        /// </summary>
        /// <param name="data">The bytes to send.</param>
        /// <param name="players">The players who receive the data.</param>
        /// <param name="sendDataMode">The mechanism used to send the data.</param>
        public void Send(byte[] data, NSArray<GKPlayer> players, GKSendDataMode sendDataMode)
        {
            var errorPointer = Interop.GKMatch_SendTo(Pointer, new NSData(data).Pointer, players.Pointer, sendDataMode);
            
            if (errorPointer != IntPtr.Zero)
            {
                throw new GameKitException(errorPointer);
            }
        }
        
        /// <summary>
        /// Transmits data to all players connected to the match.
        /// </summary>
        /// <param name="data">The bytes to send.</param>
        /// <param name="sendDataMode">The mechanism used to send the data.</param>
        public void Send(byte[] data, GKSendDataMode sendDataMode)
        {
            var errorPointer = Interop.GKMatch_SendToAll(Pointer, new NSData(data).Pointer, sendDataMode);

            if (errorPointer != IntPtr.Zero)
            {
                throw new GameKitException(errorPointer);
            }
        }
        
        /// <summary>
        /// Disconnects the local player from the match.
        /// </summary>
        public void Disconnect() => Interop.GKMatch_Disconnect(Pointer);
        
        #region ChooseBestHostingPlayer

        /// <summary>
        /// Determines the best player in the game to act as the server for a client-server topology.
        /// </summary>
        /// <returns>The player with the best estimated network performance, or nil if GameKit couldn't determine the best host.</returns>
        public Task<GKPlayer> ChooseBestHostingPlayer()
        {
            var tcs = InteropTasks.Create<GKPlayer>(out var taskId);
            Interop.GKMatch_ChooseBestHostingPlayer(Pointer, taskId, OnChooseBestHostingPlayerSuccess, OnChooseBestHostingPlayerError);
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
        
        /// <summary>
        /// Joins the local player to a voice channel.
        /// </summary>
        /// <param name="channel">The name of the channel to join.</param>
        /// <returns>A voice chat object for the channel, or nil if an error occurs or parental controls restrict the player from joining a voice chat.</returns>
        public GKVoiceChat VoiceChat(string channel)
        {
            var pointer = Interop.GKMatch_VoiceChat(Pointer, channel);
            return pointer != IntPtr.Zero ? new GKVoiceChat(pointer) : null;
        }
        
        #region Rematch

        /// <summary>
        /// Creates a new match with the players from an existing match.
        /// </summary>
        /// <returns>A new match, or nil sif an error occurs.</returns>
        public Task<GKMatch> Rematch()
        {
            var tcs = InteropTasks.Create<GKMatch>(out var taskId);
            Interop.GKMatch_Rematch(Pointer, taskId, OnRematch, OnRematchError);
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

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKMatch_GetDelegate(IntPtr matchPointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern long GKMatch_GetExpectedPlayerCount(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKMatch_GetPlayers(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKMatch_GetProperties(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKMatch_GetPlayerProperties(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKMatch_SendTo(IntPtr pointer, IntPtr nsData, IntPtr players, GKSendDataMode sendDataMode);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKMatch_SendToAll(IntPtr pointer, IntPtr nsData, GKSendDataMode sendDataMode);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatch_Disconnect(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatch_ChooseBestHostingPlayer(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKMatch_VoiceChat(IntPtr pointer, string channel);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatch_Rematch(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
        }
    }
}
