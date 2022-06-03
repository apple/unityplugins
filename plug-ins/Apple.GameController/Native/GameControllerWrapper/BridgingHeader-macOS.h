//
//  BridgingHeader-macOS.h
//  GameControllerWrapperMac
//
//  Copyright © 2020 Apple, Inc. All rights reserved.
//

#ifndef BridgingHeader_macOS_h
#define BridgingHeader_macOS_h

#include <stdbool.h>

typedef struct {
    int code;
    char * localizedDescription;
} GCWError;

#import "Controller/Controller_BridgingHeader.h"

#endif /* BridgingHeader_macOS_h */
