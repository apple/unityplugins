//
//  SpatialController_BridgingHeader.h
//  SpatialController
//
//  Copyright © 2025 Apple, Inc. All rights reserved.
//

#ifndef SpatialController_BridgingHeader_h
#define SpatialController_BridgingHeader_h

typedef enum {
    // Buttons...
    SCControllerInputNameButtonHome = 0,
    SCControllerInputNameButtonMenu = 1,
    SCControllerInputNameButtonOptions = 2,
    SCControllerInputNameButtonShare = 3,
    SCControllerInputNameButtonA = 4,
    SCControllerInputNameButtonB = 5,
    SCControllerInputNameButtonX = 6,
    SCControllerInputNameButtonY = 7,
    SCControllerInputNameButtonGrip = 8,
    SCControllerInputNameButtonLeftShoulder = 10,
    SCControllerInputNameButtonRightShoulder = 11,
    SCControllerInputNameButtonLeftBumper = 12,
    SCControllerInputNameButtonRightBumper = 13,
    SCControllerInputNameButtonTrigger = 16,
    SCControllerInputNameButtonLeftTrigger = 18,
    SCControllerInputNameButtonRightTrigger = 19,
    SCControllerInputNameButtonThumbstick = 20,
    SCControllerInputNameButtonLeftThumbstick = 22,
    SCControllerInputNameButtonRightThumbstick = 23,
    SCControllerInputNameButtonTouchpad = 24,
    // ButtonBackLeft(position) = ?,
    // ButtonBackRight(position) = ?,
    // ButtonArcade(row,column) = ?,
    SCControllerInputNameButtonStylusTip = 96,
    SCControllerInputNameButtonStylusPrimary = 97,
    SCControllerInputNameButtonStylusSecondary = 98,

    // DPads...
    SCControllerInputNameDPadDirectionPad = 192,
    SCControllerInputNameDPadThumbstick = 193,
    SCControllerInputNameDPadLeftThumbstick = 194,
    SCControllerInputNameDPadRightThumbstick = 195,
    SCControllerInputNameDPadTouchpadPrimary = 196,
    SCControllerInputNameDPadTouchpadSecondary = 197,
} SCControllerInputName;

typedef enum {
    SCControllerSettingEnableMotionSensors = 1, // boolean (value != 0)
} SCControllerSetting;

typedef enum {
    SCAccessoryChiralityUnspecified = 0,
    SCAccessoryChiralityLeft = 1,
    SCAccessoryChiralityRight = 2,
} SCAccessoryChirality;

typedef enum {
    SCAccessoryAnchorTrackingStateUntracked = 0,
    SCAccessoryAnchorTrackingStateOrientationTracked = 1,
    SCAccessoryAnchorTrackingStatePositionOrientationTracked = 2,
    SCAccessoryAnchorTrackingStatePositionOrientationTrackedLowAccuracy = 3,
} SCAccessoryAnchorTrackingState;

typedef enum {
    SCUIImageSymbolScaleDefault = -1,
    SCUIImageSymbolScaleUnspecified = 0,
    SCUIImageSymbolScaleSmall = 1,
    SCUIImageSymbolScaleMedium = 2,
    SCUIImageSymbolScaleLarge = 3,
} SCUIImageSymbolScale;

typedef enum {
    SCUIImageRenderingModeAutomatic = 0,
    SCUIImageRenderingModeAlwaysOriginal = 1,
    SCUIImageRenderingModeAlwaysTemplate = 2,
} SCUIImageRenderingMode;

typedef enum {
    SCGCDeviceBatteryStateUnknown = -1,
    SCGCDeviceBatteryStateDischarging = 0,
    SCGCDeviceBatteryStateCharging = 1,
    SCGCDeviceBatteryStateFull = 2,
} SCGCDeviceBatteryState;

typedef enum {
    SCAccessoryTrackingAuthorizationStateNotSupported = -1,
    SCAccessoryTrackingAuthorizationStateNotRequested = 0,
    SCAccessoryTrackingAuthorizationStatePending = 1,
    SCAccessoryTrackingAuthorizationStateNotAuthorized = 2,
    SCAccessoryTrackingAuthorizationStateAuthorized = 3,
} SCAccessoryTrackingAuthorizationState;

