//
//  AppleCoreNative.h
//  AppleCoreNative
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

//! Project version number for AppleCoreNative.
FOUNDATION_EXPORT double AppleCoreNativeVersionNumber;

//! Project version string for AppleCoreNative.
FOUNDATION_EXPORT const unsigned char AppleCoreNativeVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <AppleCoreNative/PublicHeader.h>

typedef struct {
    float x;
    float y;
} InteropCGPoint;

typedef struct {
    float width;
    float height;
} InteropCGSize;

typedef struct {
    InteropCGPoint origin;
    InteropCGSize size;
} InteropCGRect;

typedef struct {
    float top;
    float left;
    float bottom;
    float right;
} InteropEdgeInset;

#if TARGET_OS_IOS || TARGET_OS_TV || TARGET_OS_VISION
    #import "AppleCore_BridgingHeader.h"
#endif
