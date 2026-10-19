//
//  AdvancedCommerceProduct.swift
//  StoreKitWrapper
//

import Foundation
import StoreKit

// MARK: - AdvancedCommerceProduct

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 1.0, *)
@_cdecl("AdvancedCommerceProduct_Free")
public func AdvancedCommerceProduct_Free(
    pointer: UnsafeMutableRawPointer
)
{
    if #available(iOS 18.4, macOS 15.4, tvOS 18.4, visionOS 2.4, *) {
        let ptr = pointer.assumingMemoryBound(to: AdvancedCommerceProduct.self)
        ptr.deinitialize(count: 1)
        ptr.deallocate()
    }
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 1.0, *)
@_cdecl("AdvancedCommerceProduct_GetId")
public func AdvancedCommerceProduct_GetId(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    if #available(iOS 18.4, macOS 15.4, tvOS 18.4, visionOS 2.4, *) {
        let product = pointer.assumingMemoryBound(to: AdvancedCommerceProduct.self).pointee
        return product.id.toCharPCopy()
    }
    return "".toCharPCopy()
}

@available(iOS 15.0, macOS 12.0, tvOS 15.0, visionOS 1.0, *)
@_cdecl("AdvancedCommerceProduct_Load")
public func AdvancedCommerceProduct_Load(
    productId: char_p,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskRawPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 18.4, macOS 15.4, tvOS 18.4, visionOS 2.4, *) {
        Task {
            do {
                let product = try await AdvancedCommerceProduct(id: productId.toString())
                let ptr = UnsafeMutablePointer<AdvancedCommerceProduct>.allocate(capacity: 1)
                ptr.initialize(to: product)
                onSuccess(taskId, ptr.getRawPointer())
            } catch {
                let nsError = error as NSError
                let errorPtr = UnsafeMutablePointer<NSError>.allocate(capacity: 1)
                errorPtr.initialize(to: nsError)
                onError(taskId, errorPtr.getRawPointer())
            }
        }
    } else {
        let error = NSError(domain: "StoreKitWrapper", code: -1, userInfo: [NSLocalizedDescriptionKey: "AdvancedCommerceProduct requires iOS 18.4+"])
        let errorPtr = UnsafeMutablePointer<NSError>.allocate(capacity: 1)
        errorPtr.initialize(to: error)
        onError(taskId, errorPtr.getRawPointer())
    }
}

#if !os(macOS)
@available(iOS 15.0, tvOS 15.0, visionOS 1.0, *)
@_cdecl("AdvancedCommerceProduct_Purchase")
public func AdvancedCommerceProduct_Purchase(
    pointer: UnsafeMutableRawPointer,
    compactJWS: char_p,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskRawPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    if #available(iOS 18.4, tvOS 18.4, visionOS 2.4, *) {
        let product = pointer.assumingMemoryBound(to: AdvancedCommerceProduct.self).pointee
        guard let viewController = UiUtilities.rootViewController() else {
            let error = NSError(domain: "StoreKitWrapper", code: -1, userInfo: [NSLocalizedDescriptionKey: "No root view controller available"])
            let errorPtr = UnsafeMutablePointer<NSError>.allocate(capacity: 1)
            errorPtr.initialize(to: error)
            onError(taskId, errorPtr.getRawPointer())
            return
        }
        Task {
            do {
                let result = try await product.purchase(compactJWS: compactJWS.toString(), confirmIn: viewController)
                let resultPtr = UnsafeMutablePointer<Product.PurchaseResult>.allocate(capacity: 1)
                resultPtr.initialize(to: result)
                onSuccess(taskId, resultPtr.getRawPointer())
            } catch {
                let nsError = error as NSError
                let errorPtr = UnsafeMutablePointer<NSError>.allocate(capacity: 1)
                errorPtr.initialize(to: nsError)
                onError(taskId, errorPtr.getRawPointer())
            }
        }
    } else {
        let error = NSError(domain: "StoreKitWrapper", code: -1, userInfo: [NSLocalizedDescriptionKey: "AdvancedCommerceProduct requires iOS 18.4+"])
        let errorPtr = UnsafeMutablePointer<NSError>.allocate(capacity: 1)
        errorPtr.initialize(to: error)
        onError(taskId, errorPtr.getRawPointer())
    }
}
#endif
