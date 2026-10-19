//
//  AppleCoreRuntimeShared.swift
//  StoreKitWrapper
//
//  Copyright © 2024 Apple, Inc. All rights reserved.
//

import Foundation

public typealias char_p = UnsafeMutablePointer<Int8>;
public typealias uchar_p = UnsafeMutablePointer<UInt8>;

public typealias NSExceptionCallback = @convention(c) (UnsafeMutablePointer<NSException>) -> Void;
public typealias NSErrorCallback = @convention(c) (UnsafeMutableRawPointer /*nsErrorPtr*/) -> Void;

// async task completion callbacks
public typealias NSErrorTaskCallback = @convention(c) (Int64 /*taskId*/, UnsafeMutableRawPointer /*nsErrorPtr*/) -> Void;
public typealias SuccessTaskCallback = @convention(c) (Int64 /*taskId*/) -> Void;
public typealias SuccessTaskBoolReturningCallback = @convention(c) (Int64 /*taskId*/, UnsafeMutableRawPointer?) -> Bool;
public typealias SuccessTaskRawPtrCallback = @convention(c) (Int64 /*taskId*/, UnsafeMutableRawPointer?) -> Void;
public typealias SuccessTaskPtrCallback<T> = @convention(c) (Int64 /*taskId*/, UnsafeMutableRawPointer?) -> Void;
public typealias SuccessTaskArrayCallback = @convention(c) (Int64 /*taskId*/, UnsafeMutablePointer<UnsafeMutableRawPointer>, Int /*count*/) -> Void;
public typealias SuccessTaskIntCallback = @convention(c) (Int64 /*taskId*/, Int) -> Void;
public typealias SuccessTaskBoolCallback = @convention(c) (Int64 /*taskId*/, Bool) -> Void;
public typealias SuccessStreamBoolRawPtrCallback = @convention(c) (Int64 /*streamId*/, Bool, UnsafeMutableRawPointer?) -> Void;

public extension NSObjectProtocol where Self : NSObjectProtocol {
    @inlinable func passRetainedUnsafeMutablePointer() -> UnsafeMutablePointer<Self> {
        return Unmanaged.passRetained(self).toOpaque().assumingMemoryBound(to: Self.self);
    }

    @inlinable func passRetainedUnsafeMutableRawPointer() -> UnsafeMutableRawPointer {
        return Unmanaged.passRetained(self).toOpaque();
    }
}

public extension UnsafeMutablePointer where Pointee : NSObjectProtocol {
    @inlinable func takeUnretainedValue() -> Pointee {
        return Unmanaged<Pointee>.fromOpaque(self).takeUnretainedValue();
    }
}

public extension UnsafeMutableRawPointer {
    @inlinable func takeUnretainedValue<Pointee>() -> Pointee where Pointee : NSObjectProtocol {
        return Unmanaged<Pointee>.fromOpaque(self).takeUnretainedValue();
    }
}

public extension UnsafeMutablePointer {
    @inlinable func getRawPointer() -> UnsafeMutableRawPointer {
        return UnsafeMutableRawPointer(self);
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

public extension Array {
    func toUnsafeMutablePointer() -> UnsafeMutablePointer<Element> {
        let ptr = UnsafeMutablePointer<Element>.allocate(capacity: self.count);
        ptr.update(from: self, count: self.count);

        return ptr;
    }
}
