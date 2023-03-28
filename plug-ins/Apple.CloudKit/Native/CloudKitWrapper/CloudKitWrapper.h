//
//  CloudKitWrapper.h
//  CloudKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

//! Project version number for CloudKitWrapper.
FOUNDATION_EXPORT double CloudKitWrapperVersionNumber;

//! Project version string for CloudKitWrapper.
FOUNDATION_EXPORT const unsigned char CloudKitWrapperVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <CloudKitWrapper/PublicHeader.h>

//! iOS & tvOS Frameworks do not support bridging headers...
#if TARGET_OS_IOS || TARGET_OS_TV
    #include <stdbool.h>
    #import "UbiquitousKeyValueStore_BridgingHeader.h"
#endif

