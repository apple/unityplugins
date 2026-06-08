using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace Apple.Core.Runtime
{
    /// <summary>
    /// Array of Swift InteropReference types. Each will use <see cref="InteropReference.PointerCast{T}(IntPtr)"/>
    /// to cast the pointer to the correct type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [Preserve]
    public class SwiftArray<T> : InteropReference, IReadOnlyList<T>
        where T : InteropReference
    {
        public SwiftArray(IntPtr pointer, int count) : base(pointer)
        {
            Count = count;
        }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                SwiftArrayInterop._Apple_Core_SwiftArray_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        public T this[int index]
        {
            get
            {
                var elementPtr = SwiftArrayInterop._Apple_Core_SwiftArray_GetElementAt(Pointer, Count, index);
                return InteropReference.PointerCast<T>(elementPtr);
            }
        }
        public int Count { get; }

        public IEnumerator<T> GetEnumerator() => new SwiftArrayEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class SwiftArrayEnumerator : IEnumerator<T>
        {
            private readonly SwiftArray<T> _array;
            private int _currentIndex = -1;

            public SwiftArrayEnumerator(SwiftArray<T> array)
            {
                _array = array;
            }

            public T Current => _array[_currentIndex];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                _currentIndex++;
                return _currentIndex < _array.Count;
            }

            public void Reset()
            {
                _currentIndex = -1;
            }
        }
    }

    internal static class SwiftArrayInterop
    {
        [DllImport(InteropUtility.DLLName)]
        public static extern IntPtr _Apple_Core_SwiftArray_GetElementAt(IntPtr arrayPtr, int count, int index);

        [DllImport(InteropUtility.DLLName)]
        public static extern void _Apple_Core_SwiftArray_Free(IntPtr arrayPtr);
    }
}