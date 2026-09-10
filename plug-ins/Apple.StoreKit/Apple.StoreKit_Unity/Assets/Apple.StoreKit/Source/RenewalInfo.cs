using System;
using System.Runtime.InteropServices;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.StoreKit
{
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public enum RenewalState
    {
        Subscribed = 0,
        Expired = 1,
        InBillingRetryPeriod = 2,
        InGracePeriod = 3,
        Revoked = 4
    }

    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public enum ExpirationReason
    {
        AutoRenewDisabled = 0,
        BillingError = 1,
        DidNotConsentToPriceIncrease = 2,
        ProductUnavailable = 3
    }

    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public class RenewalInfo : InteropReference
    {
        internal RenewalInfo(IntPtr pointer) : base(pointer) { }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.RenewalInfo_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        public string CurrentProductId => Interop.RenewalInfo_GetCurrentProductID(Pointer);

        public bool WillAutoRenew => Interop.RenewalInfo_GetWillAutoRenew(Pointer);

        public string OfferId => Interop.RenewalInfo_GetOfferId(Pointer);

        public OfferType? OfferType
        {
            get
            {
                int v = Interop.RenewalInfo_GetOfferType(Pointer);
                return v >= 0 ? (OfferType?)v : null;
            }
        }

        public ExpirationReason? ExpirationReason
        {
            get
            {
                int v = Interop.RenewalInfo_GetExpirationReason(Pointer);
                return v >= 0 ? (ExpirationReason?)v : null;
            }
        }

        public bool IsInBillingRetry => Interop.RenewalInfo_GetIsInBillingRetry(Pointer);

        public DateTime? GracePeriodExpirationDate
        {
            get
            {
                long t = Interop.RenewalInfo_GetGracePeriodExpirationDate(Pointer);
                return t > 0 ? DateTimeOffset.FromUnixTimeSeconds(t).UtcDateTime : (DateTime?)null;
            }
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void RenewalInfo_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string RenewalInfo_GetCurrentProductID(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool RenewalInfo_GetWillAutoRenew(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string RenewalInfo_GetOfferId(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int RenewalInfo_GetOfferType(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int RenewalInfo_GetExpirationReason(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool RenewalInfo_GetIsInBillingRetry(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern long RenewalInfo_GetGracePeriodExpirationDate(IntPtr pointer);
        }
    }
}
