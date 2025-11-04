using UnityEngine;
using Unity.Mathematics;
using System;
using AOT;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

/// <summary>
/// Internal namespace for Apple visionOS Spatial Controller implementation.
/// Contains native interop structures, delegates, and plugin wrapper functionality.
/// </summary>
namespace Apple.visionOS.SpatialController.Internal
{
    using SCControllerInputName = ControllerInputName;
    using SCControllerSetting = ControllerSetting;
    using SCAccessoryChirality = AccessoryChirality;
    using SCAccessoryAnchorTrackingState = AccessoryAnchor.TrackingState;
    using SCUIImageSymbolScale = UIImage.SymbolScale;
    using SCUIImageRenderingMode = UIImage.RenderingMode;
    using SCGCDeviceBatteryState = GCDeviceBattery.State;
    using SCAccessoryTrackingAuthorizationState = AccessoryTrackingAuthorizationState;
    using SCAccessoryTrackingState = AccessoryTrackingState;
    using SCHapticsLocality = HapticsLocality;
    using SCHapticEngineFinishedAction = HapticEngineFinishedAction;

#nullable enable
    /// <summary>
    /// Internal callback delegate for controller connection and disconnection events.
    /// </summary>
    /// <param name="controller">The controller that was connected or disconnected.</param>
    delegate void ControllerConnectionCallbackWrapper(Controller controller);

    /// <summary>
    /// Internal callback delegate for accessory connection and disconnection events.
    /// </summary>
    /// <param name="accessory">The accessory that was connected or disconnected.</param>
    delegate void AccessoryConnectionCallbackWrapper(Accessory accessory);

    /// <summary>
    /// Main plugin class providing native interop functionality for spatial controllers.
    /// Handles conversion between native structures and managed types.
    /// </summary>
    static class Plugin
    {
        /// <summary>
        /// Callback invoked when a controller is connected.
        /// </summary>
        static event ControllerConnectionCallbackWrapper? onControllerConnected;

        /// <summary>
        /// Callback invoked when a controller is disconnected.
        /// </summary>
        static event ControllerConnectionCallbackWrapper? onControllerDisonnected;

        /// <summary>
        /// Callback invoked when a controller is connected.
        /// </summary>
        static event AccessoryConnectionCallbackWrapper? onAccessoryConnected;

        /// <summary>
        /// Callback invoked when a controller is disconnected.
        /// </summary>
        static event AccessoryConnectionCallbackWrapper? onAccessoryDisconnected;

        struct PlayHapticsCall
        {
            public readonly string uniqueId;
            public readonly SCHapticsLocality locality;
            public readonly PlayHapticsFinishedCallback onFinished;

            internal PlayHapticsCall(string uniqueId, SCHapticsLocality locality, PlayHapticsFinishedCallback onFinished)
            {
                this.uniqueId = uniqueId;
                this.locality = locality;
                this.onFinished = onFinished;
            }
        }
        static int playHapticsCallNextContext = 0;
        static Dictionary<int, PlayHapticsCall> playHapticsCalls = new();

        struct StopHapticsCall
        {
            public readonly string uniqueId;
            public readonly SCHapticsLocality locality;
            public readonly CompletionCallback onCompleted;

            internal StopHapticsCall(string uniqueId, SCHapticsLocality locality, CompletionCallback onCompleted)
            {
                this.uniqueId = uniqueId;
                this.locality = locality;
                this.onCompleted = onCompleted;
            }
        }
        static int stopHapticsCallNextContext = 0;
        static Dictionary<int, StopHapticsCall> stopHapticsCalls = new();

        /// <summary>
        /// Represents a boolean value for native interop.
        /// </summary>
        struct SCBool
        {
            private readonly byte _value;
            public SCBool(byte value)
            {
                _value = value;
            }
            public bool Value => _value != 0;
            public static implicit operator bool(SCBool sc) => sc._value != 0;
        }

        /// <summary>
        /// Represents a time value for spatial controller operations.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCTimeValue
        {
            /// <summary>
            /// The time value as a double precision floating point number.
            /// </summary>
            public double time;
        }

        /// <summary>
        /// Collection of poses with pointer and count information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct SCPose
        {
            /// <summary>
            /// The position of this pose.
            /// </summary>
            public float3 position;

            /// <summary>
            /// The rotation of this pose stored in quaternion format.
            /// </summary>
            public float4 rotation;
        }

        /// <summary>
        /// Collection of poses with pointer and count information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCPoses
        {
            /// <summary>
            /// Pointer to the array of poses.
            /// </summary>
            public IntPtr ptr;

            /// <summary>
            /// Number of poses in the collection.
            /// </summary>
            public int count;
            // pad 4 bytes
        }

        /// <summary>
        /// Represents an accessory anchor with position, velocity, and tracking information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 16)]
        struct SCAccessoryAnchor
        {
            /// <summary>
            /// Unique identifier for the accessory anchor.
            /// </summary>
            public string id;

            /// <summary>
            /// Human-readable description of the accessory anchor.
            /// </summary>
            public string description;

            /// <summary>
            /// Transform matrix from origin to anchor coordinate system.
            /// </summary>
            public float4x4 originFromAnchorTransform;

            /// <summary>
            /// Linear velocity of the anchor in 3D space.
            /// </summary>
            public float3 velocity;

            /// <summary>
            /// Angular velocity of the anchor in 3D space.
            /// </summary>
            public float3 angularVelocity;

            /// <summary>
            /// Timestamp when this anchor data was captured.
            /// </summary>
            public SCTimeValue timestamp;

            /// <summary>
            /// Current tracking state of the accessory.
            /// </summary>
            public SCAccessoryAnchorTrackingState trackingState;

            /// <summary>
            /// Chirality (handedness) when the accessory is held.
            /// </summary>
            public SCAccessoryChirality heldChirality;

            /// <summary>
            /// Indicates whether the accessory is currently being held.
            /// </summary>
            public SCBool isHeld;

            /// <summary>
            /// Indicates whether the accessory is currently being tracked.
            /// </summary>
            public SCBool isTracked;
            // pad 6 bytes

            /// <summary>
            /// Poses of all locations and correction values of this anchor.
            /// This is concatenated 2D array by ARKitCoordinateSpace.Correction (2)
            /// then accessory.locations[] + origin (typically 4).
            /// </summary>
            public SCPoses locationPoses;
        }

        /// <summary>
        /// Collection of accessory anchors with pointer and count information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCAccessoryAnchors
        {
            /// <summary>
            /// Pointer to the array of accessory anchors.
            /// </summary>
            public IntPtr ptr;

            /// <summary>
            /// Number of accessory anchors in the collection.
            /// </summary>
            public int count;
            // pad 4 bytes
        }

        /// <summary>
        /// Represents the state of a button input.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 24)]
        struct SCButtonState
        {
            /// <summary>
            /// The time at which this button last changed isPressed state.
            /// </summary>
            public SCTimeValue lastPressedStateTimestamp;

            /// <summary>
            /// The time at which this button last changed value.
            /// </summary>
            public SCTimeValue lastValueTimestamp;

            /// <summary>
            /// The current value of the button in the range 0.0 to 1.0.
            /// </summary>
            public float value;

            /// <summary>
            /// True if this button produces analog output values between 0.0 and 1.0.
            /// False if it is digital with only value 1.0 if pressed or 0.0 if not.
            /// </summary>
            public SCBool isAnalog;

            /// <summary>
            /// True if this button is currently pressed (value > 0.0).
            /// </summary>
            public SCBool isPressed;

            /// <summary>
            /// True if this button is currently touched, if it supports touch.
            /// </summary>
            public SCBool isTouched;
            // pad 1 byte
        }

        /// <summary>
        /// Represents the state of a directional pad input.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SCDPadState
        {
            /// <summary>
            /// The X-axis value of the directional pad.
            /// </summary>
            public float xAxis;

            /// <summary>
            /// The Y-axis value of the directional pad.
            /// </summary>
            public float yAxis;
        }

        /// <summary>
        /// Associates a button input name with its current state.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCInputButtonState
        {
            /// <summary>
            /// The name/identifier of the button input.
            /// </summary>
            public SCControllerInputName name;
            // pad 4 bytes

            /// <summary>
            /// The current state of the button.
            /// </summary>
            public SCButtonState state;
        }

        /// <summary>
        /// Associates a directional pad input name with its current state.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SCInputDPadState
        {
            /// <summary>
            /// The name/identifier of the directional pad input.
            /// </summary>
            public SCControllerInputName name;

            /// <summary>
            /// The current state of the directional pad.
            /// </summary>
            public SCDPadState state;
        }

        /// <summary>
        /// Collection of button input states with pointer and count information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCInputButtonStates
        {
            /// <summary>
            /// Pointer to the array of button states.
            /// </summary>
            public IntPtr ptr;

            /// <summary>
            /// Number of button states in the collection.
            /// </summary>
            public int count;
            // pad 4 bytes
        }

        /// <summary>
        /// Collection of directional pad input states with pointer and count information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCInputDPadStates
        {
            /// <summary>
            /// Pointer to the array of directional pad states.
            /// </summary>
            public IntPtr ptr;

            /// <summary>
            /// Number of directional pad states in the collection.
            /// </summary>
            public int count;
            // pad 4 bytes
        }

