//
//  AppleAccessibilityBridge.m
//  AppleAccessibility
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <AppleAccessibility/AppleAccessibility.h>
#import <TargetConditionals.h>
#import "AppleAccessibilitySafeOverride.h"
#import "AppleAccessibilityRuntime.h"

#pragma mark Notifictions

APPLE_ACCESSIBILITY_EXTERN void _UnityAX_PostFeatureEnabledNotification(const char *name, const bool value)
{
    [[NSNotificationCenter defaultCenter] postNotificationName:name == NULL ? nil : [NSString stringWithUTF8String:name] object:@(value)];
}

APPLE_ACCESSIBILITY_EXTERN void _UnityAX_PostScreenChangedNotification(void)
{
    UIAccessibilityPostNotification(UIAccessibilityScreenChangedNotification, nil);
}

APPLE_ACCESSIBILITY_EXTERN void _UnityAX_PostLayoutChangedNotification(void)
{
    UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, nil);
}

APPLE_ACCESSIBILITY_EXTERN void _UnityAX_PostAnnouncementNotification(const char *announcement)
{
    UIAccessibilityPostNotification(UIAccessibilityAnnouncementNotification, announcement == NULL ? nil : [NSString stringWithUTF8String:announcement]);
}

APPLE_ACCESSIBILITY_EXTERN void _UnityAX_PostPageScrolledNotification(const char *position)
{
    UIAccessibilityPostNotification(UIAccessibilityPageScrolledNotification, position == NULL ? nil : [NSString stringWithUTF8String:position]);
}

#pragma mark Elements

typedef char *(* AccessibilityFrameDelegate)(int32_t);
static AccessibilityFrameDelegate __axFrameDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityFrame(AccessibilityFrameDelegate delegate) { __axFrameDelegate = delegate; }

typedef char *(* AccessibilityLabelDelegate)(int32_t);
static AccessibilityLabelDelegate __axLabelDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityLabel(AccessibilityLabelDelegate delegate) { __axLabelDelegate = delegate; }

typedef uint64_t (* AccessibilityTraitsDelegate)(int32_t);
static AccessibilityTraitsDelegate __axTraitsDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityTraits(AccessibilityTraitsDelegate delegate) { __axTraitsDelegate = delegate; }

typedef bool (* AccessibilityIsElementDelegate)(int32_t);
static AccessibilityIsElementDelegate __axIsElementDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsElement(AccessibilityIsElementDelegate delegate) { __axIsElementDelegate = delegate; }

typedef char *(* AccessibilityHintDelegate)(int32_t);
static AccessibilityHintDelegate __axHintDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityHint(AccessibilityHintDelegate delegate) { __axHintDelegate = delegate; }

typedef char *(* AccessibilityValueDelegate)(int32_t);
static AccessibilityValueDelegate __axValueDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityValue(AccessibilityValueDelegate delegate) { __axValueDelegate = delegate; }

typedef char *(* AccessibilityIdentifierDelegate)(int32_t);
static AccessibilityIdentifierDelegate __axIdentifierDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIdentifier(AccessibilityIdentifierDelegate delegate) { __axIdentifierDelegate = delegate; }

typedef bool (* AccessibilityViewIsModalDelegate)(int32_t);
static AccessibilityViewIsModalDelegate __axViewIsModalDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityViewIsModal(AccessibilityViewIsModalDelegate delegate) { __axViewIsModalDelegate = delegate; }

typedef char *(* AccessibilityActivationPointDelegate)(int32_t);
static AccessibilityActivationPointDelegate __axActivationPointDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityActivationPoint(AccessibilityActivationPointDelegate delegate) { __axActivationPointDelegate = delegate; }

typedef uint64_t (* AccessibilityCustomActionsCountDelegate)(int32_t);
static AccessibilityCustomActionsCountDelegate __axCustomActionsCountDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityCustomActionsCount(AccessibilityCustomActionsCountDelegate delegate) { __axCustomActionsCountDelegate = delegate; }

