using System;
using System.Runtime.InteropServices;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.StoreKit
{
    [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "2.2")]
    public class SubscriptionStatus : InteropReference
    {
        internal SubscriptionStatus(IntPtr pointer) : base(pointer) { }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.SubscriptionStatus_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        public RenewalState State => (RenewalState)Interop.SubscriptionStatus_GetState(Pointer);

        public VerificationResult<RenewalInfo> RenewalInfo
        {
            get
            {
                IntPtr ptr = Interop.SubscriptionStatus_GetRenewalInfo(Pointer);
                return new VerificationResult<RenewalInfo>(ptr);
            }
        }

        public VerificationResult<Transaction> Transaction
        {
            get
            {
                IntPtr ptr = Interop.SubscriptionStatus_GetTransaction(Pointer);
                return new VerificationResult<Transaction>(ptr);
            }
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void SubscriptionStatus_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern int SubscriptionStatus_GetState(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr SubscriptionStatus_GetRenewalInfo(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr SubscriptionStatus_GetTransaction(IntPtr pointer);
        }
    }
}
