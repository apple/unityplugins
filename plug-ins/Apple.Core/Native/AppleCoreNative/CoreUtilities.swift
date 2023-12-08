//
//  CoreUtilities.swift
//  AppleCoreNative
//
//  Copyright © 2021-2023 Apple, Inc. All rights reserved.
//

import Foundation

// TODO: In the future, we need to figure out how to handle how the new apple logging system makes these strings private 
@_cdecl("AppleCore_NSLog")
public func AppleCore_Log(log: char_p) {
    NSLog(log.toString())
}

@_cdecl("AppleCore_GetRuntimeEnvironment")
public func AppleCore_GetRuntimeEnvironment() -> ACRuntimeEnvironment {
    
    let osVersion = ProcessInfo.processInfo.operatingSystemVersion
    
#if os(iOS)
    return ACRuntimeEnvironment(operatingSystem: iOS, majorVersion: osVersion.majorVersion, minorVersion: osVersion.minorVersion)
#elseif os(macOS)
    return ACRuntimeEnvironment(operatingSystem: macOS, majorVersion: osVersion.majorVersion, minorVersion: osVersion.minorVersion)
#elseif os(tvOS)
    return ACRuntimeEnvironment(operatingSystem: tvOS, majorVersion: osVersion.majorVersion, minorVersion: osVersion.minorVersion)
#else
    return ACRuntimeEnvironment(operatingSystem: Unknown, majorVersion: osVersion.majorVersion, minorVersion: osVersion.minorVersion)
#endif
}
