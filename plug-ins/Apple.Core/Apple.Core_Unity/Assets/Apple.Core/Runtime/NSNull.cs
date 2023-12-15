using System;
using System.Runtime.InteropServices;

namespace Apple.Core.Runtime
{
    /// <summary>
    /// C# wrapper around NSNull.
    /// </summary>
	public class NSNull : NSObject
	{
        /// <summary>
        /// Construct a new NSNull instance.
        /// </summary>
        public NSNull() : base(Interop.NSNull_Null()) { }

        /// <summary>
        /// Construct an NSNull wrapper around an existing instance.
        /// </summary>
        /// <param name="pointer"></param>
        internal NSNull(IntPtr pointer) : base(pointer) { }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr NSNull_Null();
        }
    }
}
