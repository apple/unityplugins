//
//  GameControllerWrapper.h
//  GameControllerWrapper
//
//  Copyright © 2020 Apple, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

//! Project version number for GameControllerWrapper.
FOUNDATION_EXPORT double GameControllerWrapperVersionNumber;

//! Project version string for GameControllerWrapper.
FOUNDATION_EXPORT const unsigned char GameControllerWrapperVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <GameControllerWrapper/PublicHeader.h>

//! iOS & tvOS Frameworks do not support bridging headers...
#if TARGET_OS_IOS || TARGET_OS_TV
    #include <stdbool.h>

    typedef struct {
        int code;
        char * localizedDescription;
    } GCWError;

    #import "Controller_BridgingHeader.h"
#endif
