using System;
using System.Runtime.InteropServices;

namespace Apple.CoreHaptics
{
    internal static class CHHapticPatternPlayerInterface
    {
        #region Destroy
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticPatternPlayer_Destroy")]
        internal static extern IntPtr Destroy(IntPtr playerId);
        #endregion

        #region Muted
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticPatternPlayer_GetIsMuted")]
        internal static extern bool GetIsMuted(IntPtr playerId);
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticPatternPlayer_SetIsMuted")]
        internal static extern void SetIsMuted(IntPtr playerId, bool isMuted);
        #endregion

        #region Play
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticPatternPlayer_Start")]
        internal static extern void Start(IntPtr playerId, float startTime, ErrorWithPointerCallback onError);
        #endregion

        #region Stop
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticPatternPlayer_Stop")]
        internal static extern void Stop(IntPtr playerId, float stopTime, ErrorWithPointerCallback onError);
        #endregion
        
        #region SendParameters
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticPatternPlayer_SendParameters")]
        internal static extern void SendParameters(IntPtr playerId, CHSendParametersRequest request, ErrorWithPointerCallback onError);
        #endregion

        #region SchedulParameterCurve
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticPatternPlayer_ScheduleParameterCurve")]
        internal static extern void ScheduleParameterCurve(IntPtr playerId, CHHapticParameterCurveRequest request, float atTime, ErrorWithPointerCallback onError);
        #endregion
    }
}