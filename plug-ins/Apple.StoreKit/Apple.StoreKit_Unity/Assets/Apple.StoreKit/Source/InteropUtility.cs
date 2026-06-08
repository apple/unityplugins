using UnityEngine.Scripting;

[assembly: Preserve]

namespace Apple.StoreKit
{

    internal static class InteropUtility
    {
#if (UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS) && !UNITY_EDITOR
        public const string DLLName = "__Internal";
#else
        public const string DLLName = "StoreKitWrapper";
#endif
    }
}
