//
//  NSString.m
//  AppleCoreNative
//

#import <Foundation/Foundation.h>

void * NSString_StringWithUTF8String(const char * nullTerminatedCString) {
    return (void *)CFBridgingRetain([NSString stringWithUTF8String:nullTerminatedCString]);
}

// String values returned from a native method should be UTF–8 encoded and allocated on the heap. Mono marshalling calls free for strings like this.
// Source: https://docs.unity3d.com/Manual/PluginsForIOS.html
const char * NSString_Utf8String(void * nsStringPtr) {
    return strdup([(__bridge NSString *)nsStringPtr UTF8String]);
}

void * NSString_string(void) {
    return (void *)CFBridgingRetain([NSString string]);
}
