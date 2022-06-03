using System;
using System.Collections.Generic;
using System.Linq;
using AOT;
using UnityEngine;

namespace Apple.CoreHaptics
{
	public class CHHapticEngine
	{
		#region Delegates
		/// <summary>
		/// The delegate signature that a developer should use for subscribing
		/// to the Reset Event.
		/// See <see cref="http://developer.apple.com/documentation/corehaptics/chhapticengine/3043661-resethandler"/>
		/// </summary>
		public delegate void EngineResetHandler();

		/// <summary>
		/// The delegate signature that a developer should use for subscribing
		/// to the EngineStoppedHandler Event.
		/// <see cref="http://developer.apple.com/documentation/corehaptics/chhapticengine/3043665-stoppedhandler"/>
		/// </summary>
		/// <remarks>
		/// The stopped handler is only called when the engine is stopped for a
		/// reason other than a call to CHHapticEngine.StopEngine()
		/// </remarks>
		/// <param name="reason">The reason the Engine stopped</param>
		public delegate void EngineStoppedHandler(CHHapticEngineStoppedReason reason);

		internal delegate void AllPlayersFinishedStatic(IntPtr engPtr);
		public delegate void NotifyAllPlayersFinished();
		#endregion

		#region Static Properties & Events
		private static readonly Dictionary<IntPtr, CHHapticEngine> _pointerToEngines = new Dictionary<IntPtr, CHHapticEngine>();
		private static readonly object _pointerToEnginesLock = new object();

		private static CHException _lastCreationException;
		private static CHHapticEngine _oneShotEngine;

		public static CHHapticEngine OneShotEngine => GetOneShotEngine();

		#endregion

		#region Events
		public event EngineResetHandler Reset;
		public event EngineStoppedHandler Stopped;
		public event NotifyAllPlayersFinished PlayersFinished;
		#endregion

		#region Instance Fields & Properties
		public IntPtr EnginePtr = IntPtr.Zero;

		private readonly List<CHHapticPlayerConfiguration> _playerConfigurations = new List<CHHapticPlayerConfiguration>();
		private CHException _lastOperationException;

		#endregion

		#region Interop Properties
		public bool PlaysHapticsOnly
		{
			get => EnginePtr != IntPtr.Zero && CHHapticEngineInterface.Get_PlaysHapticsOnly(EnginePtr);
			set
			{
				if (EnginePtr != IntPtr.Zero) CHHapticEngineInterface.Set_PlaysHapticsOnly(EnginePtr, value);
			}
		}

		public bool IsMutedForAudio
		{
			get => EnginePtr != IntPtr.Zero && CHHapticEngineInterface.Get_IsMutedForAudio(EnginePtr);
			set
			{
				if (EnginePtr != IntPtr.Zero) CHHapticEngineInterface.Set_IsMutedForAudio(EnginePtr, value);
			}
		}

		public bool IsMutedForHaptics
		{
			get => EnginePtr != IntPtr.Zero && CHHapticEngineInterface.Get_IsMutedForHaptics(EnginePtr);
			set
			{
				if (EnginePtr != IntPtr.Zero) CHHapticEngineInterface.Set_IsMutedForHaptics(EnginePtr, value);
			}
		}

		public bool IsAutoShutdownEnabled
		{
			get => EnginePtr != IntPtr.Zero && CHHapticEngineInterface.Get_IsAutoShutdownEnabled(EnginePtr);
			set
			{
				if (EnginePtr != IntPtr.Zero) CHHapticEngineInterface.Set_IsAutoShutdownEnabled(EnginePtr, value);
			}
		}

		public double CurrentTime => EnginePtr != IntPtr.Zero ? CHHapticEngineInterface.Get_CurrentTime(EnginePtr) : 0;

		public static bool HardwareSupportsHaptics()
		{
			return CHHapticEngineInterface.HardwareSupportsHaptics();
		}
		#endregion

		public CHHapticEngine()
		{
			CreateEngine();
			PostCreateSetup();
		}

		public CHHapticEngine(IntPtr existingEnginePtr)
		{
			EnginePtr = existingEnginePtr;
			PostCreateSetup();
		}

		private void CreateEngine()
		{
			_lastCreationException = null;

			EnginePtr = CHHapticEngineInterface.Create(OnCreateError, OnStoppedWithReason, OnReset);

			if (!(_lastCreationException is null))
				throw _lastCreationException;
		}

		private void PostCreateSetup()
		{
			if (EnginePtr == IntPtr.Zero)
				return;

			lock (_pointerToEnginesLock)
			{
				_pointerToEngines[EnginePtr] = this;
			}
		}

