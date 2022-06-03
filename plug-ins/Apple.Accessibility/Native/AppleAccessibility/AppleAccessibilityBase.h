//
//  AppleAccessibilityBase.h
//  AppleAccessibility
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#ifndef AppleAccessibilityBase_h
#define AppleAccessibilityBase_h

#import <AvailabilityMacros.h>

#ifdef __cplusplus
    #define APPLE_ACCESSIBILITY_EXTERN extern "C" __attribute__((visibility("default")))
    #define APPLE_ACCESSIBILITY_HIDDEN extern "C" __attribute__((visibility("hidden")))
#else
    #define APPLE_ACCESSIBILITY_EXTERN extern __attribute__((visibility("default")))
    #define APPLE_ACCESSIBILITY_HIDDEN extern __attribute__((visibility("hidden")))
#endif

#define kAXCenterPointDefaultPoint CGPointMake(-1, -1)

#endif /* AppleAccessibilityBase_h */
