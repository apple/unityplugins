using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit
{
    /// <summary>
    /// A challenge issued by the local player to another player.
    /// </summary>
    public class GKChallenge : InteropReference
    {
        #region Delegates
        public delegate void ChallengeReceivedHandler(GKPlayer player, GKChallenge challenge);
        private delegate void InteropChallengeReceivedHandler(IntPtr player, IntPtr challenge);

        public delegate void ChallengeOtherPlayerAcceptedHandler(GKPlayer acceptingPlayer, GKChallenge challenge);
        private delegate void InteropChallengeOtherPlayerAcceptedHandler(IntPtr player, IntPtr challenge);

        public delegate void ChallengeCompletedHandler(GKPlayer player, GKChallenge challenge, GKPlayer issuingPlayer);
        private delegate void InteropChallengeCompletedHandler(IntPtr player, IntPtr challenge, IntPtr issuingPlayer);

        public delegate void ChallengeOtherPlayerCompletedHandler(GKPlayer player, GKChallenge challenge, GKPlayer completingPlayer);
        private delegate void InteropChallengeOtherPlayerCompletedHanlder(IntPtr player, IntPtr challenge, IntPtr completingPlayer);
        #endregion
        
        #region Static Events
        /// <summary>
        /// Handles when the local player issues a challenge but the other player doesn't want to respond immediately.
        /// </summary>
        public static event ChallengeReceivedHandler ChallengeReceived;

        /// <summary>
        /// Handles when the local player issues a challenge and the other player accepts.
        /// </summary>
        public static event ChallengeOtherPlayerAcceptedHandler ChallengeOtherPlayerAccepted;

        /// <summary>
        /// Handles when the local player completes a challenge that a friend issues.
        /// </summary>
        public static event ChallengeCompletedHandler ChallengeCompleted;

        /// <summary>
        /// Handles when a friend completes a challenge that the local player issues.
        /// </summary>
        public static event ChallengeOtherPlayerCompletedHandler ChallengeOtherPlayerCompleted;
        #endregion
        
        #region Static Event Registration
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKChallenge_SetChallengeReceivedCallback(InteropChallengeReceivedHandler callback);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKChallenge_SetChallengeOtherPlayerAcceptedCallback(InteropChallengeOtherPlayerAcceptedHandler callback);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKChallenge_SetChallengeCompletedCallback(InteropChallengeCompletedHandler callback);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKChallenge_SetChallengeOtherPlayerCompletedCallback(InteropChallengeOtherPlayerCompletedHanlder callback);
        static GKChallenge()
        {
            GKChallenge_SetChallengeReceivedCallback(OnChallengeReceived);
            GKChallenge_SetChallengeCompletedCallback(OnChallengeCompleted);
            GKChallenge_SetChallengeOtherPlayerAcceptedCallback(OnChallengeOtherPlayerAccepted);
            GKChallenge_SetChallengeOtherPlayerCompletedCallback(OnChallengeOtherPlayerCompleted);
        }

        [MonoPInvokeCallback(typeof(InteropChallengeReceivedHandler))]
        private static void OnChallengeReceived(IntPtr player, IntPtr challenge)
        {
            ChallengeReceived?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKChallenge>(challenge));
        }

        [MonoPInvokeCallback(typeof(InteropChallengeOtherPlayerAcceptedHandler))]
        private static void OnChallengeOtherPlayerAccepted(IntPtr player, IntPtr challenge)
        {
            ChallengeOtherPlayerAccepted?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKChallenge>(challenge));
        }

        [MonoPInvokeCallback(typeof(InteropChallengeCompletedHandler))]
        private static void OnChallengeCompleted(IntPtr player, IntPtr challenge, IntPtr issuedByFriend)
        {
            ChallengeCompleted?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKChallenge>(challenge), PointerCast<GKPlayer>(issuedByFriend));
        }
        
        [MonoPInvokeCallback(typeof(InteropChallengeOtherPlayerCompletedHanlder))]
        private static void OnChallengeOtherPlayerCompleted(IntPtr player, IntPtr challenge, IntPtr issuedByFriend)
        {
            ChallengeOtherPlayerCompleted?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKChallenge>(challenge), PointerCast<GKPlayer>(issuedByFriend));
        }
        #endregion
        
        #region Init & Dispose
        internal GKChallenge(IntPtr pointer) : base(pointer)
        {
        }

        [DllImport(InteropUtility.DLLName)]
        private static extern void GKChallenge_Free(IntPtr pointer);
        
        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                GKChallenge_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        #endregion
        
        #region IssuingPlayer
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKChallenge_GetIssuingPlayer(IntPtr pointer);

        /// <summary>
        /// The player who issues the challenge.
        /// </summary>
        public GKPlayer IssuingPlayer => PointerCast<GKPlayer>(GKChallenge_GetIssuingPlayer(Pointer));

        #endregion
        
        #region ReceivingPlayer
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKChallenge_GetReceivingPlayer(IntPtr pointer);

        /// <summary>
        /// The player who receives the challenge.
        /// </summary>
        public GKPlayer ReceivingPlayer => PointerCast<GKPlayer>(GKChallenge_GetReceivingPlayer(Pointer));

        #endregion
        
        #region Message
        [DllImport(InteropUtility.DLLName)]
        private static extern string GKChallenge_GetMessage(IntPtr pointer);

        /// <summary>
        /// A text message that describes the challenge.
        /// </summary>
        public string Message
        {
            get => GKChallenge_GetMessage(Pointer);
        }

        #endregion
        
        #region State
        [DllImport(InteropUtility.DLLName)]
        private static extern GKChallengeState GKChallenge_GetState(IntPtr pointer);

        /// <summary>
        /// The current state of the challenge.
        /// </summary>
        public GKChallengeState State
        {
            get => GKChallenge_GetState(Pointer);
        }

        #endregion
        
        #region IssueDate
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKChallenge_GetIssueDate(IntPtr pointer);

        /// <summary>
        /// The date the player issued the challenge.
        /// </summary>
        public DateTimeOffset IssueDate
        {
            get => DateTimeOffset.FromUnixTimeSeconds(GKChallenge_GetIssueDate(Pointer));
        }
        #endregion
        
        #region CompletionDate
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKChallenge_GetCompletionDate(IntPtr pointer);

        /// <summary>
        /// The date the challenged player completed the challenge.
        /// </summary>
        public DateTimeOffset CompletionDate
        {
            get => DateTimeOffset.FromUnixTimeSeconds(GKChallenge_GetCompletionDate(Pointer));
        }
        #endregion
        
        #region Decline
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKChallenge_Decline(IntPtr pointer);

        /// <summary>
        /// Declines a challenge that another player issues to the local player.
        /// </summary>
        public void Decline()
        {
            GKChallenge_Decline(Pointer);
        }
        #endregion

        #region LoadReceivedChallenges
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKChallenge_LoadReceivedChallenges(long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);

        /// <summary>
        /// Loads the list of outstanding challenges.
        /// </summary>
        /// <returns>The challenges the local player issued. If an error occurs, this parameter may be non-nil and contain partial challenge data that GameKit downloads.</returns>
        public static Task<NSArray<GKChallenge>> LoadReceivedChallenges()
        {
            var tcs = InteropTasks.Create<NSArray<GKChallenge>>(out var taskId);
            GKChallenge_LoadReceivedChallenges(taskId, OnLoadReceivedChallenges, OnLoadReceivedChallengesError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadReceivedChallenges(long taskId, IntPtr nsArrayPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, (NSArray<GKChallenge>)PointerCast<NSArrayGKChallenge>(nsArrayPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadReceivedChallengesError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKChallenge>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        #region ChallengeType
        [DllImport(InteropUtility.DLLName)]
        private static extern GKChallengeType GKChallenge_GetChallengeType(IntPtr pointer);

        public GKChallengeType ChallengeType
        {
            get => GKChallenge_GetChallengeType(Pointer);
        }
        #endregion

        public enum GKChallengeType : long
        {
            Unknown = -1,
            Score = 0,
            Achievement = 1
        }
        
        /// <summary>
        /// The state of a challenge.
        /// </summary>
        public enum GKChallengeState : long
        {
            /// <summary>
            /// The challenge isn't valid because an error occurred.
            /// </summary>
            Invalid = 0,
            /// <summary>
            /// The player issued a challenge, but the other player hasn't accepted or refused it.
            /// </summary>
            Pending = 1,
            /// <summary>
            /// The player successfully completed the challenge.
            /// </summary>
            Completed = 2,
            /// <summary>
            /// The player declined the challenge.
            /// </summary>
            Declined = 3
        }
        
    }
}