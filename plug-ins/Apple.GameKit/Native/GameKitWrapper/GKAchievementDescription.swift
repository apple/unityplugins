//
//  GKAchievementDescription.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKAchievementDescription_GetIdentifier")
public func GKAchievementDescription_GetIdentifier
(
    pointer: UnsafeMutablePointer<GKAchievementDescription>
) -> char_p
{
    let target = pointer.takeUnretainedValue();
    return target.identifier.toCharPCopy();
}

@_cdecl("GKAchievementDescription_GetTitle")
public func GKAchievementDescription_GetTitle
(
    pointer: UnsafeMutablePointer<GKAchievementDescription>
) -> char_p
{
    let target = pointer.takeUnretainedValue();
    return target.title.toCharPCopy();
}

@_cdecl("GKAchievementDescription_GetUnachievedDescription")
public func GKAchievementDescription_GetUnachievedDescription
(
    pointer: UnsafeMutablePointer<GKAchievementDescription>
) -> char_p
{
    let target = pointer.takeUnretainedValue();
    return target.unachievedDescription.toCharPCopy();
}

@_cdecl("GKAchievementDescription_GetAchievedDescription")
public func GKAchievementDescription_GetAchievedDescription
(
    pointer: UnsafeMutablePointer<GKAchievementDescription>
) -> char_p
{
    let target = pointer.takeUnretainedValue();
    return target.achievedDescription.toCharPCopy();
}

@_cdecl("GKAchievementDescription_GetGroupIdentifier")
public func GKAchievementDescription_GetGroupIdentifier
(
    pointer: UnsafeMutablePointer<GKAchievementDescription>
) -> char_p?
{
    let target = pointer.takeUnretainedValue();
    return target.groupIdentifier?.toCharPCopy();
}

@_cdecl("GKAchievementDescription_GetMaximumPoints")
public func GKAchievementDescription_GetMaximumPoints
(
    pointer: UnsafeMutablePointer<GKAchievementDescription>
) -> Int
{
    let target = pointer.takeUnretainedValue();
    return target.maximumPoints;
}

@_cdecl("GKAchievementDescription_GetIsHidden")
public func GKAchievementDescription_GetIsHidden
(
    pointer: UnsafeMutablePointer<GKAchievementDescription>
) -> Bool
{
    let target = pointer.takeUnretainedValue();
    return target.isHidden;
}

@_cdecl("GKAchievementDescription_GetIsReplayable")
public func GKAchievementDescription_GetIsReplayable
(
    pointer: UnsafeMutablePointer<GKAchievementDescription>
) -> Bool
{
    let target = pointer.takeUnretainedValue();
    return target.isReplayable;
}

@_cdecl("GKAchievementDescription_GetRarityPercent")
public func GKAchievementDescription_GetRarityPercent
(
    pointer: UnsafeMutablePointer<GKAchievementDescription>
) -> Double
{
    if #available(iOS 17.0, macOS 14.0, tvOS 17.0, *) {
        let target = pointer.takeUnretainedValue()
        return target.rarityPercent ?? 0.0
    } else {
        DefaultNSErrorHandler.throwApiUnavailableError();
    }
}

@_cdecl("GKAchievementDescription_LoadAchievementDescriptions")
public func GKAchievementDescription_LoadAchievementDescriptions
(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    GKAchievementDescription.loadAchievementDescriptions(completionHandler: {descriptions, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        onSuccess(taskId, (descriptions as? NSArray)?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKAchievementDescription_LoadImage")
public func GKAchievementDescription_LoadImage
(
    pointer: UnsafeMutablePointer<GKAchievementDescription>,
    taskId: Int64,
    onImageLoaded: @escaping SuccessTaskImageCallback,
    onError: @escaping NSErrorTaskCallback
)
{
    let target = pointer.takeUnretainedValue();
    target.loadImage(completionHandler: {image, error in
        if let error = error as? NSError {
            onError(taskId, error.passRetainedUnsafeMutablePointer());
            return;
        }

        // On older OS versions, loadImage can return nil if the image can't be loaded.
        // Newer OS versions return GKErrorUnknown if the image can't be loaded.
        let data = image?.pngData() as? NSData;
        onImageLoaded(taskId, data?.passRetainedUnsafeMutablePointer());
    });
}

@_cdecl("GKAchievementDescription_GetIncompleteAchievementImage")
public func GKAchievementDescription_GetIncompleteAchievementImage
(
) -> UnsafeMutablePointer<NSData>?
{
    let image = GKAchievementDescription.incompleteAchievementImage()
    return (image.pngData() as? NSData)?.passRetainedUnsafeMutablePointer();
}

@_cdecl("GKAchievementDescription_GetPlaceholderCompletedAchievementImage")
public func GKAchievementDescription_GetPlaceholderCompletedAchievementImage
(
) -> UnsafeMutablePointer<NSData>?
{
    let image = GKAchievementDescription.placeholderCompletedAchievementImage()
    return (image.pngData() as? NSData)?.passRetainedUnsafeMutablePointer();
}

