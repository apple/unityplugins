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
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let target = Unmanaged<GKAchievementDescription>.fromOpaque(pointer).takeUnretainedValue();
    return target.identifier.toCharPCopy();
}

@_cdecl("GKAchievementDescription_GetTitle")
public func GKAchievementDescription_GetTitle
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let target = Unmanaged<GKAchievementDescription>.fromOpaque(pointer).takeUnretainedValue();
    return target.title.toCharPCopy();
}

@_cdecl("GKAchievementDescription_GetUnachievedDescription")
public func GKAchievementDescription_GetUnachievedDescription
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let target = Unmanaged<GKAchievementDescription>.fromOpaque(pointer).takeUnretainedValue();
    return target.unachievedDescription.toCharPCopy();
}

@_cdecl("GKAchievementDescription_GetAchievedDescription")
public func GKAchievementDescription_GetAchievedDescription
(
    pointer: UnsafeMutableRawPointer
) -> char_p
{
    let target = Unmanaged<GKAchievementDescription>.fromOpaque(pointer).takeUnretainedValue();
    return target.achievedDescription.toCharPCopy();
}

@_cdecl("GKAchievementDescription_GetGroupIdentifier")
public func GKAchievementDescription_GetGroupIdentifier
(
    pointer: UnsafeMutableRawPointer
) -> char_p?
{
    let target = Unmanaged<GKAchievementDescription>.fromOpaque(pointer).takeUnretainedValue();
    return target.groupIdentifier?.toCharPCopy();
}

@_cdecl("GKAchievementDescription_GetMaximumPoints")
public func GKAchievementDescription_GetMaximumPoints
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKAchievementDescription>.fromOpaque(pointer).takeUnretainedValue();
    return target.maximumPoints;
}

@_cdecl("GKAchievementDescription_GetIsHidden")
public func GKAchievementDescription_GetIsHidden
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let target = Unmanaged<GKAchievementDescription>.fromOpaque(pointer).takeUnretainedValue();
    return target.isHidden;
}

@_cdecl("GKAchievementDescription_GetIsReplayable")
public func GKAchievementDescription_GetIsReplayable
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let target = Unmanaged<GKAchievementDescription>.fromOpaque(pointer).takeUnretainedValue();
    return target.isReplayable;
}

@_cdecl("GKAchievementDescription_GetRarityPercent")
public func GKAchievementDescription_GetRarityPercent
(
    pointer: UnsafeMutableRawPointer
) -> Double
{
    if #available(macOS 14.0, iOS 17.0, tvOS 17.0, *) {
        let target = Unmanaged<GKAchievementDescription>.fromOpaque(pointer).takeUnretainedValue()
        return target.rarityPercent ?? 0.0
    }
    else {
        return 0.0
    }
}

@_cdecl("GKAchievementDescription_LoadAchievementDescriptions")
public func GKAchievementDescription_LoadAchievementDescriptions
(
    taskId: Int64,
    onSuccess: @escaping SuccessTaskPtrCallback,
    onError: @escaping NSErrorCallback
)
{
    GKAchievementDescription.loadAchievementDescriptions(completionHandler: {descriptions, error in
        if (error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }
        
        if (descriptions != nil) {
            onSuccess(taskId, Unmanaged.passRetained(descriptions! as NSArray).toOpaque());
        } else {
            onSuccess(taskId, nil);
        }
    });
}

@_cdecl("GKAchievementDescription_LoadImage")
public func GKAchievementDescription_LoadImage
(
    pointer: UnsafeMutableRawPointer,
    taskId: Int64,
    onImageLoaded: @escaping SuccessTaskImageCallback,
    onError: @escaping NSErrorCallback
)
{
    let target = Unmanaged<GKAchievementDescription>.fromOpaque(pointer).takeUnretainedValue();
    target.loadImage(completionHandler: {image, error in
        if (error != nil) {
            onError(taskId, Unmanaged.passRetained(error! as NSError).toOpaque());
            return;
        }

        let data = image!.pngData()!;
        onImageLoaded(
            taskId,
            Unmanaged.passRetained(data as NSData).toOpaque());
    });
}

@_cdecl("GKAchievementDescription_GetIncompleteAchievementImage")
public func GKAchievementDescription_GetIncompleteAchievementImage
(
) -> UnsafeMutableRawPointer?
{
    let image = GKAchievementDescription.incompleteAchievementImage()

    guard let data = image.pngData()
    else {
        return nil;
    }

    return Unmanaged.passRetained(data as NSData).toOpaque();
}

@_cdecl("GKAchievementDescription_GetPlaceholderCompletedAchievementImage")
public func GKAchievementDescription_GetPlaceholderCompletedAchievementImage
(
) -> UnsafeMutableRawPointer?
{
    let image = GKAchievementDescription.placeholderCompletedAchievementImage()

    guard let data = image.pngData()
    else {
        return nil;
    }

    return Unmanaged.passRetained(data as NSData).toOpaque();
}

