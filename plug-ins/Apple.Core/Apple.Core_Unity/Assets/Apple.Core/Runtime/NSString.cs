using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace Apple.Core.Runtime
{
    /// <summary>
    /// C# wrapper around NSString.
    /// </summary>
    public class NSString : NSObject
    {
        /// <summary>
        /// Construct an NSString wrapper around an existing instance.
        /// </summary>
        /// <param name="nsStringPtr"></param>
        public NSString(IntPtr nsStringPtr) : base(nsStringPtr) { }

        /// <summary>
        /// Construct an empty NSString.
        /// </summary>
        [Preserve]
        public NSString() : this(Interop.NSString_string()) { }

        /// <summary>
        /// Construct an NSString around a C# string.
        /// </summary>
        /// <param name="s"></param>
        public NSString(string s) : this(Interop.NSString_StringWithUtf8String(s)) { }

        private static readonly NSString _empty = new NSString();
        public static NSString Empty => _empty;

        /// <summary>
        /// Convert an NSString to a C# string.
        /// </summary>
        public string Utf8String => Interop.NSString_Utf8String(Pointer);
        public override string ToString() => Utf8String;
        public static implicit operator string(NSString s) => s.Utf8String;

        /// <summary>
        /// Convert a C# string to an NSString.
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator NSString(string s) => new NSString(s);

        /// <summary>
        /// Construct an NSString from UTF8 formatted NSData.
        /// </summary>
        /// <param name="data"></param>
        public NSString(NSData data) : this(Interop.NSString_StringWithUtf8Data(data.Pointer)) { }

        /// <summary>
        /// Convert a C# string to UTF8 formatted NSData.
        /// </summary>
        public NSData Utf8Data => PointerCast<NSData>(Interop.NSString_Utf8Data(Pointer));
        public static explicit operator NSData(NSString s) => s.Utf8Data;

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSString_string();
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSString_StringWithUtf8String(string s);
            [DllImport(InteropUtility.DLLName)] public static extern string NSString_Utf8String(IntPtr nsStringPtr);
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSString_StringWithUtf8Data(IntPtr nsDataPtr);
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSString_Utf8Data(IntPtr nsStringPtr);
        }
    }
}
