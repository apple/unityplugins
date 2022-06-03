using System.Collections.Generic;
using UnityEngine;

namespace Apple.CoreHaptics
{
	[CreateAssetMenu(menuName = "Apple/CoreHaptics/AHAP")]
	public class AHAPAsset : ScriptableObject
	{
		public List<CHHapticEvent> Events = new List<CHHapticEvent>();
		public List<CHHapticParameter> Parameters = new List<CHHapticParameter>();
		public List<CHHapticParameterCurve> ParameterCurves = new List<CHHapticParameterCurve>();

		public AHAPAsset()
		{
			Events.Add(new CHHapticTransientEvent());
		}

		public CHHapticPattern GetPattern()
		{
			var pattern = new CHHapticPattern();

			// Events...
			foreach (var e in Events)
			{
				pattern.Pattern.Add(new CHHapticPatternComponent(e));
			}

			// Parameters...
			foreach (var p in Parameters)
			{
				pattern.Pattern.Add(new CHHapticPatternComponent(p));
			}

			// Curves...
			foreach (var c in ParameterCurves)
			{
				pattern.Pattern.Add(new CHHapticPatternComponent(c));
			}

			return pattern;
		}
	}
}