		public void Destroy()
		{
			DestroyAllPlayers();

			if (EnginePtr == IntPtr.Zero)
				return;

			lock (_pointerToEnginesLock)
			{
				_pointerToEngines.Remove(EnginePtr);
			}

			CHHapticEngineInterface.Destroy(EnginePtr);
			EnginePtr = IntPtr.Zero;
		}

		public void Start()
		{
			ResetLastOperationException();
			CHHapticEngineInterface.Start(EnginePtr, OnThrowErrorWithPointer);
			CheckAndThrowLastOperationException();
		}
		public void Stop()
		{
			ResetLastOperationException();
			CHHapticEngineInterface.Stop(EnginePtr, OnThrowErrorWithPointer);
			CheckAndThrowLastOperationException();
		}

		#region Destroy Players
		public void DestroyAllPlayers()
		{
			foreach (var config in _playerConfigurations)
			{
				config.Player?.Destroy();
			}

			_playerConfigurations.Clear();
		}

		public void DestroyPlayer(CHHapticPatternPlayer player)
		{
			var config = _playerConfigurations.FirstOrDefault(c => c.Player == player);
			_playerConfigurations.Remove(config);

			player.Destroy();
		}
		#endregion

		#region Make Players
		public CHHapticPatternPlayer MakePlayer(List<CHHapticEvent> events)
		{
			var ahap = new CHHapticPattern(events);
			return MakePlayer(ahap);
		}

		public CHHapticPatternPlayer MakePlayer(CHHapticPattern ahap)
		{
			var ahapJson = CHSerializer.SerializeForRuntime(ahap);

			ResetLastOperationException();
			var playerId = CHHapticEngineInterface.MakePlayer(EnginePtr, ahapJson, OnThrowErrorWithPointer);
			CheckAndThrowLastOperationException();

			var player = new CHHapticPatternPlayer(playerId, this);
			_playerConfigurations.Add(new CHHapticPlayerConfiguration(ahap, player));
			player.Setup();

			return player;
		}
		#endregion

		#region MakeAdvancdPlayers
		public CHHapticAdvancedPatternPlayer MakeAdvancedPlayer(List<CHHapticEvent> events)
		{
			var ahap = new CHHapticPattern(events);
			return MakeAdvancedPlayer(ahap);
		}

		public CHHapticAdvancedPatternPlayer MakeAdvancedPlayer(CHHapticPattern ahap)
		{
			var ahapJson = CHSerializer.SerializeForRuntime(ahap);

			ResetLastOperationException();
			var playerId = CHHapticEngineInterface.MakeAdvancedPlayer(EnginePtr, ahapJson, CHHapticAdvancedPatternPlayer.OnAdvancedPlayerFinishedPlaying, OnThrowErrorWithPointer);
			CheckAndThrowLastOperationException();

			var player = new CHHapticAdvancedPatternPlayer(playerId, this);
			_playerConfigurations.Add(new CHHapticPlayerConfiguration(ahap, player));
			player.Setup();

			return player;
		}
		#endregion

		#region TryGetPlayerForPattern && TryGetAdvancedPlayerForPattner
		public bool TryGetPlayerForPattern(CHHapticPattern ahap, out CHHapticPatternPlayer player)
		{
			var config = _playerConfigurations.FirstOrDefault(c => c.Ahap == ahap);

			if (config?.Player != null)
			{
				player = config.Player;
				return true;
			}

			player = null;
			return false;
		}

		public bool TryGetAdvancedPlayerForPattern(CHHapticPattern ahap, out CHHapticAdvancedPatternPlayer player)
		{
			var config = _playerConfigurations.FirstOrDefault(c => c.Ahap == ahap);

			if (config?.Player is CHHapticAdvancedPatternPlayer advancedPatternPlayer)
			{
				player = advancedPatternPlayer;
				return true;
			}

			player = null;
			return false;
		}
		#endregion

		#region Refresh Players

		public void RefreshPlayers()
		{
			foreach (var config in _playerConfigurations)
			{
				// Ensure we destroy the player if not already done...
				config.Player.Destroy();

				// Recreate the player...
				var ahapJson = CHSerializer.SerializeForRuntime(config.Ahap);
				var isAdvancedPlayer = config.Player is CHHapticAdvancedPatternPlayer;

				if (isAdvancedPlayer)
				{
					ResetLastOperationException();
					config.Player.PlayerId = CHHapticEngineInterface.MakeAdvancedPlayer(EnginePtr, ahapJson, CHHapticAdvancedPatternPlayer.OnAdvancedPlayerFinishedPlaying, OnThrowErrorWithPointer);
					CheckAndThrowLastOperationException();
				}
				else
				{
					ResetLastOperationException();
					config.Player.PlayerId = CHHapticEngineInterface.MakePlayer(EnginePtr, ahapJson, OnThrowErrorWithPointer);
					CheckAndThrowLastOperationException();
				}
			}
		}
		#endregion