        /// <summary>
        /// Aggregates all input states for a controller.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCControllerInputState
        {
            /// <summary>
            /// Collection of all button input states.
            /// </summary>
            public SCInputButtonStates buttons;

            /// <summary>
            /// Collection of all directional pad input states.
            /// </summary>
            public SCInputDPadStates dpads;
        }

        /// <summary>
        /// Represents the battery state of a controller.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SCControllerBatteryState
        {
            /// <summary>
            /// Battery level as a percentage (0.0 to 1.0).
            /// </summary>
            public float level;

            /// <summary>
            /// Battery charging state (enumerated value).
            /// </summary>
            public SCGCDeviceBatteryState state;
        }

        /// <summary>
        /// Represents the motion detection state of a controller.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SCControllerMotionState
        {
            /// <summary>
            /// The controller attitude relative to vertical facing the user.
            /// </summary>
            public float4 attitude; // quaternion

            /// <summary>
            /// The controller rotation rate around 3 axes.
            /// </summary>
            public float3 rotationRate;

            /// <summary>
            /// The controller combined linear acceleration of gravity and user motion.
            /// </summary>
            public float3 acceleration;

            /// <summary>
            /// The controller linear acceleration of gravity.
            /// </summary>
            public float3 gravity;

            /// <summary>
            /// The controller linear acceleration due to user motion.
            /// </summary>
            public float3 userAcceleration;

            /// <summary>
            /// Indicates whether this controller's motion sensors are currently active.
            /// </summary>
            public SCBool sensorsActive;
            // pad 3 bytes
        }

        /// <summary>
        /// Represents an RGB color.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SCColor
        {
            /// <summary>Color red component in the range 0.0 to 1.0</summary>
            public float red;

            /// <summary>Color green component in the range 0.0 to 1.0</summary>
            public float green;

            /// <summary>Color blue component in the range 0.0 to 1.0</summary>
            public float blue;
        }

        /// <summary>
        /// Represents the light state of a controller.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SCControllerLightState
        {
            /// <summary>The current controller light color.</summary>
            public SCColor color;
        }

        /// <summary>
        /// Complete state information for a spatial controller.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCControllerState
        {
            /// <summary>
            /// Current input state of the controller.
            /// </summary>
            public SCControllerInputState input;

            /// <summary>
            /// Current battery state of the controller.
            /// </summary>
            public SCControllerBatteryState battery;

            /// <summary>
            /// Current motion sensing state of the controller.
            /// </summary>
            public SCControllerMotionState motion;

            /// <summary>
            /// Current light state of the controller, or (0,0,0) if no light is present.
            /// </summary>
            public SCControllerLightState light;

            /// <summary>
            /// Collection of associated accessory anchors.
            /// </summary>
            public SCAccessoryAnchors accessoryAnchors;

            /// <summary>
            /// Pointer to the associated accessory, if any.
            /// </summary>
            public IntPtr accessory;
        }

        /// <summary>
        /// Represents haptics capabilities of a controller device.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SCControllerMotionCapabilities
        {
            /// <summary>
            /// This controller supports sensing attitude relative to vertical facing the user.
            /// </summary>
            public SCBool hasAttitude;
            /// <summary>
            /// This controller supports sensing the rotation rate around 3 axes.
            /// </summary>
            public SCBool hasRotationRate;
            /// <summary>
            /// This controller supports sensing linear acceleration of gravity and user motion.
            /// </summary>
            public SCBool hasGravityAndUserAcceleration;
            /// <summary>
            /// This controller's motion sensors must be manually activated.
            /// </summary>
            public SCBool sensorsRequireManualActivation;
        }

        /// <summary>
        /// Represents haptics capabilities of a controller device.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SCDeviceHaptics
        {
            /// <summary>
            /// Set of SCHapticsLocality localities supported by this controller device.
            /// Stored as bit set union of (1<<SCHapticsLocality).
            /// </summary>
            public uint supportedLocalities;
        }

        /// <summary>
        /// Represents a spatial controller device with its capabilities and properties.
        /// Uses explicit layout to avoid padding issues.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCController
        {
            /// <summary>
            /// Unique identifier for the controller.
            /// </summary>
            public string uniqueId;

            /// <summary>
            /// Product category of the controller.
            /// </summary>
            public string productCategory;

            /// <summary>
            /// Vendor/manufacturer name of the controller.
            /// </summary>
            public string vendorName;

            /// <summary>
            /// Indicates if the controller is physically attached to the device.
            /// </summary>
            public SCBool isAttachedToDevice;

            /// <summary>
            /// Indicates if the controller has battery status reporting.
            /// </summary>
            public SCBool hasBattery;

            /// <summary>
            /// Indicates if the controller has motion sensing capabilities.
            /// </summary>
            public SCBool hasMotion;

            /// <summary>
            /// Indicates if the controller has controllable lights.
            /// </summary>
            public SCBool hasLight;

            /// <summary>
            /// Indicates if the controller supports haptic feedback.
            /// </summary>
            public SCBool hasHaptics;

            /// <summary>
            /// If hasMotion, controller motion capabilities.
            /// </summary>
            public SCControllerMotionCapabilities motion;
            // pad 3 bytes

            /// <summary>
            /// If hasHaptics, controller haptic capabilities.
            /// </summary>
            public SCDeviceHaptics haptics;
        }

        /// <summary>
        /// Represents a location name for an accessory.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCAccessoryLocation
        {
            /// <summary>
            /// The name of the accessory location.
            /// </summary>
            public string name;
        }

        /// <summary>
        /// Collection of accessory locations with pointer and count information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCAccessoryLocations
        {
            /// <summary>
            /// Pointer to the array of accessory locations.
            /// </summary>
            public IntPtr ptr;

            /// <summary>
            /// Number of accessory locations in the collection.
            /// </summary>
            public int count;
            // pad 4 bytes
        }

        /// <summary>
        /// Represents an accessory device that can be attached to a controller.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCAccessory
        {
            /// <summary>
            /// Unique identifier for the accessory.
            /// </summary>
            public string id;

            /// <summary>
            /// Display name of the accessory.
            /// </summary>
            public string name;

            /// <summary>
            /// Path to the USDZ file for 3D representation.
            /// </summary>
            public string usdzFile;

            /// <summary>
            /// Human-readable description of the accessory.
            /// </summary>
            public string description;

            /// <summary>
            /// Inherent chirality (handedness) of the accessory.
            /// </summary>
            public SCAccessoryChirality inherentChirality;
            // pad 4 bytes

            /// <summary>
            /// Collection of possible attachment locations.
            /// </summary>
            public SCAccessoryLocations locations;

            /// <summary>
            /// The source controller for this accessory.
            /// </summary>
            public SCController source;
        }

        /// <summary>
        /// Collection of controllers with pointer and count information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCControllers
        {
            /// <summary>
            /// Pointer to the array of controllers.
            /// </summary>
            public IntPtr ptr;

            /// <summary>
            /// Number of controllers in the collection.
            /// </summary>
            public int count;
            // pad 4 bytes
        }

        /// <summary>
        /// Collection of accessories with pointer and count information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCAccessories
        {
            /// <summary>
            /// Pointer to the array of accessories.
            /// </summary>
            public IntPtr ptr;

            /// <summary>
            /// Number of accessories in the collection.
            /// </summary>
            public int count;
            // pad 4 bytes
        }

        /// <summary>
        /// Represents information about a controller input element.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCControllerInputInfo
        {
            /// <summary>
            /// Localized name of the controller input element.
            /// </summary>
            public string localizedName;
            /// <summary>
            /// SF Symbols name for the symbol associated with the controller input element.
            /// </summary>
            public string symbolName;
        }

        /// <summary>
        /// Represents a symbol/icon with image data in PNG format and dimensions.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCSymbol
        {
            /// <summary>
            /// Width of the symbol in pixels.
            /// </summary>
            public int width;

            /// <summary>
            /// Height of the symbol in pixels.
            /// </summary>
            public int height;

            /// <summary>
            /// Pointer to the image data in PNG format.
            /// </summary>
            public IntPtr data;

            /// <summary>
            /// Length of the image data in bytes.
            /// </summary>
            public int dataLength;
            // pad 4 bytes
        }

        /// <summary>
        /// Represents an error with code and localized description.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct SCError
        {
            /// <summary>
            /// Numeric error code.
            /// </summary>
            public int code;
            // pad 4 bytes

            /// <summary>
            /// Localized error description.
            /// </summary>
            public string localizedDescription;
        }

        /// <summary>
        /// Callback delegate for successful operations.
        /// </summary>
        delegate void SCSuccessCallback();

        /// <summary>
        /// Callback delegate for error handling.
        /// </summary>
        /// <param name="error">The error that occurred.</param>
        delegate void SCErrorCallback(SCError error);

        /// <summary>
        /// Callback delegate for controller connection events.
        /// </summary>
        /// <param name="controller">The controller data.</param>
        delegate void SCControllerConnectionCallback(SCController controller);

        /// <summary>
        /// Callback delegate for accessory connection events.
        /// </summary>
        /// <param name="accessory">The accessory data.</param>
        delegate void SCAccessoryConnectionCallback(SCAccessory accessory);

