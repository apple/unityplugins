//
//  NSDictionary.swift
//  AppleCoreWrapper
//

import Foundation

@_cdecl("NSDictionary_Free")
public func NSDictionary_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<NSDictionary>.fromOpaque(pointer).autorelease();
}

@_cdecl("NSDictionary_GetValueForKey_AsNSDictionary")
public func NSDictionary_GetValueForKey_AsNSDictionary
(
    pointer: UnsafeMutableRawPointer,
    key: char_p
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<NSDictionary>.fromOpaque(pointer).takeUnretainedValue();
    
    if let value = target.value(forKey: key.toString()) as? NSDictionary {
        return Unmanaged.passRetained(value).toOpaque();
    }
    
    return nil;
}

@_cdecl("NSDictionary_GetValueForKey_AsNSError")
public func NSDictionary_GetValueForKey_AsNSError
(
    pointer: UnsafeMutableRawPointer,
    key: char_p
) -> UnsafeMutableRawPointer?
{
    let target = Unmanaged<NSDictionary>.fromOpaque(pointer).takeUnretainedValue();
    
    if let value = target.value(forKey: key.toString()) as? NSError {
        return Unmanaged.passRetained(value).toOpaque();
    }
    
    return nil;
}

@_cdecl("NSDictionary_GetValueForKey_AsBoolean")
public func NSDictionary_GetValueForKey_AsBoolean
(
    pointer: UnsafeMutableRawPointer,
    key: char_p
) -> Bool
{
    let target = Unmanaged<NSDictionary>.fromOpaque(pointer).takeUnretainedValue();
    
    if let value = target.value(forKey: key.toString()) as? Bool {
        return value;
    }
    
    return false;
}

@_cdecl("NSDictionary_GetValueForKey_AsInt64")
public func NSDictionary_GetValueForKey_AsInt64
(
    pointer: UnsafeMutableRawPointer,
    key: char_p
) -> Int
{
    let target = Unmanaged<NSDictionary>.fromOpaque(pointer).takeUnretainedValue();
    
    if let value = target.value(forKey: key.toString()) as? Int {
        return value;
    }
    
    return Int();
}

@_cdecl("NSDictionary_GetValueForKey_AsDouble")
public func NSDictionary_GetValueForKey_AsDouble
(
    pointer: UnsafeMutableRawPointer,
    key: char_p
) -> Double
{
    let target = Unmanaged<NSDictionary>.fromOpaque(pointer).takeUnretainedValue();
    
    if let value = target.value(forKey: key.toString()) as? Double {
        return value;
    }
    
    return Double();
}

@_cdecl("NSDictionary_GetValueForKey_AsString")
public func NSDictionary_GetValueForKey_AsString
(
    pointer: UnsafeMutableRawPointer,
    key: char_p
) -> char_p?
{
    let target = Unmanaged<NSDictionary>.fromOpaque(pointer).takeUnretainedValue();
    
    if let value = target.value(forKey: key.toString()) as? String {
        return value.toCharPCopy();
    }
    
    return nil;
}
