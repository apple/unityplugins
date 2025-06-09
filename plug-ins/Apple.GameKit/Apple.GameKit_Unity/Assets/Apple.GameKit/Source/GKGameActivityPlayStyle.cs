using Apple.Core;

namespace Apple.GameKit
{
    /// <summary>
    /// Play Style of the game activity. It can be either Asynchronous or Synchronous.
    /// </summary>
    /// <symbol>c:@E@GKGameActivityPlayStyle</symbol>
    [Introduced(iOS: "19.0.0", macOS: "16.0.0", tvOS: "19.0.0", visionOS: "3.0.0")]
    public enum GKGameActivityPlayStyle : long
    {
        /// <symbol>c:@E@GKGameActivityPlayStyle@GKGameActivityPlayStyleUnspecified</symbol>
        Unspecified = 0,

        /// <symbol>c:@E@GKGameActivityPlayStyle@GKGameActivityPlayStyleSynchronous</symbol>
        Synchronous = 1,

        /// <symbol>c:@E@GKGameActivityPlayStyle@GKGameActivityPlayStyleAsynchronous</symbol>
        Asynchronous = 2
    }
}
