//
//  GKAccessPoint_BridgingHeader.h
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#ifndef GKAccessPoint_BridgingHeader_h
#define GKAccessPoint_BridgingHeader_h

typedef struct {
    unsigned char * ptr;
    int length;
} PointerArray;

typedef struct {
    float x;
    float y;
    float width;
    float height;
} GKWAccessPointFrameInScreenCoordinates;

#endif /* GKAccessPoint_BridgingHeader_h */
