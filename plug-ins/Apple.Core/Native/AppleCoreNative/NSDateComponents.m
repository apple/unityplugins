//
//  NSDataComponents.m
//  AppleCoreNative
//

#import <Foundation/Foundation.h>

void NSDateComponents_SetLeapMonth(void * thisPtr, bool value, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (thisPtr != NULL) {
            NSDateComponents * thisObj = (__bridge NSDateComponents *)thisPtr;
            thisObj.leapMonth = value;
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
}

bool NSDateComponents_GetLeapMonth(void * thisPtr, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (thisPtr != NULL) {
            NSDateComponents * thisObj = (__bridge NSDateComponents *)thisPtr;
            return thisObj.leapMonth;
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return false;
}

NSTimeInterval NSDateComponents_GetDate(void * thisPtr, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (thisPtr != NULL) {
            NSDateComponents * thisObj = (__bridge NSDateComponents *)thisPtr;
            return thisObj.date.timeIntervalSince1970;
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return 0;
}

void NSDateComponents_SetValueForComponent(void * thisPtr, long value, NSCalendarUnit unit, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (thisPtr != NULL) {
            NSDateComponents * thisObj = (__bridge NSDateComponents *)thisPtr;
            return [thisObj setValue:value forComponent:unit];
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
}

NSInteger NSDateComponents_GetValueForComponent(void * thisPtr, NSCalendarUnit unit, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (thisPtr != NULL) {
            NSDateComponents * thisObj = (__bridge NSDateComponents *)thisPtr;
            return [thisObj valueForComponent:unit];
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return 0;
}

bool NSDateComponents_ValidDate(void * thisPtr, void (* exceptionCallback)(void * nsExceptionPtr)) {
    @try {
        if (thisPtr != NULL) {
            NSDateComponents * thisObj = (__bridge NSDateComponents *)thisPtr;
            return thisObj.validDate;
        }
    }
    @catch (NSException * e) {
        exceptionCallback((void *)CFBridgingRetain(e));
    }
    return false;
}

