using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.StoreKit
{
    /// <summary>
    /// Information about the app's transaction, proving it was legitimately purchased/downloaded from the App Store.
    /// Available on iOS 16.0+, macOS 13.0+, tvOS 16.0+
    /// </summary>
    [Introduced(iOS: "16.0", macOS: "13.0", tvOS: "16.0", visionOS: "2.2")]
    public class AppTransaction : NSObject
    {
        internal AppTransaction(IntPtr pointer) : base(pointer) { }

        #region Properties
        /// <summary>
        /// The version of the app that the user originally purchased.
        /// </summary>
        public string OriginalAppVersion => Interop.AppTransaction_GetOriginalAppVersion(Pointer);

        /// <summary>
        /// The date when the user first downloaded the app.
        /// </summary>
        public DateTime OriginalPurchaseDate => DateTimeOffset.FromUnixTimeSeconds(Interop.AppTransaction_GetOriginalPurchaseDate(Pointer)).DateTime;

        /// <summary>
        /// The date the App Store signed the app transaction.
        /// </summary>
        public DateTime SignedDate => DateTimeOffset.FromUnixTimeSeconds(Interop.AppTransaction_GetSignedDate(Pointer)).DateTime;

        /// <summary>
        /// The current version of the app.
        /// </summary>
        public string AppVersion => Interop.AppTransaction_GetAppVersion(Pointer);

        /// <summary>
        /// The number that the App Store uses to uniquely identify the version of the app.
        /// Returns null in sandbox and Xcode environments.
        /// </summary>
        public ulong? AppVersionId
        {
            get
            {
                ulong value = Interop.AppTransaction_GetAppVersionId(Pointer);
                return value > 0 ? value : null;
            }
        }

        /// <summary>
        /// The date when the user preordered the app. Returns null if the app wasn't preordered.
        /// Available on iOS 17.4+, macOS 14.4+, tvOS 17.4+
        /// </summary>
        [Introduced(iOS: "17.4", macOS: "14.4", tvOS: "17.4", visionOS: "2.2")]
        public DateTime? PreorderDate
        {
            get
            {
                long timestamp = Interop.AppTransaction_GetPreorderDate(Pointer);
                return timestamp > 0 ? DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime : null;
            }
        }

        /// <summary>
        /// The app's bundle identifier.
        /// </summary>
        public string BundleId => Interop.AppTransaction_GetBundleId(Pointer);

        /// <summary>
        /// The unique identifier the App Store uses to identify the app (Apple ID in App Store Connect).
        /// Returns null in sandbox and Xcode environments.
        /// </summary>
        public ulong? AppId
        {
            get
            {
                ulong value = Interop.AppTransaction_GetAppId(Pointer);
                return value > 0 ? value : null;
            }
        }

        /// <summary>
        /// The server environment where the app transaction was processed.
        /// </summary>
        public StoreEnvironment Environment => (StoreEnvironment)Interop.AppTransaction_GetEnvironment(Pointer);

        /// <summary>
        /// The JSON representation of the app transaction (signed JWS).
        /// </summary>
        public NSData JsonRepresentation
        {
            get
            {
                var data = Interop.AppTransaction_GetJsonRepresentation(Pointer);
                return new NSData(data.Pointer);
            }
        }

        /// <summary>
        /// Device verification data that proves the app transaction came from a specific device.
        /// Available on iOS 16.4+, macOS 13.3+, tvOS 16.4+
        /// </summary>
        [Introduced(iOS: "16.4", macOS: "13.3", tvOS: "16.4", visionOS: "2.2")]
        public NSData DeviceVerification
        {
            get
            {
                var data = Interop.AppTransaction_GetDeviceVerification(Pointer);
                return new NSData(data.Pointer);
            }
        }

        /// <summary>
        /// A unique identifier for the app download transaction.
        /// Available on iOS 16.0+, macOS 13.0+, tvOS 16.0+
        /// </summary>
        public string AppTransactionID => Interop.AppTransaction_GetAppTransactionID(Pointer);

        /// <summary>
        /// The platform on which the user originally purchased the app.
        /// Available on iOS 18.4+, macOS 15.4+, tvOS 18.4+, visionOS 2.4+
        /// </summary>
        [Introduced(iOS: "18.4", macOS: "15.4", tvOS: "18.4", visionOS: "2.4")]
        public AppStorePlatform OriginalPlatform => (AppStorePlatform)Interop.AppTransaction_GetOriginalPlatform(Pointer);

        /// <summary>
        /// A nonce used to verify the integrity of the device's information.
        /// Available on iOS 16.4+, macOS 13.3+, tvOS 16.4+
        /// </summary>
        [Introduced(iOS: "16.4", macOS: "13.3", tvOS: "16.4", visionOS: "2.2")]
        public string DeviceVerificationNonce => Interop.AppTransaction_GetDeviceVerificationNonce(Pointer);
        #endregion

        #region Disposal
        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.AppTransaction_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }
        #endregion

        #region Static Methods
        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnGetSharedSuccess(long taskId, IntPtr ptr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, new VerificationResult<AppTransaction>(ptr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnGetSharedError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<VerificationResult<AppTransaction>>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Gets the current app transaction, proving the app was legitimately purchased/downloaded.
        /// Returns a VerificationResult that can be checked for verification status and contains JWS properties.
        /// </summary>
        public static Task<VerificationResult<AppTransaction>> GetShared()
        {
            var tcs = InteropTasks.Create<VerificationResult<AppTransaction>>(out var taskId);
            Interop.AppTransaction_GetShared(taskId, OnGetSharedSuccess, OnGetSharedError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnRefreshSuccess(long taskId, IntPtr verificationResultPointer)
        {
            InteropTasks.TrySetResultAndRemove(taskId, new VerificationResult<AppTransaction>(verificationResultPointer));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnRefreshError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<VerificationResult<AppTransaction>>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Refreshes the app transaction to get the latest information.
        /// Returns a VerificationResult that can be checked for verification status and contains JWS properties.
        /// </summary>
        public static Task<VerificationResult<AppTransaction>> Refresh()
        {
            var tcs = InteropTasks.Create<VerificationResult<AppTransaction>>(out var taskId);
            Interop.AppTransaction_Refresh(taskId, OnRefreshSuccess, OnRefreshError);
            return tcs.Task;
        }
        #endregion

        #region Interop
        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void AppTransaction_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string AppTransaction_GetOriginalAppVersion(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern long AppTransaction_GetOriginalPurchaseDate(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern long AppTransaction_GetSignedDate(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string AppTransaction_GetAppVersion(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern ulong AppTransaction_GetAppVersionId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern long AppTransaction_GetPreorderDate(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string AppTransaction_GetBundleId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern ulong AppTransaction_GetAppId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int AppTransaction_GetEnvironment(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern NSData AppTransaction_GetJsonRepresentation(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern NSData AppTransaction_GetDeviceVerification(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string AppTransaction_GetDeviceVerificationNonce(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string AppTransaction_GetAppTransactionID(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int AppTransaction_GetOriginalPlatform(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern void AppTransaction_GetShared(
                long taskId,
                SuccessTaskCallback<IntPtr> onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern void AppTransaction_Refresh(
                long taskId,
                SuccessTaskCallback<IntPtr> onSuccess,
                NSErrorTaskCallback onError);
        }
        #endregion
    }
}
