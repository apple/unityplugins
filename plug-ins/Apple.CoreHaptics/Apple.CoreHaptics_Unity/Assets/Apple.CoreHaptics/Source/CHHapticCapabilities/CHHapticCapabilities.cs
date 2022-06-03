using System;
using UnityEngine;

namespace Apple.CoreHaptics
{
	public static class CHHapticCapabilities
	{
		public static float MinValueForEventParameter(CHHapticEventParameterID parameterId, CHHapticEventType eventType)
		{
			try
			{
				return CHHapticCapabilitiesInterface.MinValueForEventParameter((int)parameterId, (int)eventType);
			}
			catch (DllNotFoundException)
			{
				return -1f;
			}

		}

		public static float MinValueForEventParameter(int parameterId, int eventType)
		{
			try
			{
				return CHHapticCapabilitiesInterface.MinValueForEventParameter(parameterId, eventType);
			}
			catch (DllNotFoundException)
			{
				return -1f;
			}
		}

		public static float DefaultValueForEventParameter(CHHapticEventParameterID parameterId,
			CHHapticEventType eventType)
		{
			try
			{
				return CHHapticCapabilitiesInterface.DefaultValueForEventParameter((int)parameterId, (int)eventType);
			}
			catch (DllNotFoundException)
			{
				return 0f;
			}

		}

		public static float DefaultValueForEventParameter(int parameterId,
			int eventType)
		{
			try
			{
				return CHHapticCapabilitiesInterface.DefaultValueForEventParameter(parameterId, eventType);
			}
			catch (DllNotFoundException)
			{
				return 0f;
			}
		}

		public static float MaxValueForEventParameter(CHHapticEventParameterID parameterId,
			CHHapticEventType eventType)
		{
			try
			{
				return CHHapticCapabilitiesInterface.MaxValueForEventParameter((int)parameterId, (int)eventType);
			}
			catch (DllNotFoundException)
			{
				return 1f;
			}
		}

		public static float MaxValueForEventParameter(int parameterId,
			int eventType)
		{
			try
			{
				return CHHapticCapabilitiesInterface.MaxValueForEventParameter(parameterId, eventType);
			}
			catch (DllNotFoundException)
			{
				return 1f;
			}
		}

		public static float MinValueForDynamicParameter(CHHapticDynamicParameterID parameterId)
		{
			try
			{
				return CHHapticCapabilitiesInterface.MinValueForDynamicParameter((int)parameterId);
			}
			catch (DllNotFoundException)
			{
				return -2f;
			}
		}

		public static float MinValueForDynamicParameter(int parameterId)
		{
			try
			{
				return CHHapticCapabilitiesInterface.MinValueForDynamicParameter(parameterId);
			}
			catch (DllNotFoundException)
			{
				return -2f;
			}
		}

		public static float DefaultValueForDynamicParameter(CHHapticDynamicParameterID parameterId)
		{
			try
			{
				return CHHapticCapabilitiesInterface.DefaultValueForDynamicParameter((int)parameterId);
			}
			catch (DllNotFoundException)
			{
				return 0f;
			}
		}

		public static float DefaultValueForDynamicParameter(int parameterId)
		{
			try
			{
				return CHHapticCapabilitiesInterface.DefaultValueForDynamicParameter(parameterId);
			}
			catch (DllNotFoundException)
			{
				return 0f;
			}
		}

		public static float MaxValueForDynamicParameter(CHHapticDynamicParameterID parameterId)
		{
			try
			{
				return CHHapticCapabilitiesInterface.MaxValueForDynamicParameter((int)parameterId);
			}
			catch (DllNotFoundException)
			{
				return 2f;
			}
		}

		public static float MaxValueForDynamicParameter(int parameterId)
		{
			try
			{
				return CHHapticCapabilitiesInterface.MaxValueForDynamicParameter(parameterId);
			}
			catch (DllNotFoundException)
			{
				return 2f;
			}
		}
	}
}
