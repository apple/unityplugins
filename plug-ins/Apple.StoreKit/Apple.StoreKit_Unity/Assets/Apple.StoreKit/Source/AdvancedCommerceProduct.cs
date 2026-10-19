using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.StoreKit
{
    [Introduced(iOS: "18.4", macOS: "15.4", tvOS: "18.4", visionOS: "2.4")]
    public class AdvancedCommerceProduct : InteropReference
    {
        internal AdvancedCommerceProduct(IntPtr pointer) : base(pointer) { }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.AdvancedCommerceProduct_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        public string Id => Interop.AdvancedCommerceProduct_GetId(Pointer);

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadSuccess(long taskId, IntPtr ptr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, ptr != IntPtr.Zero ? new AdvancedCommerceProduct(ptr) : null);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<AdvancedCommerceProduct>(taskId, new StoreKitException(errorPointer));
        }

        [Unavailable(RuntimeOperatingSystem.macOS)]
        public static Task<AdvancedCommerceProduct> Load(string productId)
        {
            var tcs = InteropTasks.Create<AdvancedCommerceProduct>(out var taskId);
            Interop.AdvancedCommerceProduct_Load(productId, taskId, OnLoadSuccess, OnLoadError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnPurchaseSuccess(long taskId, IntPtr ptr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, ptr != IntPtr.Zero ? new PurchaseResult(ptr) : PurchaseResult.UserCancelled);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnPurchaseError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<PurchaseResult>(taskId, new StoreKitException(errorPointer));
        }

        [Unavailable(RuntimeOperatingSystem.macOS)]
        public Task<PurchaseResult> Purchase(string compactJWS)
        {
            var tcs = InteropTasks.Create<PurchaseResult>(out var taskId);
            Interop.AdvancedCommerceProduct_Purchase(Pointer, compactJWS, taskId, OnPurchaseSuccess, OnPurchaseError);
            return tcs.Task;
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void AdvancedCommerceProduct_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string AdvancedCommerceProduct_GetId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern void AdvancedCommerceProduct_Load(
                string productId,
                long taskId,
                SuccessTaskCallback<IntPtr> onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern void AdvancedCommerceProduct_Purchase(
                IntPtr pointer,
                string compactJWS,
                long taskId,
                SuccessTaskCallback<IntPtr> onSuccess,
                NSErrorTaskCallback onError);
        }
    }
}
