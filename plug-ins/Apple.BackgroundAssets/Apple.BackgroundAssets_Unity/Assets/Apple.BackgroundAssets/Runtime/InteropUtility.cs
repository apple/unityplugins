using UnityEngine.Scripting;

[assembly:Preserve]

namespace Apple.BackgroundAssets {
	
	internal static class InteropUtility {
		
#if (UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS) && !UNITY_EDITOR
		public const string DllName = "__Internal";
#else
		public const string DllName = "BackgroundAssetsWrapper";
#endif
		
	}
	
}
