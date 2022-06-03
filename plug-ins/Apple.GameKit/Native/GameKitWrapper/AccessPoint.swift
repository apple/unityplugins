//
//  AccessPoint.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKAccessPoint_GetShared")
public func GKAccessPoint_GetShared
(
) -> UnsafeMutableRawPointer
{
    return Unmanaged.passRetained(GKAccessPoint.shared).toOpaque();
}

@_cdecl("GKAccessPoint_Trigger")
public func GKAccessPoint_Trigger
(
    pointer: UnsafeMutableRawPointer
)
{
    let target = Unmanaged<GKAccessPoint>.fromOpaque(pointer).takeUnretainedValue();
    
    if(!target.isPresentingGameCenter) {
        target.trigger(handler: {});
    }
}

@_cdecl("GKAccessPoint_GetLocation")
public func GKAccessPoint_GetLocation
(
    pointer: UnsafeMutableRawPointer
) -> Int
{
    let target = Unmanaged<GKAccessPoint>.fromOpaque(pointer).takeUnretainedValue();
    return target.location.rawValue;
}

@_cdecl("GKAccessPoint_SetLocation")
public func GKAccessPoint_SetLocation
(
    pointer: UnsafeMutableRawPointer,
    value: Int
)
{
    let target = Unmanaged<GKAccessPoint>.fromOpaque(pointer).takeUnretainedValue();
    target.location = GKAccessPoint.Location.init(rawValue: value)!;
}

@_cdecl("GKAccessPoint_GetShowHighlights")
public func GKAccessPoint_GetShowHighlights
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let target = Unmanaged<GKAccessPoint>.fromOpaque(pointer).takeUnretainedValue();
    return target.showHighlights;
}

@_cdecl("GKAccessPoint_SetShowHighlights")
public func GKAccessPoint_SetShowHighlights
(
    pointer: UnsafeMutableRawPointer,
    value: Bool
)
{
    let target = Unmanaged<GKAccessPoint>.fromOpaque(pointer).takeUnretainedValue();
    target.showHighlights = value;
}

@_cdecl("GKAccessPoint_GetIsPresentingGameCenter")
public func GKAccessPoint_GetIsPresentingGameCenter
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let target = Unmanaged<GKAccessPoint>.fromOpaque(pointer).takeUnretainedValue();
    return target.isPresentingGameCenter;
}

@_cdecl("GKAccessPoint_GetIsFocused")
public func GKAccessPoint_GetIsFocused
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    #if os(tvOS)
    let target = Unmanaged<GKAccessPoint>.fromOpaque(pointer).takeUnretainedValue();
    return target.isFocused;
    #else
    return false;
    #endif
}

@_cdecl("GKAccessPoint_GetIsVisible")
public func GKAccessPoint_GetIsVisible
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let target = Unmanaged<GKAccessPoint>.fromOpaque(pointer).takeUnretainedValue();
    return target.isVisible;
}

@_cdecl("GKAccessPoint_GetIsActive")
public func GKAccessPoint_GetIsActive
(
    pointer: UnsafeMutableRawPointer
) -> Bool
{
    let target = Unmanaged<GKAccessPoint>.fromOpaque(pointer).takeUnretainedValue();
    return target.isActive;
}

@_cdecl("GKAccessPoint_SetIsActive")
public func GKAccessPoint_SetIsActive
(
    pointer: UnsafeMutableRawPointer,
    value: Bool
)
{
    let target = Unmanaged<GKAccessPoint>.fromOpaque(pointer).takeUnretainedValue();
    target.isActive = value;
}

@_cdecl("GKAccessPoint_GetFrameInScreenCoordinates")
public func GKAccessPoint_GetFrameInScreenCoordinates
(
    pointer: UnsafeMutableRawPointer
) -> GKWAccessPointFrameInScreenCoordinates
{
    let target = Unmanaged<GKAccessPoint>.fromOpaque(pointer).takeUnretainedValue();
    let rect = target.frameInScreenCoordinates;
    
    return GKWAccessPointFrameInScreenCoordinates(
        x: Float(rect.minX),
        y: Float(rect.minY),
        width: Float(rect.width),
        height: Float(rect.height));
}

@_cdecl("GKAccessPoint_GetFrameInUnitCoordinates")
public func GKAccessPoint_GetFrameInUnitCoordinates
(
    pointer: UnsafeMutableRawPointer
) -> GKWAccessPointFrameInScreenCoordinates
{
    let target = Unmanaged<GKAccessPoint>.fromOpaque(pointer).takeUnretainedValue();
    
    #if os(macOS)
    let screenSize = NSScreen.main!.frame;
    #else
    let screenSize = UIScreen.main.bounds;
    #endif
    let rect = target.frameInScreenCoordinates;

    return GKWAccessPointFrameInScreenCoordinates(
        x: Float(rect.minX / screenSize.width),
        y: Float(rect.minY / screenSize.height),
        width: Float(rect.width / screenSize.width),
        height: Float(rect.height / screenSize.height));
}
