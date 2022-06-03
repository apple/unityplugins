using System;

namespace Apple.UIFBG
{
	public class UISelectionFeedbackGenerator : IFeedbackGenerator
	{
		private IntPtr _generatorPtr;
		
		public UISelectionFeedbackGenerator()
		{
			_generatorPtr = UIFeedbackGeneratorInterface.CreateSelectionGenerator();
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
			if (_generatorPtr != IntPtr.Zero)
			{
				UIFeedbackGeneratorInterface.TriggerSelectionGenerator(_generatorPtr);
			}
		}

		public void Destroy()
		{
			if (_generatorPtr != IntPtr.Zero)
			{
				UIFeedbackGeneratorInterface.DestroySelectionGenerator(_generatorPtr);
				_generatorPtr = IntPtr.Zero;
			}
		}
	}
}
