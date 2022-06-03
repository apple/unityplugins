//
//  AppleAccessibilityElementOrdering.m
//  AppleAccessibility
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#import "AppleAccessibilityElementOrdering.h"
#import <UIKit/UIKit.h>

#define AXSafeRetain(x) { if (x) CFRetain(x); }
#define AXSafeRelease(x) { if (x) { CFRelease(x); x = NULL; } }

#define _AXRangeMax(range) (range.location + range.length)
#define _AXRangeMid(range) (range.location + range.length/2)
#define _AXAlmostEqual(Prefix, Type, Abs, Eps)              \
bool _AX ## Prefix ## AlmostEqual(Type lhs, Type rhs) {     \
    if (isnan(lhs) || isnan(rhs))                           \
        return false;                                       \
    Type diff = Abs(lhs-rhs);                               \
    Type epsilon = Eps;                                     \
    if (diff <= epsilon)                                    \
        return true;                                        \
    return false;                                           \
}

_AXAlmostEqual(Float, float, fabsf, FLT_EPSILON)
_AXAlmostEqual(Double, double, fabs, DBL_EPSILON)

static bool _AXCGFloatAlmostEqual(CGFloat lhs, CGFloat rhs)
{
#if CGFLOAT_IS_DOUBLE
    return _AXDoubleAlmostEqual(lhs, rhs);
#else
    return _AXFloatAlmostEqual(lhs, rhs);
#endif
}

typedef struct
{
    AccessibilityElementOrderingFrameGetter frameGetter;
    CFMutableDictionaryRef navigationOrderFrameCache;
} _AXNavigationOrderCompareContext;

// This will keep the range as long as possible, but won't allow ranges to overlap
// (Previous range will be shrunk when necessary so as not to overlap indexed range,
//  and indexed range will be shrunk so as not to overlap the following range.
static void __AXNavigationOrderConstrainRange(CFIndex index, CFArrayRef ranges, CFRange range)
{
    const CFRange rangeSizeRange = CFRangeMake(0, sizeof(CFRange));
    CFIndex rangeCount = CFArrayGetCount(ranges);

    CFRange rangeToConstrain;
    CFMutableDataRef rangeToConstrainRef = (CFMutableDataRef)CFArrayGetValueAtIndex(ranges, index);
    CFDataGetBytes(rangeToConstrainRef, rangeSizeRange, (UInt8 *)&rangeToConstrain);

    // Update this range (move this range's start location back,
    // causing the previous range to be shortened, a bit down below)
    CFIndex originDiff = rangeToConstrain.location - range.location;
    if (originDiff > 0)
    {
        rangeToConstrain.location = range.location;
        rangeToConstrain.length += originDiff;
    }

    // Make sure it doesn't overlap the following range
    CFIndex rangeEnd = _AXRangeMax(range);
    CFIndex endDiff = rangeEnd - _AXRangeMax(rangeToConstrain);
    if (endDiff > 0)
    {
        CFRange rangeToRight = { LONG_MAX, 0 };

        // Get the following range (default to zero-width range)
        if (index < (rangeCount - 1))
        {
            CFMutableDataRef rangeToRightRef = (CFMutableDataRef)CFArrayGetValueAtIndex(ranges, index + 1);
            CFDataGetBytes(rangeToRightRef, rangeSizeRange, (UInt8 *)&rangeToRight);
        }

        // Increase this range's width to match up to the following range
        if (rangeEnd < rangeToRight.location)
        {
            rangeToConstrain.length += endDiff;
        }
        // Shrink this range's width to not overlap the following range
        else
        {
            rangeToConstrain.length = rangeToRight.location - rangeToConstrain.location - 1;
        }
    }

    CFDataReplaceBytes(rangeToConstrainRef, rangeSizeRange, (UInt8 *)&rangeToConstrain, sizeof(rangeToConstrain));

    // Make sure the previous range doesn't overlap our new one
    if (index)
    {
        CFRange rangeToLeft;
        CFMutableDataRef rangeToLeftRef = (CFMutableDataRef)CFArrayGetValueAtIndex(ranges, index - 1);
        CFDataGetBytes(rangeToLeftRef, rangeSizeRange, (UInt8 *)&rangeToLeft);
        if (_AXRangeMax(rangeToLeft) >= rangeToConstrain.location)
        {
            rangeToLeft.length = rangeToConstrain.location - rangeToLeft.location - 1;
        }

        CFDataReplaceBytes(rangeToLeftRef, rangeSizeRange, (UInt8 *)&rangeToLeft, sizeof(rangeToLeft));
    }
}

// There must be at least one column in the array of columns! You must insert a
// new range (with _insertRange) to the columnRanges after inserting a new column!!
static CFMutableArrayRef __AXNavigationOrderCopyInsertedEmptyColumn(CFMutableArrayRef columns, CFIndex index) CF_RETURNS_NOT_RETAINED
{
    if (columns == NULL)
    {
        return NULL;
    }

    CFIndex rowCount = CFArrayGetCount((CFArrayRef)CFArrayGetValueAtIndex(columns, 0));
    CFMutableArrayRef column = CFArrayCreateMutable(kCFAllocatorDefault, 0, &kCFTypeArrayCallBacks);
    CFArrayInsertValueAtIndex(columns, index, column);
    CFRelease(column);

    // Fill out the new column with empty rows
    for (CFIndex emptyRowIndex = 0; emptyRowIndex < rowCount; emptyRowIndex++)
    {
        CFArrayAppendValue(column, kCFNull);
    }

    return column;
}

