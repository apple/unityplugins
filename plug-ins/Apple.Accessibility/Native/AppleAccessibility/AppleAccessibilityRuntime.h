//
//  AppleAccessibilityRuntime.h
//  AppleAccessibility
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@protocol UnityViewDelegate

- (BOOL)isAccessibilityEnabledForUnityView:(id)unityView;
- (NSArray *)accessibilityChildrenForUnityView:(id)unityView;

@end

@interface AppleAccessibilityRuntime : NSObject<UnityViewDelegate>

+ (instancetype)sharedInstance;

@property (nonatomic, copy) CGRect (^unityAccessibilityFrame)(NSNumber *identifier);
@property (nonatomic, copy) NSString *(^unityAccessibilityLabel)(NSNumber *identifier);
@property (nonatomic, copy) UIAccessibilityTraits (^unityAccessibilityTraits)(NSNumber *identifier);
@property (nonatomic, copy) BOOL (^unityIsAccessibilityElement)(NSNumber *identifier);
@property (nonatomic, copy) NSString *(^unityAccessibilityHint)(NSNumber *identifier);
@property (nonatomic, copy) NSString *(^unityAccessibilityValue)(NSNumber *identifier);
@property (nonatomic, copy) NSString *(^unityAccessibilityIdentifier)(NSNumber *identifier);
@property (nonatomic, copy) BOOL (^unityAccessibilityViewIsModal)(NSNumber *identifier);
@property (nonatomic, copy) CGPoint (^unityAccessibilityActivationPoint)(NSNumber *identifier);
@property (nonatomic, copy) BOOL (^unityAccessibilityScroll)(NSNumber *identifier, UIAccessibilityScrollDirection direction);
@property (nonatomic, copy) BOOL (^unityAccessibilityPerformMagicTap)(NSNumber *identifier);
@property (nonatomic, copy) BOOL (^unityAccessibilityPerformEscape)(NSNumber *identifier);
@property (nonatomic, copy) BOOL (^unityAccessibilityActivate)(NSNumber *identifier);
@property (nonatomic, copy) void (^unityAccessibilityIncrement)(NSNumber *identifier);
@property (nonatomic, copy) void (^unityAccessibilityDecrement)(NSNumber *identifier);
@property (nonatomic, copy) uint64_t (^unityAccessibilityCustomActionsCount)(NSNumber *identifier);
@property (nonatomic, copy) BOOL (^unityPerformCustomAction)(NSNumber *identifier, NSNumber *idx);
@property (nonatomic, copy) NSString *(^unityCustomActionName)(NSNumber *identifier, NSNumber *idx);

- (void)registerAccessibilityElementWithIdentifier:(NSNumber *)identifier parent:(NSNumber *)parent hasParent:(BOOL)hasParent;
- (void)unregisterAccessibilityElementWithIdentifier:(NSNumber *)identifier;
- (id)topLevelUnityView;
- (NSMutableArray *)elements;
- (NSMutableDictionary<NSNumber *, id> *)identifierToElement;

@end

@interface AppleAccessibilityRuntime(Testing)
- (BOOL)runUnitTestWithName:(NSString *)name;
- (BOOL)runUnitTestForIdentifier:(NSNumber *)identifier keyPath:(NSString *)keyPath expected:(NSString *)expected;
@end
