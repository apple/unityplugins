//
//  UIFBG.swift
//  UIFBG
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation

fileprivate var _implementation : UIFBG_Protocol? = nil;

fileprivate func _instance() -> UIFBG_Protocol {
    if _implementation == nil {
        #if os(iOS)
        _implementation = UIFBG_Supported();
        #else
        _implementation = UIFBG_Unsupported();
        #endif
    }
    
    return _implementation!;
}

@_cdecl("UIFeedbackGenerator_CreateImpactGenerator")
public func UIFeedbackGenerator_CreateImpactGenerator(feedbackStyle: Int) -> UnsafeMutableRawPointer? {
    return _instance().CreateImpactGenerator(feedbackStyle: feedbackStyle)
}

@_cdecl("UIFeedbackGenerator_CreateSelectionGenerator")
public func UIFeedbackGenerator_CreateSelectionGenerator() -> UnsafeMutableRawPointer? {
    return _instance().CreateSelectionGenerator()
}

@_cdecl("UIFeedbackGenerator_CreateNotificationGenerator")
public func UIFeedbackGenerator_CreateNotificationGenerator() -> UnsafeMutableRawPointer? {
    return _instance().CreateNotificationGenerator()
}

@_cdecl("UIFeedbackGenerator_DestroyImpactGenerator")
public func UIFeedbackGenerator_DestroyImpactGenerator(generatorPtr: UnsafeRawPointer) {
    _instance().DestroyImpactGenerator(generatorPtr: generatorPtr);
}

@_cdecl("UIFeedbackGenerator_DestroySelectionGenerator")
public func UIFeedbackGenerator_DestroySelectionGenerator(generatorPtr: UnsafeRawPointer) {
    _instance().DestroySelectionGenerator(generatorPtr: generatorPtr);
}

@_cdecl("UIFeedbackGenerator_DestroyNotificationGenerator")
public func UIFeedbackGenerator_DestroyNotificationGenerator(generatorPtr: UnsafeRawPointer) {
    _instance().DestroyNotificationGenerator(generatorPtr: generatorPtr);
}

@_cdecl("UIFeedbackGenerator_Prepare")
public func UIFeedbackGenerator_Prepare(generatorPtr: UnsafeRawPointer) {
    _instance().PrepareGenerator(generatorPtr: generatorPtr);
}

@_cdecl("UIImpactFeedbackGenerator_Trigger")
public func UIImpactFeedbackGenerator_Trigger(generatorPtr: UnsafeRawPointer, intensity: Float) {
    _instance().TriggerImpactGenerator(generatorPtr: generatorPtr, intensity: intensity);
}

@_cdecl("UISelectionFeedbackGenerator_Trigger")
public func UISelectionFeedbackGenerator_Trigger(generatorPtr: UnsafeRawPointer) {
    _instance().TriggerSelectionGenerator(generatorPtr: generatorPtr);
}

@_cdecl("UINotificationFeedbackGenerator_Trigger")
public func UINotificationFeedbackGenerator_Trigger(generatorPtr: UnsafeRawPointer, feedbackType: Int) {
    _instance().TriggerNotificationGenerator(generatorPtr: generatorPtr, feedbackType: feedbackType);
}