        /// <summary>
        /// Callback delegate for single controller operations.
        /// </summary>
        /// <param name="controller">The controller data.</param>
        /// <param name="context">User-defined context pointer.</param>
        delegate void SCControllerCallback(SCController controller, IntPtr context);

        /// <summary>
        /// Callback delegate for multiple controller operations.
        /// </summary>
        /// <param name="controllers">Collection of controllers.</param>
        /// <param name="context">User-defined context pointer.</param>
        delegate void SCControllersCallback(SCControllers controllers, IntPtr context);

        /// <summary>
        /// Callback delegate for single accessory operations.
        /// </summary>
        /// <param name="accessory">The accessory data.</param>
        /// <param name="context">User-defined context pointer.</param>
        delegate void SCAccessoryCallback(SCAccessory accessory, IntPtr context);

        /// <summary>
        /// Callback delegate for multiple accessory operations.
        /// </summary>
        /// <param name="accessories">Collection of accessories.</param>
        /// <param name="context">User-defined context pointer.</param>
        delegate void SCAccessoriesCallback(SCAccessories accessories, IntPtr context);

        /// <summary>
        /// Callback delegate for accessory anchor operations.
        /// </summary>
        /// <param name="accessoryAnchor">The accessory anchor data.</param>
        /// <param name="accessory">The associated accessory.</param>
        /// <param name="context">User-defined context pointer.</param>
        delegate void SCAccessoryAnchorCallback(SCAccessoryAnchor accessoryAnchor, SCAccessory accessory, IntPtr context);

        /// <summary>
        /// Callback delegate for controller state polling.
        /// </summary>
        /// <param name="state">The current controller state.</param>
        /// <param name="context">User-defined context pointer.</param>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        delegate void SCControllerStateCallback(SCControllerState state, IntPtr context, string uniqueId);

        /// <summary>
        /// Callback delegate for controller input info retrieval operations.
        /// </summary>
        /// <param name="info">The retrieved controller input info.</param>
        /// <param name="context">User-defined context pointer.</param>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="inputName">Name of the input for which the info was requested.</param>
        delegate void SCGetControllerInputInfoForInputNameCallback(SCControllerInputInfo info, IntPtr context, string uniqueId, SCControllerInputName inputName);

        /// <summary>
        /// Callback delegate for symbol/icon retrieval operations.
        /// </summary>
        /// <param name="symbol">The retrieved symbol data in PNG format.</param>
        /// <param name="context">User-defined context pointer.</param>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="inputName">Name of the input for which the symbol was requested.</param>
        /// <param name="symbolScale">Requested scale of the symbol.</param>
        /// <param name="renderingMode">Requested rendering mode for the symbol.</param>
        delegate void SCGetSymbolForInputNameCallback(SCSymbol symbol, IntPtr context, string uniqueId, SCControllerInputName inputName, SCUIImageSymbolScale symbolScale, SCUIImageRenderingMode renderingMode);

        /// <summary>
        /// Callback delegate invoked when haptics playback finishes.
        /// </summary>
        /// <param name="error">If not null, an error caused playback to finish.</param>
        /// <returns>An action specifying whether to stop or leave the engine running.</returns>
        delegate SCHapticEngineFinishedAction SCHapticEngineFinishedCallback(SCError error, IntPtr context, string uniqueId, SCHapticsLocality locality);

        /// <summary>
        /// Callback delegate invoked when haptics engine shutdown completes.
        /// </summary>
        /// <param name="error">If not null, an error occurred during shutdown.</param>
        delegate void SCStopHapticEngineCompletedCallback(SCError error, IntPtr context, string uniqueId, SCHapticsLocality locality);

        /// <summary>
        /// Writer class for receiving arrays of controllers from native callbacks.
        /// </summary>
        class ControllerArrayWriter
        {
            /// <summary>
            /// Array of controllers populated by native callback.
            /// </summary>
            public Controller[]? controllers;
        }

        /// <summary>
        /// Writer class for receiving a single controller from native callbacks.
        /// </summary>
        class ControllerWriter
        {
            /// <summary>
            /// Controller instance populated by native callback.
            /// </summary>
            public Controller? controller;
        }

        /// <summary>
        /// Writer class for receiving arrays of accessories from native callbacks.
        /// </summary>
        class AccessoryArrayWriter
        {
            /// <summary>
            /// Array of accessories populated by native callback.
            /// </summary>
            public Accessory[]? accessories;
        }

        /// <summary>
        /// Writer class for receiving a single accessory from native callbacks.
        /// </summary>
        class AccessoryWriter
        {
            /// <summary>
            /// Accessory instance populated by native callback.
            /// </summary>
            public Accessory? accessory;
        }

        /// <summary>
        /// Writer class for receiving accessory anchor data from native callbacks.
        /// </summary>
        class AccessoryAnchorWriter
        {
            /// <summary>
            /// Accessory anchor populated by native callback.
            /// </summary>
            public AccessoryAnchor? accessoryAnchor;
        }

        /// <summary>
        /// Writer class for receiving controller input info from native callbacks.
        /// </summary>
        class ControllerInputInfoWriter
        {
            /// <summary>
            /// ControllerInputInfo instance populated by native callback.
            /// </summary>
            public ControllerInputInfo? info;
        }

        /// <summary>
        /// Writer class for receiving symbol data from native callbacks.
        /// </summary>
        class SymbolWriter
        {
            /// <summary>
            /// Symbol instance populated by native callback.
            /// </summary>
            public Symbol? symbol;
        }

        /// <summary>
        /// Writer class for receiving controller state data from native callbacks.
        /// </summary>
        class ControllerStateWriter
        {
            /// <summary>
            /// Controller state populated by native callback.
            /// </summary>
            public ControllerState? state;
        }

        /// <summary>
        /// Registers controller connected and disconnected event handlers.
        /// </summary>
        internal static bool AddControllerConnectionHandlers(ControllerConnectionCallbackWrapper onConnected, ControllerConnectionCallbackWrapper onDisconnected)
        {
            bool hadAnyHandlers = onControllerConnected != null || onControllerDisonnected != null;
            onControllerConnected += onConnected;
            onControllerDisonnected += onDisconnected;
            if (hadAnyHandlers)
            {
                return true;
            }
            return SpatialController_SetControllerConnectionHandlers(OnControllerConnected, OnControllerDisconnected);
        }

        /// <summary>
        /// Unregisters controller connected and disconnected event handlers.
        /// </summary>
        internal static void RemoveControllerConnectionHandlers(ControllerConnectionCallbackWrapper onConnected, ControllerConnectionCallbackWrapper onDisconnected)
        {
            onControllerConnected -= onConnected;
            onControllerDisonnected -= onDisconnected;
        }

        /// <summary>
        /// Registers accessory connected and disconnected event handlers.
        /// </summary>
        internal static bool AddAccessoryConnectionHandlers(AccessoryConnectionCallbackWrapper onConnected, AccessoryConnectionCallbackWrapper onDisconnected)
        {
            bool hadAnyHandlers = onAccessoryConnected != null || onAccessoryDisconnected != null;
            onAccessoryConnected += onConnected;
            onAccessoryDisconnected += onDisconnected;
            if (hadAnyHandlers)
            {
                return true;
            }
            return SpatialController_SetAccessoryConnectionHandlers(OnAccessoryConnected, OnAccessoryDisconnected);
        }

        /// <summary>
        /// Unregisters accessory connected and disconnected event handlers.
        /// </summary>
        internal static void RemoveAccessoryConnectionHandlers(AccessoryConnectionCallbackWrapper onConnected, AccessoryConnectionCallbackWrapper onDisconnected)
        {
            onAccessoryConnected -= onConnected;
            onAccessoryDisconnected -= onDisconnected;
        }

        /// <summary>
        /// Discards all registered event handlers.
        /// </summary>
        internal static void RemoveAllEventHandlers()
        {
            onControllerConnected = null;
            onControllerDisonnected = null;
            onAccessoryConnected = null;
            onAccessoryDisconnected = null;
            playHapticsCalls = new();
            playHapticsCallNextContext = 0;
            stopHapticsCalls = new();
            stopHapticsCallNextContext = 0;
        }

        /// <summary>
        /// Native callback handler for controller connected events.
        /// </summary>
        /// <param name="sc">Native controller structure.</param>
        [MonoPInvokeCallback(typeof(SCControllerConnectionCallback))]
        static void OnControllerConnected(SCController sc)
        {
            if (onControllerConnected != null)
            {
                var controller = MakeController(sc);
                onControllerConnected?.Invoke(controller);
            }
        }

        /// <summary>
        /// Native callback handler for controller disconnected events.
        /// </summary>
        /// <param name="sc">Native controller structure.</param>
        [MonoPInvokeCallback(typeof(SCControllerConnectionCallback))]
        static void OnControllerDisconnected(SCController sc)
        {
            if (onControllerDisonnected != null)
            {
                var controller = MakeController(sc);
                onControllerDisonnected?.Invoke(controller);
            }
        }

        /// <summary>
        /// Native callback handler for accessory connected events.
        /// </summary>
        /// <param name="sc">Native accessory structure.</param>
        [MonoPInvokeCallback(typeof(SCAccessoryConnectionCallback))]
        static void OnAccessoryConnected(SCAccessory sc)
        {
            if (onAccessoryConnected != null)
            {
                var accessory = MakeAccessory(sc);
                onAccessoryConnected?.Invoke(accessory);
            }
        }

