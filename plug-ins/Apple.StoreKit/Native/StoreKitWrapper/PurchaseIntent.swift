//
//  PurchaseIntent.swift
//  StoreKitWrapper
//

import StoreKit

// PurchaseIntent fires when a user taps "Buy" on a promoted IAP in the App Store.
// Available iOS 16.4+ and macOS 14.4+.

#if os(iOS) || os(macOS)

@available(iOS 16.4, macOS 14.4, *)
@_cdecl("PurchaseIntent_Listen")
public func PurchaseIntent_Listen(
    taskId: Int64,
    onIntent: @escaping SuccessTaskBoolReturningCallback
) {
    Task {
        for await intent in PurchaseIntent.intents {
            let ptr = UnsafeMutablePointer<PurchaseIntent>.allocate(capacity: 1)
            ptr.initialize(to: intent)
            let shouldContinue = onIntent(taskId, ptr.getRawPointer())
            if !shouldContinue { return }
        }
    }
}

@available(iOS 16.4, macOS 14.4, *)
@_cdecl("PurchaseIntent_Free")
public func PurchaseIntent_Free(pointer: UnsafeMutableRawPointer) {
    let ptr = pointer.assumingMemoryBound(to: PurchaseIntent.self)
    ptr.deinitialize(count: 1)
    ptr.deallocate()
}

@available(iOS 16.4, macOS 14.4, *)
@_cdecl("PurchaseIntent_GetId")
public func PurchaseIntent_GetId(pointer: UnsafeMutableRawPointer) -> char_p {
    let intent = pointer.assumingMemoryBound(to: PurchaseIntent.self).pointee
    return intent.id.toCharPCopy()
}

@available(iOS 16.4, macOS 14.4, *)
@_cdecl("PurchaseIntent_GetProduct")
public func PurchaseIntent_GetProduct(pointer: UnsafeMutableRawPointer) -> UnsafeMutableRawPointer {
    let intent = pointer.assumingMemoryBound(to: PurchaseIntent.self).pointee
    let ptr = UnsafeMutablePointer<Product>.allocate(capacity: 1)
    ptr.initialize(to: intent.product)
    return ptr.getRawPointer()
}

#endif
