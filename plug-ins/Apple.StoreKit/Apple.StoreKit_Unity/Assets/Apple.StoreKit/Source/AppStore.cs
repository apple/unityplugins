using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.StoreKit
{
    public static class AppStore
    {
        #region Static Methods
        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnPresentOfferCodeRedeemSheetSuccess(long taskId)
        {
            InteropTasks.TrySetResultAndRemove<bool>(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnPresentOfferCodeRedeemSheetError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Presents a sheet for the user to redeem offer codes.
        /// </summary>
        /// <returns>Task that completes when the sheet is dismissed</returns>
        [Introduced(iOS: "16.0", macOS: "15.0", visionOS: "2.2")]
        [Unavailable(RuntimeOperatingSystem.tvOS)]
        public static Task PresentOfferCodeRedeemSheet()
        {
#if UNITY_EDITOR
            return Task.FromException(new System.NotSupportedException("PresentOfferCodeRedeemSheet is not supported in the Unity Editor."));
#else
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.AppStore_PresentOfferCodeRedeemSheet(taskId, OnPresentOfferCodeRedeemSheetSuccess, OnPresentOfferCodeRedeemSheetError);
            return tcs.Task;
#endif
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnShowManageSubscriptionsSuccess(long taskId)
        {
            InteropTasks.TrySetResultAndRemove<bool>(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnShowManageSubscriptionsError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Presents the manage subscriptions page in the App Store.
        /// </summary>
        /// <returns></returns>
        [Introduced(iOS: "17.0", visionOS: "2.2")]
        [Unavailable(RuntimeOperatingSystem.tvOS, RuntimeOperatingSystem.macOS)]
        public static Task ShowManageSubscriptions()
        {
#if UNITY_EDITOR
            return Task.FromException(new System.NotSupportedException("ShowManageSubscriptions is not supported in the Unity Editor."));
#else
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.AppStore_ShowManageSubscriptions(taskId, OnShowManageSubscriptionsSuccess, OnShowManageSubscriptionsError);
            return tcs.Task;
#endif
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnShowManageSubscriptionsForGroupSuccess(long taskId)
        {
            InteropTasks.TrySetResultAndRemove<bool>(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnShowManageSubscriptionsForGroupError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Presents the manage subscriptions page for a specific subscription group.
        /// </summary>
        [Introduced(iOS: "17.0", visionOS: "2.2")]
        [Unavailable(RuntimeOperatingSystem.tvOS, RuntimeOperatingSystem.macOS)]
        public static Task ShowManageSubscriptions(string subscriptionGroupId)
        {
#if UNITY_EDITOR
            return Task.FromException(new System.NotSupportedException("ShowManageSubscriptions is not supported in the Unity Editor."));
#else
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.AppStore_ShowManageSubscriptionsForGroup(subscriptionGroupId, taskId, OnShowManageSubscriptionsForGroupSuccess, OnShowManageSubscriptionsForGroupError);
            return tcs.Task;
#endif
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback))]
        private static void OnSyncSuccess(long taskId)
        {
            InteropTasks.TrySetResultAndRemove<bool>(taskId, true);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnSyncError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Initiates a sync with the App Store to restore missing transactions.
        /// </summary>
        [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
        public static Task Sync()
        {
#if UNITY_EDITOR
            return Task.FromException(new System.NotSupportedException("AppStore.Sync is not supported in the Unity Editor."));
#else
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.AppStore_Sync(taskId, OnSyncSuccess, OnSyncError);
            return tcs.Task;
#endif
        }

        /// <summary>
        /// Asks StoreKit to request an App Store review.
        /// Not available on tvOS.
        /// </summary>
        [Introduced(iOS: "16.0", macOS: "13.0", visionOS: "2.2")]
        [Unavailable(RuntimeOperatingSystem.tvOS)]
        public static void RequestReview()
        {
#if !UNITY_EDITOR
            Interop.AppStore_RequestReview();
#endif
        }

        /// <summary>
        /// Indicates whether the user is allowed to make payments.
        /// </summary>
        /// <returns>True if the user can make payments, otherwises false</returns>
        [Introduced(iOS: "16.0", macOS: "15.0", tvOS: "15.0", visionOS: "2.2")]
        public static bool CanMakePayments
        {
            get
            {
#if UNITY_EDITOR
                return false;
#else
                return Interop.AppStore_CanMakePayments();
#endif
            }
        }

        /// <summary>
        /// A UUID that uniquely identifies this device for StoreKit verification purposes.
        /// </summary>
        [Introduced(iOS: "16.4", macOS: "13.3", tvOS: "16.4", visionOS: "1.0")]
        public static string DeviceVerificationID
        {
            get
            {
#if UNITY_EDITOR
                return null;
#else
                return Interop.AppStore_GetDeviceVerificationID();
#endif
            }
        }
        #endregion

        #region Interop
        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void AppStore_PresentOfferCodeRedeemSheet(
                long taskId,
                SuccessTaskCallback onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern void AppStore_ShowManageSubscriptions(
                long taskId,
                SuccessTaskCallback onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern void AppStore_ShowManageSubscriptionsForGroup(
                string subscriptionGroupId,
                long taskId,
                SuccessTaskCallback onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool AppStore_CanMakePayments();

            [DllImport(InteropUtility.DLLName)]
            public static extern string AppStore_GetDeviceVerificationID();

            [DllImport(InteropUtility.DLLName)]
            public static extern void AppStore_Sync(
                long taskId,
                SuccessTaskCallback onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern void AppStore_RequestReview();
        }
        #endregion
    }
}
