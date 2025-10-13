using System;
using System.Runtime.InteropServices;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.GameKit
{
    /// <summary>
    /// A `GKLeaderboardScore` object represents a score on a leaderboard for scores you report for challenges or turn-based games.
    /// </summary>
    /// <symbol>c:objc(cs)GKLeaderboardScore</symbol>
    [Introduced(iOS: "14.0.0", macOS: "11.0.0", tvOS: "14.0.0")]
    public class GKLeaderboardScore : NSObject
    {
        internal GKLeaderboardScore(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// An integer value that your game uses.
        /// </summary>
        /// <symbol>c:objc(cs)GKLeaderboardScore(py)context</symbol>
        public ulong Context
        {
            get => Interop.GKLeaderboardScore_GetContext(Pointer);
            set => Interop.GKLeaderboardScore_SetContext(Pointer, value);
        }

        /// <summary>
        /// The ID that Game Center uses for the leaderboard.
        /// </summary>
        /// <symbol>c:objc(cs)GKLeaderboardScore(py)leaderboardID</symbol>
        public string LeaderboardID
        {
            get => Interop.GKLeaderboardScore_GetLeaderboardID(Pointer);
            set => Interop.GKLeaderboardScore_SetLeaderboardID(Pointer, value);
        }

        /// <summary>
        /// The player who earns the score.
        /// </summary>
        /// <symbol>c:objc(cs)GKLeaderboardScore(py)player</symbol>
        public GKPlayer Player
        {
            get => PointerCast<GKPlayer>(Interop.GKLeaderboardScore_GetPlayer(Pointer));
            set => Interop.GKLeaderboardScore_SetPlayer(Pointer, value?.Pointer ?? IntPtr.Zero);
        }

        /// <summary>
        /// The score that the player earns.
        /// </summary>
        /// <symbol>c:objc(cs)GKLeaderboardScore(py)value</symbol>
        public long Value
        {
            get => Interop.GKLeaderboardScore_GetValue(Pointer);
            set => Interop.GKLeaderboardScore_SetValue(Pointer, value);
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern ulong GKLeaderboardScore_GetContext(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLeaderboardScore_SetContext(IntPtr pointer, ulong context);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKLeaderboardScore_GetLeaderboardID(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLeaderboardScore_SetLeaderboardID(IntPtr pointer, string leaderboardID);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKLeaderboardScore_GetPlayer(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLeaderboardScore_SetPlayer(IntPtr pointer, IntPtr playerPtr);
            [DllImport(InteropUtility.DLLName)]
            public static extern long GKLeaderboardScore_GetValue(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKLeaderboardScore_SetValue(IntPtr pointer, long value);
        }
    }
}
