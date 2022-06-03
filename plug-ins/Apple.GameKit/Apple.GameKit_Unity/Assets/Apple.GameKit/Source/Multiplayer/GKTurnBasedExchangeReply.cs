using System;
using System.Runtime.InteropServices;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// The player's response to an exchange.
    /// </summary>
    public class GKTurnBasedExchangeReply : InteropReference
    {
        #region Init & Dispose
        public GKTurnBasedExchangeReply(IntPtr pointer) : base(pointer)
        {
        }

        [DllImport(InteropUtility.DLLName)]
        private static extern void GKTurnBasedExchangeReply_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                GKTurnBasedExchangeReply_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }
        #endregion
        
        #region Data
        [DllImport(InteropUtility.DLLName)]
        private static extern InteropData GKTurnBasedExchangeReply_GetData(IntPtr pointer);

        /// <summary>
        /// Exchange data sent by the recipient.
        /// </summary>
        public byte[] Data
        {
            get => GKTurnBasedExchangeReply_GetData(Pointer).ToBytes();
        }
        #endregion
        
        #region Message
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKTurnBasedExchangeReply_GetMessage(IntPtr pointer);

        /// <summary>
        /// Localizable message for the push notification
        /// </summary>
        public string Message
        {
            get => GKTurnBasedExchangeReply_GetMessage(Pointer);
        }
        #endregion
        
        #region Recipient
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKTurnBasedExchangeReply_GetRecipient(IntPtr pointer);

        /// <summary>
        /// The player that is replying to the exchange.
        /// </summary>
        public GKTurnBasedParticipant Recipient
        {
            get => PointerCast<GKTurnBasedParticipant>(GKTurnBasedExchangeReply_GetRecipient(Pointer));
        }
        #endregion
        
        #region ReplyDate
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKTurnBasedExchangeReply_GetReplyDate(IntPtr pointer);

        /// <summary>
        /// The date the reply exchange was sent.
        /// </summary>
        public DateTimeOffset ReplyDate
        {
            get => DateTimeOffset.FromUnixTimeSeconds(GKTurnBasedExchangeReply_GetReplyDate(Pointer));
        }

        #endregion
    }
}