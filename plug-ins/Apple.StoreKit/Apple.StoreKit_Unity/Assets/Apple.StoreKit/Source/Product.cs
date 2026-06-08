using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;
using UnityEngine;

namespace Apple.StoreKit
{
    /// <summary>
    /// Product type class matching StoreKit 2 Product.ProductType
    /// </summary>
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public class ProductType : InteropReference
    {
        internal ProductType(IntPtr pointer) : base(pointer) { }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.ProductType_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        public enum ProductTypeEnum
        {
            Consumable = 0,
            NonConsumable = 1,
            AutoRenewable = 2,
            NonRenewable = 3
        }

        /// <summary>
        /// The raw product type value
        /// </summary>
        public ProductTypeEnum Type => (ProductTypeEnum)Interop.ProductType_GetRawValue(Pointer);

        /// <summary>
        /// The localized description of the product type
        /// </summary>
         [Introduced(iOS: "15.4", macOS: "12.3", tvOS: "15.4", visionOS: "2.2")]
        public string LocalizedDescription => Interop.ProductType_GetLocalizedDescription(Pointer);

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void ProductType_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int ProductType_GetRawValue(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string ProductType_GetLocalizedDescription(IntPtr pointer);
        }
    }

    /// <summary>
    /// Subscription period unit matching StoreKit 2 Product.SubscriptionPeriod.Unit
    /// </summary>
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public enum SubscriptionPeriodUnit
    {
        Day = 0,
        Week = 1,
        Month = 2,
        Year = 3
    }

