using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    using GKMatchProperties = NSDictionary<NSString, NSObject>;

    /// <summary>
    /// An object that handles when the status of matchmaking changes.
    /// </summary>
    public class GKMatchmakerViewControllerDelegate : NSObject
    {
        private static readonly Dictionary<IntPtr, GKMatchmakerViewControllerDelegate> _delegates = new Dictionary<IntPtr, GKMatchmakerViewControllerDelegate>();
        
        #region Delegates
        public delegate void DidFindMatchHandler(GKMatchmakerViewController matchmakerViewController, GKMatch match);
        private delegate void InteropDidFindMatchHandler(IntPtr pointer, IntPtr matchmakerViewController, IntPtr match);
        public delegate void DidFindHostedPlayersHandler(GKMatchmakerViewController matchmakerViewController, NSArray<GKPlayer> hostedPlayers);
        private delegate void InteropDidFindHostedPlayersHandler(IntPtr pointer,IntPtr matchmakerViewController, IntPtr hostedPlayers);
        public delegate void MatchmakingCanceledHandler(GKMatchmakerViewController matchmakerViewController);
        private delegate void InteropMatchmakingCanceledHandler(IntPtr pointer,IntPtr matchmakerViewController);
        public delegate void DidFailWithErrorHandler(GKMatchmakerViewController matchmakerViewController, GameKitException exception);
        private delegate void InteropDidFailWithErrorHandler(IntPtr pointer,IntPtr matchmakerViewController, IntPtr errorPointer);
        public delegate void HostedPlayerDidAcceptHandler(GKMatchmakerViewController matchmakerViewController, GKPlayer acceptingPlayer);
        private delegate void InteropHostedPlayerDidAcceptHandler(IntPtr pointer, IntPtr matchmakerViewController, IntPtr player);
        #endregion

        #region Events
        public event DidFindMatchHandler DidFindMatch;
        public event DidFindHostedPlayersHandler DidFindHostedPlayers;
        public event MatchmakingCanceledHandler MatchmakingCanceled;
        public event DidFailWithErrorHandler DidFailWithError;
        public event HostedPlayerDidAcceptHandler HostedPlayerDidAccept;
        #endregion
        
        #region Init & Dispose
        public GKMatchmakerViewControllerDelegate(IntPtr pointer) : base(pointer)
        {
            _delegates.Add(pointer, this);
            
            // Delegate callbacks...
            Interop.GKMatchmakerViewControllerDelegate_SetDidFindMatchCallback(Pointer, OnDidFindMatch);
            Interop.GKMatchmakerViewControllerDelegate_SetDidFindHostedPlayersCallback(Pointer, OnDidFindHostedPlayers);
            Interop.GKMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback(Pointer, OnMatchmakingCanceled);
            Interop.GKMatchmakerViewControllerDelegate_SetHostedPlayerDidAccept(Pointer, OnHostedPlayerDidAccept);
            Interop.GKMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback(Pointer, OnDidFailWithError);

            if (Availability.IsTypeAvailable<GetMatchPropertiesForRecipientHandler>())
            {
                Interop.GKMatchmakerViewControllerDelegate_SetGetMatchPropertiesForRecipientCallback(Pointer, OnGetMatchPropertiesForRecipient);
            }
        }

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                _delegates.Remove(Pointer);
            }
            base.OnDispose(isDisposing);
        }
        #endregion
        
        #region Event Registration
        [MonoPInvokeCallback(typeof(InteropDidFindMatchHandler))]
        private static void OnDidFindMatch(IntPtr pointer, IntPtr matchmakerViewController, IntPtr match)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                if (!_delegates.TryGetValue(pointer, out var matchmakerViewControllerDelegate))
                {
                    return;
                }
                
                matchmakerViewControllerDelegate?.DidFindMatch?.Invoke(
                    PointerCast<GKMatchmakerViewController>(matchmakerViewController), 
                    PointerCast<GKMatch>(match));
            });
        }
        
        [MonoPInvokeCallback(typeof(InteropDidFindHostedPlayersHandler))]
        private static void OnDidFindHostedPlayers(IntPtr pointer, IntPtr matchmakerViewController, IntPtr hostedPlayers)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                if (!_delegates.TryGetValue(pointer, out var matchmakerViewControllerDelegate))
                {
                    return;
                }
                
                matchmakerViewControllerDelegate?.DidFindHostedPlayers?.Invoke(
                    PointerCast<GKMatchmakerViewController>(matchmakerViewController), 
                    PointerCast<NSArray<GKPlayer>>(hostedPlayers));
            });
        }
        
        [MonoPInvokeCallback(typeof(InteropMatchmakingCanceledHandler))]
        private static void OnMatchmakingCanceled(IntPtr pointer, IntPtr matchmakerViewController)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() =>
            {
                if (!_delegates.TryGetValue(pointer, out var matchmakerViewControllerDelegate))
                {
                    return;
                }
                
                matchmakerViewControllerDelegate?.MatchmakingCanceled?.Invoke(
                    PointerCast<GKMatchmakerViewController>(matchmakerViewController));
            });
        }
        
        [MonoPInvokeCallback(typeof(InteropDidFailWithErrorHandler))]
        private static void OnDidFailWithError(IntPtr pointer, IntPtr matchmakerViewController, IntPtr errorPointer)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => 
            {
                if (!_delegates.TryGetValue(pointer, out var matchmakerViewControllerDelegate))
                {
                    return;
                }
                
                matchmakerViewControllerDelegate?.DidFailWithError?.Invoke(
                    PointerCast<GKMatchmakerViewController>(matchmakerViewController), 
                    new GameKitException(errorPointer));
            });
        }
        
        [MonoPInvokeCallback(typeof(InteropHostedPlayerDidAcceptHandler))]
        private static void OnHostedPlayerDidAccept(IntPtr pointer, IntPtr matchmakerViewController, IntPtr hostedPlayer)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => 
            {
                if (!_delegates.TryGetValue(pointer, out var matchmakerViewControllerDelegate))
                {
                    return;
                }
                
                matchmakerViewControllerDelegate?.HostedPlayerDidAccept?.Invoke(
                    PointerCast<GKMatchmakerViewController>(matchmakerViewController), 
                    PointerCast<GKPlayer>(hostedPlayer));
            });
        }
        #endregion

        #region GetMatchPropertiesForRecipient
        /// <summary>
        /// Returns the properties for another player that the local player invites using the view controller interface.
        /// </summary>
        /// <remarks>Implement this method if you can provide properties for the recipients of this match request to better fine tune the Game Center matchmaking using rules.</remarks>
        /// <param name="matchmakerViewController">The view controller that finds players for the match.</param>
        /// <param name="invitedPlayer">A player to invite to the match.</param>
        /// <returns>The properties for recipient that the local player invites to the match.</returns>
        [Introduced(iOS: "17.2", macOS: "14.2", tvOS: "17.2", visionOS: "1.1")]
        public delegate Task<GKMatchProperties> GetMatchPropertiesForRecipientHandler(GKMatchmakerViewController matchmakerViewController, GKPlayer invitedPlayer);
        private delegate void InteropGetMatchPropertiesForRecipientHandler(IntPtr gkMatchmakerViewControllerDelegatePtr, IntPtr gkMatchmakerViewControllerPtr, IntPtr gkPlayerPtr, IntPtr completionHandlerPtr);

        /// <summary>
        /// Dispatches GetMatchPropertiesForRecipient events.
        /// </summary>
        [Introduced(iOS: "17.2", macOS: "14.2", tvOS: "17.2", visionOS: "1.1")]
        public event GetMatchPropertiesForRecipientHandler GetMatchPropertiesForRecipient;

        [MonoPInvokeCallback(typeof(InteropGetMatchPropertiesForRecipientHandler))]
        private static async void OnGetMatchPropertiesForRecipient(IntPtr gkMatchmakerViewControllerDelegatePtr, IntPtr gkMatchmakerViewControllerPtr, IntPtr gkPlayerPtr, IntPtr completionHandlerPtr)
        {
            await InteropPInvokeExceptionHandler.CatchAndLog(async () =>
            {
                if (!_delegates.TryGetValue(gkMatchmakerViewControllerDelegatePtr, out var gkMatchmakerViewControllerDelegate))
                {
                    return;
                }

                var gkMatchProperties = await gkMatchmakerViewControllerDelegate?.GetMatchPropertiesForRecipient?.Invoke(
                    PointerCast<GKMatchmakerViewController>(gkMatchmakerViewControllerPtr),
                    PointerCast<GKPlayer>(gkPlayerPtr));

                // call completion handler
                Interop.GKMatchmakerViewControllerDelegate_CallGetMatchPropertiesForRecipientCompletionHandler(completionHandlerPtr, gkMatchProperties.Pointer);
            });
        }
        #endregion

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmakerViewControllerDelegate_SetDidFindMatchCallback(IntPtr pointer, InteropDidFindMatchHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmakerViewControllerDelegate_SetDidFindHostedPlayersCallback(IntPtr pointer, InteropDidFindHostedPlayersHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback(IntPtr pointer, InteropMatchmakingCanceledHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback(IntPtr pointer, InteropDidFailWithErrorHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmakerViewControllerDelegate_SetHostedPlayerDidAccept(IntPtr pointer, InteropHostedPlayerDidAcceptHandler callback);

            // event registration
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmakerViewControllerDelegate_SetGetMatchPropertiesForRecipientCallback(IntPtr pointer, InteropGetMatchPropertiesForRecipientHandler callback);

            // completion handler
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKMatchmakerViewControllerDelegate_CallGetMatchPropertiesForRecipientCompletionHandler(IntPtr completionHandlerPtr, IntPtr gkMatchPropertiesPtr);
        }
    }
}
