using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// Allows a player to invite other players to a multiplayer game and automatch to fill any empty slots.
    /// </summary>
    public class GKMatchmakerViewController : NSObject
    {
        public GKMatchmakerViewController(IntPtr pointer) : base(pointer)
        {
        }
        
        /// <summary>
        /// Creates a matchmaker view controller for the local player to start inviting other players.
        /// </summary>
        /// <param name="matchRequest">The configuration for the match.</param>
        /// <returns>An initialized matchmaker view controller object or nil If an error occurs.</returns>
        public static GKMatchmakerViewController Init(GKMatchRequest matchRequest)
        {
            return PointerCast<GKMatchmakerViewController>(Interop.GKMatchmakerViewController_InitWithMatchRequest(matchRequest.Pointer));
        }

        /// <summary>
        /// Creates a matchmaker view controller to present to a player who accepts an invitation.
        /// </summary>
        /// <param name="invite">The invitation that the player accepts.</param>
        /// <returns>An initialized matchmaker view controller object. If an error occurs, returns nil.</returns>
        public static GKMatchmakerViewController Init(GKInvite invite)
        {
            return PointerCast<GKMatchmakerViewController>(Interop.GKMatchmakerViewController_InitWithInvite(invite.Pointer));
        }
        
        /// <summary>
        /// The configuration for the desired match.
        /// </summary>
        public GKMatchRequest MatchRequest
        {
            get => PointerCast<GKMatchRequest>(Interop.GKMatchmakerViewController_GetMatchRequest(Pointer));
        }
        
        /// <summary>
        /// A Boolean value that indicates whether your game can start after a minimum number of players join a match.
        /// </summary>
        [Introduced(iOS: "15.0", macOS: "12.0", tvOS: "15.0", visionOS: "1.0")]
        public bool CanStartWithMinimumPlayers
        {
            get => Interop.GKMatchmakerViewController_GetCanStartWithMinimumPlayers(Pointer);
            set => Interop.GKMatchmakerViewController_SetCanStartWithMinimumPlayers(Pointer, value);
        }

        /// <summary>
        /// The mode that a multiplayer game uses to find players.
        /// </summary>
        [Introduced(iOS: "14.0", macOS: "11.0", tvOS: "14.0", visionOS: "1.0")]
        public GKMatchmakingMode MatchmakingMode
        {
            get => Interop.GKMatchmakerViewController_GetMatchmakingMode(Pointer);
            set => Interop.GKMatchmakerViewController_SetMatchmakingMode(Pointer, value);
        }

        /// <summary>
        /// A Boolean value that indicates whether the match is hosted or peer-to-peer.
        /// </summary>
        public bool IsHosted
        {
            get => Interop.GKMatchmakerViewController_GetIsHosted(Pointer);
            set => Interop.GKMatchmakerViewController_SetIsHosted(Pointer, value);
        }

        /// <summary>
        /// Updates the connection status of a player in a hosted game.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="didConnect"></param>
        public void SetHostedPlayerDidConnect(GKPlayer player, bool didConnect)
        {
            Interop.GKMatchmakerViewController_SetHostedPlayerDidConnect(Pointer, player.Pointer, didConnect);
        }

        /// <summary>
        /// Displays the view controller.
        /// </summary>
        public void Present()
        {
            Interop.GKMatchmakerViewController_Present(Pointer);
        }

        private GKMatchmakerViewControllerDelegate _delegate;
        
        /// <summary>
        /// The object that handles matchmaker view controller changes.
        /// </summary>
        public GKMatchmakerViewControllerDelegate MatchmakerDelegate => _delegate ??= PointerCast<GKMatchmakerViewControllerDelegate>(Interop.GKMatchmakerViewController_GetMatchmakerDelegate(Pointer)); 

        /// <summary>
        /// Utility method to request a match from the view controller. Will throw
        /// a TaskCanceledException if the user canceled the operation.
        /// </summary>
        /// <param name="matchRequest"></param>
        /// <param name="mode">The matchmaking to use when making the request.</param>
        /// <param name="canStartWithMinimumPlayers">Whether your game can start after a minimum number of players join a match.</param>
        /// <param name="getMatchPropertiesForRecipientHandler">Optional handler to provide properties for each player as they join the match.</param>
        /// <returns></returns>
        public static async Task<GKMatch> Request(
            GKMatchRequest matchRequest, 
            GKMatchmakingMode mode = GKMatchmakingMode.Default,
            bool canStartWithMinimumPlayers = false,
            GKMatchmakerViewControllerDelegate.GetMatchPropertiesForRecipientHandler getMatchPropertiesForRecipientHandler = default)
        {
            var matchmaker = Init(matchRequest);

            if (Availability.IsPropertyAvailable<GKMatchmakerViewController>(nameof(MatchmakingMode)))
            {
                matchmaker.MatchmakingMode = mode;
            }

            if (Availability.IsPropertyAvailable<GKMatchmakerViewController>(nameof(CanStartWithMinimumPlayers)))
            {
                matchmaker.CanStartWithMinimumPlayers = canStartWithMinimumPlayers;
            }

            matchmaker.IsHosted = false;

            return await Request(matchmaker, getMatchPropertiesForRecipientHandler);
        }

        /// <summary>
        /// Utility method to handle a match invitation with the view controller. Will throw
        /// a TaskCanceledException if the user canceled the operation.
        /// </summary>
        /// <param name="invite"></param>
        /// <param name="mode">The matchmaking to use when making the request.</param>
        /// <param name="getMatchPropertiesForRecipientHandler">Optional handler to provide properties for each player as they join the match.</param>
        /// <returns></returns>
        public static async Task<GKMatch> Request(
            GKInvite invite, 
            GKMatchmakerViewControllerDelegate.GetMatchPropertiesForRecipientHandler getMatchPropertiesForRecipientHandler = default)
        {
            var matchmaker = Init(invite);

            return await Request(matchmaker, getMatchPropertiesForRecipientHandler);
        }

        /// <summary>
        /// Utility method to add players to an existing match via the view controller.
        /// Throws a TaskCanceledException if the user canceled the operation.
        /// </summary>
        /// <param name="matchRequest"></param>
        /// <param name="match">The existing match to which to add the new players.</param>
        /// <param name="mode">The matchmaking to use when making the request.</param>
        /// <param name="canStartWithMinimumPlayers">Whether your game can start after a minimum number of players join a match.</param>
        /// <param name="getMatchPropertiesForRecipientHandler">Optional handler to provide properties for each player as they join the match.</param>
        /// <returns></returns>
        public static async Task AddPlayersToMatch(
            GKMatchRequest matchRequest,
            GKMatch match,
            GKMatchmakingMode mode = GKMatchmakingMode.Default,
            bool canStartWithMinimumPlayers = false,
            GKMatchmakerViewControllerDelegate.GetMatchPropertiesForRecipientHandler getMatchPropertiesForRecipientHandler = default)
        {
            var matchmaker = Init(matchRequest);

            if (Availability.IsPropertyAvailable<GKMatchmakerViewController>(nameof(MatchmakingMode)))
            {
                matchmaker.MatchmakingMode = mode;
            }

            if (Availability.IsPropertyAvailable<GKMatchmakerViewController>(nameof(CanStartWithMinimumPlayers)))
            {
                matchmaker.CanStartWithMinimumPlayers = canStartWithMinimumPlayers;
            }

            matchmaker.IsHosted = false;

            Interop.GKMatchmakerViewController_AddPlayersToMatch(matchmaker.Pointer, match.Pointer);

            await Request(matchmaker, getMatchPropertiesForRecipientHandler);
        }

        private static Task<GKMatch> Request(
            GKMatchmakerViewController matchmaker, 
            GKMatchmakerViewControllerDelegate.GetMatchPropertiesForRecipientHandler getMatchPropertiesForRecipientHandler)
        {
            var tcs = new TaskCompletionSource<GKMatch>();
            
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

            if (getMatchPropertiesForRecipientHandler != default &&
                Availability.IsEventAvailable<GKMatchmakerViewControllerDelegate>(nameof(GKMatchmakerViewControllerDelegate.GetMatchPropertiesForRecipient)))
            {
                matchmaker.MatchmakerDelegate.GetMatchPropertiesForRecipient += getMatchPropertiesForRecipientHandler;
            }

            matchmaker.Present();
            
            return tcs.Task;
        }

        /// <summary>
        /// Utility method to request a hosted match from the view controller. Will throw
        /// a TaskCanceledException if the user canceled the operation.
        /// </summary>
        /// <param name="matchRequest"></param>
        /// <returns></returns>
        public static Task<NSArray<GKPlayer>> RequestHosted(
            GKMatchRequest matchRequest, 
            GKMatchmakingMode mode = GKMatchmakingMode.Default,
            bool canStartWithMinimumPlayers = false,
            GKMatchmakerViewControllerDelegate.GetMatchPropertiesForRecipientHandler getMatchPropertiesForRecipientHandler = default,
            GKMatchmakerViewControllerDelegate.HostedPlayerDidAcceptHandler hostedPlayerDidAcceptHandler = default)
        {
            var tcs = new TaskCompletionSource<NSArray<GKPlayer>>();
            
            var matchmaker = Init(matchRequest);

            if (Availability.IsPropertyAvailable<GKMatchmakerViewController>(nameof(MatchmakingMode)))
            {
                matchmaker.MatchmakingMode = mode;
            }

            if (Availability.IsPropertyAvailable<GKMatchmakerViewController>(nameof(CanStartWithMinimumPlayers)))
            {
                matchmaker.CanStartWithMinimumPlayers = canStartWithMinimumPlayers;
            }

            matchmaker.IsHosted = true;

            matchmaker.MatchmakerDelegate.DidFindHostedPlayers += (controller, players) =>
            {
                tcs.TrySetResult(players);
            };
            matchmaker.MatchmakerDelegate.DidFailWithError += (controller, exception) =>
            {
                tcs.TrySetException(exception);
            };
            matchmaker.MatchmakerDelegate.MatchmakingCanceled += controller =>
            {
                tcs.TrySetCanceled();
            };

            if (getMatchPropertiesForRecipientHandler != default &&
                Availability.IsEventAvailable<GKMatchmakerViewControllerDelegate>(nameof(GKMatchmakerViewControllerDelegate.GetMatchPropertiesForRecipient)))
            {
                matchmaker.MatchmakerDelegate.GetMatchPropertiesForRecipient += getMatchPropertiesForRecipientHandler;
            }
            if (hostedPlayerDidAcceptHandler != default)
            {
                matchmaker.MatchmakerDelegate.HostedPlayerDidAccept += hostedPlayerDidAcceptHandler;
            }

            matchmaker.Present();
            
            return tcs.Task;
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKMatchmakerViewController_InitWithInvite(IntPtr invite);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKMatchmakerViewController_InitWithMatchRequest(IntPtr matchRequest);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKMatchmakerViewController_GetMatchRequest(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKMatchmakerViewController_GetCanStartWithMinimumPlayers(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmakerViewController_SetCanStartWithMinimumPlayers(IntPtr pointer, bool value);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKMatchmakingMode GKMatchmakerViewController_GetMatchmakingMode(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmakerViewController_SetMatchmakingMode(IntPtr pointer, GKMatchmakingMode value);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKMatchmakerViewController_GetIsHosted(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmakerViewController_SetIsHosted(IntPtr pointer, bool isHosted);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmakerViewController_SetHostedPlayerDidConnect(IntPtr gkMatchmakerViewControllerPtr, IntPtr gkPlayerPtr, bool didConnect);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmakerViewController_Present(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKMatchmakerViewController_GetMatchmakerDelegate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmakerViewController_AddPlayersToMatch(IntPtr gkMatchmakerViewControllerPtr, IntPtr gkMatchPtr);
        }
    }
}
