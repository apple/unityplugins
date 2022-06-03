//
//  NSArray.swift
//  AppleCoreWrapper
//

import Foundation

@_cdecl("NSArray_GetCount")
public func NSArray_GetCount
(
    pointer: UnsafeMutableRawPointer
) -> Int32
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue();
    return Int32(array.count);
}

@_cdecl("NSArray_Free")
public func NSArray_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<NSArray>.fromOpaque(pointer).autorelease();
}

@_cdecl("NSArray_GetStringAt")
public func NSArray_GetStringAt
(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> char_p?
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue();
    
    if let value = array[Int(index)] as? String {
        return value.toCharPCopy();
    }
    
    return nil;
}

@_cdecl("NSArray_GetInt64At")
public func NSArray_GetInt64At
(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> Int
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue();
    
    if let value = array[Int(index)] as? Int {
        return value;
    }
    
    return Int();
}

@_cdecl("NSArray_GetBooleanAt")
public func NSArray_GetBooleanAt
(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> Bool
{
    let array = Unmanaged<NSArray>.fromOpaque(pointer).takeUnretainedValue();
    
    if let value = array[Int(index)] as? Bool {
        return value;
    }
    
    return false;
}
