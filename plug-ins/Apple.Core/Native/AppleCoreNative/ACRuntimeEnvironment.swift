//
//  ACRuntimeEnvironment.swift
//  AppleCoreNative
//
//  Copyright © 2023 Apple, Inc. All rights reserved.
//

import Foundation

extension ACRuntimeEnvironment {
    public init(operatingSystem: ACOperatingSystem, majorVersion: NSInteger, minorVersion: NSInteger)
    {
        self.init()
        self.operatingSystem = operatingSystem
        self.versionNumber.majorVersion = Int32(majorVersion)
        self.versionNumber.minorVersion = Int32(minorVersion)
    }
}
