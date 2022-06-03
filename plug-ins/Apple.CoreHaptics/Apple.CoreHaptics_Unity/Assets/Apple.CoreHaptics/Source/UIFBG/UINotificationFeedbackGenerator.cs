using System;
// ReSharper disable UnusedMember.Global

namespace Apple.UIFBG
{
	public class UINotificationFeedbackGenerator : IFeedbackGenerator
	{
		private IntPtr _generatorPtr;
		
		public UINotificationFeedbackGenerator()
		{
			_generatorPtr = UIFeedbackGeneratorInterface.CreateNotificationGenerator();
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
			Trigger(UINotificationStyle.Success);
		}

		public void Trigger(UINotificationStyle notificationStyle)
		{
			if (_generatorPtr != IntPtr.Zero)
			{
				UIFeedbackGeneratorInterface.TriggerNotificationGenerator(_generatorPtr, (int)notificationStyle);
			}
		}

		public void Destroy()
		{
			if (_generatorPtr != IntPtr.Zero)
			{
				UIFeedbackGeneratorInterface.DestroyNotificationGenerator(_generatorPtr);
				_generatorPtr = IntPtr.Zero;
			}
		}
		
		public enum UINotificationStyle
		{
			Error = 0,
			Success = 1,
			Warning = 2
		}
	}
}
