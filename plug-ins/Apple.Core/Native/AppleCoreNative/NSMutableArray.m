//
//  NSMutableArray.m
//  AppleCoreNative
//

#import <Foundation/Foundation.h>

void * NSMutableArray_Init(void) {
    return (void *)CFBridgingRetain([[NSMutableArray alloc]init]);
}

void NSMutableArray_AddNSObject(void * nsMutableArrayPtr, void * nsObjectPtr, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (nsMutableArrayPtr != NULL && nsObjectPtr != NULL) {
            [(__bridge NSMutableArray *)nsMutableArrayPtr addObject:(__bridge NSObject *)nsObjectPtr];
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
}

void NSMutableArray_InsertNSObjectAtIndex(void * nsMutableArrayPtr, void * nsObjectPtr, NSUInteger atIndex, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (nsMutableArrayPtr != NULL && nsObjectPtr != NULL) {
            [(__bridge NSMutableArray *)nsMutableArrayPtr insertObject:(__bridge NSObject *)nsObjectPtr atIndex:atIndex];
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
}

void NSMutableArray_RemoveNSObjectAtIndex(void * nsMutableArrayPtr, NSUInteger atIndex, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (nsMutableArrayPtr != NULL) {
            [(__bridge NSMutableArray *)nsMutableArrayPtr removeObjectAtIndex:atIndex];
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
}

void NSMutableArray_ReplaceNSObjectAtIndex(void * nsMutableArrayPtr, NSUInteger atIndex, void * nsObjectPtr, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (nsMutableArrayPtr != NULL && nsObjectPtr != NULL) {
            [(__bridge NSMutableArray *)nsMutableArrayPtr replaceObjectAtIndex:atIndex withObject:(__bridge NSObject *)nsObjectPtr];
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
}

void NSMutableArray_RemoveAllObjects(void * nsMutableArrayPtr, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (nsMutableArrayPtr != NULL) {
            [(__bridge NSMutableArray *)nsMutableArrayPtr removeAllObjects];
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
}

void NSMutableArray_RemoveNSObject(void * nsMutableArrayPtr, void * nsObjectPtr, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (nsMutableArrayPtr != NULL && nsObjectPtr != NULL) {
            [(__bridge NSMutableArray *)nsMutableArrayPtr removeObject:(__bridge NSObject *)nsObjectPtr];
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
}
