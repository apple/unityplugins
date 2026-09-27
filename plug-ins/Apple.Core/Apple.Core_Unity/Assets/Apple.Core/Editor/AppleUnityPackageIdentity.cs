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
    /// If they disagree, the package manager still installs the package, but this plug-in system will not associate any
    /// native libraries with it, and the build produces an Xcode project with no Apple plug-in libraries linked. That
    /// used to happen with no diagnostic; <see cref="IsPartialMatch"/> and <see cref="DescribeMismatch"/> exist so the
    /// condition is reported where it can still be acted on.
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
        /// Whether a package's name carries the expected prefix.
        /// </summary>
        public static bool NameMatches(PackageInfo packageInfo)
        {
            return packageInfo != null
                && !string.IsNullOrEmpty(packageInfo.name)
                && packageInfo.name.StartsWith(PackageNamePrefix);
        }

        /// <summary>
        /// Whether a package's author name matches exactly.
        /// </summary>
        /// <remarks>
        /// <c>author</c> is absent from some packages, so it is checked for null here rather than at each call site.
        /// </remarks>
        public static bool AuthorMatches(PackageInfo packageInfo)
        {
            return packageInfo != null
                && packageInfo.author != null
                && packageInfo.author.name == PackageAuthorName;
        }

        /// <summary>
        /// Whether a package reported by the Unity Package Manager is one of this plug-in collection's packages.
        /// Both the package-name prefix and the author name must agree.
        /// </summary>
        public static bool Matches(PackageInfo packageInfo)
        {
            return NameMatches(packageInfo) && AuthorMatches(packageInfo);
        }

        /// <summary>
        /// Whether a package carries one of the two identity signals but not the other.
        /// </summary>
        /// <remarks>
        /// This is the shape of a package that was meant to be one of these plug-ins but will not be recognized as one:
        /// a renamed package that kept the author, or an unchanged package name whose author was edited. Worth a warning,
        /// because the consequence -- no native libraries -- appears much later and a long way from the cause.
        /// </remarks>
        public static bool IsPartialMatch(PackageInfo packageInfo)
        {
            return packageInfo != null && NameMatches(packageInfo) != AuthorMatches(packageInfo);
        }

        /// <summary>
        /// Describes which identity signal disagreed, for use in a diagnostic message.
        /// </summary>
        public static string DescribeMismatch(PackageInfo packageInfo)
        {
            if (packageInfo == null)
            {
                return "No package information available.";
            }

            if (!NameMatches(packageInfo))
            {
                return $"Package name '{packageInfo.name}' does not begin with the expected prefix '{PackageNamePrefix}'.";
            }

            string author = packageInfo.author == null ? "<none>" : packageInfo.author.name;
            return $"Package author name is '{author}', but '{PackageAuthorName}' is expected.";
        }
    }
}
#endif // (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
