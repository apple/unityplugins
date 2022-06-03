using Apple.Core;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Apple.UIFBG.Tests
{
	public class UIFeedbackGeneratorTestSuite
	{
		private List<IFeedbackGenerator> _generators;
		
		[OneTimeSetUp]
		public void SetupScene()
		{
			_ = new AppleLogger();
		}

		[TearDown]
		public void TearDown()
		{
			if (!(_generators is null))
			{
				foreach (var g in _generators)
				{
					g.Destroy();
				}
			}
		}

		[UnityTest]
		public IEnumerator Test_UIFBG_CreateImpactWithAllTypes()
		{
			_generators = new List<IFeedbackGenerator> {
				new UIImpactFeedbackGenerator(UIImpactFeedbackGenerator.UIImpactType.Heavy),
				new UIImpactFeedbackGenerator(UIImpactFeedbackGenerator.UIImpactType.Medium),
				new UIImpactFeedbackGenerator(UIImpactFeedbackGenerator.UIImpactType.Light),
				new UIImpactFeedbackGenerator(UIImpactFeedbackGenerator.UIImpactType.Rigid),
				new UIImpactFeedbackGenerator(UIImpactFeedbackGenerator.UIImpactType.Soft)
			};
			yield return new WaitForFixedUpdate();
			foreach (var g in _generators)
			{
				UnityEngine.Assertions.Assert.IsNotNull(g);
			}
		}

		[UnityTest]
		public IEnumerator Test_UIFBG_CreateSelection()
		{
			var sel = new UISelectionFeedbackGenerator();
			yield return new WaitForFixedUpdate();
			UnityEngine.Assertions.Assert.IsNotNull(sel);
			sel.Destroy();
		}

		[UnityTest]
		public IEnumerator Test_UIFBG_CreateNotification()
		{
			var not = new UINotificationFeedbackGenerator();
			yield return new WaitForFixedUpdate();
			UnityEngine.Assertions.Assert.IsNotNull(not);
			not.Destroy();
		}

		[UnityTest]
		public IEnumerator Test_UIFBG_PrepareAllTypes()
		{
			_generators = new List<IFeedbackGenerator>
			{
				new UIImpactFeedbackGenerator(UIImpactFeedbackGenerator.UIImpactType.Heavy),
				new UISelectionFeedbackGenerator(),
				new UINotificationFeedbackGenerator()
			};
			yield return new WaitForFixedUpdate();
			foreach (var g in _generators)
			{
				g.Prepare();
			}
		}

		[UnityTest]
		public IEnumerator Test_UIFBG_TriggerAllTypes()
		{
			_generators = new List<IFeedbackGenerator>
			{
				new UIImpactFeedbackGenerator(UIImpactFeedbackGenerator.UIImpactType.Heavy),
				new UISelectionFeedbackGenerator(),
				new UINotificationFeedbackGenerator()
			};
			yield return new WaitForFixedUpdate();
			foreach (var g in _generators)
			{
				g.Trigger();
			}
		}

		[UnityTest]
		public IEnumerator Test_UIFBG_TriggerImpactWithModifiedIntensity()
		{
			var imp = new UIImpactFeedbackGenerator(UIImpactFeedbackGenerator.UIImpactType.Heavy);
			yield return new WaitForFixedUpdate();
			imp.Trigger(0.5f);
		}

		[UnityTest]
		public IEnumerator Test_UIFBG_TriggerNotificationWithEachStyle()
		{
			var not = new UINotificationFeedbackGenerator();
			yield return new WaitForFixedUpdate();
			not.Trigger(UINotificationFeedbackGenerator.UINotificationStyle.Error);
			yield return new WaitForFixedUpdate();
			not.Trigger(UINotificationFeedbackGenerator.UINotificationStyle.Success);
			yield return new WaitForFixedUpdate();
			not.Trigger(UINotificationFeedbackGenerator.UINotificationStyle.Warning);
		}
	}
}
