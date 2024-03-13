//
//  GKMatchedPlayers.m
//  GameKitWrapper
//
//  Copyright © 2023 Apple, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <GameKit/GKMatchmaker.h>

void * GKMatchedPlayers_Properties(void * gkMatchedPlayersPtr) {
    if (@available(ios 17.2, macos 14.2, tvos 17.2, visionOS 1.1, *)) {
        return (void *)CFBridgingRetain([(__bridge GKMatchedPlayers *)gkMatchedPlayersPtr properties]);
    } else {
        return NULL;
    }
}

void * GKMatchedPlayers_Players(void * gkMatchedPlayersPtr) {
    if (@available(ios 17.2, macos 14.2, tvos 17.2, visionOS 1.1, *)) {
        return (void *)CFBridgingRetain([(__bridge GKMatchedPlayers *)gkMatchedPlayersPtr players]);
    } else {
        return NULL;
    }
}

void * GKMatchedPlayers_PlayerProperties(void * gkMatchedPlayersPtr) {
    if (@available(ios 17.2, macos 14.2, tvos 17.2, visionOS 1.1, *)) {
        return (void *)CFBridgingRetain([(__bridge GKMatchedPlayers *)gkMatchedPlayersPtr playerProperties]);
    } else {
        return NULL;
    }
}
