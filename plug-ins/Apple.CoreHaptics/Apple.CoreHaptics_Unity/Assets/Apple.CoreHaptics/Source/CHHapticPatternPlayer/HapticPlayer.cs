using UnityEngine;

namespace Apple.CoreHaptics
{
	[AddComponentMenu("Apple/CoreHaptics/Haptic Player")]
	public class CHHapticPlayer : MonoBehaviour
	{
		public AHAPAsset pattern;

		public virtual void Play()
		{
			if (CHHapticEngine.HardwareSupportsHaptics() && !(pattern is null))
				CHHapticEngine.OneShotEngine.PlayPattern(pattern.GetPattern());
		}

		protected virtual void OnDestroy()
		{
			if (CHHapticEngine.HardwareSupportsHaptics()
			   && CHHapticEngine.OneShotEngine.TryGetPlayerForPattern(pattern.GetPattern(), out var player))
				CHHapticEngine.OneShotEngine.DestroyPlayer(player);
		}
	}
}
