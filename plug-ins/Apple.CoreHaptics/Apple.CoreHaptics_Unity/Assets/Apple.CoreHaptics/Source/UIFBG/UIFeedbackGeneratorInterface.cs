using System;
using System.Runtime.InteropServices;
using static Apple.CoreHaptics.CHInteropUtility;

namespace Apple.UIFBG
{
	internal static class UIFeedbackGeneratorInterface
	{
		#region Create Impact Generator
		[DllImport(DllName, EntryPoint = "UIFeedbackGenerator_CreateImpactGenerator")]
		internal static extern IntPtr CreateImpactGenerator(int feedbackStyle);
		#endregion
		#region Trigger Impact Generator
		[DllImport(DllName, EntryPoint = "UIImpactFeedbackGenerator_Trigger")]
		internal static extern void TriggerImpactGenerator(IntPtr generatorPtr, float intensity);
		#endregion
		#region Destroy Impact Generator
		[DllImport(DllName, EntryPoint = "UIFeedbackGenerator_DestroyImpactGenerator")]
		internal static extern void DestroyImpactGenerator(IntPtr generatorPtr);
		#endregion
		
		#region Create Selection Generator
		[DllImport(DllName, EntryPoint = "UIFeedbackGenerator_CreateSelectionGenerator")]
		internal static extern IntPtr CreateSelectionGenerator();
		#endregion
		#region Trigger Selection Generator
		[DllImport(DllName, EntryPoint = "UISelectionFeedbackGenerator_Trigger")]
		internal static extern void TriggerSelectionGenerator(IntPtr generatorPtr);
		#endregion
		#region Destroy Selection Generator
		[DllImport(DllName, EntryPoint = "UIFeedbackGenerator_DestroySelectionGenerator")]
		internal static extern void DestroySelectionGenerator(IntPtr generatorPtr);
		#endregion
		
		#region Create Notification Generator
		[DllImport(DllName, EntryPoint = "UIFeedbackGenerator_CreateNotificationGenerator")]
		internal static extern IntPtr CreateNotificationGenerator();
		#endregion
		#region Trigger Notification Generator
		[DllImport(DllName, EntryPoint = "UINotificationFeedbackGenerator_Trigger")]
		internal static extern void TriggerNotificationGenerator(IntPtr generatorPtr, int feedbackType);
		#endregion
		#region Destroy Notification Generator
		[DllImport(DllName, EntryPoint = "UIFeedbackGenerator_DestroyNotificationGenerator")]
		internal static extern void DestroyNotificationGenerator(IntPtr generatorPtr);
		#endregion
		
		#region Prepare Generator
		[DllImport(DllName, EntryPoint = "UIFeedbackGenerator_Prepare")]
		internal static extern void PrepareGenerator(IntPtr generatorPtr);
		#endregion
	}
}
