using System;
using System.Runtime.InteropServices;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;

namespace Apple.StoreKit
{
#if UNITY_IOS || UNITY_VISIONOS
    public enum SKOverlayPosition : int
    {
        Bottom = 0,
        BottomRaised = 1
    }

    [Introduced(iOS: "14.0", visionOS: "2.2")]
    [Unavailable(RuntimeOperatingSystem.tvOS, RuntimeOperatingSystem.macOS)]
    public static class SKOverlay
    {
        #region Delegates
        private delegate void OverlayCallback();
        private delegate void OverlayErrorCallback(IntPtr errorPointer);
        #endregion

        #region Events
        /// <summary>
        /// Raised when the overlay is about to start presenting.
        /// </summary>
        public static event Action WillStartPresentation;

        /// <summary>
        /// Raised when the overlay has finished presenting.
        /// </summary>
        public static event Action DidFinishPresentation;

        /// <summary>
        /// Raised when the overlay is about to start dismissing.
        /// </summary>
        public static event Action WillStartDismissal;

        /// <summary>
        /// Raised when the overlay has finished dismissing.
        /// </summary>
        public static event Action DidFinishDismissal;

        /// <summary>
        /// Raised when the overlay fails to load.
        /// </summary>
        public static event Action<StoreKitException> DidFailToLoad;
        #endregion

        #region Static Methods
        /// <summary>
        /// Presents an App Store overlay recommending the app with the given identifier.
        /// </summary>
        /// <param name="appIdentifier">The iTunes item identifier of the app to recommend.</param>
        /// <param name="position">The position of the overlay on screen.</param>
        public static void Present(string appIdentifier, SKOverlayPosition position = SKOverlayPosition.Bottom)
        {
            Interop.SKOverlay_Present(
                appIdentifier,
                (int)position,
                OnWillStartPresentation,
                OnDidFinishPresentation,
                OnWillStartDismissal,
                OnDidFinishDismissal,
                OnDidFailToLoad);
        }

        /// <summary>
        /// Dismisses the currently presented App Store overlay.
        /// </summary>
        public static void Dismiss()
        {
            Interop.SKOverlay_Dismiss();
        }
        #endregion

        #region Callback Handlers
        [MonoPInvokeCallback(typeof(OverlayCallback))]
        private static void OnWillStartPresentation()
        {
            WillStartPresentation?.Invoke();
        }

        [MonoPInvokeCallback(typeof(OverlayCallback))]
        private static void OnDidFinishPresentation()
        {
            DidFinishPresentation?.Invoke();
        }

        [MonoPInvokeCallback(typeof(OverlayCallback))]
        private static void OnWillStartDismissal()
        {
            WillStartDismissal?.Invoke();
        }

        [MonoPInvokeCallback(typeof(OverlayCallback))]
        private static void OnDidFinishDismissal()
        {
            DidFinishDismissal?.Invoke();
        }

        [MonoPInvokeCallback(typeof(OverlayErrorCallback))]
        private static void OnDidFailToLoad(IntPtr errorPointer)
        {
            DidFailToLoad?.Invoke(new StoreKitException(errorPointer));
        }
        #endregion

        #region Interop
        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void SKOverlay_Present(
                string appIdentifier,
                int position,
                OverlayCallback onWillStartPresentation,
                OverlayCallback onDidFinishPresentation,
                OverlayCallback onWillStartDismissal,
                OverlayCallback onDidFinishDismissal,
                OverlayErrorCallback onDidFailToLoad);

            [DllImport(InteropUtility.DLLName)]
            public static extern void SKOverlay_Dismiss();
        }
        #endregion
    }
#endif
}
