using System;
using Apple.Core;

namespace Apple.GameKit
{
    public static class GKErrorDomain
    {
        public static readonly string Name = nameof(GKErrorDomain);
    }

    public enum GKErrorCode : int
    {
        // Unity-Specific GameKit plug-in Error Codes
        InvalidOperationNotAuthenticated = -1,
        NoLeaderboardFound = -2,
        NoParticipantsInMatch = -3,
        PlayerNotFound = -4,
        NoLeaderboardSetFound = -5,
        NoRealTimeMatch = -6,
        UnsupportedOperationForOSVersion = -7,
        NoAchievementDescriptionFound = -8,
        NoAchievementDescriptionImageFound = -9,
        GameCenterDashboardAlreadyShown = -10,

        // GameKit.framework Error Codes
        UnknownError = 1,
        Cancelled = 2,
        CommunicationsFailure= 3,
        UserDenied = 4,
        InvalidCredentials = 5,
        NotAuthenticated = 6,
        AuthenticationInProgress = 7,
        InvalidPlayer = 8,
        ScoreNotSet = 9,
        ParentalControlsBlocked = 10,
        PlayerStatusExceedsMaximumLength = 11,
        PlayerStatusInvalid = 12,
        MatchRequestInvalid = 13,
        Underage = 14,
        GameUnrecognized = 15,
        NotSupported = 16,
        InvalidParameter = 17,
        UnexpectedConnection = 18,

        [Deprecated("Deprecated", iOS: "26.0.0", macOS: "26.0.0", tvOS: "26.0.0", visionOS: "26.0.0")]
        ChallengeInvalid = 19,

        TurnBasedMatchDataTooLarge = 20,
        TurnBasedTooManySessions = 21,
        TurnBasedInvalidParticipant = 22,
        TurnBasedInvalidTurn = 23,
        TurnBasedInvalidState = 24,

        // Values below this point were renumbered in the iOS 17.2 / MacOS 14.2
        // time frame to fix a discrepancy discovered between the C# version of
        // this enum and the actual values used by GameKit. The "Offline = 25"
        // value (which is not actually defined by GameKit) was included by
        // mistake, causing all subsequent values to be off by one. This
        // direcrepancy has now been corrected and the "Offline" value has been
        // removed. If your code depended on the "Offline" value being defined
        // then you will need to modify your code accordingly.

        [Obsolete("Offline value should not be used. It was mistakenly included in prior versions of this enumeration.", error: true)]
        Offline = 25,

        InvitationsDisabled = 25,
        PlayerPhotoFailure = 26,
        UbiquityContainerUnavailable = 27,
        MatchNotConnected = 28,
        GameSessionRequestInvalid = 29,
        RestrictedToAutomatch = 30,
        APINotAvailable = 31,

        [Introduced(iOS: "14.0.0", macOS: "11.0.0", tvOS: "14.0.0")]
        NotAuthorized = 32,

        [Introduced(iOS: "14.0.0", macOS: "11.0.0", tvOS: "14.0.0")]
        ConnectionTimeout = 33,

        [Introduced(iOS: "14.0.0", macOS: "11.0.0", tvOS: "14.0.0", visionOS: "1.0.0")]
        APIObsolete = 34,

        [Introduced(iOS: "17.2.0", macOS: "14.2.0", tvOS: "17.2.0", visionOS: "1.1.0")]
        ICloudUnavailable = 35,

        [Introduced(iOS: "17.2.0", macOS: "14.2.0", tvOS: "17.2.0", visionOS: "1.1.0")]
        LockdownMode = 36,

        [Introduced(iOS: "17.4.0", macOS: "14.4.0", tvOS: "17.4.0", visionOS: "1.1.0")]
        AppUnlisted = 37,

        [Introduced(iOS: "18.4.0", macOS: "15.4.0", tvOS: "18.4.0", visionOS: "2.4.0")]
        DebugMode = 38,

        [Introduced(iOS: "14.5.0", macOS: "11.3.0", tvOS: "14.5.0")]
        FriendListDescriptionMissing = 100,

        [Introduced(iOS: "14.5.0", macOS: "11.3.0", tvOS: "14.5.0")]
        FriendListRestricted = 101,

        [Introduced(iOS: "14.5.0", macOS: "11.3.0", tvOS: "14.5.0")]
        FriendListDenied = 102,

        [Introduced(iOS: "15.0.0", macOS: "12.0.0", tvOS: "15.0.0")]
        FriendRequestNotAvailable = 103,
    }
}