typedef bool (* AccessibilityPerformCustomActionDelegate)(int32_t, int32_t);
static AccessibilityPerformCustomActionDelegate __axPerformCustomActionDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityPerformCustomAction(AccessibilityPerformCustomActionDelegate delegate) { __axPerformCustomActionDelegate = delegate; }

typedef char *(* AccessibilityCustomActionNameDelegate)(int32_t, int32_t);
static AccessibilityCustomActionNameDelegate __axCustomActionNameDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityCustomActionName(AccessibilityCustomActionNameDelegate delegate) { __axCustomActionNameDelegate = delegate; }

typedef bool (* AccessibilityScrollDelegate)(int32_t, uint32_t);
static AccessibilityScrollDelegate __axScrollDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityScroll(AccessibilityScrollDelegate delegate) { __axScrollDelegate = delegate; }

typedef bool (* AccessibilityPerformMagicTapDelegate)(int32_t);
static AccessibilityPerformMagicTapDelegate __axPerformMagicTapDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityPerformMagicTap(AccessibilityPerformMagicTapDelegate delegate) { __axPerformMagicTapDelegate = delegate; }

typedef bool (* AccessibilityPerformEscapeDelegate)(int32_t);
static AccessibilityPerformEscapeDelegate __axPerformEscapeDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityPerformEscape(AccessibilityPerformEscapeDelegate delegate) { __axPerformEscapeDelegate = delegate; }

typedef bool (* AccessibilityActivateDelegate)(int32_t);
static AccessibilityActivateDelegate __axActivateDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityActivate(AccessibilityActivateDelegate delegate) { __axActivateDelegate = delegate; }

typedef void (* AccessibilityIncrementDelegate)(int32_t);
static AccessibilityIncrementDelegate __axIncrementDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIncrement(AccessibilityIncrementDelegate delegate) { __axIncrementDelegate = delegate; }

typedef void (* AccessibilityDecrementDelegate)(int32_t);
static AccessibilityDecrementDelegate __axDecrementDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityDecrement(AccessibilityDecrementDelegate delegate) { __axDecrementDelegate = delegate; }


#pragma mark Settings

typedef void (* AccessibilityNotificationDelegate)(void);

static AccessibilityNotificationDelegate __axPreferredContentSizeCategoryDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityPreferredContentSizeCategoryDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axPreferredContentSizeCategoryDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN int _UnityAX_UIAccessibilityPreferredContentSizeCategory(void)
{
    UIContentSizeCategory category = [[UIApplication sharedApplication] preferredContentSizeCategory];
    if ( [category isEqualToString:UIContentSizeCategoryExtraSmall] )
    {
        return 1;
    }
    else if ( [category isEqualToString:UIContentSizeCategorySmall] )
    {
        return 2;
    }
    else if ( [category isEqualToString:UIContentSizeCategoryMedium] )
    {
        return 3;
    }
    else if ( [category isEqualToString:UIContentSizeCategoryLarge] )
    {
        return 4;
    }
    else if ( [category isEqualToString:UIContentSizeCategoryExtraLarge] )
    {
        return 5;
    }
    else if ( [category isEqualToString:UIContentSizeCategoryExtraExtraLarge] )
    {
        return 6;
    }
    else if ( [category isEqualToString:UIContentSizeCategoryExtraExtraExtraLarge] )
    {
        return 7;
    }
    else if ( [category isEqualToString:UIContentSizeCategoryAccessibilityMedium] )
    {
        return 8;
    }
    else if ( [category isEqualToString:UIContentSizeCategoryAccessibilityLarge] )
    {
        return 9;
    }
    else if ( [category isEqualToString:UIContentSizeCategoryAccessibilityExtraLarge] )
    {
        return 10;
    }
    else if ( [category isEqualToString:UIContentSizeCategoryAccessibilityExtraExtraLarge] )
    {
        return 11;
    }
    else if ( [category isEqualToString:UIContentSizeCategoryAccessibilityExtraExtraExtraLarge] )
    {
        return 12;
    }
    return 0;
}