        /// <summary>
        /// Native callback handler for accessory disconnected events.
        /// </summary>
        /// <param name="sc">Native accessory structure.</param>
        [MonoPInvokeCallback(typeof(SCAccessoryConnectionCallback))]
        static void OnAccessoryDisconnected(SCAccessory sc)
        {
            if (onAccessoryDisconnected != null)
            {
                var accessory = MakeAccessory(sc);
                onAccessoryDisconnected?.Invoke(accessory);
            }
        }

        /// <summary>
        /// Native callback handler for asynchronous StopHapticsEngine completion.
        /// </summary>
        /// <param name="scError">Native error structure.</param>
        /// <param name="context">Opaque context pointer.</param>
        /// <param name="uniqueId">Controller uniqueId of PlayHapticData call.</param>
        /// <param name="locality">Locality of PlayHapticData call.</param>
        [MonoPInvokeCallback(typeof(SCStopHapticEngineCompletedCallback))]
        static void OnStopHapticsEngineCompleted(SCError scError, IntPtr context, string uniqueId, SCHapticsLocality locality)
        {
            var error = MakeError(scError);
            int key = (int)context;
            StopHapticsCall call;
            if (!stopHapticsCalls.TryGetValue(key, out call))
            {
                Debug.LogError($"SpatialController: OnStopHapticsEngineCompleted(context:{key}, {uniqueId}, {locality}) call context not found!");
                return;
            }
            if (uniqueId != call.uniqueId || locality != call.locality)
            {
                Debug.LogError($"SpatialController: OnStopHapticsEngineCompleted(context:{key}, {uniqueId}, {locality}) call mismatch against expected {call.uniqueId}, {call.locality}!");
            }
            stopHapticsCalls.Remove(key);
            call.onCompleted(error);
            return;
        }

        /// <summary>
        /// Native callback handler for PlayHapticData finished events.
        /// </summary>
        /// <param name="scError">Native error structure.</param>
        /// <param name="context">Opaque context pointer.</param>
        /// <param name="uniqueId">Controller uniqueId of PlayHapticData call.</param>
        /// <param name="locality">Locality of PlayHapticData call.</param>
        /// <returns>A value controlling whether the engine should be stopped or left running.</returns>
        [MonoPInvokeCallback(typeof(SCHapticEngineFinishedCallback))]
        static SCHapticEngineFinishedAction OnHapticEngineFinished(SCError scError, IntPtr context, string uniqueId, SCHapticsLocality locality)
        {
            var error = MakeError(scError);
            int key = (int)context;
            PlayHapticsCall call;
            if (!playHapticsCalls.TryGetValue(key, out call))
            {
                Debug.LogError($"SpatialController: OnHapticEngineFinished(context:{key}, {uniqueId}, {locality}) call context not found!");
                return SCHapticEngineFinishedAction.StopEngine;
            }
            if (uniqueId != call.uniqueId || locality != call.locality)
            {
                Debug.LogError($"SpatialController: OnHapticEngineFinished(context:{key}, {uniqueId}, {locality}) call mismatch against expected {call.uniqueId}, {call.locality}!");
            }
            playHapticsCalls.Remove(key);
            return call.onFinished(error);
        }

        /// <summary>
        /// Converts a native error structure to a managed Error object.
        /// </summary>
        /// <param name="sc">Native error structure.</param>
        /// <returns>Managed Error instance.</returns>
        static Error? MakeError(SCError sc)
        {
            if (sc.code == 0)
            {
                return null;
            }
            return new Error(sc.code, sc.localizedDescription);
        }

        /// <summary>
        /// Converts a native motion capabilities structure to a managed ControllerMotionCapabilities object.
        /// </summary>
        /// <param name="sc">Native motion capabilities structure.</param>
        /// <returns>Managed ControllerMotionCapabilities instance.</returns>
        static ControllerMotionCapabilities MakeControllerMotionCapabilities(SCControllerMotionCapabilities sc)
        {
            return new ControllerMotionCapabilities(
                sc.hasAttitude,
                sc.hasRotationRate,
                sc.hasGravityAndUserAcceleration,
                sc.sensorsRequireManualActivation
            );
        }

        /// <summary>
        /// Converts a native haptics structure to a managed DeviceHaptics object.
        /// </summary>
        /// <param name="sc">Native haptics structure.</param>
        /// <returns>Managed DeviceHaptics instance.</returns>
        static DeviceHaptics MakeDeviceHaptics(SCDeviceHaptics sc)
        {
            HashSet<HapticsLocality> supportedLocalities = new();
            var allLocalities = (HapticsLocality[])Enum.GetValues(typeof(HapticsLocality));
            foreach (var locality in allLocalities)
            {
                if ((sc.supportedLocalities & (1u << (int)locality)) != 0)
                {
                    supportedLocalities.Add(locality);
                }
            }
            return new DeviceHaptics(supportedLocalities);
        }

        /// <summary>
        /// Converts a native controller structure to a managed Controller object.
        /// </summary>
        /// <param name="sc">Native controller structure.</param>
        /// <returns>Managed Controller instance.</returns>
        static Controller MakeController(SCController sc)
        {
            return new Controller(
                sc.uniqueId,
                sc.productCategory,
                sc.vendorName,
                sc.isAttachedToDevice,
                sc.hasBattery,
                sc.hasMotion ? MakeControllerMotionCapabilities(sc.motion) : null,
                sc.hasLight,
                sc.hasHaptics ? MakeDeviceHaptics(sc.haptics) : null);
        }

        /// <summary>
        /// Converts a native accessory structure to a managed Accessory object.
        /// </summary>
        /// <param name="sc">Native accessory structure.</param>
        /// <returns>Managed Accessory instance.</returns>
        static Accessory MakeAccessory(SCAccessory sc)
        {
            return new Accessory(
                sc.id,
                sc.name,
                sc.usdzFile,
                sc.description,
                sc.inherentChirality,
                MakeAccessoryLocationNameArray(sc.locations),
                MakeController(sc.source)
            );
        }

        /// <summary>
        /// Converts an ARKit 4x4 matrix to a Unity Pose, handling coordinate system differences.
        /// ARKit uses left-handed coordinates while Unity uses right-handed.
        /// </summary>
        /// <param name="m">ARKit transformation matrix.</param>
        /// <returns>Unity Pose with converted coordinates.</returns>
        static Pose MakeUnityPoseFromARKitFloat4x4(float4x4 m)
        {
            // Extract position from the 4th column (translation)
            float3 position = m.c3.xyz;
            // Extract rotation by normalizing the rotation part of the matrix
            float3x3 rotationMatrix = new float3x3(
                math.normalize(m.c0.xyz),
                math.normalize(m.c1.xyz),
                math.normalize(m.c2.xyz));
            quaternion rotation = new(rotationMatrix);

            // ARKit matrices are left handed, Unity transforms are right handed (Z flipped):
            position.z = -position.z;
            rotation = new quaternion(new float4(rotation.value.x, rotation.value.y, -rotation.value.z, -rotation.value.w));

            return new Pose(position, rotation);
        }

        /// <summary>
        /// Converts an ARKit native pose to a Unity Pose, handling coordinate system differences.
        /// ARKit uses left-handed coordinates while Unity uses right-handed.
        /// </summary>
        /// <param name="sc">ARKit native pose.</param>
        /// <returns>Unity Pose with converted coordinates.</returns>
        static Pose MakeUnityPose(SCPose sc)
        {
            float3 position = sc.position;
            quaternion rotation = new(sc.rotation);

            // ARKit matrices are left handed, Unity transforms are right handed (Z flipped):
            position.z = -position.z;
            rotation = new quaternion(new float4(rotation.value.x, rotation.value.y, -rotation.value.z, -rotation.value.w));

            return new Pose(position, rotation);
        }

        /// <summary>
        /// Converts a native accessory anchor structure to a managed AccessoryAnchor object.
        /// </summary>
        /// <param name="sc">Native accessory anchor structure.</param>
        /// <param name="accessory">Associated managed accessory object.</param>
        /// <returns>Managed AccessoryAnchor instance.</returns>
        static AccessoryAnchor MakeAccessoryAnchor(SCAccessoryAnchor sc, Accessory? accessory)
        {
            return new AccessoryAnchor(
                sc.id,
                sc.description,
                MakeUnityPoseFromARKitFloat4x4(sc.originFromAnchorTransform),
                sc.velocity,
                sc.angularVelocity,
                MakeTimeValue(sc.timestamp),
                sc.trackingState,
                sc.isHeld ? sc.heldChirality : null,
                sc.isTracked,
                accessory,
                MakePoseArray(sc.locationPoses));
        }

        /// <summary>
        /// Converts a native button state structure to a managed ButtonState object.
        /// </summary>
        /// <param name="sc">Native button state structure.</param>
        /// <returns>Managed ButtonState instance.</returns>
        static ButtonState MakeButtonState(SCButtonState sc)
        {
            return new ButtonState(sc.value, sc.isAnalog, sc.isPressed, sc.isTouched, MakeTimeValue(sc.lastPressedStateTimestamp), MakeTimeValue(sc.lastValueTimestamp));
        }

