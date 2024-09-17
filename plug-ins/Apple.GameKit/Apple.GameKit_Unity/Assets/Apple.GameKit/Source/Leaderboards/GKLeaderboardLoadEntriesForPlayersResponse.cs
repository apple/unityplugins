using System;
using System.Runtime.InteropServices;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.GameKit.Leaderboards
{
    [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
    [StructLayout(LayoutKind.Sequential)]
    public struct GKLeaderboardLoadEntriesForPlayersResponse
    {
        /// <summary>
        /// The score for the local player, or nil if the player has no score.
        /// </summary>
        public GKLeaderboard.Entry LocalPlayerEntry { get; set; }
        
        /// <summary>
        /// The scores for the players during the specified time period, including the local player’s score if it exists.
        /// </summary>
        public NSArray<GKLeaderboard.Entry> Entries { get; set; }
    }
}
