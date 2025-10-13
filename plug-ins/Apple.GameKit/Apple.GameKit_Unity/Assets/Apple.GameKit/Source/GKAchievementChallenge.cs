using System;
using System.Runtime.InteropServices;
using Apple.Core;

namespace Apple.GameKit
{
    /// <summary>
    /// A type of challenge where a player must earn another player's achievement.
    /// </summary>
    /// <symbol>c:objc(cs)GKAchievementChallenge</symbol>
    [Deprecated("Deprecated", iOS: "26.0.0", macOS: "26.0.0", tvOS: "26.0.0", visionOS: "26.0.0")]
    public class GKAchievementChallenge : GKChallenge
    {
        internal GKAchievementChallenge(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// The achievement that the player must earn to complete the challenge.
        /// </summary>
        public GKAchievement Achievement => PointerCast<GKAchievement>(Interop.GKAchievementChallenge_GetAchievement(Pointer));

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKAchievementChallenge_GetAchievement(IntPtr pointer);
        }
    }
}