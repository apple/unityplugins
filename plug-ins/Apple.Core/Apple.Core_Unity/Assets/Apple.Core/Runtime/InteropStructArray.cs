using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Apple.Core.Runtime
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InteropStructArray
    {
        public IntPtr Pointer;
        public int Length;

        public static InteropStructArray From<T>(T[] array, out GCHandle gcHandle) where T : struct
        {
            // Hopefully a struct type that supports alloc...
            gcHandle = GCHandle.Alloc(array, GCHandleType.Pinned);

            var interopArray = new InteropStructArray();
            interopArray.Pointer = gcHandle.AddrOfPinnedObject();
            interopArray.Length = array.Length;

            return interopArray;
        }
        
        public static InteropStructArray From<T>(List<T> list, out GCHandle gcHandle)  where T : struct
        {
            var array = list.ToArray();
            return From(array, out gcHandle);
        }

        /// <summary>
        /// Marshals the pointer of structures to
        /// a C# array of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] ToArray<T>()  where T : struct
        {
            if (Length <= 0)
                return new T[0];

            var elements = new T[Length];
            var size = Marshal.SizeOf<T>();
            var target = new IntPtr(Pointer.ToInt64());

            for (var i = 0; i < Length; i++)
            {
                elements[i] = Marshal.PtrToStructure<T>(target);
                target = new IntPtr(target.ToInt64() + size);
            }

            return elements;
        }
        
        /// <summary>
        /// Marshals the pointer of structures to
        /// a C# List<T>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ToList<T>()  where T : struct
        {
            if (Length <= 0)
                return new List<T>();

            var elements = new List<T>(Length);
            var size = Marshal.SizeOf<T>();
            var target = new IntPtr(Pointer.ToInt64());

            for (var i = 0; i < Length; i++)
            {
                elements[i] = Marshal.PtrToStructure<T>(target);
                target = new IntPtr(target.ToInt64() + size);
            }

            return elements;
        }
    }
}
