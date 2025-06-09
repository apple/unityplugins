using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.GameKit
{
    /// <summary>
    /// A challenge issued by the local player to another player.
    /// </summary>
    /// <symbol>c:objc(cs)GKChallenge</symbol>
    [Deprecated("Deprecated", iOS: "19.0.0", macOS: "16.0.0", tvOS: "19.0.0", visionOS: "3.0.0")]
    public class GKChallenge : NSObject
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
        static GKChallenge()
        {
            Interop.GKChallenge_SetChallengeReceivedCallback(OnChallengeReceived);
            Interop.GKChallenge_SetChallengeCompletedCallback(OnChallengeCompleted);
            Interop.GKChallenge_SetChallengeOtherPlayerAcceptedCallback(OnChallengeOtherPlayerAccepted);
            Interop.GKChallenge_SetChallengeOtherPlayerCompletedCallback(OnChallengeOtherPlayerCompleted);
        }

        [MonoPInvokeCallback(typeof(InteropChallengeReceivedHandler))]
        private static void OnChallengeReceived(IntPtr player, IntPtr challenge)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => ChallengeReceived?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKChallenge>(challenge)));
        }

        [MonoPInvokeCallback(typeof(InteropChallengeOtherPlayerAcceptedHandler))]
        private static void OnChallengeOtherPlayerAccepted(IntPtr player, IntPtr challenge)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => ChallengeOtherPlayerAccepted?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKChallenge>(challenge)));
        }

        [MonoPInvokeCallback(typeof(InteropChallengeCompletedHandler))]
        private static void OnChallengeCompleted(IntPtr player, IntPtr challenge, IntPtr issuedByFriend)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => ChallengeCompleted?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKChallenge>(challenge), PointerCast<GKPlayer>(issuedByFriend)));
        }
        
        [MonoPInvokeCallback(typeof(InteropChallengeOtherPlayerCompletedHanlder))]
        private static void OnChallengeOtherPlayerCompleted(IntPtr player, IntPtr challenge, IntPtr issuedByFriend)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => ChallengeOtherPlayerCompleted?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKChallenge>(challenge), PointerCast<GKPlayer>(issuedByFriend)));
        }
        #endregion
        
        internal GKChallenge(IntPtr pointer) : base(pointer)
        {
        }
        
        /// <summary>
        /// The player who issues the challenge.
        /// </summary>
        public GKPlayer IssuingPlayer => PointerCast<GKPlayer>(Interop.GKChallenge_GetIssuingPlayer(Pointer));
        
        /// <summary>
        /// The player who receives the challenge.
        /// </summary>
        public GKPlayer ReceivingPlayer => PointerCast<GKPlayer>(Interop.GKChallenge_GetReceivingPlayer(Pointer));

        /// <summary>
        /// A text message that describes the challenge.
        /// </summary>
        public string Message => Interop.GKChallenge_GetMessage(Pointer);
        
        /// <summary>
        /// The current state of the challenge.
        /// </summary>
        public GKChallengeState State => Interop.GKChallenge_GetState(Pointer);
        
        /// <summary>
        /// The date the player issued the challenge.
        /// </summary>
        public DateTimeOffset IssueDate => DateTimeOffsetExtensions.FromUnixTimeSeconds(Interop.GKChallenge_GetIssueDate(Pointer));

        /// <summary>
        /// The date the challenged player completed the challenge.
        /// </summary>
        public DateTimeOffset CompletionDate => DateTimeOffsetExtensions.FromUnixTimeSeconds(Interop.GKChallenge_GetCompletionDate(Pointer));
        
        /// <summary>
        /// Declines a challenge that another player issues to the local player.
        /// </summary>
        public void Decline() => Interop.GKChallenge_Decline(Pointer);

        #region LoadReceivedChallenges

        /// <summary>
        /// Loads the list of outstanding challenges.
        /// </summary>
        /// <returns>The challenges the local player issued. If an error occurs, this parameter may be non-nil and contain partial challenge data that GameKit downloads.</returns>
        public static Task<NSArray<GKChallenge>> LoadReceivedChallenges()
        {
            var tcs = InteropTasks.Create<NSArray<GKChallenge>>(out var taskId);
            Interop.GKChallenge_LoadReceivedChallenges(taskId, OnLoadReceivedChallenges, OnLoadReceivedChallengesError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadReceivedChallenges(long taskId, IntPtr nsArrayPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSArray<GKChallenge>>(nsArrayPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadReceivedChallengesError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSArray<GKChallenge>>(taskId, new GameKitException(errorPointer));
        }
        #endregion
        
        public GKChallengeType ChallengeType => Interop.GKChallenge_GetChallengeType(Pointer);

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

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKChallenge_SetChallengeReceivedCallback(InteropChallengeReceivedHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKChallenge_SetChallengeOtherPlayerAcceptedCallback(InteropChallengeOtherPlayerAcceptedHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKChallenge_SetChallengeCompletedCallback(InteropChallengeCompletedHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKChallenge_SetChallengeOtherPlayerCompletedCallback(InteropChallengeOtherPlayerCompletedHanlder callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKChallenge_GetIssuingPlayer(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKChallenge_GetReceivingPlayer(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKChallenge_GetMessage(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKChallengeState GKChallenge_GetState(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKChallenge_GetIssueDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKChallenge_GetCompletionDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKChallenge_Decline(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKChallenge_LoadReceivedChallenges(long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKChallengeType GKChallenge_GetChallengeType(IntPtr pointer);
        }
    }
}
