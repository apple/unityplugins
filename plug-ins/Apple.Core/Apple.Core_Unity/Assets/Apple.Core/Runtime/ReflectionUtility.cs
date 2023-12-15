using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Scripting;

[assembly:Preserve]

namespace Apple.Core.Runtime
{
    internal static class ReflectionUtility
    {
        private static readonly Dictionary<Type, ConstructorInfo> _constructors = new Dictionary<Type, ConstructorInfo>();

        public static ConstructorInfo GetConstructor(Type type)
        {
            if (_constructors.TryGetValue(type, out var constructor))
            {
                return constructor;
            }

            foreach (var constructorInfo in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var parameters = constructorInfo.GetParameters();

                if (parameters.Length == 1 && parameters[0].ParameterType == typeof(IntPtr))
                {
                    _constructors.Add(type, constructorInfo);
                    return constructorInfo;
                }
            }

            // No constructor found...
            _constructors.Add(type, null);
            return _constructors[type];
        }

        public static ConstructorInfo GetConstructor<T>()
        {
            return GetConstructor(typeof(T));
        }

        public static InteropReference CreateInstanceOrDefault(Type type, IntPtr pointer)
        {
            if (!type.IsSubclassOf(typeof(InteropReference)) && !type.Equals(typeof(InteropReference)))
            {
                throw new ArgumentException("Type must be a subclass of InteropReference", nameof(type));
            }

            if (pointer == IntPtr.Zero)
                return default;

            var constructor = GetConstructor(type);

            if (constructor != null)
            {
                return (InteropReference)constructor.Invoke(new object[] { pointer });
            }

            var instance = (InteropReference)Activator.CreateInstance(type);
            instance.Pointer = pointer;

            return instance;

        }

        public static T CreateInstanceOrDefault<T>(IntPtr pointer) where T : InteropReference
        {
            return CreateInstanceOrDefault(typeof(T), pointer) as T;
        }
    }
}
