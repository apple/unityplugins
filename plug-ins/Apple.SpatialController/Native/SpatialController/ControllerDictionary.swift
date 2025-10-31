//
//  ControllerDictionary.swift
//  SpatialController
//
//  Copyright © 2025 Apple, Inc. All rights reserved.
//

import Foundation
import GameController

/// A thread-safe dictionary for managing game controllers.
///
/// Uses a concurrent dispatch queue with barrier operations to ensure thread safety
/// when storing and accessing `GCController` instances by string identifiers.
class ControllerDictionary {
  
    /// Concurrent queue for thread-safe dictionary access.
    private let _queue = DispatchQueue(label: "ControllerDictionary.queue", attributes: .concurrent)
    
    /// Internal dictionary storing controller mappings.
    private var _elements = [String : GCController]()

    /// Thread-safe copy of all controller mappings.
    ///
    /// - Returns: A snapshot of the current controller dictionary.
    var elements: [String : GCController] {
        var result = [String: GCController]()
        
        _queue.sync {
            result = _elements
        }

        return result
    }
  
    /// Updates or adds a controller mapping.
    ///
    /// - Parameters:
    ///   - value: The controller to store.
    ///   - forKey: The identifier for the controller.
    func updateValue(_ value: GCController, forKey: String) {
        _queue.async(flags: .barrier) {
            self._elements.updateValue(value, forKey: forKey)
        }
    }
    
    /// Removes a controller mapping.
    ///
    /// - Parameter forKey: The identifier of the controller to remove.
    func removeValue(forKey: String) {
        _queue.async(flags: .barrier) {
            self._elements.removeValue(forKey: forKey)
        }
    }
}
