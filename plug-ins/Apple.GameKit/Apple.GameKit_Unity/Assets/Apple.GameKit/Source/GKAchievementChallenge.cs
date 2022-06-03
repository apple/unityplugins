using System;
using System.Runtime.InteropServices;

namespace Apple.GameKit
{
    /// <summary>
    /// A type of challenge where a player must earn another playerâ€™s achievement.
    /// </summary>
    public class GKAchievementChallenge : GKChallenge
    {
        #region Init & Dispose
        internal GKAchievementChallenge(IntPtr pointer) : base(pointer)
        {
        }
        #endregion
        
        #region Achievement
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKAchievementChallenge_GetAchievement(IntPtr pointer);

        /// <summary>
        /// The achievement that the player must earn to complete the challenge.
        /// </summary>
        public GKAchievement Achievement
        {
            get => PointerCast<GKAchievement>(GKAchievementChallenge_GetAchievement(Pointer));
        }
        #endregion
    }
}