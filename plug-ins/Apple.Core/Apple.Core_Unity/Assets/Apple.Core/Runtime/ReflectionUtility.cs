using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apple.Core.Runtime
{
    internal static class ReflectionUtility
    {
        private static readonly Dictionary<Type, ConstructorInfo> _constructors = new Dictionary<Type, ConstructorInfo>();

        public static ConstructorInfo GetConstructor<T>() where T : InteropReference
        {
            if(_constructors.TryGetValue(typeof(T), out var constructor))
            {
                return constructor;
            }

            var type = typeof(T);
            
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


        public static T CreateInstanceOrDefault<T>(IntPtr pointer) where T : InteropReference
        {
            if (pointer == IntPtr.Zero)
                return default;
            
            var constructor = GetConstructor<T>();

            if (constructor != null)
            {
                return (T)constructor.Invoke(new object[] { pointer });
            }

            var instance = Activator.CreateInstance<T>();
            instance.Pointer = pointer;
            
            return instance;
        }
    }
}
