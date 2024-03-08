using System;
using System.Runtime.InteropServices;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// An invitation to join a match sent to the local player from another player.
    /// </summary>
    public class GKInvite : NSObject
    {
        public delegate void InviteAcceptedHandler(GKPlayer invitedPlayer, GKInvite invite);
        private delegate void InteropInviteAcceptedHandler(IntPtr player, IntPtr invite);
        
        /// <summary>
        /// Handles the event when the local player accepts an invitation from another player.
        /// </summary>
        public static event InviteAcceptedHandler InviteAccepted;

        static GKInvite()
        {
            Interop.GKInvite_SetInviteAcceptedCallback(OnInviteAccepted);
        }

        [MonoPInvokeCallback(typeof(InteropInviteAcceptedHandler))]
        private static void OnInviteAccepted(IntPtr player, IntPtr invite)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => InviteAccepted?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKInvite>(invite)));
        }
        
        internal GKInvite(IntPtr pointer) : base(pointer){}
        
        /// <summary>
        /// The player who sends the invitation.
        /// </summary>
        public GKPlayer Sender
        {
            get
            {
                var pointer = Interop.GKInvite_GetSender(Pointer);
                
                if(pointer != IntPtr.Zero)
                    return new GKPlayer(pointer);

                return null;
            }
        }

        /// <summary>
        /// The player attributes for the match.
        /// </summary>
        public uint PlayerAttributes => Interop.GKInvite_GetPlayerAttributes(Pointer);
        
        /// <summary>
        /// The player group for the match.
        /// </summary>
        public long PlayerGroup => Interop.GKInvite_GetPlayerGroup(Pointer);
        
        /// <summary>
        /// A Boolean value that indicates whether you host the game on your own servers.
        /// </summary>
        public bool IsHosted => Interop.GKInvite_GetIsHosted(Pointer);

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKInvite_SetInviteAcceptedCallback(InteropInviteAcceptedHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKInvite_GetSender(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern uint GKInvite_GetPlayerAttributes(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern long GKInvite_GetPlayerGroup(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKInvite_GetIsHosted(IntPtr pointer);
        }
    }
}
