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