APPLE_ACCESSIBILITY_EXTERN float _UnityAX_UIAccessibilityPreferredContentSizeMultiplier(void)
{
    if ( @available(iOS 10.0, tvOS 10.0, *) )
    {
        UITraitCollection *defaultTraitCollection = [UITraitCollection traitCollectionWithPreferredContentSizeCategory:UIContentSizeCategoryLarge];
        UITraitCollection *newTraitCollection = [UITraitCollection traitCollectionWithPreferredContentSizeCategory:UIApplication.sharedApplication.preferredContentSizeCategory];

        UIFont *defaultFont = [UIFont preferredFontForTextStyle:UIFontTextStyleBody compatibleWithTraitCollection:defaultTraitCollection];
        UIFont *newFont = [UIFont preferredFontForTextStyle:UIFontTextStyleBody compatibleWithTraitCollection:newTraitCollection];

        CGFloat percentage = ((newFont.pointSize / defaultFont.pointSize) * 100);

        int roundedPercentage = (int)percentage;
        roundedPercentage = roundedPercentage - (roundedPercentage % 5); // bucketize by 5s
        if ( roundedPercentage == 100 ) // avoid rounding error
        {
            return 1;
        }
        return (float)roundedPercentage / (float)100;
    }
    return 1;
}

static AccessibilityNotificationDelegate __axIsVoiceOverRunningDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsVoiceOverRunningDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsVoiceOverRunningDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsVoiceOverRunning(void)
{
    return UIAccessibilityIsVoiceOverRunning();
}

static AccessibilityNotificationDelegate __axIsSwitchControlRunningDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsSwitchControlRunningDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsSwitchControlRunningDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsSwitchControlRunning(void)
{
    return UIAccessibilityIsSwitchControlRunning();
}

static AccessibilityNotificationDelegate __axIsMonoAudioEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsMonoAudioEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsMonoAudioEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsMonoAudioEnabled(void)
{
    return UIAccessibilityIsMonoAudioEnabled();
}

static AccessibilityNotificationDelegate __axIsClosedCaptioningEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsClosedCaptioningEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsClosedCaptioningEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsClosedCaptioningEnabled(void)
{
    return UIAccessibilityIsClosedCaptioningEnabled();
}

static AccessibilityNotificationDelegate __axIsInvertColorsEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsInvertColorsEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsInvertColorsEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsInvertColorsEnabled(void)
{
    return UIAccessibilityIsInvertColorsEnabled();
}

static AccessibilityNotificationDelegate __axIsGuidedAccessEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsGuidedAccessEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsGuidedAccessEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsGuidedAccessEnabled(void)
{
    return UIAccessibilityIsGuidedAccessEnabled();
}

static AccessibilityNotificationDelegate __axIsBoldTextEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsBoldTextEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsBoldTextEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsBoldTextEnabled(void)
{
    return UIAccessibilityIsBoldTextEnabled();
}

static AccessibilityNotificationDelegate __axButtonShapesEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityButtonShapesEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axButtonShapesEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityButtonShapesEnabled(void)
{
    if ( @available(iOS 14.0, tvOS 14.0, *) )
    {
        return UIAccessibilityButtonShapesEnabled();
    }
    else
    {
        return false;
    }
}

static AccessibilityNotificationDelegate __axIsGrayscaleEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsGrayscaleEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsGrayscaleEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsGrayscaleEnabled(void)
{
    return UIAccessibilityIsGrayscaleEnabled();
}

static AccessibilityNotificationDelegate __axIsReduceTransparencyEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsReduceTransparencyEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsReduceTransparencyEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsReduceTransparencyEnabled(void)
{
    return UIAccessibilityIsReduceTransparencyEnabled();
}

