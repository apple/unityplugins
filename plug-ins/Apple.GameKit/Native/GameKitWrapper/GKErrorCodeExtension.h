//
//  GKErrorCodeExtension.h
//  GameKitWrapper
//
//  Copyright © 2023 Apple, Inc. All rights reserved.
//

#ifndef GKErrorCodeExtension_h
#define GKErrorCodeExtension_h

#import <Foundation/Foundation.h>

// Unity-Specific GameKit plug-in Error Codes
typedef NS_ENUM(NSInteger, GKErrorCodeExtension) {
    GKErrorCodeExtensionInvalidOperationNotAuthenticated = -1,
    GKErrorCodeExtensionNoLeaderboardFound = -2,
    GKErrorCodeExtensionNoParticipantsInMatch = -3,
    GKErrorCodeExtensionPlayerNotFound = -4,
    GKErrorCodeExtensionNoLeaderboardSetFound = -5,
    GKErrorCodeExtensionNoRealTimeMatch = -6,
    GKErrorCodeExtensionUnsupportedOperationForOSVersion = -7,
    GKErrorCodeExtensionNoAchievementDescriptionFound = -8,
    GKErrorCodeExtensionNoAchievementDescriptionImageFound = -9,
    GKErrorCodeExtensionGameCenterDashboardAlreadyShown = -10,
};

#endif /* GKErrorCodeExtension_h */
