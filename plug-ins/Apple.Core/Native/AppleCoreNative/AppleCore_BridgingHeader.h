//
//  AppleCore_BridgingHeader.h
//  AppleCoreNative
//
//  Copyright © 2023 Apple, Inc. All rights reserved.
//

#ifndef AppleCore_BridgingHeader_h
#define AppleCore_BridgingHeader_h

typedef enum {
    Unknown = 0,
    iOS = 1,
    macOS = 2,
    tvOS = 3,
    visionOS = 4
} ACOperatingSystem;

typedef struct {
    int majorVersion;
    int minorVersion;
} ACVersionNumber;

typedef struct {
    ACOperatingSystem operatingSystem;
    ACVersionNumber versionNumber;
} ACRuntimeEnvironment;

#endif /* AppleCore_BridgingHeader_h */
