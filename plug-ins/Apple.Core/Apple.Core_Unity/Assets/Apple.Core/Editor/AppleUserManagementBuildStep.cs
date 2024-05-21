using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
using UnityEditor.iOS.Xcode;
#endif

namespace Apple.Core
{
    public class AppleUserManagementBuildStep : AppleBuildStep
    {
        public override string DisplayName => "Apple.Core.UserManagementForAppleTV";

        [Tooltip("The value that grants access to TVUserManager, so you can map your own profiles to users in the system.")]
        public bool AllowGetCurrentUser = false;
        [Tooltip("The value that grants access to a separate set of data for your app for each user from GameCenter, iCloud, and local storage. Available in tvOS 14 or later.")]
        public bool AllowRunsAsCurrentUser = false;

        public void Reset()
        {
            IsEnabled = false;
        }

#if (UNITY_EDITOR_OSX && (UNITY_IOS || UNITY_TVOS || UNITY_STANDALONE_OSX || UNITY_VISIONOS))
        public override void OnProcessEntitlements(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltTarget, PlistDocument entitlements)
        {
            if (buildTarget == BuildTarget.tvOS)
            {
                var userEntitlements = entitlements.root.CreateArray("com.apple.developer.user-management");
                
                if(AllowGetCurrentUser)
                {
                    userEntitlements.AddString("get-current-user");
                }
                
                if(AllowRunsAsCurrentUser)
                {
                    userEntitlements.AddString("runs-as-current-user");
                }
            }
        }
#endif
    }
}
