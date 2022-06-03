//
//  GameKitUIDelegateHandler.swift
//  GameKitWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import GameKit

public class GameKitUIDelegateHandler : NSObject {
    var _taskId : Int64? = nil;
    var _onSuccess : SuccessTaskCallback? = nil;
    
    init(taskId: Int64, onSuccess : @escaping SuccessTaskCallback) {
        _taskId = taskId;
        _onSuccess = onSuccess;
    }
}

extension GameKitUIDelegateHandler : GKGameCenterControllerDelegate {
    public func gameCenterViewControllerDidFinish(_ gameCenterViewController: GKGameCenterViewController) {
        #if os(iOS) || os(tvOS)
            gameCenterViewController.dismiss(animated: true, completion: {
                self._onSuccess?(self._taskId!);
            });
        #else
            GKDialogController.shared().dismiss(gameCenterViewController);
            self._onSuccess?(self._taskId!);
        #endif
    }
}