        /// <summary>
        /// Converts a native directional pad state structure to a managed DPadState object.
        /// </summary>
        /// <param name="sc">Native directional pad state structure.</param>
        /// <returns>Managed DPadState instance.</returns>
        static DPadState MakeDPadState(SCDPadState sc)
        {
            return new DPadState(sc.xAxis, sc.yAxis);
        }

        /// <summary>
        /// Converts a native controller input state structure to a managed ControllerInputState object.
        /// </summary>
        /// <param name="sc">Native controller input state structure.</param>
        /// <returns>Managed ControllerInputState instance.</returns>
        static ControllerInputState MakeControllerInputState(SCControllerInputState sc)
        {
            return new ControllerInputState(
                MakeButtonStateDictionary(sc.buttons),
                MakeDPadStateDictionary(sc.dpads)
            );
        }

        /// <summary>
        /// Converts a native controller battery state structure to a managed ControllerBatteryState object.
        /// </summary>
        /// <param name="sc">Native controller battery state structure.</param>
        /// <returns>Managed ControllerBatteryState instance.</returns>
        static ControllerBatteryState MakeControllerBatteryState(SCControllerBatteryState sc)
        {
            return new ControllerBatteryState(sc.level, sc.state);
        }

        /// <summary>
        /// Converts a native motion state structure to a managed ControllerMotionState object.
        /// </summary>
        /// <param name="sc">Native controller motion state structure.</param>
        /// <returns>Managed ControllerMotionState instance.</returns>
        static ControllerMotionState MakeControllerMotionState(SCControllerMotionState sc)
        {
            return new ControllerMotionState(sc.attitude, sc.rotationRate, sc.acceleration, sc.gravity, sc.userAcceleration, sc.sensorsActive);
        }

        /// <summary>
        /// Converts a native color structure to a managed Unity.Color object.
        /// </summary>
        /// <param name="sc">Native color structure.</param>
        /// <returns>Managed Unity.Color instance.</returns>
        static Color MakeColor(SCColor sc)
        {
            return new Color(sc.red, sc.green, sc.blue, 1.0f);
        }

        /// <summary>
        /// Converts a native controller light state structure to a managed ControllerLightState object.
        /// </summary>
        /// <param name="sc">Native controller light state structure.</param>
        /// <returns>Managed ControllerLightState instance.</returns>
        static ControllerLightState MakeControllerLightState(SCControllerLightState sc)
        {
            return new ControllerLightState(MakeColor(sc.color));
        }

        /// <summary>
        /// Converts a native controller state structure to a managed ControllerState object.
        /// </summary>
        /// <param name="sc">Native controller state structure.</param>
        /// <returns>Managed ControllerState instance.</returns>
        static ControllerState MakeControllerState(SCControllerState sc)
        {
            Accessory? accessory = null;
            if (sc.accessory != IntPtr.Zero)
            {
                var o = Marshal.PtrToStructure<SCAccessory>(sc.accessory);
                accessory = MakeAccessory(o);
            }
            return new ControllerState(
                MakeControllerInputState(sc.input),
                MakeControllerBatteryState(sc.battery),
                MakeControllerMotionState(sc.motion),
                MakeControllerLightState(sc.light),
                MakeAccessoryAnchorArray(sc.accessoryAnchors, accessory),
                accessory
            );
        }

        /// <summary>
        /// Converts a native controller input info structure to a managed ControllerInputInfo object.
        /// </summary>
        /// <param name="sc">Native symbol structure.</param>
        /// <returns>Managed Symbol instance with copied image data.</returns>
        static ControllerInputInfo MakeControllerInputInfo(SCControllerInputInfo sc)
        {
            return new ControllerInputInfo(sc.localizedName, sc.symbolName);
        }

        /// <summary>
        /// Converts a native symbol structure to a managed Symbol object.
        /// </summary>
        /// <param name="sc">Native symbol structure.</param>
        /// <returns>Managed Symbol instance with copied image data.</returns>
        static Symbol MakeSymbol(SCSymbol sc)
        {
            byte[] data = new byte[sc.dataLength];
            Marshal.Copy(sc.data, data, 0, sc.dataLength);
            return new Symbol(sc.width, sc.height, data);
        }

        /// <summary>
        /// Converts a native time value structure to a managed TimeValue object.
        /// </summary>
        /// <param name="sc">Native time value structure.</param>
        /// <returns>Managed TimeValue instance.</returns>
        static TimeValue MakeTimeValue(SCTimeValue sc)
        {
            return new TimeValue(sc.time);
        }

        /// <summary>
        /// Converts a native collection of button states to a managed dictionary.
        /// </summary>
        /// <param name="buttons">Native button states collection.</param>
        /// <returns>Dictionary mapping input names to button states.</returns>
        static Dictionary<ControllerInputName, ButtonState> MakeButtonStateDictionary(SCInputButtonStates buttons)
        {
            Dictionary<ControllerInputName, ButtonState> result = new();
            var ptr = buttons.ptr;
            var count = buttons.count;
            var structSize = Marshal.SizeOf(typeof(SCInputButtonState));
            for (int i = 0; i < count; i++)
            {
                var o = Marshal.PtrToStructure<SCInputButtonState>(ptr);
                result.Add(o.name, MakeButtonState(o.state));
                ptr += structSize;
            }
            return result;
        }

        /// <summary>
        /// Converts a native collection of directional pad states to a managed dictionary.
        /// </summary>
        /// <param name="dpads">Native directional pad states collection.</param>
        /// <returns>Dictionary mapping input names to directional pad states.</returns>
        static Dictionary<ControllerInputName, DPadState> MakeDPadStateDictionary(SCInputDPadStates dpads)
        {
            Dictionary<ControllerInputName, DPadState> result = new();
            var ptr = dpads.ptr;
            var count = dpads.count;
            var structSize = Marshal.SizeOf(typeof(SCInputDPadState));
            for (int i = 0; i < count; i++)
            {
                var o = Marshal.PtrToStructure<SCInputDPadState>(ptr);
                result.Add(o.name, MakeDPadState(o.state));
                ptr += structSize;
            }
            return result;
        }

        /// <summary>
        /// Converts a native collection of controllers to a managed array.
        /// </summary>
        /// <param name="controllers">Native controllers collection.</param>
        /// <returns>Array of managed Controller instances.</returns>
        static Controller[] MakeControllerArray(SCControllers controllers)
        {
            Controller[] result = new Controller[controllers.count];
            var ptr = controllers.ptr;
            var count = controllers.count;
            var structSize = Marshal.SizeOf(typeof(SCController));
            for (int i = 0; i < count; i++)
            {
                var o = Marshal.PtrToStructure<SCController>(ptr);
                result[i] = MakeController(o);
                ptr += structSize;
            }
            return result;
        }

        /// <summary>
        /// Converts a native collection of accessory locations to a managed array.
        /// </summary>
        /// <param name="locations">Native accessory locations collection.</param>
        /// <returns>Array of managed LocationName instances.</returns>
        static Accessory.LocationName[] MakeAccessoryLocationNameArray(SCAccessoryLocations locations)
        {
            Accessory.LocationName[] result = new Accessory.LocationName[locations.count];
            var ptr = locations.ptr;
            var count = locations.count;
            var structSize = Marshal.SizeOf(typeof(SCAccessoryLocation));
            for (int i = 0; i < count; i++)
            {
                var o = Marshal.PtrToStructure<SCAccessoryLocation>(ptr);
                result[i] = new Accessory.LocationName(o.name);
                ptr += structSize;
            }
            return result;
        }

        /// <summary>
        /// Converts a native collection of controllers to a managed array.
        /// </summary>
        /// <param name="controllers">Native controllers collection.</param>
        /// <returns>Array of managed Controller instances.</returns>
        static Pose[] MakePoseArray(SCPoses poses)
        {
            Pose[] result = new Pose[poses.count];
            var ptr = poses.ptr;
            var count = poses.count;
            var structSize = Marshal.SizeOf(typeof(SCPose));
            for (int i = 0; i < count; i++)
            {
                var o = Marshal.PtrToStructure<SCPose>(ptr);
                result[i] = MakeUnityPose(o);
                ptr += structSize;
            }
            return result;
        }

        /// <summary>
        /// Converts a native collection of accessory anchors to a managed array.
        /// </summary>
        /// <param name="accessoryAnchors">Native accessory anchors collection.</param>
        /// <param name="accessory">Associated managed accessory object.</param>
        /// <returns>Array of managed AccessoryAnchor instances.</returns>
        static AccessoryAnchor[] MakeAccessoryAnchorArray(SCAccessoryAnchors accessoryAnchors, Accessory? accessory)
        {
            AccessoryAnchor[] result = new AccessoryAnchor[accessoryAnchors.count];
            var ptr = accessoryAnchors.ptr;
            var count = accessoryAnchors.count;
            var structSize = Marshal.SizeOf(typeof(SCAccessoryAnchor));
            for (int i = 0; i < count; i++)
            {
                var o = Marshal.PtrToStructure<SCAccessoryAnchor>(ptr);
                result[i] = MakeAccessoryAnchor(o, accessory);
                ptr += structSize;
            }
            return result;
        }

