using System;
using System.Runtime.InteropServices;

namespace Apple.CoreHaptics
{
	internal static class CHHapticAdvancedPatternPlayerInterface
	{
		#region Destroy
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_Destroy")]
		internal static extern IntPtr Destroy(IntPtr playerId);
		#endregion

		#region Muted
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_GetIsMuted")]
		internal static extern bool GetIsMuted(IntPtr playerId);
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_SetIsMuted")]
		internal static extern void SetIsMuted(IntPtr playerId, bool isMuted);
		#endregion

		#region Play
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_Start")]
		internal static extern void Start(IntPtr playerId, float startTime, ErrorWithPointerCallback onError);
		#endregion

		#region Stop
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_Stop")]
		internal static extern void Stop(IntPtr playerId, float stopTime, ErrorWithPointerCallback onError);
		#endregion

		#region Pause
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_Pause")]
		internal static extern void Pause(IntPtr playerId, float pauseTime, ErrorWithPointerCallback onError);
		#endregion

		#region Resume
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_Resume")]
		internal static extern void Resume(IntPtr playerId, float resumeTime, ErrorWithPointerCallback onError);
		#endregion

		#region Seek
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_Seek")]
		internal static extern void Seek(IntPtr playerId, float toOffset, ErrorWithPointerCallback onError);
		#endregion

		#region LoopEnabled
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_GetLoopEnabled")]
		internal static extern bool GetLoopEnabled(IntPtr playerId);
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_SetLoopEnabled")]
		internal static extern void SetLoopEnabled(IntPtr playerId, bool loopEnabled);
		#endregion

		#region LoopEnd
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_GetLoopEnd")]
		internal static extern double GetLoopEnd(IntPtr playerId);
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_SetLoopEnd")]
		internal static extern void SetLoopEnd(IntPtr playerId, double loopEnd);
		#endregion

		#region PlaybackRate
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_GetPlaybackRate")]
		internal static extern float GetPlaybackRate(IntPtr playerId);
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_SetPlaybackRate")]
		internal static extern void SetPlaybackRate(IntPtr playerId, float playbackRate);
		#endregion

		#region SendParameters
		[DllImport(CHInteropUtility.DllName, EntryPoint = "CoreHaptics_CHHapticAdvancedPatternPlayer_SendParameters")]
		internal static extern void SendParameters(IntPtr playerId, CHSendParametersRequest request, ErrorWithPointerCallback onError);
		#endregion
	}
}