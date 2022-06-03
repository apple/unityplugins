using System;
using System.Runtime.InteropServices;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit.Multiplayer
{
    /// <summary>
    /// An invitation to join a match sent to the local player from another player.
    /// </summary>
    public class GKInvite : InteropReference
    {
        #region Delegates
        public delegate void InviteAcceptedHandler(GKPlayer invitingPlayer, GKInvite invite);
        private delegate void InteropInviteAcceptedHandler(IntPtr player, IntPtr invite);
        #endregion
        
        #region Static Events
        /// <summary>
        /// Handles the event when the local player accepts an invitation from another player.
        /// </summary>
        public static event InviteAcceptedHandler InviteAccepted;
        #endregion
        
        #region Static Event Registration
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKInvite_SetInviteAcceptedCallback(InteropInviteAcceptedHandler callback);

        static GKInvite()
        {
            GKInvite_SetInviteAcceptedCallback(OnInviteAccepted);
        }

        [MonoPInvokeCallback(typeof(InteropInviteAcceptedHandler))]
        private static void OnInviteAccepted(IntPtr player, IntPtr invite)
        {
            InviteAccepted?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKInvite>(invite));
        }
        #endregion
        
        #region Init & Dipose
        internal GKInvite(IntPtr pointer) : base(pointer){}
        
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKInvite_Free(IntPtr pointer);

        protected override void OnDispose(bool isDisposing)
        {
            if (Pointer != IntPtr.Zero)
            {
                GKInvite_Free(Pointer);
                Pointer = IntPtr.Zero;
            }
        }

        #endregion
        
        #region Sender
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKInvite_GetSender(IntPtr pointer);

        /// <summary>
        /// The player who sends the invitation.
        /// </summary>
        public GKPlayer Sender
        {
            get
            {
                var pointer = GKInvite_GetSender(Pointer);
                
                if(pointer != IntPtr.Zero)
                    return new GKPlayer(pointer);

                return null;
            }
        }
        #endregion
        
        #region PlayerAttributes
        [DllImport(InteropUtility.DLLName)]
        private static extern uint GKInvite_GetPlayerAttributes(IntPtr pointer);

        /// <summary>
        /// The player attributes for the match.
        /// </summary>
        public uint PlayerAttributes
        {
            get => GKInvite_GetPlayerAttributes(Pointer);
        }
        #endregion
        
        #region PlayerGroup
        [DllImport(InteropUtility.DLLName)]
        private static extern long GKInvite_GetPlayerGroup(IntPtr pointer);

        /// <summary>
        /// The player group for the match.
        /// </summary>
        public long PlayerGroup
        {
            get => GKInvite_GetPlayerGroup(Pointer);
        }
        #endregion
        
        #region IsHosted
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKInvite_GetIsHosted(IntPtr pointer);

        /// <summary>
        /// A Boolean value that indicates whether you host the game on your own servers.
        /// </summary>
        public bool IsHosted
        {
            get => GKInvite_GetIsHosted(Pointer);
        }
        #endregion
    }
}