static AccessibilityNotificationDelegate __axIsReduceMotionEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsReduceMotionEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsReduceMotionEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsReduceMotionEnabled(void)
{
    return UIAccessibilityIsReduceMotionEnabled();
}

static AccessibilityNotificationDelegate __axPrefersCrossFadeTransitionsDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityPrefersCrossFadeTransitionsDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axPrefersCrossFadeTransitionsDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityPrefersCrossFadeTransitions(void)
{
    if ( @available(iOS 14.0, tvOS 14.0, *) )
    {
        return UIAccessibilityPrefersCrossFadeTransitions();
    }
    else
    {
        return false;
    }
}

static AccessibilityNotificationDelegate __axIsVideoAutoplayEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsVideoAutoplayEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsVideoAutoplayEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsVideoAutoplayEnabled(void)
{
    if ( @available(iOS 13.0, tvOS 13.0, *) )
    {
        return UIAccessibilityIsVideoAutoplayEnabled();
    }
    else
    {
        return false;
    }
}

static AccessibilityNotificationDelegate __axDarkerSystemColorsEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityDarkerSystemColorsEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axDarkerSystemColorsEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityDarkerSystemColorsEnabled(void)
{
    return UIAccessibilityDarkerSystemColorsEnabled();
}

static AccessibilityNotificationDelegate __axIsSpeakSelectionEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsSpeakSelectionEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsSpeakSelectionEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsSpeakSelectionEnabled(void)
{
    return UIAccessibilityIsSpeakSelectionEnabled();
}

static AccessibilityNotificationDelegate __axIsSpeakScreenEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsSpeakScreenEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsSpeakScreenEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsSpeakScreenEnabled(void)
{
    return UIAccessibilityIsSpeakScreenEnabled();
}

static AccessibilityNotificationDelegate __axIsShakeToUndoEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsShakeToUndoEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsShakeToUndoEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsShakeToUndoEnabled(void)
{
    return UIAccessibilityIsShakeToUndoEnabled();
}

static AccessibilityNotificationDelegate __axShouldDifferentiateWithoutColorDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityShouldDifferentiateWithoutColorDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axShouldDifferentiateWithoutColorDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityShouldDifferentiateWithoutColor(void)
{
    if ( @available(iOS 13.0, tvOS 13.0, *) )
    {
        return UIAccessibilityShouldDifferentiateWithoutColor();
    }
    else
    {
        return false;
    }
}

static AccessibilityNotificationDelegate __axIsOnOffSwitchLabelsEnabledDidChangeNotificationDelegate = NULL;
APPLE_ACCESSIBILITY_EXTERN void _UnityAX_registerAccessibilityIsOnOffSwitchLabelsEnabledDidChangeNotification(AccessibilityNotificationDelegate delegate) { __axIsOnOffSwitchLabelsEnabledDidChangeNotificationDelegate = delegate; }
APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_UIAccessibilityIsOnOffSwitchLabelsEnabled(void)
{
    if ( @available(iOS 13.0, tvOS 13.0, *) )
    {
        return UIAccessibilityIsOnOffSwitchLabelsEnabled();
    }
    else
    {
        return false;
    }
}


APPLE_ACCESSIBILITY_HIDDEN
@interface _AppleAccessibilityNotificationObserver: NSObject
- (void)_accessibilityStatusChangedCallback:(NSNotification *)notification;
@end