typedef enum {
    SCAccessoryTrackingStateARTrackingError = -1,
    SCAccessoryTrackingStateStopped = 0,
    SCAccessoryTrackingStateRunning = 1,
} SCAccessoryTrackingState;

typedef enum {
    SCHapticsLocalityDefault = 0,
    SCHapticsLocalityAll = 1,
    SCHapticsLocalityHandles = 2,
    SCHapticsLocalityLeftHandle = 3,
    SCHapticsLocalityRightHandle = 4,
    SCHapticsLocalityTriggers = 5,
    SCHapticsLocalityLeftTrigger = 6,
    SCHapticsLocalityRightTrigger = 7,
} SCHapticsLocality;

typedef enum {
    SCHapticEngineFinishedActionLeaveEngineRunning = 0,
    SCHapticEngineFinishedActionStopEngine = 1,
} SCHapticEngineFinishedAction;

typedef unsigned char SCBool;
static const SCBool SCFalse = 0;
static const SCBool SCTrue = 1;

// NOTE: Unity does not have a simd_float3
// equivalent that pads to 16 bytes.
typedef struct {
    // align 4
    float x;
    float y;
    float z;
} SCVectorFloat3;

typedef struct {
    // align 4
    float x;
    float y;
    float z;
    float w;
} SCVectorFloat4;

// align 16
typedef simd_float4x4 SCTransformFloat4x4;

typedef struct {
    // align 4
    unsigned int supportedLocalities; // bit set of (1<<SCHapticsLocality)
} SCDeviceHaptics;

typedef struct {
    // align 1
    SCBool hasAttitude;
    SCBool hasRotationRate;
    SCBool hasGravityAndUserAcceleration;
    SCBool sensorsRequireManualActivation;
} SCControllerMotionCapabilities;

typedef struct {
    // align 8
    char * uniqueId;
    char * productCategory;
    char * vendorName;
    SCBool isAttachedToDevice;
    SCBool hasBattery;
    SCBool hasMotion;
    SCBool hasLight;
    SCBool hasHaptics;
    SCControllerMotionCapabilities motion;
    // pad 3 bytes
    SCDeviceHaptics haptics;
} SCController;

typedef struct {
    // align 8
    char * name;
} SCAccessoryLocation;

typedef struct {
    // align 8
    SCAccessoryLocation * ptr;
    int count;
    // pad 4 bytes
} SCAccessoryLocations;

typedef struct {
    // align 8
    char * id;
    char * name;
    char * usdzFile;
    char * description;
    SCAccessoryChirality inherentChirality;
    // pad 4 bytes
    SCAccessoryLocations locations;
    SCController source;
} SCAccessory;

typedef struct {
    // align 8
    double time;
} SCTimeValue;

typedef struct {
    // align 4
    SCVectorFloat3 position;
    SCVectorFloat4 rotation; // quaternion
} SCPose;

typedef struct {
    // align 8
    SCPose * ptr;
    int count;
    // pad 4 bytes
} SCPoses;

typedef struct {
    // align 16
    char * id;
    char * description;
    SCTransformFloat4x4 originFromAnchorTransform;
    SCVectorFloat3 velocity;
    SCVectorFloat3 angularVelocity;
    SCTimeValue timestamp;
    SCAccessoryAnchorTrackingState trackingState;
    SCAccessoryChirality heldChirality;
    SCBool isHeld;
    SCBool isTracked;
    // pad 6 bytes
    SCPoses locationPoses;
} SCAccessoryAnchor;

typedef struct {
    // align 8
    SCAccessoryAnchor * ptr;
    int count;
    // pad 4 bytes
} SCAccessoryAnchors;

typedef struct {
    // align 8
    SCTimeValue lastPressedStateTimestamp;
    SCTimeValue lastValueTimestamp;
    float value;
    SCBool isAnalog;
    SCBool isPressed;
    SCBool isTouched;
    // pad 1 byte
} SCButtonState;

