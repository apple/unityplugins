using System;
using System.Runtime.InteropServices;
using Apple.Core.Runtime;
using UnityEngine;

namespace Apple.GameKit
{
    /// <summary>
    /// An object that allows players to view and manage their Game Center information from within your game.
    /// </summary>
    public class GKAccessPoint : InteropReference
    {
        #region Init & Dispose
        public GKAccessPoint(IntPtr pointer) : base(pointer)
        {
        }
        #endregion
        
        #region Shared
        [DllImport(InteropUtility.DLLName)]
        private static extern IntPtr GKAccessPoint_GetShared();

        private static GKAccessPoint _shared;

        /// <summary>
        /// The shared access point object.
        /// </summary>
        public static GKAccessPoint Shared
        {
            get
            {
                if(_shared == null)
                    _shared = PointerCast<GKAccessPoint>(GKAccessPoint_GetShared());

                return _shared;
            }
        }

        #endregion
        
        #region Location
        [DllImport(InteropUtility.DLLName)]
        private static extern GKAccessPointLocation GKAccessPoint_GetLocation(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAccessPoint_SetLocation(IntPtr pointer, GKAccessPointLocation location);

        /// <summary>
        /// The corner of the screen to display the access point.
        /// </summary>
        public GKAccessPointLocation Location
        {
            get => GKAccessPoint_GetLocation(Pointer);
            set => GKAccessPoint_SetLocation(Pointer, value);
        }
        #endregion
        
        #region FrameInScreenCoordinates
        [DllImport(InteropUtility.DLLName)]
        private static extern GKAccessPointFrameInScreenCoordinates GKAccessPoint_GetFrameInScreenCoordinates(IntPtr pointer);

        /// <summary>
        /// The frame of the access point in screen coordinates.
        /// </summary>
        public Rect FrameInScreenCoordinates
        {
            get => GKAccessPoint_GetFrameInScreenCoordinates(Pointer).ToRect();
        }
        #endregion
        
        #region FrameInUnitCoordinates
        [DllImport(InteropUtility.DLLName)]
        private static extern GKAccessPointFrameInScreenCoordinates GKAccessPoint_GetFrameInUnitCoordinates(IntPtr pointer);

        /// <summary>
        /// The normalized frame of the access point in unit (0 -> 1) coordinates.
        /// </summary>
        public Rect FrameInUnitCoordinates
        {
            get => GKAccessPoint_GetFrameInUnitCoordinates(Pointer).ToRect();
        }
        #endregion
        
        #region IsActive
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKAccessPoint_GetIsActive(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAccessPoint_SetIsActive(IntPtr pointer, bool isActive);

        /// <summary>
        /// A Boolean value that determines whether to display the access point.
        /// </summary>
        public bool IsActive
        {
            get => GKAccessPoint_GetIsActive(Pointer);
            set => GKAccessPoint_SetIsActive(Pointer, value);
        }
        #endregion
        
        #region IsPresentingGameCenter
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKAccessPoint_GetIsPresentingGameCenter(IntPtr pointer);

        /// <summary>
        /// A Boolean value that indicates whether the game is presenting the Game Center dashboard.
        /// </summary>
        public bool IsPresentingGameCenter
        {
            get => GKAccessPoint_GetIsPresentingGameCenter(Pointer);
        }
        #endregion
        
        #region IsVisible
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKAccessPoint_GetIsVisible(IntPtr pointer);

        /// <summary>
        /// A Boolean value that indicates whether the access point is visible.
        /// </summary>
        public bool IsVisible
        {
            get => GKAccessPoint_GetIsVisible(Pointer);
        }
        #endregion
        
        #region ShowHighlights
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKAccessPoint_GetShowHighlights(IntPtr pointer);
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAccessPoint_SetShowHighlights(IntPtr pointer, bool isActive);

        /// <summary>
        /// A Boolean value that indicates whether to display highlights for achievements and current ranks for leaderboards.
        /// </summary>
        public bool ShowHighlights
        {
            get => GKAccessPoint_GetShowHighlights(Pointer);
            set => GKAccessPoint_SetShowHighlights(Pointer, value);
        }
        #endregion
        
#if UNITY_TVOS
        #region IsFocused
        [DllImport(InteropUtility.DLLName)]
        private static extern bool GKAccessPoint_GetIsFocused(IntPtr pointer);

        /// <summary>
        /// A Boolean value that indicates whether the access point is in focus on tvOS.
        /// </summary>
        public bool IsFocused
        {
            get => GKAccessPoint_GetIsFocused(Pointer);
        }
        #endregion
#endif
        
        #region Trigger
        [DllImport(InteropUtility.DLLName)]
        private static extern void GKAccessPoint_Trigger(IntPtr pointer);

        /// <summary>
        /// Displays the Game Center dashboard.
        /// </summary>
        public void Trigger()
        {
            GKAccessPoint_Trigger(Pointer);
        }
        #endregion
        
        /// <summary>
        /// Specifies the corner of the screen to display the access point.
        /// </summary>
        public enum GKAccessPointLocation : long
        {
            /// <summary>
            /// The upper-left corner of the screen.
            /// </summary>
            TopLeading = 0,
            /// <summary>
            /// The upper-right corner of the screen.
            /// </summary>
            TopTrailing = 1,
            /// <summary>
            /// The lower-left corner of the screen.
            /// </summary>
            BottomLeading = 2,
            /// <summary>
            /// The lower-right corner of the screen.
            /// </summary>
            BottomTrailing = 3
        }
    }
}