@implementation _AppleAccessibilityNotificationObserver
- (void)_accessibilityStatusChangedCallback:(NSNotification *)notification {
#if TARGET_OS_IOS
    if ( [notification.name isEqualToString:UIContentSizeCategoryDidChangeNotification] )
    {
        if ( __axPreferredContentSizeCategoryDidChangeNotificationDelegate != NULL )
        {
            __axPreferredContentSizeCategoryDidChangeNotificationDelegate();
        }
        return;
    }
#endif
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
    if ( [notification.name isEqualToString:UIAccessibilityVoiceOverStatusChanged] )
#pragma clang diagnostic pop
    {
        if ( __axIsVoiceOverRunningDidChangeNotificationDelegate != NULL )
        {
            __axIsVoiceOverRunningDidChangeNotificationDelegate();
        }
        return;
    }
    if ( [notification.name isEqualToString:UIAccessibilitySwitchControlStatusDidChangeNotification] )
    {
        if ( __axIsSwitchControlRunningDidChangeNotificationDelegate != NULL )
        {
            __axIsSwitchControlRunningDidChangeNotificationDelegate();
        }
        return;
    }
    if ( [notification.name isEqualToString:UIAccessibilityMonoAudioStatusDidChangeNotification] )
    {
        if ( __axIsMonoAudioEnabledDidChangeNotificationDelegate != NULL )
        {
            __axIsMonoAudioEnabledDidChangeNotificationDelegate();
        }
        return;
    }
    if ( [notification.name isEqualToString:UIAccessibilityClosedCaptioningStatusDidChangeNotification] )
    {
        if ( __axIsClosedCaptioningEnabledDidChangeNotificationDelegate != NULL )
        {
            __axIsClosedCaptioningEnabledDidChangeNotificationDelegate();
        }
        return;
    }
    if ( [notification.name isEqualToString:UIAccessibilityInvertColorsStatusDidChangeNotification] )
    {
        if ( __axIsInvertColorsEnabledDidChangeNotificationDelegate != NULL )
        {
            __axIsInvertColorsEnabledDidChangeNotificationDelegate();
        }
        return;
    }
    if ( [notification.name isEqualToString:UIAccessibilityGuidedAccessStatusDidChangeNotification] )
    {
        if ( __axIsGuidedAccessEnabledDidChangeNotificationDelegate != NULL )
        {
            __axIsGuidedAccessEnabledDidChangeNotificationDelegate();
        }
        return;
    }
    if ( [notification.name isEqualToString:UIAccessibilityBoldTextStatusDidChangeNotification] )
    {
        if ( __axIsBoldTextEnabledDidChangeNotificationDelegate != NULL )
        {
            __axIsBoldTextEnabledDidChangeNotificationDelegate();
        }
        return;
    }
    if ( @available(iOS 14.0, tvOS 14.0, *) ) {
        if ( [notification.name isEqualToString:UIAccessibilityButtonShapesEnabledStatusDidChangeNotification] )
        {
            if ( __axButtonShapesEnabledDidChangeNotificationDelegate != NULL )
            {
                __axButtonShapesEnabledDidChangeNotificationDelegate();
            }
            return;
        }
    }
    if ( [notification.name isEqualToString:UIAccessibilityGrayscaleStatusDidChangeNotification] )
    {
        if ( __axIsGrayscaleEnabledDidChangeNotificationDelegate != NULL )
        {
            __axIsGrayscaleEnabledDidChangeNotificationDelegate();
        }
        return;
    }
    if ( [notification.name isEqualToString:UIAccessibilityReduceTransparencyStatusDidChangeNotification] )
    {
        if ( __axIsReduceTransparencyEnabledDidChangeNotificationDelegate != NULL )
        {
            __axIsReduceTransparencyEnabledDidChangeNotificationDelegate();
        }
        return;
    }
    if ( [notification.name isEqualToString:UIAccessibilityReduceMotionStatusDidChangeNotification] )
    {
        if ( __axIsReduceMotionEnabledDidChangeNotificationDelegate != NULL )
        {
            __axIsReduceMotionEnabledDidChangeNotificationDelegate();
        }
        return;
    }
    if ( @available(iOS 14.0, tvOS 14.0, *) ) {
        if ( [notification.name isEqualToString:UIAccessibilityPrefersCrossFadeTransitionsStatusDidChangeNotification] )
        {
            if ( __axPrefersCrossFadeTransitionsDidChangeNotificationDelegate != NULL )
            {
                __axPrefersCrossFadeTransitionsDidChangeNotificationDelegate();
            }
            return;
        }
    }
    if ( @available(iOS 13.0, tvOS 13.0, *) ) {
        if ( [notification.name isEqualToString:UIAccessibilityVideoAutoplayStatusDidChangeNotification] )
        {
            if ( __axIsVideoAutoplayEnabledDidChangeNotificationDelegate != NULL )
            {
                __axIsVideoAutoplayEnabledDidChangeNotificationDelegate();
            }
            return;
        }
    }
    if ( [notification.name isEqualToString:UIAccessibilityDarkerSystemColorsStatusDidChangeNotification] )
    {
        if ( __axDarkerSystemColorsEnabledDidChangeNotificationDelegate != NULL )
        {
            __axDarkerSystemColorsEnabledDidChangeNotificationDelegate();
        }
        return;
    }
    if ( [notification.name isEqualToString:UIAccessibilitySpeakSelectionStatusDidChangeNotification] )
    {
        if ( __axIsSpeakSelectionEnabledDidChangeNotificationDelegate != NULL )
        {
            __axIsSpeakSelectionEnabledDidChangeNotificationDelegate();
        }
        return;
    }
    if ( [notification.name isEqualToString:UIAccessibilitySpeakScreenStatusDidChangeNotification] )
    {
        if ( __axIsSpeakScreenEnabledDidChangeNotificationDelegate != NULL )
        {
            __axIsSpeakScreenEnabledDidChangeNotificationDelegate();
        }
        return;
    }
    if ( [notification.name isEqualToString:UIAccessibilityShakeToUndoDidChangeNotification] )
    {
        if ( __axIsShakeToUndoEnabledDidChangeNotificationDelegate != NULL )
        {
            __axIsShakeToUndoEnabledDidChangeNotificationDelegate();
        }
        return;
    }
    if ( @available(iOS 13.0, tvOS 13.0, *) )
    {
        if ( [notification.name isEqualToString:UIAccessibilityShouldDifferentiateWithoutColorDidChangeNotification] )
        {
            if ( __axShouldDifferentiateWithoutColorDidChangeNotificationDelegate != NULL ) {
                __axShouldDifferentiateWithoutColorDidChangeNotificationDelegate();
            }
            return;
        }
    }
    if ( @available(iOS 13.0, tvOS 13.0, *) )
    {
        if ( [notification.name isEqualToString:UIAccessibilityOnOffSwitchLabelsDidChangeNotification] )
        {
            if ( __axIsOnOffSwitchLabelsEnabledDidChangeNotificationDelegate != NULL ) {
                __axIsOnOffSwitchLabelsEnabledDidChangeNotificationDelegate();
            }
            return;
        }
    }
}
@end

