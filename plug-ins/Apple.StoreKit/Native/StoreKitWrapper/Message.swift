//
//  Message.swift
//  StoreKitWrapper
//
//  Created by Andrew Hall on 1/23/26.
//  Copyright © 2026 Apple, Inc. All rights reserved.
//

#if os(iOS) || os(visionOS)

import StoreKit

@available(iOS 16.0, visionOS 2.2, *)
@_cdecl("Message_Free")
public func Message_Free
(
    pointer: UnsafeMutableRawPointer
)
{
    let messagePtr = pointer.assumingMemoryBound(to: Message.self);
    messagePtr.deinitialize(count: 1);
    messagePtr.deallocate();
}

@available(iOS 16.0, visionOS 2.2, *)
@_cdecl("Message_Updates")
public func Message_Updates
(
    taskId: Int64,
    onAdd: @escaping SuccessTaskBoolReturningCallback
)
{
    Task {
        for await message in Message.messages {
            let ptr = UnsafeMutablePointer<Message>.allocate(capacity: 1)
            ptr.initialize(to: message)
            let shouldContinue = onAdd(taskId, ptr.getRawPointer())
            if (!shouldContinue)
            {
                return
            }
        }
    }
}

@available(iOS 16.0, visionOS 2.2, *)
@_cdecl("Message_Reason")
public func Message_Reason(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let message = pointer.assumingMemoryBound(to: Message.self).pointee
    let reason = message.reason

    if (reason == .generic) {
        return 0
    }

    if #available(iOS 16.4, *), reason == .billingIssue {
        return 1
    }

    if (reason == .priceIncreaseConsent) {
        return 2
    }

    if #available(iOS 18.0, *), reason == .winBackOffer {
        return 3
    }

    return -1
}

@available(iOS 16.0, visionOS 2.2, *)
@_cdecl("Message_Display")
public func Message_Display(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onSuccess: @escaping SuccessTaskCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    Task
    {
        do
        {
            guard let windowScene = await UiUtilities.defaultWindow()?.windowScene else {
                onError(taskId, NSError(domain: "com.apple.swift.storekit", code: 0, userInfo: [NSLocalizedDescriptionKey: "No window scene available."]).passRetainedUnsafeMutableRawPointer())
                return
            }

            let message = pointer.assumingMemoryBound(to: Message.self).pointee
            try? await message.display(in: windowScene)
            onSuccess(taskId)
        }
        catch
        {
            onError(taskId, (error as NSError).passRetainedUnsafeMutableRawPointer())
        }
    }
}
#endif