        /// <summary>
        /// Converts a native collection of accessories to a managed array.
        /// </summary>
        /// <param name="accessories">Native accessories collection.</param>
        /// <returns>Array of managed Accessory instances.</returns>
        static Accessory[] MakeAccessoryArray(SCAccessories accessories)
        {
            Accessory[] result = new Accessory[accessories.count];
            var ptr = accessories.ptr;
            var count = accessories.count;
            var structSize = Marshal.SizeOf(typeof(SCAccessory));
            for (int i = 0; i < count; i++)
            {
                var o = Marshal.PtrToStructure<SCAccessory>(ptr);
                result[i] = MakeAccessory(o);
                ptr += structSize;
            }
            return result;
        }

        /// <summary>
        /// Initializes the spatial controller system.
        /// </summary>
        [DllImport("__Internal")]
        static extern void SpatialController_Init();

        /// <summary>
        /// Destroys and cleans up the spatial controller system.
        /// </summary>
        [DllImport("__Internal")]
        static extern void SpatialController_Destroy();

        /// <summary>
        /// Returns the current authorization state for AR accessory tracking.
        /// </summary>
        [DllImport("__Internal")]
        static extern SCAccessoryTrackingAuthorizationState SpatialController_GetAccessoryTrackingAuthorizationState();

        /// <summary>
        /// Returns the current running or error state for AR accessory tracking.
        /// </summary>
        [DllImport("__Internal")]
        static extern SCAccessoryTrackingState SpatialController_GetAccessoryTrackingState();

        /// <summary>
        /// Sets callback handlers for controller connection events.
        /// </summary>
        /// <param name="onConnected">Callback for controller connected events.</param>
        /// <param name="onDisconnected">Callback for controller disconnected events.</param>
        /// <returns>True if handlers were set successfully.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_SetControllerConnectionHandlers(SCControllerConnectionCallback onConnected, SCControllerConnectionCallback onDisconnected);

        /// <summary>
        /// Sets callback handlers for accessory connection events.
        /// </summary>
        /// <param name="onConnected">Callback for accessory connected events.</param>
        /// <param name="onDisconnected">Callback for accessory disconnected events.</param>
        /// <returns>True if handlers were set successfully.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_SetAccessoryConnectionHandlers(SCAccessoryConnectionCallback onConnected, SCAccessoryConnectionCallback onDisconnected);

        /// <summary>
        /// Retrieves all currently connected controllers.
        /// </summary>
        /// <param name="callback">Callback to receive the controllers.</param>
        /// <param name="context">User-defined context pointer.</param>
        /// <returns>True if the operation was initiated successfully.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_GetConnectedControllers(SCControllersCallback callback, IntPtr context);

        /// <summary>
        /// Retrieves a specific connected controller by unique ID.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="callback">Callback to receive the controller.</param>
        /// <param name="context">User-defined context pointer.</param>
        /// <returns>True if the operation was initiated successfully.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_GetConnectedController(string uniqueId, SCControllerCallback callback, IntPtr context);

        /// <summary>
        /// Retrieves all currently connected accessories.
        /// </summary>
        /// <param name="callback">Callback to receive the accessories.</param>
        /// <param name="context">User-defined context pointer.</param>
        /// <returns>True if the operation was initiated successfully.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_GetConnectedAccessories(SCAccessoriesCallback callback, IntPtr context);

        /// <summary>
        /// Retrieves a specific connected accessory by unique ID.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the accessory.</param>
        /// <param name="callback">Callback to receive the accessory.</param>
        /// <param name="context">User-defined context pointer.</param>
        /// <returns>True if the operation was initiated successfully.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_GetConnectedAccessory(string uniqueId, SCAccessoryCallback callback, IntPtr context);

        /// <summary>
        /// Sets a default configuration setting applied to controllers on connection.
        /// </summary>
        /// <param name="setting">Which controller setting to configure.</param>
        /// <param name="value">The value to configure, interpretation dependend on setting.</param>
        /// <returns>True if the setting was configured successfully.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_ConfigureDefaultControllerSetting(SCControllerSetting setting, int value);

        /// <summary>
        /// Sets a configuration setting to a specified controller.
        /// All connected controllers appropriate for a setting may be targeted by
        /// using the special uniqueId value "all" or "*".
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller, or "all" or "*".</param>
        /// <param name="setting">Which controller setting to configure.</param>
        /// <param name="value">The value to configure, interpretation dependend on setting.</param>
        /// <returns>True if the controller is a valid target and the setting was configured successfully.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_ConfigureControllerSetting(string uniqueId, SCControllerSetting setting, int value);

        /// <summary>
        /// Retrieves controller input info for a specific controller input.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="inputName">Name of the input for which to get the info.</param>
        /// <param name="callback">Callback to receive the info.</param>
        /// <param name="context">User-defined context pointer.</param>
        /// <returns>True if the operation was initiated successfully.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_GetControllerInputInfoForInputName(string uniqueId, SCControllerInputName inputName, SCGetControllerInputInfoForInputNameCallback callback, IntPtr context);

        /// <summary>
        /// Retrieves a symbol/icon in PNG format for a specific controller input.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="inputName">Name of the input for which to get the symbol.</param>
        /// <param name="symbolScale">Desired scale of the symbol.</param>
        /// <param name="renderingMode">Desired rendering mode for the symbol.</param>
        /// <param name="callback">Callback to receive the symbol.</param>
        /// <param name="context">User-defined context pointer.</param>
        /// <returns>True if the operation was initiated successfully.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_GetSymbolForInputName(string uniqueId, SCControllerInputName inputName, SCUIImageSymbolScale symbolScale, SCUIImageRenderingMode renderingMode, SCGetSymbolForInputNameCallback callback, IntPtr context);

        /// <summary>
        /// Sets the light color for a controller that supports lighting.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="r">Red component (0.0 to 1.0).</param>
        /// <param name="g">Green component (0.0 to 1.0).</param>
        /// <param name="b">Blue component (0.0 to 1.0).</param>
        /// <returns>True if the color was set successfully.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_SetControllerLightColor(string uniqueId, float r, float g, float b);

        /// <summary>
        /// Polls the current state of a controller.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="callback">Callback to receive the controller state.</param>
        /// <param name="context">User-defined context pointer.</param>
        /// <returns>True if the polling was initiated successfully.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_PollController(string uniqueId, SCControllerStateCallback callback, IntPtr context);

        /// <summary>
        /// Gets the current system time.
        /// </summary>
        /// <returns>Current time value.</returns>
        [DllImport("__Internal")]
        static extern SCTimeValue SpatialController_GetCurrentTime();

        /// <summary>
        /// Gets the predicted display time of the next frame for the current application window.
        /// </summary>
        /// <returns>Predicted time value or invalid (-1) if no prediction is available.</returns>
        [DllImport("__Internal")]
        static extern SCTimeValue SpatialController_GetPredictedNextFrameTime();

        /// <summary>
        /// Predicts the position of an accessory anchor at a future time.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the accessory.</param>
        /// <param name="time">Future time for prediction.</param>
        /// <param name="callback">Callback to receive the predicted anchor.</param>
        /// <param name="context">User-defined context pointer.</param>
        /// <returns>True if the prediction was initiated successfully.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_PredictAnchor(string uniqueId, SCTimeValue time, SCAccessoryAnchorCallback callback, IntPtr context);

        /// <summary>
        /// Create or restart the haptics engine for locality on the specified controller.
        /// Controllers which support multiple localities (excluding Default) allow
        /// an independent engine per locality. See Controller.supportedLocalities.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="locality">Locality of haptics. Must be supported by controller.</param>
        /// <returns>True if the haptics engine was started for controller uniqueId.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_CreateHapticsEngine(string uniqueId, SCHapticsLocality locality);

        /// <summary>
        /// Stops a haptics engine for the specified controller and locality.
        /// This function shuts down any `CHHapticEngine` for the controller identified
        /// by the given unique ID with the specified locality, if running.
        /// Controllers which support multiple localities (excluding Default) allow
        /// an independent engine per locality. See Controller.supportedLocalities.
        /// Engines with apparent locality overlap, such as All and Handles, can be
        /// independently controlled, and their outputs are composed, so stopping
        /// locality All, for instance, will not stop an engine running on Handles.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="locality">Locality of haptics. Must be supported by controller.</param>
        /// <param name="onCompleted">If return value is true, onCompleted will be called when shutdown completes.</param>
        /// <param name="context">User supplied opaque context pointer passed back to onCompleted callback.</param>
        /// <returns>True if a haptics engine was shutdown for controller uniqueId, or false if no engine needed to be shutdown.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_StopHapticsEngine(string uniqueId, SCHapticsLocality locality, SCStopHapticEngineCompletedCallback onCompleted, IntPtr context);

