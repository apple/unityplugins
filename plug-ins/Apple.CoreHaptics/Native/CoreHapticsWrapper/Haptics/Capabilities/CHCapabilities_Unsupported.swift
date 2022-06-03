//
//  CHCapabilities_Unsupported.swift
//  CHCapabilities_Unsupported
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation

fileprivate class ParameterAttributes {
    let minVal: Float
    let defaultVal: Float
    let maxVal: Float
    
    init(_ minVal: Float, _ defaultVal: Float, _ maxVal: Float) {
        self.minVal = minVal
        self.defaultVal = defaultVal
        self.maxVal = maxVal
    }
}

class CHCapabilities_Unsupported : CHCapabilities_Protocol {
    
    enum CHCapabilityValueType : Int {
        case Minimum = 0
        case Default = 1
        case Maximum = 2
    }
    
    // Hardcoded values for all (EventType, Parameter) pairs EXCEPT HapticTransient default values
    // The default values for Intensity and Sharpness are different than shown below, which is handled in `defaultValueForEventParameter()`
    private let _eventParamCapabilities: [Int : ParameterAttributes] = [
        // Haptic params: Intensity, Sharpness
        0: ParameterAttributes(0, 0.6, 1),
        1: ParameterAttributes(0, 0.422601, 1),
        
        // ADSR params: Attack, Decay, Release, Sustain
        2: ParameterAttributes(0, 0, 1),
        3: ParameterAttributes(0, 0, 1),
        4: ParameterAttributes(0, 0, 1),
        5: ParameterAttributes(0, 0, 1),
        
        // Audio params: Volume, Pitch, Pan, Brightness
        6: ParameterAttributes(0, 1, 1),
        7: ParameterAttributes(-1, 0, 1),
        8: ParameterAttributes(-1, 0, 1),
        9: ParameterAttributes(0, 1, 1)
    ]
    
    private let _dynamicParamCapabilities: [Int : ParameterAttributes] = [
        // Haptic params: Intensity, Sharpness, AttackTime, DecayTime, ReleaseTime
        0: ParameterAttributes(0, 1, 1),
        1: ParameterAttributes(-1, 0, 1),
        2: ParameterAttributes(-1, 0, 1),
        3: ParameterAttributes(-1, 0, 1),
        4: ParameterAttributes(-1, 0, 1),
        
        // Audio params: Volume, Pitch, Pan, Brightness, AttackTime, DecayTime, ReleaseTime
        5: ParameterAttributes(0, 1, 1),
        6: ParameterAttributes(-1, 0, 1),
        7: ParameterAttributes(-1, 0, 1),
        8: ParameterAttributes(-1, 0, 1),
        9: ParameterAttributes(-1, 0, 1),
        10: ParameterAttributes(-1, 0, 1),
        11: ParameterAttributes(-1, 0, 1)
    ]
    
    func minValueForEventParameter(parameterID: Int, eventType: Int) -> Float {
        if let paramLookup = _eventParamCapabilities[parameterID] {
            return paramLookup.minVal
        }
        
        return 0
    }
    
    func defaultValueForEventParameter(parameterID: Int, eventType: Int) -> Float {
        if eventType == 0 {
            if parameterID == 0 {
                return 0.75 // Default Transient Intensity
            } else if parameterID == 1 {
                return 0.5 // Default Transient Sharpness
            }
        }
        
        if let paramLookup = _eventParamCapabilities[parameterID] {
            return paramLookup.defaultVal
        }
        
        return 0
    }
    
    func maxValueForEventParameter(parameterID: Int, eventType: Int) -> Float {
        if let paramLookup = _eventParamCapabilities[parameterID] {
            return paramLookup.maxVal
        }
        
        return 1
    }
    
    func minValueForDynamicParameter(parameterID: Int) -> Float {
        if let paramLookup = _dynamicParamCapabilities[parameterID] {
            return paramLookup.minVal
        }
        
        return -1
    }
    
    func defaultValueForDynamicParameter(parameterID: Int) -> Float {
        if let paramLookup = _dynamicParamCapabilities[parameterID] {
            return paramLookup.defaultVal
        }
        
        return 0
    }
    
    func maxValueForDynamicParameter(parameterID: Int) -> Float {
        if let paramLookup = _dynamicParamCapabilities[parameterID] {
            return paramLookup.maxVal
        }
        
        return 1
    }
}
