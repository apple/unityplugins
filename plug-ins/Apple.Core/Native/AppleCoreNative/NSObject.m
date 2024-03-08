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

bool NSObject_IsEqual(void * nsObject1Ptr, void * nsObject2Ptr) {

    // Pointers to same address mean both objects are the same.
    if (nsObject1Ptr == nsObject2Ptr) {
        return true;
    }

    // Pointers are not the same but if one is NULL then they are definitely not the same object.
    if (nsObject1Ptr == NULL || nsObject2Ptr == NULL) {
        return false;
    }

    // Pointers are not the same and neither are NULL. Use isEqual to determine equality.
    return [(__bridge NSObject *)nsObject1Ptr isEqual:(__bridge NSObject *)nsObject2Ptr];
}

unsigned long NSObject_Hash(void * nsObjectPtr) {
    return (unsigned long)((__bridge NSObject *)nsObjectPtr).hash;
}
