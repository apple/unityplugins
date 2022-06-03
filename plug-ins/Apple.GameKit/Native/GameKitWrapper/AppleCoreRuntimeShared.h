//
//  AppleCoreRuntimeShared.h
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#ifndef AppleCoreRuntimeShared_h
#define AppleCoreRuntimeShared_h

typedef struct {
    int code;
    char * localizedDescription;
    long taskId;
} InteropError;

typedef struct {
    void * pointer;
    int length;
} InteropStructArray;

typedef struct {
    InteropStructArray keys;
    InteropStructArray values;
} InteropStructDictionary;

#endif /* AppleCoreRuntimeShared_h */
