//
//  AppleAccessibilityElementOrdering.h
//  AppleAccessibility
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#import <CoreFoundation/CoreFoundation.h>
#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>
#import <AppleAccessibility/AppleAccessibilityBase.h>

NS_ASSUME_NONNULL_BEGIN

typedef CGRect (*AccessibilityElementOrderingFrameGetter)(id element);
APPLE_ACCESSIBILITY_HIDDEN NSArray *_Nullable _AccessibilityElementOrdering(NSArray *elements, AccessibilityElementOrderingFrameGetter frameGetter);

typedef BOOL (*AccessibilityElementModalGetter)(id element);
APPLE_ACCESSIBILITY_HIDDEN NSArray *_Nullable _AccessibilityElementModalFiltering(NSArray *elements, AccessibilityElementModalGetter modalGetter);

APPLE_ACCESSIBILITY_HIDDEN CGRect _AccessibilityElementOrderingFrameGetter(id object);
APPLE_ACCESSIBILITY_HIDDEN BOOL _AccessibilityElementModalFilteringGetter(id object);

NS_ASSUME_NONNULL_END
