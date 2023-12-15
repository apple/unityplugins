using System;
using System.Runtime.InteropServices;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    public class GKTurnBasedParticipant : NSObject
    {
        internal GKTurnBasedParticipant(IntPtr pointer) : base(pointer)
        {
        }
        
        public GKPlayer Player => PointerCast<GKPlayer>(Interop.GKTurnBasedParticipant_GetPlayer(Pointer));

        /// <summary>
        /// The current status of the participant.
        /// </summary>
        public GKTurnBasedParticipantStatus Status => Interop.GKTurnBasedParticipant_GetStatus(Pointer);

        /// <summary>
        /// The date and time that this participant last took a turn in the game.
        /// </summary>
        public DateTimeOffset LastTurnDate => DateTimeOffset.FromUnixTimeSeconds(Interop.GKTurnBasedParticipant_GetLastTurnDate(Pointer));

        /// <summary>
        /// The date and time that the participant's turn timed out.
        /// </summary>
        public DateTimeOffset TimeoutDate => DateTimeOffset.FromUnixTimeSeconds(Interop.GKTurnBasedParticipant_GetTimeoutDate(Pointer));

        /// <summary>
        /// The end-state of this participant in the match.
        /// </summary>
        public GKTurnBasedMatch.Outcome MatchOutcome
        {
            get => Interop.GKTurnBasedParticipant_GetMatchOutcome(Pointer);
            set => Interop.GKTurnBasedParticipant_SetMatchOutcome(Pointer, value);
        }

        /// <summary>
        /// The information that describes a participant in a turn-based match.
        /// </summary>
        public enum GKTurnBasedParticipantStatus : long
        {
            /// <summary>
            /// The participant is in an unexpected state.
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// The participant was invited to the match, but has not responded to the invitation.
            /// </summary>
            Invited = 1,
            /// <summary>
            /// The participant declined the invitation to join the match. When a participant declines an invitation to join a match, the match is automatically terminated.
            /// </summary>
            Declined = 2,
            /// <summary>
            /// The participant represents an unfilled position in the match that Game Center promises to fill when needed. When you make this participant the next person to take a turn in the match, Game Center fills the position and updates the status and playerID properties.
            /// </summary>
            Matching = 3,
            /// <summary>
            /// The participant has joined the match and is an active player in it.
            /// </summary>
            Active = 4,
            /// <summary>
            /// The participant has exited the match. Your game sets the matchOutcome property to state why the participant left the match.
            /// </summary>
            Done = 5
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKTurnBasedParticipant_GetPlayer(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKTurnBasedParticipantStatus GKTurnBasedParticipant_GetStatus(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern long GKTurnBasedParticipant_GetLastTurnDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern long GKTurnBasedParticipant_GetTimeoutDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKTurnBasedMatch.Outcome GKTurnBasedParticipant_GetMatchOutcome(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedParticipant_SetMatchOutcome(IntPtr pointer, GKTurnBasedMatch.Outcome value);
        }
    }
}
