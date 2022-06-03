using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// An object that handles when the status of matchmaking changes.
    /// </summary>
    public class GKMatchmakerViewControllerDelegate : InteropReference
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
        private delegate void InteropHostedPlayerDidAcceptHandler(IntPtr pointer,IntPtr matchmakerViewController, IntPtr player);
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
            GKMatchmakerViewControllerDelegate_SetDidFindMatchCallback(Pointer, OnDidFindMatch);
            GKMatchmakerViewControllerDelegate_SetDidFindHostedPlayersCallback(Pointer, OnDidFindHostedPlayers);
            GKMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback(Pointer, OnMatchmakingCanceled);
            GKMatchmakerViewControllerDelegate_SetHostedPlayerDidAccept(Pointer, OnHostedPlayerDidAccept);
            GKMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback(Pointer, OnDidFailWithError);
        }

        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmakerViewControllerDelegate_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                GKMatchmakerViewControllerDelegate_Free(Pointer);
                _delegates.Remove(Pointer);
                Pointer = IntPtr.Zero;
            }
        }
        #endregion
        
        #region Event Registration
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmakerViewControllerDelegate_SetDidFindMatchCallback(IntPtr pointer, InteropDidFindMatchHandler callback);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmakerViewControllerDelegate_SetDidFindHostedPlayersCallback(IntPtr pointer, InteropDidFindHostedPlayersHandler callback);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmakerViewControllerDelegate_SetMatchmakingCanceledCallback(IntPtr pointer, InteropMatchmakingCanceledHandler callback);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmakerViewControllerDelegate_SetDidFailWithErrorCallback(IntPtr pointer, InteropDidFailWithErrorHandler callback);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKMatchmakerViewControllerDelegate_SetHostedPlayerDidAccept(IntPtr pointer, InteropHostedPlayerDidAcceptHandler callback);

        [MonoPInvokeCallback(typeof(InteropDidFindMatchHandler))]
        private static void OnDidFindMatch(IntPtr pointer, IntPtr matchmakerViewController, IntPtr match)
        {
            if (!_delegates.TryGetValue(pointer, out var matchmakerViewControllerDelegate))
                return;
            
            matchmakerViewControllerDelegate?.DidFindMatch?.Invoke(
                PointerCast<GKMatchmakerViewController>(matchmakerViewController), 
                PointerCast<GKMatch>(match));
        }
        
        [MonoPInvokeCallback(typeof(InteropDidFindHostedPlayersHandler))]
        private static void OnDidFindHostedPlayers(IntPtr pointer, IntPtr matchmakerViewController, IntPtr hostedPlayers)
        {
            if (!_delegates.TryGetValue(pointer, out var matchmakerViewControllerDelegate))
                return;
            
            matchmakerViewControllerDelegate?.DidFindHostedPlayers?.Invoke(
                PointerCast<GKMatchmakerViewController>(matchmakerViewController), 
                PointerCast<NSArrayGKPlayer>(hostedPlayers));
        }
        
        [MonoPInvokeCallback(typeof(InteropMatchmakingCanceledHandler))]
        private static void OnMatchmakingCanceled(IntPtr pointer, IntPtr matchmakerViewController)
        {
            if (!_delegates.TryGetValue(pointer, out var matchmakerViewControllerDelegate))
                return;
            
            matchmakerViewControllerDelegate?.MatchmakingCanceled?.Invoke(
                PointerCast<GKMatchmakerViewController>(matchmakerViewController));
        }
        
        [MonoPInvokeCallback(typeof(InteropDidFailWithErrorHandler))]
        private static void OnDidFailWithError(IntPtr pointer, IntPtr matchmakerViewController, IntPtr errorPointer)
        {
            if (!_delegates.TryGetValue(pointer, out var matchmakerViewControllerDelegate))
                return;
            
            matchmakerViewControllerDelegate?.DidFailWithError?.Invoke(
                PointerCast<GKMatchmakerViewController>(matchmakerViewController), 
                new GameKitException(errorPointer));
        }
        
        [MonoPInvokeCallback(typeof(InteropHostedPlayerDidAcceptHandler))]
        private static void OnHostedPlayerDidAccept(IntPtr pointer, IntPtr matchmakerViewController, IntPtr hostedPlayer)
        {
            if (!_delegates.TryGetValue(pointer, out var matchmakerViewControllerDelegate))
                return;
            
            matchmakerViewControllerDelegate?.HostedPlayerDidAccept?.Invoke(
                PointerCast<GKMatchmakerViewController>(matchmakerViewController), 
                PointerCast<GKPlayer>(hostedPlayer));
        }
        #endregion
    }
}