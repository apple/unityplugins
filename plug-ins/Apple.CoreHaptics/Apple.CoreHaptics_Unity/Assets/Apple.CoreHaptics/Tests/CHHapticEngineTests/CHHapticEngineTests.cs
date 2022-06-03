using Apple.Core;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Apple.CoreHaptics.Tests
{
	public class CHHapticEngineTestSuite
	{

		private CHHapticEngine _eng;

		private bool _notifyCalled;

		[OneTimeSetUp]
		public void SetupScene()
		{
			_ = new AppleLogger();
		}

		[OneTimeTearDown]
		public void TearDownScene()
		{

		}

		[TearDown]
		public void Teardown()
		{
			_eng.DestroyAllPlayers();
			_eng.Destroy();
		}

		[UnityTest]
		public IEnumerator Test_Engine_Creation()
		{
			_eng = new CHHapticEngine();

			yield return new WaitForSeconds(0.1f);

			UnityEngine.Assertions.Assert.IsNotNull(_eng, "Failed to create the CHHapticEngine.");
			Assert.IsTrue(_eng.EnginePtr != IntPtr.Zero, "Creating engine resulted in null object pointer.");
		}

		[UnityTest]
		public IEnumerator Test_Engine_Default_Booleans()
		{
			_eng = new CHHapticEngine();

			yield return new WaitForSeconds(0.1f);

			Assert.AreEqual(false, _eng.IsMutedForAudio, "Expected the engine to not be muted for audio, but it is.");
			Assert.AreEqual(false, _eng.IsMutedForHaptics, "Expected the engine to not be muted for haptics, but it is.");
			Assert.AreEqual(false, _eng.PlaysHapticsOnly, "Expected the engine to not play haptics only, but it's in haptics only mode.");
			Assert.AreEqual(false, _eng.IsAutoShutdownEnabled, "Expected the engine to not be in auto shutdown mode, but it is.");
		}

		[UnityTest]
		public IEnumerator Test_Engine_SupportsHaptics()
		{
			Assert.IsTrue(CHHapticEngine.HardwareSupportsHaptics(), "Expected device capabilities to support haptics, but got false.");
			yield return null;
		}

		[UnityTest]
		public IEnumerator Test_Engine_StartStop_DoesNotThrow()
		{
			_eng = new CHHapticEngine();

			Assert.DoesNotThrow(() => _eng.Start(), "Engine threw an error when starting.");
			yield return new WaitForSeconds(0.1f);

			Assert.DoesNotThrow(() => _eng.Stop(), "Engine threw an error when stopping.");
		}

		[UnityTest]
		public IEnumerator Test_Engine_MakesPlayer_WithEvent()
		{
			_eng = new CHHapticEngine();
			var e = new CHHapticTransientEvent();
			var p = _eng.MakePlayer(new List<CHHapticEvent> { e });

			yield return new WaitForSeconds(0.1f);

			UnityEngine.Assertions.Assert.IsNotNull(p, "Failed to create a player with an event.");
		}

		[UnityTest]
		public IEnumerator Test_Engine_MakesPlayer_WithProgrammaticPattern()
		{
			_eng = new CHHapticEngine();
			var e = new CHHapticTransientEvent();
			var ahap = new CHHapticPattern(new List<CHHapticEvent> { e });
			var p = _eng.MakePlayer(ahap);

			yield return new WaitForSeconds(0.1f);

			UnityEngine.Assertions.Assert.IsNotNull(p, "Failed to create a player with a CHHapticPattern.");
		}

		[UnityTest]
		public IEnumerator Test_Engine_MakesPlayer_WithTextAsset()
		{
			_eng = new CHHapticEngine();
			var ahapText = Resources.Load<TextAsset>("Boing");
			UnityEngine.Assertions.Assert.IsNotNull(ahapText, "Failed to load Boing from resources.");
			var ahap = new CHHapticPattern(ahapText);
			UnityEngine.Assertions.Assert.IsNotNull(ahap, "Failed to create a CHHapticPattern from the text asset.");
			var p = _eng.MakePlayer(ahap);

			yield return new WaitForSeconds(0.1f);

			UnityEngine.Assertions.Assert.IsNotNull(p, "Failed to create a player with a CHHapticPattern built from a TextAsset.");
		}

		[UnityTest]
		public IEnumerator Test_Engine_MakesAdvancedPlayer_WithEvent()
		{
			_eng = new CHHapticEngine();
			var e = new CHHapticTransientEvent();
			CHHapticPatternPlayer p = _eng.MakeAdvancedPlayer(new List<CHHapticEvent> { e });

			yield return new WaitForSeconds(0.1f);

			UnityEngine.Assertions.Assert.IsNotNull(p, "Failed to create an advanced player with an event.");
		}

		[UnityTest]
		public IEnumerator Test_Engine_MakesAdvancedPlayer_WithCHHapticPattern()
		{
			_eng = new CHHapticEngine();
			var e = new CHHapticTransientEvent();
			var ahap = new CHHapticPattern(new List<CHHapticEvent> { e });
			CHHapticPatternPlayer p = _eng.MakeAdvancedPlayer(ahap);

			yield return new WaitForSeconds(0.1f);

			UnityEngine.Assertions.Assert.IsNotNull(p, "Failed to create an advanced player with a CHHapticPattern.");
		}

		[UnityTest]
		public IEnumerator Test_Engine_MakesAdvancedPlayer_WithTextAsset()
		{
			_eng = new CHHapticEngine();
			var ahapResource = Resources.Load<TextAsset>("Boing");
			UnityEngine.Assertions.Assert.IsNotNull(ahapResource, "Failed to load Boing from resources.");
			var ahap = new CHHapticPattern(ahapResource);
			UnityEngine.Assertions.Assert.IsNotNull(ahap, "Failed to create a CHHapticPattern from the TextAsset.");
			var p = _eng.MakePlayer(ahap);

			yield return new WaitForSeconds(0.1f);

			UnityEngine.Assertions.Assert.IsNotNull(p, "Failed to create an advanced player with a CHHapticPattern built from a TextAsset.");
		}

		[UnityTest]
		public IEnumerator Test_Engine_MakesAdvancedPlayer_WithTextAssetContents()
		{
			_eng = new CHHapticEngine();
			var ahapResource = Resources.Load<TextAsset>("Boing");
			UnityEngine.Assertions.Assert.IsNotNull(ahapResource, "Failed to load Boing from resources.");
			var ahap = new CHHapticPattern(ahapResource.text);
			UnityEngine.Assertions.Assert.IsNotNull(ahap, "Failed to create a CHHapticPattern from the text asset's contents.");
			var p = _eng.MakePlayer(ahap);

			yield return new WaitForSeconds(0.1f);

			UnityEngine.Assertions.Assert.IsNotNull(p, "Failed to create an advanced player with a CHHapticPattern built from a TextAsset's contents.");
		}

		private void TestNotificationHandler()
		{
			_notifyCalled = true;
		}

		[UnityTest]
		public IEnumerator Test_Engine_NotifyHandler_CalledImmediatelyWithNoPlayersPlaying()
		{
			_eng = new CHHapticEngine();
			_eng.Start();

			yield return new WaitForSeconds(0.1f);
			_eng.PlayersFinished += TestNotificationHandler;
			_notifyCalled = false;
			_eng.NotifyWhenPlayersFinished(false);
			yield return new WaitForSeconds(0.1f);
			UnityEngine.Assertions.Assert.IsTrue(_notifyCalled, "Notification handler wasn't called.");
		}

		[UnityTest]
		public IEnumerator Test_Engine_NotifyHandler_CalledWhenPlayerFinishesPlaying()
		{
			_eng = new CHHapticEngine();
			var e = new CHHapticContinuousEvent
			{
				EventDuration = 1f,
				Time = 0f
			};
			var p = _eng.MakePlayer(new List<CHHapticEvent> { e });
			_eng.Start();

			yield return new WaitForSeconds(0.1f);

			p.Start();

			yield return new WaitForSeconds(0.2f);
			_eng.PlayersFinished += TestNotificationHandler;
			_notifyCalled = false;
			_eng.NotifyWhenPlayersFinished(false);

			yield return new WaitForSeconds(1f);

			UnityEngine.Assertions.Assert.IsTrue(_notifyCalled, "Notification handler wasn't called.");
		}

		[UnityTest]
		public IEnumerator Test_Engine_NotifyHandler_FalseLeavesEngineRunning()
		{
			_eng = new CHHapticEngine();
			var e = new CHHapticContinuousEvent
			{
				EventDuration = 1f,
				Time = 0f
			};
			var p = _eng.MakePlayer(new List<CHHapticEvent> { e });
			_eng.Start();

			yield return new WaitForSeconds(0.2f);
			_eng.PlayersFinished += TestNotificationHandler;
			_notifyCalled = false;
			_eng.NotifyWhenPlayersFinished();

			yield return new WaitForSeconds(1f);

			try
			{
				p.Start();
			}
			catch
			{
				Assert.Fail("Playing a player after NotifyWhenPlayersFinished(true) should have succeeded.");
			}
		}

		[UnityTest]
		public IEnumerator Test_Engine_NotifyHandler_TrueShutsDownEngine()
		{
			_eng = new CHHapticEngine();
			var e = new CHHapticContinuousEvent
			{
				EventDuration = 1f,
				Time = 0f
			};
			var p = _eng.MakePlayer(new List<CHHapticEvent> { e });
			_eng.Start();

			yield return new WaitForSeconds(0.2f);
			_eng.PlayersFinished += TestNotificationHandler;
			_notifyCalled = false;
			_eng.NotifyWhenPlayersFinished(false);

			yield return new WaitForSeconds(1f);

			Assert.That(() => p.Start(), Throws.TypeOf<CHException>(),
				"Playing a player after NotifyWhenPlayersFinished(false) should have failed, but did not.");
		}
	}
}
