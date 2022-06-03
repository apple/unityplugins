//
//  CHCapabilities_Protocol.swift
//  CHCapabilities_Protocol
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import CoreHaptics
import Foundation

protocol CHCapabilities_Protocol {
    func minValueForEventParameter(parameterID: Int, eventType: Int) -> Float;
    func defaultValueForEventParameter(parameterID: Int, eventType: Int) -> Float;
    func maxValueForEventParameter(parameterID: Int, eventType: Int) -> Float;
    
    func minValueForDynamicParameter(parameterID: Int) -> Float;
    func defaultValueForDynamicParameter(parameterID: Int) -> Float;
    func maxValueForDynamicParameter(parameterID: Int) -> Float;
}
