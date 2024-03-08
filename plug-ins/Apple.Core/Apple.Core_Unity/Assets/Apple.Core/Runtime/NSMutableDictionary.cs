using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace Apple.Core.Runtime
{
    /// <summary>
    /// C# wrapper around NSMutableDictionary&lt;<typeparamref name="TKey"/>, <typeparamref name="TValue"/>&gt;.
    /// </summary>
    /// <typeparam name="TKey">Key type that must be a primitive number, string, or NSObject subclass that implements the NSCopying protocol.</typeparam>
    /// <typeparam name="TValue">Type of values stored in the dictionary. Must be a primitive number, string, NSObject or NSObject subclass.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [Preserve]
    public class NSMutableDictionary<TKey, TValue> : NSDictionary<TKey, TValue>, IDictionary<TKey, TValue>
    {
        /// <summary>
        /// Construct an empty NSMutableDictionary.
        /// </summary>
        public NSMutableDictionary() : base(NSMutableDictionaryInterop.NSMutableDictionary_Init()) { }

        /// <summary>
        /// Construct an NSMutableDictionary wrapper around an existing instance.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="isReadOnly">By default, NSMutableDictionary is writable. Set to false to make this instance read-only.</param>
        public NSMutableDictionary(IntPtr pointer, bool isReadOnly = false) : base(pointer)
        {
            IsReadOnly = isReadOnly;
        }

        #region IDictionary<TKey, TValue>
        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                ThrowIfReadOnly();
                NSMutableDictionaryInterop.NSMutableDictionary_SetNSObjectForKey(Pointer, _keyBoxer.Box(key).Pointer, _valueBoxer.Box(value).Pointer, NSException.ThrowOnExceptionCallback);
            }
        }

        public new ICollection<TKey> Keys => new NSMutableArray<TKey>(NSDictionaryInterop.NSDictionary_AllKeys(Pointer, NSException.ThrowOnExceptionCallback), isReadOnly: true);
        public new ICollection<TValue> Values => new NSMutableArray<TValue>(NSDictionaryInterop.NSDictionary_AllValues(Pointer, NSException.ThrowOnExceptionCallback), isReadOnly: true);

        public void Add(TKey key, TValue value) => this[key] = value;

        public bool Remove(TKey key)
        {
            ThrowIfReadOnly();

            bool didContainKey = ContainsKey(key);

            NSMutableDictionaryInterop.NSMutableDictionary_RemoveObjectForKey(Pointer, _keyBoxer.Box(key).Pointer, NSException.ThrowOnExceptionCallback);

            bool wasRemoved = didContainKey && !ContainsKey(key);

            return wasRemoved;
        }
        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>>
        public bool IsReadOnly { get; }

        public void Add(KeyValuePair<TKey, TValue> item) => this[item.Key] = item.Value;

        public void Clear()
        {
            ThrowIfReadOnly();
            NSMutableDictionaryInterop.NSMutableDictionary_RemoveAllObjects(Pointer, NSException.ThrowOnExceptionCallback);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => TryGetValue(item.Key, out var value) && Equals(item.Value, value);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (array.Rank > 1)
            {
                throw new ArgumentException("Array cannot be multidimensional");
            }
            if (arrayIndex < 0 || arrayIndex + Count > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            var keys = Keys as NSArray<TKey>;
            var values = Values as NSArray<TValue>;
            for (int i = 0; i < Count; i++)
            {
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
        #endregion

        private void ThrowIfReadOnly()
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException("Collection is read-only.");
            }
        }

        /// <summary>
        /// Helps the debugger to visualize the collection.
        /// </summary>
        [Preserve]
        private KeyValuePair<TKey, TValue>[] DebugView => this.ToArray();
    }

    /// <summary>
    /// DllImports used by NSMutableDictionary.
    /// </summary>
    internal static class NSMutableDictionaryInterop
    {
        [DllImport(InteropUtility.DLLName)]
        public static extern IntPtr NSMutableDictionary_Init();
        [DllImport(InteropUtility.DLLName)]
        public static extern void NSMutableDictionary_SetNSObjectForKey(IntPtr nsMutableDictionaryPtr, IntPtr nsObjectKeyPtr, IntPtr nsObjectValuePtr, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern void NSMutableDictionary_RemoveObjectForKey(IntPtr nsMutableDictionaryPtr, IntPtr nsObjectKeyPtr, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern void NSMutableDictionary_RemoveAllObjects(IntPtr nsMutableDictionaryPtr, NSExceptionCallback onException);
    }
}
