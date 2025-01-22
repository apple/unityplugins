using Apple.Core;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// Possible modes that a multiplayer game uses to find matches.
    /// </summary>
    [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
    public enum GKMatchmakingMode : long
    {
        Default = 0,
        /// <summary>
        /// A mode that matches the local player only with nearby players.
        /// </summary>
        NearbyOnly = 1,
        /// <summary>
        /// A mode that matches the local player only with players who are also actively looking for a match.
        /// </summary>
        AutomatchOnly = 2,
        /// <summary>
        /// A mode that matches the local player only with players who they invite, and doesn't use automatch to fill empty slots.
        /// </summary>
        [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "1.0")]
        InviteOnly = 3
    }
}