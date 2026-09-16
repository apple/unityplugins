using System;
using System.Runtime.InteropServices;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.StoreKit
{
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public class SubscriptionStatus : InteropReference
    {
        internal SubscriptionStatus(IntPtr pointer) : base(pointer) { }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.SubscriptionStatus_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        public RenewalState State => (RenewalState)Interop.SubscriptionStatus_GetState(Pointer);

        public VerificationResult<RenewalInfo> RenewalInfo
        {
            get
            {
                IntPtr ptr = Interop.SubscriptionStatus_GetRenewalInfo(Pointer);
                return new VerificationResult<RenewalInfo>(ptr);
            }
        }

        public VerificationResult<Transaction> Transaction
        {
            get
            {
                IntPtr ptr = Interop.SubscriptionStatus_GetTransaction(Pointer);
                return new VerificationResult<Transaction>(ptr);
            }
        }

        private static EventHandler<SubscriptionStatus> _updatesEventHandler;
        private static long _currentTaskId;

        /// <summary>Fires on subscription status changes (Product.SubscriptionInfo.Status.updates).</summary>
        public static event EventHandler<SubscriptionStatus> Updates
        {
            add
            {
                if (_updatesEventHandler == null) StartUpdates();
                _updatesEventHandler += value;
            }
            remove
            {
                _updatesEventHandler -= value;
                if (_updatesEventHandler == null)
                    InteropTasks.TrySetResultAndRemove(_currentTaskId, IntPtr.Zero);
            }
        }

        private static void StartUpdates()
        {
#if !UNITY_EDITOR
            InteropTasks.Create<IntPtr>(out _currentTaskId);
            Interop.SubscriptionStatus_Updates(_currentTaskId, OnStatusUpdate);
#endif
        }

        [MonoPInvokeCallback(typeof(SuccessTaskBoolReturningCallback<IntPtr>))]
        [return: MarshalAs(UnmanagedType.I1)]
        private static bool OnStatusUpdate(long taskId, IntPtr ptr)
        {
            if (!InteropTasks.TryGet<IntPtr>(taskId, out var task) || task.Task.IsCompleted)
                return false;
            _updatesEventHandler?.Invoke(null, new SubscriptionStatus(ptr));
            return true;
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void SubscriptionStatus_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int SubscriptionStatus_GetState(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr SubscriptionStatus_GetRenewalInfo(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr SubscriptionStatus_GetTransaction(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern void SubscriptionStatus_Updates(
                long taskId,
                SuccessTaskBoolReturningCallback<IntPtr> onUpdate);
        }
    }
}
