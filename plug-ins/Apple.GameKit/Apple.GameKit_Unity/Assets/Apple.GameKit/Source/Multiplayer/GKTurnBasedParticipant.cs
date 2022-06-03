using System;
using System.Runtime.InteropServices;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    public class GKTurnBasedParticipant : InteropReference
    {
        #region Init & Dispose
        internal GKTurnBasedParticipant(IntPtr pointer) : base(pointer)
        {
        }

        [DllImport(InteropUtility.DLLName)]
        private static extern void GKTurnBasedParticipant_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                GKTurnBasedParticipant_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        #endregion
        
        #region Player
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKTurnBasedParticipant_GetPlayer(IntPtr pointer);

        public GKPlayer Player
        {
            get => PointerCast<GKPlayer>(GKTurnBasedParticipant_GetPlayer(Pointer));
        }
        #endregion
        
        #region Status
        [DllImport(InteropUtility.DLLName)]
        private static extern GKTurnBasedParticipantStatus GKTurnBasedParticipant_GetStatus(IntPtr pointer);

        /// <summary>
        /// The current status of the participant.
        /// </summary>
        public GKTurnBasedParticipantStatus Status
        {
            get => GKTurnBasedParticipant_GetStatus(Pointer);
        }
        #endregion
        
        #region LastTurnDate
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKTurnBasedParticipant_GetLastTurnDate(IntPtr pointer);

        /// <summary>
        /// The date and time that this participant last took a turn in the game.
        /// </summary>
        public DateTimeOffset LastTurnDate
        {
            get => DateTimeOffset.FromUnixTimeSeconds(GKTurnBasedParticipant_GetLastTurnDate(Pointer));
        }
        #endregion
        
        #region TimeoutDate
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKTurnBasedParticipant_GetTimeoutDate(IntPtr pointer);

        /// <summary>
        /// The date and time that the participant's turn timed out.
        /// </summary>
        public DateTimeOffset TimeoutDate
        {
            get => DateTimeOffset.FromUnixTimeSeconds(GKTurnBasedParticipant_GetTimeoutDate(Pointer));
        }
        #endregion
        
        #region MatchOutcome
        [DllImport(InteropUtility.DLLName)]
        private static extern GKTurnBasedMatch.Outcome GKTurnBasedParticipant_GetMatchOutcome(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKTurnBasedParticipant_SetMatchOutcome(IntPtr pointer, GKTurnBasedMatch.Outcome value);

        /// <summary>
        /// The end-state of this participant in the match.
        /// </summary>
        public GKTurnBasedMatch.Outcome MatchOutcome
        {
            get => GKTurnBasedParticipant_GetMatchOutcome(Pointer);
            set => GKTurnBasedParticipant_SetMatchOutcome(Pointer, value);
        }
        #endregion

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
    }
}