using System;
using System.Runtime.InteropServices;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// An object that encapsulates the parameters to create a real-time or turn-based match.
    /// </summary>
    public class GKMatchRequest : InteropReference
    {
        /// <summary>
        /// A player's response to an invitation to join a match.
        /// </summary>
        public enum GKInviteRecipientResponse : long
        {
            /// <summary>
            /// A response when the player accepts the invitation
            /// </summary>
            Accepted = 0,
            /// <summary>
            /// A response when the player rejects the invitation.
            /// </summary>
            Declined = 1,
            /// <summary>
            /// A response when the system fails to deliver the invitation to the player.
            /// </summary>
            Failed = 2,
            /// <summary>
            /// A response when the player isn't running a compatible version of the game.
            /// </summary>
            Incompatible = 3,
            /// <summary>
            /// A response when the system can't contact the player.
            /// </summary>
            UnableToConnect = 4,
            /// <summary>
            /// A response when the invitation times out because the player doesn't answer it.
            /// </summary>
            NoAnswer = 5
        }
        
        /// <summary>
        /// The kind of match managed by Game Center.
        /// </summary>
        public enum GKMatchType : ulong
        {
            /// <summary>
            /// A peer-to-peer match hosted by Game Center.
            /// </summary>
            PeerToPeer = 0,
            /// <summary>
            /// A match hosted on your private server.
            /// </summary>
            Hosted = 1,
            /// <summary>
            /// A turn-based match hosted by Game Center.
            /// </summary>
            TurnBased = 2
        }
        
        #region Init & Dispose
        internal GKMatchRequest(IntPtr pointer) : base(pointer)
        {
        }

        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchRequest_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                GKMatchRequest_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        #endregion
        
        #region MaxPlayers
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKMatchRequest_GetMaxPlayers(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchRequest_SetMaxPlayers(IntPtr pointer, long maxPlayers);

        /// <summary>
        /// The maximum number of players that can join the match.
        /// </summary>
        public long MaxPlayers
        {
            get => GKMatchRequest_GetMaxPlayers(Pointer);
            set => GKMatchRequest_SetMaxPlayers(Pointer, value);
        }
        #endregion
        
        #region MinPlayers
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKMatchRequest_GetMinPlayers(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchRequest_SetMinPlayers(IntPtr pointer, long maxPlayers);

        /// <summary>
        /// The minimum number of players that can join the match.
        /// </summary>
        public long MinPlayers
        {
            get => GKMatchRequest_GetMinPlayers(Pointer);
            set => GKMatchRequest_SetMinPlayers(Pointer, value);
        }
        #endregion
        
        #region PlayerGroup
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKMatchRequest_GetPlayerGroup(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchRequest_SetPlayerGroup(IntPtr pointer, long maxPlayers);

        /// <summary>
        /// A number identifying a subset of players invited to join a match.
        /// </summary>
        public long PlayerGroup
        {
            get => GKMatchRequest_GetPlayerGroup(Pointer);
            set => GKMatchRequest_SetPlayerGroup(Pointer, value);
        }
        #endregion
        
        #region PlayerAttributes
        [DllImport(InteropUtility.DLLName)]
        private static extern uint GKMatchRequest_GetPlayerAttributes(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchRequest_SetPlayerAttributes(IntPtr pointer, uint value);

        /// <summary>
        /// A mask that specifies the role that the local player would like to play in the game.
        /// </summary>
        public uint PlayerAttributes
        {
            get => GKMatchRequest_GetPlayerAttributes(Pointer);
            set => GKMatchRequest_SetPlayerAttributes(Pointer, value);
        }
        #endregion
        
        #region InviteMessage
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKMatchRequest_GetInviteMessage(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchRequest_SetInviteMessage(IntPtr pointer, string value);

        /// <summary>
        /// The message sent to other players when the local player invites them to join a match.
        /// </summary>
        public string InviteMessage
        {
            get => GKMatchRequest_GetInviteMessage(Pointer);
            set => GKMatchRequest_SetInviteMessage(Pointer, value);
        }
        #endregion
        
        #region Recipients
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKMatchRequest_GetRecipients(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchRequest_SetRecipients(IntPtr pointer, IntPtr value);

        /// <summary>
        /// The message sent to other players when the local player invites them to join a match.
        /// </summary>
        public NSArray<GKPlayer> Recipients
        {
            get => PointerCast<NSArrayGKPlayer>(GKMatchRequest_GetRecipients(Pointer));
            set => GKMatchRequest_SetRecipients(Pointer, value.Pointer);
        }
        #endregion
        
        #region Static Init
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKMatchRequest_Init();

        public static GKMatchRequest Init()
        {
            return PointerCast<GKMatchRequest>(GKMatchRequest_Init());
        }

        #endregion
    }
}