using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace Apple.Core.Runtime
{
    /// <summary>
    /// C# wrapper around NSDictionary&lt;<typeparamref name="TKey"/>, <typeparamref name="TValue"/>&gt;.
    /// </summary>
    /// <typeparam name="TKey">Key type that must be a primitive number, string, or NSObject subclass that implements the NSCopying protocol.</typeparam>
    /// <typeparam name="TValue">Type of values stored in the dictionary. Must be a primitive number, string, NSObject or NSObject subclass.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [Preserve]
    public class NSDictionary<TKey, TValue> : NSObject, IReadOnlyDictionary<TKey, TValue>
    {
        protected InteropBoxer.Boxer _keyBoxer;
        protected InteropBoxer.Boxer _valueBoxer;

        /// <summary>
        /// Construct an NSDictionary wrapper around an existing instance.
        /// </summary>
        /// <param name="nsDictionaryPtr"></param>
        /// <exception cref="NotSupportedException">Thrown if <typeparamref name="TKey"/> or <typeparamref name="TValue"/> are not supported types.</exception>
        public NSDictionary(IntPtr nsDictionaryPtr) : base(nsDictionaryPtr)
        {
            _keyBoxer = InteropBoxer.LookupBoxer<TKey>();
            if (_keyBoxer == null)
            {
                throw new NotSupportedException($"NSDictionary<TKey, TValue> does not support type TKey = {typeof(TKey).FullName}");
            }

            // Keys must implement the NSCopying protocol. Confirm.
            if (!NSDictionaryInterop.NSDictionary_IsValidKeyType(GetInteropTypeName(_keyBoxer.BoxType), NSException.ThrowOnExceptionCallback))
            {
                throw new NotSupportedException("NSDictionary key type must implement the NSCopying protocol.");
            }

            _valueBoxer = InteropBoxer.LookupBoxer<TValue>();
            if (_valueBoxer == null)
            {
                throw new NotSupportedException($"NSDictionary<TKey, TValue> does not support type TValue = {typeof(TValue).FullName}");
            }
        }

        /// <summary>
        /// Returns a dictionary from the given JSON string.
        /// </summary>
        /// <remarks>
        /// The JSON must conform to the limitations imposed by <see href="https://developer.apple.com/documentation/foundation/nsjsonserialization?language=objc">NSJSONSerialization</see>.
        /// </remarks>
        /// <param name="jsonString"></param>
        /// <returns>NSDictionary&lt;<typeparamref name="TKey"/>, <typeparamref name="TValue"/>&gt;</returns>
        public static NSDictionary<TKey, TValue> FromJson(string jsonString)
        {
            return new NSDictionary<TKey, TValue>(NSDictionaryInterop.NSDictionary_FromJson(jsonString, NSError.ThrowOnErrorCallback, NSException.ThrowOnExceptionCallback));
        }

        /// <summary>
        /// Serializes the dictionary to a JSON string.
        /// </summary>
        /// <remarks>
        /// The dictionary must conform to the limitations imposed by <see href="https://developer.apple.com/documentation/foundation/nsjsonserialization?language=objc">NSJSONSerialization</see>.
        /// </remarks>
        /// <param name="jsonString"></param>
        /// <returns>string</returns>
        public string ToJson()
        {
            return NSDictionaryInterop.NSDictionary_ToJson(Pointer, NSError.ThrowOnErrorCallback, NSException.ThrowOnExceptionCallback);
        }

        #region IReadOnlyDictionary<TKey, TValue>
        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var value))
                {
                    return value;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
        }

        public IEnumerable<TKey> Keys => new NSArray<TKey>(NSDictionaryInterop.NSDictionary_AllKeys(Pointer, NSException.ThrowOnExceptionCallback));
        public IEnumerable<TValue> Values => new NSArray<TValue>(NSDictionaryInterop.NSDictionary_AllValues(Pointer, NSException.ThrowOnExceptionCallback));

        public bool ContainsKey(TKey key) => NSDictionaryInterop.NSDictionary_ContainsNSObjectKey(Pointer, _keyBoxer.Box(key).Pointer, NSException.ThrowOnExceptionCallback);

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (NSDictionaryInterop.NSDictionary_TryGetValueAsNSObject(Pointer, _keyBoxer.Box(key).Pointer, out var nsObjectValuePtr, NSException.ThrowOnExceptionCallback) &&
                _valueBoxer.TryUnbox(PointerCast<NSObject>(nsObjectValuePtr), out var objValue))
            {
                value = (TValue)objValue;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        #endregion

        #region IReadOnlyCollection<KeyValuePair<TKey, TValue>>
        public int Count => NSDictionaryInterop.NSDictionary_Count(Pointer, NSException.ThrowOnExceptionCallback);
        #endregion

        #region IEnumerable<KeyValuePair<TKey, TValue>>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new NSDictionaryEnumerator<TKey, TValue>(this);
        #endregion

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        /// <summary>
        /// Helps the debugger to visualize the collection.
        /// </summary>
        [Preserve]
        private KeyValuePair<TKey, TValue>[] DebugView => this.ToArray();
    }

    /// <summary>
    /// Enumerator for key/value pairs in an NSDictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class NSDictionaryEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private NSArray<TKey> _keys;
        private NSArray<TValue> _values;
        private int _currentIndex = -1;

        public NSDictionaryEnumerator(NSDictionary<TKey, TValue> dictionary)
        {
            _keys = dictionary.Keys as NSArray<TKey>;
            _values = dictionary.Values as NSArray<TValue>;
        }

        public void Dispose()
        {
            _keys?.Dispose();
            _keys = null;

            _values?.Dispose();
            _values = null;
        }

        public bool MoveNext()
        {
            return ++_currentIndex < _keys.Count;
        }

        public void Reset()
        {
            _currentIndex = -1;
        }

        public KeyValuePair<TKey, TValue> Current => new KeyValuePair<TKey, TValue>(_keys[_currentIndex], _values[_currentIndex]);

        object IEnumerator.Current => Current;
    }

    /// <summary>
    /// This subclass is provided for backwards compatibility with prior releases.
    /// </summary>
    [Obsolete("NSDictionary is deprecated. Please use NSDictionary<,> instead.")]
    public class NSDictionary : NSDictionary<string, NSObject>
    {
        public NSDictionary(IntPtr nsDictionaryPtr) : base(nsDictionaryPtr) { }

        private InteropBoxer.Boxer _nsErrorBoxer;
        private InteropBoxer.Boxer NsErrorBoxer => _nsErrorBoxer ??= InteropBoxer.LookupBoxer<NSError>();

        private InteropBoxer.Boxer _stringBoxer;
        private InteropBoxer.Boxer StringBoxer => _stringBoxer ??= InteropBoxer.LookupBoxer<string>();

        private InteropBoxer.Boxer _boolBoxer;
        private InteropBoxer.Boxer BoolBoxer => _boolBoxer ??= InteropBoxer.LookupBoxer<bool>();

        private InteropBoxer.Boxer _longBoxer;
        private InteropBoxer.Boxer LongBoxer => _longBoxer ??= InteropBoxer.LookupBoxer<long>();

        private InteropBoxer.Boxer _doubleBoxer;
        private InteropBoxer.Boxer DoubleBoxer => _doubleBoxer ??= InteropBoxer.LookupBoxer<double>();

        private InteropBoxer.Boxer _nsDictionaryBoxer;
        private InteropBoxer.Boxer NsDictionaryBoxer => _nsDictionaryBoxer ??= InteropBoxer.LookupBoxer<NSDictionary>();

        private T GetValue<T>(string key, InteropBoxer.Boxer boxer)
        {
            return (TryGetValue(key, out var nsObject) && boxer.TryUnbox(nsObject, out var value)) ? (T)value : default;
        }

        public NSError GetNSError(string key) => GetValue<NSError>(key, NsErrorBoxer);

        public string GetString(string key) => GetValue<string>(key, StringBoxer);

        public bool GetBoolean(string key) => GetValue<bool>(key, BoolBoxer);

        public long GetInt64(string key) => GetValue<long>(key, LongBoxer);

        public double GetDouble(string key) => GetValue<double>(key, DoubleBoxer);

        public NSDictionary GetNSDictionary(string key) => GetValue<NSDictionary>(key, NsDictionaryBoxer);
    }

    /// <summary>
    /// DllImports used by NSDictionary.
    /// </summary>
    internal static class NSDictionaryInterop
    {
        [DllImport(InteropUtility.DLLName)]
        public static extern bool NSDictionary_IsValidKeyType(string keyClassName, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern bool NSDictionary_TryGetValueAsNSObject(IntPtr nsDictionaryPtr, IntPtr nsObjectKeyPtr, out IntPtr nsObjectValuePtr, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern bool NSDictionary_ContainsNSObjectKey(IntPtr nsDictionaryPtr, IntPtr nsObjectKeyPtr, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern int NSDictionary_Count(IntPtr nsDictionaryPtr, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern IntPtr NSDictionary_AllKeys(IntPtr nsDictionaryPtr, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern IntPtr NSDictionary_AllValues(IntPtr nsDictionaryPtr, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern IntPtr NSDictionary_FromJson(string jsonString, NSErrorCallback onError, NSExceptionCallback onException);
        [DllImport(InteropUtility.DLLName)]
        public static extern string NSDictionary_ToJson(IntPtr nsDictionaryPtr, NSErrorCallback onError, NSExceptionCallback onException);
    }
}
