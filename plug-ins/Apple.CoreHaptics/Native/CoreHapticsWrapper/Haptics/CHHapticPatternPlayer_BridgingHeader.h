//
//  CHHapticPatternPlayer_BridgingHeader.h
//  CoreHapticsWrapper
//
//  Copyright © 2021 Apple, Inc. All rights reserved.
//

#ifndef CHHapticPatternPlayer_BridgingHeader_h
#define CHHapticPatternPlayer_BridgingHeader_h

#pragma pack(push)
#pragma pack(8)
typedef struct {
    int parameterID;
    float relativeTime;
    float value;
} CHWDynamicParameter;

typedef struct {
    CHWDynamicParameter * parameters;
    int parametersLength;
    float atTime;
} CHWSendParametersRequest;

typedef struct {
    float relativeTime;
    float value;
} CHWHapticParameterCurveControlPoint;

typedef struct {
    int parameterId;
    double relativeTime;
    CHWHapticParameterCurveControlPoint * points;
    int pointsLength;
} CHWHapticParameterCurve;
#pragma pack(pop)

#endif /* CHHapticPatternPlayer_BridgingHeader_h */
