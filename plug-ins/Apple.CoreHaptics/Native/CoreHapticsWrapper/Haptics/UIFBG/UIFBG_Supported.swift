//
//  UIFBG_Supported.swift
//  UIFBG_Supported
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
#if os(iOS)
import UIKit

class UIFBG_Supported : UIFBG_Protocol {
    func CreateImpactGenerator(feedbackStyle: Int) -> UnsafeMutableRawPointer? {
        let generator = UIImpactFeedbackGenerator.init(style: FeedbackStyleForInt(typeInt: feedbackStyle))
        return Unmanaged.passRetained(generator).toOpaque();
    }
    
    func CreateSelectionGenerator() -> UnsafeMutableRawPointer? {
        let generator = UISelectionFeedbackGenerator.init()
        return Unmanaged.passRetained(generator).toOpaque();
    }
    
    func CreateNotificationGenerator() -> UnsafeMutableRawPointer? {
        let generator = UINotificationFeedbackGenerator.init()
        return Unmanaged.passRetained(generator).toOpaque();
    }
    
    func DestroyImpactGenerator(generatorPtr: UnsafeRawPointer) {
        _ = Unmanaged<UIImpactFeedbackGenerator>.fromOpaque(generatorPtr).autorelease();
    }
    
    func DestroySelectionGenerator(generatorPtr: UnsafeRawPointer) {
        _ = Unmanaged<UISelectionFeedbackGenerator>.fromOpaque(generatorPtr).autorelease();
    }
    
    func DestroyNotificationGenerator(generatorPtr: UnsafeRawPointer) {
        _ = Unmanaged<UINotificationFeedbackGenerator>.fromOpaque(generatorPtr).autorelease();
    }
    
    func PrepareGenerator(generatorPtr: UnsafeRawPointer) {
        let generator = Unmanaged<UIFeedbackGenerator>.fromOpaque(generatorPtr).takeUnretainedValue();
        generator.prepare()
    }
    
    func TriggerImpactGenerator(generatorPtr: UnsafeRawPointer, intensity: Float) {
        let generator = Unmanaged<UIImpactFeedbackGenerator>.fromOpaque(generatorPtr).takeUnretainedValue();
        generator.impactOccurred(intensity: CGFloat(intensity))
    }
    
    func TriggerSelectionGenerator(generatorPtr: UnsafeRawPointer) {
        let generator = Unmanaged<UISelectionFeedbackGenerator>.fromOpaque(generatorPtr).takeUnretainedValue();
        generator.selectionChanged()
    }
    
    func TriggerNotificationGenerator(generatorPtr: UnsafeRawPointer, feedbackType: Int) {
        let generator = Unmanaged<UINotificationFeedbackGenerator>.fromOpaque(generatorPtr).takeUnretainedValue();
        generator.notificationOccurred(FeedbackTypeForInt(typeInt: feedbackType))
    }
}

#endif
