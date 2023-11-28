using System;
using System.Collections.Generic;

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
        public RuntimeVersion? iOS { get => _osVersions[RuntimeOperatingSystem.iOS]; }
        public RuntimeVersion? macOS { get => _osVersions[RuntimeOperatingSystem.macOS]; }
        public RuntimeVersion? tvOS { get => _osVersions[RuntimeOperatingSystem.tvOS]; }

        // Access by OperatingSystem
        public RuntimeVersion? OperatingSystemVersion(RuntimeOperatingSystem operatingSystem)
        {
            if (!Enum.IsDefined(typeof(RuntimeOperatingSystem), operatingSystem) || (operatingSystem == RuntimeOperatingSystem.Unknown) || (operatingSystem == RuntimeOperatingSystem.RuntimeOperatingSystemCount))
            {
                return null;
            }

            return _osVersions[operatingSystem];
        }

        protected SortedList<RuntimeOperatingSystem, RuntimeVersion?> _osVersions;

        public IntroducedAttribute(string iOS = "", string macOS = "", string tvOS = "")
        {
            _osVersions = new SortedList<RuntimeOperatingSystem, RuntimeVersion?>();
            
            _osVersions[RuntimeOperatingSystem.iOS] = RuntimeVersion.FromString(iOS);
            _osVersions[RuntimeOperatingSystem.macOS] = RuntimeVersion.FromString(macOS);
            _osVersions[RuntimeOperatingSystem.tvOS] = RuntimeVersion.FromString(tvOS);
        }
    }

    /// <summary>
    /// UnavailableAttribute is used to indicate operating systems for which an API is *not* available.
    /// Example: [Apple.Core.Unavailable(RuntimeOperatingSystem.tvOS)] means that a given API is not available for tvOS, but is available for all other supported OSes.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class UnavailableAttribute : Attribute
    {
        public RuntimeOperatingSystem[] UnavailableOperatingSystems { get; protected set; }

        public UnavailableAttribute(params RuntimeOperatingSystem[] unavailableOperatingSystems)
        {
            UnavailableOperatingSystems = new RuntimeOperatingSystem[unavailableOperatingSystems.Length];
            unavailableOperatingSystems.CopyTo(UnavailableOperatingSystems, 0);
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
