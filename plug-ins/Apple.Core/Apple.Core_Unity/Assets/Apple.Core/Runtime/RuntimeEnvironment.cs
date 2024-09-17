using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Apple.Core
{
    public enum RuntimeOperatingSystem : uint
    {
        Unknown = 0,
        iOS = 1,
        macOS = 2,
        tvOS = 3,
        visionOS = 4,

        // New operating systems should be added above this line.
        RuntimeOperatingSystemCount
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RuntimeVersion
    {
        public int Major;
        public int Minor;
        public int Patch;

        public RuntimeVersion(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public override readonly string ToString() => (Patch == 0) ? ((Minor == 0) ? $"{Major}" : $"{Major}.{Minor}") : $"{Major}.{Minor}.{Patch}";

        /// <summary>
        /// Accepted string formats are "Major.Minor.Patch", "Major.Minor" or "Major" where Major, Minor, and Patch are represented by integer values.
        ///   Minor value is assumed to be 0 when no Minor value is provided in the format string.
        /// Examples:
        /// <code>
        /// RuntimeVersion rtVersionA = RuntimeVersion.FromString("10.15.5");
        /// RuntimeVersion rtVersionB = RuntimeVersion.FromString("10.2");
        /// RuntimeVersion rtVersionC = RuntimeVersion.FromString("11");
        /// </code>
        /// </summary>
        public static RuntimeVersion? FromString(string versionString)
        {
            if (versionString == "")
            {
                return null;
            }

            // Ensure strings are formatted as "Major.Minor.Patch", "Major.Minor" or "Major" where Major, Minor, and Patch are strings of numbers, e.g. "10.15.5", "12.5", or "14"
            int major = 0, minor = 0, patch = 0;
            var numeralStrings = versionString.Split('.');
            if (numeralStrings.Length >= 1 && numeralStrings.Length <= 3 &&
                Int32.TryParse(numeralStrings[0], out major) &&
                (numeralStrings.Length < 2 || Int32.TryParse(numeralStrings[1], out minor)) &&
                (numeralStrings.Length < 3 || Int32.TryParse(numeralStrings[2], out patch)))
            {
                return new RuntimeVersion(major, minor, patch);
            }
            else
            {
                Debug.Log($"[Apple.Core Plug-In] RuntimeEnvironment failed to recognize \"{versionString}\" as a valid version string.");
                return null;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RuntimeEnvironment
    {
        public RuntimeOperatingSystem RuntimeOperatingSystem;
        public RuntimeVersion VersionNumber;

        public RuntimeEnvironment(RuntimeOperatingSystem operatingSystem, int major, int minor = 0, int patch = 0)
        {
            RuntimeOperatingSystem = operatingSystem;
            VersionNumber = new RuntimeVersion(major, minor, patch);
        }

        public readonly bool IsUnknown => RuntimeOperatingSystem == RuntimeOperatingSystem.Unknown;

        public override readonly string ToString() => $"{RuntimeOperatingSystem} {VersionNumber}";
 
    }
}
