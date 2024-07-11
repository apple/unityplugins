using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Apple.Core.Runtime
{
    // InteropWeakMap is a collection of weak references to existing C# wrappers.

    // This collection is useful for C# wrappers around native objects that send event style p-invoke
    // callbacks to C#.
    
    // Use this to map the address of the underlying native object to the corresponding C# wrapper
    // to guarantee that we can always keep the two instances associated with each other.
    // Without this mapping, we might end up creating a duplicate C# wrapper around the underlying native
    // object upon receiving a p-invoke callback and the duplicate might not have the same state as the
    // original instance of the wrapper. That could cause equality tests to fail or other difficult to
    // diagnose bugs due to object states being out of sync.

    // Usage pattern:
    /*
    public class MyCsWrapper : InteropReference
    {
        private static readonly InteropWeakMap<MyCsWrapper> _instanceMap = new InteropWeakMap<MyCsWrapper>();

        internal MyCsWrapper(IntPtr pointer) : base(pointer)
        {
            _instanceMap.Add(this);
        }

        protected override void OnDispose(bool isDisposing)
        {
            _instanceMap.Remove(this);
            base.OnDispose(isDisposing);
        }

        public delegate void MyDelegate(int somePayload);
        internal delegate void MyInternalDelegate(IntPtr myCsWrapperPtr, int somePayload);

        public event MyDelegate MyEvent;

        [MonoPInvokeCallback(typeof(MyInternalDelegate))]
        private static void OnMyCallback(IntPtr myCsWrapperPtr, int somePayload)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                if (_instanceMap.TryGet(myCsWrapperPtr, out var myCsWrapper))
                {
                    myCsWrapper?.MyEvent?.Invoke(somePayload);
                }
            });
        }
    }
    */

    public class InteropWeakMap<TCsWrapper> where TCsWrapper : InteropReference
    {
        private ConcurrentDictionary<IntPtr /*native address*/, WeakReference<TCsWrapper>> _map = new ConcurrentDictionary<IntPtr, WeakReference<TCsWrapper>>();

        /// <summary>
        /// Add a C# wrapper instance to the weak map.
        /// </summary>
        /// <param name="wrapper"></param>
        public void Add(TCsWrapper wrapper)
        {
            if (wrapper?.Pointer != default)
            {
                if (!_map.TryAdd(wrapper.Pointer, new WeakReference<TCsWrapper>(wrapper)))
                {
                    throw new InvalidOperationException("Object instance is already in the dictionary.");
                }
            }
        }

        /// <summary>
        /// Remove a C# wrapper instance from the weak map.
        /// </summary>
        /// <param name="wrapper"></param>
        public void Remove(TCsWrapper wrapper)
        {
            if (wrapper?.Pointer != default)
            {
                if (!_map.TryRemove(wrapper.Pointer, out var _))
                {
                    throw new InvalidOperationException("Object instance was already removed from the dictionary.");
                }
            }
        }

        /// <summary>
        /// Try to get a C# wrapper instance from the weak map.
        /// </summary>
        /// <param name="pointer">The address of the native object.</param>
        /// <param name="wrapper">The corresponding C# wrapper object for the native object.</param>
        /// <returns>True if the wrapper was found; false otherwise.</returns>
        public bool TryGet(IntPtr pointer, out TCsWrapper wrapper)
        {
            if (_map.TryGetValue(pointer, out var wrapperRef) &&
                wrapperRef.TryGetTarget(out wrapper) &&
                wrapper != default &&
                !wrapper.IsDisposed)
            {
                return true;
            }
            else
            {
                wrapper = null;
                return false;
            }
        }

        /// <summary>
        /// Trim any weak references that no longer correspond to live objects.
        /// </summary>
        public void Trim()
        {
            List<IntPtr> referencesToRemove = default;

            foreach (var kvp in _map)
            {
                if (!TryGet(kvp.Key, out _))
                {
                    (referencesToRemove ??= new List<IntPtr>()).Add(kvp.Key);
                }
            }

            if (referencesToRemove != default)
            {
                foreach (var ptr in referencesToRemove)
                {
                    if (!_map.TryRemove(ptr, out var _))
                    {
                        throw new InvalidOperationException("Object instance was already removed from the dictionary.");
                    }
                }

                referencesToRemove.Clear();
            }
        }

        /// <summary>
        /// Get the number of items in the map.
        /// </summary>
        public int Count => _map.Count;
    }
}
