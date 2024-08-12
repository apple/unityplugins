using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Apple.Core.Runtime;

namespace Apple.GameKit
{
    public class GKSavedGame : NSObject
    {
        internal GKSavedGame(IntPtr pointer) : base(pointer) { }

        /// <summary>
        /// The name of the saved game.
        /// </summary>
        public string Name => Interop.GKSavedGame_GetName(Pointer);

        /// <summary>
        /// The date when you saved the game data or modified it.
        /// </summary>
        public DateTimeOffset ModificationDate => DateTimeOffset.FromUnixTimeSeconds((long)Interop.GKSavedGame_GetModificationDate(Pointer));

        /// <summary>
        /// The name of the device that the player uses to save the game.
        /// </summary>
        public string DeviceName => Interop.GKSavedGame_GetDeviceName(Pointer);

        #region LoadData

        /// <summary>
        /// Loads the game data from the file.
        /// </summary>
        /// <returns>
        /// The data object that you saved to the file using the SaveGameData(byte[] data, string name)
        /// </returns>
        public Task<byte[]> LoadData()
        {
            var tcs = InteropTasks.Create<byte[]>(out var taskId);
            Interop.GKSavedGame_LoadData(Pointer, taskId, OnLoadData, OnLoadDataError);
            return tcs.Task;
        }

        [MonoPInvokeCallback(typeof(SuccessTaskCallback<InteropData>))]
        private static void OnLoadData(long taskId, InteropData data)
        {
            InteropTasks.TrySetResultAndRemove(taskId, data.ToBytes());
        }

        [MonoPInvokeCallback(typeof(NSErrorTaskCallback))]
        private static void OnLoadDataError(long taskId, IntPtr errorPointer)
        {
            InteropTasks.TrySetExceptionAndRemove<byte[]>(taskId, new GameKitException(errorPointer));
        }
        #endregion

        private static class Interop
        {
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKSavedGame_GetName(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern double GKSavedGame_GetModificationDate(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern string GKSavedGame_GetDeviceName(IntPtr pointer);
            [DllImport(InteropUtility.DLLName)]
            public static extern void GKSavedGame_LoadData(IntPtr pointer, long taskId, SuccessTaskCallback<InteropData> onSuccess, NSErrorTaskCallback onError);
        }
    }
}