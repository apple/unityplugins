//
//  CoreHapticsWrapper.h
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

//! Project version number for CoreHapticsWrapper.
FOUNDATION_EXPORT double CoreHapticsWrapperVersionNumber;

//! Project version string for CoreHapticsWrapper.
FOUNDATION_EXPORT const unsigned char CoreHapticsWrapperVersionString[];

//! iOS & tvOS Frameworks do not support bridging headers...
#if TARGET_OS_IOS || TARGET_OS_TV
    #include <stdbool.h>

    typedef struct {
        int code;
        char * localizedDescription;
    } CHWError;

    #include "CHHapticPatternPlayer_BridgingHeader.h"
#endif