        /// <summary>
        /// Plays haptics data on the specified controller and locality.
        /// If a haptics engine has not yet been created, attempts to create one first.
        /// Controllers which support multiple localities (excluding Default) allow
        /// an independent engine per locality. See Controller.supportedLocalities.
        /// Engines with apparent locality overlap, such as All and Handles, can be
        /// independently controlled, and their outputs are composed.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="data">Binary haptics data from a haptics data file in a supported format such as AHAP.</param>
        /// <param name="dataLength">Length of binary haptics data in bytes.</param>
        /// <param name="locality">Locality of haptics. Must be supported by controller.</param>
        /// <param name="onFinished">If return value is true, onFinished will be called when playback finishes.</param>
        /// <param name="context">User supplied opaque context pointer passed back to onFinished callback.</param>
        /// <returns>True if haptics playback was started for controller uniqueId.</returns>
        [DllImport("__Internal")]
        static extern bool SpatialController_PlayHapticsData(string uniqueId, IntPtr data, int dataLength, SCHapticsLocality locality, SCHapticEngineFinishedCallback onFinished, IntPtr context);

        /// <summary>
        /// Native callback handler for connected controllers retrieval.
        /// </summary>
        /// <param name="controllers">Native controllers collection.</param>
        /// <param name="context">Native context pointer to ControllerArrayWriter.</param>
        [MonoPInvokeCallback(typeof(SCControllersCallback))]
        static void GetConnectedControllersCallback(SCControllers controllers, IntPtr context)
        {
            GCHandle handle = GCHandle.FromIntPtr(context);
            ControllerArrayWriter result = (ControllerArrayWriter)handle.Target;
            result.controllers = MakeControllerArray(controllers);
        }

        /// <summary>
        /// Native callback handler for single controller retrieval.
        /// </summary>
        /// <param name="controller">Native controller structure.</param>
        /// <param name="context">Native context pointer to ControllerWriter.</param>
        [MonoPInvokeCallback(typeof(SCControllerCallback))]
        static void GetConnectedControllerCallback(SCController controller, IntPtr context)
        {
            GCHandle handle = GCHandle.FromIntPtr(context);
            ControllerWriter result = (ControllerWriter)handle.Target;
            result.controller = MakeController(controller);
        }

        /// <summary>
        /// Native callback handler for connected accessories retrieval.
        /// </summary>
        /// <param name="accessories">Native accessories collection.</param>
        /// <param name="context">Native context pointer to AccessoryArrayWriter.</param>
        [MonoPInvokeCallback(typeof(SCAccessoriesCallback))]
        static void GetConnectedAccessoriesCallback(SCAccessories accessories, IntPtr context)
        {
            GCHandle handle = GCHandle.FromIntPtr(context);
            AccessoryArrayWriter result = (AccessoryArrayWriter)handle.Target;
            result.accessories = MakeAccessoryArray(accessories);
        }

        /// <summary>
        /// Native callback handler for single accessory retrieval.
        /// </summary>
        /// <param name="accessory">Native accessory structure.</param>
        /// <param name="context">Native context pointer to AccessoryWriter.</param>
        [MonoPInvokeCallback(typeof(SCAccessoryCallback))]
        static void GetConnectedAccessoryCallback(SCAccessory accessory, IntPtr context)
        {
            GCHandle handle = GCHandle.FromIntPtr(context);
            AccessoryWriter result = (AccessoryWriter)handle.Target;
            result.accessory = MakeAccessory(accessory);
        }

        /// <summary>
        /// Native callback handler for controller input info retrieval operations.
        /// </summary>
        /// <param name="symbol">Native controller input info structure.</param>
        /// <param name="context">Native context pointer to SymbolWriter.</param>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="inputName">Name of the input for which the info was requested.</param>
        [MonoPInvokeCallback(typeof(SCGetControllerInputInfoForInputNameCallback))]
        static void GetControllerInputInfoForInputNameCallback(SCControllerInputInfo info, IntPtr context, string uniqueId, SCControllerInputName inputName)
        {
            GCHandle handle = GCHandle.FromIntPtr(context);
            ControllerInputInfoWriter result = (ControllerInputInfoWriter)handle.Target;
            result.info = MakeControllerInputInfo(info);
        }

        /// <summary>
        /// Native callback handler for symbol retrieval operations.
        /// </summary>
        /// <param name="symbol">Native symbol structure.</param>
        /// <param name="context">Native context pointer to SymbolWriter.</param>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="inputName">Name of the input for which the symbol was requested.</param>
        /// <param name="symbolScale">Requested scale of the symbol.</param>
        /// <param name="renderingMode">Requested rendering mode for the symbol.</param>
        [MonoPInvokeCallback(typeof(SCGetSymbolForInputNameCallback))]
        static void GetSymbolForInputNameCallback(SCSymbol symbol, IntPtr context, string uniqueId, SCControllerInputName inputName, SCUIImageSymbolScale symbolScale, SCUIImageRenderingMode renderingMode)
        {
            GCHandle handle = GCHandle.FromIntPtr(context);
            SymbolWriter result = (SymbolWriter)handle.Target;
            result.symbol = MakeSymbol(symbol);
        }

        /// <summary>
        /// Native callback handler for controller state polling operations.
        /// </summary>
        /// <param name="state">Native controller state structure.</param>
        /// <param name="context">Native context pointer to ControllerStateWriter.</param>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        [MonoPInvokeCallback(typeof(SCControllerStateCallback))]
        static void PollControllerCallback(SCControllerState state, IntPtr context, string uniqueId)
        {
            GCHandle handle = GCHandle.FromIntPtr(context);
            ControllerStateWriter result = (ControllerStateWriter)handle.Target;
            result.state = MakeControllerState(state);
        }

        /// <summary>
        /// Native callback handler for accessory anchor prediction operations.
        /// </summary>
        /// <param name="scAnchor">Native accessory anchor structure.</param>
        /// <param name="scAccessory">Native accessory structure.</param>
        /// <param name="context">Native context pointer to AccessoryAnchorWriter.</param>
        [MonoPInvokeCallback(typeof(SCAccessoryAnchorCallback))]
        static void PredictAnchorCallback(SCAccessoryAnchor scAnchor, SCAccessory scAccessory, IntPtr context)
        {
            GCHandle handle = GCHandle.FromIntPtr(context);
            AccessoryAnchorWriter result = (AccessoryAnchorWriter)handle.Target;
            var accessory = MakeAccessory(scAccessory);
            result.accessoryAnchor = MakeAccessoryAnchor(scAnchor, accessory);
        }

        /// <summary>
        /// Converts a managed TimeValue to a native SCTimeValue structure.
        /// </summary>
        /// <param name="time">Managed time value.</param>
        /// <returns>Native time value structure.</returns>
        static SCTimeValue MakeSCTimeValue(TimeValue time)
        {
            SCTimeValue scTime = new();
            scTime.time = time.time;
            return scTime;
        }

        /// <summary>
        /// Initializes the spatial controller system.
        /// Must be called before using any other spatial controller functionality.
        /// </summary>
        internal static void Init()
        {
            SpatialController_Init();
        }

        /// <summary>
        /// Destroys and cleans up the spatial controller system.
        /// Should be called when spatial controller functionality is no longer needed.
        /// </summary>
        internal static void Destroy()
        {
            SpatialController_Destroy();
            RemoveAllEventHandlers();
        }

        /// <summary>
        /// Returns the current authorization state for AR accessory tracking.
        /// </summary>
        internal static SCAccessoryTrackingAuthorizationState GetAccessoryTrackingAuthorizationState()
        {
            return SpatialController_GetAccessoryTrackingAuthorizationState();
        }

        /// <summary>
        /// Returns the current running or error state for AR accessory tracking.
        /// </summary>
        internal static SCAccessoryTrackingState GetAccessoryTrackingState()
        {
            return SpatialController_GetAccessoryTrackingState();
        }

        /// <summary>
        /// Retrieves all currently connected controllers.
        /// </summary>
        /// <returns>An array of connected controllers, or an empty array if none are connected.</returns>
        public static Controller[] GetConnectedControllers()
        {
            ControllerArrayWriter result = new();
            GCHandle handle = GCHandle.Alloc(result);
            if (!SpatialController_GetConnectedControllers(GetConnectedControllersCallback, GCHandle.ToIntPtr(handle)))
            {
                return new Controller[0];
            }
            Controller[] controllers = result.controllers ?? new Controller[0];
            handle.Free();
            return controllers;
        }

        /// <summary>
        /// Retrieves a specific connected controller by its unique identifier.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the controller to retrieve.</param>
        /// <returns>The controller with the specified ID, or null if not found.</returns>
        internal static Controller? GetConnectedController(string uniqueId)
        {
            ControllerWriter result = new();
            GCHandle handle = GCHandle.Alloc(result);
            if (!SpatialController_GetConnectedController(uniqueId, GetConnectedControllerCallback, GCHandle.ToIntPtr(handle)))
            {
                return null;
            }
            Controller? controller = result.controller;
            handle.Free();
            return controller;
        }

        /// <summary>
        /// Retrieves all currently connected accessories.
        /// </summary>
        /// <returns>An array of connected accessories, or an empty array if none are connected.</returns>
        public static Accessory[] GetConnectedAccessories()
        {
            AccessoryArrayWriter result = new();
            GCHandle handle = GCHandle.Alloc(result);
            if (!SpatialController_GetConnectedAccessories(GetConnectedAccessoriesCallback, GCHandle.ToIntPtr(handle)))
            {
                return new Accessory[0];
            }
            Accessory[] accessories = result.accessories ?? new Accessory[0];
            handle.Free();
            return accessories;
        }

