using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Apple.Core
{
    public static class Availability
    {
        [DllImport(InteropUtility.DLLName, EntryPoint = "AppleCore_GetRuntimeEnvironment")]
        private static extern RuntimeEnvironment AppleCore_GetRuntimeEnvironment();

        private static RuntimeEnvironment _runtimeEnvironment;
        public static RuntimeEnvironment RuntimeEnvironment => _runtimeEnvironment.IsUnknown ? (_runtimeEnvironment = AppleCore_GetRuntimeEnvironment()) : _runtimeEnvironment;

        /// <summary>
        /// Use to ensure API methods are only called on platforms which support those calls.
        /// </summary>
        public static bool Available(RuntimeOperatingSystem targetOperatingSystem, int targetMajorVersion, int targetMinorVersion = 0)
        {
            if (RuntimeEnvironment.RuntimeOperatingSystem == targetOperatingSystem)
            {
                if (RuntimeEnvironment.VersionNumber.Major > targetMajorVersion)
                {
                    return true;
                }
                else if ((RuntimeEnvironment.VersionNumber.Major == targetMajorVersion) && (RuntimeEnvironment.VersionNumber.Minor >= targetMinorVersion))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool IsAvailable(RuntimeOperatingSystem targetOperatingSystem, int targetMajorVersion, int targetMinorVersion = 0) => Available(targetOperatingSystem, targetMajorVersion, targetMinorVersion);

        /// <summary>
        /// Test whether the given runtime environment is available.
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public static bool IsAvailable(RuntimeEnvironment env)
        {
            return IsAvailable(env.RuntimeOperatingSystem, env.VersionNumber.Major, env.VersionNumber.Minor);
        }

        /// <summary>
        /// Check availability attributes to see if the item is available in the given runtime environment.
        /// </summary>
        /// <param name="attrs"></param>
        /// <param name="env">The runtime environment to test or default to test the current environment.</param>
        /// <returns></returns>
        private static bool IsAvailable(Attribute[] attrs, RuntimeEnvironment env = default)
        {
            env = env.IsUnknown ? RuntimeEnvironment : env;

            // Return false if any attribute indicates non-availability.
            return !attrs.Any(attr => attr is AvailabilityAttributeBase availability && !availability.IsAvailable(env));
        }

        /// <summary>
        /// Test whether a given type is available in the given runtime environment.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="env">The runtime environment to test or default to test the current environment.</param>
        /// <returns></returns>
        public static bool IsTypeAvailable(Type type, RuntimeEnvironment env = default)
        {            
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // Test the current type and any declaring types.
            for (var currentType = type; currentType != null; currentType = currentType.DeclaringType)
            {
                if (!IsAvailable(Attribute.GetCustomAttributes(currentType), env))
                {
                    return false;
                }
            }
            return true;
        } 
        public static bool IsTypeAvailable<T>(RuntimeEnvironment env = default) => IsTypeAvailable(typeof(T), env);

        /// <summary>
        /// Test whether the given class member is available in the given runtime environment
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="env">The runtime environment to test or default to test the current environment.</param>
        /// <returns></returns>
        public static bool IsMemberAvailable(MemberInfo memberInfo, RuntimeEnvironment env = default)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            if (!IsAvailable(Attribute.GetCustomAttributes(memberInfo), env))
            {
                return false;
            }

            if (!IsTypeAvailable(memberInfo.DeclaringType, env))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Test whether the constructor of the given type is available in the given runtime environment.
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="parameterTypes">Array of parameter types or Type.EmptyTypes if none</param>
        /// <param name="env">The runtime environment to test or default to test the current environment.</param>
        /// <returns></returns>
        public static bool IsConstructorAvailable(Type declaringType, Type[] parameterTypes, RuntimeEnvironment env = default) => IsMemberAvailable(declaringType.GetConstructor(parameterTypes), env);
        public static bool IsConstructorAvailable<DeclaringType>(Type[] parameterTypes, RuntimeEnvironment env = default) => IsConstructorAvailable(typeof(DeclaringType), parameterTypes, env);

        /// <summary>
        /// Test whether the event declared by the given type is available in the given runtime environment.
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="eventName"></param>
        /// <param name="env">The runtime environment to test or default to test the current environment.</param>
        /// <returns></returns>
        public static bool IsEventAvailable(Type declaringType, string eventName, RuntimeEnvironment env = default) => IsMemberAvailable(declaringType.GetEvent(eventName), env);
        public static bool IsEventAvailable<DeclaringType>(string eventName, RuntimeEnvironment env = default) => IsEventAvailable(typeof(DeclaringType), eventName, env);

        /// <summary>
        /// Test whether the field declared by the given type is available in the given runtime environment.
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="fieldName"></param>
        /// <param name="env">The runtime environment to test or default to test the current environment.</param>
        /// <returns></returns>
        public static bool IsFieldAvailable(Type declaringType, string fieldName, RuntimeEnvironment env = default) => IsMemberAvailable(declaringType.GetField(fieldName), env);
        public static bool IsFieldAvailable<DeclaringType>(string fieldName, RuntimeEnvironment env = default) => IsFieldAvailable(typeof(DeclaringType), fieldName, env);

        /// <summary>
        /// Test whether the method declared by the given type is available in the given runtime environment.
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="methodName"></param>
        /// <param name="parameterTypes">Array of method parameter types, Type.EmptyTypes for none, or null for unspecified.</param>
        /// <param name="env">The runtime environment to test or default to test the current environment.</param>
        /// <returns></returns>
        public static bool IsMethodAvailable(Type declaringType, string methodName, Type[] parameterTypes = null, RuntimeEnvironment env = default)
        {
            bool isAvailable = true;

            // Method lookup might be ambiguous if there are overloads.
            try
            {
                isAvailable = IsMemberAvailable(
                    (parameterTypes == null) ?
                        declaringType.GetMethod(methodName) :
                        declaringType.GetMethod(methodName, parameterTypes),
                    env);
            }
            catch (AmbiguousMatchException)
            {
                // There are overloads. See if ALL methods with this name have the same availability.
                // If not, then the result is ambiguous.
                var distinctAvailabilities = declaringType.GetMethods()
                    .Where(m => m.Name == methodName)
                    .Select(m => IsMemberAvailable(m, env))
                    .Distinct()
                    .ToArray();
                
                if (distinctAvailabilities.Length == 1)
                {
                    // All method overloads have the same availability.
                    isAvailable = distinctAvailabilities[0];
                }
                else
                {
                    // Result is ambiguous (some overloads available; some not).
                    throw;
                }
            }

            return isAvailable;
        } 
        public static bool IsMethodAvailable<DeclaringType>(string methodName, Type[] parameterTypes = null, RuntimeEnvironment env = default) => IsMethodAvailable(typeof(DeclaringType), methodName, parameterTypes, env);

        /// <summary>
        /// Test whether the property declared by the given type is available in the given runtime environment.
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="propertyName"></param>
        /// <param name="env">The runtime environment to test or default to test the current environment.</param>
        /// <returns></returns>
        public static bool IsPropertyAvailable(Type declaringType, string propertyName, RuntimeEnvironment env = default) => IsMemberAvailable(declaringType.GetProperty(propertyName), env);
        public static bool IsPropertyAvailable<DeclaringType>(string propertyName, RuntimeEnvironment env = default) => IsPropertyAvailable(typeof(DeclaringType), propertyName, env);

        #region Init & Shutdown
        [RuntimeInitializeOnLoadMethod]
        private static void OnApplicationStart()
        {
            Debug.Log("[Apple.Core Plug-In Runtime] Initializing API Availability Checking");

            var env = RuntimeEnvironment;

            Debug.Log($"[Apple.Core Plug-In Runtime] Availability Runtime Environment: {env.RuntimeOperatingSystem.ToString()} {env.VersionNumber.Major}.{env.VersionNumber.Minor}");
        }
        #endregion
    }
}