static NSArray *__AXNavigationOrderCopyIntoLists(CFArrayRef columns, CFArrayRef columnRanges, CFArrayRef rowRanges)
{
    CFIndex rowIndex, columnIndex;
    CFIndex rowCount = CFArrayGetCount(rowRanges);
    CFIndex columnCount = CFArrayGetCount(columnRanges);
    CFTypeRef indexedElement;

    // Order the children horizontally in accordance with the current system locale,
    // so that if the character direction is right to left, the children are ordered
    // starting at the top-right corner as opposed to the top-left corner, and wraps
    // down when reaching the left edge of a row as opposed to the right edge, and
    // wraps left when reaching the bottom edge of a column as opposed to the top edge.
    bool isLocaleCharacterDirectionRightToLeft = false;
    CFLocaleRef currentLocale = CFLocaleCopyCurrent();
    if (currentLocale != NULL)
    {
        CFStringRef isoLangCode = CFLocaleGetIdentifier(currentLocale);
        if (isoLangCode != NULL)
        {
            isLocaleCharacterDirectionRightToLeft = (CFLocaleGetLanguageCharacterDirection(isoLangCode) == kCFLocaleLanguageDirectionRightToLeft);
        }
        CFRelease(currentLocale);
    }

    // Copy the horizontal and vertical ordered arrays into the navigation order dictionary
    NSMutableArray *list = [NSMutableArray array];
    for (rowIndex = 0; rowIndex < rowCount; rowIndex++)
    {
        for (columnIndex = 0; columnIndex < columnCount; columnIndex++)
        {
            CFIndex columnIndexAdjustedForCharacterDirection = (isLocaleCharacterDirectionRightToLeft ? columnCount - 1 - columnIndex : columnIndex);
            indexedElement = CFArrayGetValueAtIndex((CFMutableArrayRef)CFArrayGetValueAtIndex(columns, columnIndexAdjustedForCharacterDirection), rowIndex);
            if (CFGetTypeID(indexedElement) != CFNullGetTypeID())
            {
                [list addObject:(__bridge id)indexedElement];
            }
        }
    }

    return list;
}

static inline CGRect __AXNavigationOrderFrameFromCustomGetter(CFTypeRef element, AccessibilityElementOrderingFrameGetter frameGetter, CFMutableDictionaryRef navigationOrderFrameCache)
{
    CGRect elementFrame = CGRectZero;

    // Check if the frame is cached
    CFDataRef frameData = NULL;
    if (navigationOrderFrameCache != NULL)
    {
        frameData = CFDictionaryGetValue(navigationOrderFrameCache, element);
    }

    if (frameData != NULL)
    {
        CFDataGetBytes(frameData, CFRangeMake(0, sizeof(CGRect)), (UInt8 *)&elementFrame);
    }
    else
    {
        // Frame is not cached, get and cache it
        elementFrame = frameGetter((__bridge id)element);

        frameData = CFDataCreate(kCFAllocatorDefault, (const UInt8 *)&elementFrame, sizeof(elementFrame));
        if (frameData != NULL)
        {
            if (navigationOrderFrameCache != NULL)
            {
                CFDictionaryAddValue(navigationOrderFrameCache, element, frameData);
            }
            CFRelease(frameData);
        }
    }

    return elementFrame;
}

static inline CFComparisonResult __AXNavigationOrderCompareUIElementFrames(CFTypeRef a, CFTypeRef b, void *context)
{
    _AXNavigationOrderCompareContext *compareContext = (_AXNavigationOrderCompareContext *)context;

    CGRect a_rect = __AXNavigationOrderFrameFromCustomGetter(a, compareContext->frameGetter, compareContext->navigationOrderFrameCache);
    CGRect b_rect = __AXNavigationOrderFrameFromCustomGetter(b, compareContext->frameGetter, compareContext->navigationOrderFrameCache);

    CFComparisonResult result = kCFCompareEqualTo;

    // whether rect a's center is "less than" (above or to the left of) b's center
    bool aCenterLessThanBCenter = (CGRectGetMidX(a_rect) < CGRectGetMidX(b_rect) || CGRectGetMidY(a_rect) < CGRectGetMidY(b_rect));
    bool bCenterLessThanACenter = (CGRectGetMidX(b_rect) < CGRectGetMidX(a_rect) || CGRectGetMidY(b_rect) < CGRectGetMidY(a_rect));

    if (CGRectIsNull(a_rect) || CGRectIsNull(b_rect))
    {
        result = kCFCompareEqualTo;
    }
    // check if a_rect and b_rect have the same origin
    else if (a_rect.origin.x == b_rect.origin.x)
    {
        if (a_rect.origin.y == b_rect.origin.y)
        {
            if (a_rect.size.width == b_rect.size.width)
            {
                if (a_rect.size.height == b_rect.size.height)
                {
                    result = kCFCompareEqualTo;
                }
                else if (a_rect.size.height < b_rect.size.height)
                {
                    result = kCFCompareLessThan;
                }
                else
                {
                    result = kCFCompareGreaterThan;
                }
            }
            else if (a_rect.size.width < b_rect.size.width)
            {
                result = kCFCompareLessThan;
            }
            else
            {
                result = kCFCompareGreaterThan;
            }
        }

        if (result)
        {
            // already determined the comparison result
        }
        // if b is completely inside of a and its center is < a, we want b to come first
        else if (CGRectContainsRect(a_rect, b_rect) && bCenterLessThanACenter)
        {
            result = kCFCompareGreaterThan;
        }
        // if a is completely inside of b and its center is < b, we want a to come first
        else if (CGRectContainsRect(b_rect, a_rect) && aCenterLessThanBCenter)
        {
            result = kCFCompareLessThan;
        }
        else if (a_rect.origin.y > b_rect.origin.y) // a's origin is below b's
        {
            result = kCFCompareGreaterThan;
        }
        else // a's origin is above b's
        {
            result = kCFCompareLessThan;
        }
    }
    if (result)
    {
        // already determined the comparison result
    }
    else if (a_rect.origin.x <= b_rect.origin.x) // a's origin is to the left of b's
    {
        if (a_rect.origin.y > b_rect.origin.y && CGRectGetMaxX(a_rect) >= b_rect.origin.x) // a's origin is below b's and a extends to the right of b
        {
            result = kCFCompareGreaterThan;
        }
        // if b is completely inside of a and its center is < a, we want b to come first
        else if (CGRectContainsRect(a_rect, b_rect) && bCenterLessThanACenter)
        {
            result = kCFCompareGreaterThan;
        }
        else
        {
            result = kCFCompareLessThan;
        }
    }
    else // a's origin is to the right of b's
    {
        if (a_rect.origin.y < b_rect.origin.y && CGRectGetMaxX(b_rect) >= a_rect.origin.x) // a's origin is above b's and b extends to the right of a
        {
            result = kCFCompareLessThan;
        }
        // if a is completely inside of b and its center is < b, we want a to come first
        else if (CGRectContainsRect(b_rect, a_rect) && aCenterLessThanBCenter)
        {
            result = kCFCompareLessThan;
        }
        else
        {
            result = kCFCompareGreaterThan;
        }
    }
    return result;
}

