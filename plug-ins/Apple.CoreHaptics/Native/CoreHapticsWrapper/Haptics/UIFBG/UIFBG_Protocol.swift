//
//  UIFBG_Protocol.swift
//  UIFBG_Protocol
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation

protocol UIFBG_Protocol {
    func CreateImpactGenerator(feedbackStyle: Int) -> UnsafeMutableRawPointer?;
    func CreateSelectionGenerator() -> UnsafeMutableRawPointer?;
    func CreateNotificationGenerator() -> UnsafeMutableRawPointer?;
    
    func DestroyImpactGenerator(generatorPtr: UnsafeRawPointer);
    func DestroySelectionGenerator(generatorPtr: UnsafeRawPointer);
    func DestroyNotificationGenerator(generatorPtr: UnsafeRawPointer);
    
    func PrepareGenerator(generatorPtr: UnsafeRawPointer);
    
    func TriggerImpactGenerator(generatorPtr: UnsafeRawPointer, intensity: Float);
    func TriggerSelectionGenerator(generatorPtr: UnsafeRawPointer);
    func TriggerNotificationGenerator(generatorPtr: UnsafeRawPointer, feedbackType: Int);
}
