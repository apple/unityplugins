//
//  NSMutableArray.swift
//  AppleCoreWrapper
//

import Foundation

@_cdecl("NSMutableArray_Init")
public func NSMutableArray_Init() -> UnsafeMutableRawPointer {
    let array = NSMutableArray.init();
    return Unmanaged.passRetained(array).toOpaque();
}

@_cdecl("NSMutableArray_AddString")
public func NSMutableArray_AddString
(
    pointer: UnsafeMutableRawPointer,
    value: char_p
)
{
    let array = Unmanaged<NSMutableArray>.fromOpaque(pointer).takeUnretainedValue();
    array.add(value.toString());
}

@_cdecl("NSMutableArray_AddInt64")
public func NSMutableArray_AddInt64
(
    pointer: UnsafeMutableRawPointer,
    value: Int
)
{
    let array = Unmanaged<NSMutableArray>.fromOpaque(pointer).takeUnretainedValue();
    array.add(value);
}

@_cdecl("NSMutableArray_AddBoolean")
public func NSMutableArray_AddBoolean
(
    pointer: UnsafeMutableRawPointer,
    value: Bool
)
{
    let array = Unmanaged<NSMutableArray>.fromOpaque(pointer).takeUnretainedValue();
    array.add(value);
}
