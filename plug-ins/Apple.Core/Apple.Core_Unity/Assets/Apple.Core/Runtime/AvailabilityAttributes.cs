using System;
using System.Collections.Generic;
using System.Linq;

namespace Apple.Core
{
    public abstract class AvailabilityAttributeBase : Attribute
    {
        public abstract bool IsAvailable(RuntimeEnvironment env);
    }

    public abstract class RuntimeEnvironmentAttributeBase : AvailabilityAttributeBase
    {
        protected string _message;
        public string Message { get => _message; }

        // Access by property
        public RuntimeVersion? iOS { get => _osVersions[RuntimeOperatingSystem.iOS]; }
        public RuntimeVersion? macOS { get => _osVersions[RuntimeOperatingSystem.macOS]; }
        public RuntimeVersion? tvOS { get => _osVersions[RuntimeOperatingSystem.tvOS]; }
        public RuntimeVersion? visionOS { get => _osVersions[RuntimeOperatingSystem.visionOS]; }

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

        public RuntimeEnvironmentAttributeBase(string iOS, string macOS, string tvOS, string visionOS)
        {
            _osVersions = new SortedList<RuntimeOperatingSystem, RuntimeVersion?>();
            
            _osVersions[RuntimeOperatingSystem.iOS] = RuntimeVersion.FromString(iOS);
            _osVersions[RuntimeOperatingSystem.macOS] = RuntimeVersion.FromString(macOS);
            _osVersions[RuntimeOperatingSystem.tvOS] = RuntimeVersion.FromString(tvOS);
            _osVersions[RuntimeOperatingSystem.visionOS] = RuntimeVersion.FromString(visionOS);
        }

        public RuntimeEnvironmentAttributeBase(string message, string iOS, string macOS, string tvOS, string visionOS) : this(iOS, macOS, tvOS, visionOS)
        {
            _message = message;
        }
    }

    /// <summary>
    /// IntroducedAttribute is used to indicate for which OS a given API was introduced and became available.
    /// Note: Attributes should be declared with named parameters and a version string which represents the major.minor version of the desired platform SDK
    /// Example: [Apple.Core.Introduced(iOS: "13.5")] or [Apple.Core.Introduced(iOS: "14", macOS: "11")]
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class IntroducedAttribute : RuntimeEnvironmentAttributeBase
    {
        public IntroducedAttribute(string iOS = "", string macOS = "", string tvOS = "", string visionOS = "") : base(iOS, macOS, tvOS, visionOS)
        {
        }

        public override bool IsAvailable(RuntimeEnvironment env)
        {
            var osVersion = _osVersions[env.RuntimeOperatingSystem];
            return 
                osVersion.HasValue &&
                ((env.VersionNumber.Major > osVersion.Value.Major) ||
                 (env.VersionNumber.Major == osVersion.Value.Major && env.VersionNumber.Minor >= osVersion.Value.Minor));
        }
    }

    /// <summary>
    /// UnavailableAttribute is used to indicate operating systems for which an API is *not* available.
    /// Example: [Apple.Core.Unavailable(RuntimeOperatingSystem.tvOS)] means that a given API is not available for tvOS, but is available for all other supported OSes.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class UnavailableAttribute : AvailabilityAttributeBase
    {
        public RuntimeOperatingSystem[] UnavailableOperatingSystems { get; protected set; }

        public UnavailableAttribute(params RuntimeOperatingSystem[] unavailableOperatingSystems)
        {
            UnavailableOperatingSystems = new RuntimeOperatingSystem[unavailableOperatingSystems.Length];
            unavailableOperatingSystems.CopyTo(UnavailableOperatingSystems, 0);
        }

        public bool IsAvailable(RuntimeOperatingSystem os) => !UnavailableOperatingSystems?.Contains(os) ?? true;
        public override bool IsAvailable(RuntimeEnvironment env) => IsAvailable(env.RuntimeOperatingSystem);
    }

    /// <summary>
    /// DeprecatedAttribute is used to indicate when an API became deprecated.
    /// This attribute also includes a message field which should be used to indicate the replacement API to be used.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class DeprecatedAttribute : RuntimeEnvironmentAttributeBase
    {
        public DeprecatedAttribute(string message, string iOS = "", string macOS = "", string tvOS = "", string visionOS = "") : base(message, iOS, macOS, tvOS, visionOS)
        {
        }

        // deprecated APIs are still available
        public override bool IsAvailable(RuntimeEnvironment env) => true;
    }

    /// <summary>
    /// RenamedAttribute is used to indicate when an API has been renamed.
    /// This attribute also includes a message field which should be used to indicate the former API.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class RenamedAttribute : AvailabilityAttributeBase
    {
        public string NewName { get; private set; }

        public RenamedAttribute(string newName)
        {
            NewName = newName;
        }

        // renamed APIs are still available unless marked as unavailable with other attributes.
        public override bool IsAvailable(RuntimeEnvironment env) => true;
    }

    /// <summary>
    /// ObsoletedAttribute is used to indicate when an API became obsolete.
    /// This attribute also includes a message field which should be used to indicate the replacement API to be used and whether or not this API will continue to be evaluated.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class ObsoletedAttribute : RuntimeEnvironmentAttributeBase
    {
        public ObsoletedAttribute(string message, string iOS = "", string macOS = "", string tvOS = "", string visionOS = "") : base(message, iOS, macOS, tvOS, visionOS)
        {
        }

        public override bool IsAvailable(RuntimeEnvironment env)
        {
            var osVersion = _osVersions[env.RuntimeOperatingSystem];
            var isObsoleted = 
                osVersion.HasValue &&
                ((env.VersionNumber.Major > osVersion.Value.Major) ||
                 (env.VersionNumber.Major == osVersion.Value.Major && env.VersionNumber.Minor >= osVersion.Value.Minor));

            return !isObsoleted;
        }
    }
}
