using System;
using System.Runtime.InteropServices;
using Apple.Core.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace Apple.GameKit
{
    /// <summary>
    /// An object that allows players to view and manage their Game Center information from within your game.
    /// </summary>
    public class GKAccessPoint : NSObject
    {
        [Preserve]
        public GKAccessPoint(IntPtr pointer) : base(pointer)
        {
        }
        
        private static GKAccessPoint _shared;

        /// <summary>
        /// The shared access point object.
        /// </summary>
        public static GKAccessPoint Shared => _shared ??= PointerCast<GKAccessPoint>(Interop.GKAccessPoint_GetShared());

        /// <summary>
        /// The corner of the screen to display the access point.
        /// </summary>
        public GKAccessPointLocation Location
        {
            get => Interop.GKAccessPoint_GetLocation(Pointer);
            set => Interop.GKAccessPoint_SetLocation(Pointer, value);
        }
        
        /// <summary>
        /// The frame of the access point in screen coordinates.
        /// </summary>
        public Rect FrameInScreenCoordinates => Interop.GKAccessPoint_GetFrameInScreenCoordinates(Pointer).ToRect();
        
        /// <summary>
        /// The normalized frame of the access point in unit (0 -> 1) coordinates.
        /// </summary>
        public Rect FrameInUnitCoordinates => Interop.GKAccessPoint_GetFrameInUnitCoordinates(Pointer).ToRect();
        
        /// <summary>
        /// A Boolean value that determines whether to display the access point.
        /// </summary>
        public bool IsActive
        {
            get => Interop.GKAccessPoint_GetIsActive(Pointer);
            set => Interop.GKAccessPoint_SetIsActive(Pointer, value);
        }

        /// <summary>
        /// A Boolean value that indicates whether the game is presenting the Game Center dashboard.
        /// </summary>
        public bool IsPresentingGameCenter => Interop.GKAccessPoint_GetIsPresentingGameCenter(Pointer);
        
        /// <summary>
        /// A Boolean value that indicates whether the access point is visible.
        /// </summary>
        public bool IsVisible => Interop.GKAccessPoint_GetIsVisible(Pointer);
        
        /// <summary>
        /// A Boolean value that indicates whether to display highlights for achievements and current ranks for leaderboards.
        /// </summary>
        public bool ShowHighlights
        {
            get => Interop.GKAccessPoint_GetShowHighlights(Pointer);
            set => Interop.GKAccessPoint_SetShowHighlights(Pointer, value);
        }
        
#if UNITY_TVOS
        /// <summary>
        /// A Boolean value that indicates whether the access point is in focus on tvOS.
        /// </summary>
        public bool IsFocused => Interop.GKAccessPoint_GetIsFocused(Pointer);
#endif
        
        /// <summary>
        /// Displays the Game Center dashboard.
        /// </summary>
        public void Trigger() => Interop.GKAccessPoint_Trigger(Pointer);
        
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

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern IntPtr GKAccessPoint_GetShared();
            [DllImport(InteropUtility.DLLName)]
            public static extern GKAccessPointLocation GKAccessPoint_GetLocation(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_SetLocation(IntPtr pointer, GKAccessPointLocation location);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKAccessPointFrameInScreenCoordinates GKAccessPoint_GetFrameInScreenCoordinates(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern GKAccessPointFrameInScreenCoordinates GKAccessPoint_GetFrameInUnitCoordinates(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKAccessPoint_GetIsActive(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_SetIsActive(IntPtr pointer, bool isActive);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKAccessPoint_GetIsPresentingGameCenter(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKAccessPoint_GetIsVisible(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKAccessPoint_GetShowHighlights(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_SetShowHighlights(IntPtr pointer, bool isActive);

#if UNITY_TVOS
            [DllImport(InteropUtility.DLLName)]
            public static extern bool GKAccessPoint_GetIsFocused(IntPtr pointer);
#endif

            [DllImport(InteropUtility.DLLName)]
            public static extern void GKAccessPoint_Trigger(IntPtr pointer);
        }

    }
}