static _AppleAccessibilityNotificationObserver *_observer;

APPLE_ACCESSIBILITY_EXTERN void _UnityAX_InitializeAXRuntime(void)
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _AppleAccessibilitySafeOverrideInstall(@"UnityViewAccessibility");

        _observer = [[_AppleAccessibilityNotificationObserver alloc] init];
        NSNotificationCenter *center = [NSNotificationCenter defaultCenter];
#if TARGET_OS_IOS
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIContentSizeCategoryDidChangeNotification object:nil];
#endif
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityVoiceOverStatusChanged object:nil];
#pragma clang diagnostic pop
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilitySwitchControlStatusDidChangeNotification object:nil];
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityMonoAudioStatusDidChangeNotification object:nil];
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityClosedCaptioningStatusDidChangeNotification object:nil];
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityInvertColorsStatusDidChangeNotification object:nil];
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityGuidedAccessStatusDidChangeNotification object:nil];
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityBoldTextStatusDidChangeNotification object:nil];
        if ( @available(iOS 14.0, tvOS 14.0, *) )
        {
            [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityButtonShapesEnabledStatusDidChangeNotification object:nil];
        }
        if ( @available(iOS 14.0, tvOS 14.0, *) )
        {
            [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityPrefersCrossFadeTransitionsStatusDidChangeNotification object:nil];
        }
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityGrayscaleStatusDidChangeNotification object:nil];
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityReduceTransparencyStatusDidChangeNotification object:nil];
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityReduceMotionStatusDidChangeNotification object:nil];
        if ( @available(iOS 13.0, tvOS 13.0, *) )
        {
            [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityVideoAutoplayStatusDidChangeNotification object:nil];
        }
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityDarkerSystemColorsStatusDidChangeNotification object:nil];
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilitySpeakSelectionStatusDidChangeNotification object:nil];
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilitySpeakScreenStatusDidChangeNotification object:nil];
        [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityShakeToUndoDidChangeNotification object:nil];
        if ( @available(iOS 13.0, tvOS 13.0, *) )
        {
            [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityShouldDifferentiateWithoutColorDidChangeNotification object:nil];
        }
        if ( @available(iOS 13.0, tvOS 13.0, *) )
        {
            [center addObserver:_observer selector:@selector(_accessibilityStatusChangedCallback:) name:UIAccessibilityOnOffSwitchLabelsDidChangeNotification object:nil];
        }

        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityFrame:^CGRect(NSNumber *identifier) {
            if ( __axFrameDelegate == NULL )
            {
                return CGRectZero;
            }
            char *rectStr = __axFrameDelegate((int32_t)identifier.integerValue);
            return rectStr == NULL ? CGRectZero : CGRectFromString([NSString stringWithUTF8String:rectStr]);
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityLabel:^NSString *(NSNumber *identifier) {
            if ( __axLabelDelegate == NULL )
            {
                return nil;
            }
            char *str = __axLabelDelegate((int32_t)identifier.integerValue);
            return str == NULL ? nil : [NSString stringWithUTF8String:str];
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityTraits:^UIAccessibilityTraits(NSNumber *identifier) {
            if ( __axTraitsDelegate == NULL )
            {
                return (uint64_t)0;
            }
            return __axTraitsDelegate((int32_t)identifier.integerValue);
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityCustomActionsCount:^uint64_t(NSNumber *identifier) {
            if ( __axCustomActionsCountDelegate == NULL )
            {
                return (uint64_t)0;
            }
            return __axCustomActionsCountDelegate((int32_t)identifier.integerValue);
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityPerformCustomAction:^BOOL(NSNumber *identifier, NSNumber *idx) {
            if ( __axPerformCustomActionDelegate == NULL )
            {
                return NO;
            }
            return __axPerformCustomActionDelegate((int32_t)identifier.integerValue, (int32_t)idx.integerValue);
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityCustomActionName:^NSString *(NSNumber *identifier, NSNumber *idx) {
            if ( __axCustomActionNameDelegate == NULL )
            {
                return nil;
            }
            char *str = __axCustomActionNameDelegate((int32_t)identifier.integerValue, (int32_t)idx.integerValue);
            return str == NULL ? nil : [NSString stringWithUTF8String:str];
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityIsAccessibilityElement:^BOOL(NSNumber *identifier) {
            if ( __axIsElementDelegate == NULL )
            {
                return YES;
            }
            return __axIsElementDelegate((int32_t)identifier.integerValue);
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityHint:^NSString *(NSNumber *identifier) {
            if ( __axHintDelegate == NULL )
            {
                return nil;
            }
            char *str = __axHintDelegate((int32_t)identifier.integerValue);
            return str == NULL ? nil : [NSString stringWithUTF8String:str];
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityValue:^NSString *(NSNumber *identifier) {
            if ( __axValueDelegate == NULL )
            {
                return nil;
            }
            char *str = __axValueDelegate((int32_t)identifier.integerValue);
            return str == NULL ? nil : [NSString stringWithUTF8String:str];
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityActivationPoint:^CGPoint(NSNumber *identifier) {
            if ( __axActivationPointDelegate == NULL )
            {
                return kAXCenterPointDefaultPoint;
            }
            char *pointStr = __axActivationPointDelegate((int32_t)identifier.integerValue);
            return pointStr == NULL ? kAXCenterPointDefaultPoint : CGPointFromString([NSString stringWithUTF8String:pointStr]);
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityIdentifier:^NSString *(NSNumber *identifier) {
            if ( __axIdentifierDelegate == NULL )
            {
                return nil;
            }
            char *str = __axIdentifierDelegate((int32_t)identifier.integerValue);
            return str == NULL ? nil : [NSString stringWithUTF8String:str];
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityViewIsModal:^BOOL(NSNumber *identifier) {
            if ( __axViewIsModalDelegate == NULL )
            {
                return NO;
            }
            return __axViewIsModalDelegate((int32_t)identifier.integerValue);
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityScroll:^BOOL(NSNumber *identifier, UIAccessibilityScrollDirection direction) {
            if ( __axScrollDelegate == NULL )
            {
                return NO;
            }
            return __axScrollDelegate((int32_t)identifier.integerValue, (int32_t)direction);
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityPerformMagicTap:^BOOL(NSNumber *identifier) {
            if ( __axPerformMagicTapDelegate == NULL )
            {
                return NO;
            }
            return __axPerformMagicTapDelegate((int32_t)identifier.integerValue);
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityPerformEscape:^BOOL(NSNumber *identifier) {
            if ( __axPerformEscapeDelegate == NULL )
            {
                return NO;
            }
            return __axPerformEscapeDelegate((int32_t)identifier.integerValue);
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityActivate:^BOOL(NSNumber *identifier) {
            if ( __axActivateDelegate == NULL )
            {
                return NO;
            }
            return __axActivateDelegate((int32_t)identifier.integerValue);
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityIncrement:^void(NSNumber *identifier) {
            if ( __axIncrementDelegate == NULL )
            {
                return;
            }
            __axIncrementDelegate((int32_t)identifier.integerValue);
        }];
        [AppleAccessibilityRuntime.sharedInstance setUnityAccessibilityDecrement:^void(NSNumber *identifier) {
            if ( __axDecrementDelegate == NULL )
            {
                return;
            }
            __axDecrementDelegate((int32_t)identifier.integerValue);
        }];
    });
}

APPLE_ACCESSIBILITY_EXTERN void _UnityAX_PostUnityViewChanged(void)
{
    UIAccessibilityPostNotification(UIAccessibilityScreenChangedNotification, nil);
}

APPLE_ACCESSIBILITY_EXTERN void _UnityAX_RegisterElementWithIdentifier(int32_t identifier, int32_t parentIdentifier, bool hasParent)
{
    [AppleAccessibilityRuntime.sharedInstance registerAccessibilityElementWithIdentifier:@(identifier) parent: hasParent ? @(parentIdentifier) : nil hasParent:hasParent];
}

APPLE_ACCESSIBILITY_EXTERN void _UnityAX_UnregisterElementWithIdentifier(int32_t identifier)
{
    [AppleAccessibilityRuntime.sharedInstance unregisterAccessibilityElementWithIdentifier:@(identifier)];
}

APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_RuniOSSideUnitTestWithName(const char *name)
{
    return [AppleAccessibilityRuntime.sharedInstance runUnitTestWithName:[NSString stringWithUTF8String:name]];
}

APPLE_ACCESSIBILITY_EXTERN bool _UnityAX_RuniOSSideUnitTestWithKeyPathExpectingStringResult(int32_t identifier, const char *keyPath, const char *expected)
{
    return [AppleAccessibilityRuntime.sharedInstance runUnitTestForIdentifier:@(identifier) keyPath:[NSString stringWithUTF8String:keyPath] expected:[NSString stringWithUTF8String:expected]];
}
