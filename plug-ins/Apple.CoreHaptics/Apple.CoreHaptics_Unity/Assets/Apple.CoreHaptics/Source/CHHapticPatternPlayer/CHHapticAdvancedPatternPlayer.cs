using AOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Apple.CoreHaptics
{
	public class CHHapticAdvancedPatternPlayer : CHHapticPatternPlayer
	{
		public delegate void AdvancedPlayerCompletionHandler();
		public event AdvancedPlayerCompletionHandler CompletionHandler;

		public CHHapticAdvancedPatternPlayer(IntPtr existingPlayerId, CHHapticEngine engine) : base(existingPlayerId, engine)
		{
		}

		public new bool Muted
		{
			get => PlayerId != IntPtr.Zero && CHHapticAdvancedPatternPlayerInterface.GetIsMuted(PlayerId);
			set
			{
				if (PlayerId != IntPtr.Zero) CHHapticAdvancedPatternPlayerInterface.SetIsMuted(PlayerId, value);
			}
		}

		public bool LoopEnabled
		{
			get => PlayerId != IntPtr.Zero && CHHapticAdvancedPatternPlayerInterface.GetLoopEnabled(PlayerId);
			set
			{
				if (PlayerId != IntPtr.Zero) CHHapticAdvancedPatternPlayerInterface.SetLoopEnabled(PlayerId, value);
			}
		}

		public double LoopEnd
		{
			get => PlayerId != IntPtr.Zero ? CHHapticAdvancedPatternPlayerInterface.GetLoopEnd(PlayerId) : 0;
			set
			{
				if (PlayerId != IntPtr.Zero) CHHapticAdvancedPatternPlayerInterface.SetLoopEnd(PlayerId, value);
			}
		}

		public float PlaybackRate
		{
			get => PlayerId != IntPtr.Zero ? CHHapticAdvancedPatternPlayerInterface.GetPlaybackRate(PlayerId) : 0;
			set
			{
				if (PlayerId != IntPtr.Zero) CHHapticAdvancedPatternPlayerInterface.SetPlaybackRate(PlayerId, value);
			}
		}

		public override void Start(float startTime = 0f)
		{
			if (PlayerId == IntPtr.Zero)
				return;

			ResetLastOperationException();
			CHHapticAdvancedPatternPlayerInterface.Start(PlayerId, startTime, OnThrowError);
			CheckAndThrowLastOperationException();
		}

		public override void Stop(float stopTime = 0f)
		{
			if (PlayerId == IntPtr.Zero)
				return;

			ResetLastOperationException();
			CHHapticAdvancedPatternPlayerInterface.Stop(PlayerId, stopTime, OnThrowError);
			CheckAndThrowLastOperationException();
		}

		public void Pause(float pauseTime = 0f)
		{
			if (PlayerId == IntPtr.Zero)
				return;

			ResetLastOperationException();
			CHHapticAdvancedPatternPlayerInterface.Pause(PlayerId, pauseTime, OnThrowError);
			CheckAndThrowLastOperationException();
		}

		public void Resume(float resumeTime = 0f)
		{
			if (PlayerId == IntPtr.Zero)
				return;

			ResetLastOperationException();
			CHHapticAdvancedPatternPlayerInterface.Resume(PlayerId, resumeTime, OnThrowError);
			CheckAndThrowLastOperationException();
		}

		public void Seek(float toOffset)
		{
			if (PlayerId == IntPtr.Zero)
				return;

			ResetLastOperationException();
			CHHapticAdvancedPatternPlayerInterface.Seek(PlayerId, toOffset, OnThrowError);
			CheckAndThrowLastOperationException();
		}

		public override void SendParameters(IEnumerable<CHHapticParameter> parameters, float atTime = 0f)
		{
			if (PlayerId == IntPtr.Zero)
				return;

			// Build request...
			var sendParameters = parameters.Select(p => new CHSendParameter(p)).ToArray();
			var handle = GCHandle.Alloc(sendParameters, GCHandleType.Pinned);

			var request = new CHSendParametersRequest
			{
				Parameters = handle.AddrOfPinnedObject(),
				ParametersLength = sendParameters.Length,
				AtTime = atTime
			};

			// Send request...
			ResetLastOperationException();
			CHHapticAdvancedPatternPlayerInterface.SendParameters(PlayerId, request, OnThrowError);
			handle.Free();
			CheckAndThrowLastOperationException();
		}

		public override void ScheduleParameterCurve(CHHapticParameterCurve curve, float atTime = 0f)
		{
			throw new NotSupportedException("AdvancedPatternPlayer does not support ScheduleParameterCurve");
		}

		public new void Destroy()
		{
			if (PlayerId == IntPtr.Zero)
				return;

			CHHapticAdvancedPatternPlayerInterface.Destroy(PlayerId);
			PlayerId = IntPtr.Zero;
		}

		#region Interop Callbacks
		[MonoPInvokeCallback(typeof(SuccessWithPointerCallback))]
		internal static void OnAdvancedPlayerFinishedPlaying(IntPtr playerPointer)
		{
			if (PointerToPlayers.TryGetValue(playerPointer, out var player)
				&& player is CHHapticAdvancedPatternPlayer advancedPatternPlayer)
			{
				advancedPatternPlayer.CompletionHandler?.Invoke();
			}
		}
		#endregion
	}
}
