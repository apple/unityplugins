//
//  Storefront.swift
//  StoreKitWrapper
//

import Foundation
import StoreKit

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Storefront_GetCurrent")
public func Storefront_GetCurrent(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskRawPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    Task {
        guard let storefront = await Storefront.current else {
            onSuccess(taskId, nil)
            return
        }
        let ptr = UnsafeMutablePointer<Storefront>.allocate(capacity: 1)
        ptr.initialize(to: storefront)
        onSuccess(taskId, ptr.getRawPointer())
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Storefront_Free")
public func Storefront_Free(
    pointer: UnsafeMutableRawPointer
)
{
    let ptr = pointer.assumingMemoryBound(to: Storefront.self)
    ptr.deinitialize(count: 1)
    ptr.deallocate()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Storefront_GetId")
public func Storefront_GetId(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let storefront = pointer.assumingMemoryBound(to: Storefront.self).pointee
    return storefront.id.toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Storefront_GetCountryCode")
public func Storefront_GetCountryCode(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let storefront = pointer.assumingMemoryBound(to: Storefront.self).pointee
    return storefront.countryCode.toCharPCopy()
}
