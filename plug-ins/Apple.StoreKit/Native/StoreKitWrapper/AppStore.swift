//
//  AppStore.swift
//  StoreKitWrapper
//
//  Created by Andrew Hall on 1/23/26.
//  Copyright © 2026 Apple, Inc. All rights reserved.
//

import StoreKit

#if !os(tvOS)
@available(iOS 16.0, macOS 15.0, visionOS 2.2, *)
@_cdecl("AppStore_PresentOfferCodeRedeemSheet")
public func AppStore_PresentOfferCodeRedeemSheet(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    #if os(macOS)
    guard let viewController = UiUtilities.rootViewController() else {
        onError(taskId, NSError(domain:"Apple.StoreKit.Unity", code: 0, userInfo: nil).passRetainedUnsafeMutableRawPointer())
        return
    }
    Task
    {
        do
        {
            try await AppStore.presentOfferCodeRedeemSheet(from: viewController)
            onSuccess(taskId)
        }
        catch
        {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
        }
    }
    #else
    guard let scene = UiUtilities.defaultWindow()?.windowScene else {
        onError(taskId, NSError(domain:"Apple.StoreKit.Unity", code: 0, userInfo: nil).passRetainedUnsafeMutableRawPointer())
        return
    }

    Task
    {
        do
        {
            try await AppStore.presentOfferCodeRedeemSheet(in: scene)
            onSuccess(taskId)
        }
        catch
        {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
        }
    }
    #endif
}
#endif


#if os(iOS) || os(visionOS)
@available(iOS 17.0, visionOS 2.2, *)
@_cdecl("AppStore_ShowManageSubscriptions")
public func AppStore_ShowManageSubscriptions(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    Task {
        do {
            guard let scene = await UiUtilities.defaultWindow()?.windowScene else {
                onError(taskId, NSError(domain: "StoreKitWrapper", code: 0, userInfo: [NSLocalizedDescriptionKey: "No scene available"]).passRetainedUnsafeMutableRawPointer())
                return
            }
            try await AppStore.showManageSubscriptions(in: scene)
            onSuccess(taskId)
        } catch {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
        }
    }
}

@available(iOS 17.0, visionOS 2.2, *)
@_cdecl("AppStore_ShowManageSubscriptionsForGroup")
public func AppStore_ShowManageSubscriptionsForGroup(
    subscriptionGroupId: char_p,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let groupId = subscriptionGroupId.toString()
    Task {
        do {
            guard let scene = await UiUtilities.defaultWindow()?.windowScene else {
                onError(taskId, NSError(domain: "StoreKitWrapper", code: 0, userInfo: [NSLocalizedDescriptionKey: "No scene available"]).passRetainedUnsafeMutableRawPointer())
                return
            }
            try await AppStore.showManageSubscriptions(in: scene, subscriptionGroupID: groupId)
            onSuccess(taskId)
        } catch {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
        }
    }
}

#endif

@available(iOS 16.0, macOS 15.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("AppStore_CanMakePayments")
public func AppStore_CanMakePayments() -> Bool
{
    return AppStore.canMakePayments
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 2.2, *)
@_cdecl("AppStore_Sync")
public func AppStore_Sync(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    Task {
        do {
            try await AppStore.sync()
            onSuccess(taskId)
        } catch {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
        }
    }
}

@available(iOS 16.0, macOS 13.0, visionOS 2.2, *)
@_cdecl("AppStore_RequestReview")
public func AppStore_RequestReview()
{
#if !os(tvOS)
    Task { @MainActor in
        #if os(macOS)
        guard let viewController = UiUtilities.rootViewController() else { return }
        AppStore.requestReview(in: viewController)
        #else
        guard let scene = UiUtilities.defaultWindow()?.windowScene else { return }
        AppStore.requestReview(in: scene)
        #endif
    }
#endif
}

@available(iOS 16.4, macOS 13.3, tvOS 16.4, visionOS 1.0, *)
@_cdecl("AppStore_GetDeviceVerificationID")
public func AppStore_GetDeviceVerificationID() -> char_p?
{
    return AppStore.deviceVerificationID?.uuidString.toCharPCopy()
}

// PaymentMethodBinding (iOS only). Lives here rather than a new file so it's part of an
// Xcode target already in the project (no pbxproj changes needed).
#if os(iOS)
@available(iOS 16.4, *)
@_cdecl("PaymentMethodBinding_Bind")
public func PaymentMethodBinding_Bind(
    inAppPinningId: char_p,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let pinningId = inAppPinningId.toString()
    Task {
        do {
            let binding = try await PaymentMethodBinding(id: pinningId)
            try await binding.bind()
            onSuccess(taskId)
        } catch {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
        }
    }
}
#endif

// Age rating code (AppStore.ageRatingCode → Int?). iOS 26.2+; returns -1 when nil/unsupported.
@available(iOS 16.0, macOS 13.0, tvOS 16.0, visionOS 2.2, *)
@_cdecl("AppStore_GetAgeRatingCode")
public func AppStore_GetAgeRatingCode(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskIntCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 26.2, macOS 26.2, tvOS 26.2, visionOS 26.2, *) {
        Task {
            let code = await AppStore.ageRatingCode
            onSuccess(taskId, code ?? -1)
        }
    } else {
        onError(taskId, NSError(domain: "StoreKitWrapper", code: -1, userInfo: [NSLocalizedDescriptionKey: "ageRatingCode requires iOS 26.2+"]).passRetainedUnsafeMutableRawPointer())
    }
}

