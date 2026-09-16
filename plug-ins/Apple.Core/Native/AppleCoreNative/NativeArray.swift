//
//  NativeArray.swift
//  AppleCoreNative
//
//  Created by Andrew Hall on 2/4/26.
//


@_cdecl("_Apple_Core_SwiftArray_GetElementAt")
public func _Apple_Core_SwiftArray_GetElementAt
(
    _ arrayPointer: UnsafeMutableRawPointer,
    _ arraySize: Int,
    _ index: Int) -> UnsafeMutableRawPointer?
{
    let arr = arrayPointer.assumingMemoryBound(to: UnsafeMutableRawPointer.self);
    return arr[index];
}

@_cdecl("_Apple_Core_SwiftArray_Free")
public func _Apple_Core_SwiftArray_Free
(
    _ arrayPointer: UnsafeMutableRawPointer
)
{
    let arr = arrayPointer.assumingMemoryBound(to: UnsafeMutableRawPointer.self);
    arr.deallocate();
}
