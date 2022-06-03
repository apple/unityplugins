using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// Allows a player to invite other players to a multiplayer game and automatch to fill any empty slots.
    /// </summary>
    public class GKMatchmakerViewController : InteropReference
    {
        #region Init & Dispose
        public GKMatchmakerViewController(IntPtr pointer) : base(pointer)
        {
        }
        
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmakerViewController_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                GKMatchmakerViewController_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        #endregion
        
        #region Static Init
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKMatchmakerViewController_InitWithInvite(IntPtr invite);
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKMatchmakerViewController_InitWithMatchRequest(IntPtr matchRequest);
        
        /// <summary>
        /// Creates a matchmaker view controller for the local player to start inviting other players.
        /// </summary>
        /// <param name="matchRequest">The configuration for the match.</param>
        /// <returns>An initialized matchmaker view controller object or nil If an error occurs.</returns>
        public static GKMatchmakerViewController Init(GKMatchRequest matchRequest)
        {
            return PointerCast<GKMatchmakerViewController>(GKMatchmakerViewController_InitWithMatchRequest(matchRequest.Pointer));
        }

        /// <summary>
        /// Creates a matchmaker view controller to present to a player who accepts an invitation.
        /// </summary>
        /// <param name="invite">The invitation that the player accepts.</param>
        /// <returns>An initialized matchmaker view controller object. If an error occurs, returns nil.</returns>
        public static GKMatchmakerViewController Init(GKInvite invite)
        {
            return PointerCast<GKMatchmakerViewController>(GKMatchmakerViewController_InitWithInvite(invite.Pointer));
        }
        #endregion
        
        #region MatchRequest
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKMatchmakerViewController_GetMatchRequest(IntPtr pointer);

        /// <summary>
        /// The configuration for the desired match.
        /// </summary>
        public GKMatchRequest MatchRequest
        {
            get => PointerCast<GKMatchRequest>(GKMatchmakerViewController_GetMatchRequest(Pointer));
        }
        #endregion
        
        #region CanStartWithMinimumPlayers
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKMatchmakerViewController_GetCanStartWithMinimumPlayers(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmakerViewController_SetCanStartWithMinimumPlayers(IntPtr pointer, bool value);

        /// <summary>
        /// A Boolean value that indicates whether your game can start after a minimum number of players join a match.
        /// </summary>
        public bool CanStartWithMinimumPlayers
        {
            get => GKMatchmakerViewController_GetCanStartWithMinimumPlayers(Pointer);
            set => GKMatchmakerViewController_SetCanStartWithMinimumPlayers(Pointer, value);
        }
        #endregion
        
        #region Matchmaking Mode

        [DllImport(InteropUtility.DLLName)]
        private static extern GKMatchmakingMode GKMatchmakerViewController_GetMatchmakingMode(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmakerViewController_SetMatchmakingMode(IntPtr pointer, GKMatchmakingMode value);

        /// <summary>
        /// The mode that a multiplayer game uses to find players.
        /// </summary>
        public GKMatchmakingMode MatchmakingMode
        {
            get => GKMatchmakerViewController_GetMatchmakingMode(Pointer);
            set => GKMatchmakerViewController_SetMatchmakingMode(Pointer, value);
        }
        #endregion
        
        #region IsHosted
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKMatchmakerViewController_GetIsHosted(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmakerViewController_SetIsHosted(IntPtr pointer, bool isHosted);

        /// <summary>
        /// A Boolean value that indicates whether the match is hosted or peer-to-pee
        /// </summary>
        public bool IsHosted
        {
            get => GKMatchmakerViewController_GetIsHosted(Pointer);
            set => GKMatchmakerViewController_SetIsHosted(Pointer, value);
        }
        #endregion
        
        #region Present
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmakerViewController_Present(IntPtr pointer);

        /// <summary>
        /// Displays the view controller.
        /// </summary>
        public void Present()
        {
            GKMatchmakerViewController_Present(Pointer);
        }
        #endregion
        
        #region MatchmakerDelegate
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKMatchmakerViewController_GetMatchmakerDelegate(IntPtr pointer);

        private GKMatchmakerViewControllerDelegate _delegate;
        
        /// <summary>
        /// The object that handles matchmaker view controller changes.
        /// </summary>
        public GKMatchmakerViewControllerDelegate MatchmakerDelegate
        {
            get
            {
                if(_delegate == null)
                    _delegate = PointerCast<GKMatchmakerViewControllerDelegate>(GKMatchmakerViewController_GetMatchmakerDelegate(Pointer));

                return _delegate;
            }
        }

        #endregion
        
        #region Request Utility
        /// <summary>
        /// Utility method to request a match from the view controller. Will throw
        /// a TaskCanceledException if the user canceled the operation.
        /// </summary>
        /// <param name="matchRequest"></param>
        /// <returns></returns>
        public static Task<GKMatch> Request(GKMatchRequest matchRequest)
        {
            var tcs = new TaskCompletionSource<GKMatch>();
            
            var matchmaker = Init(matchRequest);
            matchmaker.MatchmakerDelegate.DidFindMatch += (controller, match) =>
            {
                tcs.TrySetResult(match);
            };
            matchmaker.MatchmakerDelegate.DidFailWithError += (controller, exception) =>
            {
                tcs.TrySetException(exception);
            };
            matchmaker.MatchmakerDelegate.MatchmakingCanceled += controller =>
            {
                tcs.TrySetCanceled();
            };
            matchmaker.Present();
            
            return tcs.Task;
        }
        #endregion
    }
}