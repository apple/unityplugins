using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Apple.Core.Runtime
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InteropStructDictionary
    {
        public InteropStructArray Keys;
        public InteropStructArray Values;

        public static InteropStructDictionary From<TKey, TValue>(Dictionary<TKey, TValue> dictionary, out GCHandle gcHandleKeys, out GCHandle gcHandleValues)  where TKey : struct where TValue : struct
        {
            var keys = dictionary.Keys.ToArray();
            var values = dictionary.Values.ToArray();

            var interopDictionary = new InteropStructDictionary();
            interopDictionary.Keys = InteropStructArray.From(keys, out gcHandleKeys);
            interopDictionary.Values = InteropStructArray.From(values, out gcHandleValues);

            return interopDictionary;
        }

        public Dictionary<TKey, TValue> ToDictionary<TKey, TValue>() where TKey : struct where TValue : struct
        {
            var keys = Keys.ToArray<TKey>();
            var values = Values.ToArray<TValue>();
            
            var dictionary = new Dictionary<TKey, TValue>();

            for (var i = 0; i < keys.Length; i++)
            {
                if (i < values.Length)
                {
                    dictionary.Add(keys[i], values[i]);
                }
            }

            return dictionary;
        }
    }
}
