using System.Runtime.InteropServices;
using static Apple.CoreHaptics.CHInteropUtility;

namespace Apple.CoreHaptics
{
	internal static class CHHapticCapabilitiesInterface
	{
		[DllImport(DllName, EntryPoint = "CoreHaptics_Capabilities_MinValueForEventParameter")]
		internal static extern float MinValueForEventParameter(int integerParameterId,
			int integerEventType);
		
		[DllImport(DllName, EntryPoint = "CoreHaptics_Capabilities_DefaultValueForEventParameter")]
		internal static extern float DefaultValueForEventParameter(int integerParameterId,
			int integerEventType);
		
		[DllImport(DllName, EntryPoint = "CoreHaptics_Capabilities_MaxValueForEventParameter")]
		internal static extern float MaxValueForEventParameter(int integerParameterId,
			int integerEventType);
		
		[DllImport(DllName, EntryPoint = "CoreHaptics_Capabilities_MinValueForDynamicParameter")]
		internal static extern float MinValueForDynamicParameter(int integerParameterId);
		
		[DllImport(DllName, EntryPoint = "CoreHaptics_Capabilities_DefaultValueForDynamicParameter")]
		internal static extern float DefaultValueForDynamicParameter(int integerParameterId);
		
		[DllImport(DllName, EntryPoint = "CoreHaptics_Capabilities_MaxValueForDynamicParameter")]
		internal static extern float MaxValueForDynamicParameter(int integerParameterId);
	}
}
