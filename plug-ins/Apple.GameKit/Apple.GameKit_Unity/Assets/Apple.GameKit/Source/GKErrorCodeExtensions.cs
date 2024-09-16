using System;
using Apple.Core;
using UnityEngine;

namespace Apple.GameKit
{
    public static class GKErrorCodeExtensions
    {
        public static string GetExplanatoryText(this GKErrorCode error)
        {
            string result = null;
            switch (error)
            {
#if UNITY_STANDALONE_OSX
                case GKErrorCode.CommunicationsFailure:
                    result =
                        "For Mac targets, ensure that both Incoming Connections and Outgoing Connections are enabled in the App Sandbox capability. " +
                        "In the Unity Editor, navigate to Edit -> Project Settings... -> Apple Build Settings -> Apple.Core.Security -> Network and enable Allow Network Server and Allow Network Client. " +
                        "(See also https://developer.apple.com/documentation/gamekit/enabling_and_configuring_game_center/)";
                    break;
#endif

                // Authentication will fail when running in the Unity Editor because Game Center will think it's an 
                // unregistered app. This is because Game Center sees Unity's bundle id rather than the app's bundle id.
                // This is normal, but not ideal during development.
                case GKErrorCode.GameUnrecognized:
                case GKErrorCode.NotAuthenticated:
                    if (Application.isEditor)
                    {
                        result = "Game Center services are unavailable when running in the Unity Editor.";
                    }
                    break;

                case GKErrorCode.UnsupportedOperationForOSVersion:
                    result = $"A call was attempted to an API that is not supported on {Availability.RuntimeEnvironment}";
                    break;

                case GKErrorCode.FriendListDescriptionMissing:
                    result = $"Friend list usage description missing. In the Unity Editor navigate to Edit -> Project Settings... -> Apple Build Settings -> Apple.GameKit -> Friend List Usage Description and add a message string.";                
                    break;

                case GKErrorCode.UbiquityContainerUnavailable:
                    result = 
                        "iCloud container unavailable. " +
                        "Follow the instructions for configuring iCloud services for saving games here: https://developer.apple.com/documentation/gamekit/saving_the_player_s_game_data_to_an_icloud_account. " +
                        "In the Unity Editor navigate to Edit -> Project Settings... -> Apple Build Settings -> Apple.GameKit -> Saved Game Container Identifier and add the iCloud container name.";
                    break;
            }

            return result;
        }

        public static void LogException(Exception exception)
        {
            // Provide some guidance for developers when certain exceptions are thrown.
            if (exception is GameKitException gkex)
            {
                var message = ((GKErrorCode)gkex.Code).GetExplanatoryText();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Debug.LogWarning(message);
                }
            }

            Debug.LogException(exception);
        }
    }
}