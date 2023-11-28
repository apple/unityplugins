using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Apple.Core
{
    public static class Availability
    {
        private static RuntimeEnvironment _runtimeEnvironment;

        [DllImport(InteropUtility.DLLName, EntryPoint = "AppleCore_GetRuntimeEnvironment")]
        private static extern RuntimeEnvironment AppleCore_GetRuntimeEnvironment();

        /// <summary>
        /// Use to ensure API methods are only called on platforms which support those calls.
        /// </summary>
        public static bool Available(RuntimeOperatingSystem targetOperatingSystem, int targetMajorVersion, int targetMinorVersion = 0)
        {
            if (_runtimeEnvironment.RuntimeOperatingSystem == targetOperatingSystem)
            {
                if (_runtimeEnvironment.VersionNumber.Major > targetMajorVersion)
                {
                    return true;
                }
                else if ((_runtimeEnvironment.VersionNumber.Major == targetMajorVersion) && (_runtimeEnvironment.VersionNumber.Minor >= targetMinorVersion))
                {
                    return true;
                }
            }

            return false;
        }

        #region Init & Shutdown
        [RuntimeInitializeOnLoadMethod]
        private static void OnApplicationStart()
        {
            Debug.Log("[Apple.Core Plug-In] Initializing API Availability Checking");

            _runtimeEnvironment = AppleCore_GetRuntimeEnvironment();

            Debug.Log($"[Apple.Core Plug-In] Availability Runtime Environment: {_runtimeEnvironment.RuntimeOperatingSystem.ToString()} {_runtimeEnvironment.VersionNumber.Major}.{_runtimeEnvironment.VersionNumber.Minor}");
        }
        #endregion
    }
}
