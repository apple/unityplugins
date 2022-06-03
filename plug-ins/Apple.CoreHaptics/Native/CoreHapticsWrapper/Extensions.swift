//
//  Extensions.swift
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import CoreHaptics
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
    func toCHWError() -> CHWError {
        return CHWError(
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

@available(iOS 13, macOS 10, tvOS 14, *)
public extension CHWHapticParameterCurve {
    func toCHHapticParameterCurve() -> CHHapticParameterCurve {
        let chwPoints = self.points.toArray(length: Int(self.pointsLength));
        
        // Convert to control points...
        var points = [CHHapticParameterCurve.ControlPoint]();
        
        for p in chwPoints {
            points.append(p.toCHHapticParameterCurveControlPoint());
        }
        
        return CHHapticParameterCurve(
            parameterID: dynamicParameterForInt(Int(parameterId)),
            controlPoints: points,
            relativeTime: self.relativeTime)
    }
}

@available(iOS 13, macOS 10, tvOS 14, *)
public extension CHWHapticParameterCurveControlPoint {
    func toCHHapticParameterCurveControlPoint() -> CHHapticParameterCurve.ControlPoint {
        return CHHapticParameterCurve.ControlPoint(
            relativeTime: TimeInterval(self.relativeTime),
            value: self.value);
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

