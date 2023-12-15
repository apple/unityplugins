using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace Apple.Core.Runtime
{
    /// <summary>
    /// C# wrapper around NSMutableArray&lt;<typeparamref name="T"/>&gt;.
    /// </summary>
    /// <typeparam name="T">Type of values stored in the array. Must be a primitive number, string, NSObject or NSObject subclass.</typeparam>
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [Preserve]
    public class NSMutableArray<T> : NSArray<T>
    {
        /// <summary>
        /// Construct an empty NSMutableArray.
        /// </summary>
        public NSMutableArray() : base(NSMutableArrayInterop.NSMutableArray_Init()) { }

        /// <summary>
        /// Construct an NSMutableArray wrapper around an existing instance.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="isReadOnly">By default, NSMutableArray is writable. Set to false to make this instance read-only.</param>
        public NSMutableArray(IntPtr pointer, bool isReadOnly = false) : base(pointer)
        {
            IsReadOnly = isReadOnly;
        }

        #region IList<T>
        public new T this[int index]
        {
            get => base[index];
            set
            {
                ThrowIfReadOnly();

                if (index >= 0 && index < Count)
                {
                    NSMutableArrayInterop.NSMutableArray_ReplaceNSObjectAtIndex(Pointer, index, _boxer.Box(value).Pointer, NSException.ThrowOnExceptionCallback);
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public void Insert(int index, T item)
        {
            ThrowIfReadOnly();

            if (index >= 0 && index < Count)
            {
                NSMutableArrayInterop.NSMutableArray_InsertNSObjectAtIndex(Pointer, _boxer.Box(item).Pointer, index, NSException.ThrowOnExceptionCallback);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public void RemoveAt(int index)
        {
            ThrowIfReadOnly();

            if (index >= 0 && index < Count)
            {
                NSMutableArrayInterop.NSMutableArray_RemoveNSObjectAtIndex(Pointer, index, NSException.ThrowOnExceptionCallback);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        #region ICollection<T>
        public new bool IsReadOnly { get; }

        public void Add(T item)
        {
            ThrowIfReadOnly();
            NSMutableArrayInterop.NSMutableArray_AddNSObject(Pointer, _boxer.Box(item).Pointer, NSException.ThrowOnExceptionCallback);
        }

        public void Clear()
        {
            ThrowIfReadOnly();
            NSMutableArrayInterop.NSMutableArray_RemoveAllObjects(Pointer, NSException.ThrowOnExceptionCallback);
        }

        public bool Remove(T item)
        {
            ThrowIfReadOnly();

            bool didContainItem = Contains(item);

            NSMutableArrayInterop.NSMutableArray_RemoveNSObject(Pointer, _boxer.Box(item).Pointer, NSException.ThrowOnExceptionCallback);

            bool wasRemoved = didContainItem && !Contains(item);

            return wasRemoved;
        }
        #endregion

        /// <summary>
        /// Helps the debugger to visualize the collection.
        /// </summary>
        [Preserve]
        private T[] DebugView => this.ToArray();

        private void ThrowIfReadOnly()
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
        }
    }

    /// <summary>
    /// This NSMutableArray subclass is provided for backwards compatibility with earlier releases.
    /// </summary>
    [Obsolete("NSMutableArray is deprecated. Please use NSMutableArray<,> instead.")]
    public class NSMutableArray : NSMutableArray<NSObject>
    {
        public NSMutableArray() : base() { }
        public NSMutableArray(IntPtr pointer) : base(pointer) { }
    }

    /// <summary>
    /// DllImports used by NSMutableArray.
    /// </summary>
    internal static class NSMutableArrayInterop
    {
        [DllImport(InteropUtility.DLLName)]
        public static extern IntPtr NSMutableArray_Init();
        [DllImport(InteropUtility.DLLName)]
        public static extern void NSMutableArray_AddNSObject(IntPtr nsMutableArrayPtr, IntPtr nsObjectPtr, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern void NSMutableArray_InsertNSObjectAtIndex(IntPtr nsMutableArrayPtr, IntPtr nsObjectPtr, int atIndex, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern void NSMutableArray_RemoveNSObjectAtIndex(IntPtr nsMutableArrayPtr, int atIndex, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern void NSMutableArray_ReplaceNSObjectAtIndex(IntPtr nsMutableArrayPtr, int atIndex, IntPtr nsObjectPtr, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern void NSMutableArray_RemoveAllObjects(IntPtr nsMutableArrayPtr, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern void NSMutableArray_RemoveNSObject(IntPtr nsMutableArrayPtr, IntPtr nsObjectPtr, NSExceptionCallback onException);
    }
}
