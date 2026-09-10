using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;
using UnityEngine;

namespace Apple.StoreKit
{
    /// <summary>
    /// Message reason enumeration matching StoreKit 2 Message.Reason
    /// </summary>
    [Introduced(iOS: "16.0", visionOS: "2.2")]
    [Unavailable(RuntimeOperatingSystem.macOS, RuntimeOperatingSystem.tvOS)]
    public enum MessageReason
    {
        Generic = 0,

        [Introduced(iOS: "16.4", visionOS: "2.2")]
        BillingIssue = 1,

        PriceIncreaseConsent = 2,

        [Introduced(iOS: "18.0", visionOS: "2.2")]
        WinBackOffer = 3,

        Unknown = -1
    }

    /// <summary>
    /// In-app messages from the App Store.
    /// Available on iOS 16.0+, visionOS 2.2+
    /// </summary>
    [Introduced(iOS: "16.0", visionOS: "2.2")]
    [Unavailable(RuntimeOperatingSystem.macOS, RuntimeOperatingSystem.tvOS)]
    public class Message : InteropReference
    {
        internal Message(IntPtr pointer) : base(pointer) { }

        #region Properties
        /// <summary>
        /// The reason for the message.
        /// </summary>
        public MessageReason Reason => (MessageReason)Interop.Message_Reason(Pointer);

        private static EventHandler<Message> _updatesEventHandler;
        private static long _currentTaskId;
        public static event EventHandler<Message> MessageReceived
        {
            add
            {
                if (_updatesEventHandler == null)
                {
                    StartMessageUpdates();
                }
                _updatesEventHandler += value;
            }
            remove
            {
                _updatesEventHandler -= value;
                if (_updatesEventHandler == null)
                {
                    Debug.Log("Message: No more update handlers, stopping updates is not currently supported.");
                    if (!InteropTasks.TrySetResultAndRemove(_currentTaskId, IntPtr.Zero))
                    {
                        Debug.LogWarning("Message: Failed to stop message updates task.");
                    }
                }
            }
        }

        private static void StartMessageUpdates()
        {
            Debug.Log("Message: Starting message updates");
            InteropTasks.Create<IntPtr>(out _currentTaskId);
            Interop.Message_Updates(_currentTaskId, OnMessageUpdate);
        }

        [MonoPInvokeCallback(typeof(SuccessTaskBoolReturningCallback<IntPtr>))]
        [return:MarshalAs(UnmanagedType.I1)]
        private static bool OnMessageUpdate(long taskId, IntPtr ptr)
        {
            if (!InteropTasks.TryGet<IntPtr>(taskId, out var task)
                || task.Task.IsCompleted)
            {
                Debug.Log("Message: Received update for completed task, ignoring");
                return false;
            }

            Debug.Log("Update received for message");
            var message = new Message(ptr);
            _updatesEventHandler?.Invoke(null, message);
            return true;
        }
        #endregion

        #region Disposal
        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.Message_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }
        #endregion

        #region Methods
        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnDisplaySuccess(long taskId)
        {
            InteropTasks.TrySetResultAndRemove<bool>(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnDisplayError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Displays the message in the current window scene.
        /// </summary>
        /// <returns>Task that completes when the message is displayed</returns>
        public Task Display()
        {
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.Message_Display(Pointer, taskId, OnDisplaySuccess, OnDisplayError);
            return tcs.Task;
        }
        #endregion

        #region Interop
        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void Message_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int Message_Reason(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern void Message_Display(
                IntPtr pointer,
                long taskId,
                SuccessTaskCallback onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern void Message_Updates(
                long taskId,
                SuccessTaskBoolReturningCallback<IntPtr> onUpdate
            );
        }
        #endregion
    }
}
