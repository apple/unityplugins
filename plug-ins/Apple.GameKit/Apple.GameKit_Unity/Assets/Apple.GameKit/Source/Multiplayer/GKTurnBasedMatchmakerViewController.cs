using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// Allows a player to invite other players to a turn-based match and fills any empty slots using auto-match.
    /// </summary>
    public class GKTurnBasedMatchmakerViewController : NSObject
    {
        public GKTurnBasedMatchmakerViewController(IntPtr pointer) : base(pointer)
        {
        }
        
        /// <summary>
        /// Creates a matchmaker view controller for the local player to start inviting other players to a turn-based game.
        /// </summary>
        /// <param name="matchRequest">The configuration for the match.</param>
        /// <returns>An initialized matchmaker view controller object or nil If an error occurs.</returns>
        public static GKTurnBasedMatchmakerViewController Init(GKMatchRequest matchRequest)
        {
            return PointerCast<GKTurnBasedMatchmakerViewController>(Interop.GKTurnBasedMatchmakerViewController_InitWithMatchRequest(matchRequest.Pointer));
        }

        /// <summary>
        /// A Boolean value that determines whether the view controller shows existing matches.
        /// </summary>
        public bool ShowExistingMatches
        {
            get => Interop.GKTurnBasedMatchmakerViewController_GetShowExistingMatches(Pointer);
            set => Interop.GKTurnBasedMatchmakerViewController_SetShowExistingMatches(Pointer, value);
        }

        /// <summary>
        /// The mode that a multiplayer game uses to find players.
        /// </summary>
        public GKMatchmakingMode MatchmakingMode
        {
            get => Interop.GKTurnBasedMatchmakerViewController_GetMatchmakingMode(Pointer);
            set => Interop.GKTurnBasedMatchmakerViewController_SetMatchmakingMode(Pointer, value);
        }

        private GKTurnBasedMatchmakerViewControllerDelegate _delegate;
        
        /// <summary>
        /// The object that handles matchmaker view controller changes.
        /// </summary>
        public GKTurnBasedMatchmakerViewControllerDelegate MatchmakerDelegate => _delegate ??= PointerCast<GKTurnBasedMatchmakerViewControllerDelegate>(Interop.GKTurnBasedMatchmakerViewController_GetMatchmakerDelegate(Pointer));

        /// <summary>
        /// Displays the view controller.
        /// </summary>
        public void Present()
        {
            Interop.GKTurnBasedMatchmakerViewController_Present(Pointer);
        }
        
        /// <summary>
        /// A utility request method to show the view controller and
        /// matchmake. Will throw a TaskCanceledException if the user canceled.
        /// </summary>
        /// <param name="matchRequest"></param>
        /// <returns></returns>
        public static Task<GKTurnBasedMatch> Request(GKMatchRequest matchRequest)
        {
            var tcs = new TaskCompletionSource<GKTurnBasedMatch>();
            var matchmaker = Init(matchRequest);
            matchmaker.MatchmakerDelegate.DidFindMatch += (controller, match) =>
            {
                tcs.TrySetResult(match);
            };
            matchmaker.MatchmakerDelegate.MatchmakingCanceled += controller =>
            {
                tcs.TrySetCanceled();
            };
            matchmaker.MatchmakerDelegate.DidFailWithError += (controller, exception) =>
            {
                tcs.TrySetException(exception);
            };
            
            matchmaker.Present();
            
            return tcs.Task;
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKTurnBasedMatchmakerViewController_InitWithMatchRequest(IntPtr matchRequest);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKTurnBasedMatchmakerViewController_GetShowExistingMatches(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatchmakerViewController_SetShowExistingMatches(IntPtr pointer, bool value);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKMatchmakingMode GKTurnBasedMatchmakerViewController_GetMatchmakingMode(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatchmakerViewController_SetMatchmakingMode(IntPtr pointer, GKMatchmakingMode value);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKTurnBasedMatchmakerViewController_GetMatchmakerDelegate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKTurnBasedMatchmakerViewController_Present(IntPtr pointer);
        }
    }
}
