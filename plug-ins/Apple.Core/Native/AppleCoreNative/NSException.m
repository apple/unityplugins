//
//  NSException.m
//  AppleCoreNative
//

#import <Foundation/Foundation.h>

// String values returned from a native method should be UTF–8 encoded and allocated on the heap. Mono marshalling calls free for strings like this.
// Source: https://docs.unity3d.com/Manual/PluginsForIOS.html

const char * NSException_GetName(void * nsExceptionPtr) {
    return strdup([((__bridge NSException *)nsExceptionPtr).name UTF8String]);
}

const char * NSException_GetReason(void * nsExceptionPtr) {
    return strdup([((__bridge NSException *)nsExceptionPtr).reason UTF8String]);
}

void * NSException_GetUserInfo(void * nsExceptionPtr) {
    return (void *)CFBridgingRetain(((__bridge NSException *)nsExceptionPtr).userInfo);
}
