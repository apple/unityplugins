//
//  NSNull.m
//  AppleCoreNative
//

#import <Foundation/Foundation.h>

void * NSNull_Null(void) {
    return (void *)CFBridgingRetain([NSNull null]);
}
