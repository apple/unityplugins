using System;
using System.Runtime.InteropServices;

namespace Apple.CoreHaptics
{
    internal static class CHHapticEngineInterface
    {
        #region Create
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_Create")]
        internal static extern IntPtr Create(ErrorCallback onError, GenericCallback<IntPtr, CHHapticEngineStoppedReason> onStopped, SuccessWithPointerCallback onReset);
        #endregion

        #region Destroy
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_Destroy")]
        internal static extern void Destroy(IntPtr enginePtr);
        #endregion

        #region Start
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_Start")]
        internal static extern void Start(IntPtr enginePtr, ErrorWithPointerCallback onError);
        #endregion

        #region Stop
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_Stop")]
        internal static extern void Stop(IntPtr enginePtr, ErrorWithPointerCallback onError);
        #endregion

        #region MakePlayer
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_MakePlayer")]
        internal static extern IntPtr MakePlayer(IntPtr enginePtr, string ahapJson, ErrorWithPointerCallback onError);
        #endregion

        #region MakeAdvancedPlayer
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_MakeAdvancedPlayer")]
        internal static extern IntPtr MakeAdvancedPlayer(IntPtr enginePtr, string ahapJson, SuccessWithPointerCallback onFinishedPlaying, ErrorWithPointerCallback onError);
        #endregion
        
        #region PlayPatternFromUrl
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_PlayPatternFromUrl")]
        internal static extern void PlayPatternFromUrl(IntPtr enginePtr, string ahapUrl, ErrorWithPointerCallback onError);
        #endregion
        
        #region PlayPatternFromJson
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_PlayPatternFromJson")]
        internal static extern void PlayPatternFromJson(IntPtr enginePtr, string ahapJson, ErrorWithPointerCallback onError);
        #endregion
        
        #region NotifyWhenPlayersFinished

        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_NotifyWhenPlayersFinished")]
        internal static extern void NotifyWhenPlayersFinished(IntPtr enginePointer, bool leaveEngineRunning, CHHapticEngine.AllPlayersFinishedStatic onFinished, ErrorWithPointerCallback onError);
        #endregion

        #region HardwareSupportsHaptics
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_HardwareSupportsHaptics")]
        internal static extern bool HardwareSupportsHaptics();
        #endregion
        
        #region PlayHapticsOnly
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_Get_PlaysHapticsOnly")]
        internal static extern bool Get_PlaysHapticsOnly(IntPtr enginePointer);
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_Set_PlaysHapticsOnly")]
        internal static extern void Set_PlaysHapticsOnly(IntPtr enginePointer, bool playsHapticsOnly);
        #endregion

        #region IsMutedForAudio
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_Get_IsMutedForAudio")]
        internal static extern bool Get_IsMutedForAudio(IntPtr enginePointer);
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_Set_IsMutedForAudio")]
        internal static extern void Set_IsMutedForAudio(IntPtr enginePointer, bool isMutedForAudio);
        #endregion

        #region IsMutedForHaptics
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_Get_IsMutedForHaptics")]
        internal static extern bool Get_IsMutedForHaptics(IntPtr enginePointer);
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_Set_IsMutedForHaptics")]
        internal static extern void Set_IsMutedForHaptics(IntPtr enginePointer, bool isMutedForHaptics);
        #endregion
        
        #region IsAutoShutdownEnabled
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_Get_IsAutoShutdownEnabled")]
        internal static extern bool Get_IsAutoShutdownEnabled(IntPtr enginePointer);
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_Set_IsAutoShutdownEnabled")]
        internal static extern void Set_IsAutoShutdownEnabled(IntPtr enginePointer, bool isAutoShutdownEnabled);
        #endregion
        
        #region CurrentTime
        [DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticEngine_Get_CurrentTime")]
        internal static extern double Get_CurrentTime(IntPtr enginePointer);
        #endregion
    }
}
