//
//  ThreadSafeCollection.swift
//  GameControllerWrapper
//
//  Copyright © 2020 Apple, Inc. All rights reserved.
//

import Foundation
import GameController

class ControllerDictionary {
  
    private let _queue = DispatchQueue(label: "ControllerDictionary.queue", attributes: .concurrent)
    private var _elements = [String : GCController]();
  
    var elements: [String : GCController] {
        var result = [String: GCController]();
        
        _queue.sync {
            result = _elements
        }

        return result
    }
  
    func updateValue(_ value: GCController, forKey: String) {
        _queue.async(flags: .barrier) {
            self._elements.updateValue(value, forKey: forKey);
        }
    }
    
    func removeValue(forKey: String) {
        _queue.async(flags: .barrier) {
            self._elements.removeValue(forKey: forKey);
        }
    }
}
