using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// The object that handles turn-based matchmaker view controller changes.
    /// </summary>
    public class GKTurnBasedMatchmakerViewControllerDelegate : InteropReference
    {
        private static readonly Dictionary<IntPtr, GKTurnBasedMatchmakerViewControllerDelegate> _delegates = new Dictionary<IntPtr, GKTurnBasedMatchmakerViewControllerDelegate>();

        #region Init & Dispose
        public GKTurnBasedMatchmakerViewControllerDelegate(IntPtr pointer) : base(pointer)
        {
            _delegates.Add(pointer, this);
            
            // Handles events from matchmaker...
            GKTurnBasedMatchmakerViewControllerDelegate_SetDidFindMatchCallback(Pointer, OnDidFindMatch);
            GKTurnBasedMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback(Pointer, OnMatchmakingCanceled);
            GKTurnBasedMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback(Pointer, OnDidFailWithError);
        }
        #endregion
        
        #region Delegates
        public delegate void DidFindMatchHandler(GKTurnBasedMatchmakerViewController matchmakerViewController, GKTurnBasedMatch match);
        private delegate void _DidFindMatchHandler(IntPtr pointer, IntPtr matchmakerViewController, IntPtr match);
        public delegate void MatchmakingCanceledHandler(GKTurnBasedMatchmakerViewController matchmakerViewController);
        private delegate void _MatchmakingCanceledHandler(IntPtr pointer, IntPtr matchmakerViewController);
        public delegate void DidFailWithErrorHandler(GKTurnBasedMatchmakerViewController matchmakerViewController, GameKitException exception);
        private delegate void _DidFailWithErrorHandler(IntPtr pointer, IntPtr matchmakerViewController, IntPtr errorPointer);
        #endregion
        
        #region Events
        /// <summary>
        /// Invoked via the GKTurnBasedMatch.TurnEventReceived callback. Provided here only as convenience. 
        /// </summary>
        public event DidFindMatchHandler DidFindMatch;
        /// <summary>
        /// Handles when a player cancels matchmaking.
        /// </summary>
        public event MatchmakingCanceledHandler MatchmakingCanceled;
        /// <summary>
        /// Handles when the view controller encounters an error while finding players for a match.
        /// </summary>
        public event DidFailWithErrorHandler DidFailWithError;
        #endregion
        
        #region Event Registration
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKTurnBasedMatchmakerViewControllerDelegate_SetDidFindMatchCallback(IntPtr pointer, _DidFindMatchHandler callback);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKTurnBasedMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback(IntPtr pointer, _MatchmakingCanceledHandler callback);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKTurnBasedMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback(IntPtr pointer, _DidFailWithErrorHandler callback);

        [MonoPInvokeCallback(typeof(_DidFindMatchHandler))]
        private static void OnDidFindMatch(IntPtr pointer, IntPtr matchmakerViewController, IntPtr match)
        {
            if (!_delegates.TryGetValue(pointer, out var matchmakerViewControllerDelegate))
                return;
            
            matchmakerViewControllerDelegate?.DidFindMatch?.Invoke(
                PointerCast<GKTurnBasedMatchmakerViewController>(matchmakerViewController),
                PointerCast<GKTurnBasedMatch>(match));
        }
        
        [MonoPInvokeCallback(typeof(_MatchmakingCanceledHandler))]
        private static void OnMatchmakingCanceled(IntPtr pointer, IntPtr matchmakerViewController)
        {
            if (!_delegates.TryGetValue(pointer, out var matchmakerViewControllerDelegate))
                return;
            
            matchmakerViewControllerDelegate?.MatchmakingCanceled?.Invoke(PointerCast<GKTurnBasedMatchmakerViewController>(matchmakerViewController));
        }

        [MonoPInvokeCallback(typeof(_DidFailWithErrorHandler))]
        private static void OnDidFailWithError(IntPtr pointer, IntPtr matchmakerViewController, IntPtr errorPointer)
        {
            if (!_delegates.TryGetValue(pointer, out var matchmakerViewControllerDelegate))
                return;
            
            matchmakerViewControllerDelegate?.DidFailWithError?.Invoke(
                PointerCast<GKTurnBasedMatchmakerViewController>(matchmakerViewController),
                new GameKitException(errorPointer));
        }
        #endregion
    }
}