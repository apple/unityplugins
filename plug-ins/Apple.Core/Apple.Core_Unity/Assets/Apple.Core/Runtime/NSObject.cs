using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Apple.Core.Runtime
{
    /// <summary>
    /// C# wrapper around NSObject.
    /// </summary>
    public class NSObject : InteropReference, IEquatable<NSObject>
    {
        #region Init & Dispose
        /// <summary>
        /// Construct an NSObject around an existing instance.
        /// </summary>
        /// <param name="nsObjectPtr"></param>
        public NSObject(IntPtr nsObjectPtr) : base(nsObjectPtr) { }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                Interop.NSObject_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }
        #endregion

        /// <summary>
        /// Get the name of the underlying Objective-C type wrapped by this object.
        /// Used for internal, interoperability purposes only.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected static string GetInteropTypeName(Type type)
        {
            var typeName = type.Name;

            // Special case for types that provide a custom interop type name.
            if (type.GetProperty("InteropTypeName", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    ?.GetValue(null) is string customTypeName)
            {
                typeName = customTypeName;
            }

            // Special cases for NSArray<>, NSDictionary<>, and other NS*<> generics.
            // Strip off the C# generic notation since it's not used for the underlying types on the Swift/ObjC side.
            if (type.IsGenericType && typeName.StartsWith("NS"))
            {
                // We want everything before the accent symbol (e.g. "NSArray`1" => "NSArray")
                int acceptPos = typeName.IndexOf('`');
                if (acceptPos >= 0)
                {
                    typeName = typeName.Substring(0, acceptPos);
                }
            }

            return typeName;
        }

        #region As
        private IntPtr NSObject_As(IntPtr nsObjectPtr, string targetClassName)
        {
            var ptr = Interop.NSObject_As(nsObjectPtr, targetClassName);

            // Log a warning message to the debug console if the type conversion wasn't possible.
            // This isn't necessarily fatal but might indicate a problematic type naming inconsistency between C# and Objective-C.
            if (ptr == IntPtr.Zero)
            {
                Debug.LogWarning($"Unable to convert object to type {targetClassName}");
            }

            return ptr;
        }
        /// <summary>
        /// Try to cast this NSObject to the given type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>A reference to the object of type <typeparamref name="T"/> or null if the cast could not be performed.</returns>
        public T As<T>() where T : NSObject
        {
            return Pointer != IntPtr.Zero ? PointerCast<T>(NSObject_As(Pointer, GetInteropTypeName(typeof(T)))) : null;
        }

        /// <summary>
        /// Try to cast this NSObject to the given type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>A reference to the object of type <paramref name="type"/> or null if the cast could not be performed.
        /// Because this method isn't generic, the caller is responsible for casting the returned object to the desired type if the call was successful.</returns>
        public NSObject As(Type type)
        {
            return (Pointer != IntPtr.Zero) ? PointerCast(type, NSObject_As(Pointer, GetInteropTypeName(type))) as NSObject : null;
        }
        #endregion

        #region Is
        /// <summary>
        /// Test if this NSObject is an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>True if this object is of the given type; false otherwise.</returns>
        public bool Is<T>() where T : NSObject
        {
            return Interop.NSObject_Is(Pointer, GetInteropTypeName(typeof(T)));
        }

        /// <summary>
        /// Test if this NSObject is an object of type <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>True if this object is of the given type; false otherwise.</returns>
        public bool Is(Type type)
        {
            return Interop.NSObject_Is(Pointer, GetInteropTypeName(type));
        }
        #endregion

        #region Equals
        /// <summary>
        /// Check for equality with some other object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => (obj is NSObject nsObject2) && Equals(nsObject2);

        /// <summary>
        /// Check for equality with some other NSObject via the native NSObject isEqual method.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(NSObject obj) => (obj != null) && Interop.NSObject_IsEqual(Pointer, obj.Pointer);

        /// <summary>
        /// Override of object.GetHashCode() that gets the has code from the native NSObject.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => (int)Interop.NSObject_Hash(Pointer);
        #endregion

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void NSObject_Free(IntPtr nsObjectPtr);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr NSObject_As(IntPtr nsObjectPtr, string targetClassName);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool NSObject_Is(IntPtr nsObjectPtr, string targetClassName);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool NSObject_IsEqual(IntPtr nsObject1Ptr, IntPtr nsObject2Ptr);
            [DllImport(InteropUtility.DLLName)]
            public static extern ulong NSObject_Hash(IntPtr nsObjectPtr);
        }
    }
}
