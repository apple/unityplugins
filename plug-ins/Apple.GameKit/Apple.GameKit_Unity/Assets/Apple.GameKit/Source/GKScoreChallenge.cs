using System;

namespace Apple.GameKit
{
    /// <summary>
    /// A type of challenge where a player must beat the leaderboard score of another player.
    /// </summary>
    [Obsolete("Since GKScore was deprecated in iOS 14, tvOS 14, and macOS 11, this challenge type is deprecated.")]
    public class GKScoreChallenge : GKChallenge
    {
        #region Init & Dispose
        internal GKScoreChallenge(IntPtr pointer) : base(pointer)
        {
        }
        #endregion
    }
}