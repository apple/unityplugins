using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// The exchange information sent between players even when a player is not the current player.
    /// </summary>
    public class GKTurnBasedExchange : NSObject
    {
        public GKTurnBasedExchange(IntPtr pointer) : base(pointer)
        {
        }

        #region Cancel

        /// <summary>
        /// Cancels an exchange.
        /// </summary>
        /// <param name="localizableMessageKey">A string in the Localizable.strings file for the current localization.</param>
        /// <param name="arguments">An array of objects to be substituted using the format string.</param>
        /// <returns></returns>
        public Task Cancel(string localizableMessageKey, string[] arguments)
        {
            // Arguments...
            var mutableArguments = new NSMutableArray<NSString>(arguments?.Select(arg => new NSString(arg)));

            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKTurnBasedExchange_Cancel(Pointer, taskId, localizableMessageKey, mutableArguments.Pointer, OnCancel, OnCancelError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnCancel(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnCancelError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        /// <summary>
        /// The date when the exchange was completed.
        /// </summary>
        public DateTimeOffset CompletionDate => DateTimeOffset.FromUnixTimeSeconds(Interop.GKTurnBasedExchange_GetCompletionDate(Pointer));

        /// <summary>
        /// The date that the exchange was sent out.
        /// </summary>
        public DateTimeOffset SendDate => DateTimeOffset.FromUnixTimeSeconds(Interop.GKTurnBasedExchange_GetSendDate(Pointer));

        /// <summary>
        /// The amount of time the exchange is to stay active before timing out.
        /// </summary>
        public DateTimeOffset TimeoutDate => DateTimeOffset.FromUnixTimeSeconds(Interop.GKTurnBasedExchange_GetTimeoutDate(Pointer));

        /// <summary>
        /// The participant who sent the exchange.
        /// </summary>
        public GKTurnBasedParticipant Sender => PointerCast<GKTurnBasedParticipant>(Interop.GKTurnBasedExchange_GetSender(Pointer));
        
        /// <summary>
        /// Data that is sent with the exchange.
        /// </summary>
        public byte[] Data => Interop.GKTurnBasedExchange_GetData(Pointer).ToBytes();

        /// <summary>
        /// A persistent identifier that is used when referring to this exchange.
        /// </summary>
        public string ExchangeId => Interop.GKTurnBasedExchange_GetExchangeID(Pointer);

        /// <summary>
        /// The localized message that is pushed to all of the recipients of the exchange.
        /// </summary>
        public string Message => Interop.GKTurnBasedExchange_GetMessage(Pointer);

        /// <summary>
        /// The players to receive the exchange.
        /// </summary>
        public NSArray<GKTurnBasedParticipant> Recipients => PointerCast<NSArray<GKTurnBasedParticipant>>(Interop.GKTurnBasedExchange_GetRecipients(Pointer));
        
        /// <summary>
        /// List of exchange replies.
        /// </summary>
        public NSArray<GKTurnBasedExchangeReply> Replies => PointerCast<NSArray<GKTurnBasedExchangeReply>>(Interop.GKTurnBasedExchange_GetReplies(Pointer));

        /// <summary>
        /// The current status of the exchange.
        /// </summary>
        public GKTurnBasedExchangeStatus Status => Interop.GKTurnBasedExchange_GetStatus(Pointer);

        #region Reply
        
        /// <summary>
        /// Replies to an exchange.
        /// </summary>
        /// <param name="localizableMessageKey">A string in the Localizable.strings file for the current localization.</param>
        /// <param name="arguments">An array of objects to be substituted using the format string.</param>
        /// <param name="data">The data associated with the exchange.</param>
        /// <returns></returns>
        public Task Reply(string localizableMessageKey, string[] arguments, byte[] data)
        {
            // Data...
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var interopData = new InteropData
            {
                DataPtr = handle.AddrOfPinnedObject(),
                DataLength = data.Length
            };
            
            // Arguments...
            var mutableArguments = new NSMutableArray<NSString>(arguments?.Select(arg => new NSString(arg)));

            // Execute...
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.GKTurnBasedExchange_Reply(Pointer, taskId, localizableMessageKey, mutableArguments.Pointer, interopData, OnReply, OnReplyError);
            handle.Free();
            
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnReply(long taskId)
        {
            InteropTasks.TrySetResultAndRemove(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnReplyError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        /// <summary>
        /// The status of an exchange or reply.
        /// </summary>
        public enum GKTurnBasedExchangeStatus : sbyte
        {
            /// <summary>
            /// The state of the exchange or reply is not currently known.
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// The exchange or reply is currently active.
            /// </summary>
            Active = 1,
            /// <summary>
            /// The exchange or reply has been completed.
            /// </summary>
            Complete = 2,
            /// <summary>
            /// The exchange or reply has been resolved.
            /// </summary>
            Resolved = 3,
            /// <summary>
            /// The exchange or reply has been cancelled.
            /// </summary>
            Canceled = 4
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedExchange_Cancel(IntPtr pointer, long taskId, string localizableMessageKey, IntPtr arguments, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern long GKTurnBasedExchange_GetCompletionDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern long GKTurnBasedExchange_GetSendDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern long GKTurnBasedExchange_GetTimeoutDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKTurnBasedExchange_GetSender(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern InteropData GKTurnBasedExchange_GetData(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKTurnBasedExchange_GetExchangeID(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKTurnBasedExchange_GetMessage(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKTurnBasedExchange_GetRecipients(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKTurnBasedExchange_GetReplies(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKTurnBasedExchangeStatus GKTurnBasedExchange_GetStatus(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedExchange_Reply(IntPtr pointer, long taskId, string localizableMessageKey, IntPtr arguments, InteropData data, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
        }
    }
}
