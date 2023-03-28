//
//  UbiquitousKeyValueStore_BridgingHeader.h
//  CloudKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#ifndef UbiquitousKeyValueStore_BridgingHeader_h
#define UbiquitousKeyValueStore_BridgingHeader_h

#import "AppleCoreRuntimeShared.h"

typedef enum {
    UbiquitousKeyValueStoreServerChange = 1,
    UbiquitousKeyValueStoreInitialSyncChange = 2,
    UbiquitousKeyValueStoreQuotaViolationChange = 3,
    UbiquitousKeyValueStoreAccountChange = 4
} UbiquitousKeyValueStoreChangeReason;

#endif /* UbiquitousKeyValueStore_BridgingHeader_h */
