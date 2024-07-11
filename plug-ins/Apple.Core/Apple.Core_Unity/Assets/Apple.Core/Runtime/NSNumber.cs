using System;
using System.Runtime.InteropServices;

namespace Apple.Core.Runtime
{
    /// <summary>
    /// C# wrapper around NSNumber.
    /// </summary>
    public class NSNumber : NSObject
    {
        #region Init
        /// <summary>
        /// Construct an NSNumber wrapper around an existing instance.
        /// </summary>
        /// <param name="nsNumberPtr"></param>
        public NSNumber(IntPtr nsNumberPtr) : base(nsNumberPtr) { }

        /// <summary>
        /// Construct a new NSNumber from any of the supported primitive number types.
        /// </summary>
        /// <param name="value"></param>
        public NSNumber(Boolean value) : base(Interop.NSNumber_NumberWithBool((SByte)(value ? 1 : 0))) { }
        public NSNumber(SByte value) : base(Interop.NSNumber_NumberWithChar(value)) { }
        public NSNumber(Double value) : base(Interop.NSNumber_NumberWithDouble(value)) { }
        public NSNumber(Single value) : base(Interop.NSNumber_NumberWithFloat(value)) { }
        public NSNumber(Int32 value) : base(Interop.NSNumber_NumberWithLong(value)) { }
        public NSNumber(Int64 value) : base(Interop.NSNumber_NumberWithLongLong(value)) { }
        public NSNumber(Int16 value) : base(Interop.NSNumber_NumberWithShort(value)) { }
        public NSNumber(Byte value) : base(Interop.NSNumber_NumberWithUnsignedChar(value)) { }
        public NSNumber(UInt32 value) : base(Interop.NSNumber_NumberWithUnsignedLong(value)) { }
        public NSNumber(UInt64 value) : base(Interop.NSNumber_NumberWithUnsignedLongLong(value)) { }
        public NSNumber(UInt16 value) : base(Interop.NSNumber_NumberWithUnsignedShort(value)) { }
        #endregion

        #region Payload Info

        /// <summary>
        /// Return the name of the Objective-C type of the primitive value contained in this NSNumber instance.
        /// </summary>
        public String ObjCType => Interop.NSNumber_ObjCType(Pointer);
        #endregion

        #region Accessors

        /// <summary>
        /// Convert and return the value contained in the NSNumber.
        /// </summary>
        public Boolean BoolValue => Interop.NSNumber_BoolValue(Pointer) != 0;
        public SByte CharValue => Interop.NSNumber_CharValue(Pointer);
        public Double DoubleValue => Interop.NSNumber_DoubleValue(Pointer);
        public Single FloatValue => Interop.NSNumber_FloatValue(Pointer);
        public Int32 IntValue => Interop.NSNumber_IntValue(Pointer);
        public Int32 LongValue => Interop.NSNumber_LongValue(Pointer);
        public Int64 LongLongValue => Interop.NSNumber_LongLongValue(Pointer);
        public Int16 ShortValue => Interop.NSNumber_ShortValue(Pointer);
        public Byte UnsignedCharValue => Interop.NSNumber_UnsignedCharValue(Pointer);
        public UInt32 UnsignedIntValue => Interop.NSNumber_UnsignedIntValue(Pointer);
        public UInt32 UnsignedLongValue => Interop.NSNumber_UnsignedLongValue(Pointer);
        public UInt64 UnsignedLongLongValue => Interop.NSNumber_UnsignedLongLongValue(Pointer);
        public UInt16 UnsignedShortValue => Interop.NSNumber_UnsignedShortValue(Pointer);
        #endregion

        #region Overloaded Accessors
        /// <summary>
        /// Convert and pass back the value contained in the NSNumber.
        /// </summary>
        public void GetValue(out Boolean value) => value = BoolValue;
        public void GetValue(out SByte value) => value = CharValue;
        public void GetValue(out Double value) => value = DoubleValue;
        public void GetValue(out Single value) => value = FloatValue;
        public void GetValue(out Int32 value) => value = IntValue;
        public void GetValue(out Int64 value) => value = LongLongValue;
        public void GetValue(out Int16 value) => value = ShortValue;
        public void GetValue(out Byte value) => value = UnsignedCharValue;
        public void GetValue(out UInt32 value) => value = UnsignedIntValue;
        public void GetValue(out UInt64 value) => value = UnsignedLongLongValue;
        public void GetValue(out UInt16 value) => value = UnsignedShortValue;
        #endregion

