#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX))
using System;
using System.IO;
using UnityEngine;

namespace Apple.Core
{
    /// <summary>
    /// Simple representation of an Apple native plug-in library, used to keep track of libraries and paths within a given plug-in.
    /// </summary>
    public class AppleNativeLibrary
    {
        /// <summary>
        /// Used to classify library types; linking behaviors will vary by this type.
        /// </summary>
        public enum LibraryType
        {
            Framework,
            Bundle,
            Static,
            Unknown
        }

        /// <summary>
        /// Construct an AppleNativeLibrary object using it's filename, debug symbols filename, and full path.
        /// </summary>
        /// <param name="fileName">The library's name including extension.</param>
        /// <param name="debugSymbolsFileName">The debug symbols filename, including dSYM extension</param>
        /// <param name="applePlatform">The ApplePlatformID for which this library was built.</param>
        /// <param name="path">The fully qualified path to the containing folder.</param>
        public AppleNativeLibrary(string fileName, string debugSymbolsFileName, string applePlatform, string path)
        {
            Type = LibraryType.Unknown;
            FileName = string.Empty;
            DebugSymbolsFileName = string.Empty;
            Platform = string.Empty;
            Path = string.Empty;
            
            if (fileName != string.Empty && applePlatform != string.Empty && path != string.Empty)
            {
                string fileNameSuffix = fileName[(fileName.LastIndexOf('.') + 1)..];
                if (fileNameSuffix.Equals("a", StringComparison.OrdinalIgnoreCase))
                {
                    Type = LibraryType.Static;
                }
                else if (fileNameSuffix.Equals("framework", StringComparison.OrdinalIgnoreCase))
                {
                    Type = LibraryType.Framework;
                }
                else if (fileNameSuffix.Equals("bundle", StringComparison.OrdinalIgnoreCase))
                {
                    Type = LibraryType.Bundle;
                }
                else
                {
                    string errorMessage = $"[AppleNativeLibrary] Encountered unknown library with extension \"{fileNameSuffix}\"\n"
                    + "  Supported library types are: \'.a\', \'.framework\', or \'.bundle.\'\n"
                    + $"  Provided path: {path}";

                    Debug.LogError(errorMessage);
                }
            }

            if (Type != LibraryType.Unknown)
            {
                FileName = fileName;
                DebugSymbolsFileName = debugSymbolsFileName;
                Platform = applePlatform;
                Path = path;
            }
        }

        /// <summary>
        /// Access the library's classified type.
        /// </summary>
        public LibraryType Type { get; private set; }

        /// <summary>
        /// Represents the library's file name along with extension, e.g. SomeLibrary.framework or SomeLibrary.bundle
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// If there's a .dSYM file, the filename, with .dSYM extension, will be stored here.
        /// </summary>
        /// <remarks>
        /// Will be string.Empty if there is no associated .dSYM file
        /// </remarks>
        public string DebugSymbolsFileName { get; private set; }

        /// <summary>
        /// The ApplePlatformID representing which platform this library targets.
        /// </summary>
        public string Platform { get; private set; }

        /// <summary>
        /// The fully qualified path to this library's containing folder.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Helper retrives the fully qualified path including the file name of the library.
        /// </summary>
        public string FullPath => $"{Path}/{FileName}";

        /// <summary>
        /// If there <c>DebugSymbolsFileName</c> is not string.Empty, this property will retrieve the fully qualified path including the file name.
        /// </summary>
        public string FullDebugSymbolsPath => (DebugSymbolsFileName != string.Empty) ? $"{Path}/{DebugSymbolsFileName}" : string.Empty;

        /// <summary>
        /// Helper to provide an invalid AppleNativeLibrary object.
        /// </summary>
        public static AppleNativeLibrary Invalid => new AppleNativeLibrary(string.Empty, string.Empty, string.Empty, string.Empty);

        /// <summary>
        /// Returns true when the combination of the Path/FileName exists
        /// </summary>
        public bool IsValid => ((FileName != string.Empty) && (Platform != string.Empty) && (Path != string.Empty)) && (Directory.Exists(FullPath) || File.Exists(FullPath));
    }
}
#endif // (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX))