    /// <summary>
    /// Purchase result enumeration matching StoreKit 2 Product.PurchaseResult
    /// </summary>
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public class PurchaseResult : InteropReference
    {
        internal static readonly PurchaseResult UserCancelled = new PurchaseResult(IntPtr.Zero);
        internal static readonly PurchaseResult Pending = new PurchaseResult(IntPtr.Zero);
        internal PurchaseResult(IntPtr pointer) : base(pointer) { }

        protected override void OnDispose(bool isDisposing)
        {
            // Don't free the static sentinel values
            if (!ReferenceEquals(this, UserCancelled) && !ReferenceEquals(this, Pending) && Pointer != IntPtr.Zero)
            {
                Interop.PurchaseResult_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        public enum ResultEnum
        {
            Success,
            UserCancelled,
            Pending
        }

        public ResultEnum Result
        {
            get
            {
                if (ReferenceEquals(this, UserCancelled))
                    return ResultEnum.UserCancelled;
                if (ReferenceEquals(this, Pending))
                    return ResultEnum.Pending;

                // For actual PurchaseResult pointers, check the native type
                if (Pointer != IntPtr.Zero)
                {
                    int resultType = Interop.PurchaseResult_GetResultType(Pointer);
                    return resultType switch
                    {
                        0 => ResultEnum.Success,
                        1 => ResultEnum.UserCancelled,
                        2 => ResultEnum.Pending,
                        _ => ResultEnum.Success
                    };
                }

                return ResultEnum.Success;
            }
        }

        public VerificationResult<Transaction> TransactionVerification
        {
            get
            {
                if (Result != ResultEnum.Success)
                    throw new InvalidOperationException("Transaction verification is only available for successful purchases.");

                IntPtr verificationPointer = Interop.PurchaseResult_GetVerificationResult(Pointer);
                if (verificationPointer == IntPtr.Zero)
                    throw new InvalidOperationException("Failed to get verification result from purchase result.");

                return new VerificationResult<Transaction>(verificationPointer);
            }
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void PurchaseResult_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int PurchaseResult_GetResultType(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr PurchaseResult_GetVerificationResult(IntPtr pointer);
        }
    }


    /// <summary>
    /// Purchase option for product purchases
    /// </summary>
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public class PurchaseOption : InteropReference
    {
        internal PurchaseOption(IntPtr pointer) : base(pointer) { }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.PurchaseOption_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Create a purchase option with an app account token
        /// </summary>
        /// <param name="uuid">UUID string for the app account token</param>
        /// <returns>Purchase option or null if invalid UUID</returns>
        public static PurchaseOption AppAccountToken(string uuid)
        {
            IntPtr pointer = Interop.PurchaseOption_AppAccountToken(uuid);
            return pointer != IntPtr.Zero ? new PurchaseOption(pointer) : null;
        }

        /// <summary>
        /// Create a purchase option with quantity
        /// </summary>
        /// <param name="quantity">Number of consumable items to purchase</param>
        /// <returns>Purchase option</returns>
        public static PurchaseOption Quantity(int quantity)
        {
            IntPtr pointer = Interop.PurchaseOption_Quantity(quantity);
            return new PurchaseOption(pointer);
        }

        public static PurchaseOption SimulatesAskToBuyInSandbox(bool value)
        {
            IntPtr pointer = Interop.PurchaseOption_SimulatesAskToBuyInSandbox(value);
            return new PurchaseOption(pointer);
        }

        /// <summary>
        /// Returns a pair of purchase options encoding the promotional offer ID and its compact JWS signature.
        /// Both options must be passed together when purchasing.
        /// </summary>
        public static PurchaseOption[] PromotionalOffer(string offerID, string compactJWS)
        {
            return new[]
            {
                CustomString("adHocOfferId", offerID),
                CustomString("adHocSignature", System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(compactJWS)))
            };
        }

        [Introduced(iOS: "18.0", macOS: "15.0", tvOS: "18.0", visionOS: "2.0")]
        public static PurchaseOption WinBackOffer(SubscriptionOffer offer)
        {
            IntPtr pointer = Interop.PurchaseOption_WinBackOffer(offer.Pointer);
            return new PurchaseOption(pointer);
        }

        public static PurchaseOption IntroductoryOfferEligibility(string compactJWS)
        {
            return CustomString("introOfferEligibilityParam", compactJWS);
        }

        public static PurchaseOption CustomString(string key, string value)
        {
            IntPtr pointer = Interop.PurchaseOption_CustomString(key, value);
            return new PurchaseOption(pointer);
        }

        public static PurchaseOption CustomDouble(string key, double value)
        {
            IntPtr pointer = Interop.PurchaseOption_CustomDouble(key, value);
            return new PurchaseOption(pointer);
        }

        public static PurchaseOption CustomBool(string key, bool value)
        {
            IntPtr pointer = Interop.PurchaseOption_CustomBool(key, value);
            return new PurchaseOption(pointer);
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void PurchaseOption_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr PurchaseOption_AppAccountToken(string uuidString);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr PurchaseOption_Quantity(int quantity);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr PurchaseOption_SimulatesAskToBuyInSandbox([MarshalAs(UnmanagedType.I1)] bool value);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr PurchaseOption_WinBackOffer(IntPtr offerPointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr PurchaseOption_CustomString(string key, string value);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr PurchaseOption_CustomDouble(string key, double value);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr PurchaseOption_CustomBool(string key, [MarshalAs(UnmanagedType.I1)] bool value);
        }
    }

    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public enum SubscriptionOfferPaymentMode
    {
        FreeTrial = 0,
        PayAsYouGo = 1,
        PayUpFront = 2
    }

    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public class SubscriptionOffer : InteropReference
    {
        internal SubscriptionOffer(IntPtr pointer) : base(pointer) { }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.SubscriptionOffer_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        public string Id => Interop.SubscriptionOffer_GetId(Pointer);

        public string DisplayPrice => Interop.SubscriptionOffer_GetDisplayPrice(Pointer);
        public SubscriptionPeriodUnit PeriodUnit => (SubscriptionPeriodUnit)Interop.SubscriptionOffer_GetPeriodUnit(Pointer);
        public int PeriodValue => Interop.SubscriptionOffer_GetPeriodValue(Pointer);
        public int PeriodCount => Interop.SubscriptionOffer_GetPeriodCount(Pointer);
        public SubscriptionOfferPaymentMode PaymentMode => (SubscriptionOfferPaymentMode)Interop.SubscriptionOffer_GetPaymentMode(Pointer);

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void SubscriptionOffer_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string SubscriptionOffer_GetId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string SubscriptionOffer_GetDisplayPrice(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int SubscriptionOffer_GetPeriodUnit(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int SubscriptionOffer_GetPeriodValue(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int SubscriptionOffer_GetPeriodCount(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int SubscriptionOffer_GetPaymentMode(IntPtr pointer);
        }
    }

    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public class SubscriptionInfo : InteropReference
    {
        internal SubscriptionInfo(IntPtr pointer) : base(pointer) { }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.SubscriptionInfo_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        public string SubscriptionGroupId => Interop.SubscriptionInfo_GetSubscriptionGroupId(Pointer);

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<bool>))]
        private static void OnIsEligibleForIntroOfferSuccess(long taskId, bool result)
        {
            InteropTasks.TrySetResultAndRemove<bool>(taskId, result);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnIsEligibleForIntroOfferError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<bool>(taskId, new StoreKitException(errorPointer));
        }

        public Task<bool> IsEligibleForIntroOffer()
        {
#if UNITY_EDITOR
            return Task.FromResult(false);
#else
            var tcs = InteropTasks.Create<bool>(out var taskId);
            Interop.SubscriptionInfo_GetIsEligibleForIntroOffer(Pointer, taskId, OnIsEligibleForIntroOfferSuccess, OnIsEligibleForIntroOfferError);
            return tcs.Task;
#endif
        }

        public SubscriptionPeriodUnit PeriodUnit => (SubscriptionPeriodUnit)Interop.SubscriptionInfo_GetPeriodUnit(Pointer);
        public int PeriodValue => Interop.SubscriptionInfo_GetPeriodValue(Pointer);

        public SubscriptionOffer IntroductoryOffer
        {
            get
            {
                IntPtr ptr = Interop.SubscriptionInfo_GetIntroductoryOffer(Pointer);
                return ptr != IntPtr.Zero ? new SubscriptionOffer(ptr) : null;
            }
        }

        [Introduced(iOS: "18.0", macOS: "15.0", tvOS: "18.0", visionOS: "2.4")]
        public SubscriptionOffer[] WinBackOffers
        {
            get
            {
                int count = Interop.SubscriptionInfo_GetWinBackOffersCount(Pointer);
                if (count <= 0) return Array.Empty<SubscriptionOffer>();
                var offers = new SubscriptionOffer[count];
                for (int i = 0; i < count; i++)
                {
                    IntPtr ptr = Interop.SubscriptionInfo_GetWinBackOfferAt(Pointer, i);
                    offers[i] = ptr != IntPtr.Zero ? new SubscriptionOffer(ptr) : null;
                }
                return offers;
            }
        }

        [MonoPInvokeCallback(typeof(SuccessTaskArrayCallback))]
        private static void OnGetStatusSuccess(long taskId, IntPtr ptr, int count)
        {
            var statuses = new SubscriptionStatus[count];
            for (int i = 0; i < count; i++)
            {
                var itemPtr = Marshal.ReadIntPtr(ptr, i * IntPtr.Size);
                statuses[i] = new SubscriptionStatus(itemPtr);
            }
            InteropTasks.TrySetResultAndRemove(taskId, statuses);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnGetStatusError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<SubscriptionStatus[]>(taskId, new StoreKitException(errorPointer));
        }

        public Task<SubscriptionStatus[]> GetStatus()
        {
#if UNITY_EDITOR
            return Task.FromResult(Array.Empty<SubscriptionStatus>());
#else
            var tcs = InteropTasks.Create<SubscriptionStatus[]>(out var taskId);
            Interop.SubscriptionInfo_GetStatus(Pointer, taskId, OnGetStatusSuccess, OnGetStatusError);
            return tcs.Task;
#endif
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void SubscriptionInfo_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string SubscriptionInfo_GetSubscriptionGroupId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern void SubscriptionInfo_GetIsEligibleForIntroOffer(IntPtr pointer, long taskId, SuccessTaskCallback<bool> onSuccess, NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern int SubscriptionInfo_GetPeriodUnit(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int SubscriptionInfo_GetPeriodValue(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr SubscriptionInfo_GetIntroductoryOffer(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int SubscriptionInfo_GetWinBackOffersCount(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr SubscriptionInfo_GetWinBackOfferAt(IntPtr pointer, int index);

            [DllImport(InteropUtility.DLLName)]
            public static extern void SubscriptionInfo_GetStatus(IntPtr pointer, long taskId, SuccessTaskArrayCallback onSuccess, NSErrorTaskCallback onError);
        }
    }

    /// <summary>
    /// Information about a product registered in App Store Connect (StoreKit 2).
    /// Available on iOS 15.0+, macOS 12.0+, tvOS 15.0+
    /// </summary>
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public class Product : InteropReference
    {
        internal Product(IntPtr pointer) : base(pointer) { }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.Product_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        #region Properties
        /// <summary>
        /// The unique product identifier.
        /// </summary>
        public string Id => Interop.Product_GetId(Pointer);

        /// <summary>
        /// The localized display name of the product.
        /// </summary>
        public string DisplayName => Interop.Product_GetDisplayName(Pointer);

        /// <summary>
        /// The localized description of the product.
        /// </summary>
        public string Description => Interop.Product_GetDescription(Pointer);

        /// <summary>
        /// The price of the product in the local currency.
        /// </summary>
        public double Price => Interop.Product_GetPrice(Pointer);

        /// <summary>
        /// The localized string representation of the product price.
        /// </summary>
        public string DisplayPrice => Interop.Product_GetDisplayPrice(Pointer);

        /// <summary>
        /// The type of the product (consumable, non-consumable, auto-renewable, non-renewable).
        /// </summary>
        public ProductType Type
        {
            get
            {
                IntPtr typePointer = Interop.Product_GetType(Pointer);
                return typePointer != IntPtr.Zero ? new ProductType(typePointer) : null;
            }
        }

        /// <summary>
        /// A Boolean value that indicates whether the product is available for family sharing.
        /// </summary>
        public bool IsFamilyShareable => Interop.Product_GetIsFamilyShareable(Pointer);

        /// <summary>
        /// The JSON representation of the product.
        /// </summary>
        public string JsonRepresentation => Interop.Product_GetJsonRepresentation(Pointer);
        #endregion

        #region Subscription Properties
        /// <summary>
        /// A Boolean value that indicates whether the product has subscription information.
        /// </summary>
        public bool HasSubscription => Interop.Product_GetHasSubscription(Pointer);

        /// <summary>
        /// The subscription group identifier, if this product is a subscription.
        /// Returns null if the product is not a subscription.
        /// </summary>
        public string SubscriptionGroupId => Interop.Product_GetSubscriptionGroupId(Pointer);

        /// <summary>
        /// The unit of the subscription period (day, week, month, year).
        /// Returns -1 if the product is not a subscription.
        /// </summary>
        public SubscriptionPeriodUnit SubscriptionPeriodUnit => (SubscriptionPeriodUnit)Interop.Product_GetSubscriptionPeriodUnit(Pointer);

        /// <summary>
        /// The value of the subscription period (e.g., 1 for monthly, 3 for quarterly).
        /// Returns 0 if the product is not a subscription.
        /// </summary>
        public int SubscriptionPeriodValue => Interop.Product_GetSubscriptionPeriodValue(Pointer);

        /// <summary>
        /// Full subscription information object. Returns null for non-subscription products.
        /// </summary>
        public SubscriptionInfo Subscription
        {
            get
            {
                IntPtr ptr = Interop.Product_GetSubscriptionInfo(Pointer);
                return ptr != IntPtr.Zero ? new SubscriptionInfo(ptr) : null;
            }
        }
        #endregion

        #region Instance Methods
        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnPurchaseSuccess(long taskId, IntPtr purchaseResultPointer)
        {
            if (purchaseResultPointer == IntPtr.Zero)
            {
                // Null pointer could indicate user cancellation or other edge case
                InteropTasks.TrySetResultAndRemove(taskId, PurchaseResult.UserCancelled);
                return;
            }

            var purchaseResult = new PurchaseResult(purchaseResultPointer);
            InteropTasks.TrySetResultAndRemove(taskId, purchaseResult);
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnPurchaseError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<PurchaseResult>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Purchase the product with optional purchase options
        /// </summary>
        /// <param name="options">Array of purchase options (optional)</param>
        /// <returns>Task that resolves to a PurchaseResult</returns>
        public Task<PurchaseResult> Purchase(PurchaseOption[] options = null)
        {
            IntPtr optionsPointer = IntPtr.Zero;
            GCHandle optionsHandle = default;
            Action cleanup = null;

            if (options != null && options.Length > 0)
            {
                IntPtr[] optionPointers = new IntPtr[options.Length];
                for (int i = 0; i < options.Length; i++)
                {
                    optionPointers[i] = options[i].Pointer;
                }

                optionsHandle = GCHandle.Alloc(optionPointers, GCHandleType.Pinned);
                optionsPointer = optionsHandle.AddrOfPinnedObject();

                cleanup = () => optionsHandle.Free();
            }

            var tcs = InteropTasks.Create<PurchaseResult>(cleanup, out var taskId);
            Interop.Product_Purchase(Pointer, optionsPointer, options?.Length ?? 0, taskId, OnPurchaseSuccess, OnPurchaseError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnGetLatestTransactionSuccess(long taskId, IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                InteropTasks.TrySetResultAndRemove<VerificationResult<Transaction>>(taskId, null);
            else
                InteropTasks.TrySetResultAndRemove(taskId, new VerificationResult<Transaction>(ptr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnGetLatestTransactionError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<VerificationResult<Transaction>>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Gets the most recent transaction for this product.
        /// Returns null if the product has never been purchased.
        /// </summary>
        public Task<VerificationResult<Transaction>> GetLatestTransaction()
        {
            var tcs = InteropTasks.Create<VerificationResult<Transaction>>(out var taskId);
            Interop.Product_GetLatestTransaction(Pointer, taskId, OnGetLatestTransactionSuccess, OnGetLatestTransactionError);
            return tcs.Task;
        }
        #endregion

        #region Static Methods
        [MonoPInvokeCallback(typeof(SuccessTaskArrayCallback))]
        private static void OnFetchProductsSuccess(long taskId, IntPtr productsPointer, int count)
        {
            Debug.Log($"OnFetchProductsSuccess called with pointer: {productsPointer}, count: {count}");
            InteropTasks.TrySetResultAndRemove(taskId, new SwiftArray<Product>(productsPointer, count));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnFetchProductsError(long taskId, IntPtr errorPointer)
        {
            Debug.Log("OnFetchProductsError called with error pointer: " + errorPointer);
            InteropTasks.TrySetExceptionAndRemove<SwiftArray<Product>>(taskId, new StoreKitException(errorPointer));
        }

        /// <summary>
        /// Fetches information about products from the App Store.
        /// </summary>
        /// <param name="productIds">Array of product IDs to fetch</param>
        /// <returns>A task that completes when products are fetched</returns>
        public static Task<SwiftArray<Product>> FetchProducts(string[] productIds)
        {
            // Marshal each string to a char* pointer
            IntPtr[] stringPointers = new IntPtr[productIds.Length];
            for (int i = 0; i < productIds.Length; i++)
            {
                stringPointers[i] = Marshal.StringToHGlobalAnsi(productIds[i]);
            }

            // Pin the array of pointers
            GCHandle handle = GCHandle.Alloc(stringPointers, GCHandleType.Pinned);
            IntPtr arrayPtr = handle.AddrOfPinnedObject();

            Action cleanup = () =>
            {
                // Free marshalled strings
                for (int i = 0; i < stringPointers.Length; i++)
                {
                    Marshal.FreeHGlobal(stringPointers[i]);
                }
                handle.Free();
            };

            var tcs = InteropTasks.Create<SwiftArray<Product>>(cleanup, out var taskId);
            Interop.Product_FetchProducts(
                arrayPtr,
                productIds.Length,
                taskId,
                OnFetchProductsSuccess,
                OnFetchProductsError);

            return tcs.Task;
        }
        #endregion

        #region Interop
        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void Product_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Product_GetId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Product_GetDisplayName(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Product_GetDescription(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern double Product_GetPrice(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Product_GetDisplayPrice(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr Product_GetType(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool Product_GetIsFamilyShareable(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Product_GetJsonRepresentation(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool Product_GetHasSubscription(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Product_GetSubscriptionGroupId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int Product_GetSubscriptionPeriodUnit(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int Product_GetSubscriptionPeriodValue(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr Product_GetSubscriptionInfo(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern void Product_FetchProducts(
                IntPtr productIds,
                int productIdsCount,
                long taskId,
                SuccessTaskArrayCallback onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern void Product_Purchase(
                IntPtr productPointer,
                IntPtr optionsPointer,
                int optionsCount,
                long taskId,
                SuccessTaskCallback<IntPtr> onSuccess,
                NSErrorTaskCallback onError);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr PurchaseOption_AppAccountToken(string uuidString);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr PurchaseOption_Quantity(int quantity);

            [DllImport(InteropUtility.DLLName)]
            public static extern void Product_GetLatestTransaction(
                IntPtr productPointer,
                long taskId,
                SuccessTaskCallback<IntPtr> onSuccess,
                NSErrorTaskCallback onError);
        }
        #endregion
    }
}
