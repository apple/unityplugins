namespace Apple.GameController
{
    internal static class InteropUtility
    {
#if UNITY_IOS || UNITY_TVOS
        public const string DLLName = "__Internal";
#else
        public const string DLLName = "GameControllerWrapper";
#endif
    }
}