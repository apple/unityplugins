//
//  NSData.m
//  AppleCoreNative
//

#import <Foundation/Foundation.h>

NSUInteger NSData_GetLength(void * nsDataPtr, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (nsDataPtr != NULL) {
            NSData * nsData = (__bridge NSData *)nsDataPtr;
            return nsData.length;
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return 0;
}

const void * NSData_GetBytes(void * nsDataPtr, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (nsDataPtr != NULL) {
            NSData * nsData = (__bridge NSData *)nsDataPtr;
            return nsData.bytes;
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return 0;
}
