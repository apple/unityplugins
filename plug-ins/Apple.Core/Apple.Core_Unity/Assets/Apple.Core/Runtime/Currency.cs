using System;
using System.Runtime.InteropServices;

namespace Apple.Core.Runtime
{
    [Introduced(iOS: "16.0", macOS: "13.0", tvOS: "16.0", visionOS: "1.0")]
    public class Currency : InteropReference
    {
        public Currency(IntPtr pointer) : base(pointer) { }

        public string Identifier => Interop.Currency_GetIdentifier(Pointer);

        public bool IsISOCurrency => Interop.Currency_GetIsISOCurrency(Pointer);

        public override string ToString() => Identifier;

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.Currency_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void Currency_Free(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            public static extern string Currency_GetIdentifier(IntPtr pointer);

            [DllImport(InteropUtility.DLLName)]
            [return: MarshalAs(UnmanagedType.U1)]
            public static extern bool Currency_GetIsISOCurrency(IntPtr pointer);
        }
    }
}