        #region Cast Operators
        /// <summary>
        /// Cast operators to convert and return the value contained in the NSNumber.
        /// </summary>
        public static explicit operator Boolean(NSNumber n) => n.BoolValue;
        public static explicit operator SByte(NSNumber n) => n.CharValue;
        public static explicit operator Double(NSNumber n) => n.DoubleValue;
        public static explicit operator Single(NSNumber n) => n.FloatValue;
        public static explicit operator Int32(NSNumber n) => n.LongValue;
        public static explicit operator Int64(NSNumber n) => n.LongLongValue;
        public static explicit operator Int16(NSNumber n) => n.ShortValue;
        public static explicit operator Byte(NSNumber n) => n.UnsignedCharValue;
        public static explicit operator UInt32(NSNumber n) => n.UnsignedLongValue;
        public static explicit operator UInt64(NSNumber n) => n.UnsignedLongLongValue;
        public static explicit operator UInt16(NSNumber n) => n.UnsignedShortValue;

        /// <summary>
        /// Cast operators to convert primitive numbers values to NSNumbers.
        /// </summary>
        public static implicit operator NSNumber(Boolean n) => new NSNumber(n);
        public static implicit operator NSNumber(SByte n) => new NSNumber(n);
        public static implicit operator NSNumber(Double n) => new NSNumber(n);
        public static implicit operator NSNumber(Single n) => new NSNumber(n);
        public static implicit operator NSNumber(Int32 n) => new NSNumber(n);
        public static implicit operator NSNumber(Int64 n) => new NSNumber(n);
        public static implicit operator NSNumber(Int16 n) => new NSNumber(n);
        public static implicit operator NSNumber(Byte n) => new NSNumber(n);
        public static implicit operator NSNumber(UInt32 n) => new NSNumber(n);
        public static implicit operator NSNumber(UInt64 n) => new NSNumber(n);
        public static implicit operator NSNumber(UInt16 n) => new NSNumber(n);
        #endregion

        #region String Conversions
        /// <summary>
        /// Convert the value contained in the NSNumber to a string.
        /// </summary>
        public String StringValue => Interop.NSNumber_StringValue(Pointer);
        public override String ToString() => StringValue;
        public static explicit operator String(NSNumber n) => n.StringValue;
        #endregion

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSNumber_NumberWithBool(SByte value);  // Objective-C BOOL type is actually signed 8-bit char
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSNumber_NumberWithChar(SByte value);  // Objective-C char is 8-bits
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSNumber_NumberWithDouble(Double value);
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSNumber_NumberWithFloat(Single value);
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSNumber_NumberWithInt(Int32 value);   // Objective-C int can be either 16- or 32-bits
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSNumber_NumberWithLong(Int32 value);
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSNumber_NumberWithLongLong(Int64 value);
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSNumber_NumberWithShort(Int16 value);
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSNumber_NumberWithUnsignedChar(Byte value);   // Objective-C unsigned char is 8-bits
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSNumber_NumberWithUnsignedInt(UInt32 value);  // Objective-C int can be either 16- or 32-bits
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSNumber_NumberWithUnsignedLong(UInt32 value);
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSNumber_NumberWithUnsignedLongLong(UInt64 value);
            [DllImport(InteropUtility.DLLName)] public static extern IntPtr NSNumber_NumberWithUnsignedShort(UInt16 value);

            [DllImport(InteropUtility.DLLName)] public static extern string NSNumber_ObjCType(IntPtr nsNumberPtr);

            [DllImport(InteropUtility.DLLName)] public static extern SByte NSNumber_BoolValue(IntPtr nsNumberPtr);
            [DllImport(InteropUtility.DLLName)] public static extern SByte NSNumber_CharValue(IntPtr nsNumberPtr);
            [DllImport(InteropUtility.DLLName)] public static extern Double NSNumber_DoubleValue(IntPtr nsNumberPtr);
            [DllImport(InteropUtility.DLLName)] public static extern Single NSNumber_FloatValue(IntPtr nsNumberPtr);
            [DllImport(InteropUtility.DLLName)] public static extern Int32 NSNumber_IntValue(IntPtr nsNumberPtr);
            [DllImport(InteropUtility.DLLName)] public static extern Int32 NSNumber_LongValue(IntPtr nsNumberPtr);
            [DllImport(InteropUtility.DLLName)] public static extern Int64 NSNumber_LongLongValue(IntPtr nsNumberPtr);
            [DllImport(InteropUtility.DLLName)] public static extern Int16 NSNumber_ShortValue(IntPtr nsNumberPtr);
            [DllImport(InteropUtility.DLLName)] public static extern Byte NSNumber_UnsignedCharValue(IntPtr nsNumberPtr);
            [DllImport(InteropUtility.DLLName)] public static extern UInt32 NSNumber_UnsignedIntValue(IntPtr nsNumberPtr);
            [DllImport(InteropUtility.DLLName)] public static extern UInt32 NSNumber_UnsignedLongValue(IntPtr nsNumberPtr);
            [DllImport(InteropUtility.DLLName)] public static extern UInt64 NSNumber_UnsignedLongLongValue(IntPtr nsNumberPtr);
            [DllImport(InteropUtility.DLLName)] public static extern UInt16 NSNumber_UnsignedShortValue(IntPtr nsNumberPtr);

            [DllImport(InteropUtility.DLLName)] public static extern String NSNumber_StringValue(IntPtr nsNumberPtr);
        }
    }
}
