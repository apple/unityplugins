//
//  NSArray.m
//  AppleCoreNative
//

#import <Foundation/Foundation.h>

NSUInteger NSArray_GetCount(void * nsArrayPtr, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (nsArrayPtr != NULL) {
            NSArray * nsArray = (__bridge NSArray *)nsArrayPtr;
            return nsArray.count;
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return 0;
}

bool NSArray_TryGetNSObjectAt(void * nsArrayPtr, NSUInteger index, void ** nsObjectOutPtr, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (nsArrayPtr != NULL && nsObjectOutPtr != NULL) {
            NSArray * nsArray = (__bridge NSArray *)nsArrayPtr;
            id item = nsArray[index];
            if ([item isKindOfClass:[NSObject class]]) {
                *nsObjectOutPtr = (void *)CFBridgingRetain((NSObject *)item);
                return true;
            }
        }
    }
    @catch (NSException * e) {
        // Return false for index out of range; throw for everything else.
        if (![e.name isEqualToString:NSRangeException]) {
            exceptionCallback((void *)CFBridgingRetain(e));
        }
    }
    return false;
}

bool NSArray_IndexOfNSObject(void * nsArrayPtr, void * nsObjectPtr, NSUInteger * indexOutPtr, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (nsArrayPtr != NULL && nsObjectPtr != NULL && indexOutPtr != NULL) {
            NSArray * nsArray = (__bridge NSArray *)nsArrayPtr;
            NSObject * nsObject = (__bridge NSObject *)nsObjectPtr;

            *indexOutPtr = [nsArray indexOfObject:nsObject];
            return *indexOutPtr != NSNotFound;
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return false;
}

bool NSArray_ContainsNSObject(void * nsArrayPtr, void * nsObjectPtr, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (nsArrayPtr != NULL && nsObjectPtr != NULL) {
            NSArray * nsArray = (__bridge NSArray *)nsArrayPtr;
            NSObject * nsObject = (__bridge NSObject *)nsObjectPtr;

            return [nsArray containsObject:nsObject];
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return false;
}

void * NSArray_FromJson(const char * jsonString, void (* errorCallback)(void * nsErrorPtr), void (* exceptionCallback)(void * nsExceptionPtr))
{
    @try {
        NSData * nsData = [[NSString stringWithUTF8String:jsonString] dataUsingEncoding:NSUTF8StringEncoding];

        NSJSONReadingOptions options = 0;
        if (@available(macos 12.0, ios 15.0, watchos 8.0, tvos 15.0, *)) {
            options = NSJSONReadingJSON5Allowed;
        }

        NSError * nsError = nil;
        NSArray * nsArray = [NSJSONSerialization JSONObjectWithData:nsData options:options error:&nsError];

        if (nsError != nil) {
            errorCallback((void *)CFBridgingRetain(nsError));
            return NULL;
        }

        return nsArray != nil ? (void *)CFBridgingRetain(nsArray) : NULL;
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }

    return NULL;
}

const char * NSArray_ToJson(void * nsArrayPtr, void (* errorCallback)(void * nsErrorPtr), void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (nsArrayPtr != NULL) {
            NSArray * nsArray = (__bridge NSArray *)nsArrayPtr;

            NSError * nsError = nil;
            NSData * nsData = [NSJSONSerialization dataWithJSONObject:nsArray options:0 error:&nsError];

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

