//
//  BridgingHeader-macOS.h
//  CoreHapticsWrapperMac
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#ifndef BridgingHeader_macOS_h
#define BridgingHeader_macOS_h

#include <stdbool.h>

typedef struct {
    int code;
    char * localizedDescription;
} CHWError;

#include "Haptics/CHHapticPatternPlayer_BridgingHeader.h"
#endif /* BridgingHeader_macOS_h */
