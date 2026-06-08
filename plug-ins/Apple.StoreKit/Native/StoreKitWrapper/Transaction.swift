//
//  Transaction.swift
//  StoreKitWrapper
//
//  Created by Andrew Hall on 11/20/25.
//  Copyright © 2025 Apple, Inc. All rights reserved.
//

import Foundation
import StoreKit

// MARK: - Transaction Properties

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_Free")
public func Transaction_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    let transactionPtr = pointer.assumingMemoryBound(to: Transaction.self);
    transactionPtr.deinitialize(count: 1);
    transactionPtr.deallocate();
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetId")
public func Transaction_GetId
(
    pointer: UnsafeMutableRawPointer
) -> UInt64
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    return transaction.id
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetOriginalId")
public func Transaction_GetOriginalId
(
    pointer: UnsafeMutableRawPointer
) -> UInt64
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    return transaction.originalID
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetProductId")
public func Transaction_GetProductId
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    return transaction.productID.toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetPurchaseDate")
public func Transaction_GetPurchaseDate
(
    pointer: UnsafeMutableRawPointer
) -> Int64
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    return Int64(transaction.purchaseDate.timeIntervalSince1970)
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetOriginalPurchaseDate")
public func Transaction_GetOriginalPurchaseDate
(
    pointer: UnsafeMutableRawPointer
) -> Int64
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    return Int64(transaction.originalPurchaseDate.timeIntervalSince1970)
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetSignedDate")
public func Transaction_GetSignedDate
(
    pointer: UnsafeMutableRawPointer
) -> Int64
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    return Int64(transaction.signedDate.timeIntervalSince1970)
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetExpirationDate")
public func Transaction_GetExpirationDate
(
    pointer: UnsafeMutableRawPointer
) -> Int64
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee

    if let expirationDate = transaction.expirationDate {
        return Int64(expirationDate.timeIntervalSince1970)
    }

    return 0
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetRevocationDate")
public func Transaction_GetRevocationDate
(
    pointer: UnsafeMutableRawPointer
) -> Int64
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee

    if let revocationDate = transaction.revocationDate {
        return Int64(revocationDate.timeIntervalSince1970)
    }

    return 0
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetRevocationReason")
public func Transaction_GetRevocationReason
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee

    guard let revocationReason = transaction.revocationReason else {
        return nil
    }

    let ptr = UnsafeMutablePointer<Transaction.RevocationReason>.allocate(capacity: 1)
    ptr.initialize(to: revocationReason)
    return ptr.getRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetIsUpgraded")
public func Transaction_GetIsUpgraded
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    return transaction.isUpgraded
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetOfferId")
public func Transaction_GetOfferId
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee

    guard let offerID = transaction.offerID else {
        return nil
    }

    return offerID.toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetOfferType")