		#region NotifyWhenPlayersFinished

		public void NotifyWhenPlayersFinished(bool leaveEngineRunning = true)
		{
			Debug.Log("Registering notify finished handler");
			CHHapticEngineInterface.NotifyWhenPlayersFinished(EnginePtr, leaveEngineRunning, OnAllPlayersFinished, OnThrowErrorWithPointer);
		}
		#endregion

		#region PlayPatternFromUrl, PlayPatternFromJson, and PlayPatternFromAHAP
		public void PlayPatternFromUrl(string ahapFilePath)
		{
			if (!ahapFilePath.Contains("StreamingAssets") && !ahapFilePath.Contains(Application.streamingAssetsPath))
			{
				throw new ArgumentException("AHAP URL must be in StreamingAssets for proper playback.");
			}
			ResetLastOperationException();
			CHHapticEngineInterface.PlayPatternFromUrl(EnginePtr, ahapFilePath, OnThrowErrorWithPointer);
			CheckAndThrowLastOperationException();
		}

		public void PlayPatternFromJson(string ahapJson)
		{
			ResetLastOperationException();
			CHHapticEngineInterface.PlayPatternFromJson(EnginePtr, ahapJson, OnThrowErrorWithPointer);
			CheckAndThrowLastOperationException();
		}

		public void PlayPattern(CHHapticPattern ahap)
		{
			if (ahap is null)
			{
				throw new ArgumentException("Cannot play a null AHAP.");
			}

			var ahapJson = CHSerializer.SerializeForRuntime(ahap);

			ResetLastOperationException();
			CHHapticEngineInterface.PlayPatternFromJson(EnginePtr, ahapJson, OnThrowErrorWithPointer);
			CheckAndThrowLastOperationException();
		}

		public void PlayPatternFromAhap(TextAsset ahap)
		{
			if (ahap is null)
			{
				throw new ArgumentException("Cannot play a null AHAP.");
			}
			PlayPattern(new CHHapticPattern(ahap));
		}

		public void PlayPatternFromAhap(AHAPAsset ahap)
		{
			if (ahap is null)
			{
				throw new ArgumentException("Cannot play a null AHAP.");
			}

			PlayPattern(ahap.GetPattern());
		}
		#endregion

		#region Protected Utility

		private void ResetLastOperationException()
		{
			_lastOperationException = null;
		}

		private void CheckAndThrowLastOperationException()
		{
			// Throw exception from last native operation...
			if (!(_lastOperationException is null))
			{
				throw _lastOperationException;
			}
		}

		private static CHHapticEngine GetOneShotEngine()
		{
			return _oneShotEngine ?? (_oneShotEngine = new CHHapticEngine());
		}
		#endregion

		#region Static Callbacks

		[MonoPInvokeCallback(typeof(AllPlayersFinishedStatic))]
		private static void OnAllPlayersFinished(IntPtr pointer)
		{
			lock (_pointerToEnginesLock)
			{
				if (_pointerToEngines.ContainsKey(pointer) && !(_pointerToEngines[pointer] is null) && !(_pointerToEngines[pointer].PlayersFinished is null))
				{
					_pointerToEngines[pointer].PlayersFinished?.Invoke();
				}
			}
		}

		[MonoPInvokeCallback(typeof(GenericCallback<IntPtr, CHHapticEngineStoppedReason>))]
		private static void OnStoppedWithReason(IntPtr pointer, CHHapticEngineStoppedReason reason)
		{
			lock (_pointerToEnginesLock)
			{
				if (_pointerToEngines.ContainsKey(pointer) && !(_pointerToEngines[pointer] is null))
				{
					_pointerToEngines[pointer].Stopped?.Invoke(reason);
				}
			}
		}

		[MonoPInvokeCallback(typeof(SuccessWithPointerCallback))]
		private static void OnReset(IntPtr pointer)
		{
			lock (_pointerToEnginesLock)
			{
				if (_pointerToEngines.ContainsKey(pointer) && !(_pointerToEngines[pointer] is null))
				{
					_pointerToEngines[pointer].Reset?.Invoke();
				}
			}
		}

		[MonoPInvokeCallback(typeof(ErrorCallback))]
		private static void OnCreateError(CHError error)
		{
			_lastCreationException = new CHException(error);
		}

		[MonoPInvokeCallback(typeof(ErrorWithPointerCallback))]
		private static void OnThrowErrorWithPointer(IntPtr pointer, CHError error)
		{
			lock (_pointerToEnginesLock)
			{
				if (_pointerToEngines.ContainsKey(pointer) && !(_pointerToEngines[pointer] is null))
				{
					_pointerToEngines[pointer]._lastOperationException = new CHException(error);
				}
			}
		}
		#endregion
	}
}
