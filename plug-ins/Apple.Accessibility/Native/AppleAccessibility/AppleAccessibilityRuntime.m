//
//  AppleAccessibilityRuntime.m
//  AppleAccessibility
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "AppleAccessibilityBase.h"
#import "AppleAccessibilityRuntime.h"
#import "AppleAccessibilityElementOrdering.h"
#import "AppleAccessibilityElement.h"

@interface NSObject (UnityAccessibilityPrivate)
- (id)unityView;
@end

APPLE_ACCESSIBILITY_HIDDEN
@interface AppleAccessibilityElementData : NSObject
@property (nonatomic, copy) NSNumber *identifier;
@property (nonatomic, copy) NSNumber *parentIdentifier;
@end

@implementation AppleAccessibilityElementData
@end

@implementation AppleAccessibilityRuntime
{
    NSMutableArray<AccessibilityElementIndexPair *> *_elements;
    NSMutableDictionary<NSNumber *, AccessibilityElementIndexPair *> *_identifierToElement;
}

+ (instancetype)sharedInstance
{
    static AppleAccessibilityRuntime *__sharedInstance;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        __sharedInstance = [[AppleAccessibilityRuntime alloc] init];
    });
    return __sharedInstance;
}

- (NSMutableArray<AccessibilityElementIndexPair *> *)elements
{
    return _elements;
}

- (NSMutableDictionary<NSNumber *, AccessibilityElementIndexPair *> *)identifierToElement
{
    return _identifierToElement;
}

- (instancetype)init
{
    self = [super init];
    _elements = [NSMutableArray array];
    _identifierToElement = [NSMutableDictionary dictionary];
    return self;
}

- (AccessibilityElementIndexPair *)_accessibilityElementIndexPairForIdentifier:(NSNumber *)identifier
{
    AppleAccessibilityElement *element = [[AppleAccessibilityElement alloc] initWithAccessibilityContainer:self.topLevelUnityView];
    element.identifier = identifier;

    AccessibilityElementIndexPair *pair = [[AccessibilityElementIndexPair alloc] init];
    pair.element = element;
    pair.identifier = identifier;
    return pair;
}

- (void)registerAccessibilityElementWithIdentifier:(NSNumber *)identifier parent:(NSNumber *)parentIdentifier hasParent:(BOOL)hasParent
{
    AccessibilityElementIndexPair *pair = [_identifierToElement objectForKey:identifier];
    if ( pair == nil )
    {
        pair = [self _accessibilityElementIndexPairForIdentifier:identifier];
        [_identifierToElement setObject:pair forKey:identifier];
        [_elements addObject:pair];
    }

    AccessibilityElementIndexPair *parentPair = hasParent ? [_identifierToElement objectForKey:parentIdentifier] : nil;
    if ( hasParent && parentPair == nil )
    {
        parentPair = [self _accessibilityElementIndexPairForIdentifier:parentIdentifier];
        [_identifierToElement setObject:parentPair forKey:parentIdentifier];
        [_elements addObject:parentPair];
    }

    if ( hasParent )
    {
        pair.parentIdentifier = parentIdentifier;
        pair.element.parent = parentIdentifier;
        NSArray *children = parentPair.element.accessibilityElements ?: NSArray.new;
        NSMutableArray *mutable = [children mutableCopy];
        [mutable addObject:pair.element];

        NSArray *newArray = _AccessibilityElementOrdering(mutable, _AccessibilityElementOrderingFrameGetter);
        newArray = _AccessibilityElementModalFiltering(newArray, _AccessibilityElementModalFilteringGetter);
    }
}

- (void)unregisterAccessibilityElementWithIdentifier:(NSNumber *)identifier
{
    AccessibilityElementIndexPair *element = _identifierToElement[identifier];
    if ( element != nil )
    {
        [_elements removeObject:element];
    }
    [_identifierToElement removeObjectForKey:identifier];
}

