using UnityEngine;

namespace Apple.CoreHaptics
{
	// Unity 6000+ deprecates Rigidbody2D.velocity in favor of linearVelocity, which
	// doesn't exist on the 2022.3 floor. Branch on the Unity version to support both.
	internal static class Rigidbody2DCompat
	{
		public static Vector2 GetVelocity(this Rigidbody2D rigidbody)
		{
#if UNITY_6000_0_OR_NEWER
			return rigidbody.linearVelocity;
#else
			return rigidbody.velocity;
#endif
		}

		public static void SetVelocity(this Rigidbody2D rigidbody, Vector2 value)
		{
#if UNITY_6000_0_OR_NEWER
			rigidbody.linearVelocity = value;
#else
			rigidbody.velocity = value;
#endif
		}
	}
}