typedef struct {
    // align 4
    float xAxis;
    float yAxis;
} SCDPadState;

typedef struct {
    // align 8
    SCControllerInputName name;
    // pad 4 bytes
    SCButtonState state;
} SCInputButtonState;

typedef struct {
    // align 4
    SCControllerInputName name;
    SCDPadState state;
} SCInputDPadState;

typedef struct {
    // align 8
    SCInputButtonState * ptr;
    int count;
    // pad 4 bytes
} SCInputButtonStates;

typedef struct {
    // align 8
    SCInputDPadState * ptr;
    int count;
    // pad 4 bytes
} SCInputDPadStates;

typedef struct {
    // align 8
    SCInputButtonStates buttons;
    SCInputDPadStates dpads;
} SCControllerInputState;

typedef struct {
    // align 4
    float level;
    SCGCDeviceBatteryState state;
} SCControllerBatteryState;

typedef struct {
    // align 4
    SCVectorFloat4 attitude; // quaternion
    SCVectorFloat3 rotationRate;
    SCVectorFloat3 acceleration;
    SCVectorFloat3 gravity;
    SCVectorFloat3 userAcceleration;
    SCBool sensorsActive;
    // pad 3 bytes
} SCControllerMotionState;

typedef struct {
    // align 4
    float red;
    float green;
    float blue;
} SCColor;

typedef struct {
    // align 4
    SCColor color;
} SCControllerLightState;

typedef struct {
    // align 8
    SCControllerInputState input;
    SCControllerBatteryState battery;
    SCControllerMotionState motion;
    SCControllerLightState light;
    SCAccessoryAnchors anchors;
    SCAccessory * accessory;
} SCControllerState;

typedef struct {
    // align 8
    SCController * ptr;
    int count;
    // pad 4 bytes
} SCControllers;

typedef struct {
    SCAccessory * ptr;
    int count;
    // pad 4 bytes
} SCAccessories;

typedef struct {
    // align 8
    int width;
    int height;
    unsigned char * data;
    int dataLength;
    // pad 4 bytes
} SCSymbol;

typedef struct {
    // align 8
    char* localizedName;
    char* symbolName;
} SCControllerInputInfo;

typedef struct {
    // align 8
    int code;
    // pad 4 bytes
    char * localizedDescription;
} SCError;

// All pointer members of data passed to callbacks are only valid within the scope of the callback
typedef void (*SCControllerConnectionCallback)(SCController controller);
typedef void (*SCAccessoryConnectionCallback)(SCAccessory accessory);
typedef void (*SCControllerCallback)(SCController controller, void* context);
typedef void (*SCControllersCallback)(SCControllers controllers, void* context);
typedef void (*SCAccessoryCallback)(SCAccessory accessory, void* context);
typedef void (*SCAccessoriesCallback)(SCAccessories accessories, void* context);
typedef void (*SCAccessoryAnchorCallback)(SCAccessoryAnchor accessoryAnchor, SCAccessory accessory, void* context);
typedef void (*SCControllerStateCallback)(SCControllerState state, void* context, char* uniqueId);
typedef void (*SCGetControllerInputInfoForInputNameCallback)(SCControllerInputInfo info, void* context, char* uniqueId, SCControllerInputName inputName);
typedef void (*SCGetSymbolForInputNameCallback)(SCSymbol symbol, void* context, char* uniqueId, SCControllerInputName inputName, SCUIImageSymbolScale symbolSize, SCUIImageRenderingMode renderingMode);
typedef SCHapticEngineFinishedAction (*SCHapticEngineFinishedCallback)(SCError error, void* context, char* uniqueId, SCHapticsLocality locality);
typedef void (*SCStopHapticEngineCompletedCallback)(SCError error, void* context, char* uniqueId, SCHapticsLocality locality);

typedef void (*SCSuccessCallback)();
typedef void (*SCErrorCallback)(SCError error);

#endif /* SpatialController_BridgingHeader_h */
