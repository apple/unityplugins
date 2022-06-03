using System;
using System.Runtime.InteropServices;
using Apple.Core.Runtime;

namespace Apple.GameKit.Leaderboards
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GKLeaderboardLoadEntriesResponse
    {
        /// <summary>
        /// The score for the local player.
        /// </summary>
        public GKLeaderboard.Entry LocalPlayerEntry { get; set; }
        
        /// <summary>
        /// The scores this method loads that match the playerScope, timeScope, and range parameters.
        /// </summary>
        public NSArray<GKLeaderboard.Entry> Entries { get; set; }
        
        /// <summary>
        /// The total number of players whose scores match the playerScope and timeScope parameters, but not the range parameter.
        /// </summary>
        public long TotalPlayerCount;
    }
}