//
//  GKNotificationBanner.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

#if !os(visionOS)
@_cdecl("GKNotificationBanner_Show")
public func GKNotificationBanner_Show
(
    title: char_p,
    message: char_p
)
{
    GKNotificationBanner.show(withTitle: title.toString(), message: message.toString(), completionHandler: nil);
}
#endif

#if !os(visionOS)
@_cdecl("GKNotificationBanner_ShowWithDuration")
public func GKNotificationBanner_ShowWithDuration
(
    title: char_p,
    message: char_p,
    duration: TimeInterval // aka Double
)
{
    GKNotificationBanner.show(withTitle: title.toString(), message: message.toString(), duration: duration, completionHandler: nil);
}
#endif
