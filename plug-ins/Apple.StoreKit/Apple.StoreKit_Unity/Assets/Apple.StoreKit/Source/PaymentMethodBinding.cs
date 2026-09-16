using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.StoreKit
{
    /// <summary>
    /// Binds a payment method via an in-app pinning id (PaymentMethodBinding). iOS only.
    /// </summary>
    [Introduced(iOS: "16.4")]
    [Unavailable(RuntimeOperatingSystem.macOS, RuntimeOperatingSystem.tvOS, RuntimeOperatingSystem.visionOS)]
    public static class PaymentMethodBinding
    {
        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnBindSuccess(long taskId)
            => InteropTasks.TrySetResultAndRemove<bool>(taskId, true);

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnBindError(long taskId, IntPtr errorPointer)
            => InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new StoreKitException(errorPointer));

        /// <summary>Creates a PaymentMethodBinding for the given in-app pinning id and binds it.</summary>
        public static Task Bind(string inAppPinningId)
        {
#if UNITY_EDITOR
            return Task.FromException(new NotSupportedException("PaymentMethodBinding is not supported in the Unity Editor."));
#elif UNITY_IOS
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.PaymentMethodBinding_Bind(inAppPinningId, taskId, OnBindSuccess, OnBindError);
            return tcs.Task;
#else
            return Task.FromException(new NotSupportedException("PaymentMethodBinding is available on iOS only."));
#endif
        }

        private static class Interop
        {
#if UNITY_IOS && !UNITY_EDITOR
            [DllImport(InteropUtility.DLLName)]
            public static extern void PaymentMethodBinding_Bind(
                string inAppPinningId,
                long taskId,
                SuccessTaskCallback onSuccess,
                NSErrorTaskCallback onError);
#endif
        }
    }
}
