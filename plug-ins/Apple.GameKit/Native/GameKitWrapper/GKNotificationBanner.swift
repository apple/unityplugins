//
//  GKNotificationBanner.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

@_cdecl("GKNotificationBanner_Show")
public func GKNotificationBanner_Show
(
    title: char_p,
    message: char_p
)
{
    GKNotificationBanner.show(withTitle: title.toString(), message: message.toString(), completionHandler: nil);
}

@_cdecl("GKNotificationBanner_ShowWithDuration")
public func GKNotificationBanner_ShowWithDuration
(
    title: char_p,
    message: char_p,
    duration: Double
)
{
    GKNotificationBanner.show(withTitle: title.toString(), message: message.toString(), duration: TimeInterval.init(duration), completionHandler: nil);
}
