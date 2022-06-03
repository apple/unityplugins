//
//  UnityViewAccessibility.m
//  AppleAccessibility
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <objc/runtime.h>
#import "AppleAccessibilityRuntime.h"
#import "AppleAccessibilitySafeOverride.h"

AppleAccessibilityDefineSafeOverride(@"UnityView", UnityViewAccessibility)

@implementation UnityViewAccessibility

// by default Unity engine sets this to YES
- (BOOL)isAccessibilityElement
{
    if ( [AppleAccessibilityRuntime.sharedInstance isAccessibilityEnabledForUnityView:self] )
    {
        return NO;
    }
    return [super isAccessibilityElement];
}

// by default Unity engine sets this to direct touch container, so we need to reset
- (UIAccessibilityTraits)accessibilityTraits
{
    if ( [AppleAccessibilityRuntime.sharedInstance isAccessibilityEnabledForUnityView:self] )
    {
        return UIAccessibilityTraitNone;
    }
    return [super accessibilityTraits];
}

- (NSArray *)accessibilityElements
{
    return [AppleAccessibilityRuntime.sharedInstance accessibilityChildrenForUnityView:self];
}

@end
