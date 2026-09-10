using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.StoreKit
{
    /// <summary>
    /// Controls the per-device promoted IAP order and visibility.
    /// Available on iOS 16.4+
    /// </summary>
    [Introduced(iOS: "16.4")]
    [Unavailable(RuntimeOperatingSystem.tvOS, RuntimeOperatingSystem.macOS, RuntimeOperatingSystem.visionOS)]
    public static class ProductPromotionInfo
    {
        /// <summary>
        /// Visibility of a promoted IAP on this device.
        /// </summary>
        public enum Visibility
        {
            /// <summary>Uses the default visibility configured in App Store Connect.</summary>
            AppStoreConnectDefault = 0,
            /// <summary>Always show this product in the promoted list on this device.</summary>
            Visible = 1,
            /// <summary>Hide this product from the promoted list on this device.</summary>
            Hidden = 2
        }

        #region GetCurrentOrder

        [MonoPInvokeCallback(typeof(SuccessTaskArrayCallback))]
        private static void OnGetCurrentOrderSuccess(long taskId, IntPtr ptr, int count)
        {
            var result = new string[count];
            for (int i = 0; i < count; i++)
            {
                var strPtr = Marshal.ReadIntPtr(ptr, i * IntPtr.Size);
                result[i] = Marshal.PtrToStringAnsi(strPtr) ?? string.Empty;
            }
            InteropTasks.TrySetResultAndRemove(taskId, result);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnGetCurrentOrderError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<string[]>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Returns the current per-device promoted product order as an array of product IDs.
        /// </summary>
        public static Task<string[]> GetCurrentOrder()
        {
#if UNITY_EDITOR
            return Task.FromException<string[]>(new System.NotSupportedException("ProductPromotionInfo is not supported in the Unity Editor."));
#else
            var tcs = InteropTasks.Create<string[]>(out var taskId);
            Interop.ProductPromotionInfo_GetCurrentOrder(taskId, OnGetCurrentOrderSuccess, OnGetCurrentOrderError);
            return tcs.Task;
#endif
        }

        #endregion

        #region UpdateProductOrder

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnUpdateProductOrderSuccess(long taskId)
        {
            InteropTasks.TrySetResultAndRemove<bool>(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnUpdateProductOrderError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Sets a custom per-device promoted product order.
        /// </summary>
        public static Task UpdateProductOrder(string[] productIds)
        {
#if UNITY_EDITOR
            return Task.FromException(new System.NotSupportedException("ProductPromotionInfo is not supported in the Unity Editor."));
#else
            IntPtr[] ptrs = new IntPtr[productIds.Length];
            for (int i = 0; i < productIds.Length; i++)
                ptrs[i] = Marshal.StringToHGlobalAnsi(productIds[i]);

            var handle = GCHandle.Alloc(ptrs, GCHandleType.Pinned);
            var arrayPtr = handle.AddrOfPinnedObject();

            Action cleanup = () =>
            {
                for (int i = 0; i < ptrs.Length; i++)
                    Marshal.FreeHGlobal(ptrs[i]);
                handle.Free();
            };

            var tcs = InteropTasks.Create<bool>(cleanup, out var taskId);
            Interop.ProductPromotionInfo_UpdateProductOrder(arrayPtr, productIds.Length, taskId, OnUpdateProductOrderSuccess, OnUpdateProductOrderError);
            return tcs.Task;
#endif
        }

        #endregion

        #region UpdateAll

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnUpdateAllSuccess(long taskId)
        {
            InteropTasks.TrySetResultAndRemove<bool>(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnUpdateAllError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Sets the promoted IAP order and visibility for all products at once on this device.
        /// </summary>
        [Introduced(iOS: "16.4")]
        [Unavailable(RuntimeOperatingSystem.tvOS, RuntimeOperatingSystem.macOS, RuntimeOperatingSystem.visionOS)]
        public static Task UpdateAll(string[] productIds, Visibility[] visibilities)
        {
#if UNITY_EDITOR
            return Task.FromException(new System.NotSupportedException("ProductPromotionInfo is not supported in the Unity Editor."));
#else
            IntPtr[] ptrs = new IntPtr[productIds.Length];
            for (int i = 0; i < productIds.Length; i++)
                ptrs[i] = Marshal.StringToHGlobalAnsi(productIds[i]);

            var handle = GCHandle.Alloc(ptrs, GCHandleType.Pinned);
            var arrayPtr = handle.AddrOfPinnedObject();

            int[] visInts = new int[visibilities.Length];
            for (int i = 0; i < visibilities.Length; i++)
                visInts[i] = (int)visibilities[i];

            var visHandle = GCHandle.Alloc(visInts, GCHandleType.Pinned);
            var visArrayPtr = visHandle.AddrOfPinnedObject();

            Action cleanup = () =>
            {
                for (int i = 0; i < ptrs.Length; i++)
                    Marshal.FreeHGlobal(ptrs[i]);
                handle.Free();
                visHandle.Free();
            };

            var tcs = InteropTasks.Create<bool>(cleanup, out var taskId);
            Interop.ProductPromotionInfo_UpdateAll(arrayPtr, visArrayPtr, productIds.Length, taskId, OnUpdateAllSuccess, OnUpdateAllError);
            return tcs.Task;
#endif
        }

        #endregion

        #region UpdateProductVisibility

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnUpdateProductVisibilitySuccess(long taskId)
        {
            InteropTasks.TrySetResultAndRemove<bool>(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnUpdateProductVisibilityError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Sets the visibility of a specific product in the promoted list on this device.
        /// </summary>
        public static Task UpdateProductVisibility(string productId, Visibility visibility)
        {
#if UNITY_EDITOR
            return Task.FromException(new System.NotSupportedException("ProductPromotionInfo is not supported in the Unity Editor."));
#else
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.ProductPromotionInfo_UpdateProductVisibility(productId, (int)visibility, taskId, OnUpdateProductVisibilitySuccess, OnUpdateProductVisibilityError);
            return tcs.Task;
#endif
        }

        #endregion

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void ProductPromotionInfo_GetCurrentOrder(
                long taskId,
                SuccessTaskArrayCallback onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern void ProductPromotionInfo_UpdateProductOrder(
                IntPtr productIds,
                int count,
                long taskId,
                SuccessTaskCallback onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern void ProductPromotionInfo_UpdateAll(
                IntPtr productIds,
                IntPtr visibilities,
                int count,
                long taskId,
                SuccessTaskCallback onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern void ProductPromotionInfo_UpdateProductVisibility(
                string productId,
                int visibility,
                long taskId,
                SuccessTaskCallback onSuccess,
                NSErrorTaskCallback onError);
        }
    }
}
