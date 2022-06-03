//
//  CHCapabilities.swift
//  CHCapabilities
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation

fileprivate var _implementation : CHCapabilities_Protocol? = nil;

fileprivate func _instance() -> CHCapabilities_Protocol {
    if _implementation == nil {
        if #available(iOS 13, tvOS 14.0, macOS 11, *) {
            _implementation = CHCapabilities_Supported();
        } else {
            _implementation = CHCapabilities_Unsupported();
        }
    }
        
    return _implementation!;
}

@_cdecl("CoreHaptics_Capabilities_MinValueForEventParameter")
public func CoreHaptics_Capabilities_MinValueForEventParameter(parameterId: Int, eventType: Int) -> Float {
    return _instance().minValueForEventParameter(parameterID: parameterId, eventType: eventType)
}

@_cdecl("CoreHaptics_Capabilities_DefaultValueForEventParameter")
public func CoreHaptics_Capabilities_DefaultValueForEventParameter(parameterId: Int, eventType: Int) -> Float {
    return _instance().defaultValueForEventParameter(parameterID: parameterId, eventType: eventType)
}

@_cdecl("CoreHaptics_Capabilities_MaxValueForEventParameter")
public func CoreHaptics_Capabilities_MaxValueForEventParameter(parameterId: Int, eventType: Int) -> Float {
    return _instance().maxValueForEventParameter(parameterID: parameterId, eventType: eventType)
}

@_cdecl("CoreHaptics_Capabilities_MinValueForDynamicParameter")
public func CoreHaptics_Capabilities_MinValueForDynamicParameter(parameterId: Int) -> Float {
    return _instance().minValueForDynamicParameter(parameterID: parameterId)
}

@_cdecl("CoreHaptics_Capabilities_DefaultValueForDynamicParameter")
public func CoreHaptics_Capabilities_DefaultValueForDynamicParameter(parameterId: Int) -> Float {
    return _instance().defaultValueForDynamicParameter(parameterID: parameterId)
}

@_cdecl("CoreHaptics_Capabilities_MaxValueForDynamicParameter")
public func CoreHaptics_Capabilities_MaxValueForDynamicParameter(parameterId: Int) -> Float {
    return _instance().maxValueForDynamicParameter(parameterID: parameterId)
}
