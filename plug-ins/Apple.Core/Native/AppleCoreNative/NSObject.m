//
//  NSObject.m
//  AppleCoreNative
//

#import <Foundation/Foundation.h>

// Info about bridging:
// https://developer.apple.com/library/archive/documentation/CoreFoundation/Conceptual/CFDesignConcepts/Articles/tollFreeBridgedTypes.html

void NSObject_Free(void * nsObjectPtr) {
    if (nsObjectPtr != NULL) {
        CFBridgingRelease(nsObjectPtr);
    }
}

// Try to dynamically downcast an NSObject to an object of the named type.
// Returns a retained pointer to the downcasted object if successful; NULL if the cast fails.
void * NSObject_As(void * nsObjectPtr, const char * targetClassName) {
    if (nsObjectPtr != NULL && targetClassName != NULL) {
        Class targetClass = NSClassFromString([NSString stringWithUTF8String:targetClassName]);
        if (targetClass != nil) {
            NSObject * nsObject = (__bridge NSObject *)nsObjectPtr;
            if ([nsObject isKindOfClass:targetClass]) {
                return (void *)CFBridgingRetain(nsObject);
            }
        }
    }
    return NULL;
}

bool NSObject_Is(void * nsObjectPtr, const char * targetClassName) {
    if (nsObjectPtr != NULL && targetClassName != NULL) {
        Class targetClass = NSClassFromString([NSString stringWithUTF8String:targetClassName]);
        if (targetClass != nil) {
            NSObject * nsObject = (__bridge NSObject *)nsObjectPtr;
            if ([nsObject isKindOfClass:targetClass]) {
                return true;
            }
        }
    }
    return false;
}
