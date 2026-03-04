//
//  SpatialController.h
//  SpatialController
//
//  Copyright © 2025 Apple, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

//! Project version number for SpatialController.
FOUNDATION_EXPORT double SpatialControllerVersionNumber;

//! Project version string for SpatialController.
FOUNDATION_EXPORT const unsigned char SpatialControllerVersionString[];

//! iOS & tvOS Frameworks do not support bridging headers...
#if TARGET_OS_IOS || TARGET_OS_TV || TARGET_OS_VISION
    #include <stdbool.h>
    #include <simd/simd.h>

    #import <SpatialController/SpatialController_BridgingHeader.h>
#endif
