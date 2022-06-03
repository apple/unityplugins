using System;
using System.Runtime.InteropServices;

namespace Apple.Core.Runtime
{
    public interface INSMutableArray<T> : INSArray<T>
    {
        void Add(T value);
    }

    public static class NSMutableArrayFactory
    {
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr NSMutableArray_Init();
        
        public static TBase Init<TBase, TElement>() where TBase : InteropReference, INSMutableArray<TElement>
        {
            return ReflectionUtility.CreateInstanceOrDefault<TBase>(NSMutableArray_Init());
        }
    }
    
    public class NSMutableArrayString : NSArrayString, INSMutableArray<string>
    {
        public NSMutableArrayString(IntPtr pointer) : base(pointer) {}

        [DllImport(InteropUtility.DLLName)]
        private static extern void NSMutableArray_AddString(IntPtr pointer, string value);
        
        public void Add(string value)
        {
            NSMutableArray_AddString(Pointer, value);
        }
    }

    public class NSMutableArrayBoolean : NSArrayBoolean, INSMutableArray<bool>
    {
        public NSMutableArrayBoolean(IntPtr pointer) : base(pointer) {}

        [DllImport(InteropUtility.DLLName)]
        private static extern void NSMutableArray_AddBoolean(IntPtr pointer, bool value);
        
        public void Add(bool value)
        {
            NSMutableArray_AddBoolean(Pointer, value);
        }
    }

    public class NSMutableArrayInt64 : NSArrayInt64, INSMutableArray<long>
    {
        public NSMutableArrayInt64(IntPtr pointer) : base(pointer)
        {
        }

        [DllImport(InteropUtility.DLLName)]
        private static extern void NSMutableArray_AddInt64(IntPtr pointer, long value);
        
        public void Add(long value)
        {
            NSMutableArray_AddInt64(Pointer, value);
        }
    }
}
