//
//  NSNumber.m
//  AppleCoreNative
//

#import <Foundation/Foundation.h>


// String values returned from a native method should be UTF–8 encoded and allocated on the heap. Mono marshalling calls free for strings like this.
// Source: https://docs.unity3d.com/Manual/PluginsForIOS.html
const char * NSNumber_ObjCType(void * instancePtr) {
    return strdup([(__bridge NSNumber *)instancePtr objCType]);
}

const char * NSNumber_StringValue(void * instancePtr) {
    return strdup([[(__bridge NSNumber *)instancePtr stringValue] UTF8String]);
}

/*
The following macro simplifies declaration of methods like the following (shown for booleans):

void * NSNumber_NumberWithBool(BOOL value) {
    return (void *)CFBridgingRetain([NSNumber numberWithBool:value]);
}

BOOL NSNumber_BoolValue(void * instancePtr) {
    return [(__bridge NSNumber *)instancePtr boolValue];
}
*/
#define NSNumberMethods(MethodPrefix, MethodSuffix, InputOutputType) \
void * NSNumber_NumberWith##MethodSuffix(InputOutputType value) { return (void *)CFBridgingRetain([NSNumber numberWith##MethodSuffix:value]); } \
InputOutputType NSNumber_ ## MethodSuffix ## Value(void * instancePtr) { return [(__bridge NSNumber *)instancePtr MethodPrefix ## Value]; }

NSNumberMethods(bool, Bool, BOOL);
NSNumberMethods(char, Char, char);
NSNumberMethods(double, Double, double);
NSNumberMethods(float, Float, float);
NSNumberMethods(int, Int, int);
NSNumberMethods(long, Long, long);
NSNumberMethods(longLong, LongLong, long long);
NSNumberMethods(short, Short, short);
NSNumberMethods(unsignedChar, UnsignedChar, unsigned char);
NSNumberMethods(unsignedInt, UnsignedInt, unsigned int);
NSNumberMethods(unsignedLong, UnsignedLong, unsigned long);
NSNumberMethods(unsignedLongLong, UnsignedLongLong, unsigned long long);
NSNumberMethods(unsignedShort, UnsignedShort, unsigned short);
