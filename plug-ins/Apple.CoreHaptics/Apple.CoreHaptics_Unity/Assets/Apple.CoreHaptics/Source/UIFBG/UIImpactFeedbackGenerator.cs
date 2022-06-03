using System;
// ReSharper disable UnusedMember.Global

namespace Apple.UIFBG
{
	public class UIImpactFeedbackGenerator : IFeedbackGenerator
	{
		private IntPtr _generatorPtr;
		
		public UIImpactFeedbackGenerator(UIImpactType impactType)
		{
			_generatorPtr = UIFeedbackGeneratorInterface.CreateImpactGenerator((int)impactType);
		}

		public void Prepare()
		{
			if (_generatorPtr != IntPtr.Zero)
			{
				UIFeedbackGeneratorInterface.PrepareGenerator(_generatorPtr);
			}
		}

		public void Trigger()
		{
			Trigger(1f);
		}

		public void Trigger(float intensity=1f)
		{
			if (_generatorPtr != IntPtr.Zero)
			{
				UIFeedbackGeneratorInterface.TriggerImpactGenerator(_generatorPtr, intensity);
			}
		}

		public void Destroy()
		{
			if (_generatorPtr != IntPtr.Zero)
			{
				UIFeedbackGeneratorInterface.DestroyImpactGenerator(_generatorPtr);
				_generatorPtr = IntPtr.Zero;
			}
		}

		public enum UIImpactType
		{
			Heavy = 0,
			Light = 1,
			Medium = 2,
			Rigid = 3,
			Soft = 4
		}
	}
}
