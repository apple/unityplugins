using Apple.Core;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Apple.CoreHaptics.Tests {
	public class CHHapticPlayerTestSuite {

		private CHHapticEngine _eng;
		private bool _notifyCalled;
		private CHHapticPatternPlayer _player;

		/*
		 * A sneaky way to confirm playback has stopped when we think it should
		 */
		private void TestNotificationHandler() {
			_notifyCalled = true;
		}

		[OneTimeSetUp]
		public void SetupScene() {
			_ = new AppleLogger();
			_eng = new CHHapticEngine {
				IsAutoShutdownEnabled = true
			};
			_eng.PlayersFinished += TestNotificationHandler;
		}

		[OneTimeTearDown]
		public void TearDownScene() {
			_eng.DestroyAllPlayers();
			_eng.Destroy();
		}

		[SetUp]
		public void Setup() {
			_player = _eng.MakePlayer(new List<CHHapticEvent> {
				new CHHapticTransientEvent {
					Time = 0f
				},
				new CHHapticContinuousEvent {
					EventDuration = 1f,
					Time = 0.5f
				}
			});
		}

		[TearDown]
		public void Teardown() {
			_eng.DestroyAllPlayers();
		}

		[UnityTest]
		public IEnumerator Test_CHHapticPlayer_PlayImmediate_Succeeds() {
			_player.Start();
			yield return new WaitForSeconds(1.6f);
		}

		[UnityTest]
		public IEnumerator Test_CHHapticPlayer_PlayFuture_Succeeds() {
			_player.Start(1f);
			yield return new WaitForSeconds(2.6f);
		}

		[UnityTest]
		public IEnumerator Test_CHHapticPlayer_StopImmediate_Succeeds() {
			_player.Start();
			_notifyCalled = false;
			_eng.NotifyWhenPlayersFinished();
			yield return new WaitForSeconds(0.5f);
			_player.Stop();
			yield return new WaitForSeconds(0.1f);
			UnityEngine.Assertions.Assert.IsTrue(_notifyCalled,
				"Expected the engine's notifyHandler to be called, but it wasn't.");
		}

		[UnityTest]
		public IEnumerator Test_CHHapticPlayer_StopFuture_Succeeds() {
			_player.Start();
			_notifyCalled = false;
			_eng.NotifyWhenPlayersFinished();
			_player.Stop(1f);
			yield return new WaitForSeconds(1.1f);
			UnityEngine.Assertions.Assert.IsTrue(_notifyCalled,
				"Expected the engine's notifyHandler to be called, but it wasn't.");
		}

		[UnityTest]
		public IEnumerator Test_CHHapticPlayer_SendParameters_Succeeds() {
			_player.Start();
			yield return new WaitForSeconds(0.1f);
			_player.SendParameters(new List<CHHapticParameter> {
				new CHHapticParameter(CHHapticDynamicParameterID.HapticIntensityControl, 1f),
				new CHHapticParameter(CHHapticDynamicParameterID.HapticSharpnessControl, 0f)
			});
			yield return new WaitForSeconds(1.5f);
		}

		[UnityTest]
		public IEnumerator Test_CHHapticPlayer_SendNullParameters_Fails() {
			_player.Start();
			yield return new WaitForSeconds(0.1f);
			Assert.That(() => _player.SendParameters(null),
				Throws.TypeOf<ArgumentException>(),
				"Sending null parameters to player didn't throw an Arg exception."
				);
			yield return new WaitForSeconds(0.1f);
		}

		[UnityTest]
		public IEnumerator Test_CHHapticPlayer_ScheduleParamCurve_Succeeds() {
			_player.Start();
			yield return new WaitForSeconds(0.1f);
			_player.ScheduleParameterCurve(new CHHapticParameterCurve {
				ParameterID = CHHapticDynamicParameterID.HapticIntensityControl,
				ParameterCurveControlPoints = new List<CHHapticParameterCurveControlPoint> {
					new CHHapticParameterCurveControlPoint(0f, 1f),
					new CHHapticParameterCurveControlPoint(0.5f, 0.5f),
					new CHHapticParameterCurveControlPoint(1f, 0f)
				}
			});
			yield return new WaitForSeconds(1.5f);
		}

		[UnityTest]
		public IEnumerator Test_CHHapticPlayer_ScheduleNullParamCurve_Fails() {
			_player.Start();
			yield return new WaitForSeconds(0.1f);
			Assert.That(() => _player.ScheduleParameterCurve(null),
				Throws.TypeOf<ArgumentException>(),
				"Sending null ParamCurve to player didn't throw an Arg exception."
				);
			yield return new WaitForSeconds(0.1f);
		}

		[UnityTest]
		public IEnumerator Test_CHHapticPlayer_DefaultsNotMuted() {
			yield return new WaitForFixedUpdate();

			UnityEngine.Assertions.Assert.IsFalse(_player.Muted,
				"Expected a brand new player to not be muted.");
		}

		[UnityTest]
		public IEnumerator Test_CHHapticPlayer_SetMuted() {
			UnityEngine.Assertions.Assert.IsFalse(_player.Muted,
				"Expected a brand new player to not be muted.");

			yield return new WaitForFixedUpdate();

			_player.Muted = true;

			UnityEngine.Assertions.Assert.IsTrue(_player.Muted,
				"Expected player to be muted after setter.");
		}

		[UnityTest]
		public IEnumerator Test_CHHapticPlayer_ID_NotNull() {
			yield return new WaitForFixedUpdate();
			UnityEngine.Assertions.Assert.AreNotEqual(_player.PlayerId, IntPtr.Zero,
				"Expected the player to have a real PlayerId, but got IntPtr.Zero.");
		}

		[UnityTest]
		public IEnumerator Test_CHHapticPlayer_Destroy() {
			yield return new WaitForFixedUpdate();
			_player.Destroy();

			UnityEngine.Assertions.Assert.AreEqual(_player.PlayerId, IntPtr.Zero,
				"Expected the player to have a null ID, but got a real value.");
		}
	}
}
