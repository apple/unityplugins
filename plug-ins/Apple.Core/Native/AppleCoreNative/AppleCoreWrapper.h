//
//  AppleCoreWrapper.h
//  AppleCoreWrapper
//

#import <Foundation/Foundation.h>

//! Project version number for AppleCoreWrapper.
FOUNDATION_EXPORT double AppleCoreWrapperVersionNumber;

//! Project version string for AppleCoreWrapper.
FOUNDATION_EXPORT const unsigned char AppleCoreWrapperVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <AppleCoreWrapper/PublicHeader.h>

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
