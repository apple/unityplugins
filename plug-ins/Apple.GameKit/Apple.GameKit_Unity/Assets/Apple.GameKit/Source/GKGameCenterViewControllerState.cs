using Apple.Core;

namespace Apple.GameKit
{
    /// <symbol>c:@E@GKGameCenterViewControllerState</symbol>
    public enum GKGameCenterViewControllerState : long
    {
        /// <summary>
        /// The view controller should present the default screen.
        /// </summary>
        /// <symbol>c:@E@GKGameCenterViewControllerState@GKGameCenterViewControllerStateDefault</symbol>
        Default = -1,

        /// <summary>
        /// The view controller should present leaderboard sets or leaderboards if there are no sets.
        /// </summary>
        /// <symbol>c:@E@GKGameCenterViewControllerState@GKGameCenterViewControllerStateLeaderboards</symbol>
        Leaderboards = 0,

        /// <summary>
        /// The view controller should present a list of achievements.
        /// </summary>
        /// <symbol>c:@E@GKGameCenterViewControllerState@GKGameCenterViewControllerStateAchievements</symbol>
        Achievements = 1,

        /// <summary>
        /// The view controller should present a list of challenges.
        /// </summary>
        /// <symbol>c:@E@GKGameCenterViewControllerState@GKGameCenterViewControllerStateChallenges</symbol>
        [Deprecated("Deprecated", iOS: "26.0.0", macOS: "26.0.0", tvOS: "26.0.0", visionOS: "26.0.0")]
        Challenges = 2,

        /// <summary>
        /// The view controller should present the local player's profile.
        /// </summary>
        /// <symbol>c:@E@GKGameCenterViewControllerState@GKGameCenterViewControllerStateLocalPlayerProfile</symbol>
        [Introduced(iOS: "14.0.0", macOS: "11.0.0", tvOS: "14.0.0")]
        LocalPlayerProfile = 3,

        /// <summary>
        /// The view controller should present the dashboard.
        /// </summary>
        /// <symbol>c:@E@GKGameCenterViewControllerState@GKGameCenterViewControllerStateDashboard</symbol>
        [Introduced(iOS: "14.0.0", macOS: "11.0.0", tvOS: "14.0.0")]
        Dashboard = 4,

        /// <summary>
        /// The view controller should present the friends list.
        /// </summary>
        /// <symbol>c:@E@GKGameCenterViewControllerState@GKGameCenterViewControllerStateLocalPlayerFriendsList</symbol>
        [Introduced(iOS: "15.0.0", macOS: "12.0.0", tvOS: "15.0.0")]
        LocalPlayerFriendsList = 5
    }
}
