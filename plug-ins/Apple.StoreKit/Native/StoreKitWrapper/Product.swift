//
//  Product.swift
//  StoreKitWrapper
//
//  Created by Andrew Hall on 11/20/25.
//  Copyright © 2025 Apple, Inc. All rights reserved.
//

import Foundation
import StoreKit

// MARK: - Product Properties

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_GetId")
public func Product_GetId
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee;
    return product.id.toCharPCopy();
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_GetDisplayName")
public func Product_GetDisplayName
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee;
    return product.displayName.toCharPCopy();
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_GetDescription")
public func Product_GetDescription
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee;
    return product.description.toCharPCopy();
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_GetPrice")
public func Product_GetPrice
(
    pointer: UnsafeMutableRawPointer
) -> Double
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee;
    return NSDecimalNumber(decimal: product.price).doubleValue;
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_GetDisplayPrice")
public func Product_GetDisplayPrice
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee;
    return product.displayPrice.toCharPCopy();
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_GetType")
public func Product_GetType
(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee;
    let ptr = UnsafeMutablePointer<Product.ProductType>.allocate(capacity: 1)
    ptr.initialize(to: product.type)
    return ptr.getRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_GetIsFamilyShareable")
public func Product_GetIsFamilyShareable
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee;
    return product.isFamilyShareable;
}

// MARK: - ProductType Properties

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("ProductType_Free")
public func ProductType_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    let productTypePtr = pointer.assumingMemoryBound(to: Product.ProductType.self);
    productTypePtr.deinitialize(count: 1);
    productTypePtr.deallocate();
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("ProductType_GetRawValue")
public func ProductType_GetRawValue
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let productType = pointer.assumingMemoryBound(to: Product.ProductType.self).pointee;

    switch productType {
    case .consumable:
        return 0
    case .nonConsumable:
        return 1
    case .autoRenewable:
        return 2
    case .nonRenewable:
        return 3
    default:
        return -1
    }
}

@available(iOS 15.4, macOS 12.3, tvOS 15.4, visionOS 2.2, *)
@_cdecl("ProductType_GetLocalizedDescription")
public func ProductType_GetLocalizedDescription
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let productType = pointer.assumingMemoryBound(to: Product.ProductType.self).pointee;
    return productType.localizedDescription.toCharPCopy();
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_GetJsonRepresentation")
public func Product_GetJsonRepresentation
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee;
    let jsonString = String(data: product.jsonRepresentation, encoding: .utf8) ?? ""
    return jsonString.toCharPCopy()
}

// MARK: - Subscription Info

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_GetHasSubscription")
public func Product_GetHasSubscription
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee;
    return product.subscription != nil;
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_GetSubscriptionGroupId")
public func Product_GetSubscriptionGroupId
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee;

    guard let subscription = product.subscription else {
        return nil;
    }

    return subscription.subscriptionGroupID.toCharPCopy();
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_GetSubscriptionPeriodUnit")
public func Product_GetSubscriptionPeriodUnit
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee;

    guard let subscription = product.subscription else {
        return -1;
    }

    switch subscription.subscriptionPeriod.unit {
    case .day:
        return 0;
    case .week:
        return 1;
    case .month:
        return 2;
    case .year:
        return 3;
    @unknown default:
        return -1;
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_GetSubscriptionPeriodValue")
public func Product_GetSubscriptionPeriodValue
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee;

    guard let subscription = product.subscription else {
        return 0;
    }

    return subscription.subscriptionPeriod.value;
}

