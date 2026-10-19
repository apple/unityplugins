//
//  VerificationResult.swift
//  StoreKitWrapper
//
//  Created by Andrew Hall on 1/29/26.
//  Copyright © 2026 Apple, Inc. All rights reserved.
//

import StoreKit

// MARK: - Transaction Verification Result - Common Functions

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_Transaction_Free")
public func VerificationResult_Transaction_Free(pointer: UnsafeMutableRawPointer) {
    let ptr = pointer.assumingMemoryBound(to: VerificationResult<Transaction>.self)
    ptr.deinitialize(count: 1)
    ptr.deallocate()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_Transaction_IsVerified")
public func VerificationResult_Transaction_IsVerified(pointer: UnsafeMutableRawPointer) -> Bool {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<Transaction>.self).pointee
    switch verificationResult {
    case .verified:
        return true
    case .unverified:
        return false
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_Transaction_UnsafePayloadValue")
public func VerificationResult_Transaction_UnsafePayloadValue(ptr: UnsafeMutableRawPointer) -> char_p {
    // Returns empty string - use GetPayloadPointer for actual payload
    return "".toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_Transaction_VerificationError")
public func VerificationResult_Transaction_VerificationError(pointer: UnsafeMutableRawPointer) -> UnsafeMutableRawPointer? {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<Transaction>.self).pointee
    switch verificationResult {
    case .verified:
        return nil
    case .unverified(_, let error):
        let nsError = error as NSError
        return Unmanaged.passRetained(nsError).toOpaque()
    }
}

// MARK: - Transaction Verification Result - JWS Functions

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_Transaction_GetPayloadPointer")
public func VerificationResult_Transaction_GetPayloadPointer(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<Transaction>.self).pointee
    let payload = verificationResult.unsafePayloadValue
    let ptr = UnsafeMutablePointer<Transaction>.allocate(capacity: 1)
    ptr.initialize(to: payload)
    return ptr.getRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_Transaction_GetJwsRepresentation")
public func VerificationResult_Transaction_GetJwsRepresentation(
    pointer: UnsafeMutableRawPointer
) -> char_p {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<Transaction>.self).pointee
    return verificationResult.jwsRepresentation.toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_Transaction_GetHeaderData")
public func VerificationResult_Transaction_GetHeaderData(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<Transaction>.self).pointee
    return NSData(data: verificationResult.headerData).passRetainedUnsafeMutableRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_Transaction_GetPayloadData")
public func VerificationResult_Transaction_GetPayloadData(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<Transaction>.self).pointee
    return NSData(data: verificationResult.payloadData).passRetainedUnsafeMutableRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_Transaction_GetSignatureData")
public func VerificationResult_Transaction_GetSignatureData(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<Transaction>.self).pointee
    return NSData(data: verificationResult.signatureData).passRetainedUnsafeMutableRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_Transaction_GetSignature")
public func VerificationResult_Transaction_GetSignature(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<Transaction>.self).pointee
    return NSData(data: verificationResult.signature.rawRepresentation).passRetainedUnsafeMutableRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_Transaction_GetSignedData")
public func VerificationResult_Transaction_GetSignedData(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<Transaction>.self).pointee
    return NSData(data: verificationResult.signedData).passRetainedUnsafeMutableRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_Transaction_GetSignedDate")
public func VerificationResult_Transaction_GetSignedDate(
    pointer: UnsafeMutableRawPointer
) -> Int64 {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<Transaction>.self).pointee
    return Int64(verificationResult.signedDate.timeIntervalSince1970)
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_Transaction_GetDeviceVerification")
public func VerificationResult_Transaction_GetDeviceVerification(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<Transaction>.self).pointee
    return NSData(data: verificationResult.deviceVerification).passRetainedUnsafeMutableRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_Transaction_GetDeviceVerificationNonce")
public func VerificationResult_Transaction_GetDeviceVerificationNonce(
    pointer: UnsafeMutableRawPointer
) -> char_p {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<Transaction>.self).pointee
    return verificationResult.deviceVerificationNonce.uuidString.toCharPCopy()
}

// MARK: - AppTransaction Verification Result Functions

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("VerificationResult_AppTransaction_Free")
public func VerificationResult_AppTransaction_Free(pointer: UnsafeMutableRawPointer) {
    let ptr = pointer.assumingMemoryBound(to: VerificationResult<AppTransaction>.self)
    ptr.deinitialize(count: 1)
    ptr.deallocate()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("VerificationResult_AppTransaction_IsVerified")
public func VerificationResult_AppTransaction_IsVerified(pointer: UnsafeMutableRawPointer) -> Bool {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<AppTransaction>.self).pointee
    switch verificationResult {
    case .verified:
        return true
    case .unverified:
        return false
    }
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("VerificationResult_AppTransaction_UnsafePayloadValue")
public func VerificationResult_AppTransaction_UnsafePayloadValue(ptr: UnsafeMutableRawPointer) -> char_p {
    // Returns empty string - use GetPayloadPointer for actual payload
    return "".toCharPCopy()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("VerificationResult_AppTransaction_VerificationError")
public func VerificationResult_AppTransaction_VerificationError(pointer: UnsafeMutableRawPointer) -> UnsafeMutableRawPointer? {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<AppTransaction>.self).pointee
    switch verificationResult {
    case .verified:
        return nil
    case .unverified(_, let error):
        let nsError = error as NSError
        return Unmanaged.passRetained(nsError).toOpaque()
    }
}

// JWS Functions
@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("VerificationResult_AppTransaction_GetPayloadPointer")
public func VerificationResult_AppTransaction_GetPayloadPointer(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<AppTransaction>.self).pointee
    let payload = verificationResult.unsafePayloadValue
    let ptr = UnsafeMutablePointer<AppTransaction>.allocate(capacity: 1)
    ptr.initialize(to: payload)
    return ptr.getRawPointer()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("VerificationResult_AppTransaction_GetJwsRepresentation")
public func VerificationResult_AppTransaction_GetJwsRepresentation(
    pointer: UnsafeMutableRawPointer
) -> char_p {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<AppTransaction>.self).pointee
    return verificationResult.jwsRepresentation.toCharPCopy()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("VerificationResult_AppTransaction_GetHeaderData")
public func VerificationResult_AppTransaction_GetHeaderData(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<AppTransaction>.self).pointee
    return NSData(data: verificationResult.headerData).passRetainedUnsafeMutableRawPointer()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("VerificationResult_AppTransaction_GetPayloadData")
public func VerificationResult_AppTransaction_GetPayloadData(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<AppTransaction>.self).pointee
    return NSData(data: verificationResult.payloadData).passRetainedUnsafeMutableRawPointer()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("VerificationResult_AppTransaction_GetSignatureData")
public func VerificationResult_AppTransaction_GetSignatureData(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<AppTransaction>.self).pointee
    return NSData(data: verificationResult.signatureData).passRetainedUnsafeMutableRawPointer()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("VerificationResult_AppTransaction_GetSignature")
public func VerificationResult_AppTransaction_GetSignature(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<AppTransaction>.self).pointee
    return NSData(data: verificationResult.signature.rawRepresentation).passRetainedUnsafeMutableRawPointer()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("VerificationResult_AppTransaction_GetSignedData")
public func VerificationResult_AppTransaction_GetSignedData(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<AppTransaction>.self).pointee
    return NSData(data: verificationResult.signedData).passRetainedUnsafeMutableRawPointer()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("VerificationResult_AppTransaction_GetSignedDate")
public func VerificationResult_AppTransaction_GetSignedDate(
    pointer: UnsafeMutableRawPointer
) -> Int64 {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<AppTransaction>.self).pointee
    return Int64(verificationResult.signedDate.timeIntervalSince1970)
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("VerificationResult_AppTransaction_GetDeviceVerification")
public func VerificationResult_AppTransaction_GetDeviceVerification(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<AppTransaction>.self).pointee
    return NSData(data: verificationResult.deviceVerification).passRetainedUnsafeMutableRawPointer()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("VerificationResult_AppTransaction_GetDeviceVerificationNonce")
public func VerificationResult_AppTransaction_GetDeviceVerificationNonce(
    pointer: UnsafeMutableRawPointer
) -> char_p {
    let verificationResult = pointer.assumingMemoryBound(to: VerificationResult<AppTransaction>.self).pointee
    return verificationResult.deviceVerificationNonce.uuidString.toCharPCopy()
}
