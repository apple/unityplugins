using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace Apple.Core.Runtime
{
    /// <summary>
    /// C# wrapper around NSArray&lt;<typeparamref name="T"/>&gt;
    /// </summary>
    /// <typeparam name="T">Type of values stored in the array. Must be a primitive number, string, NSObject or NSObject subclass.</typeparam>
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [Preserve]
    public class NSArray<T> : NSObject, IReadOnlyList<T>, IList<T>
    {
        protected InteropBoxer.Boxer _boxer;

        /// <summary>
        /// Construct an NSArray wrapper around an existing instance.
        /// </summary>
        /// <param name="pointer"></param>
        /// <exception cref="NotSupportedException">Throws this exception type if <typeparamref name="T"/> is unsupported.</exception>
        public NSArray(IntPtr pointer) : base(pointer)
        {
            _boxer = InteropBoxer.LookupBoxer<T>();
            if (_boxer == null)
            {
                throw new NotSupportedException($"NSArray<T> does not support type T = {typeof(T).FullName}");
            }
        }

        /// <summary>
        /// Returns an NSArray from the given JSON string.
        /// </summary>
        /// <remarks>
        /// The JSON must conform to the limitations imposed by <see href="https://developer.apple.com/documentation/foundation/nsjsonserialization?language=objc">NSJSONSerialization</see>.
        /// </remarks>
        /// <param name="jsonString"></param>
        /// <returns>NSArray&lt;<typeparamref name="T"/>&gt;</returns>
        public static NSArray<T> FromJson(string jsonString)
        {
            return new NSArray<T>(NSArrayInterop.NSArray_FromJson(jsonString, NSError.ThrowOnErrorCallback, NSException.ThrowOnExceptionCallback));
        }

        /// <summary>
        /// Serializes the array to a JSON string.
        /// </summary>
        /// <remarks>
        /// The array must conform to the limitations imposed by <see href="https://developer.apple.com/documentation/foundation/nsjsonserialization?language=objc">NSJSONSerialization</see>.
        /// </remarks>
        /// <param name="jsonString"></param>
        /// <returns>string</returns>
        public string ToJson()
        {
            return NSArrayInterop.NSArray_ToJson(Pointer, NSError.ThrowOnErrorCallback, NSException.ThrowOnExceptionCallback);
        }

        #region IReadOnlyList<T>
        /// <summary>
        /// Array index operator.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The item at the specified <paramref name="index"/>.</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T this[int index]
        {
            get
            {
                if (index >= 0 && index < Count &&
                    TryGetValueAt(index, out var nsObject))
                {
                    return nsObject;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }
        #endregion

        #region IList<T>
        T IList<T>.this[int index]
        {
            get => this[index];
            set
            {
                ThrowReadOnly();
            }
        }

        public int IndexOf(T item)
        {
            if (NSArrayInterop.NSArray_IndexOfNSObject(Pointer, _boxer.Box(item).Pointer, out var index, NSException.ThrowOnExceptionCallback))
            {
                return (int)index;
            }
            else
            {
                return -1;
            }
        }

        void IList<T>.Insert(int index, T item)
        {
            ThrowReadOnly();
        }

        void IList<T>.RemoveAt(int index)
        {
            ThrowReadOnly();
        }
        #endregion

        #region IReadOnlyCollection<T>, ICollection<T>
        public int Count => NSArrayInterop.NSArray_GetCount(Pointer, NSException.ThrowOnExceptionCallback);
        #endregion

        #region ICollection<T>
        void ICollection<T>.Add(T item)
        {
            ThrowReadOnly();
        }

        void ICollection<T>.Clear()
        {
            ThrowReadOnly();
        }

        public bool Contains(T item)
        {
            return NSArrayInterop.NSArray_ContainsNSObject(Pointer, _boxer.Box(item).Pointer, NSException.ThrowOnExceptionCallback);
        }

        public void CopyTo(T[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (array.Rank > 1)
            {
                throw new ArgumentException("Array cannot be multidimensional");
            }
            if (index < 0 || index + Count > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            for (int i = 0; i < Count; i++)
            {
                array[index + i] = this[i];
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            ThrowReadOnly();
            return false;
        }

        public bool IsReadOnly => true;
        #endregion

        #region IEnumerable<T>
        public IEnumerator<T> GetEnumerator()
        {
            return new NSArrayEnumerator<T>(this);
        }
        #endregion

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new NSArrayEnumerator<T>(this);
        }
        #endregion

        /// <summary>
        /// Try to get the <paramref name="value"/> of type <typeparamref name="T"/> at the given <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns>True if successful; false otherwise.</returns>
        public bool TryGetValueAt(int index, out T value)
        {
            if (NSArrayInterop.NSArray_TryGetNSObjectAt(Pointer, index, out var nsObjectPtr, NSException.ThrowOnExceptionCallback) &&
                _boxer.TryUnbox(PointerCast<NSObject>(nsObjectPtr), out var objValue))
            {
                value = (T)objValue;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Helps the debugger to visualize the collection.
        /// </summary>
        [Preserve]
        private T[] DebugView => this.ToArray();

        private void ThrowReadOnly()
        {
            throw new NotSupportedException("Collection is read-only.");
        }
    }

    /// <summary>
    /// Enumerator for NSArrays containing type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NSArrayEnumerator<T> : IEnumerator<T>
    {
        private NSArray<T> _array;
        private int _currentIndex = -1;

        public NSArrayEnumerator(NSArray<T> array)
        {
            _array = array;
        }

        public void Dispose()
        {
            _array = null;
        }

        public bool MoveNext()
        {
            return ++_currentIndex < _array.Count;
        }

        public void Reset()
        {
            _currentIndex = -1;
        }

        public T Current => _array[_currentIndex];
        object IEnumerator.Current => Current;
    }

    /// <summary>
    /// This NSArray type is declared for backwards compatibility with earlier releases.
    /// </summary>
    [Obsolete("NSArray is deprecated. Please use NSArray<> instead.")]
    public class NSArray : NSArray<NSObject>
    {
        #region Init
        internal NSArray(IntPtr pointer) : base(pointer) { }
        #endregion
    }

    /// <summary>
    /// This NSArray type is declared for backwards compatibility with earlier releases.
    /// </summary>
    [Obsolete("NSArrayInt64 is deprecated. Please use NSArray<System.Int64> instead.")]
    public class NSArrayInt64 : NSArray<long>
    {
        public NSArrayInt64(IntPtr pointer) : base(pointer) { }
    }

    /// <summary>
    /// This NSArray type is declared for backwards compatibility with earlier releases.
    /// </summary>
    [Obsolete("NSArrayBoolean is deprecated. Please use NSArray<System.Boolean> instead.")]
    public class NSArrayBoolean : NSArray<bool>
    {
        public NSArrayBoolean(IntPtr pointer) : base(pointer) { }
    }

    /// <summary>
    /// This NSArray type is declared for backwards compatibility with earlier releases.
    /// </summary>
    [Obsolete("NSArrayString is deprecated. Please use NSArray<System.String> instead.")]
    public class NSArrayString : NSArray<string>
    {
        public NSArrayString(IntPtr pointer) : base(pointer) { }
    }

    /// <summary>
    /// DllImports used by NSArray.
    /// </summary>
    internal static class NSArrayInterop
    {
        [DllImport(InteropUtility.DLLName)]
        public static extern int NSArray_GetCount(IntPtr pointer, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern bool NSArray_IndexOfNSObject(IntPtr nsArrayPtr, IntPtr nsObjectPtr, out long index, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern bool NSArray_ContainsNSObject(IntPtr nsArrayPtr, IntPtr nsObjectPtr, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern bool NSArray_TryGetNSObjectAt(IntPtr nsArrayPtr, int index, out IntPtr nsObjectValuePtr, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern IntPtr NSArray_FromJson(string jsonString, NSErrorCallback onError, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern string NSArray_ToJson(IntPtr nsArrayPtr, NSErrorCallback onError, NSExceptionCallback onException);
    }
}
