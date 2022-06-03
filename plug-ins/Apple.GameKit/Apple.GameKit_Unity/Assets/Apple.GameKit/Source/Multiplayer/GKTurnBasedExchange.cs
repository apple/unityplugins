using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// The exchange information sent between players even when a player is not the current player.
    /// </summary>
    public class GKTurnBasedExchange : InteropReference
    {
        #region Init & Dispose
        public GKTurnBasedExchange(IntPtr pointer) : base(pointer)
        {
        }
        
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKTurnBasedExchange_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                GKTurnBasedExchange_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }
        #endregion
        
        #region Cancel
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKTurnBasedExchange_Cancel(IntPtr pointer, long taskId, string localizableMessageKey, IntPtr arguments, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Cancels an exchange.
        /// </summary>
        /// <param name="localizableMessageKey">A string in the Localizable.strings file for the current localization.</param>
        /// <param name="arguments">An array of objects to be substituted using the format string.</param>
        /// <returns></returns>
        public Task Cancel(string localizableMessageKey, string[] arguments)
        {
            // Arguments...
            var mutableArguments = NSMutableArrayFactory.Init<NSMutableArrayString, string>();
            if(arguments != null)
                foreach(var argument in arguments)
                    mutableArguments.Add(argument);

            var tcs = InteropTasks.Create<bool>(out var taskId);
            GKTurnBasedExchange_Cancel(Pointer, taskId, localizableMessageKey, mutableArguments.Pointer, OnCancel, OnCancelError);
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
        
        #region CompletionDate
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKTurnBasedExchange_GetCompletionDate(IntPtr pointer);

        /// <summary>
        /// The date when the exchange was completed.
        /// </summary>
        public DateTimeOffset CompletionDate
        {
            get => DateTimeOffset.FromUnixTimeSeconds(GKTurnBasedExchange_GetCompletionDate(Pointer));
        }
        #endregion
        
        #region SendDate
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKTurnBasedExchange_GetSendDate(IntPtr pointer);

        /// <summary>
        /// The date that the exchange was sent out.
        /// </summary>
        public DateTimeOffset SendDate
        {
            get => DateTimeOffset.FromUnixTimeSeconds(GKTurnBasedExchange_GetSendDate(Pointer));
        }
        #endregion
        
        #region TimeoutDate
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKTurnBasedExchange_GetTimeoutDate(IntPtr pointer);

        /// <summary>
        /// The amount of time the exchange is to stay active before timing out.
        /// </summary>
        public DateTimeOffset TimeoutDate
        {
            get => DateTimeOffset.FromUnixTimeSeconds(GKTurnBasedExchange_GetTimeoutDate(Pointer));
        }
        #endregion
        
        #region Sender
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKTurnBasedExchange_GetSender(IntPtr pointer);
        
        /// <summary>
        /// The participant who sent the exchange.
        /// </summary>
        public GKTurnBasedParticipant Sender
        {
            get => PointerCast<GKTurnBasedParticipant>(GKTurnBasedExchange_GetSender(Pointer));
        }
        
        #endregion
        
        #region Data
        [DllImport(InteropUtility.DLLName)]
        private static extern InteropData GKTurnBasedExchange_GetData(IntPtr pointer);

        /// <summary>
        /// Data that is sent with the exchange.
        /// </summary>
        public byte[] Data
        {
            get => GKTurnBasedExchange_GetData(Pointer).ToBytes();
        }
        #endregion
        
        #region ExchangeId
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKTurnBasedExchange_GetExchangeID(IntPtr pointer);

        /// <summary>
        /// A persistent identifier that is used when referring to this exchange.
        /// </summary>
        public string ExchangeId
        {
            get => GKTurnBasedExchange_GetExchangeID(Pointer);
        }
        #endregion
        
        #region Message
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKTurnBasedExchange_GetMessage(IntPtr pointer);

        /// <summary>
        /// The localized message that is pushed to all of the recipients of the exchange.
        /// </summary>
        public string Message
        {
            get => GKTurnBasedExchange_GetMessage(Pointer);
        }
        #endregion
        
        #region Recipients
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKTurnBasedExchange_GetRecipients(IntPtr pointer);

        /// <summary>
        /// The players to receive the exchange.
        /// </summary>
        public NSArray<GKTurnBasedParticipant> Recipients => PointerCast<NSArrayGKTurnBasedParticipant>(GKTurnBasedExchange_GetRecipients(Pointer));
        #endregion
        
        #region Replies
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKTurnBasedExchange_GetReplies(IntPtr pointer);

        /// <summary>
        /// List of exchange replies.
        /// </summary>
        public NSArray<GKTurnBasedExchangeReply> Replies => PointerCast<NSArrayGKTurnBasedExchangeReply>(GKTurnBasedExchange_GetReplies(Pointer));
        #endregion
        
        #region Status
        [DllImport(InteropUtility.DLLName)]
        private static extern GKTurnBasedExchangeStatus GKTurnBasedExchange_GetStatus(IntPtr pointer);

        /// <summary>
        /// The current status of the exchange.
        /// </summary>
        public GKTurnBasedExchangeStatus Status
        {
            get => GKTurnBasedExchange_GetStatus(Pointer);
        }
        #endregion

        #region Reply
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKTurnBasedExchange_Reply(IntPtr pointer, long taskId, string localizableMessageKey, IntPtr arguments, InteropData data, SuccessTaskCallback onSuccess, NSErrorTaskCallback onError);
        
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
            var mutableArguments = NSMutableArrayFactory.Init<NSMutableArrayString, string>();
            if(arguments != null)
                foreach(var argument in arguments)
                    mutableArguments.Add(argument);

            // Execute...
            var tcs = InteropTasks.Create<bool>(out var taskId);
            GKTurnBasedExchange_Reply(Pointer, taskId, localizableMessageKey, mutableArguments.Pointer, interopData, OnReply, OnReplyError);
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
    }
}