// MARK: - SubscriptionInfo

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_GetSubscriptionInfo")
public func Product_GetSubscriptionInfo(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee
    guard let subscription = product.subscription else { return nil }
    let ptr = UnsafeMutablePointer<Product.SubscriptionInfo>.allocate(capacity: 1)
    ptr.initialize(to: subscription)
    return ptr.getRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionInfo_Free")
public func SubscriptionInfo_Free(
    pointer: UnsafeMutableRawPointer
)
{
    let ptr = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.self)
    ptr.deinitialize(count: 1)
    ptr.deallocate()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionInfo_GetSubscriptionGroupId")
public func SubscriptionInfo_GetSubscriptionGroupId(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.self).pointee
    return info.subscriptionGroupID.toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionInfo_GetIsEligibleForIntroOffer")
public func SubscriptionInfo_GetIsEligibleForIntroOffer(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskBoolCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.self).pointee
    Task {
        let result = await info.isEligibleForIntroOffer
        onSuccess(taskId, result)
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionInfo_GetPeriodUnit")
public func SubscriptionInfo_GetPeriodUnit(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.self).pointee
    switch info.subscriptionPeriod.unit {
    case .day: return 0
    case .week: return 1
    case .month: return 2
    case .year: return 3
    @unknown default: return -1
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionInfo_GetPeriodValue")
public func SubscriptionInfo_GetPeriodValue(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.self).pointee
    return info.subscriptionPeriod.value
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionInfo_GetIntroductoryOffer")
public func SubscriptionInfo_GetIntroductoryOffer(
    pointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer?
{
    let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.self).pointee
    guard let offer = info.introductoryOffer else { return nil }
    let ptr = UnsafeMutablePointer<Product.SubscriptionOffer>.allocate(capacity: 1)
    ptr.initialize(to: offer)
    return ptr.getRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionInfo_GetWinBackOffersCount")
public func SubscriptionInfo_GetWinBackOffersCount(
    pointer: UnsafeMutableRawPointer
) -> Int32
{
    if #available(iOS 18.0, macOS 15.0, tvOS 18.0, visionOS 2.4, *) {
        let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.self).pointee
        return Int32(info.winBackOffers.count)
    }
    return 0
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionInfo_GetWinBackOfferAt")
public func SubscriptionInfo_GetWinBackOfferAt(
    pointer: UnsafeMutableRawPointer,
    index: Int32
) -> UnsafeMutableRawPointer?
{
    if #available(iOS 18.0, macOS 15.0, tvOS 18.0, visionOS 2.4, *) {
        let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.self).pointee
        let offers = info.winBackOffers
        guard Int(index) < offers.count else { return nil }
        let ptr = UnsafeMutablePointer<Product.SubscriptionOffer>.allocate(capacity: 1)
        ptr.initialize(to: offers[Int(index)])
        return ptr.getRawPointer()
    }
    return nil
}

// MARK: - SubscriptionInfo.Status

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionInfo_GetStatus")
public func SubscriptionInfo_GetStatus(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskArrayCallback,
    onError: @escaping NSErrorTaskCallback
) {
    let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.self).pointee
    Task {
        do {
            let statuses = try await info.status
            let wrapped = statuses.map { s -> UnsafeMutableRawPointer in
                let ptr = UnsafeMutablePointer<Product.SubscriptionInfo.Status>.allocate(capacity: 1)
                ptr.initialize(to: s)
                return ptr.getRawPointer()
            }
            let ptr = wrapped.toUnsafeMutablePointer()
            onSuccess(taskId, ptr, statuses.count)
        } catch {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
        }
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionStatus_Free")
public func SubscriptionStatus_Free(pointer: UnsafeMutableRawPointer) {
    let ptr = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.Status.self)
    ptr.deinitialize(count: 1)
    ptr.deallocate()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionStatus_GetState")
public func SubscriptionStatus_GetState(pointer: UnsafeMutableRawPointer) -> Int {
    let status = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.Status.self).pointee
    switch status.state {
    case .subscribed: return 0
    case .expired: return 1
    case .inBillingRetryPeriod: return 2
    case .inGracePeriod: return 3
    case .revoked: return 4
    default: return -1
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionStatus_GetRenewalInfo")
public func SubscriptionStatus_GetRenewalInfo(pointer: UnsafeMutableRawPointer) -> UnsafeMutableRawPointer {
    let status = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.Status.self).pointee
    let ptr = UnsafeMutablePointer<VerificationResult<Product.SubscriptionInfo.RenewalInfo>>.allocate(capacity: 1)
    ptr.initialize(to: status.renewalInfo)
    return ptr.getRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionStatus_GetTransaction")
public func SubscriptionStatus_GetTransaction(pointer: UnsafeMutableRawPointer) -> UnsafeMutableRawPointer {
    let status = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.Status.self).pointee
    let ptr = UnsafeMutablePointer<VerificationResult<Transaction>>.allocate(capacity: 1)
    ptr.initialize(to: status.transaction)
    return ptr.getRawPointer()
}

// MARK: - VerificationResult<RenewalInfo>

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_RenewalInfo_Free")
public func VerificationResult_RenewalInfo_Free(pointer: UnsafeMutableRawPointer) {
    let ptr = pointer.assumingMemoryBound(to: VerificationResult<Product.SubscriptionInfo.RenewalInfo>.self)
    ptr.deinitialize(count: 1)
    ptr.deallocate()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_RenewalInfo_IsVerified")
public func VerificationResult_RenewalInfo_IsVerified(pointer: UnsafeMutableRawPointer) -> Bool {
    let vr = pointer.assumingMemoryBound(to: VerificationResult<Product.SubscriptionInfo.RenewalInfo>.self).pointee
    if case .verified = vr { return true }
    return false
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_RenewalInfo_GetPayloadPointer")
public func VerificationResult_RenewalInfo_GetPayloadPointer(pointer: UnsafeMutableRawPointer) -> UnsafeMutableRawPointer {
    let vr = pointer.assumingMemoryBound(to: VerificationResult<Product.SubscriptionInfo.RenewalInfo>.self).pointee
    let payload = vr.unsafePayloadValue
    let ptr = UnsafeMutablePointer<Product.SubscriptionInfo.RenewalInfo>.allocate(capacity: 1)
    ptr.initialize(to: payload)
    return ptr.getRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("VerificationResult_RenewalInfo_GetVerificationError")
public func VerificationResult_RenewalInfo_GetVerificationError(pointer: UnsafeMutableRawPointer) -> UnsafeMutableRawPointer? {
    let vr = pointer.assumingMemoryBound(to: VerificationResult<Product.SubscriptionInfo.RenewalInfo>.self).pointee
    if case .unverified(_, let error) = vr {
        return Unmanaged.passRetained(error as NSError).toOpaque()
    }
    return nil
}

// MARK: - RenewalInfo

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("RenewalInfo_Free")
public func RenewalInfo_Free(pointer: UnsafeMutableRawPointer) {
    let ptr = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.RenewalInfo.self)
    ptr.deinitialize(count: 1)
    ptr.deallocate()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("RenewalInfo_GetCurrentProductID")
public func RenewalInfo_GetCurrentProductID(pointer: UnsafeMutableRawPointer) -> char_p {
    let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.RenewalInfo.self).pointee
    return info.currentProductID.toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("RenewalInfo_GetWillAutoRenew")
public func RenewalInfo_GetWillAutoRenew(pointer: UnsafeMutableRawPointer) -> Bool {
    let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.RenewalInfo.self).pointee
    return info.willAutoRenew
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("RenewalInfo_GetOfferId")
public func RenewalInfo_GetOfferId(pointer: UnsafeMutableRawPointer) -> char_p? {
    let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.RenewalInfo.self).pointee
    guard let id = info.offerID else { return nil }
    return id.toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("RenewalInfo_GetOfferType")
public func RenewalInfo_GetOfferType(pointer: UnsafeMutableRawPointer) -> Int {
    let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.RenewalInfo.self).pointee
    guard let offerType = info.offerType else { return -1 }
    switch offerType {
    case .introductory: return 0
    case .promotional: return 1
    case .winBack: return 3
    default: return -1
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("RenewalInfo_GetExpirationReason")
public func RenewalInfo_GetExpirationReason(pointer: UnsafeMutableRawPointer) -> Int {
    let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.RenewalInfo.self).pointee
    guard let reason = info.expirationReason else { return -1 }
    switch reason {
    case .autoRenewDisabled: return 0
    case .billingError: return 1
    case .didNotConsentToPriceIncrease: return 2
    case .productUnavailable: return 3
    default: return -1
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("RenewalInfo_GetIsInBillingRetry")
public func RenewalInfo_GetIsInBillingRetry(pointer: UnsafeMutableRawPointer) -> Bool {
    let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.RenewalInfo.self).pointee
    return info.isInBillingRetry
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("RenewalInfo_GetGracePeriodExpirationDate")
public func RenewalInfo_GetGracePeriodExpirationDate(pointer: UnsafeMutableRawPointer) -> Int64 {
    let info = pointer.assumingMemoryBound(to: Product.SubscriptionInfo.RenewalInfo.self).pointee
    guard let date = info.gracePeriodExpirationDate else { return 0 }
    return Int64(date.timeIntervalSince1970)
}

// MARK: - SubscriptionOffer

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionOffer_Free")
public func SubscriptionOffer_Free(
    pointer: UnsafeMutableRawPointer
)
{
    let ptr = pointer.assumingMemoryBound(to: Product.SubscriptionOffer.self)
    ptr.deinitialize(count: 1)
    ptr.deallocate()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionOffer_GetId")
public func SubscriptionOffer_GetId(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let offer = pointer.assumingMemoryBound(to: Product.SubscriptionOffer.self).pointee
    guard let id = offer.id else { return nil }
    return id.toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionOffer_GetDisplayPrice")
public func SubscriptionOffer_GetDisplayPrice(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let offer = pointer.assumingMemoryBound(to: Product.SubscriptionOffer.self).pointee
    return offer.displayPrice.toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionOffer_GetPeriodUnit")
public func SubscriptionOffer_GetPeriodUnit(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let offer = pointer.assumingMemoryBound(to: Product.SubscriptionOffer.self).pointee
    switch offer.period.unit {
    case .day: return 0
    case .week: return 1
    case .month: return 2
    case .year: return 3
    @unknown default: return -1
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionOffer_GetPeriodValue")
public func SubscriptionOffer_GetPeriodValue(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let offer = pointer.assumingMemoryBound(to: Product.SubscriptionOffer.self).pointee
    return offer.period.value
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionOffer_GetPeriodCount")
public func SubscriptionOffer_GetPeriodCount(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let offer = pointer.assumingMemoryBound(to: Product.SubscriptionOffer.self).pointee
    return offer.periodCount
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("SubscriptionOffer_GetPaymentMode")
public func SubscriptionOffer_GetPaymentMode(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let offer = pointer.assumingMemoryBound(to: Product.SubscriptionOffer.self).pointee
    switch offer.paymentMode {
    case .freeTrial: return 0
    case .payAsYouGo: return 1
    case .payUpFront: return 2
    default: return -1
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_Free")
public func Product_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    let productPtr = pointer.assumingMemoryBound(to: Product.self);
    productPtr.deinitialize(count: 1);
    productPtr.deallocate();
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_FetchProducts")
public func Product_FetchProducts
(
    productIds: UnsafeMutablePointer<char_p>,
    productIdsCount: Int,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskArrayCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    // Convert array of char_p to Swift [String]
    var productIdsArray = [String]()
    for i in 0..<productIdsCount {
        let charPtr = productIds[i]
        productIdsArray.append(charPtr.toString())
    }

    Task {
        do {
            let products = try await Product.products(for: productIdsArray);
            let wrapped = products.map {
                let ptr = UnsafeMutablePointer<Product>.allocate(capacity: 1)
                ptr.initialize(to: $0)
                return ptr.getRawPointer()
            }
            let ptr = wrapped.toUnsafeMutablePointer()
            onSuccess(taskId, ptr, products.count);
        } catch {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer());
        }
    }
}

// MARK: - Product Purchasing

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("PurchaseResult_Free")
public func PurchaseResult_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    let purchaseResultPtr = pointer.assumingMemoryBound(to: Product.PurchaseResult.self);
    purchaseResultPtr.deinitialize(count: 1);
    purchaseResultPtr.deallocate();
}

// MARK: - PurchaseResult Functions

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("PurchaseResult_GetResultType")
public func PurchaseResult_GetResultType(ptr: UnsafeMutableRawPointer) -> Int
{
    let purchaseResult = ptr.assumingMemoryBound(to: Product.PurchaseResult.self).pointee
    switch purchaseResult {
    case .success:
        return 0
    case .userCancelled:
        return 1
    case .pending:
        return 2
    @unknown default:
        return -1
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("PurchaseResult_GetVerificationResult")
public func PurchaseResult_GetVerificationResult(ptr: UnsafeMutableRawPointer) -> UnsafeMutableRawPointer?
{
    let purchaseResult = ptr.assumingMemoryBound(to: Product.PurchaseResult.self).pointee
    switch purchaseResult {
    case .success(let verificationResult):
        let ptr = UnsafeMutablePointer<VerificationResult<Transaction>>.allocate(capacity: 1)
        ptr.initialize(to: verificationResult)
        return ptr.getRawPointer()
    default:
        return nil
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_Purchase")
public func Product_Purchase
(
    pointer: UnsafeMutableRawPointer,
    optionsPointer: UnsafeMutablePointer<UnsafeMutableRawPointer>?,
    optionsCount: Int,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskRawPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee;
    Task
    {
        do
        {
            var options: [Product.PurchaseOption] = []
            if let optionsPointer = optionsPointer, optionsCount > 0 {
                for i in 0..<optionsCount {
                    let optionPtr = optionsPointer[i]
                    let option = optionPtr.assumingMemoryBound(to: Product.PurchaseOption.self).pointee
                    options.append(option)
                }
            }

            let result = try await Product_Purchase_OSSpecific(product: product, options: Set(options))
            guard let result else {
                onSuccess(taskId, nil as UnsafeMutableRawPointer?)
                return
            }

            let ptr = UnsafeMutablePointer<Product.PurchaseResult>.allocate(capacity: 1)
            ptr.initialize(to: result)
            onSuccess(taskId, ptr.getRawPointer())

        } catch {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer());
        }
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
func Product_Purchase_OSSpecific
(
    product: Product,
    options: Set<Product.PurchaseOption>
) async throws -> Product.PurchaseResult?
{
    #if os(visionOS)
    // It's difficult to get anything other than UIWindow for the running process. Prior to 2.2+
    // this was either not avaiable or required a UIView which is not available in Unity
    guard let window = UiUtilities.rootViewController() else {
        throw NSError(domain:"Apple.StoreKit.Unity", code: 0, userInfo: nil)
    }
    return try await product.purchase(confirmIn: window, options: options)
    
    #elseif os(macOS)
    // macOS supports purchasing with an NSWindow or without. Attempt to get one
    // and fall back to purchasing without. This should result in the right behavior regardless
    guard #available(macOS 15.2, *), let window = UiUtilities.defaultWindow() else {
        return try await product.purchase(options: options)
    }
    return try await product.purchase(confirmIn: window, options: options)
    #else
    // Similar to macOS, iOS supports using a UIWindow for the running process on 18.2+,
    // otherwise use the fallback with no confirmIn
    guard #available(iOS 18.2, tvOS 18.2, *), let window = UiUtilities.rootViewController()  else {
        return try await product.purchase(options: options)
    }
    return try await product.purchase(confirmIn: window, options: options)
    #endif
}

// MARK: - Product PurchaseOption

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("PurchaseOption_Free")
public func PurchaseOption_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    let purchaseOptionPtr = pointer.assumingMemoryBound(to: Product.PurchaseOption.self);
    purchaseOptionPtr.deinitialize(count: 1);
    purchaseOptionPtr.deallocate();
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("PurchaseOption_AppAccountToken")
public func PurchaseOption_AppAccountToken(
    uuidString: char_p
) -> UnsafeMutableRawPointer?
{
    let uuid = UUID(uuidString: uuidString.toString())
    guard uuid != nil else {
        return nil
    }

    let option = Product.PurchaseOption.appAccountToken(uuid!)
    let ptr = UnsafeMutablePointer<Product.PurchaseOption>.allocate(capacity: 1)
    ptr.initialize(to: option)
    return ptr.getRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("PurchaseOption_Quantity")
public func PurchaseOption_Quantity
(
    quantity: Int64
) -> UnsafeMutableRawPointer
{
    let option = Product.PurchaseOption.quantity(Int(quantity))
    let ptr = UnsafeMutablePointer<Product.PurchaseOption>.allocate(capacity: 1)
    ptr.initialize(to: option)
    return ptr.getRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("PurchaseOption_SimulatesAskToBuyInSandbox")
public func PurchaseOption_SimulatesAskToBuyInSandbox(
    value: Bool
) -> UnsafeMutableRawPointer
{
    let option = Product.PurchaseOption.simulatesAskToBuyInSandbox(value)
    let ptr = UnsafeMutablePointer<Product.PurchaseOption>.allocate(capacity: 1)
    ptr.initialize(to: option)
    return ptr.getRawPointer()
}

@available(iOS 18.0, macOS 15.0, tvOS 18.0, visionOS 2.0, *)
@_cdecl("PurchaseOption_WinBackOffer")
public func PurchaseOption_WinBackOffer(
    offerPointer: UnsafeMutableRawPointer
) -> UnsafeMutableRawPointer
{
    let offer = offerPointer.assumingMemoryBound(to: Product.SubscriptionOffer.self).pointee
    let option = Product.PurchaseOption.winBackOffer(offer)
    let ptr = UnsafeMutablePointer<Product.PurchaseOption>.allocate(capacity: 1)
    ptr.initialize(to: option)
    return ptr.getRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("PurchaseOption_CustomString")
public func PurchaseOption_CustomString(
    key: char_p,
    value: char_p
) -> UnsafeMutableRawPointer
{
    let option = Product.PurchaseOption.custom(key: key.toString(), value: value.toString())
    let ptr = UnsafeMutablePointer<Product.PurchaseOption>.allocate(capacity: 1)
    ptr.initialize(to: option)
    return ptr.getRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("PurchaseOption_CustomDouble")
public func PurchaseOption_CustomDouble(
    key: char_p,
    value: Double
) -> UnsafeMutableRawPointer
{
    let option = Product.PurchaseOption.custom(key: key.toString(), value: value)
    let ptr = UnsafeMutablePointer<Product.PurchaseOption>.allocate(capacity: 1)
    ptr.initialize(to: option)
    return ptr.getRawPointer()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("PurchaseOption_CustomBool")
public func PurchaseOption_CustomBool(
    key: char_p,
    value: Bool
) -> UnsafeMutableRawPointer
{
    let option = Product.PurchaseOption.custom(key: key.toString(), value: value)
    let ptr = UnsafeMutablePointer<Product.PurchaseOption>.allocate(capacity: 1)
    ptr.initialize(to: option)
    return ptr.getRawPointer()
}

// MARK: - Product Latest Transaction

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("Product_GetLatestTransaction")
public func Product_GetLatestTransaction(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskRawPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let product = pointer.assumingMemoryBound(to: Product.self).pointee
    Task {
        guard let verificationResult = await product.latestTransaction else {
            onSuccess(taskId, nil)
            return
        }
        let ptr = UnsafeMutablePointer<VerificationResult<Transaction>>.allocate(capacity: 1)
        ptr.initialize(to: verificationResult)
        onSuccess(taskId, ptr.getRawPointer())
    }
}

// MARK: - Product.PromotionInfo

@available(iOS 16.4, *)
@available(tvOS, unavailable)
@available(macOS, unavailable)
@available(visionOS, unavailable)
@_cdecl("ProductPromotionInfo_GetCurrentOrder")
public func ProductPromotionInfo_GetCurrentOrder(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskArrayCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    Task {
        do {
            let order = try await Product.PromotionInfo.currentOrder
            let strings = order.map { UnsafeMutableRawPointer($0.productID.toCharPCopy()) }
            let ptr = strings.toUnsafeMutablePointer()
            onSuccess(taskId, ptr, strings.count)
        } catch {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
        }
    }
}

@available(iOS 16.4, *)
@available(tvOS, unavailable)
@available(macOS, unavailable)
@available(visionOS, unavailable)
@_cdecl("ProductPromotionInfo_UpdateProductOrder")
public func ProductPromotionInfo_UpdateProductOrder(
    productIds: UnsafeMutablePointer<char_p>,
    productIdsCount: Int,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    var ids = [String]()
    for i in 0..<productIdsCount {
        ids.append(productIds[i].toString())
    }
    Task {
        do {
            try await Product.PromotionInfo.updateProductOrder(byID: ids)
            onSuccess(taskId)
        } catch {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
        }
    }
}

@available(iOS 16.4, *)
@available(tvOS, unavailable)
@available(macOS, unavailable)
@available(visionOS, unavailable)
@_cdecl("ProductPromotionInfo_UpdateProductVisibility")
public func ProductPromotionInfo_UpdateProductVisibility(
    productId: char_p,
    visibility: Int,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let id = productId.toString()
    let vis: Product.PromotionInfo.Visibility
    switch visibility {
    case 1: vis = .visible
    case 2: vis = .hidden
    default: vis = .appStoreConnectDefault
    }
    Task {
        do {
            try await Product.PromotionInfo.updateProductVisibility(vis, for: id)
            onSuccess(taskId)
        } catch {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
        }
    }
}

// MARK: - Product.PromotionInfo.updateAll

@available(iOS 16.4, *)
@available(tvOS, unavailable)
@available(macOS, unavailable)
@available(visionOS, unavailable)
@_cdecl("ProductPromotionInfo_UpdateAll")
public func ProductPromotionInfo_UpdateAll(
    productIds: UnsafeMutablePointer<UnsafeMutablePointer<Int8>>,
    visibilities: UnsafeMutablePointer<Int32>,
    count: Int,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
) {
    var ids: [String] = []
    var visMap: [(String, Product.PromotionInfo.Visibility)] = []
    for i in 0..<count {
        let id = String(cString: productIds[i])
        ids.append(id)
        let vis: Product.PromotionInfo.Visibility
        switch visibilities[i] {
        case 1: vis = .visible
        case 2: vis = .hidden
        default: vis = .appStoreConnectDefault
        }
        visMap.append((id, vis))
    }
    Task {
        do {
            try await Product.PromotionInfo.updateProductOrder(byID: ids)
            for (id, vis) in visMap {
                try await Product.PromotionInfo.updateProductVisibility(vis, for: id)
            }
            onSuccess(taskId)
        } catch {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
        }
    }
}
