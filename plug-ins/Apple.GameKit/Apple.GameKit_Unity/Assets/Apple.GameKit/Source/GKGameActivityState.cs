using Apple.Core;

namespace Apple.GameKit
{
    /// <symbol>c:@E@GKGameActivityState</symbol>
    [Introduced(iOS: "26.0.0", macOS: "26.0.0", tvOS: "26.0.0", visionOS: "26.0.0")]
    public enum GKGameActivityState : ulong
    {
        /// <summary>
        /// The game activity is initialized but has not started.
        /// </summary>
        /// <symbol>c:@E@GKGameActivityState@GKGameActivityStateInitialized</symbol>
        Initialized = 0,

        /// <summary>
        /// The game activity is active.
        /// </summary>
        /// <symbol>c:@E@GKGameActivityState@GKGameActivityStateActive</symbol>
        Active = 1,

        /// <summary>
        /// The game activity is paused.
        /// </summary>
        /// <symbol>c:@E@GKGameActivityState@GKGameActivityStatePaused</symbol>
        Paused = 2,

        /// <summary>
        /// The game activity has ended. This is a terminal state.
        /// </summary>
        /// <symbol>c:@E@GKGameActivityState@GKGameActivityStateEnded</symbol>
        Ended = 4
    }
}