public func Transaction_GetOfferType
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee

    guard let offerType = transaction.offerType else {
        return -1
    }

    switch offerType {
    case .introductory:
        return 0
    case .promotional:
        return 1
    case .code:
        return 2
    case .winBack:
        return 3
    default:
        return -1
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetEnvironment")
public func Transaction_GetEnvironment
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee

    if #available(iOS 16.0, macOS 13.0, tvOS 16.0, *) {
        switch transaction.environment {
        case .production:
            return 0
        case .sandbox:
            return 1
        case .xcode:
            return 2
        default:
            return -1
        }
    } else {
        return -1
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetOwnershipType")
public func Transaction_GetOwnershipType
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee

    switch transaction.ownershipType {
    case .purchased:
        return 0
    case .familyShared:
        return 1
    default:
        return -1
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetAppAccountToken")
public func Transaction_GetAppAccountToken
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee

    guard let token = transaction.appAccountToken else {
        return nil
    }

    return token.uuidString.toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetJsonRepresentation")
public func Transaction_GetJsonRepresentation
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    let jsonString = String(data: transaction.jsonRepresentation, encoding: .utf8) ?? ""
    return jsonString.toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetPurchasedQuantity")
public func Transaction_GetPurchasedQuantity
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    return transaction.purchasedQuantity
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetProductType")
public func Transaction_GetProductType(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    switch transaction.productType {
    case .consumable: return 0
    case .nonConsumable: return 1
    case .autoRenewable: return 2
    case .nonRenewable: return 3
    default: return -1
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetAppBundleId")
public func Transaction_GetAppBundleId(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    return transaction.appBundleID.toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetSubscriptionGroupId")
public func Transaction_GetSubscriptionGroupId(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    guard let id = transaction.subscriptionGroupID else { return nil }
    return id.toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetWebOrderLineItemId")
public func Transaction_GetWebOrderLineItemId(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    guard let id = transaction.webOrderLineItemID else { return nil }
    return id.toCharPCopy()
}

#if !os(visionOS)
@available(iOS 15.0, macOS 12.0, tvOS 15.0, *)
@_cdecl("Transaction_GetStorefrontCountryCode")
public func Transaction_GetStorefrontCountryCode(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    return transaction.storefrontCountryCode.toCharPCopy()
}
#endif

@available(iOS 17.0, macOS 14.0, tvOS 17.0, visionOS 2.2, *)
@_cdecl("Transaction_GetReason")
public func Transaction_GetReason(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    switch transaction.reason {
    case .purchase: return 0
    case .renewal: return 1
    default: return -1
    }
}

@available(iOS 17.0, macOS 14.0, tvOS 17.0, visionOS 2.2, *)
@_cdecl("Transaction_GetPrice")
public func Transaction_GetPrice(
    pointer: UnsafeMutableRawPointer
) -> Double
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    guard let price = transaction.price else { return -1.0 }
    return NSDecimalNumber(decimal: price).doubleValue
}

@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 1.0, *)
@_cdecl("Transaction_GetCurrency")
public func Transaction_GetCurrency(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    guard let currency = transaction.currency else { return nil }
    let ptr = UnsafeMutablePointer<Locale.Currency>.allocate(capacity: 1)
    ptr.initialize(to: currency)
    return UnsafeMutableRawPointer(ptr)
}

@available(iOS 17.2, macOS 14.2, tvOS 17.2, visionOS 2.2, *)
@_cdecl("Transaction_GetOffer")
public func Transaction_GetOffer(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    guard let offer = transaction.offer else { return nil }
    let ptr = UnsafeMutablePointer<Transaction.Offer>.allocate(capacity: 1)
    ptr.initialize(to: offer)
    return ptr.getRawPointer()
}

@available(iOS 17.2, macOS 14.2, tvOS 17.2, visionOS 2.2, *)
@_cdecl("TransactionOffer_Free")
public func TransactionOffer_Free(
    pointer: UnsafeMutableRawPointer
)
{
    let ptr = pointer.assumingMemoryBound(to: Transaction.Offer.self)
    ptr.deinitialize(count: 1)
    ptr.deallocate()
}

@available(iOS 17.2, macOS 14.2, tvOS 17.2, visionOS 2.2, *)
@_cdecl("TransactionOffer_GetId")
public func TransactionOffer_GetId(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let offer = pointer.assumingMemoryBound(to: Transaction.Offer.self).pointee
    guard let id = offer.id else { return nil }
    return id.toCharPCopy()
}

@available(iOS 17.2, macOS 14.2, tvOS 17.2, visionOS 2.2, *)
@_cdecl("TransactionOffer_GetType")
public func TransactionOffer_GetType(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let offer = pointer.assumingMemoryBound(to: Transaction.Offer.self).pointee
    switch offer.type {
    case .introductory: return 0
    case .promotional: return 1
    case .code: return 2
    case .winBack: return 3
    default: return -1
    }
}

@available(iOS 16.4, macOS 13.3, tvOS 16.4, visionOS 2.2, *)
@_cdecl("Transaction_GetDeviceVerificationId")
public func Transaction_GetDeviceVerificationId(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    return transaction.deviceVerificationNonce.uuidString.toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetAppTransactionID")
public func Transaction_GetAppTransactionID(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    if #available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 1.0, *) {
        let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
        return transaction.appTransactionID.toCharPCopy()
    }
    return nil
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetRevocationPercentage")
public func Transaction_GetRevocationPercentage(
    pointer: UnsafeMutableRawPointer
) -> Double
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
    guard let percentage = transaction.revocationPercentage else { return -1 }
    return (percentage as NSDecimalNumber).doubleValue
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetRevocationType")
public func Transaction_GetRevocationType(
    pointer: UnsafeMutableRawPointer
) -> Int32
{
    if #available(iOS 26.4, macOS 26.4, tvOS 26.4, visionOS 26.4, *) {
        let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee
        guard let revocationType = transaction.revocationType else { return -1 }
        switch revocationType {
        case .familyRevocation: return 0
        case .fullRefund: return 1
        case .proratedRefund: return 2
        default: return -1
        }
    }
    return -1
}

// MARK: - RevocationReason Properties

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("RevocationReason_Free")
public func RevocationReason_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    let revocationReasonPtr = pointer.assumingMemoryBound(to: Transaction.RevocationReason.self);
    revocationReasonPtr.deinitialize(count: 1);
    revocationReasonPtr.deallocate();
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("RevocationReason_GetRawValue")
public func RevocationReason_GetRawValue
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let revocationReason = pointer.assumingMemoryBound(to: Transaction.RevocationReason.self).pointee;

    switch revocationReason {
    case .developerIssue:
        return 0
    case .other:
        return 1
    default:
        return -1
    }
}

@available(iOS 15.4, macOS 12.3, tvOS 15.4, visionOS 2.2, *)
@_cdecl("RevocationReason_GetLocalizedDescription")
public func RevocationReason_GetLocalizedDescription
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let revocationReason = pointer.assumingMemoryBound(to: Transaction.RevocationReason.self).pointee;
    return revocationReason.localizedDescription.toCharPCopy();
}

// MARK: - RefundRequestStatus Properties

@available(iOS 15.0, macOS 12.0, tvOS 17.0, visionOS 2.2, *)
@_cdecl("RefundRequestStatus_Free")
public func RefundRequestStatus_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    let refundRequestStatusPtr = pointer.assumingMemoryBound(to: Transaction.RefundRequestStatus.self);
    refundRequestStatusPtr.deinitialize(count: 1);
    refundRequestStatusPtr.deallocate();
}

@available(iOS 15.0, macOS 12.0, tvOS 17.0, visionOS 2.2, *)
@_cdecl("RefundRequestStatus_GetRawValue")
public func RefundRequestStatus_GetRawValue
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let refundRequestStatus = pointer.assumingMemoryBound(to: Transaction.RefundRequestStatus.self).pointee;

    switch refundRequestStatus {
    case .success:
        return 0
    case .userCancelled:
        return 1
    @unknown default:
        return -1
    }
}