// You must insert a new range (with _insertRange) to the rowRanges after inserting
// a new row!!
static void __AXNavigationOrderInsertEmptyRowIntoColumns(CFMutableArrayRef columns, CFIndex index)
{
    CFIndex columnCount = CFArrayGetCount(columns);
    for (CFIndex columnIndex = 0; columnIndex < columnCount; columnIndex++)
    {
        CFArrayInsertValueAtIndex((CFMutableArrayRef)CFArrayGetValueAtIndex(columns, columnIndex), index, kCFNull);
    }
}

static void __AXNavigationOrderInsertRange(CFRange range, CFMutableArrayRef array, CFIndex index)
{
    CFMutableDataRef rangeRef = CFDataCreateMutable(kCFAllocatorDefault, sizeof(range));
    CFDataAppendBytes(rangeRef, (const UInt8 *)&range, sizeof(range));
    CFArrayInsertValueAtIndex(array, index, rangeRef);
    AXSafeRelease(rangeRef);
}

static void __AXNavigationOrderRebuildColumnWithIndex(CFIndex index, CFArrayRef columns, CFArrayRef ranges, AccessibilityElementOrderingFrameGetter frameGetter, CFMutableDictionaryRef navigationOrderFrameCache)
{
    CFMutableArrayRef column = (CFMutableArrayRef)CFArrayGetValueAtIndex(columns, index);
    CFIndex rowCount = CFArrayGetCount(column);

    if (rowCount > 0)
    {
        bool rangeInitialized = false;
        for (CFIndex rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            CFTypeRef element = CFArrayGetValueAtIndex(column, rowIndex);

            if (CFGetTypeID(element) != CFNullGetTypeID())
            {
                CGRect elementFrame = __AXNavigationOrderFrameFromCustomGetter(element, frameGetter, navigationOrderFrameCache);

                if (rangeInitialized)
                {
                    __AXNavigationOrderConstrainRange(index, ranges, CFRangeMake(elementFrame.origin.x, elementFrame.size.width));
                }
                else
                {
                    // We need to start the array out with the first range,
                    // though we need to honor the column to the right, if one.
                    CFRange range;
                    CFMutableDataRef rangeRef = (CFMutableDataRef)CFArrayGetValueAtIndex(ranges, index);
                    CFDataGetBytes(rangeRef, CFRangeMake(0, sizeof(range)), (UInt8 *)&range);
                    range.location = elementFrame.origin.x;
                    CFDataReplaceBytes(rangeRef, CFRangeMake(0, sizeof(range)), (const UInt8 *)&range, sizeof(range));
                    __AXNavigationOrderConstrainRange(index, ranges, CFRangeMake(elementFrame.origin.x, elementFrame.size.width));
                    rangeInitialized = true;
                }
            }
        }
    }
}

static CGFloat kAXNavigationOrderRequiredRowElementOverlapPercent = 0.20f;

// Returns the intersection of two ranges
static inline CFRange __AXNavigationOrderIntersectionRange(CFRange a, CFRange b)
{
    CFIndex start = MAX(a.location, b.location);
    CFIndex end = MIN(_AXRangeMax(a), _AXRangeMax(b));
    if (end <= start)
    {
        return CFRangeMake(0, 0);
    }
    else
    {
        return CFRangeMake(start, end - start);
    }
}

static inline CFComparisonResult __AXNavigationOrderElementHeightsOverlap(CFTypeRef element1, CFTypeRef element2, CFIndex *overlapGap, AccessibilityElementOrderingFrameGetter frameGetter, CFMutableDictionaryRef navigationOrderFrameCache)
{
    CFComparisonResult returnValue;

    CGRect element1Rect = __AXNavigationOrderFrameFromCustomGetter(element1, frameGetter, navigationOrderFrameCache);
    CGRect element2Rect = __AXNavigationOrderFrameFromCustomGetter(element2, frameGetter, navigationOrderFrameCache);

    CGFloat element1Height = element1Rect.size.height;
    CGFloat element2Height = element2Rect.size.height;
    CGFloat element1YPos = element1Rect.origin.y;
    CGFloat element2YPos = element2Rect.origin.y;

    // if they are the same element, return that they overlap (this comparison is
    // cheaper than isEqual)
    if (element1YPos == element2YPos && element1Height == element2Height)
    {
        return kCFCompareEqualTo;
    }

    // make sure that the current element overlap every other element in this row
    CFRange element1HeightRange = CFRangeMake(element1YPos, element1Height);
    CFRange element2HeightRange = CFRangeMake(element2YPos, element2Height);

    // determine if they overlap
    CFRange overLapRange = __AXNavigationOrderIntersectionRange(element1HeightRange, element2HeightRange);

    // if the top of Element 1 is below the top of element 2
    if (element1HeightRange.location > element2HeightRange.location)
    {
        returnValue = kCFCompareGreaterThan;
        *overlapGap = element1HeightRange.location - _AXRangeMax(element2HeightRange);
    }
    else
    {
        // the top of Element 2 is below the top of element 1
        returnValue = kCFCompareLessThan;
        *overlapGap = element2HeightRange.location - _AXRangeMax(element1HeightRange);
    }

    // see if they overlap
    if (overLapRange.location != 0)
    {
        // if they overlap by at requiredOverlap of either height, then consider
        // them sufficiently overlapping
        if (overLapRange.length > kAXNavigationOrderRequiredRowElementOverlapPercent * element1Height ||
            overLapRange.length > kAXNavigationOrderRequiredRowElementOverlapPercent * element2Height)
        {
            returnValue = kCFCompareEqualTo;
        }
    }

    return returnValue;
}

