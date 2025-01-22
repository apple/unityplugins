//
//  GKErrorCodeExtension.swift
//  GameKitWrapper
//

import Foundation
import GameKit

public extension NSError {
    convenience init(code: GKError.Code) {
        self.init(domain: "GKErrorDomain", code: GKErrorCodeExtension.unsupportedOperationForOSVersion.rawValue, userInfo: nil);
    }

    convenience init(code: GKErrorCodeExtension) {
        self.init(domain: "GKErrorDomain", code: code.rawValue, userInfo: nil);
    }
}

