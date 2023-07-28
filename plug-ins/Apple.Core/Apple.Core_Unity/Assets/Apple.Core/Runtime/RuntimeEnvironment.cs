using System;
using System.Runtime.InteropServices;

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

        public override string ToString() => $"{Major}.{Minor}";

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

            string[] vStrings = versionString.Split('.', 3);

            // String provided in the format: "Major.Minor"
            if (vStrings.Length >= 2)
            {
                int major = 0, minor = 0;

                if (Int32.TryParse(vStrings[0], out major) && Int32.TryParse(vStrings[1], out minor))
                {
                    return new RuntimeVersion(major, minor);
                }
                else
                {
                    return null;
                }
            }
            // String provided in the format: "Major"
            else if (vStrings.Length == 1)
            {
                int major = 0;

                if (Int32.TryParse(vStrings[0], out major))
                {
                    return new RuntimeVersion(major, 0);
                }
                else
                {
                    return null;
                }
            }
            // Unsupported formatting.
            else
            {
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
