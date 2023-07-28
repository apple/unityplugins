using System;

namespace Apple.Core
{
    /// <summary>
    /// IntroducedAttribute is used to indicate for which OS a given API was introduced and became available.
    /// Note: Attributes should be declared with named parameters and a version string which represents the major.minor version of the desired platform SDK
    /// Example: [Apple.Core.Introduced(iOS: "13.5")] or [Apple.Core.Introduced(iOS: "14", macOS: "11")]
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class IntroducedAttribute : Attribute
    {
        // Access by property
        public RuntimeVersion? iOS { get => _osVersionNumbers[(int)RuntimeOperatingSystem.iOS]; }
        public RuntimeVersion? macOS { get => _osVersionNumbers[(int)RuntimeOperatingSystem.macOS]; }
        public RuntimeVersion? tvOS { get => _osVersionNumbers[(int)RuntimeOperatingSystem.tvOS]; }

        // Access by OperatingSystem
        public RuntimeVersion? OperatingSystemVersion(RuntimeOperatingSystem operatingSystem)
        {
            return _osVersionNumbers[(int)operatingSystem];
        }

        protected RuntimeVersion?[] _osVersionNumbers;

        public IntroducedAttribute(string iOS = "", string macOS = "", string tvOS = "")
        {
            _osVersionNumbers = new RuntimeVersion?[(int)RuntimeOperatingSystem.RuntimeOperatingSystemCount];
            
            _osVersionNumbers[(int)RuntimeOperatingSystem.iOS] = RuntimeVersion.FromString(iOS);
            _osVersionNumbers[(int)RuntimeOperatingSystem.macOS] = RuntimeVersion.FromString(macOS);
            _osVersionNumbers[(int)RuntimeOperatingSystem.tvOS] = RuntimeVersion.FromString(tvOS);
        }
    }

    /// <summary>
    /// DeprecatedAttribute is used to indicate when an API became deprecated.
    /// This attribute also includes a message field which should be used to indicate the replacement API to be used.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class DeprecatedAttribute : IntroducedAttribute
    {
        protected string _message;
        public string Message { get => _message; }

        public DeprecatedAttribute(string message, string iOS = "", string macOS = "", string tvOS = "") : base(iOS, macOS, tvOS)
        {
            _message = message;
        }
    }

    /// <summary>
    /// RenamedAttribute is used to indicate when an API has been renamed.
    /// This attribute also includes a message field which should be used to indicate the former API.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class RenamedAttribute : DeprecatedAttribute
    {
        public RenamedAttribute(string message, string iOS = "", string macOS = "", string tvOS = "") : base(message, iOS, macOS, tvOS) {}
    }    

    /// <summary>
    /// ObsoletedAttribute is used to indicate when an API became obsolete.
    /// This attribute also includes a message field which should be used to indicate the replacement API to be used and whether or not this API will continue to be evaluated.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class ObsoletedAttribute : DeprecatedAttribute
    {
        public ObsoletedAttribute(string message, string iOS = "", string macOS = "", string tvOS = "") : base(message, iOS, macOS, tvOS) {}
    }
}
