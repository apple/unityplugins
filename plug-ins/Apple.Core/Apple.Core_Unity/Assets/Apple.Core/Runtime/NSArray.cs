using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Apple.Core.Runtime
{
    public interface INSArray
    {
        IntPtr Pointer { get; set; }
        int Count { get; }
    }

    public interface INSArray<T> : INSArray, IEnumerable<T>
    {
        T this[int index] { get; }
        T ElementAtIndex(int index);
    }
    
    public class NSArray : InteropReference, INSArray
    {
        #region Init & Dispose
        internal NSArray() : base(IntPtr.Zero) {}
        internal NSArray(IntPtr pointer) : base(pointer) { }
        
        [DllImport(InteropUtility.DLLName)]
        private static extern void NSArray_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                NSArray_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }
        #endregion
        
        #region Count
        [DllImport(InteropUtility.DLLName)]
        private static extern int NSArray_GetCount(IntPtr pointer);
        public int Count
        {
            get => NSArray_GetCount(Pointer);
        }
        #endregion
    }

    /// <summary>
    /// Prevents LINQ methods from cleaning up
    /// native memory when we still need the result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NSArrayEnumerator<T> : IEnumerator<T>
    {
        private readonly NSArray<T> _array;
        private int _currentIndex = -1;

        public NSArrayEnumerator(NSArray<T> array)
        {
            _array = array;
        }
        
        public bool MoveNext()
        {
            return ++_currentIndex < _array.Count;
        }

        public void Reset()
        {
            _currentIndex = -1;
        }

        public T Current
        {
            get => _array.ElementAtIndex(_currentIndex);
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            
        }
    }

    public abstract class NSArray<T> : NSArray, INSArray<T>
    {
        private readonly NSArrayEnumerator<T> _enumerator;
        
        #region Init & Dispose

        public NSArray() : base(IntPtr.Zero)
        {
            _enumerator = new NSArrayEnumerator<T>(this);
        }

        public NSArray(IntPtr pointer) : base(pointer)
        {
            _enumerator = new NSArrayEnumerator<T>(this);
        }
        #endregion
        
        #region this[index]
        public virtual T this[int index]
        {
            get => ElementAtIndex(index);
        }
        #endregion
        
        #region ElementAtIndex

        public abstract T ElementAtIndex(int index);
        #endregion

        #region IEnumerable
        public virtual IEnumerator<T> GetEnumerator()
        {
            return _enumerator;
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }

    public class NSArrayInt64 : NSArray<long>
    {
        public NSArrayInt64(IntPtr pointer) : base(pointer) {}
        
        [DllImport(InteropUtility.DLLName)]
        private static extern long NSArray_GetInt64At(IntPtr pointer, int index);
        
        public override long ElementAtIndex(int index)
        {
            return NSArray_GetInt64At(Pointer, index);
        }
    }
    
    public class NSArrayBoolean : NSArray<bool>
    {
        public NSArrayBoolean(IntPtr pointer) : base(pointer) {}

        [DllImport(InteropUtility.DLLName)]
        private static extern bool NSArray_GetBooleanAt(IntPtr pointer, int index);
        
        public override bool ElementAtIndex(int index)
        {
            return NSArray_GetBooleanAt(Pointer, index);
        }
    }

    public class NSArrayString : NSArray<string>
    {
        public NSArrayString(IntPtr pointer) : base(pointer) {}

        [DllImport(InteropUtility.DLLName)]
        private static extern string NSArray_GetStringAt(IntPtr pointer, int index);
        
        public override string ElementAtIndex(int index)
        {
            return NSArray_GetStringAt(Pointer, index);
        }
    }
}
