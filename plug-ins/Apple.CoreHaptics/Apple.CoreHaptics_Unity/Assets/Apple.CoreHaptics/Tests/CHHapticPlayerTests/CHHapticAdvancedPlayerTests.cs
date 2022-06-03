using Apple.Core;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Apple.CoreHaptics.Tests
{
	public class CHHapticAdvancedPlayerTestSuite
	{
		private CHHapticEngine _eng;
		private bool _notifyCalled;

		private CHHapticAdvancedPatternPlayer _player;
		private bool _completionHandlerCalled;

		/*
		 * A sneaky way to confirm playback has stopped when we think it should
		 */
		private void TestNotificationHandler()
		{
			_notifyCalled = true;
		}

		/*
		 * A sneaky way to confirm the player completes its playback.
		 * Also used for testing the completion handler functionality
		 */
		private void TestCompletionHandler()
		{
			_completionHandlerCalled = true;
		}

		[OneTimeSetUp]
		public void SetupScene()
		{
			_ = new AppleLogger();
			_eng = new CHHapticEngine
			{
				IsAutoShutdownEnabled = true
			};
			_eng.PlayersFinished += TestNotificationHandler;
		}

		[OneTimeTearDown]
		public void TearDownScene()
		{
			_eng.DestroyAllPlayers();
			_eng.Destroy();
		}

		[SetUp]
		public void Setup()
		{
			_completionHandlerCalled = false;
			_notifyCalled = false;
			_player = _eng.MakeAdvancedPlayer(new List<CHHapticEvent> {
				new CHHapticTransientEvent {
					Time = 0f
				},
				new CHHapticContinuousEvent {
					EventDuration = 1f,
					Time = 0.5f
				}
			});

			_player.CompletionHandler += TestCompletionHandler;
		}

		[TearDown]
		public void Teardown()
		{
			_eng.DestroyAllPlayers();
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_PlayImmediate_Succeeds()
		{
			_player.Start();
			yield return new WaitForSeconds(2f);
			UnityEngine.Assertions.Assert.IsTrue(_completionHandlerCalled,
				"Expected the player to have completed playback by now...");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_PlayFuture_Succeeds()
		{
			_player.Start(1f);
			yield return new WaitForSeconds(2.6f);
			UnityEngine.Assertions.Assert.IsTrue(_completionHandlerCalled,
				"Expected the player to have completed playback by now...");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_StopImmediate_Succeeds()
		{
			_player.Start();
			_notifyCalled = false;
			_eng.NotifyWhenPlayersFinished();
			yield return new WaitForSeconds(0.5f);
			_player.Stop();
			yield return new WaitForSeconds(0.1f);
			UnityEngine.Assertions.Assert.IsTrue(_completionHandlerCalled,
				"Expected the player to have completed playback by now...");
			UnityEngine.Assertions.Assert.IsTrue(_notifyCalled,
				"Expected the engine's notifyHandler to be called, but it wasn't.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_StopFuture_Succeeds()
		{
			_player.Start();
			_notifyCalled = false;
			_eng.NotifyWhenPlayersFinished();
			_player.Stop(1f);
			yield return new WaitForSeconds(1.1f);
			UnityEngine.Assertions.Assert.IsTrue(_completionHandlerCalled,
				"Expected the player to have completed playback by now...");
			UnityEngine.Assertions.Assert.IsTrue(_notifyCalled,
				"Expected the engine's notifyHandler to be called, but it wasn't.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_PauseImmediate_Succeeds()
		{
			_notifyCalled = false;
			_completionHandlerCalled = false;

			_player.Start();
			yield return new WaitForSeconds(0.2f);
			_player.Pause();
			_eng.NotifyWhenPlayersFinished();
			yield return new WaitForSeconds(1f);

			UnityEngine.Assertions.Assert.IsFalse(_completionHandlerCalled,
				"Expected the player to not complete playback but seems like it did...");
			UnityEngine.Assertions.Assert.IsFalse(_notifyCalled,
				"Expected the engine's notifyHandler to not be called, but it was.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_PauseFuture_Succeeds()
		{
			_notifyCalled = false;

			_player.Start();
			yield return new WaitForSeconds(0.2f);
			_player.Pause(1f);
			yield return new WaitForSeconds(3f);

			UnityEngine.Assertions.Assert.IsFalse(_completionHandlerCalled,
				"Expected the player to not complete playback but seems like it did...");
			UnityEngine.Assertions.Assert.IsFalse(_notifyCalled,
				"Expected the engine's notifyHandler to not be called, but it was.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_ResumeImmediate_Succeeds()
		{
			_notifyCalled = false;

			_player.Start();
			yield return new WaitForSeconds(0.2f);
			_player.Pause();
			yield return new WaitForSeconds(2f);

			UnityEngine.Assertions.Assert.IsFalse(_completionHandlerCalled,
				"Expected the player to not complete playback but seems like it did...");
			UnityEngine.Assertions.Assert.IsFalse(_notifyCalled,
				"Expected the engine's notifyHandler to not be called, but it was.");

			_player.Resume();
			_eng.NotifyWhenPlayersFinished();
			yield return new WaitForSeconds(1.5f);
			UnityEngine.Assertions.Assert.IsTrue(_completionHandlerCalled,
				"Expected the player to have completed playback by now...");
			UnityEngine.Assertions.Assert.IsTrue(_notifyCalled,
				"Expected the engine's notifyHandler to be called, but it wasn't.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_ResumeFuture_Succeeds()
		{
			_notifyCalled = false;

			_player.Start();
			yield return new WaitForSeconds(0.2f);
			_player.Pause();
			yield return new WaitForSeconds(2f);

			UnityEngine.Assertions.Assert.IsFalse(_completionHandlerCalled,
				"Expected the player to not complete playback but seems like it did...");
			UnityEngine.Assertions.Assert.IsFalse(_notifyCalled,
				"Expected the engine's notifyHandler to not be called, but it was.");

			_player.Resume(1f);
			yield return new WaitForSeconds(1.2f);
			_eng.NotifyWhenPlayersFinished();
			yield return new WaitForSeconds(2.5f);
			UnityEngine.Assertions.Assert.IsTrue(_completionHandlerCalled,
				"Expected the player to have completed playback by now...");
			UnityEngine.Assertions.Assert.IsTrue(_notifyCalled,
				"Expected the engine's notifyHandler to be called, but it wasn't.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_Seek_Succeeds()
		{
			_notifyCalled = false;

			_player.Seek(0.4f);
			_player.Start();
			_eng.NotifyWhenPlayersFinished();
			yield return new WaitForSeconds(2f);
			UnityEngine.Assertions.Assert.IsTrue(_completionHandlerCalled,
				"Expected the player to have completed playback by now...");
			UnityEngine.Assertions.Assert.IsTrue(_notifyCalled,
				"Expected the engine's notifyHandler to be called, but it wasn't.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_SendParameters_Succeeds()
		{
			_player.Start();
			yield return new WaitForSeconds(0.1f);
			_player.SendParameters(new List<CHHapticParameter> {
				new CHHapticParameter(CHHapticDynamicParameterID.HapticIntensityControl, 1f),
				new CHHapticParameter(CHHapticDynamicParameterID.HapticSharpnessControl, 0f)
			});
			yield return new WaitForSeconds(1.5f);
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_SendNullParameters_Fails()
		{
			_player.Start();
			yield return new WaitForSeconds(0.1f);
			Assert.That(() => _player.SendParameters(null),
				Throws.TypeOf<ArgumentNullException>(),
				"Sending null parameters to player didn't throw an Arg exception."
				);
			yield return new WaitForSeconds(0.1f);
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_ScheduleParamCurve_ThrowsNotSupported()
		{
			_player.Start();
			yield return new WaitForSeconds(0.1f);
			Assert.That(() => _player.ScheduleParameterCurve(new CHHapticParameterCurve
			{
				ParameterID = CHHapticDynamicParameterID.HapticIntensityControl,
				ParameterCurveControlPoints = new List<CHHapticParameterCurveControlPoint> {
					new CHHapticParameterCurveControlPoint(0f, 1f),
					new CHHapticParameterCurveControlPoint(0.5f, 0.5f),
					new CHHapticParameterCurveControlPoint(1f, 0f)
				}
			}),
				Throws.TypeOf<NotSupportedException>(),
				"Scheduling ParamCurve to advanced player didn't throw a NotSupported exception."
				);
			yield return new WaitForSeconds(0.1f);
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_DefaultsNotMuted()
		{
			yield return new WaitForFixedUpdate();

			UnityEngine.Assertions.Assert.IsFalse(_player.Muted,
				"Expected a brand new player to not be muted.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_SetMuted()
		{
			UnityEngine.Assertions.Assert.IsFalse(_player.Muted,
				"Expected a brand new player to not be muted.");

			yield return new WaitForFixedUpdate();

			_player.Muted = true;

			UnityEngine.Assertions.Assert.IsTrue(_player.Muted,
				"Expected player to be muted after setter.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_PlaybackRate_DefaultIsOne()
		{
			yield return new WaitForFixedUpdate();
			var rate = _player.PlaybackRate;
			UnityEngine.Assertions.Assert.AreEqual(1f, rate,
				$"Expected playback rate to be 1, but got {rate}");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_PlaybackRate_CanSet()
		{
			const float testValue = 2f;
			_player.PlaybackRate = testValue;
			yield return new WaitForFixedUpdate();

			var rate = _player.PlaybackRate;
			UnityEngine.Assertions.Assert.AreEqual(testValue, rate,
				$"Expected playback rate to be {testValue}, but got {rate}");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_PlaybackRate_Double_ShortensPlayback()
		{
			const float testValue = 2f;
			_player.PlaybackRate = testValue;
			yield return new WaitForFixedUpdate();

			_player.Start();
			_eng.NotifyWhenPlayersFinished();
			yield return new WaitForSeconds(2f);
			UnityEngine.Assertions.Assert.IsTrue(_completionHandlerCalled,
				"Expected the player to have completed playback by now...");
			UnityEngine.Assertions.Assert.IsTrue(_notifyCalled,
				"Expected the engine's notifyHandler to be called, but it wasn't.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_PlaybackRate_Half_LengthensPlayback()
		{
			const float testValue = 0.5f;
			_notifyCalled = false;
			_completionHandlerCalled = false;
			_player.PlaybackRate = testValue;
			yield return new WaitForFixedUpdate();

			_player.Start();
			_eng.NotifyWhenPlayersFinished();
			yield return new WaitForSeconds(0.5f);
			UnityEngine.Assertions.Assert.IsFalse(_completionHandlerCalled,
				"Expected the player to not complete playback yet but seems like it did...");
			UnityEngine.Assertions.Assert.IsFalse(_notifyCalled,
				"Expected the engine's notifyHandler to not be called yet, but it was.");

			yield return new WaitForSeconds(4f);
			UnityEngine.Assertions.Assert.IsTrue(_completionHandlerCalled,
				"Expected the player to have completed playback by now...");
			UnityEngine.Assertions.Assert.IsTrue(_notifyCalled,
				"Expected the engine's notifyHandler to be called, but it wasn't.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_Looping_DefaultIsFalse()
		{
			yield return new WaitForFixedUpdate();
			UnityEngine.Assertions.Assert.IsFalse(_player.LoopEnabled,
				"Expected LoopEnabled to default to false but got true.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_Looping_CanSet()
		{
			UnityEngine.Assertions.Assert.IsFalse(_player.LoopEnabled,
				"Expected advanced player LoopEnabled to start false, but got true.");
			yield return new WaitForFixedUpdate();
			_player.LoopEnabled = !_player.LoopEnabled;
			UnityEngine.Assertions.Assert.IsTrue(_player.LoopEnabled,
				"Failed to switch player looping to true.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_LoopEnd_DefaultIsPatternLength()
		{
			yield return new WaitForFixedUpdate();
			var endVal = _player.LoopEnd;
			UnityEngine.Assertions.Assert.AreEqual(1.5f, endVal,
				$"Expected LoopEnd to default to pattern duration (1.5) but got {endVal}.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_LoopEnd_CanSet()
		{
			const float testValue = 1.5f;
			_player.LoopEnd = testValue;
			yield return new WaitForFixedUpdate();
			var endVal = _player.LoopEnd;
			UnityEngine.Assertions.Assert.AreEqual(testValue, endVal,
				$"Expected LoopEnd to be {testValue} but got {endVal}.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_Looping_KeepsPlaying()
		{
			_player.LoopEnabled = true;
			_player.LoopEnd = 1;
			yield return new WaitForFixedUpdate();
			_player.Start();
			yield return new WaitForSeconds(3f);

			UnityEngine.Assertions.Assert.IsFalse(_completionHandlerCalled,
				"Expected the player to not complete playback yet but seems like it did...");
			UnityEngine.Assertions.Assert.IsFalse(_notifyCalled,
				"Expected the engine's notifyHandler to not be called yet, but it was.");

			_eng.NotifyWhenPlayersFinished();
			_player.LoopEnabled = false;
			yield return new WaitForSeconds(1.5f);
			UnityEngine.Assertions.Assert.IsTrue(_completionHandlerCalled,
				"Expected the player to have completed playback by now...");
			UnityEngine.Assertions.Assert.IsTrue(_notifyCalled,
				"Expected the engine's notifyHandler to be called, but it wasn't.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_CompletionHandler_IsCalled()
		{
			_player.Start();
			yield return new WaitForSeconds(2f);

			UnityEngine.Assertions.Assert.IsTrue(_completionHandlerCalled,
				"Expected the player CompletionHandler to be called, but it wasn't.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_ID_NotNull()
		{
			yield return new WaitForFixedUpdate();
			UnityEngine.Assertions.Assert.AreNotEqual(_player.PlayerId, IntPtr.Zero,
				"Expected the player to have a real PlayerId, but got IntPtr.Zero.");
		}

		[UnityTest]
		public IEnumerator Test_AdvancedPlayer_Destroy()
		{
			yield return new WaitForFixedUpdate();
			_player.Destroy();

			UnityEngine.Assertions.Assert.AreEqual(_player.PlayerId, IntPtr.Zero,
				"Expected the player to have a null ID, but got a real value.");
		}
	}
}
