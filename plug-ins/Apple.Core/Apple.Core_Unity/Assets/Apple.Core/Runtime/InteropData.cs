using System;
using System.Runtime.InteropServices;

namespace Apple.Core.Runtime
{
    /// <summary>
    /// Represents the Swift Data type, comparable
    /// to the NSData type from Objective-C.
    /// </summary>
    public struct InteropData
    {
        public IntPtr DataPtr;
        public int DataLength;

        public byte[] ToBytes()
        {
            var bytes = new byte[DataLength];
            Marshal.Copy(DataPtr, bytes, 0, DataLength);

            return bytes;
        }
    }
}
