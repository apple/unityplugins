namespace Apple.CoreHaptics
{
    internal static class CHInteropUtility
    {
#if (UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS) && !UNITY_EDITOR
        public const string DllName = "__Internal";
#else
        public const string DllName = "CoreHapticsWrapper";
#endif
    }
}