// ForceSplit is an important concept. If ForceSplit is YES, then child has not
// yet been added to the row, because there is an item in the location that we
// want to add child into. Therefore, we need to force the split, and insert child
// into the column forceSplitAddToColumn, and it needs to go in either row 1
// (addToTopRow==YES) or row 2 (addToTopRow==NO). The element in the existing
// location needs to be moved to the opposite row
static void __AXNavigationOrderAddElementToRowAndColumnWithForceSplit(CFTypeRef element, CFIndex rowIndex, CFIndex columnIndex, CFMutableArrayRef columns, CFMutableArrayRef rowRanges, CFIndex *rowCount, bool forceSplit, bool addToTopRow, AccessibilityElementOrderingFrameGetter frameGetter, CFMutableDictionaryRef navigationOrderFrameCache)
{
    // make sure that we were given a valid element
    if (element == NULL)
    {
        return;
    }

    // if we are not forcing a split, then the first thing we need to do is
    // actually add the element
    CGRect elementFrame = __AXNavigationOrderFrameFromCustomGetter(element, frameGetter, navigationOrderFrameCache);

    if (!forceSplit)
    {
        CFMutableArrayRef indexedColumn = (CFMutableArrayRef)CFArrayGetValueAtIndex(columns, columnIndex);
        CFArraySetValueAtIndex(indexedColumn, rowIndex, element);
        __AXNavigationOrderConstrainRange(rowIndex, rowRanges, CFRangeMake(elementFrame.origin.y, elementFrame.size.height));
    }

    CFIndex columnCount = CFArrayGetCount(columns);

    // grab the starting size of the row, we will want to make sure that if we
    // split the row that the two row still reside within this original range
    CFRange originalRowHeightRange;
    CFMutableDataRef originalRowHeightRangeRef = (CFMutableDataRef)CFArrayGetValueAtIndex(rowRanges, rowIndex);
    CFDataGetBytes(originalRowHeightRangeRef, CFRangeMake(0, sizeof(CFRange)), (UInt8 *)&originalRowHeightRange);

    bool needToSplitRow = forceSplit;

    // these are just used for convenience, firstRow is the original row,
    // secondRow is the newly added row (if one was added)
    CFIndex firstRowIndex = rowIndex;
    CFIndex secondRowIndex = rowIndex + 1;

    // if we are forcing a split, then we already know which row the child is
    // going to go into
    enum rowChoice
    {
        firstRow,
        secondRow
    } rowForChildWhenSplitting = addToTopRow ? firstRow : secondRow;

    // keep track of gap between child and the closest item that is not
    // sufficiently overlapping
    CFIndex smallestOverlapGap = LONG_MAX;

    // what column is the closest item in that is not sufficiently overlapping
    // and has the smallest gap
    bool foundSmallestOverlapColumn = false;
    CFIndex smallestOverlapColumn = 0;

    // iterate through all of the elements in this row, and make sure that child
    // sufficiently overlaps each one of them
    for (CFIndex i = 0; i < columnCount; i++)
    {
        CFTypeRef existingElement = CFArrayGetValueAtIndex(CFArrayGetValueAtIndex(columns, i), rowIndex);

        if (CFGetTypeID(existingElement) != CFNullGetTypeID())
        {
            CFIndex overlapGap = 0;
            CFComparisonResult overlapResult = __AXNavigationOrderElementHeightsOverlap(element, existingElement, &overlapGap, frameGetter, navigationOrderFrameCache);

            // keep track of the smallest gap, this is used if the newChild is going into the first
            // row so that we can determine where to split the row (at the top of the closest element
            // that is going into the second row)
            if (overlapResult != kCFCompareEqualTo && overlapGap < smallestOverlapGap)
            {
                smallestOverlapGap = overlapGap;
                smallestOverlapColumn = i;
                foundSmallestOverlapColumn = true;
            }

            // we don't need to check the overlap result if we already know that we are going
            // to split the row
            if (!needToSplitRow)
            {
                switch (overlapResult)
                {
                    case kCFCompareLessThan:
                        // child and element do not overlap + child is above element;
                        needToSplitRow = true;
                        rowForChildWhenSplitting = firstRow;
                        break;
                    case kCFCompareGreaterThan:
                        // child and element do not overlap + child is below element;
                        needToSplitRow = true;
                        rowForChildWhenSplitting = secondRow;
                        break;
                    case kCFCompareEqualTo:
                    default:
                        // child and element overlap, do nothing
                        break;
                };
            }
        }
    }

    // if we determined that we need to split this row, then now we need to determine
    // where to split the row, do the actual split, and then move elements accordingly
    if (needToSplitRow)
    {
        CGFloat splitLocation = 0;

        // first, determine where the split will occur
        if (rowForChildWhenSplitting == secondRow)
        {
            // the split will occur at the top of the new item
            splitLocation = elementFrame.origin.y;
        }
        else
        {
            // the split will occur at the top of the closest object that is not
            // sufficiently overlapping
            if (foundSmallestOverlapColumn)
            {
                CFTypeRef closestToOverlapElement = CFArrayGetValueAtIndex(CFArrayGetValueAtIndex(columns, smallestOverlapColumn), rowIndex);

                CGRect tempFrame = __AXNavigationOrderFrameFromCustomGetter(closestToOverlapElement, frameGetter, navigationOrderFrameCache);
                splitLocation = tempFrame.origin.y;
            }
            else
            {
                // all elements overlap, so we need to choose a somewhat arbitrary point because
                // we must be forcing a split.
                splitLocation = CGRectGetMaxY(elementFrame) - 1;
            }
        }

        // make sure that splitLocation is within the original row. occasionaly
        // it will be outside of the row (especially for hidden elements, for example
        // there is a splitter in mail with a height of -1 which can cause this)
        if (splitLocation <= originalRowHeightRange.location)
        {
            splitLocation = originalRowHeightRange.location + 1;
        }
        else if (splitLocation >= _AXRangeMax(originalRowHeightRange))
        {
            splitLocation = _AXRangeMax(originalRowHeightRange) - 1;
        }

        // Now, actually add the new row
        __AXNavigationOrderInsertEmptyRowIntoColumns(columns, secondRowIndex);

        // set the ranges appropriately
        CFRange firstRowHeightRange = CFRangeMake(originalRowHeightRange.location, splitLocation - originalRowHeightRange.location - 1);
        CFRange secondRowHeightRange = CFRangeMake(splitLocation, _AXRangeMax(originalRowHeightRange) - splitLocation);
        CFArrayRemoveValueAtIndex(rowRanges, firstRowIndex);
        __AXNavigationOrderInsertRange(firstRowHeightRange, rowRanges, firstRowIndex);
        __AXNavigationOrderInsertRange(secondRowHeightRange, rowRanges, secondRowIndex);

        // Now, simply go through all of the elements and add them to the proper row
        // this uses similar logic as the original method's row-determination
        for (CFIndex i = 0; i < columnCount; i++)
        {
            CFMutableArrayRef currentColumn = (CFMutableArrayRef)CFArrayGetValueAtIndex(columns, i);

            CFTypeRef existingElement = CFArrayGetValueAtIndex(currentColumn, rowIndex);
            if (CFGetTypeID(existingElement) != CFNullGetTypeID())
            {
                CGRect existingElementFrame = __AXNavigationOrderFrameFromCustomGetter(existingElement, frameGetter, navigationOrderFrameCache);

                if ((CGRectGetMidY(existingElementFrame) < _AXRangeMax(firstRowHeightRange)) ||
                    (existingElementFrame.origin.y < _AXRangeMid(firstRowHeightRange)))
                {
                    // keep in row 1
                }
                else
                {
                    // move to row 2;
                    CFArraySetValueAtIndex(currentColumn, secondRowIndex, existingElement);
                    CFArraySetValueAtIndex(currentColumn, firstRowIndex, kCFNull);
                }
            }

            // if we are forcing the split, then we also need to insert the new child
            // (reminder: if forceSplit==YES, then child has not yet been added to the
            // row because an element exists in the location that we want to insert child)
            if (forceSplit && columnIndex == i)
            {
                if (addToTopRow)
                {
                    // check if an item exists in the location where we want to insert child
                    CFTypeRef existingElementAtInsertionPoint = CFArrayGetValueAtIndex(currentColumn, firstRowIndex);
                    if (existingElementAtInsertionPoint == kCFNull)
                    {
                        // no element exists in this location, just add the child
                        CFArraySetValueAtIndex(currentColumn, firstRowIndex, element);
                    }
                    else
                    {
                        // there is already an item here, move it down (because we are
                        // forcing child into the top) and then insert child
                        CFArraySetValueAtIndex(currentColumn, secondRowIndex, existingElementAtInsertionPoint);
                        CFArraySetValueAtIndex(currentColumn, firstRowIndex, element);
                    }
                }
                else
                {
                    // check if an item exists in the location where we want to insert child
                    CFTypeRef existingElementAtInsertionPoint = CFArrayGetValueAtIndex(currentColumn, secondRowIndex);
                    if (existingElementAtInsertionPoint == kCFNull)
                    {
                        // no element exists in this location, just add the child
                        CFArraySetValueAtIndex(currentColumn, secondRowIndex, element);
                    }
                    else
                    {
                        // there is already an item here, move it up (because we are forcing
                        // child into the bottom) and then insert child
                        CFArraySetValueAtIndex(currentColumn, firstRowIndex, existingElementAtInsertionPoint);
                        CFArraySetValueAtIndex(currentColumn, secondRowIndex, element);
                    }
                }
            }
        } // loop through the columns to place elements
    } // if (needToSplitRow)

    *rowCount = CFArrayGetCount(rowRanges);
}

