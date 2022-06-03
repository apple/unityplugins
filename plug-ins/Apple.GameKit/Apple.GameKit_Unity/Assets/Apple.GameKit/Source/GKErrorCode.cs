namespace Apple.GameKit
{
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
        ChallengeInvalid = 19,
        TurnBasedMatchDataTooLarge = 20,
        TurnBasedTooManySessions = 21,
        TurnBasedInvalidParticipant = 22,
        TurnBasedInvalidTurn = 23,
        TurnBasedInvalidState = 24,
        Offline = 25,
        InvitationsDisabled = 26,
        PlayerPhotoFailure = 27,
        UbiquityContainerUnavailable = 28,
        MatchNotConnected = 29,
        GameSessionRequestInvalid = 30,
        RestrictedToAutomatch = 31,
        APINotAvailable = 32,
        NotAuthorized = 33,
        ConnectionTimeout = 34,
        APIObsolete = 35
    }
}