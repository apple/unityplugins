using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Apple.Core
{
    public enum RuntimeOperatingSystem : uint
    {
        Unknown = 0,
        iOS = 1,
        macOS = 2,
        tvOS = 3,

        // New operating systems should be added above this line.
        RuntimeOperatingSystemCount
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RuntimeVersion
    {
        public int Major;
        public int Minor;

        public RuntimeVersion(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }

        public override string ToString() => (Minor == 0) ? $"{Major}" : $"{Major}.{Minor}";

        static readonly Regex _versionStringFormat = new Regex(@"^((?:\d+)(?:\.\d+)?)$");
                
        /// <summary>
        /// Accepted string formats are "Major.Minor" or "Major" where Major and Minor are represented by integer values.
        ///   Minor value is assumed to be 0 when no Minor value is provided in the format string.
        /// Examples:
        /// <code>
        /// RuntimeVersion rtVersionA = RuntimeVersion.FromString("10.2");
        /// RuntimeVersion rtVersionB = RuntimeVersion.FromString("11");
        /// </code>
        /// </summary>
        public static RuntimeVersion? FromString(string versionString)
        {
            if (versionString == "")
            {
                return null;
            }

            // Ensure strings are formatted as "Major.Minor" or "Major" where Major and Minor are strings of numbers, e.g. "12.5" or "14"
            if (_versionStringFormat.IsMatch(versionString))
            {
                int major = 0, minor = 0;

                // Get first string of numerals and try to parse
                Match currMatch = Regex.Match(versionString, @"\d+");
                if (currMatch.Success && Int32.TryParse(currMatch.Value, out major))
                {
                    // Get the string of numerals, if they exist, and try to parse
                    currMatch = currMatch.NextMatch();
                    if (currMatch.Success && Int32.TryParse(currMatch.Value, out minor))
                    {
                        return new RuntimeVersion(major, minor);
                    }
                    else
                    {
                        return new RuntimeVersion(major, 0);
                    }
                }
                else
                {
                    Debug.Log($"[Apple.Core Plug-In] RuntimeEnvironment failed to parse \"{currMatch.Value}\" as Int32.");
                    return null;
                }
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

        public RuntimeEnvironment(RuntimeOperatingSystem operatingSystem, int major, int minor = 0)
        {
            RuntimeOperatingSystem = operatingSystem;
            VersionNumber = new RuntimeVersion(major, minor);
        }
    }
}
