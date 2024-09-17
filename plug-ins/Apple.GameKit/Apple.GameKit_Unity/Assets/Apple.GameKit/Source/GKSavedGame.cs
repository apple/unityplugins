using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core;
using Apple.Core.Runtime;

#if !UNITY_TVOS

namespace Apple.GameKit
{
    /// <summary>
    /// An object that represents a file containing saved game data.
    /// </summary>
    [Unavailable(RuntimeOperatingSystem.tvOS)]
    public class GKSavedGame : NSObject
    {
        #region Delegates
        public delegate void SavedGameModifiedHandler(GKPlayer player, GKSavedGame savedGame);
        private delegate void InteropSavedGameModifiedHandler(IntPtr player, IntPtr savedGame);

        public delegate void SavedGamesConflictingHandler(GKPlayer player, NSArray<GKSavedGame> savedGames);
        private delegate void InteropSavedGamesConflictingHandler(IntPtr player, IntPtr savedGames);
        #endregion

        #region Static Events
        /// <summary>
        /// Handles when data changes in a saved game file.
        /// </summary>
        public static event SavedGameModifiedHandler SavedGameModified;

        /// <summary>
        /// Chooses the correct game data from the saved games that contain conflicts.
        /// </summary>
        public static event SavedGamesConflictingHandler SavedGamesConflicting;
        #endregion

        #region Static Event Registration
        static GKSavedGame()
        {
            Interop.GKSavedGame_SetSavedGameModifiedCallback(OnSavedGameModified);
            Interop.GKSavedGame_SetSavedGamesConflictingCallback(OnSavedGamesConflicting);
        }

        [MonoPInvokeCallback(typeof(InteropSavedGameModifiedHandler))]
        private static void OnSavedGameModified(IntPtr player, IntPtr savedGame)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => SavedGameModified?.Invoke(PointerCast<GKPlayer>(player), PointerCast<GKSavedGame>(savedGame)));
        }

        [MonoPInvokeCallback(typeof(InteropSavedGamesConflictingHandler))]
        private static void OnSavedGamesConflicting(IntPtr player, IntPtr savedGames)
        {
            InteropPInvokeExceptionHandler.CatchAndLog(() => SavedGamesConflicting?.Invoke(PointerCast<GKPlayer>(player), PointerCast<NSArray<GKSavedGame>>(savedGames)));
        }
        #endregion

        internal GKSavedGame(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// The name of the device that the player uses to save the game.
        /// </summary>
        /// <symbol>c:objc(cs)GKSavedGame(py)deviceName</symbol>
        public NSString DeviceName => Interop.GKSavedGame_GetDeviceName(Pointer);

        /// <summary>
        /// The date when you saved the game data or modified it.
        /// </summary>
        /// <symbol>c:objc(cs)GKSavedGame(py)modificationDate</symbol>
        public DateTimeOffset ModificationDate => DateTimeOffsetExtensions.FromUnixTimeSeconds(Interop.GKSavedGame_GetModificationDate(Pointer));

        /// <summary>
        /// The name of the saved game.
        /// </summary>
        /// <symbol>c:objc(cs)GKSavedGame(py)name</symbol>
        public NSString Name => Interop.GKSavedGame_GetName(Pointer);
        public override string ToString() => Name.ToString();

        /// <summary>
        /// Loads the game data from the file.
        /// </summary>
        /// <symbol>c:objc(cs)GKSavedGame(im)loadDataWithCompletionHandler:</symbol>
        public Task<NSData> LoadData()
        {
            var tcs = InteropTasks.Create<NSData>(out var taskId);
            Interop.GKSavedGame_LoadData(Pointer, taskId, OnLoadData, OnLoadDataError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<IntPtr>))]
        private static void OnLoadData(long taskId, IntPtr nsDataPtr)
        {
            InteropTasks.TrySetResultAndRemove(taskId, PointerCast<NSData>(nsDataPtr));
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadDataError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<NSData>(taskId, new GameKitException(errorPointer));
        }

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKSavedGame_SetSavedGameModifiedCallback(InteropSavedGameModifiedHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKSavedGame_SetSavedGamesConflictingCallback(InteropSavedGamesConflictingHandler callback);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKSavedGame_GetName(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKSavedGame_GetModificationDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKSavedGame_GetDeviceName(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKSavedGame_LoadData(IntPtr pointer, long taskId, SuccessTaskCallback<IntPtr> onSuccess, NSErrorTaskCallback onError);
        }
    }
}

#endif // !UNITY_TVOS