// MARK: - Transaction Methods

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_Finish")
public func Transaction_Finish
(
    pointer: UnsafeMutableRawPointer
)
{
    let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee

    Task {
        await transaction.finish()
    }
}

// MARK: - Static Methods

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetCurrentEntitlements")
public func Transaction_GetCurrentEntitlements
(
    taskId: Int64,
    onAdd: @escaping SuccessTaskRawPtrCallback,
    onComplete: @escaping SuccessTaskCallback
)
{
    Task {
        defer {
            onComplete(taskId)
        }

        for await verificationResult in Transaction.currentEntitlements {
            let ptr = UnsafeMutablePointer<VerificationResult<Transaction>>.allocate(capacity: 1);
            ptr.initialize(to: verificationResult)
            onAdd(taskId, ptr.getRawPointer())
        }
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetAll")
public func Transaction_GetAll
(
    taskId: Int64,
    onAdd: @escaping SuccessTaskRawPtrCallback,
    onComplete: @escaping SuccessTaskCallback
)
{
    Task {
        defer {
            onComplete(taskId)
        }

        for await verificationResult in Transaction.all {
            let ptr = UnsafeMutablePointer<VerificationResult<Transaction>>.allocate(capacity: 1);
            ptr.initialize(to: verificationResult)
            onAdd(taskId, ptr.getRawPointer())
        }
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetUnfinished")
public func Transaction_GetUnfinished
(
    taskId: Int64,
    onAdd: @escaping SuccessTaskRawPtrCallback,
    onComplete: @escaping SuccessTaskCallback
)
{
    Task {
        defer
        {
            onComplete(taskId)
        }

        for await verificationResult in Transaction.unfinished {
            let ptr = UnsafeMutablePointer<VerificationResult<Transaction>>.allocate(capacity: 1);
            ptr.initialize(to: verificationResult)
            onAdd(taskId, ptr.getRawPointer())
        }
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_GetLatest")
public func Transaction_GetLatest
(
    productId: char_p,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskRawPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let productIdString = productId.toString()

    Task {
        guard let verificationResult = await Transaction.latest(for: productIdString) else {
            onSuccess(taskId, nil)
            return
        }

        let ptr = UnsafeMutablePointer<VerificationResult<Transaction>>.allocate(capacity: 1);
        ptr.initialize(to: verificationResult)
        onSuccess(taskId, ptr.getRawPointer())
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Transaction_Updates")
public func Transaction_Updates
(
    taskId: Int64,
    onAdd: @escaping SuccessTaskBoolReturningCallback
)
{
    Task {
        for await verificationResult in Transaction.updates {
            let ptr = UnsafeMutablePointer<VerificationResult<Transaction>>.allocate(capacity: 1);
            ptr.initialize(to: verificationResult)
            let shouldContinue = onAdd(taskId, ptr.getRawPointer())
            if (!shouldContinue)
            {
                return
            }
        }
    }
}

// BeginRefund is not available on tvOS
#if !os(tvOS)
@available(iOS 15.0, macOS 12.0, visionOS 2.2, *)
@_cdecl("Transaction_BeginRefundWithId")
public func Transaction_BeginRefundWithId
(
    transactionId: UInt64,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskRawPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    Task {
        do
        {
            #if os(macOS)
            guard let viewController = UiUtilities.rootViewController() else  {
                onError(taskId, NSError(domain: "StoreKitWrapper", code: 0, userInfo: [NSLocalizedDescriptionKey: "No view controller available"]).passRetainedUnsafeMutableRawPointer())
                return
            }

            let status = try await Transaction.beginRefundRequest(for: transactionId,in: viewController)
            #else
            guard let scene = await UiUtilities.defaultWindow()?.windowScene else {
                onError(taskId, NSError(domain: "StoreKitWrapper", code: 0, userInfo: [NSLocalizedDescriptionKey: "No scene available"]).passRetainedUnsafeMutableRawPointer())
                return
            }

            let status = try await Transaction.beginRefundRequest(for: transactionId, in: scene)
            #endif
            let ptr = UnsafeMutablePointer<Transaction.RefundRequestStatus>.allocate(capacity: 1)
            ptr.initialize(to: status)
            onSuccess(taskId, ptr.getRawPointer())
        }
        catch
        {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
        }
    }
}

@available(iOS 15.0, macOS 12.0, visionOS 2.2, *)
@_cdecl("Transaction_BeginRefund")
public func Transaction_BeginRefund
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskRawPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    Task {
        do
        {
            let transaction = pointer.assumingMemoryBound(to: Transaction.self).pointee

            #if os(macOS)
            guard let viewController = UiUtilities.rootViewController() else  {
                onError(taskId, NSError(domain: "StoreKitWrapper", code: 0, userInfo: [NSLocalizedDescriptionKey: "No view controller available"]).passRetainedUnsafeMutableRawPointer())
                return
            }

            let status = try await transaction.beginRefundRequest(in: viewController)
            #else
            guard let scene = await UiUtilities.defaultWindow()?.windowScene else {
                onError(taskId, NSError(domain: "StoreKitWrapper", code: 0, userInfo: [NSLocalizedDescriptionKey: "No scene available"]).passRetainedUnsafeMutableRawPointer())
                return
            }

            let status = try await transaction.beginRefundRequest(in: scene)
            #endif

            let ptr = UnsafeMutablePointer<Transaction.RefundRequestStatus>.allocate(capacity: 1)
            ptr.initialize(to: status)
            onSuccess(taskId, ptr.getRawPointer())
        }
        catch
        {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
        }
    }
}
#endif