- (id)topLevelUnityView
{
    id appController = [[UIApplication sharedApplication] delegate];
    if ( [appController isKindOfClass:NSClassFromString(@"UnityAppController")] && [appController respondsToSelector:@selector(unityView)] )
    {
        return [appController unityView];
    }
    return nil;
}

- (BOOL)isAccessibilityEnabledForUnityView:(id)unityView
{
    return _elements.count > 0;
}

- (NSArray *)accessibilityChildrenForUnityView:(id)unityView
{
    NSMutableArray *elements = [NSMutableArray array];
    for ( AccessibilityElementIndexPair *pair in _elements )
    {
        if ( pair.element.parent == nil )
        {
            [elements addObject:pair.element];
        }
    }
    NSArray *newArray = _AccessibilityElementOrdering(elements, _AccessibilityElementOrderingFrameGetter);
    newArray = _AccessibilityElementModalFiltering(elements, _AccessibilityElementModalFilteringGetter);
    return newArray;
}

@end

@implementation AppleAccessibilityRuntime(Testing)

- (BOOL)runUnitTestWithName:(NSString *)name
{
    SEL testSel = NSSelectorFromString(name);
    if ( [self respondsToSelector:testSel] )
    {
        IMP testImp = [self methodForSelector:testSel];
        if ( testImp )
        {
            bool (*testFunc)(id, SEL) = (void *)testImp;
            return testFunc(self, testSel);
        }
    }
    return false;
}

- (BOOL)runUnitTestForIdentifier:(NSNumber *)identifier keyPath:(NSString *)keyPath expected:(NSString *)expected
{
    for ( AccessibilityElementIndexPair *pair in _elements )
    {
        if ( [pair.element.identifier isEqual:identifier] )
        {
            return [[pair.element valueForKeyPath:keyPath] isEqualToString:expected];
        }
    }
    return NO;
}

- (BOOL)test_SmokeTestSucceeded
{
    return true;
}

- (BOOL)test_SmokeTestFailed
{
    return false;
}

#if 0
- (BOOL)test_SmokeTestUnimplemented
{
    return false;
}
#endif

- (BOOL)_test_elementNamedButtonExists
{
    for ( AccessibilityElementIndexPair *pair in _elements )
    {
        if ( [pair.element.accessibilityLabel isEqualToString:@"Button"] )
        {
            return YES;
        }
    }
    return NO;
}

- (BOOL)_test_anyElementsExist
{
    return [_elements count] > 0;
}

- (BOOL)_test_performMagicTapOnButton
{
    for ( AccessibilityElementIndexPair *pair in _elements )
    {
        if ( [pair.element.accessibilityLabel isEqualToString:@"Button"] )
        {
            BOOL succeeded = [pair.element accessibilityPerformMagicTap];
            return succeeded;
        }
    }
    return false;
}

- (BOOL)_test_performEscapeOnButton
{
    for ( AccessibilityElementIndexPair *pair in _elements )
    {
        if ( [pair.element.accessibilityLabel isEqualToString:@"Button"] )
        {
            BOOL succeeded = [pair.element accessibilityPerformEscape];
            return succeeded;
        }
    }
    return false;
}

- (BOOL)_test_AccessibilityIncrementOnButton
{
    for ( AccessibilityElementIndexPair *pair in _elements )
    {
        if ( [pair.element.accessibilityLabel isEqualToString:@"Button"] )
        {
            [pair.element accessibilityIncrement];
            return true;
        }
    }
    return false;
}

- (BOOL)_test_AccessibilityDecrementOnButton
{
    for ( AccessibilityElementIndexPair *pair in _elements )
    {
        if ( [pair.element.accessibilityLabel isEqualToString:@"Button"] )
        {
            [pair.element accessibilityDecrement];
            return true;
        }
    }
    return false;
}


- (BOOL)test_elementWithId:(NSNumber *)identifier hasAccessibilityLabel:(NSString *)label
{
    return NO;
}

@end
