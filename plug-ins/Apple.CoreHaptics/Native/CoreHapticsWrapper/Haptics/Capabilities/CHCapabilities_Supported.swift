//
//  CHCapabilities_Supported.swift
//  CHCapabilities_Supported
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import CoreHaptics
import Foundation

@available(iOS 13, tvOS 14, macOS 11, *)
class CHCapabilities_Supported : CHCapabilities_Protocol {
    
    let _capabilities: CHHapticDeviceCapability = CHHapticEngine.capabilitiesForHardware()
    
    func minValueForEventParameter(parameterID: Int, eventType: Int) -> Float {
        let eventParamId = eventParameterForInt(parameterID)
        let chEventType = eventTypeForUnityInt(eventType)
        do {
            return try _capabilities.attributes(forEventParameter: eventParamId, eventType: chEventType).minValue
        } catch {
            return CHCapabilities_Unsupported().minValueForEventParameter(parameterID: parameterID, eventType: eventType)
        }
    }
    
    func defaultValueForEventParameter(parameterID: Int, eventType: Int) -> Float {
        let eventParamId = eventParameterForInt(parameterID)
        let chEventType = eventTypeForUnityInt(eventType)
        do {
            return try _capabilities.attributes(forEventParameter: eventParamId, eventType: chEventType).defaultValue
        } catch {
            return CHCapabilities_Unsupported().defaultValueForEventParameter(parameterID: parameterID, eventType: eventType)
        }
    }
    
    func maxValueForEventParameter(parameterID: Int, eventType: Int) -> Float {
        let eventParamId = eventParameterForInt(parameterID)
        let chEventType = eventTypeForUnityInt(eventType)
        do {
            return try _capabilities.attributes(forEventParameter: eventParamId, eventType: chEventType).maxValue
        } catch {
            return CHCapabilities_Unsupported().maxValueForEventParameter(parameterID: parameterID, eventType: eventType)
        }
    }
    
    func minValueForDynamicParameter(parameterID: Int) -> Float {
        let dynamicParamId = dynamicParameterForInt(parameterID)
        do {
            return try _capabilities.attributes(forDynamicParameter: dynamicParamId).minValue
        } catch {
            return CHCapabilities_Unsupported().minValueForDynamicParameter(parameterID: parameterID)
        }
    }
    
    func defaultValueForDynamicParameter(parameterID: Int) -> Float {
        let dynamicParamId = dynamicParameterForInt(parameterID)
        do {
            return try _capabilities.attributes(forDynamicParameter: dynamicParamId).defaultValue
        } catch {
            return CHCapabilities_Unsupported().defaultValueForDynamicParameter(parameterID: parameterID)
        }
    }
    
    func maxValueForDynamicParameter(parameterID: Int) -> Float {
        let dynamicParamId = dynamicParameterForInt(parameterID)
        do {
            return try _capabilities.attributes(forDynamicParameter: dynamicParamId).maxValue
        } catch {
            return CHCapabilities_Unsupported().maxValueForDynamicParameter(parameterID: parameterID)
        }
    }
}