        /// <summary>
        /// Retrieves a specific connected accessory by its unique identifier.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the accessory to retrieve.</param>
        /// <returns>The accessory with the specified ID, or null if not found.</returns>
        internal static Accessory? GetConnectedAccessory(string uniqueId)
        {
            AccessoryWriter result = new();
            GCHandle handle = GCHandle.Alloc(result);
            if (!SpatialController_GetConnectedAccessory(uniqueId, GetConnectedAccessoryCallback, GCHandle.ToIntPtr(handle)))
            {
                return null;
            }
            Accessory? accessory = result.accessory;
            handle.Free();
            return accessory;
        }

        /// <summary>
        /// Sets a default configuration setting applied to controllers on connection.
        /// </summary>
        /// <param name="setting">Which controller setting to configure.</param>
        /// <param name="value">The value to configure, interpretation dependend on setting.</param>
        /// <returns>True if the setting was configured successfully.</returns>
        internal static bool ConfigureDefaultControllerSetting(SCControllerSetting setting, int value)
        {
            return SpatialController_ConfigureDefaultControllerSetting(setting, value);
        }

        /// <summary>
        /// Sets a configuration setting to a specified controller.
        /// All connected controllers appropriate for a setting may be targeted by
        /// using the special uniqueId value "all" or "*".
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller, or "all" or "*".</param>
        /// <param name="setting">Which controller setting to configure.</param>
        /// <param name="value">The value to configure, interpretation dependend on setting.</param>
        /// <returns>True if the controller is a valid target and the setting was configured successfully.</returns>
        internal static bool ConfigureControllerSetting(string uniqueId, SCControllerSetting setting, int value)
        {
            return SpatialController_ConfigureControllerSetting(uniqueId, setting, value);
        }

        /// <summary>
        /// Retrieves controller input info for a specific controller input.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the controller.</param>
        /// <param name="inputName">The name of the input for which to get the info.</param>
        /// <returns>The info for the specified input, or null if not available.</returns>
        internal static ControllerInputInfo? GetControllerInputInfoForInputName(string uniqueId, SCControllerInputName inputName)
        {
            ControllerInputInfoWriter result = new ControllerInputInfoWriter();
            GCHandle handle = GCHandle.Alloc(result);
            if (!SpatialController_GetControllerInputInfoForInputName(uniqueId, inputName, GetControllerInputInfoForInputNameCallback, GCHandle.ToIntPtr(handle)))
            {
                return null;
            }
            ControllerInputInfo? info = result.info;
            handle.Free();
            return info;
        }

        /// <summary>
        /// Retrieves a symbol/icon in PNG format for a specific controller input.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the controller.</param>
        /// <param name="inputName">The name of the input for which to get the symbol.</param>
        /// <param name="symbolScale">The desired scale of the symbol.</param>
        /// <param name="renderingMode">The desired rendering mode for the symbol.</param>
        /// <returns>The symbol for the specified input, or null if not available.</returns>
        internal static Symbol? GetSymbolForInputName(string uniqueId, SCControllerInputName inputName, SCUIImageSymbolScale symbolScale, SCUIImageRenderingMode renderingMode)
        {
            SymbolWriter result = new SymbolWriter();
            GCHandle handle = GCHandle.Alloc(result);
            if (!SpatialController_GetSymbolForInputName(uniqueId, inputName, symbolScale, renderingMode, GetSymbolForInputNameCallback, GCHandle.ToIntPtr(handle)))
            {
                return null;
            }
            Symbol? symbol = result.symbol;
            handle.Free();
            return symbol;
        }

        /// <summary>
        /// Polls the current state of a controller, including input, battery, and accessory information.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the controller to poll.</param>
        /// <returns>The current state of the controller, or null if the controller is not available.</returns>
        internal static ControllerState? PollController(string uniqueId)
        {
            ControllerStateWriter result = new ControllerStateWriter();
            GCHandle handle = GCHandle.Alloc(result);
            SpatialController_PollController(uniqueId, PollControllerCallback, GCHandle.ToIntPtr(handle));
            ControllerState? state = result.state;
            handle.Free();
            return state;
        }

        /// <summary>
        /// Gets the current system time as a managed TimeValue.
        /// </summary>
        /// <returns>Current time as a managed TimeValue instance.</returns>
        internal static TimeValue GetCurrentTime()
        {
            var sc = SpatialController_GetCurrentTime();
            return MakeTimeValue(sc);
        }

        /// <summary>
        /// Gets the predicted display time of the next frame for the current application window.
        /// </summary>
        /// <returns>Predicted time value or invalid (-1) if no prediction is available.</returns>
        internal static TimeValue GetPredictedNextFrameTime()
        {
            var sc = SpatialController_GetPredictedNextFrameTime();
            return MakeTimeValue(sc);
        }

        /// <summary>
        /// Predicts the position and state of an accessory anchor at a future time.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the accessory.</param>
        /// <param name="time">The future time for which to predict the anchor state.</param>
        /// <returns>The predicted accessory anchor state, or null if prediction is not available.</returns>
        internal static AccessoryAnchor? PredictAnchor(string uniqueId, TimeValue time)
        {
            AccessoryAnchorWriter result = new();
            GCHandle handle = GCHandle.Alloc(result);
            SCTimeValue scTime = MakeSCTimeValue(time);
            if (!SpatialController_PredictAnchor(uniqueId, scTime, PredictAnchorCallback, GCHandle.ToIntPtr(handle)))
            {
                return null;
            }
            AccessoryAnchor? accessoryAnchor = result.accessoryAnchor;
            handle.Free();
            return accessoryAnchor;
        }

        /// <summary>
        /// Sets the light color for a controller that supports lighting.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="r">Red component (0.0 to 1.0).</param>
        /// <param name="g">Green component (0.0 to 1.0).</param>
        /// <param name="b">Blue component (0.0 to 1.0).</param>
        /// <returns>True if the color was set successfully.</returns>
        internal static bool SetControllerLightColor(string uniqueId, float r, float g, float b)
        {
            return SpatialController_SetControllerLightColor(uniqueId, r, g, b);
        }

        /// <summary>
        /// Create or restart the haptics engine for locality on the specified controller.
        /// Controllers which support multiple localities (excluding Default) allow
        /// an independent engine per locality. See Controller.supportedLocalities.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="locality">Locality of haptics. Must be supported by controller.</param>
        /// <returns>True if the haptics engine was started for controller uniqueId.</returns>
        internal static bool CreateHapticsEngine(string uniqueId, SCHapticsLocality locality)
        {
            return SpatialController_CreateHapticsEngine(uniqueId, locality);
        }

        /// Stops the haptics engine on the specified controller and locality, if running.
        /// Controllers which support multiple localities (excluding Default) allow
        /// an independent engine per locality. See Controller.supportedLocalities.
        /// Engines with apparent locality overlap, such as All and Handles, can be
        /// independently controlled, and their outputs are composed, so stopping an
        /// engine with locality All does not also stop engines with narrower localities.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="locality">Locality of haptics. Must be supported by controller.</param>
        /// <param name="onCompleted">If return value is true, onCompleted will be called when shutdown completes.</param>
        /// <returns>True if a haptics engine was stopped, or false if not.</returns>
        internal static bool StopHapticsEngine(string uniqueId, SCHapticsLocality locality, CompletionCallback onCompleted)
        {
            // Register the onCompleted callback for a later callback by key.
            // Do this before calling SpatialController_StopHapticsEngine in case there's any chance
            // of the callback being called synchronously from within the call.
            stopHapticsCalls[stopHapticsCallNextContext] = new(uniqueId, locality, onCompleted);
            int key = stopHapticsCallNextContext++;

            bool success = SpatialController_StopHapticsEngine(uniqueId, locality, OnStopHapticsEngineCompleted, (IntPtr)key);
            if (!success)
            {
                // No callback with this key is expected if the return value is false.
                stopHapticsCalls.Remove(key);
            }
            return success;
        }

        /// <summary>
        /// Plays haptics data on the specified controller.
        /// If a haptics engine has not yet been created, attempts to create one first.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="data">Binary haptics data from a haptics data file in a supported format such as AHAP.</param>
        /// <param name="locality">Locality of haptics. Must be supported by controller.</param>
        /// <param name="onFinished">If return value is true, onFinished will be called when playback finishes.</param>
        /// <returns>True if haptics playback was started for controller uniqueId.</returns>
        internal static bool PlayHapticsData(string uniqueId, byte[] data, SCHapticsLocality locality, PlayHapticsFinishedCallback onFinished)
        {
            // Register the onFinished callback for a later callback by key.
            // Do this before calling SpatialController_PlayHapticsData in case there's any chance
            // of the callback being called synchronously from within the call.
            playHapticsCalls[playHapticsCallNextContext] = new(uniqueId, locality, onFinished);
            int key = playHapticsCallNextContext++;

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            bool success = SpatialController_PlayHapticsData(uniqueId, handle.AddrOfPinnedObject(), data.Length, locality, OnHapticEngineFinished, (IntPtr)key);
            if (!success)
            {
                // No callback with this key is expected if the return value is false.
                playHapticsCalls.Remove(key);
            }
            handle.Free();
            return success;
        }

    }
#nullable disable
}
