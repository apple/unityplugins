using System;
using System.Runtime.InteropServices;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.StoreKit
{
    [Introduced(iOS: "16.4", macOS: "14.4")]
    [Unavailable(RuntimeOperatingSystem.tvOS, RuntimeOperatingSystem.visionOS)]
    public class PurchaseIntent : InteropReference
    {
        internal PurchaseIntent(IntPtr pointer) : base(pointer) { }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
#if !UNITY_EDITOR
                Interop.PurchaseIntent_Free(Pointer);
#endif
                Pointer = IntPtr.Zero;
            }
        }

        public string Id
        {
            get
            {
#if UNITY_EDITOR
                return string.Empty;
#else
                return Interop.PurchaseIntent_GetId(Pointer);
#endif
            }
        }

        public Product Product
        {
            get
            {
#if UNITY_EDITOR
                return null;
#else
                IntPtr ptr = Interop.PurchaseIntent_GetProduct(Pointer);
                return ptr != IntPtr.Zero ? new Product(ptr) : null;
#endif
            }
        }

        // --- Static stream ---

        private static EventHandler<PurchaseIntent> _intentsHandler;
        private static long _currentTaskId;

        /// <summary>
        /// Subscribe to this event to receive purchase intents when a user taps
        /// "Buy" on a promoted IAP in the App Store.
        /// </summary>
        public static event EventHandler<PurchaseIntent> Intents
        {
            add
            {
                if (_intentsHandler == null)
                    StartListening();
                _intentsHandler += value;
            }
            remove
            {
                _intentsHandler -= value;
                if (_intentsHandler == null)
                    InteropTasks.TrySetResultAndRemove(_currentTaskId, IntPtr.Zero);
            }
        }

        private static void StartListening()
        {
#if !UNITY_EDITOR
            InteropTasks.Create<IntPtr>(out _currentTaskId);
            Interop.PurchaseIntent_Listen(_currentTaskId, OnIntent);
#endif
        }

        [MonoPInvokeCallback(typeof(SuccessTaskBoolReturningCallback<IntPtr>))]
        [return: MarshalAs(UnmanagedType.I1)]
        private static bool OnIntent(long taskId, IntPtr ptr)
        {
            if (!InteropTasks.TryGet<IntPtr>(taskId, out var task) || task.Task.IsCompleted)
                return false;

            var intent = new PurchaseIntent(ptr);
            _intentsHandler?.Invoke(null, intent);
            return true;
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void PurchaseIntent_Listen(
                long taskId,
                SuccessTaskBoolReturningCallback<IntPtr> onIntent);

            [DllImport(InteropUtility.DLLName)]
            public static extern void PurchaseIntent_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string PurchaseIntent_GetId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr PurchaseIntent_GetProduct(IntPtr pointer);
        }
    }
}
