using Apple.Core;

namespace Apple.GameKit
{
    /// <summary>
    /// Describes the release state of an App Store Connect resource, such as an Achievement or Leaderboard.
    /// </summary>
    /// <symbol>c:@E@GKReleaseState</symbol>
    [Introduced(iOS: "18.4.0", macOS: "15.4.0", tvOS: "18.4.0", visionOS: "2.4.0")]
    public enum GKReleaseState : ulong
    {
        /// <summary>
        /// The system can't determine the release state of the resource.
        /// </summary>
        /// <symbol>c:@E@GKReleaseState@GKReleaseStateUnknown</symbol>
        Unknown = 0,

        /// <summary>
        /// The resource is associated with a release in App Store Connect. This has no relationship with the "archived" state of a resource (i.e., A resource can be release _and_ archived).
        /// </summary>
        /// <symbol>c:@E@GKReleaseState@GKReleaseStateReleased</symbol>
        Released = 1,

        /// <summary>
        /// The resource has been created in App Store Connect but isn't yet associated with a released version of an App.
        /// </summary>
        /// <symbol>c:@E@GKReleaseState@GKReleaseStatePrereleased</symbol>
        Prereleased = 2
    }
}
