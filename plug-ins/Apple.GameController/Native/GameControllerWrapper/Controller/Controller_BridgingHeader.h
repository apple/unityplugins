//
//  Controller_BridgingHeader.h
//  GameControllerWrapper
//
//  Copyright © 2020 Apple, Inc. All rights reserved.
//

#ifndef Controller_BridgingHeader_h
#define Controller_BridgingHeader_h

typedef enum {
    ButtonHome = 0,
    ButtonMenu = 1,
    ButtonOptions = 2,
    ButtonA = 3,
    ButtonB = 4,
    ButtonY = 5,
    ButtonX = 6,
    // Shoulder...
    ShoulderRightFront = 7,
    ShoulderRightBack = 8,
    ShoulderLeftFront = 9,
    ShoulderLeftBack = 10,
    // Dpad...
    DpadHorizontal = 11,
    DpadVertical = 12,
    // Thumbsticks...
    ThumbstickLeftHorizontal = 13,
    ThumbstickLeftVertical = 14,
    ThumbstickLeftButton = 15,
    ThumbstickRightHorizontal = 16,
    ThumbstickRightVertical = 17,
    ThumbstickRightButton = 18,
    // Dualshock and DualSense
    TouchpadButton = 19,
    TouchpadPrimaryHorizontal = 20,
    TouchpadPrimaryVertical = 21,
    TouchpadSecondaryHorizontal= 22,
    TouchpadSecondaryVertical = 23
} GCWControllerInputName;

typedef struct {
    float buttonHome;
    float buttonMenu;
    float buttonOptions;
    float buttonA;
    float buttonB;
    float buttonY;
    float buttonX;
    // Shoulder...
    float shoulderRightFront;
    float shoulderRightBack;
    float shoulderLeftFront;
    float shoulderLeftBack;
    // Dpad...
    float dpadHorizontal;
    float dpadVertical;
    // Thumbsticks...
    float thumbstickLeftHorizontal;
    float thumbstickLeftVertical;
    float thumbstickLeftButton;
    float thumbstickRightHorizontal;
    float thumbstickRightVertical;
    float thumbstickRightButton;
    // Dualshock and DualSense
    float touchpadButton;
    float touchpadPrimaryHorizontal;
    float touchpadPrimaryVertical;
    float touchpadSecondaryHorizontal;
    float touchpadSecondaryVertical;
    
    //Battery
    float batteryLevel;
    int batteryState;
} GCWControllerInputState;

typedef struct {
    char * uniqueId;
    char * productCategory;
    char * vendorName;
    bool isAttachedToDevice;
    bool hasHaptics;
    bool hasLight;
    bool hasBattery;
} GCWController;

typedef struct {
    GCWController * controllers;
    int controllerCount;
} GCWGetConnectedControllersResponse;

typedef struct {
    int width;
    int height;
    unsigned char * data;
    int dataLength;
} GCWGetSymbolForInputNameResponse;

typedef void (*GCWControllerConnectionStateChangedCallback)(GCWController controller);

#endif /* Controller_BridgingHeader_h */
