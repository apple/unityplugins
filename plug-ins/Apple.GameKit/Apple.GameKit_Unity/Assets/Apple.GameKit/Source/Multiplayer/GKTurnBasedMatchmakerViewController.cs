using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// Allows a player to invite other players to a turn-based match and fills any empty slots using auto-match.
    /// </summary>
    public class GKTurnBasedMatchmakerViewController : InteropReference
    {
        #region Init & Dispose
        public GKTurnBasedMatchmakerViewController(IntPtr pointer) : base(pointer)
        {
        }

        [DllImport(InteropUtility.DLLName)]
        private static extern void GKTurnBasedMatchmakerViewController_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                GKTurnBasedMatchmakerViewController_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }
        #endregion
        
        #region Static Init
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKTurnBasedMatchmakerViewController_InitWithMatchRequest(IntPtr matchRequest);
        
        /// <summary>
        /// Creates a matchmaker view controller for the local player to start inviting other players to a turn-based game.
        /// </summary>
        /// <param name="matchRequest">The configuration for the match.</param>
        /// <returns>An initialized matchmaker view controller object or nil If an error occurs.</returns>
        public static GKTurnBasedMatchmakerViewController Init(GKMatchRequest matchRequest)
        {
            return PointerCast<GKTurnBasedMatchmakerViewController>(GKTurnBasedMatchmakerViewController_InitWithMatchRequest(matchRequest.Pointer));
        }
        #endregion
        
        #region ShowExistingMatches
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKTurnBasedMatchmakerViewController_GetShowExistingMatches(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKTurnBasedMatchmakerViewController_SetShowExistingMatches(IntPtr pointer, bool value);

        /// <summary>
        /// A Boolean value that determines whether the view controller shows existing matches.
        /// </summary>
        public bool ShowExistingMatches
        {
            get => GKTurnBasedMatchmakerViewController_GetShowExistingMatches(Pointer);
            set => GKTurnBasedMatchmakerViewController_SetShowExistingMatches(Pointer, value);
        }
        #endregion
        
        #region Matchmaking Mode
        [DllImport(InteropUtility.DLLName)]
        private static extern GKMatchmakingMode GKTurnBasedMatchmakerViewController_GetMatchmakingMode(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKTurnBasedMatchmakerViewController_SetMatchmakingMode(IntPtr pointer, GKMatchmakingMode value);

        /// <summary>
        /// The mode that a multiplayer game uses to find players.
        /// </summary>
        public GKMatchmakingMode MatchmakingMode
        {
            get => GKTurnBasedMatchmakerViewController_GetMatchmakingMode(Pointer);
            set => GKTurnBasedMatchmakerViewController_SetMatchmakingMode(Pointer, value);
        }
        #endregion
        
        #region MatchmakerDelegate
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKTurnBasedMatchmakerViewController_GetMatchmakerDelegate(IntPtr pointer);

        private GKTurnBasedMatchmakerViewControllerDelegate _delegate;
        
        /// <summary>
        /// The object that handles matchmaker view controller changes.
        /// </summary>
        public GKTurnBasedMatchmakerViewControllerDelegate MatchmakerDelegate
        {
            get
            {
                if(_delegate == null)
                    _delegate = PointerCast<GKTurnBasedMatchmakerViewControllerDelegate>(GKTurnBasedMatchmakerViewController_GetMatchmakerDelegate(Pointer));

                return _delegate;
            }
        }

        #endregion
        
        #region Present
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKTurnBasedMatchmakerViewController_Present(IntPtr pointer);

        /// <summary>
        /// Displays the view controller.
        /// </summary>
        public void Present()
        {
            GKTurnBasedMatchmakerViewController_Present(Pointer);
        }
        #endregion
        
        #region Utility Request
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
        #endregion
    }
}