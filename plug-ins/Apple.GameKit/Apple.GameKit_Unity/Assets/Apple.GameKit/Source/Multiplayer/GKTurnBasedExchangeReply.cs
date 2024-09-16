using System;
using System.Runtime.InteropServices;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// The player's response to an exchange.
    /// </summary>
    public class GKTurnBasedExchangeReply : NSObject
    {
        public GKTurnBasedExchangeReply(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// Exchange data sent by the recipient.
        /// </summary>
        public byte[] Data => NSData.GetBytes(Interop.GKTurnBasedExchangeReply_GetData(Pointer));

        /// <summary>
        /// Localizable message for the push notification
        /// </summary>
        public string Message => Interop.GKTurnBasedExchangeReply_GetMessage(Pointer);

        /// <summary>
        /// The player that is replying to the exchange.
        /// </summary>
        public GKTurnBasedParticipant Recipient => PointerCast<GKTurnBasedParticipant>(Interop.GKTurnBasedExchangeReply_GetRecipient(Pointer));
        
        /// <summary>
        /// The date the reply exchange was sent.
        /// </summary>
        public DateTimeOffset ReplyDate => DateTimeOffsetExtensions.FromUnixTimeSeconds(Interop.GKTurnBasedExchangeReply_GetReplyDate(Pointer));

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKTurnBasedExchangeReply_GetData(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKTurnBasedExchangeReply_GetMessage(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKTurnBasedExchangeReply_GetRecipient(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKTurnBasedExchangeReply_GetReplyDate(IntPtr pointer);
        }
    }
}
