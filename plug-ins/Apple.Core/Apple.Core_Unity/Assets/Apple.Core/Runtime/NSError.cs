using System;
using System.Runtime.InteropServices;
using AOT;

namespace Apple.Core.Runtime
{
    public class NSError : NSObject
    {
        public NSError(IntPtr pointer) : base(pointer)
        {
        }

        public long Code => Interop.NSError_GetCode(Pointer);

        public string Domain => Interop.NSError_GetDomain(Pointer);

        public string LocalizedDescription => Interop.NSError_GetLocalizedDescription(Pointer);

        private NSDictionary<NSString, NSObject> _userInfo;

        public NSDictionary<NSString, NSObject> UserInfo
        {
            get
            {
                if (_userInfo == null)
                {
                    var pointer = Interop.NSError_GetUserInfo(Pointer);
                    
                    if (pointer != IntPtr.Zero)
                        _userInfo = new NSDictionary<NSString, NSObject>(pointer);
                }

                return _userInfo;
            }
        }

        /// <summary>
        /// C# exception class thrown when an NSError is received from the Objective-C layer.
        /// </summary>
        public class Exception : System.Exception
        {
            public Exception(NSError nsError)
            {
                NSError = nsError;
            }

            public NSError NSError { get; }

            public override string Message => $"{NSError.Domain} ({NSError.Code}): {NSError.LocalizedDescription}";
        }

        /// <summary>
        /// Throw a C# exception containing this NSError.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Throw()
        {
            throw new Exception(this);
        }

        /// <summary>
        /// Implementation of the NSErrorCallback delegate that throws a C# exception passed back from Objective-C.
        /// </summary>
        /// <param name="nsExceptionPtr"></param>
        [MonoPInvokeCallback(typeof(NSErrorCallback))]
        internal static void ThrowOnErrorCallback(IntPtr nsErrorPtr)
        {
            PointerCast<NSError>(nsErrorPtr).Throw();
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern long NSError_GetCode(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string NSError_GetDomain(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string NSError_GetLocalizedDescription(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr NSError_GetUserInfo(IntPtr pointer);
        }
    }
}
