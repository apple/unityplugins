//
//  Logger.swift
//  AppleCoreNative
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation

// TODO: In the future, we need to figure out how to handle how the new apple logging system makes these strings private 
@_cdecl("AppleCore_NSLog")
public func AppleCore_Log(log: char_p) {
    NSLog(log.toString())
}
