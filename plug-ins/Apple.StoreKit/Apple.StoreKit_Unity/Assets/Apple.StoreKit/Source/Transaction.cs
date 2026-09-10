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
    /// Offer type enumeration matching StoreKit 2 Transaction.OfferType
    /// </summary>
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public enum OfferType
    {
        Introductory = 0,
        Promotional = 1,

        [Introduced(iOS: "16.0", macOS: "13.0", tvOS: "16.0", visionOS: "2.2")]
        Code = 2,

        [Introduced(iOS: "17.4", macOS: "14.4", tvOS: "17.4", visionOS: "2.4")]
        WinBackOffer = 3
    }

    /// <summary>
    /// Environment enumeration matching StoreKit 2 AppStore.Environment
    /// </summary>
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public enum StoreEnvironment
    {
        Production = 0,
        Sandbox = 1,
        Xcode = 2
    }

    /// <summary>
    /// Ownership type enumeration matching StoreKit 2 Transaction.OwnershipType
    /// </summary>
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public enum OwnershipType
    {
        Purchased = 0,
        FamilyShared = 1
    }

    /// <summary>
    /// Platform enumeration matching StoreKit 2 AppStore.Platform
    /// </summary>
    [Introduced(iOS: "18.4", macOS: "15.4", tvOS: "18.4", visionOS: "2.4")]
    public enum AppStorePlatform
    {
        iOS = 0,
        macOS = 1,
        tvOS = 2,
        visionOS = 4,
        Unknown = 5
    }

    [Introduced(iOS: "26.4", macOS: "26.4", tvOS: "26.4", visionOS: "26.4")]
    public enum RevocationType
    {
        FamilyRevocation = 0,
        FullRefund = 1,
        ProratedRefund = 2
    }

    /// <summary>
    /// Refund request status enumeration matching StoreKit 2 RefundRequestStatus
    /// </summary>
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public enum RefundRequestStatus
    {
        Success = 0,
        UserCancelled = 1
    }

    /// <summary>
    /// Revocation reason class matching StoreKit 2 Transaction.RevocationReason
    /// </summary>
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public class RevocationReason : InteropReference
    {
        internal RevocationReason(IntPtr pointer) : base(pointer) { }

        public enum RevocationReasonEnum
        {
            DeveloperIssue = 0,
            Other = 1
        }

        /// <summary>
        /// The raw revocation reason value
        /// </summary>
        public RevocationReasonEnum Reason => (RevocationReasonEnum)Interop.RevocationReason_GetRawValue(Pointer);

        /// <summary>
        /// The localized description of the revocation reason
        /// </summary>
        public string LocalizedDescription => Interop.RevocationReason_GetLocalizedDescription(Pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.RevocationReason_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void RevocationReason_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int RevocationReason_GetRawValue(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string RevocationReason_GetLocalizedDescription(IntPtr pointer);
        }
    }

    /// <summary>
    /// The reason a transaction occurred.
    /// Available on iOS 17.0+, macOS 14.0+, tvOS 17.0+
    /// </summary>
    [Introduced(iOS: "17.0", macOS: "14.0", tvOS: "17.0", visionOS: "2.2")]
    public enum TransactionReason
    {
        Purchase = 0,
        Renewal = 1
    }

    /// <summary>
    /// Structured offer information for a transaction.
    /// Available on iOS 17.2+, macOS 14.2+, tvOS 17.2+
    /// </summary>
    [Introduced(iOS: "17.2", macOS: "14.2", tvOS: "17.2", visionOS: "2.2")]
    public class TransactionOffer : InteropReference
    {
        internal TransactionOffer(IntPtr pointer) : base(pointer) { }

        /// <summary>
        /// The offer identifier, or null if not set.
        /// </summary>
        public string Id => Interop.TransactionOffer_GetId(Pointer);

        /// <summary>
        /// The offer type (introductory, promotional, code, or win-back).
        /// </summary>
        public OfferType Type => (OfferType)Interop.TransactionOffer_GetType(Pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.TransactionOffer_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void TransactionOffer_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string TransactionOffer_GetId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int TransactionOffer_GetType(IntPtr pointer);
        }
    }

    /// <summary>
    /// Information about a transaction that was completed through the App Store.
    /// Available on iOS 15.0+, macOS 12.0+, tvOS 15.0+
    /// </summary>
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public class Transaction : InteropReference
    {
        internal Transaction(IntPtr pointer) : base(pointer) { }

        #region Properties
        /// <summary>
        /// The unique identifier for the transaction.
        /// </summary>
        public ulong Id => Interop.Transaction_GetId(Pointer);

        /// <summary>
        /// The identifier of the original transaction for auto-renewable subscriptions.
        /// </summary>
        public ulong OriginalId => Interop.Transaction_GetOriginalId(Pointer);

        /// <summary>
        /// The product identifier for the transaction.
        /// </summary>
        public string ProductId => Interop.Transaction_GetProductId(Pointer);

        /// <summary>
        /// The date the App Store charged the customer's account for a purchase.
        /// </summary>
        public DateTime PurchaseDate => DateTimeOffset.FromUnixTimeSeconds(Interop.Transaction_GetPurchaseDate(Pointer)).DateTime;

        /// <summary>
        /// The date of the original purchase, before a subscription renewal or restore.
        /// </summary>
        public DateTime OriginalPurchaseDate => DateTimeOffset.FromUnixTimeSeconds(Interop.Transaction_GetOriginalPurchaseDate(Pointer)).DateTime;

        /// <summary>
        /// The date that the App Store signed the transaction.
        /// </summary>
        public DateTime SignedDate => DateTimeOffset.FromUnixTimeSeconds(Interop.Transaction_GetSignedDate(Pointer)).DateTime;

        /// <summary>
        /// The expiration date for auto-renewable subscriptions. Returns null for non-subscription products.
        /// </summary>
        public DateTime? ExpirationDate
        {
            get
            {
                long timestamp = Interop.Transaction_GetExpirationDate(Pointer);
                return timestamp > 0 ? DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime : null;
            }
        }

        /// <summary>
        /// The date that the App Store refunded the transaction or revoked it from family sharing. Returns null if not revoked.
        /// </summary>
        public DateTime? RevocationDate
        {
            get
            {
                long timestamp = Interop.Transaction_GetRevocationDate(Pointer);
                return timestamp > 0 ? DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime : null;
            }
        }

        /// <summary>
        /// The reason for the revocation of the transaction. Returns null if not revoked.
        /// </summary>
        public RevocationReason RevocationReason
        {
            get
            {
                IntPtr reasonPointer = Interop.Transaction_GetRevocationReason(Pointer);
                return reasonPointer != IntPtr.Zero ? new RevocationReason(reasonPointer) : null;
            }
        }

        /// <summary>
        /// A Boolean value that indicates whether the auto-renewable subscription is subject to an upgrade.
        /// </summary>
        public bool IsUpgraded => Interop.Transaction_GetIsUpgraded(Pointer);

        /// <summary>
        /// The promotional offer identifier, if the customer redeemed a promotional offer.
        /// </summary>
        public string OfferId => Interop.Transaction_GetOfferId(Pointer);

        /// <summary>
        /// The type of promotional offer, if the customer redeemed one.
        /// </summary>
        public OfferType? OfferType
        {
            get
            {
                int value = Interop.Transaction_GetOfferType(Pointer);
                return value >= 0 ? (OfferType?)value : null;
            }
        }

        /// <summary>
        /// The server environment where the transaction was processed.
        /// </summary>
        public StoreEnvironment Environment => (StoreEnvironment)Interop.Transaction_GetEnvironment(Pointer);

        /// <summary>
        /// The ownership type of the transaction.
        /// </summary>
        public OwnershipType OwnershipType => (OwnershipType)Interop.Transaction_GetOwnershipType(Pointer);

        /// <summary>
        /// The UUID that associates the transaction with a user on your own service.
        /// </summary>
        public string AppAccountToken => Interop.Transaction_GetAppAccountToken(Pointer);

        /// <summary>
        /// The JSON representation of the transaction.
        /// </summary>
        public string JsonRepresentation => Interop.Transaction_GetJsonRepresentation(Pointer);

        /// <summary>
        /// The number of consumable products purchased.
        /// </summary>
        public int PurchasedQuantity => Interop.Transaction_GetPurchasedQuantity(Pointer);

        /// <summary>
        /// The product type for the transaction (consumable, non-consumable, auto-renewable, non-renewing).
        /// </summary>
        public ProductType.ProductTypeEnum ProductType => (ProductType.ProductTypeEnum)Interop.Transaction_GetProductType(Pointer);

        /// <summary>
        /// The bundle ID of the app that created the transaction.
        /// </summary>
        public string AppBundleId => Interop.Transaction_GetAppBundleId(Pointer);

        /// <summary>
        /// The subscription group identifier for auto-renewable subscriptions. Null for non-subscription products.
        /// </summary>
        public string SubscriptionGroupId => Interop.Transaction_GetSubscriptionGroupId(Pointer);

        /// <summary>
        /// A unique identifier used for server-side receipt reconciliation. May be null for some product types.
        /// </summary>
        public string WebOrderLineItemId => Interop.Transaction_GetWebOrderLineItemId(Pointer);

        /// <summary>
        /// The App Store storefront country code where the transaction occurred.
        /// </summary>
        [Unavailable(RuntimeOperatingSystem.visionOS)]
        public string StorefrontCountryCode => Interop.Transaction_GetStorefrontCountryCode(Pointer);

        /// <summary>
        /// The reason the transaction occurred: a purchase or an auto-renewal. Available on iOS 17.0+.
        /// Returns null on older OS versions.
        /// </summary>
        [Introduced(iOS: "17.0", macOS: "14.0", tvOS: "17.0", visionOS: "2.2")]
        public TransactionReason? Reason
        {
            get
            {
                int value = Interop.Transaction_GetReason(Pointer);
                return value >= 0 ? (TransactionReason?)value : null;
            }
        }

        /// <summary>
        /// The actual price the customer paid. Null for free products. Available on iOS 17.0+.
        /// </summary>
        [Introduced(iOS: "17.0", macOS: "14.0", tvOS: "17.0", visionOS: "2.2")]
        public double? Price
        {
            get
            {
                double value = Interop.Transaction_GetPrice(Pointer);
                return value >= 0 ? value : null;
            }
        }

        [Introduced(iOS: "16.0", macOS: "13.0", tvOS: "16.0", visionOS: "1.0")]
        public Currency Currency
        {
            get
            {
                IntPtr ptr = Interop.Transaction_GetCurrency(Pointer);
                return ptr != IntPtr.Zero ? new Currency(ptr) : null;
            }
        }

        /// <summary>
        /// Structured offer information if the customer redeemed an offer. Available on iOS 17.2+.
        /// </summary>
        [Introduced(iOS: "17.2", macOS: "14.2", tvOS: "17.2", visionOS: "2.2")]
        public TransactionOffer Offer
        {
            get
            {
                IntPtr ptr = Interop.Transaction_GetOffer(Pointer);
                return ptr != IntPtr.Zero ? new TransactionOffer(ptr) : null;
            }
        }

        /// <summary>
        /// The nonce used when computing the device verification hash. Available on iOS 16.4+.
        /// </summary>
        [Introduced(iOS: "16.4", macOS: "13.3", tvOS: "16.4", visionOS: "2.2")]
        public string DeviceVerificationNonce => Interop.Transaction_GetDeviceVerificationId(Pointer);

        [Introduced(iOS: "16.0", macOS: "13.0", tvOS: "16.0", visionOS: "1.0")]
        public string AppTransactionID => Interop.Transaction_GetAppTransactionID(Pointer);

        [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
        public double? RevocationPercentage
        {
            get
            {
                double value = Interop.Transaction_GetRevocationPercentage(Pointer);
                return value >= 0 ? value : null;
            }
        }

        [Introduced(iOS: "26.4", macOS: "26.4", tvOS: "26.4", visionOS: "26.4")]
        public RevocationType? RevocationType
        {
            get
            {
                int value = Interop.Transaction_GetRevocationType(Pointer);
                return value >= 0 ? (RevocationType?)value : null;
            }
        }

        private static EventHandler<VerificationResult<Transaction>> _updatesEventHandler;
        private static long _currentTaskId;
        public static event EventHandler<VerificationResult<Transaction>> Updates
        {
            add
            {
                if (_updatesEventHandler == null)
                {
                    StartTransactionUpdates();
                }
                _updatesEventHandler += value;
            }
            remove
            {
                _updatesEventHandler -= value;
                if (_updatesEventHandler == null)
                {
                    Debug.Log("Transaction: No more update handlers, stopping updates is not currently supported.");
                    if (!InteropTasks.TrySetResultAndRemove(_currentTaskId, IntPtr.Zero))
                    {
                        Debug.LogWarning("Transaction: Failed to stop transaction updates task.");
                    }
                }
            }
        }

        private static void StartTransactionUpdates()
        {
            Debug.Log("Transaction: Starting transaction updates");
            InteropTasks.Create<IntPtr>(out _currentTaskId);
            Interop.Transaction_Updates(_currentTaskId, OnTransactionUpdate);
        }

        [MonoPInvokeCallback(typeof(SuccessTaskBoolReturningCallback<IntPtr>))]
        [return:MarshalAs(UnmanagedType.I1)]
        private static bool OnTransactionUpdate(long taskId, IntPtr ptr)
        {
            if (!InteropTasks.TryGet<IntPtr>(taskId, out var task)
                || task.Task.IsCompleted)
            {
                Debug.Log("Transaction: Received update for completed task, ignoring");
                return false;
            }

            Debug.Log("Update received for transaction");
            var result = new VerificationResult<Transaction>(ptr);
            _updatesEventHandler?.Invoke(null, result);
            return true;
        }

        #endregion

        #region Disposal
        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.Transaction_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"Transaction(Id={Id}, OriginalId={OriginalId}, ProductId={ProductId}, ProductType={ProductType}");
            sb.Append($", PurchasedQuantity={PurchasedQuantity}, PurchaseDate={PurchaseDate:yyyy-MM-dd HH:mm}");
            sb.Append($", AppBundleId={AppBundleId}");
#if !UNITY_VISIONOS
            sb.Append($", StorefrontCountryCode={StorefrontCountryCode}");
#endif
            if (SubscriptionGroupId != null) sb.Append($", SubscriptionGroupId={SubscriptionGroupId}");
            if (WebOrderLineItemId != null) sb.Append($", WebOrderLineItemId={WebOrderLineItemId}");
            if (ExpirationDate.HasValue) sb.Append($", ExpirationDate={ExpirationDate:yyyy-MM-dd HH:mm}");
            if (RevocationDate.HasValue) sb.Append($", RevocationDate={RevocationDate:yyyy-MM-dd HH:mm}");
            sb.Append($", IsUpgraded={IsUpgraded}, OwnershipType={OwnershipType}, Environment={Environment}");
            if (Reason.HasValue) sb.Append($", Reason={Reason}");
            if (Price.HasValue) sb.Append($", Price={Price} {Currency}");
            using var offer = Offer;
            if (offer != null) sb.Append($", Offer(Type={offer.Type}, Id={offer.Id ?? "null"})");
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Marks the transaction as finished after the app delivers the purchased content.
        /// </summary>
        public void Finish()
        {
            Interop.Transaction_Finish(Pointer);
        }
        #endregion

        #region AsyncSequence Methods

        /// <summary>
        /// Gets the current entitlements (active purchases and subscriptions) for the user.
        /// </summary>
        public static IAsyncEnumerable<VerificationResult<Transaction>> GetCurrentEntitlements()
        {
            var enumerable = AsyncStreamInterop.Create(static ptr => new VerificationResult<Transaction>(ptr));
            Interop.Transaction_GetCurrentEntitlements(enumerable.Id, AsyncStreamInterop.AddValue, AsyncStreamInterop.Completed);
            return enumerable;
        }

        /// <summary>
        /// Gets all transactions for the user, including past purchases.
        /// </summary>
        public static IAsyncEnumerable<VerificationResult<Transaction>> GetAll()
        {
            var enumerable = AsyncStreamInterop.Create(static ptr => new VerificationResult<Transaction>(ptr));
            Interop.Transaction_GetAll(enumerable.Id, AsyncStreamInterop.AddValue, AsyncStreamInterop.Completed);
            return enumerable;
        }

        public static IAsyncEnumerable<VerificationResult<Transaction>> GetUnfinished()
        {
            var enumerable = AsyncStreamInterop.Create(static ptr => new VerificationResult<Transaction>(ptr));
            Interop.Transaction_GetUnfinished(enumerable.Id, AsyncStreamInterop.AddValue, AsyncStreamInterop.Completed);
            return enumerable;
        }
        #endregion

        #region Async Methods
        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnGetLatestSuccess(long taskId, IntPtr ptr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, new VerificationResult<Transaction>(ptr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnGetLatestError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<VerificationResult<Transaction>>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Gets the most recent transaction for a specific product.
        /// </summary>
        public static Task<VerificationResult<Transaction>> GetLatest(string productId)
        {
            var tcs = InteropTasks.Create<VerificationResult<Transaction>>(out var taskId);
            Interop.Transaction_GetLatest(productId, taskId, OnGetLatestSuccess, OnGetLatestError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<int>))]
        private static void OnBeginRefundRequestSuccess(long taskId, int refundRequestStatus)
        {
            InteropTasks.TrySetResultAndRemove(taskId, (RefundRequestStatus)refundRequestStatus);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnBeginRefundRequestError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<RefundRequestStatus>(taskId, new StoreKitException(errorPointer));
        }

        [Unavailable(RuntimeOperatingSystem.tvOS)]
        public static Task<RefundRequestStatus> BeginRefundRequest(ulong transactionId)
        {
            var tcs = InteropTasks.Create<RefundRequestStatus>(out var taskId);
            Interop.Transaction_BeginRefundWithId(transactionId, taskId, OnBeginRefundRequestSuccess, OnBeginRefundRequestError);
            return tcs.Task;
        }

        [Unavailable(RuntimeOperatingSystem.tvOS)]
        public Task<RefundRequestStatus> BeginRefundRequest()
        {
            var tcs = InteropTasks.Create<RefundRequestStatus>(out var taskId);
            Interop.Transaction_BeginRefund(Pointer, taskId, OnBeginRefundRequestSuccess, OnBeginRefundRequestError);
            return tcs.Task;
        }
        #endregion

        #region Interop
        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void Transaction_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern ulong Transaction_GetId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern ulong Transaction_GetOriginalId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Transaction_GetProductId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern long Transaction_GetPurchaseDate(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern long Transaction_GetOriginalPurchaseDate(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern long Transaction_GetSignedDate(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern long Transaction_GetExpirationDate(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern long Transaction_GetRevocationDate(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr Transaction_GetRevocationReason(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool Transaction_GetIsUpgraded(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Transaction_GetOfferId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int Transaction_GetOfferType(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int Transaction_GetEnvironment(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int Transaction_GetOwnershipType(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Transaction_GetAppAccountToken(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Transaction_GetJsonRepresentation(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int Transaction_GetPurchasedQuantity(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int Transaction_GetProductType(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Transaction_GetAppBundleId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Transaction_GetSubscriptionGroupId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Transaction_GetWebOrderLineItemId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Transaction_GetStorefrontCountryCode(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int Transaction_GetReason(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern double Transaction_GetPrice(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr Transaction_GetCurrency(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr Transaction_GetOffer(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Transaction_GetDeviceVerificationId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Transaction_GetAppTransactionID(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern double Transaction_GetRevocationPercentage(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int Transaction_GetRevocationType(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern void Transaction_Finish(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern void Transaction_BeginRefund(
                IntPtr transactionPointer,
                long taskId,
                SuccessTaskCallback<int> onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern void Transaction_GetCurrentEntitlements(
                long sequenceId,
                SuccessTaskCallback<IntPtr> addItem,
                SuccessTaskCallback onComplete);

            [DllImport(InteropUtility.DLLName)]
            public static extern void Transaction_GetAll(
                long sequenceId,
                SuccessTaskCallback<IntPtr> addItem,
                SuccessTaskCallback onComplete);

            [DllImport(InteropUtility.DLLName)]
            public static extern void Transaction_GetUnfinished(
                long sequenceId,
                SuccessTaskCallback<IntPtr> addItem,
                SuccessTaskCallback onComplete);

            [DllImport(InteropUtility.DLLName)]
            public static extern void Transaction_GetLatest(
                string productId,
                long taskId,
                SuccessTaskCallback<IntPtr> onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern void Transaction_Updates(
                long taskId,
                SuccessTaskBoolReturningCallback<IntPtr> onUpdate
            );

            [DllImport(InteropUtility.DLLName)]
            public static extern void Transaction_BeginRefundWithId(
                ulong transactionId,
                long taskId,
                SuccessTaskCallback<int> onSuccess,
                NSErrorTaskCallback onError
            );
        }
        #endregion
    }
}
