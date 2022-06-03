//
//  Utilities.swift
//  Utilities
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

import Foundation
import CoreHaptics

@available(iOS 13, tvOS 14, macOS 10, *)
func dynamicParameterForInt(_ val: Int) -> CHHapticDynamicParameter.ID {
    switch val {
    case 0:
        return CHHapticDynamicParameter.ID.hapticIntensityControl
    case 1:
        return CHHapticDynamicParameter.ID.hapticSharpnessControl
    case 2:
        return CHHapticDynamicParameter.ID.hapticAttackTimeControl
    case 3:
        return CHHapticDynamicParameter.ID.hapticDecayTimeControl
    case 4:
        return CHHapticDynamicParameter.ID.hapticReleaseTimeControl
    case 5:
        return CHHapticDynamicParameter.ID.audioVolumeControl
    case 6:
        return CHHapticDynamicParameter.ID.audioPanControl
    case 7:
        return CHHapticDynamicParameter.ID.audioPitchControl
    case 8:
        return CHHapticDynamicParameter.ID.audioBrightnessControl
    case 9:
        return CHHapticDynamicParameter.ID.audioAttackTimeControl
    case 10:
        return CHHapticDynamicParameter.ID.audioDecayTimeControl
    case 11:
        return CHHapticDynamicParameter.ID.audioReleaseTimeControl
    default:
        return CHHapticDynamicParameter.ID.hapticIntensityControl
    }
}

@available(iOS 13, tvOS 14, macOS 10, *)
func eventParameterForInt(_ val: Int) -> CHHapticEvent.ParameterID {
    switch val {
    case 0:
        return CHHapticEvent.ParameterID.hapticIntensity
    case 1:
        return CHHapticEvent.ParameterID.hapticSharpness
    case 2:
        return CHHapticEvent.ParameterID.attackTime
    case 3:
        return CHHapticEvent.ParameterID.decayTime
    case 4:
        return CHHapticEvent.ParameterID.releaseTime
    case 5:
        return CHHapticEvent.ParameterID.sustained
    case 6:
        return CHHapticEvent.ParameterID.audioVolume
    case 7:
        return CHHapticEvent.ParameterID.audioPan
    case 8:
        return CHHapticEvent.ParameterID.audioPitch
    case 9:
        return CHHapticEvent.ParameterID.audioBrightness
    default:
        return CHHapticEvent.ParameterID.hapticIntensity
    }
}

@available(iOS 13, tvOS 14, macOS 10, *)
func eventTypeForUnityInt(_ val: Int) -> CHHapticEvent.EventType {
    switch val {
    case 0:
        return .hapticTransient
    case 1:
        return .hapticContinuous
    case 2:
        return .audioContinuous
    case 3:
        return .audioCustom
    default:
        return .hapticTransient
    }
}


#if os(iOS)
import UIKit

func FeedbackTypeForInt(typeInt: Int) -> UINotificationFeedbackGenerator.FeedbackType {
    switch typeInt {
    case 0:
        return .error
    case 1:
        return .success
    case 2:
        return .warning
    default:
        return .success
    }
}

func FeedbackStyleForInt(typeInt: Int) -> UIImpactFeedbackGenerator.FeedbackStyle {
    switch typeInt {
    case 0:
        return .heavy
    case 1:
        return .light
    case 2:
        return .medium
    case 3:
        return .rigid
    case 4:
        return .soft
    default:
        return .heavy
    }
}
#endif
