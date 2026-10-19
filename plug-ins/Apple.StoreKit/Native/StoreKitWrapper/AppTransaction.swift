import Foundation
import StoreKit

// MARK: - Property Getters

@available(iOS 16.0, macOS 13.0, tvOS 16.0, *)
@_cdecl("AppTransaction_Free")
public func AppTransaction_Free(pointer: UnsafeMutableRawPointer) {
    let appTransactionPtr = pointer.assumingMemoryBound(to: AppTransaction.self);
    appTransactionPtr.deinitialize(count: 1);
    appTransactionPtr.deallocate();
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, *)
@_cdecl("AppTransaction_GetOriginalAppVersion")
public func AppTransaction_GetOriginalAppVersion(pointer: UnsafeMutableRawPointer) -> char_p {
    let appTransaction = pointer.assumingMemoryBound(to: AppTransaction.self).pointee
    return appTransaction.originalAppVersion.toCharPCopy()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, *)
@_cdecl("AppTransaction_GetOriginalPurchaseDate")
public func AppTransaction_GetOriginalPurchaseDate(pointer: UnsafeMutableRawPointer) -> Int64 {
    let appTransaction = pointer.assumingMemoryBound(to: AppTransaction.self).pointee
    return Int64(appTransaction.originalPurchaseDate.timeIntervalSince1970)
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, *)
@_cdecl("AppTransaction_GetSignedDate")
public func AppTransaction_GetSignedDate(pointer: UnsafeMutableRawPointer) -> Int64 {
    let appTransaction = pointer.assumingMemoryBound(to: AppTransaction.self).pointee
    return Int64(appTransaction.signedDate.timeIntervalSince1970)
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, *)
@_cdecl("AppTransaction_GetAppVersion")
public func AppTransaction_GetAppVersion(pointer: UnsafeMutableRawPointer) -> char_p {
    let appTransaction = pointer.assumingMemoryBound(to: AppTransaction.self).pointee
    return appTransaction.appVersion.toCharPCopy()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, *)
@_cdecl("AppTransaction_GetAppVersionId")
public func AppTransaction_GetAppVersionId(pointer: UnsafeMutableRawPointer) -> UInt64 {
    let appTransaction = pointer.assumingMemoryBound(to: AppTransaction.self).pointee
    return appTransaction.appVersionID ?? 0
}

@available(iOS 17.4, macOS 14.4, tvOS 17.4, *)
@_cdecl("AppTransaction_GetPreorderDate")
public func AppTransaction_GetPreorderDate(pointer: UnsafeMutableRawPointer) -> Int64 {
    let appTransaction = pointer.assumingMemoryBound(to: AppTransaction.self).pointee

    if let preorderDate = appTransaction.preorderDate {
        return Int64(preorderDate.timeIntervalSince1970)
    }

    return 0
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, *)
@_cdecl("AppTransaction_GetBundleId")
public func AppTransaction_GetBundleId(pointer: UnsafeMutableRawPointer) -> char_p {
    let appTransaction = pointer.assumingMemoryBound(to: AppTransaction.self).pointee
    return appTransaction.bundleID.toCharPCopy()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, *)
@_cdecl("AppTransaction_GetAppId")
public func AppTransaction_GetAppId(pointer: UnsafeMutableRawPointer) -> UInt64 {
    let appTransaction = pointer.assumingMemoryBound(to: AppTransaction.self).pointee
    return appTransaction.appID ?? 0
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, *)
@_cdecl("AppTransaction_GetEnvironment")
public func AppTransaction_GetEnvironment(pointer: UnsafeMutableRawPointer) -> Int {
    let appTransaction = pointer.assumingMemoryBound(to: AppTransaction.self).pointee
    switch appTransaction.environment {
    case .production:
        return 0
    case .sandbox:
        return 1
    case .xcode:
        return 2
    default:
        return 0
    }
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, *)
@_cdecl("AppTransaction_GetJsonRepresentation")
public func AppTransaction_GetJsonRepresentation(pointer: UnsafeMutableRawPointer) -> NSData {
    let appTransaction = pointer.assumingMemoryBound(to: AppTransaction.self).pointee
    return NSData(data: appTransaction.jsonRepresentation)
}

@available(iOS 16.4, macOS 13.3, tvOS 16.4, *)
@_cdecl("AppTransaction_GetDeviceVerification")
public func AppTransaction_GetDeviceVerification(pointer: UnsafeMutableRawPointer) -> NSData {
    let appTransaction = pointer.assumingMemoryBound(to: AppTransaction.self).pointee
    return NSData(data: appTransaction.deviceVerification)
}

@available(iOS 16.4, macOS 13.3, tvOS 16.4, *)
@_cdecl("AppTransaction_GetDeviceVerificationNonce")
public func AppTransaction_GetDeviceVerificationNonce(pointer: UnsafeMutableRawPointer) -> char_p {
    let appTransaction = pointer.assumingMemoryBound(to: AppTransaction.self).pointee
    return appTransaction.deviceVerificationNonce.uuidString.toCharPCopy()
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, *)
@_cdecl("AppTransaction_GetAppTransactionID")
public func AppTransaction_GetAppTransactionID(pointer: UnsafeMutableRawPointer) -> char_p {
    let appTransaction = pointer.assumingMemoryBound(to: AppTransaction.self).pointee
    return appTransaction.appTransactionID.toCharPCopy()
}

@available(iOS 18.4, macOS 15.4, tvOS 18.4, visionOS 2.4, *)
@_cdecl("AppTransaction_GetOriginalPlatform")
public func AppTransaction_GetOriginalPlatform(pointer: UnsafeMutableRawPointer) -> Int {
    let appTransaction = pointer.assumingMemoryBound(to: AppTransaction.self).pointee
    let platform = appTransaction.originalPlatform
    switch platform
    {
    case .iOS:
        return 0
    case .macOS:
        return 1
    case .tvOS:
        return 2
    case .visionOS:
        return 4
    default:
        return -1
    }
}

// MARK: - Static Methods

@_cdecl("AppTransaction_GetShared")
public func AppTransaction_GetShared(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskRawPtrCallback,
    onError: @escaping NSErrorTaskCallback
) {
    if #available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *) {
        Task {
            do {
                let verificationResult = try await AppTransaction.shared
                let ptr = UnsafeMutablePointer<VerificationResult<AppTransaction>>.allocate(capacity: 1); ptr.initialize(to: verificationResult)
                onSuccess(taskId, ptr.getRawPointer())
            } catch {
                onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
            }
        }
    } else {
        onError(taskId, NSError(domain: "StoreKitWrapper", code: -1, userInfo: [NSLocalizedDescriptionKey: "AppTransaction requires iOS 16.0+"]).passRetainedUnsafeMutableRawPointer())
    }
}

@_cdecl("AppTransaction_Refresh")
public func AppTransaction_Refresh(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskRawPtrCallback,
    onError: @escaping NSErrorTaskCallback
) {
    if #available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *) {
        Task {
            do {
                let verificationResult = try await AppTransaction.refresh()
                let ptr = UnsafeMutablePointer<VerificationResult<AppTransaction>>.allocate(capacity: 1); ptr.initialize(to: verificationResult)
                onSuccess(taskId, ptr.getRawPointer())
            } catch {
                onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
            }
        }
    } else {
        onError(taskId, NSError(domain: "StoreKitWrapper", code: -1, userInfo: [NSLocalizedDescriptionKey: "AppTransaction requires iOS 16.0+"]).passRetainedUnsafeMutableRawPointer())
    }
}
