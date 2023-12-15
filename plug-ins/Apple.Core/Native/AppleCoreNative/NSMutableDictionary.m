//
//  NSMutableDictionary.m
//  AppleCoreNative
//

#import <Foundation/Foundation.h>

void * NSMutableDictionary_Init(void)
{
    return (void *)CFBridgingRetain([[NSMutableDictionary alloc] init]);
}

void NSMutableDictionary_SetNSObjectForKey(void * nsMutableDictionaryPtr, void * nsObjectKeyPtr, void * nsObjectValuePtr, void (* exceptionCallback)(void * nsExceptionPtr))
{
    @try {
        if (nsMutableDictionaryPtr != NULL && nsObjectKeyPtr != NULL && nsObjectValuePtr != NULL) {
            [(__bridge NSMutableDictionary *)nsMutableDictionaryPtr setObject:(__bridge NSObject *)nsObjectValuePtr forKey:(__bridge NSObject<NSCopying> *)nsObjectKeyPtr];
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
}

void NSMutableDictionary_RemoveObjectForKey(void * nsMutableDictionaryPtr, void * nsObjectKeyPtr, void (* exceptionCallback)(void * nsExceptionPtr))
{
    @try {
        if (nsMutableDictionaryPtr != NULL && nsObjectKeyPtr != NULL) {
            [(__bridge NSMutableDictionary *)nsMutableDictionaryPtr removeObjectForKey:(__bridge NSObject<NSCopying> *)nsObjectKeyPtr];
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
}

void NSMutableDictionary_RemoveAllObjects(void * nsMutableDictionaryPtr, void (* exceptionCallback)(void * nsExceptionPtr))
{
    @try {
        if (nsMutableDictionaryPtr != NULL) {
            [(__bridge NSMutableDictionary *)nsMutableDictionaryPtr removeAllObjects];
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
}
