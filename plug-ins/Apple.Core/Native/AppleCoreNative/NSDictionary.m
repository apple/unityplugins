//
//  NSDictionary.m
//  AppleCoreNative
//

#import <Foundation/Foundation.h>
#import <Foundation/NSJSONSerialization.h>

bool NSDictionary_IsValidKeyType(const char * keyClassName, void (* exceptionCallback)(void * nsExceptionPtr))
{
    @try {
        Class keyClass = NSClassFromString([NSString stringWithUTF8String:keyClassName]);
        if ([keyClass conformsToProtocol:@protocol(NSCopying)]) {
            return true;
        }
    }
    @catch (NSException * e)
    {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return false;
}

bool NSDictionary_TryGetValueAsNSObject(void * nsDictionaryPtr, void * nsObjectKeyPtr, void ** nsObjectValuePtr, void (* exceptionCallback)(void * nsExceptionPtr))
{
    @try {
        if (nsDictionaryPtr != NULL && nsObjectKeyPtr != NULL && nsObjectValuePtr != NULL) {
            NSDictionary * nsDictionary = (__bridge NSDictionary *)nsDictionaryPtr;
            id value = [nsDictionary objectForKey:(__bridge NSObject<NSCopying> *)nsObjectKeyPtr];
            if ([value isKindOfClass:[NSObject class]]) {
                *nsObjectValuePtr = (void *)CFBridgingRetain((NSObject *)value);
                return true;
            }
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return false;
}

bool NSDictionary_ContainsNSObjectKey(void * nsDictionaryPtr, void * nsObjectKeyPtr, void (* exceptionCallback)(void * nsExceptionPtr))
{
    @try {
        if (nsDictionaryPtr != NULL && nsObjectKeyPtr != NULL) {
            NSDictionary * nsDictionary = (__bridge NSDictionary *)nsDictionaryPtr;
            id value = [nsDictionary objectForKey:(__bridge NSObject<NSCopying> *)nsObjectKeyPtr];
            if ([value isKindOfClass:[NSObject class]]) {
                return true;
            }
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return false;
}

NSUInteger NSDictionary_Count(void * nsDictionaryPtr, void (* exceptionCallback)(void * nsExceptionPtr))
{
    @try {
        if (nsDictionaryPtr != NULL) {
            NSDictionary * nsDictionary = (__bridge NSDictionary *)nsDictionaryPtr;
            return nsDictionary.count;
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return 0;
}

void * NSDictionary_AllKeys(void * nsDictionaryPtr, void (* exceptionCallback)(void * nsExceptionPtr))
{
    @try {
        if (nsDictionaryPtr != NULL) {
            NSDictionary * nsDictionary = (__bridge NSDictionary *)nsDictionaryPtr;
            return (void *)CFBridgingRetain(nsDictionary.allKeys);
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return NULL;
}

void * NSDictionary_AllValues(void * nsDictionaryPtr, void (* exceptionCallback)(void * nsExceptionPtr))
{
    @try {
        if (nsDictionaryPtr != NULL) {
            NSDictionary * nsDictionary = (__bridge NSDictionary *)nsDictionaryPtr;
            return (void *)CFBridgingRetain(nsDictionary.allValues);
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return NULL;
}

void * NSDictionary_FromJson(const char * jsonString, void (* errorCallback)(void * nsErrorPtr), void (* exceptionCallback)(void * nsExceptionPtr))
{
    @try {
        NSData * nsData = [[NSString stringWithUTF8String:jsonString] dataUsingEncoding:NSUTF8StringEncoding];

        NSJSONReadingOptions options = 0;
        if (@available(macos 12.0, ios 15.0, watchos 8.0, tvos 15.0, *)) {
            options = NSJSONReadingJSON5Allowed | NSJSONReadingTopLevelDictionaryAssumed;
        }

        NSError * nsError = nil;
        NSDictionary * nsDictionary = [NSJSONSerialization JSONObjectWithData:nsData options:options error:&nsError];

        if (nsError != nil) {
            errorCallback((void *)CFBridgingRetain(nsError));
            return NULL;
        }

        return nsDictionary != nil ? (void *)CFBridgingRetain(nsDictionary) : NULL;
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }

    return NULL;
}

const char * NSDictionary_ToJson(void * nsDictionaryPtr, void (* errorCallback)(void * nsErrorPtr), void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (nsDictionaryPtr != NULL) {
            NSDictionary * nsDictionary = (__bridge NSDictionary *)nsDictionaryPtr;

            NSError * nsError = nil;
            NSData * nsData = [NSJSONSerialization dataWithJSONObject:nsDictionary options:0 error:&nsError];

            if (nsError != nil) {
                errorCallback((void *)CFBridgingRetain(nsError));
                return NULL;
            }

            return strdup([[[NSString alloc] initWithData:nsData encoding:NSUTF8StringEncoding] UTF8String]);
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }

    return NULL;
}
