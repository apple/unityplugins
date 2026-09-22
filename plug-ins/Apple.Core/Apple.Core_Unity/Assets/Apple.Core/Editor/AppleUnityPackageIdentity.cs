#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
using UnityEditor.PackageManager;

namespace Apple.Core
{
    /// <summary>
    /// Single point of configuration for the package identity strings that <c>ApplePlugInEnvironment</c> uses to recognize
    /// Apple Unity Plug-In packages when the Unity Package Manager reports the contents of a project.
    /// </summary>
    /// <remarks>
    /// These values must agree with the <c>name</c> and <c>author.name</c> fields of each plug-in's <c>package.json</c>.
    /// If they disagree, the package manager still installs the package, but this plug-in system does not associate any
    /// native libraries with it, and the build then produces an Xcode project with no Apple plug-in libraries linked and
    /// no error explaining why.
    ///
    /// Keeping the strings here rather than inline at the comparison sites means that agreement is one thing to check
    /// instead of several, and that a project which needs different values has one file to edit.
    /// </remarks>
    public static class AppleUnityPackageIdentity
    {
        /// <summary>
        /// All plug-in package names (the <c>name</c> field of package.json) are expected to begin with this string.
        /// </summary>
        public const string PackageNamePrefix = "com.apple.unityplugin";

        /// <summary>
        /// All plug-in package author names (the <c>author.name</c> field of package.json) are expected to match this string exactly.
        /// </summary>
        public const string PackageAuthorName = "Apple, Inc";

        /// <summary>
        /// Determines whether a package reported by the Unity Package Manager is one of this plug-in collection's packages.
        /// </summary>
        /// <remarks>
        /// A package qualifies if it carries either the expected package-name prefix or the expected author name. Either
        /// alone is a strong enough signal, and requiring both turns a single edited field into a silent failure.
        /// Callers that can match a package against a compiled-in <c>AppleBuildStep</c> have a stronger signal available
        /// and should prefer it; see <c>ApplePlugInEnvironment.AddPackagesFromCollection</c>.
        /// </remarks>
        /// <param name="packageInfo">Package metadata as reported by the Unity Package Manager.</param>
        /// <returns>True when the package appears to be part of this plug-in collection.</returns>
        public static bool Matches(PackageInfo packageInfo)
        {
            if (packageInfo == null)
            {
                return false;
            }

            bool nameMatches = !string.IsNullOrEmpty(packageInfo.name) && packageInfo.name.StartsWith(PackageNamePrefix);
            bool authorMatches = packageInfo.author != null && packageInfo.author.name == PackageAuthorName;

            return nameMatches || authorMatches;
        }
    }
}
#endif // (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
