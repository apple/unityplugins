//
//  AppleAccessibilityElement.h
//  AppleAccessibility
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface AppleAccessibilityElement : UIAccessibilityElement
@property (nonatomic, copy) NSNumber *identifier;
@property (nonatomic, copy) NSNumber *parent;
@property (nonatomic, strong) NSArray<UIAccessibilityCustomAction *> *actions;
@end

@interface AccessibilityElementIndexPair : NSObject
@property (nonatomic, copy) NSNumber *identifier;
@property (nonatomic, copy) NSNumber *parentIdentifier;
@property (nonatomic, strong) AppleAccessibilityElement *element;
@end

NS_ASSUME_NONNULL_END
