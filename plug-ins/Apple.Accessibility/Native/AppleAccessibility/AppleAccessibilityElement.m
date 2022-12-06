//
//  AppleAccessibilityElement.m
//  AppleAccessibility
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#import "AppleAccessibilityElement.h"
#import "AppleAccessibilityRuntime.h"
#import "AppleAccessibilityElementOrdering.h"

@implementation AppleAccessibilityElement

- (CGRect)accessibilityFrame
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityFrame != nil )
    {
        CGRect rect = [AppleAccessibilityRuntime sharedInstance].unityAccessibilityFrame(self.identifier);
        CGFloat scale = [[UIScreen mainScreen] nativeScale];
        rect.origin.x /= scale;
        rect.origin.y /= scale;
        rect.size.width /= scale;
        rect.size.height /= scale;
        return rect;
    }
    return [super accessibilityFrame];
}

- (NSString *)accessibilityLabel
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityLabel != nil )
    {
        return [AppleAccessibilityRuntime sharedInstance].unityAccessibilityLabel(self.identifier);
    }
    return [super accessibilityLabel];
}

- (UIAccessibilityTraits)accessibilityTraits
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityTraits != nil )
    {
        return [AppleAccessibilityRuntime sharedInstance].unityAccessibilityTraits(self.identifier);
    }
    return [super accessibilityTraits];
}

- (BOOL)isAccessibilityElement
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityIsAccessibilityElement != nil )
    {
        return [AppleAccessibilityRuntime sharedInstance].unityIsAccessibilityElement(self.identifier);
    }
    return [super isAccessibilityElement];
}

- (NSString *)accessibilityHint
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityHint != nil )
    {
        return [AppleAccessibilityRuntime sharedInstance].unityAccessibilityHint(self.identifier);
    }
    return [super accessibilityHint];
}

- (NSString *)accessibilityValue
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityValue != nil )
    {
        return [AppleAccessibilityRuntime sharedInstance].unityAccessibilityValue(self.identifier);
    }
    return [super accessibilityValue];
}

- (NSString *)accessibilityIdentifier
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityIdentifier != nil )
    {
        return [AppleAccessibilityRuntime sharedInstance].unityAccessibilityIdentifier(self.identifier);
    }
    return [super accessibilityIdentifier];
}

- (BOOL)accessibilityViewIsModal
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityViewIsModal != nil )
    {
        return [AppleAccessibilityRuntime sharedInstance].unityAccessibilityViewIsModal(self.identifier);
    }
    return [super accessibilityViewIsModal];
}

- (CGPoint)accessibilityActivationPoint
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityActivationPoint != nil )
    {
        CGPoint point = [AppleAccessibilityRuntime sharedInstance].unityAccessibilityActivationPoint(self.identifier);
        if ( !CGPointEqualToPoint(point, kAXCenterPointDefaultPoint) )
        {
            return point;
        }
    }
    return [super accessibilityActivationPoint];
}

- (NSArray *)accessibilityElements
{
    NSArray *allElements = [AppleAccessibilityRuntime.sharedInstance elements];
    NSMutableArray *elements = [NSMutableArray array];
    for ( AccessibilityElementIndexPair *pair in allElements )
    {
        if ( [pair.element.parent isEqualToNumber:self.identifier] )
        {
            [elements addObject:pair.element];
        }
    }
    NSArray *newArray = _AccessibilityElementOrdering(elements, _AccessibilityElementOrderingFrameGetter);
    newArray = _AccessibilityElementModalFiltering(newArray, _AccessibilityElementModalFilteringGetter);
    return newArray;
}

- (BOOL)accessibilityScroll:(UIAccessibilityScrollDirection)direction
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityScroll != nil )
    {
        return [AppleAccessibilityRuntime sharedInstance].unityAccessibilityScroll(self.identifier, direction);
    }
    return [super accessibilityScroll:direction];
}

- (BOOL)accessibilityPerformMagicTap
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityPerformMagicTap != nil )
    {
        return [AppleAccessibilityRuntime sharedInstance].unityAccessibilityPerformMagicTap(self.identifier);
    }
    return [super accessibilityPerformMagicTap];
}

- (BOOL)accessibilityPerformEscape
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityPerformEscape != nil )
    {
        return [AppleAccessibilityRuntime sharedInstance].unityAccessibilityPerformEscape(self.identifier);
    }
    return [super accessibilityPerformEscape];
}

- (BOOL)accessibilityActivate
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityActivate != nil )
    {
        return [AppleAccessibilityRuntime sharedInstance].unityAccessibilityActivate(self.identifier);
    }
    return [super accessibilityActivate];
}

- (void)accessibilityIncrement
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityIncrement != nil )
    {
        return [AppleAccessibilityRuntime sharedInstance].unityAccessibilityIncrement(self.identifier);
    }
    return [super accessibilityIncrement];
}

- (void)accessibilityDecrement
{
    if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityDecrement != nil )
    {
        return [AppleAccessibilityRuntime sharedInstance].unityAccessibilityDecrement(self.identifier);
    }
    return [super accessibilityDecrement];
}

- (id)accessibilityContainer
{
    if ( self.parent != nil )
    {
        AccessibilityElementIndexPair *pair = [[AppleAccessibilityRuntime.sharedInstance identifierToElement] objectForKey:self.parent];
        if ( pair != nil && pair.element != nil )
        {
            return pair.element;
        }
    }
    return [AppleAccessibilityRuntime.sharedInstance topLevelUnityView];
}

- (NSArray<UIAccessibilityCustomAction *> *)accessibilityCustomActions
{
    if ( @available(iOS 13.0, tvOS 13.0, *) )
    {
        if ( [AppleAccessibilityRuntime sharedInstance].unityAccessibilityCustomActionsCount != nil )
        {
            NSMutableArray<UIAccessibilityCustomAction *> *actions = [NSMutableArray array];
            uint64_t actionsCount = [AppleAccessibilityRuntime sharedInstance].unityAccessibilityCustomActionsCount(self.identifier);
            for ( uint64_t i = 0; i < actionsCount; i++ )
            {
                NSString *name = [NSString stringWithFormat:@"%@", @(i)];
                if ( [AppleAccessibilityRuntime sharedInstance].unityCustomActionName != nil )
                {
                    name = [AppleAccessibilityRuntime sharedInstance].unityCustomActionName(self.identifier, @(i));
                }
                UIAccessibilityCustomAction *action = [[UIAccessibilityCustomAction alloc] initWithName:name actionHandler:^BOOL(UIAccessibilityCustomAction * _Nonnull customAction) {
                    if ( [AppleAccessibilityRuntime sharedInstance].unityPerformCustomAction != nil )
                    {
                        return [AppleAccessibilityRuntime sharedInstance].unityPerformCustomAction(self.identifier, @(i));
                    }
                    return NO;
                }];
                [actions addObject:action];
            }
            return actions;
        }
    }
    return [super accessibilityCustomActions];
}

@end

@implementation AccessibilityElementIndexPair

- (NSString *)description
{
    return [NSString stringWithFormat:@"%@: identifier: %d, parentIdentifier: %d, element: %@", super.description, self.identifier.intValue, self.parentIdentifier.intValue, self.element];
}

@end
