//
//  AppleCoreRuntimeShared.swift
//  CloudKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation

public typealias char_p = UnsafeMutablePointer<Int8>;
public typealias uchar_p = UnsafeMutablePointer<UInt8>;

public typealias ErrorCallback = @convention(c) (InteropError) -> Void;
public typealias NSErrorCallback = @convention(c) (Int64, UnsafeMutableRawPointer) -> Void;
public typealias SuccessCallback = @convention(c) () -> Void;
public typealias SuccessTaskCallback = @convention(c) (Int64) -> Void;
public typealias SuccessTaskPtrCallback = @convention(c) (Int64, UnsafeMutableRawPointer?) -> Void;
public typealias SuccessTaskIntCallback = @convention(c) (Int64, Int) -> Void;
public typealias SuccessTaskImageCallback = @convention(c) (Int64, Int32, Int32, uchar_p, Int32) -> Void;

public extension InteropStructArray {
    func toData() -> Data {
        let opaquePointer = UnsafeMutablePointer<UInt8>(OpaquePointer(self.pointer));
        let buffer = UnsafeMutableBufferPointer<UInt8>(start: opaquePointer, count: Int(self.length));
        return Data(buffer: buffer);
    }
    func toArray<Element>() -> [Element] {
        let opaquePointer = UnsafeMutablePointer<Element>(OpaquePointer(self.pointer));
        let buffer = UnsafeBufferPointer<Element>(start: opaquePointer, count: Int(self.length));
        return Array(buffer);
    }
}

public extension InteropStructDictionary {
    func toDictionary<KElement, VElement>() -> [KElement:VElement] {
        var dict : [KElement: VElement] = [:];
        
        let keys : [KElement] = self.keys.toArray();
        let values : [VElement] = self.values.toArray();
        
        for i in 0..<keys.count {
            if(i < values.count) {
                dict.updateValue(values[i], forKey: keys[i]);
            }
        }
        
        return dict;
    }
}

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
    func toCKWError(taskId: Int64) -> InteropError {
        return InteropError(
            code: Int32(self.code()),
            localizedDescription: self.localizedDescription.toCharPCopy(),
            taskId: Int(taskId));
    }
}

public extension Array {
    func toUnsafeMutablePointer() -> UnsafeMutablePointer<Element> {
        let ptr = UnsafeMutablePointer<Element>.allocate(capacity: self.count);
        ptr.assign(from: self, count: self.count);
        
        return ptr;
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
