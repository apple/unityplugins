using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.StoreKit
{
    /// <summary>
    /// The App Store storefront for the device.
    /// Available on iOS 15.0+, macOS 12.0+, tvOS 15.0+
    /// </summary>
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public class Storefront : InteropReference
    {
        internal Storefront(IntPtr pointer) : base(pointer) { }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.Storefront_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        /// <summary>
        /// The App Store storefront identifier.
        /// </summary>
        public string Id => Interop.Storefront_GetId(Pointer);

        /// <summary>
        /// The three-letter country code for the storefront.
        /// </summary>
        public string CountryCode => Interop.Storefront_GetCountryCode(Pointer);

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnGetCurrentSuccess(long taskId, IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                InteropTasks.TrySetResultAndRemove<Storefront>(taskId, null);
            else
                InteropTasks.TrySetResultAndRemove(taskId, new Storefront(ptr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnGetCurrentError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<Storefront>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Gets the current App Store storefront.
        /// Returns null if not available.
        /// </summary>
        public static Task<Storefront> GetCurrent()
        {
#if UNITY_EDITOR
            return Task.FromResult<Storefront>(null);
#else
            var tcs = InteropTasks.Create<Storefront>(out var taskId);
            Interop.Storefront_GetCurrent(taskId, OnGetCurrentSuccess, OnGetCurrentError);
            return tcs.Task;
#endif
        }

        private static EventHandler<Storefront> _updatesEventHandler;
        private static long _currentTaskId;

        /// <summary>Fires when the device's App Store storefront changes (Storefront.updates).</summary>
        public static event EventHandler<Storefront> Updates
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
            Interop.Storefront_Updates(_currentTaskId, OnStorefrontUpdate);
#endif
        }

        [MonoPInvokeCallback(typeof(SuccessTaskBoolReturningCallback<IntPtr>))]
        [return: MarshalAs(UnmanagedType.I1)]
        private static bool OnStorefrontUpdate(long taskId, IntPtr ptr)
        {
            if (!InteropTasks.TryGet<IntPtr>(taskId, out var task) || task.Task.IsCompleted)
                return false;
            _updatesEventHandler?.Invoke(null, new Storefront(ptr));
            return true;
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void Storefront_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern void Storefront_GetCurrent(
                long taskId,
                SuccessTaskCallback<IntPtr> onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern void Storefront_Updates(
                long taskId,
                SuccessTaskBoolReturningCallback<IntPtr> onUpdate);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Storefront_GetId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Storefront_GetCountryCode(IntPtr pointer);
        }
    }
}