//This method adds a child to an existing row. This method determines if the row
// needs to be split. If a split is necessary, then the method does the split,
// and moves elements in to the appropriate rows.
static void __AXNavigationOrderAddElementToRowAndColumn(CFTypeRef element, CFIndex rowIndex, CFIndex columnIndex, CFMutableArrayRef columns, CFMutableArrayRef rowRanges, CFIndex *rowCount, AccessibilityElementOrderingFrameGetter frameGetter, CFMutableDictionaryRef navigationOrderFrameCache)
{
    __AXNavigationOrderAddElementToRowAndColumnWithForceSplit(element, rowIndex, columnIndex, columns, rowRanges, rowCount, false, false, frameGetter, navigationOrderFrameCache);
}

NSArray *_AccessibilityElementOrdering(NSArray *elements, AccessibilityElementOrderingFrameGetter frameGetter)
{
    if (frameGetter == NULL || elements == nil || ![elements isKindOfClass:NSArray.class])
    {
        return nil;
    }

    NSUInteger childCount = elements.count;
    if (childCount == 0 || childCount == 1)
    {
        return elements;
    }

    CFMutableArrayRef children = (__bridge_retained CFMutableArrayRef)[elements mutableCopy];

    // Create the array of columns
    CFMutableArrayRef columns = CFArrayCreateMutable(kCFAllocatorDefault, 0, &kCFTypeArrayCallBacks);

    // Create the column headers (array of column ranges)
    // (will give the ranges for each of the columns in columns)
    CFMutableArrayRef columnRanges = CFArrayCreateMutable(kCFAllocatorDefault, 0, &kCFTypeArrayCallBacks);
    CFIndex columnCount = 0;

    // Create the row labels (array of row ranges) (will give the ranges for
    // each of the "rows" found in the columns in columns array)
    CFMutableArrayRef rowRanges = CFArrayCreateMutable(kCFAllocatorDefault, 0, &kCFTypeArrayCallBacks);
    CFIndex rowCount = 0;

    // Create the first column array of "rows" and add it to the array of columns
    CFMutableArrayRef indexedColumn = CFArrayCreateMutable(kCFAllocatorDefault, 0, &kCFTypeArrayCallBacks);
    CFArrayAppendValue(columns, indexedColumn);
    AXSafeRelease(indexedColumn);

    // Create the frame cache
    CFMutableDictionaryRef navigationOrderFrameCache = CFDictionaryCreateMutable(kCFAllocatorDefault, childCount, &kCFTypeDictionaryKeyCallBacks, &kCFTypeDictionaryValueCallBacks);

    // Sort the frames based on origin (left to right)
    _AXNavigationOrderCompareContext compareContext;
    compareContext.frameGetter = frameGetter;
    compareContext.navigationOrderFrameCache = navigationOrderFrameCache;

    CFArraySortValues(children, CFRangeMake(0, childCount), (CFComparatorFunction)__AXNavigationOrderCompareUIElementFrames, &compareContext);

    CFTypeRef child = CFArrayGetValueAtIndex(children, 0);
    const CFRange rangeSizeRange = CFRangeMake(0, sizeof(CFRange));

    CGRect childFrame = __AXNavigationOrderFrameFromCustomGetter(child, frameGetter, navigationOrderFrameCache);

    // Add first child to first row in first column
    CFArrayAppendValue((CFMutableArrayRef)CFArrayGetValueAtIndex(columns, 0), child);

    // Create and insert the column header for the first column
    CFRange columnRange = CFRangeMake(childFrame.origin.x, childFrame.size.width);
    CFMutableDataRef columnRangeRef = CFDataCreateMutable(kCFAllocatorDefault, sizeof(CFRange));

    CFDataAppendBytes(columnRangeRef, (const UInt8 *)&columnRange, sizeof(CFRange));
    CFArrayAppendValue(columnRanges, columnRangeRef);
    AXSafeRelease(columnRangeRef);
    columnCount++;

    // Create and insert the row label for the first row
    CFRange rowRange = CFRangeMake(childFrame.origin.y, childFrame.size.height);
    CFMutableDataRef rowRangeRef = CFDataCreateMutable(kCFAllocatorDefault, sizeof(CFRange));

    CFDataAppendBytes(rowRangeRef, (const UInt8 *)&rowRange, sizeof(CFRange));
    CFArrayAppendValue(rowRanges, rowRangeRef);
    AXSafeRelease(rowRangeRef);
    rowCount++;

    // Go through each of the children determining where they should go
    CFIndex rowIndex = 0;
    CFIndex columnIndex = 0;
    CFTypeRef indexedElement = NULL;
    for (CFIndex index = 1; index < childCount; index++)
    {
        child = CFArrayGetValueAtIndex(children, index);

        childFrame = __AXNavigationOrderFrameFromCustomGetter(child, frameGetter, navigationOrderFrameCache);

        // Find the correct row this element should go in (create one if it's not
        // already there). When done the rowRanges should be correct and up to date
        rowIndex = 0;
        while (true)
        {
            // Get the range of the indexed row
            rowRangeRef = (CFMutableDataRef)CFArrayGetValueAtIndex(rowRanges, rowIndex);
            CFDataGetBytes(rowRangeRef, rangeSizeRange, (UInt8 *)&rowRange);

            // if this row is completely inside the top half of the element (but not equal to it), then this element should be "below" this row
            if ((rowRange.location >= childFrame.origin.y) && (_AXRangeMax(rowRange) <= CGRectGetMidY(childFrame)) &&
                !((rowRange.location == childFrame.origin.y) && (rowRange.length == childFrame.size.height)))
            {
                rowIndex++; //check the next row
            }

            // If we found the correct row then check if the row's current range matches
            // (either most of the element should be in the range or most of the range
            // should overlap the element)
            else if ((CGRectGetMidY(childFrame) < _AXRangeMax(rowRange)) || (childFrame.origin.y < _AXRangeMid(rowRange)))
            {
                // If the child overlaps this row's range then we only need to update the range
                if (CGRectGetMaxY(childFrame) > rowRange.location)
                {
                    __AXNavigationOrderConstrainRange(rowIndex, rowRanges, CFRangeMake(childFrame.origin.y, childFrame.size.height));
                    break;
                }
                // Otherwise we need to move the remaining rows in all the columns
                // down so that the row we found can have the correct range
                else
                {
                    __AXNavigationOrderInsertEmptyRowIntoColumns(columns, rowIndex);

                    // Now insert the new row range
                    __AXNavigationOrderInsertRange(CFRangeMake(childFrame.origin.y, childFrame.size.height), rowRanges, rowIndex);
                    rowCount++;
                    break;
                }
            }
            // Otherwise we should check the next row
            else
            {
                rowIndex++;
            }

            // If there are no more rows then just append a new one to each all of the column arrays
            if (rowIndex >= rowCount)
            {
                __AXNavigationOrderInsertEmptyRowIntoColumns(columns, rowIndex);

                // Now append the new row range for the new row
                __AXNavigationOrderInsertRange(CFRangeMake(childFrame.origin.y, childFrame.size.height), rowRanges, rowCount);
                rowCount++;
                break;
            }
        }

        // The following code can juggle children received and/or removed from arrays
        // and get a bit confusing.  We need to keep track of the case when we need
        // to explicitly retain a child received from an array.  This case occurs
        // when we find that the current child needs to replace a child we find already
        // in a column, and the child in the column needs to move to the next column
        // to the right.  We'll need to retain it so that when we replace it with the
        // current child it doesn't get a retain count of zero.
        bool childNeedsRelease = false;

        // Find the correct column this element should go in
        columnIndex = 0;
        while (columnIndex < columnCount)
        {
            // Get the range of the indexed column
            columnRangeRef = (CFMutableDataRef)CFArrayGetValueAtIndex(columnRanges, columnIndex);
            CFDataGetBytes(columnRangeRef, rangeSizeRange, (UInt8 *)&columnRange);

            // If the child is to the left of this column then we need to insert a new column
            // (this would require that we then check each and every row in the columns to the
            //  right and see if any of those elements should be shifted over to the left (ad
            //  infinitum).  This would be too expensive so instead we make sure we don't have
            //  to do this at all by initially sorting the children from left to right.  That
            //  way we should never need to insert a new column to the left.)  LABEL:order
            if (CGRectGetMaxX(childFrame) < columnRange.location)
            {
                indexedColumn = __AXNavigationOrderCopyInsertedEmptyColumn(columns, columnCount);

                // Add the element to the new column
                CFArraySetValueAtIndex(indexedColumn, rowIndex, child);

                // Flag that it's been added
                if (childNeedsRelease)
                {
                    AXSafeRelease(child);
                }
                child = NULL;

                // Now add the header for the new column
                __AXNavigationOrderInsertRange(CFRangeMake(childFrame.origin.x, childFrame.size.width), columnRanges, columnIndex);
                columnCount++;
                break;
            }
            // If the majority of this child overlaps this column's range (or visa versa)
            // then compare it with the element that is in that position (if one)
            else if ((CGRectGetMidX(childFrame) < _AXRangeMax(columnRange)) || (childFrame.origin.x < _AXRangeMid(columnRange)))
            {
                indexedColumn = (CFMutableArrayRef)CFArrayGetValueAtIndex(columns, columnIndex);
                indexedElement = CFArrayGetValueAtIndex(indexedColumn, rowIndex);

                // If an element is already there then determine whether this child should replace it,
                // go into the next column, or get its own new row
                if (CFGetTypeID(indexedElement) != CFNullGetTypeID())
                {
                    CGRect indexedFrame = __AXNavigationOrderFrameFromCustomGetter(indexedElement, frameGetter, navigationOrderFrameCache);

                    bool childAboveIndexed = childFrame.origin.y < indexedFrame.origin.y;
                    CGFloat vOverlap = childAboveIndexed ? (CGRectGetMaxY(childFrame) - indexedFrame.origin.y) : (CGRectGetMaxY(indexedFrame) - childFrame.origin.y);

                    if ((vOverlap < (indexedFrame.size.height / 2)) && (vOverlap < (childFrame.size.height / 2)))
                    {

                        // If child is above then we need to create a new row and push the existing element down into it
                        if (childAboveIndexed)
                        {
                            __AXNavigationOrderAddElementToRowAndColumnWithForceSplit(child, rowIndex, columnIndex, columns, rowRanges, &rowCount, true, true, frameGetter, navigationOrderFrameCache);
                            // Flag that it's been added
                            if (childNeedsRelease)
                            {
                                AXSafeRelease(child);
                            }
                            child = NULL;
                            break;
                        }

                        // If child is below then we need to create a new row and put child into it
                        else
                        {
                            __AXNavigationOrderAddElementToRowAndColumnWithForceSplit(child, rowIndex, columnIndex, columns, rowRanges, &rowCount, true, false, frameGetter, navigationOrderFrameCache);

                            // Flag that it's been added
                            if (childNeedsRelease)
                            {
                                AXSafeRelease(child);
                            }
                            child = NULL;
                            break;
                        }
                    }
                    else
                    {
                        // If child is to the right then we need to increment the column index and check the next one
                        if ((indexedFrame.origin.x < childFrame.origin.x) ||
                            ((fabs(indexedFrame.origin.x - childFrame.origin.x) <= 0.0f) && (childFrame.size.width > indexedFrame.size.width)) ||
                            _AXCGFloatAlmostEqual(indexedFrame.origin.x, childFrame.origin.x))
                        {
                            columnIndex++;
                        }

                        //indexedElement is inside of child && indexedElement's center is to the left or above child's
                        else if (CGRectContainsRect(childFrame, indexedFrame) &&
                                 (CGRectGetMidX(indexedFrame) < CGRectGetMidX(childFrame) || CGRectGetMidY(indexedFrame) < CGRectGetMidY(childFrame)))
                        {
                            columnIndex++; //child is to the right of indexedElement
                        }

                        // Otherwise we need to move the element in there now to a column to the right
                        // (we'll just go through this column loop again for the element we're removing)
                        else
                        {
                            AXSafeRetain(indexedElement);

                            // We've just retained the element in the column (we'll
                            // shove it in a new column below) so we can replace it
                            // with the correct one now
                            CFArraySetValueAtIndex(indexedColumn, rowIndex, child);
                            if (childNeedsRelease)
                            {
                                AXSafeRelease(child);
                            }
                            else
                            {
                                // set this AFTER checking its value above
                                childNeedsRelease = true;
                            }

                            // We're updating child so we also need to update childFrame
                            child = indexedElement;
                            childFrame = __AXNavigationOrderFrameFromCustomGetter(child, frameGetter, navigationOrderFrameCache);

                            // Update the column's range
                            // (if either of the old element's range ends (now set to child)
                            //  equal the current range's ends then we should rebuild the entire
                            //  column range, otherwise just constrain it normally)
                            if ((childFrame.origin.x == columnRange.location) || (CGRectGetMaxX(childFrame) == _AXRangeMax(columnRange)))
                            {
                                __AXNavigationOrderRebuildColumnWithIndex(columnIndex, columns, columnRanges, frameGetter, navigationOrderFrameCache);
                            }
                            else
                            {
                                __AXNavigationOrderConstrainRange(columnIndex, columnRanges, CFRangeMake(childFrame.origin.x, childFrame.size.width));
                            }
                            columnIndex++;
                        }
                    }
                }
                // If there isn't already an element there, this element may still need to be put into
                // the following column if it doesn't overlap enough with the rest of the elements in
                // the column, or it may need to be in a new row if the child doesn't overlap enough
                // with the rest of the elements in the row
                else
                {
                    // This element needs to overlap with a majority of elements already in the column
                    // otherwise it should probably go in the next column.  This will stop a few wide
                    // elements from creating an overly wide column
                    CFIndex foundCount = 0;
                    CFIndex overlappingCount = 0;
                    for (CFIndex i = 0; i < rowCount; i++)
                    {
                        CFTypeRef element = CFArrayGetValueAtIndex(indexedColumn, i);
                        if (CFGetTypeID(element) != CFNullGetTypeID())
                        {
                            foundCount++;
                            CGRect elementFrame = __AXNavigationOrderFrameFromCustomGetter(element, frameGetter, navigationOrderFrameCache);
                            if ((CGRectGetMidX(childFrame) < CGRectGetMaxX(elementFrame)) || (childFrame.origin.x < CGRectGetMidX(elementFrame)))
                            {
                                overlappingCount++;
                            }
                        }
                    }

                    // Go around again to the next column
                    if (overlappingCount <= foundCount / 2)
                    {
                        columnIndex++;
                        continue;
                    }
                    __AXNavigationOrderAddElementToRowAndColumn(child, rowIndex, columnIndex, columns, rowRanges, &rowCount, frameGetter, navigationOrderFrameCache);
                    __AXNavigationOrderConstrainRange(columnIndex, columnRanges, CFRangeMake(childFrame.origin.x, childFrame.size.width));

                    // Flag that it's been added
                    if (childNeedsRelease)
                    {
                        AXSafeRelease(child);
                    }
                    child = NULL;
                    break;
                }
            }
            else
            {
                columnIndex++;
            }
        }

        // If the child hasn't been added then we'll need to
        // append another column and stuff the child in it
        if (child)
        {
            indexedColumn = __AXNavigationOrderCopyInsertedEmptyColumn(columns, columnIndex);

            // Now add the header for the new column
            CGRect headerchildFrame = __AXNavigationOrderFrameFromCustomGetter(child, frameGetter, navigationOrderFrameCache);

            CFRange childRange = CFRangeMake(headerchildFrame.origin.x, headerchildFrame.size.width);
            __AXNavigationOrderInsertRange(childRange, columnRanges, columnCount);
            columnCount++;

            // We need to constrain the previous column if necessary
            __AXNavigationOrderConstrainRange(columnIndex, columnRanges, childRange);
            // Now we'll check to see if any of the elements in the
            // previous column would better fit in this new column
            if (columnCount > 1)
            {
                CFIndex columnToLeftIndex = columnCount - 2;
                CFMutableArrayRef columnToLeft = (CFMutableArrayRef)CFArrayGetValueAtIndex(columns, columnToLeftIndex);

                for (CFIndex i = 0; i < rowCount; i++)
                {
                    // Since we just created this column there won't be any element already in
                    // any of the rows (except for the current rowIndex), so we don't bother
                    // checking first before moving any elements from the previous column
                    if (i != rowIndex)
                    {
                        CFTypeRef element = CFArrayGetValueAtIndex(columnToLeft, i);
                        if (CFGetTypeID(element) != CFNullGetTypeID())
                        {
                            CGRect elementFrame = __AXNavigationOrderFrameFromCustomGetter(element, frameGetter, navigationOrderFrameCache);
                            CFRange columnToLeftRange;
                            CFDataGetBytes((CFMutableDataRef)CFArrayGetValueAtIndex(columnRanges, columnToLeftIndex), rangeSizeRange, (UInt8 *)&columnToLeftRange);
                            CGFloat elementSizeInLeftColumn = _AXRangeMax(columnToLeftRange) - elementFrame.origin.x;

                            // if the element is completely inside of the new column, leave it in its current column
                            if (CGRectContainsRect(childFrame, elementFrame))
                            {
                                // don't move it to the new column
                            }

                            // If the element doesn't take up a majority of the column it's in and
                            // more of it lands in the new column, then move it to the new column
                            // (Left column end location can change at each iteration due to call
                            //  to _constrain so call it inside the for loop)
                            else if ((elementSizeInLeftColumn < (columnToLeftRange.length / 2.0)) &&
                                     ((CGRectGetMaxX(elementFrame) - _AXRangeMax(columnToLeftRange)) > elementSizeInLeftColumn))
                            {
                                CFArraySetValueAtIndex(indexedColumn, i, element);
                                CFArraySetValueAtIndex(columnToLeft, i, kCFNull);
                                __AXNavigationOrderConstrainRange(columnIndex, columnRanges, CFRangeMake(elementFrame.origin.x, elementFrame.size.width));
                                // Do this each time because the column's width needs to be adjusted
                                // for each one because we want the decision for subsequent rows to
                                // be based on the latest column width value (ughs)
                                __AXNavigationOrderRebuildColumnWithIndex(columnToLeftIndex, columns, columnRanges, frameGetter, navigationOrderFrameCache);
                            }
                        }
                    }
                }

                // Now, actually add the new element and make sure that everything actually belongs in this row
                __AXNavigationOrderAddElementToRowAndColumn(child, rowIndex, columnCount - 1, columns, rowRanges, &rowCount, frameGetter, navigationOrderFrameCache);
            }

            // Only in some code paths do we need to retain the
            // child so we don't need to always release it
            if (childNeedsRelease)
            {
                AXSafeRelease(child);
            }
        }
    }

    // Transfer the navigation order to the xList and yLists
    NSArray *orderedChildren = __AXNavigationOrderCopyIntoLists(columns, columnRanges, rowRanges);

    AXSafeRelease(children);
    AXSafeRelease(navigationOrderFrameCache);
    AXSafeRelease(rowRanges);
    AXSafeRelease(columnRanges);
    AXSafeRelease(columns);

    return orderedChildren;
}

NSArray *_AccessibilityElementModalFiltering(NSArray *elements, AccessibilityElementModalGetter modalGetter)
{
    for (id element in elements)
    {
        if (modalGetter(element))
        {
            return @[ element ];
        }
    }
    return elements;
}

CGRect _AccessibilityElementOrderingFrameGetter(id object)
{
    if ( object == nil || ![object respondsToSelector:@selector(accessibilityFrame)] )
    {
        return CGRectNull;
    }
    return [object accessibilityFrame];
}

BOOL _AccessibilityElementModalFilteringGetter(id object)
{
    if ( object == nil || ![object respondsToSelector:@selector(accessibilityViewIsModal)] )
    {
        return NO;
    }
    return [object accessibilityViewIsModal];
}
