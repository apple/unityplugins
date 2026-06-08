//
//  Currency.swift
//  AppleCoreNative
//

import Foundation

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 1.0, *)
@_cdecl("Currency_Free")
public func Currency_Free(pointer: UnsafeMutableRawPointer)
{
    let currencyPtr = pointer.assumingMemoryBound(to: Locale.Currency.self)
    currencyPtr.deinitialize(count: 1)
    currencyPtr.deallocate()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 1.0, *)
@_cdecl("Currency_GetIdentifier")
public func Currency_GetIdentifier(pointer: UnsafeMutableRawPointer) -> char_p
{
    let currency = pointer.assumingMemoryBound(to: Locale.Currency.self).pointee
    return currency.identifier.toCharPCopy()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 1.0, *)
@_cdecl("Currency_GetIsISOCurrency")
public func Currency_GetIsISOCurrency(pointer: UnsafeMutableRawPointer) -> Bool
{
    let currency = pointer.assumingMemoryBound(to: Locale.Currency.self).pointee
    return currency.isISOCurrency
}
