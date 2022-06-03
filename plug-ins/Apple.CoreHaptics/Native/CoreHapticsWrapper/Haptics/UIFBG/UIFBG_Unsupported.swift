//
//  UIFBG_Unsupported.swift
//  UIFBG_Unsupported
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation

class UIFBG_Unsupported : UIFBG_Protocol {
    func CreateImpactGenerator(feedbackStyle: Int) -> UnsafeMutableRawPointer? {
        return UnsafeMutableRawPointer?.none;
    }
    
    func CreateSelectionGenerator() -> UnsafeMutableRawPointer? {
        return UnsafeMutableRawPointer?.none;
    }
    
    func CreateNotificationGenerator() -> UnsafeMutableRawPointer? {
        return UnsafeMutableRawPointer?.none;
    }
    
    func DestroyImpactGenerator(generatorPtr: UnsafeRawPointer) {
        // NOOP
    }
    
    func DestroySelectionGenerator(generatorPtr: UnsafeRawPointer) {
        // NOOP
    }
    
    func DestroyNotificationGenerator(generatorPtr: UnsafeRawPointer) {
        // NOOP
    }
    
    func PrepareGenerator(generatorPtr: UnsafeRawPointer) {
        // NOOP
    }
    
    func TriggerImpactGenerator(generatorPtr: UnsafeRawPointer, intensity: Float) {
        // NOOP
    }
    
    func TriggerSelectionGenerator(generatorPtr: UnsafeRawPointer) {
        // NOOP
    }
    
    func TriggerNotificationGenerator(generatorPtr: UnsafeRawPointer, feedbackType: Int) {
        // NOOP
    }
    
    
}
