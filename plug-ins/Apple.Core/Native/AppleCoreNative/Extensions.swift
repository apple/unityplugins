//
//  Extensions.swift
//  AppleCoreNative
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation

public extension String {
    /**
     Returns a copy of the string as an UnsafeMutablePointer<Int8> to match char * expectations in c.
     
     - Remark: Doing this in Swift allows for C# to simply use string instead of char *.
     */
    func toCharPCopy() -> char_p {
        let utfText = (self as NSString).utf8String!;
        let pointer = UnsafeMutablePointer<Int8>.allocate(capacity: (8 * self.count) + 1);
        return strcpy(pointer, utfText);
    }
}

public extension char_p {
    func toString() -> String {
        return String(cString: self);
    }
}
