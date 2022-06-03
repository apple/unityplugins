//
//  Extensions.swift
//  GameControllerWrapper
//
//  Copyright © 2020 Apple, Inc. All rights reserved.
//

import Foundation
import GameController

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

public extension Data {
    func toUCharP() -> uchar_p {
        let pointer = uchar_p.allocate(capacity: self.count);
        let buffer = UnsafeMutableBufferPointer<UInt8>(start: pointer, count: self.count);
        _ = self.copyBytes(to: buffer);
        
        return pointer;
    }
}

public extension uchar_p {
    func toData(count : Int) -> Data {
        let buffer = UnsafeMutableBufferPointer<UInt8>(start: self, count: count);
        return Data(buffer: buffer);
    }
}

public extension Error {
    func code() -> Int {
        return (self as NSError).code;
    }
    func toGCWError() -> GCWError {
        return GCWError(
            code: Int32(self.code()),
            localizedDescription: self.localizedDescription.toCharPCopy())
    }
}

public extension Array {
    func toUnsafeMutablePointer() -> UnsafeMutablePointer<Element> {
        let ptr = UnsafeMutablePointer<Element>.allocate(capacity: self.count);
        ptr.assign(from: self, count: self.count);
        
        return ptr;
    }
}

public extension UnsafeMutablePointer {
    func toArray(length : Int) -> [Pointee] {
        return Array(UnsafeBufferPointer(start: self, count: length));
    }
    func toArray<T>(length : Int) -> [T] {
        let rawPtr = UnsafeMutableRawPointer(self);
        let ptr = rawPtr.bindMemory(to: T.self, capacity: length);
        let buffer = UnsafeBufferPointer(start: ptr, count: length);
        
        return Array(buffer);
    }
}

/**
 Support bidirectional dictionary look-ups...
 */
extension Dictionary where Value: Equatable {
    func someKey(forValue val: Value) -> Key? {
        return first(where: { $1 == val })?.key
    }
}

extension Bool {
    func toFloat() -> Float {
        return self ? 1 : 0;
    }
}

extension GCController {
    func toGCWController(uid : String) -> GCWController {
        if #available(macOS 10.16, tvOS 14, iOS 14, *) {
            return GCWController(
                uniqueId: uid.toCharPCopy(),
                productCategory: self.productCategory.toCharPCopy(),
                vendorName: self.vendorName?.toCharPCopy() ?? _emptyString,
                isAttachedToDevice: self.isAttachedToDevice,
                hasHaptics: self.haptics != nil,
                hasLight: self.light != nil,
                hasBattery: self.battery != nil
            );
        } else {
            return GCWController(
                uniqueId: uid.toCharPCopy(),
                productCategory: self.productCategory.toCharPCopy(),
                vendorName: self.vendorName?.toCharPCopy() ?? _emptyString,
                isAttachedToDevice: self.isAttachedToDevice,
                hasHaptics: false,
                hasLight: false,
                hasBattery: false
            );
        }
    }
}

#if os(macOS)
import AppKit
public extension NSImage {
    func pngData() -> Data? {
        guard let cgImage = self.cgImage(forProposedRect: nil, context: nil, hints: nil)
            else { return nil }
        let imageRep = NSBitmapImageRep(cgImage: cgImage)
        imageRep.size = self.size // display size in points
        return imageRep.representation(using: .png, properties: [:])
    }
}
#endif
