using System;
using System.Runtime.InteropServices;
using AOT;

namespace Apple.Core.Runtime
{
    /// <summary>
    /// C# wrapper around NSException
    /// </summary>
    public class NSException : NSObject
    {
        /// <summary>
        /// Construct an NSException wrapper around an existing instance.
        /// </summary>
        /// <param name="pointer"></param>
        public NSException(IntPtr pointer) : base (pointer) { }

        /// <summary>
        /// A string used to uniquely identify the receiver.
        /// </summary>
        public string Name => Interop.NSException_GetName(Pointer);

        /// <summary>
        /// A string containing a “human-readable” reason for the receiver.
        /// </summary>
        public string Reason => Interop.NSException_GetReason(Pointer);

        /// <summary>
        /// A dictionary containing application-specific data pertaining to the receiver.
        /// </summary>
        /// <remarks>Use .As&lt;NSDictionary&lt;TKey, TValue&gt;&gt; to convert to a specific dictionary type.</remarks>
        public NSObject UserInfo => PointerCast<NSObject>(Interop.NSException_GetUserInfo(Pointer));

        /// <summary>
        /// C# exception class thrown when an NSException is received from the Objective-C layer.
        /// </summary>
        public class Exception : System.Exception
        {
            public Exception(NSException nsException)
            {
                NSException = nsException;
            }

            public NSException NSException { get; }

            public override string Message => $"{NSException.Name}: {NSException.Reason}";
        }

        /// <summary>
        /// Throw a C# exception containing this NSExeption.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Throw()
        {
            throw new Exception(this);
        }

        /// <summary>
        /// Implementation of the NSExceptionCallback delegate that throws a C# exception passed back from Objective-C.
        /// </summary>
        /// <param name="nsExceptionPtr"></param>
        [MonoPInvokeCallback(typeof(NSExceptionCallback))]
        internal static void ThrowOnExceptionCallback(IntPtr nsExceptionPtr)
        {
            PointerCast<NSException>(nsExceptionPtr).Throw();
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern string NSException_GetName(IntPtr nsExceptionPtr);
            [DllImport(InteropUtility.DLLName)]
            public static extern string NSException_GetReason(IntPtr nsExceptionPtr);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr /*nsDictionaryPtr*/ NSException_GetUserInfo(IntPtr nsExceptionPtr);
        }
    }
}