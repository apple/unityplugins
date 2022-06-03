//
//  NSError.swift
//  AppleCoreWrapper_iOS
//

import Foundation

@_cdecl("NSError_Free")
public func NSError_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    _ = Unmanaged<NSError>.fromOpaque(pointer).autorelease();
}

@_cdecl("NSError_GetCode")
public func NSError_GetCode
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let error = Unmanaged<NSError>.fromOpaque(pointer).takeUnretainedValue();
    return error.code;
}

@_cdecl("NSError_GetDomain")
public func NSError_GetDomain
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let error = Unmanaged<NSError>.fromOpaque(pointer).takeUnretainedValue();
    return error.domain.toCharPCopy();
}

@_cdecl("NSError_GetLocalizedDescription")
public func NSError_GetLocalizedDescription
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let error = Unmanaged<NSError>.fromOpaque(pointer).takeUnretainedValue();
    return error.localizedDescription.toCharPCopy();
}

@_cdecl("NSError_GetLocalizedRecoverySuggestion")
public func NSError_GetLocalizedRecoverySuggestion
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let error = Unmanaged<NSError>.fromOpaque(pointer).takeUnretainedValue();
    
    if(error.localizedRecoverySuggestion != nil) {
        return error.localizedRecoverySuggestion!.toCharPCopy();
    }
    
    return nil;
}

@_cdecl("NSError_GetLocalizedFailureReason")
public func NSError_GetLocalizedFailureReason
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let error = Unmanaged<NSError>.fromOpaque(pointer).takeUnretainedValue();
    
    if(error.localizedFailureReason != nil) {
        return error.localizedFailureReason!.toCharPCopy();
    }
    
    return nil;
}

@_cdecl("NSError_GetUserInfo")
public func NSError_GetUserInfo
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let error = Unmanaged<NSError>.fromOpaque(pointer).takeUnretainedValue();
    
    let dictionary = error.userInfo as NSDictionary;
    return Unmanaged.passRetained(dictionary).toOpaque();
}
