using UnityEngine;
using Unity.Mathematics;
using System;
using AOT;
using Apple.Core;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Public API for Apple visionOS Spatial Controller functionality.
/// Provides managed interfaces for interacting with spatial controllers and accessories.
/// </summary>
namespace Apple.visionOS.SpatialController
{
#nullable enable

    /// <summary>
    /// Enumeration of controller input names for buttons and directional pads.
    /// </summary>
    public enum ControllerInputName
    {
        /// <summary>Home button input.</summary>
        ButtonHome = 0,
        /// <summary>Menu button input.</summary>
        ButtonMenu = 1,
        /// <summary>Options button input.</summary>
        ButtonOptions = 2,
        /// <summary>Share button input.</summary>
        ButtonShare = 3,
        /// <summary>A or first button input.</summary>
        ButtonA = 4,
        /// <summary>B or second button input.</summary>
        ButtonB = 5,
        /// <summary>X or third button input.</summary>
        ButtonX = 6,
        /// <summary>Y or fourth button input.</summary>
        ButtonY = 7,
        /// <summary>Grip button input, if only one on controller.</summary>
        ButtonGrip = 8,
        /// <summary>Left shoulder button input.</summary>
        ButtonLeftShoulder = 10,
        /// <summary>Right shoulder button input.</summary>
        ButtonRightShoulder = 11,
        /// <summary>Left bumper button input.</summary>
        ButtonLeftBumper = 12,
        /// <summary>Right bumper button input.</summary>
        ButtonRightBumper = 13,
        /// <summary>Trigger button input, if only one on controller.</summary>
        ButtonTrigger = 16,
        /// <summary>Left trigger button input.</summary>
        ButtonLeftTrigger = 18,
        /// <summary>Right trigger button input.</summary>
        ButtonRightTrigger = 19,
        /// <summary>Thumbstick button input (clickable thumbstick), if only one on controller.</summary>
        ButtonThumbstick = 20,
        /// <summary>Left thumbstick button input (clickable thumbstick).</summary>
        ButtonLeftThumbstick = 22,
        /// <summary>Right thumbstick button input (clickable thumbstick).</summary>
        ButtonRightThumbstick = 23,
        /// <summary>Touchpad button input (clickable touchpad).</summary>
        ButtonTouchpad = 24,
        /// <summary>Stylus tip button input.</summary>
        ButtonStylusTip = 96,
        /// <summary>Primary stylus button input.</summary>
        ButtonStylusPrimary = 97,
        /// <summary>Secondary stylus button input.</summary>
        ButtonStylusSecondary = 98,
        /// <summary>Directional pad input.</summary>
        DPadDirectionPad = 192,
        /// <summary>Thumbstick directional input, if only one.</summary>
        DPadThumbstick = 193,
        /// <summary>Left thumbstick directional input.</summary>
        DPadLeftThumbstick = 194,
        /// <summary>Right thumbstick directional input.</summary>
        DPadRightThumbstick = 195,
        /// <summary>Touchpad first touch position, if any.</summary>
        DPadTouchpadPrimary = 196,
        /// <summary>Touchpad second touch position, if any.</summary>
        DPadTouchpadSecondary = 197,
    }

    /// <summary>
    /// Enumeration of configurable controller settings.
    /// </summary>
    public enum ControllerSetting
    {
        /// <summary>
        /// Enable motion sensors, if controller has manual sensor activation.
        /// value is interpretted as boolean (value != 0).
        /// </summary>
        EnableMotionSensors = 1,
    }

    /// <summary>
    /// Enumeration representing the handedness or chirality of an accessory.
    /// </summary>
    [Unavailable(RuntimeOperatingSystem.macOS, RuntimeOperatingSystem.iOS, RuntimeOperatingSystem.tvOS)]
    [Introduced(visionOS: "26.0.0")]
    public enum AccessoryChirality
    {
        /// <summary>Chirality is not specified or not applicable.</summary>
        Unspecified = 0,
        /// <summary>Left-handed accessory.</summary>
        Left = 1,
        /// <summary>Right-handed accessory.</summary>
        Right = 2,
    }

    public static class UIImage
    {
        /// <summary>
        /// Enumeration representing the requested scale of a symbol image.
        /// </summary>
        public enum SymbolScale
        {
            /// <summary>The default scale variant that matches the system usage.</summary>
            Default = -1,
            /// <summary>An unspecified scale.</summary>
            Unspecified = 0,
            /// <summary>The small variant of the symbol image.</summary>
            Small = 1,
            /// <summary>The medium variant of the symbol image.</summary>
            Medium = 2,
            /// <summary>The large variant of the symbol image.</summary>
            Large = 3,
        }

        /// <summary>
        /// Enumeration representing the rendering mode of a symbol image.
        /// </summary>
        public enum RenderingMode
        {
            /// <summary>Draw the image using the context’s default rendering mode.</summary>
            Automatic = 0,
            /// <summary>Always draw the original image, without treating it as a template.</summary>
            AlwaysOriginal = 1,
            /// <summary>Always draw the image as a template image, ignoring its color information.</summary>
            AlwaysTemplate = 2,
        }
    }

    public static class GCDeviceBattery
    {
        /// <summary>
        /// Enumeration representing the current charge state of a battery.
        /// </summary>
        public enum State
        {
            /// <summary>The battery state cannot be determined.</summary>
            Unknown = -1,
            /// <summary>The device is not connected to power and is running on battery.</summary>
            Discharging = 0,
            /// <summary>The device is plugged into power, but the battery is not yet fully charged.</summary>
            Charging = 1,
            /// <summary>The device is plugged into power and the battery is at 100%.</summary>
            Full = 2,
        }
    }

    /// <summary>
    /// Enumeration representing the current state of authorization for accessory tracking.
    /// </summary>
    [Unavailable(RuntimeOperatingSystem.macOS, RuntimeOperatingSystem.iOS, RuntimeOperatingSystem.tvOS)]
    [Introduced(visionOS: "26.0.0")]
    public enum AccessoryTrackingAuthorizationState
    {
        /// <summary>Accessory tracking is not supported by this OS version.</summary>
        NotSupported = -1,
        /// <summary>Accessory tracking authorization has not yet been requested.</summary>
        NotRequested = 0,
        /// <summary>Accessory tracking authorization has been requested but the response is pending.</summary>
        Pending = 1,
        /// <summary>Accessory tracking authorization was denied.</summary>
        NotAuthorized = 2,
        /// <summary>Accessory tracking authorization was granted.</summary>
        StateAuthorized = 3,
    }

    /// <summary>
    /// Enumeration representing the current state accessory tracking.
    /// </summary>
    [Unavailable(RuntimeOperatingSystem.macOS, RuntimeOperatingSystem.iOS, RuntimeOperatingSystem.tvOS)]
    [Introduced(visionOS: "26.0.0")]
    public enum AccessoryTrackingState
    {
        /// <summary>Accessory tracking was stopped due to an ARKit error.</summary>
        ARTrackingError = -1,
        /// <summary>Accessory tracking is not currently running.</summary>
        Stopped = 0,
        /// <summary>Accessory tracking is currently running.</summary>
        Running = 1,
    }

    public static class ARKitCoordinateSpace
    {
        /// <summary>
        /// Enumeration representing possible coordinate space corrections.
        /// </summary>
        public enum Correction
        {
            /// <summary>Coordinate spaces are unaltered and represent actual locations.</summary>
            None = 0,
            /// <summary>Coordinate spaces are corrected to render over physical objects in passthrough displays.</summary>
            Rendered = 1,
        }
    }

    /// <summary>
    /// Enumeration specifying the location of the actuators a haptic engine will target.
    /// </summary>
    public enum HapticsLocality
    {
        /// <summary>The default location of a haptics actuator on a game controller.  </summary>
        Default = 0,
        /// <summary>All locations of haptics actuators on a game controller.</summary>
        All = 1,
        /// <summary>All handles on a game controller.</summary>
        Handles = 2,
        /// <summary>The left handle on a game controller.</summary>
        LeftHandle = 3,
        /// <summary>The right handle on a game controller.</summary>
        RightHandle = 4,
        /// <summary>All triggers on a game controller.</summary>
        Triggers = 5,
        /// <summary>The left trigger on a game controller.</summary>
        LeftTrigger = 6,
        /// <summary>The right trigger on a game controller.</summary>
        RightTrigger = 7,
    }

    /// <summary>
    /// Enumeration of possible actions to take when Haptic playback finishes.
    /// </summary>
    public enum HapticEngineFinishedAction
    {
        /// <summary>Leave the haptic engine for the locality running.</summary>
        LeaveEngineRunning = 0,
        /// <summary>Stop the haptic engine for the locality.</summary>
        StopEngine = 1,
    }

    /// <summary>
    /// Represents haptics capabilities of a controller device.
    /// </summary>
    public struct ControllerMotionCapabilities
    {
        /// <summary>
        /// This controller supports sensing attitude relative to vertical facing the user.
        /// </summary>
        public readonly bool hasAttitude;
        /// <summary>
        /// This controller supports sensing the rotation rate around 3 axes.
        /// </summary>
        public readonly bool hasRotationRate;
        /// <summary>
        /// This controller supports sensing linear acceleration of gravity and user motion.
        /// </summary>
        public readonly bool hasGravityAndUserAcceleration;
        /// <summary>
        /// This controller's motion sensors must be manually activated.
        /// </summary>
        public readonly bool sensorsRequireManualActivation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerMotionCapabilities"/> class.
        /// </summary>
        /// <param name="hasAttitude">This controller supports sensing attitude relative to vertical facing the user.</param>
        /// <param name="hasRotationRate">This controller supports sensing the rotation rate around 3 axes.</param>
        /// <param name="hasGravityAndUserAcceleration">This controller supports sensing linear acceleration of gravity and user motion.</param>
        /// <param name="sensorsRequireManualActivation">This controller's motion sensors must be manually activated.</param>
        internal ControllerMotionCapabilities(bool hasAttitude, bool hasRotationRate, bool hasGravityAndUserAcceleration, bool sensorsRequireManualActivation)
        {
            this.hasAttitude = hasAttitude;
            this.hasRotationRate = hasRotationRate;
            this.hasGravityAndUserAcceleration = hasGravityAndUserAcceleration;
            this.sensorsRequireManualActivation = sensorsRequireManualActivation;
        }
    }

    /// <summary>
    /// Represents haptics capabilities of a controller device.
    /// </summary>
    public class DeviceHaptics
    {
        /// <summary>
        /// Set of localities supported by this controller device.
        /// </summary>
        public HashSet<HapticsLocality> supportedLocalities { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceHaptics"/> class.
        /// </summary>
        /// <param name="supportedLocalities">Set of localities supported by this controller device.</param>
        internal DeviceHaptics(HashSet<HapticsLocality> supportedLocalities)
        {
            this.supportedLocalities = supportedLocalities;
        }
    }

    /// <summary>
    /// Represents a spatial controller device with its properties and capabilities.
    /// </summary>
    public class Controller
    {
        /// <summary>
        /// Gets the unique identifier for this controller.
        /// </summary>
        public string uniqueId { get; private set; }

        /// <summary>
        /// Gets the product category of this controller.
        /// </summary>
        public string productCategory { get; private set; }

        /// <summary>
        /// Gets the vendor/manufacturer name of this controller.
        /// </summary>
        public string vendorName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this controller is physically attached to the device.
        /// </summary>
        public bool isAttachedToDevice { get; private set; }

        /// <summary>
        /// Controller motion sensing capabilities or Null
        /// </summary>
        public ControllerMotionCapabilities? motion { get; private set; }

        /// <summary>
        /// Controller haptic capabilities or Null
        /// </summary>
        public DeviceHaptics? haptics { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this controller has battery status reporting.
        /// </summary>
        public bool hasBattery { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this controller has motion sensing capabilities.
        /// </summary>
        public bool hasMotion  { get { return motion != null; } private set { } }

        /// <summary>
        /// Gets a value indicating whether this controller has controllable lights.
        /// </summary>
        public bool hasLight { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this controller supports haptic feedback.
        /// </summary>
        public bool hasHaptics { get { return haptics != null; } private set { } }

        /// <summary>
        /// Gets a value indicating whether this controller is a spatial controller that supports accessory tracking.
        /// </summary>
        public bool isSpatial { get { return productCategory.StartsWith("Spatial "); } private set { } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="uniqueId">The unique identifier for the controller.</param>
        /// <param name="productCategory">The product category of the controller.</param>
        /// <param name="vendorName">The vendor/manufacturer name.</param>
        /// <param name="isAttachedToDevice">Whether the controller is attached to the device.</param>
        /// <param name="hasBattery">Whether the controller has battery reporting.</param>
        /// <param name="motion">Motion sensing capabilities if the controller supports motion sensing.</param>
        /// <param name="hasLight">Whether the controller has controllable lights.</param>
        /// <param name="haptics">Haptics capabilities if the controller supports haptics.</param>
        internal Controller(string uniqueId, string productCategory, string vendorName, bool isAttachedToDevice, bool hasBattery, ControllerMotionCapabilities? motion, bool hasLight, DeviceHaptics? haptics)
        {
            this.uniqueId = uniqueId;
            this.productCategory = productCategory;
            this.vendorName = vendorName;
            this.isAttachedToDevice = isAttachedToDevice;
            this.hasBattery = hasBattery;
            this.motion = motion;
            this.hasLight = hasLight;
            this.haptics = haptics;
        }
    }

    /// <summary>
    /// Represents an accessory that can be attached to a controller.
    /// </summary>
    [Unavailable(RuntimeOperatingSystem.macOS, RuntimeOperatingSystem.iOS, RuntimeOperatingSystem.tvOS)]
    [Introduced(visionOS: "26.0.0")]
    public class Accessory
    {
        /// <summary>
        /// Represents a location name where an accessory can be attached.
        /// </summary>
        public struct LocationName
        {
            /// <summary>
            /// The raw string value of the location name.
            /// </summary>
            public readonly string rawValue;

            /// <summary>
            /// Initializes a new instance of a <see cref="LocationName"/>.
            /// </summary>
            /// <param name="rawValue">The location name raw string value.</param>
            internal LocationName(string rawValue)
            {
                this.rawValue = rawValue;
            }

            /// <summary>
            /// Aim point for spatial gamepads and styluses.
            /// </summary>
            public static readonly LocationName aim = new LocationName("aim");

            /// <summary>
            /// Grip for spatial gamepads.
            /// </summary>
            public static readonly LocationName grip = new LocationName("grip");

            /// <summary>
            /// Grip surface for spatial gamepads.
            /// </summary>
            public static readonly LocationName gripSurface = new LocationName("grip_surface");
        }

        /// <summary>
        /// Gets the unique identifier for this accessory.
        /// </summary>
        public string id { get; private set; }

        /// <summary>
        /// Gets the display name of this accessory.
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// Gets the path to the USDZ file for 3D representation of this accessory.
        /// </summary>
        public string usdzFile { get; private set; }

        /// <summary>
        /// Gets the human-readable description of this accessory.
        /// </summary>
        public string description { get; private set; }

        /// <summary>
        /// Gets the inherent chirality (handedness) of this accessory.
        /// </summary>
        public AccessoryChirality inherentChirality { get; private set; }

        /// <summary>
        /// Gets the array of possible attachment locations for this accessory.
        /// </summary>
        public LocationName[] locations { get; private set; }

        /// <summary>
        /// Gets the source controller for this accessory.
        /// </summary>
        public Controller source { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Accessory"/> class.
        /// </summary>
        /// <param name="id">The unique identifier for the accessory.</param>
        /// <param name="name">The display name of the accessory.</param>
        /// <param name="usdzFile">The path to the USDZ file.</param>
        /// <param name="description">The description of the accessory.</param>
        /// <param name="inherentChirality">The inherent chirality of the accessory.</param>
        /// <param name="locations">The possible attachment locations.</param>
        /// <param name="source">The source controller.</param>
        internal Accessory(string id, string name, string usdzFile, string description, AccessoryChirality inherentChirality, Accessory.LocationName[] locations, Controller source)
        {
            this.id = id;
            this.name = name;
            this.usdzFile = usdzFile;
            this.description = description;
            this.inherentChirality = inherentChirality;
            this.locations = locations;
            this.source = source;
        }
    }

    /// <summary>
    /// Represents a tracked anchor for an accessory with position, velocity, and tracking information.
    /// </summary>
    [Unavailable(RuntimeOperatingSystem.macOS, RuntimeOperatingSystem.iOS, RuntimeOperatingSystem.tvOS)]
    [Introduced(visionOS: "26.0.0")]
    public class AccessoryAnchor
    {
        /// <summary>
        /// Enumeration representing the tracking state of an accessory.
        /// </summary>
        public enum TrackingState
        {
            /// <summary>Accessory is not being tracked.</summary>
            Untracked = 0,
            /// <summary>Only orientation is being tracked.</summary>
            OrientationTracked = 1,
            /// <summary>Both position and orientation are being tracked with high accuracy.</summary>
            PositionOrientationTracked = 2,
            /// <summary>Both position and orientation are being tracked with low accuracy.</summary>
            PositionOrientationTrackedLowAccuracy = 3,
        }

        /// <summary>
        /// Gets the unique identifier for this accessory anchor.
        /// </summary>
        public string id { get; private set; }

        /// <summary>
        /// Gets the human-readable description of this accessory anchor.
        /// </summary>
        public string description { get; private set; }

        /// <summary>
        /// Gets the transform from origin to anchor coordinate system as a Unity Pose.
        /// In Windowed mode, this transform remains relative to ARKit world space,
        /// not the origin of the application scene volume. Call coordinateSpace()
        /// instead to access poses which are always converted to the same coordinate
        /// space as the applicaiton root (world or windowed volume scene).
        /// </summary>
        public Pose originFromAnchorTransform { get; private set; }

        /// <summary>
        /// Gets the linear velocity of the anchor in 3D space.
        /// </summary>
        public float3 velocity { get; private set; }

        /// <summary>
        /// Gets the angular velocity of the anchor in 3D space.
        /// </summary>
        public float3 angularVelocity { get; private set; }

        /// <summary>
        /// Gets the timestamp when this anchor data was captured.
        /// </summary>
        public TimeValue timestamp { get; private set; }

        /// <summary>
        /// Gets the current tracking state of the accessory.
        /// </summary>
        public TrackingState trackingState { get; private set; }

        /// <summary>
        /// Gets the chirality (handedness) when the accessory is held, or null if not held.
        /// </summary>
        public AccessoryChirality? heldChirality { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the accessory is currently being tracked.
        /// </summary>
        public bool isTracked { get; private set; }

        /// <summary>
        /// Gets the associated accessory, if any.
        /// </summary>
        public Accessory? accessory { get; private set; }

        private Pose[] locationPoses;

        /// <summary>
        /// Gets the transform from application root to anchor coordinate
        /// system as a Unity Pose.
        /// </summary>
        /// <param name="correction">Whether the pose is corrected to render over physical objects in passthrough displays.</param>
        /// <returns>The pose of the anchor origin in application root space.</returns>
        public Pose? coordinateSpace(ARKitCoordinateSpace.Correction correction)
        {
            var locationCount = accessory?.locations.Length ?? 0;
            var poseIndex = (int)correction * (locationCount + 1) + locationCount;
            if (poseIndex > locationPoses.Length)
            {
                return null;
            }
            return locationPoses[poseIndex];
        }

        /// <summary>
        /// Gets the transform from application root to a specified accessory
        /// location in the anchor coordinate system as a Unity Pose.
        /// </summary>
        /// <param name="location">Which accessory location of anchor.accessory.locations[].</param>
        /// <param name="correction">Whether the pose is corrected to render over physical objects in passthrough displays.</param>
        /// <returns>The pose of the accessory location in application root space.</returns>
        public Pose? coordinateSpace(Accessory.LocationName location, ARKitCoordinateSpace.Correction correction)
        {
            if (accessory == null)
            {
                return null;
            }
            var locationCount = accessory?.locations.Length ?? 0;
            var locationIndex = Array.IndexOf(accessory?.locations, location);
            if (locationIndex == -1)
            {
                return null;
            }
            var poseIndex = (int)correction * (locationCount + 1) + locationIndex;
            if (poseIndex > locationPoses.Length)
            {
                return null;
            }
            return locationPoses[poseIndex];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessoryAnchor"/> class.
        /// </summary>
        /// <param name="id">The unique identifier for the anchor.</param>
        /// <param name="description">The description of the anchor.</param>
        /// <param name="originFromAnchorTransform">The transform from origin to anchor.</param>
        /// <param name="velocity">The linear velocity of the anchor.</param>
        /// <param name="angularVelocity">The angular velocity of the anchor.</param>
        /// <param name="timestamp">The timestamp of the anchor data.</param>
        /// <param name="trackingState">The tracking state of the accessory.</param>
        /// <param name="heldChirality">The chirality when held, or null.</param>
        /// <param name="isTracked">Whether the accessory is tracked.</param>
        /// <param name="accessory">The associated accessory.</param>
        /// <param name="locationPoses">The table of location pose values for this anchor.</param>
        internal AccessoryAnchor(string id, string description, Pose originFromAnchorTransform, float3 velocity, float3 angularVelocity, TimeValue timestamp, TrackingState trackingState, AccessoryChirality? heldChirality, bool isTracked, Accessory? accessory, Pose[] locationPoses)
        {
            this.id = id;
            this.description = description;
            this.originFromAnchorTransform = originFromAnchorTransform;
            this.velocity = velocity;
            this.angularVelocity = angularVelocity;
            this.timestamp = timestamp;
            this.trackingState = trackingState;
            this.heldChirality = heldChirality;
            this.isTracked = isTracked;
            this.accessory = accessory;
            this.locationPoses = locationPoses;
        }
    }

    /// <summary>
    /// Represents the state of a button input on a controller.
    /// </summary>
    public struct ButtonState
    {
        /// <summary>
        /// The current value of the button in the range 0.0 (not pressed) to 1.0 (fully pressed).
        /// </summary>
        public readonly float value;
        /// <summary>
        /// True if this button produces analog output values between 0.0 and 1.0.
        /// False if it is digital with only value 1.0 if pressed or 0.0 if not.
        /// </summary>
        public readonly bool isAnalog;
        /// <summary>
        /// True if this button is currently pressed (value > 0.0).
        /// </summary>
        public readonly bool isPressed;
        /// <summary>
        /// True if this button is currently touched, if the button supports touch.
        /// </summary>
        public readonly bool isTouched;
        /// <summary>
        /// The time at which this button last changed isPressed state.
        /// Subtract this from GetCurrentTime() to determine the latency.
        /// </summary>
        public readonly TimeValue lastPressedStateTimestamp;
        /// <summary>
        /// The time at which this button last changed value.
        /// Subtract this from GetCurrentTime() to determine the latency.
        /// </summary>
        public readonly TimeValue lastValueTimestamp;

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonState"/> struct.
        /// </summary>
        /// <param name="value">The button value.</param>
        internal ButtonState(float value, bool isAnalog, bool isPressed, bool isTouched, TimeValue lastPressedStateTimestamp, TimeValue lastValueTimestamp)
        {
            this.value = value;
            this.isAnalog = isAnalog;
            this.isPressed = isPressed;
            this.isTouched = isTouched;
            this.lastPressedStateTimestamp = lastPressedStateTimestamp;
            this.lastValueTimestamp = lastValueTimestamp;
        }
    }

    /// <summary>
    /// Represents the state of a directional pad input on a controller.
    /// </summary>
    public struct DPadState
    {
        /// <summary>
        /// The X-axis value of the directional pad, typically ranging from -1.0 to 1.0.
        /// </summary>
        public readonly float xAxis;

        /// <summary>
        /// The Y-axis value of the directional pad, typically ranging from -1.0 to 1.0.
        /// </summary>
        public readonly float yAxis;

        /// <summary>
        /// Initializes a new instance of the <see cref="DPadState"/> struct.
        /// </summary>
        /// <param name="xAxis">The X-axis value.</param>
        /// <param name="yAxis">The Y-axis value.</param>
        internal DPadState(float xAxis, float yAxis)
        {
            this.xAxis = xAxis;
            this.yAxis = yAxis;
        }
    }

    /// <summary>
    /// Represents the complete input state of a controller, including all buttons and directional pads.
    /// </summary>
    public class ControllerInputState
    {
        /// <summary>
        /// Gets a dictionary mapping controller input names to their current button states.
        /// </summary>
        public Dictionary<ControllerInputName, ButtonState> buttons { get; private set; }

        /// <summary>
        /// Gets a dictionary mapping controller input names to their current directional pad states.
        /// </summary>
        public Dictionary<ControllerInputName, DPadState> dpads { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerInputState"/> class.
        /// </summary>
        /// <param name="buttons">The dictionary of button states.</param>
        /// <param name="dpads">The dictionary of directional pad states.</param>
        internal ControllerInputState(Dictionary<ControllerInputName, ButtonState> buttons, Dictionary<ControllerInputName, DPadState> dpads)
        {
            this.buttons = buttons;
            this.dpads = dpads;
        }
    }

    /// <summary>
    /// Represents the battery state of a controller.
    /// </summary>
    public struct ControllerBatteryState
    {
        /// <summary>
        /// The battery level as a percentage, ranging from 0.0 (empty) to 1.0 (full).
        /// </summary>
        public readonly float level;

        /// <summary>
        /// The battery charging state as an integer value.
        /// TODO: Should be converted to an enum for better type safety.
        /// </summary>
        public readonly GCDeviceBattery.State state;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerBatteryState"/> struct.
        /// </summary>
        /// <param name="level">The battery level percentage.</param>
        /// <param name="state">The battery charging state.</param>
        internal ControllerBatteryState(float level, GCDeviceBattery.State state)
        {
            this.level = level;
            this.state = state;
        }
    };

    /// <summary>
    /// Represents the motion detection state of a controller.
    /// </summary>
    public struct ControllerMotionState
    {
        /// <summary>
        /// The controller attitude relative to vertical facing the user.
        /// </summary>
        public readonly quaternion attitude;

        /// <summary>
        /// The controller rotation rate around 3 axes.
        /// </summary>
        public readonly float3 rotationRate;

        /// <summary>
        /// The controller combined linear acceleration of gravity and user motion.
        /// </summary>
        public readonly float3 acceleration;

        /// <summary>
        /// The controller linear acceleration of gravity.
        /// </summary>
        public readonly float3 gravity;

        /// <summary>
        /// The controller linear acceleration due to user motion.
        /// </summary>
        public readonly float3 userAcceleration;

        /// <summary>
        /// Indicates whether this controller's motion sensors are currently active.
        /// </summary>
        public readonly bool sensorsActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerMotionState"/> struct.
        /// </summary>
        /// <param name="attitude">The controller attitude relative to vertical facing the user.</param>
        /// <param name="rotationRate">The controller rotation rate around 3 axes.</param>
        /// <param name="acceleration">The controller combined linear acceleration of gravity and user motion.</param>
        /// <param name="gravity">The controller linear acceleration of gravity.</param>
        /// <param name="userAcceleration">The controller linear acceleration due to user motion.</param>
        /// <param name="sensorsActive">Indicates whether this controller's motion sensors are currently active.</param>
        internal ControllerMotionState(quaternion attitude, float3 rotationRate, float3 acceleration, float3 gravity, float3 userAcceleration, bool sensorsActive)
        {
            this.attitude = attitude;
            this.rotationRate = rotationRate;
            this.acceleration = acceleration;
            this.gravity = gravity;
            this.userAcceleration = userAcceleration;
            this.sensorsActive = sensorsActive;
        }
    }

    /// <summary>
    /// Represents the light state of a controller.
    /// </summary>
    public struct ControllerLightState
    {
        /// <summary>The current color of the controler light, or black if no light.</summary>
        public readonly Color color;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerLightState"/> struct.
        /// </summary>
        /// <param name="color">The light color state..</param>
        internal ControllerLightState(Color color)
        {
            this.color = color;
        }
    }

    /// <summary>
    /// Represents the complete state of a controller, including input, battery, and accessory information.
    /// </summary>
    public class ControllerState
    {
        /// <summary>
        /// Gets the current input state of the controller.
        /// </summary>
        public ControllerInputState input { get; private set; }

        /// <summary>
        /// Gets the current battery state of the controller.
        /// </summary>
        public ControllerBatteryState battery { get; private set; }

        /// <summary>
        /// Gets the current battery state of the controller.
        /// </summary>
        public ControllerMotionState motion { get; private set; }

        /// <summary>
        /// Gets the current light state of the controller.
        /// </summary>
        public ControllerLightState light { get; private set; }

        /// <summary>
        /// Gets the array of accessory anchors associated with this controller.
        /// </summary>
        public AccessoryAnchor[] accessoryAnchors { get; private set; }

        /// <summary>
        /// Gets the accessory associated with this controller, if any.
        /// </summary>
        public Accessory? accessory { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerState"/> class.
        /// </summary>
        /// <param name="input">The input state of the controller.</param>
        /// <param name="battery">The battery state of the controller.</param>
        /// <param name="motion">The motion sensor state of the controller.</param>
        /// <param name="light">The light state of the controller.</param>
        /// <param name="accessoryAnchors">The array of accessory anchors.</param>
        /// <param name="accessory">The associated accessory.</param>
        internal ControllerState(ControllerInputState input, ControllerBatteryState battery, ControllerMotionState motion, ControllerLightState light, AccessoryAnchor[] accessoryAnchors, Accessory? accessory)
        {
            this.input = input;
            this.battery = battery;
            this.motion = motion;
            this.light = light;
            this.accessoryAnchors = accessoryAnchors;
            this.accessory = accessory;
        }
    };

    /// <summary>
    /// Represents information about a controller input element.
    /// </summary>
    public struct ControllerInputInfo
    {
        /// <summary>
        /// Localized name of the controller input element.
        /// </summary>
        public readonly string localizedName;
        /// <summary>
        /// SF Symbols name for the symbol associated with the controller input element.
        /// </summary>
        public readonly string symbolName;

        internal ControllerInputInfo(string localizedName, string symbolName)
        {
            this.localizedName = localizedName;
            this.symbolName = symbolName;
        }
    }

    /// <summary>
    /// Represents a symbol or icon with image data in PNG format and dimensions.
    /// </summary>
    public class Symbol
    {
        /// <summary>
        /// Gets the width of the symbol in pixels.
        /// </summary>
        public int width { get; private set; }

        /// <summary>
        /// Gets the height of the symbol in pixels.
        /// </summary>
        public int height { get; private set; }

        /// <summary>
        /// Gets theimage data in PNG format of the symbol as a byte array.
        /// </summary>
        public byte[] data { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Symbol"/> class.
        /// </summary>
        /// <param name="width">The width of the symbol in pixels.</param>
        /// <param name="height">The height of the symbol in pixels.</param>
        /// <param name="data">The raw image data.</param>
        internal Symbol(int width, int height, byte[] data)
        {
            this.width = width;
            this.height = height;
            this.data = data;
        }
    };

    /// <summary>
    /// Represents a time value for spatial controller operations with arithmetic operators.
    /// </summary>
    public struct TimeValue
    {
        /// <summary>
        /// The time value as a double precision floating point number.
        /// </summary>
        public readonly double time;

        /// <summary>
        /// Checks if this is a valid time value or a value that indicates no valid time.
        /// </summary>
        /// <returns>True if this time value is valid, false if it is not a valid time value.</returns>.
        public bool isValid()
        {
            return time >= 0.0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeValue"/> struct.
        /// </summary>
        /// <param name="rawValue">The raw time value.</param>
        internal TimeValue(double rawValue)
        {
            time = rawValue;
        }

        /// <summary>
        /// Adds a time interval in seconds to a TimeValue.
        /// </summary>
        /// <param name="time">The base time value.</param>
        /// <param name="timeIntervalSeconds">The time interval to add in seconds.</param>
        /// <returns>A new TimeValue with the added interval.</returns>
        public static TimeValue operator +(TimeValue time, double timeIntervalSeconds)
        {
            return new TimeValue(time.time + timeIntervalSeconds);
        }

        /// <summary>
        /// Subtracts a time interval in seconds from a TimeValue.
        /// </summary>
        /// <param name="time">The base time value.</param>
        /// <param name="timeIntervalSeconds">The time interval to subtract in seconds.</param>
        /// <returns>A new TimeValue with the subtracted interval.</returns>
        public static TimeValue operator -(TimeValue time, double timeIntervalSeconds)
        {
            return new TimeValue(time.time - timeIntervalSeconds);
        }

        /// <summary>
        /// Calculates the time difference between two TimeValue instances.
        /// </summary>
        /// <param name="time">The later time value.</param>
        /// <param name="timeBase">The earlier time value.</param>
        /// <returns>The time difference in seconds.</returns>
        public static double operator -(TimeValue time, TimeValue timeBase)
        {
            return time.time - timeBase.time;
        }

        /// <summary>
        /// Compares two TimeValue instances.
        /// </summary>
        /// <param name="timeA">The first time value.</param>
        /// <param name="timeB">The second time value.</param>
        /// <returns>True if timeA is greater than timeB.</returns>
        public static bool operator >(TimeValue timeA, TimeValue timeB)
        {
            return timeA.time > timeB.time;
        }

        /// <summary>
        /// Compares two TimeValue instances.
        /// </summary>
        /// <param name="timeA">The first time value.</param>
        /// <param name="timeB">The second time value.</param>
        /// <returns>True if timeA is less than timeB.</returns>
        public static bool operator <(TimeValue timeA, TimeValue timeB)
        {
            return timeA.time < timeB.time;
        }

        /// <summary>
        /// Compares two TimeValue instances.
        /// </summary>
        /// <param name="timeA">The first time value.</param>
        /// <param name="timeB">The second time value.</param>
        /// <returns>True if timeA is greater than or equal to timeB.</returns>
        public static bool operator >=(TimeValue timeA, TimeValue timeB)
        {
            return timeA.time >= timeB.time;
        }

        /// <summary>
        /// Compares two TimeValue instances.
        /// </summary>
        /// <param name="timeA">The first time value.</param>
        /// <param name="timeB">The second time value.</param>
        /// <returns>True if timeA is less than or equal to timeB.</returns>
        public static bool operator <=(TimeValue timeA, TimeValue timeB)
        {
            return timeA.time <= timeB.time;
        }
    };

    /// <summary>
    /// Represents an error with code and localized description.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Numeric error code.
        /// </summary>
        public int code { get; private set; }

        /// <summary>
        /// Localized error description.
        /// </summary>
        public string localizedDescription { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="code">Numeric error code.</param>
        /// <param name="localizedDescription">Localized error description.</param>
        internal Error(int code, string localizedDescription)
        {
            this.code = code;
            this.localizedDescription = localizedDescription;
        }
    }

    /// <summary>
    /// Callback delegate for controller connection and disconnection events.
    /// </summary>
    /// <param name="controller">The controller that was connected or disconnected.</param>
    /// <param name="context">User-defined context object.</param>
    public delegate void ControllerConnectionCallback<T>(Controller controller, T context);

    /// <summary>
    /// Callback delegate for accessory connection and disconnection events.
    /// </summary>
    /// <param name="accessory">The accessory that was connected or disconnected.</param>
    /// <param name="context">User-defined context object.</param>
    [Unavailable(RuntimeOperatingSystem.macOS, RuntimeOperatingSystem.iOS, RuntimeOperatingSystem.tvOS)]
    [Introduced(visionOS: "26.0.0")]
    public delegate void AccessoryConnectionCallback<T>(Accessory accessory, T context);

    /// <summary>
    /// Callback delegate invoked when haptics playback finishes.
    /// </summary>
    /// <param name="error">If not null, an error caused playback to finish.</param>
    /// <returns>An action specifying whether to stop or leave the engine running.</returns>
    public delegate HapticEngineFinishedAction PlayHapticsFinishedCallback(Error? error);

    /// <summary>
    /// Callback delegate invoked when an asynchronous request completes.
    /// </summary>
    /// <param name="error">If not null, an error prevented succeessful completion.</param>
    public delegate void CompletionCallback(Error? error);

    /// <summary>
    /// Main plugin class providing the public API for spatial controller functionality.
    /// </summary>
    [Unavailable(RuntimeOperatingSystem.macOS, RuntimeOperatingSystem.iOS, RuntimeOperatingSystem.tvOS)]
    [Introduced(visionOS: "26.0.0")]
    public static class AccessoryTracking
    {
        private static Dictionary<object, Internal.ControllerConnectionCallbackWrapper> controllerConnectionCallbackWrapperFromCallback = new();
        private static Dictionary<object, Internal.AccessoryConnectionCallbackWrapper> accessoryConnectionCallbackWrapperFromCallback = new();

        /// <summary>
        /// Initializes the spatial controller system.
        /// Must be called before using any other spatial controller functionality.
        /// </summary>
        public static void Init()
        {
            controllerConnectionCallbackWrapperFromCallback = new();
            accessoryConnectionCallbackWrapperFromCallback = new();
            Internal.Plugin.Init();
        }

        /// <summary>
        /// Destroys and cleans up the spatial controller system.
        /// Should be called when spatial controller functionality is no longer needed.
        /// </summary>
        public static void Destroy()
        {
            Internal.Plugin.Destroy();
            Internal.Plugin.RemoveAllEventHandlers();
            controllerConnectionCallbackWrapperFromCallback = new();
            accessoryConnectionCallbackWrapperFromCallback = new();
        }

        /// <summary>
        /// Returns the current authorization state for AR accessory tracking.
        /// </summary>
        public static AccessoryTrackingAuthorizationState GetAccessoryTrackingAuthorizationState()
        {
            return Internal.Plugin.GetAccessoryTrackingAuthorizationState();
        }

        /// <summary>
        /// Returns the current running or error state for AR accessory tracking.
        /// </summary>
        public static AccessoryTrackingState GetAccessoryTrackingState()
        {
            return Internal.Plugin.GetAccessoryTrackingState();
        }

        /// <summary>
        /// Adds callback handlers for controller connection and disconnection events.
        /// </summary>
        /// <param name="onConnected">Callback invoked when a controller is connected.</param>
        /// <param name="onDisconnected">Callback invoked when a controller is disconnected.</param>
        /// <param name="context">User-defined context object passed to callbacks.</param>
        /// <returns>True if the handlers were set successfully; otherwise, false.</returns>
        public static bool AddControllerConnectionHandlers<T>(ControllerConnectionCallback<T> onConnected, ControllerConnectionCallback<T> onDisconnected, T context)
        {
            Internal.ControllerConnectionCallbackWrapper onConnectedWrapper = (controller) =>
            {
                onConnected(controller, context);
            };
            Internal.ControllerConnectionCallbackWrapper onDisconnectedWrapper = (controller) =>
            {
                onDisconnected(controller, context);
            };
            if (!Internal.Plugin.AddControllerConnectionHandlers(onConnectedWrapper, onDisconnectedWrapper))
            {
                return false;
            }
            controllerConnectionCallbackWrapperFromCallback.Add(onConnected, onConnectedWrapper);
            controllerConnectionCallbackWrapperFromCallback.Add(onDisconnected, onDisconnectedWrapper);
            return true;
        }

        /// <summary>
        /// Removes callback handlers for controller connection and disconnection events.
        /// </summary>
        /// <param name="onConnected">Callback invoked when a controller is connected.</param>
        /// <param name="onDisconnected">Callback invoked when a controller is disconnected.</param>
        /// <param name="context">User-defined context object passed to callbacks.</param>
        public static void RemoveControllerConnectionHandlers<T>(ControllerConnectionCallback<T> onConnected, ControllerConnectionCallback<T> onDisconnected, T context)
        {
            Internal.ControllerConnectionCallbackWrapper? onConnectedWrapper = null;
            Internal.ControllerConnectionCallbackWrapper? onDisconnectedWrapper = null;
            if (controllerConnectionCallbackWrapperFromCallback.TryGetValue(onConnected, out onConnectedWrapper))
            {
                controllerConnectionCallbackWrapperFromCallback.Remove(onConnected);
            }
            if (controllerConnectionCallbackWrapperFromCallback.TryGetValue(onDisconnected, out onDisconnectedWrapper))
            {
                controllerConnectionCallbackWrapperFromCallback.Remove(onDisconnected);
            }
            Internal.Plugin.RemoveControllerConnectionHandlers(onConnectedWrapper, onDisconnectedWrapper);
        }

        /// <summary>
        /// Sets callback handlers for accessory connection and disconnection events.
        /// </summary>
        /// <param name="onConnected">Callback invoked when an accessory is connected.</param>
        /// <param name="onDisconnected">Callback invoked when an accessory is disconnected.</param>
        /// <param name="context">User-defined context object passed to callbacks.</param>
        /// <returns>True if the handlers were set successfully; otherwise, false.</returns>
        public static bool AddAccessoryConnectionHandlers<T>(AccessoryConnectionCallback<T> onConnected, AccessoryConnectionCallback<T> onDisconnected, T context)
        {
            Internal.AccessoryConnectionCallbackWrapper onConnectedWrapper = (accessory) =>
            {
                onConnected(accessory, context);
            };
            Internal.AccessoryConnectionCallbackWrapper onDisconnectedWrapper = (accessory) =>
            {
                onDisconnected(accessory, context);
            };
            if (!Internal.Plugin.AddAccessoryConnectionHandlers(onConnectedWrapper, onDisconnectedWrapper))
            {
                return false;
            }
            accessoryConnectionCallbackWrapperFromCallback.Add(onConnected, onConnectedWrapper);
            accessoryConnectionCallbackWrapperFromCallback.Add(onDisconnected, onDisconnectedWrapper);
            return true;
        }

        /// <summary>
        /// Removes callback handlers for accessory connection and disconnection events.
        /// </summary>
        /// <param name="onConnected">Callback invoked when an accessory is connected.</param>
        /// <param name="onDisconnected">Callback invoked when an accessory is disconnected.</param>
        /// <param name="context">User-defined context object passed to callbacks.</param>
        public static void RemoveAccessoryConnectionHandlers<T>(AccessoryConnectionCallback<T> onConnected, AccessoryConnectionCallback<T> onDisconnected, T context)
        {
            Internal.AccessoryConnectionCallbackWrapper? onConnectedWrapper = null;
            Internal.AccessoryConnectionCallbackWrapper? onDisconnectedWrapper = null;
            if (accessoryConnectionCallbackWrapperFromCallback.TryGetValue(onConnected, out onConnectedWrapper))
            {
                accessoryConnectionCallbackWrapperFromCallback.Remove(onConnected);
            }
            if (accessoryConnectionCallbackWrapperFromCallback.TryGetValue(onDisconnected, out onDisconnectedWrapper))
            {
                accessoryConnectionCallbackWrapperFromCallback.Remove(onDisconnected);
            }
            Internal.Plugin.RemoveAccessoryConnectionHandlers(onConnectedWrapper, onDisconnectedWrapper);
        }

        /// <summary>
        /// Retrieves all currently connected controllers.
        /// </summary>
        /// <returns>An array of connected controllers, or an empty array if none are connected.</returns>
        public static Controller[] GetConnectedControllers()
        {
            return Internal.Plugin.GetConnectedControllers();
        }

        /// <summary>
        /// Retrieves a specific connected controller by its unique identifier.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the controller to retrieve.</param>
        /// <returns>The controller with the specified ID, or null if not found.</returns>
        public static Controller? GetConnectedController(string uniqueId)
        {
            return Internal.Plugin.GetConnectedController(uniqueId);
        }

        /// <summary>
        /// Retrieves all currently connected accessories.
        /// </summary>
        /// <returns>An array of connected accessories, or an empty array if none are connected.</returns>
        public static Accessory[] GetConnectedAccessories()
        {
            return Internal.Plugin.GetConnectedAccessories();
        }

        /// <summary>
        /// Retrieves a specific connected accessory by its unique identifier.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the accessory to retrieve.</param>
        /// <returns>The accessory with the specified ID, or null if not found.</returns>
        public static Accessory? GetConnectedAccessory(string uniqueId)
        {
            return Internal.Plugin.GetConnectedAccessory(uniqueId);
        }

        /// <summary>
        /// Sets a default configuration setting applied to controllers on connection.
        /// </summary>
        /// <param name="setting">Which controller setting to configure.</param>
        /// <param name="value">The value to configure, interpretation dependend on setting.</param>
        /// <returns>True if the setting was configured successfully.</returns>
        public static bool ConfigureDefaultControllerSetting(ControllerSetting setting, int value)
        {
            return Internal.Plugin.ConfigureDefaultControllerSetting(setting, value);
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
        internal static bool ConfigureControllerSetting(string uniqueId, ControllerSetting setting, int value)
        {
            return Internal.Plugin.ConfigureControllerSetting(uniqueId, setting, value);
        }

        /// <summary>
        /// Retrieves controller input info for a specific controller input.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the controller.</param>
        /// <param name="inputName">The name of the input for which to get the info.</param>
        /// <returns>The info for the specified input, or null if not available.</returns>
        public static ControllerInputInfo? GetControllerInputInfoForInputName(string uniqueId, ControllerInputName inputName)
        {
            return Internal.Plugin.GetControllerInputInfoForInputName(uniqueId, inputName);
        }

        /// <summary>
        /// Retrieves a symbol/icon in PNG format for a specific controller input.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the controller.</param>
        /// <param name="inputName">The name of the input for which to get the symbol.</param>
        /// <param name="symbolScale">The desired scale of the symbol.</param>
        /// <param name="renderingMode">The desired rendering mode for the symbol.</param>
        /// <returns>The symbol for the specified input, or null if not available.</returns>
        public static Symbol? GetSymbolForInputName(string uniqueId, ControllerInputName inputName, UIImage.SymbolScale symbolScale, UIImage.RenderingMode renderingMode)
        {
            return Internal.Plugin.GetSymbolForInputName(uniqueId, inputName, symbolScale, renderingMode);
        }

        /// <summary>
        /// Polls the current state of a controller, including input, battery, and accessory information.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the controller to poll.</param>
        /// <returns>The current state of the controller, or null if the controller is not available.</returns>
        public static ControllerState? PollController(string uniqueId)
        {
            return Internal.Plugin.PollController(uniqueId);
        }

        /// <summary>
        /// Gets the current system time as a TimeValue.
        /// </summary>
        /// <returns>The current system time.</returns>
        public static TimeValue GetCurrentTime()
        {
            return Internal.Plugin.GetCurrentTime();
        }

        /// <summary>
        /// Gets the predicted display time of the next frame for the current application window.
        /// </summary>
        /// <returns>Predicted time value or invalid (-1) if no prediction is available.</returns>
        public static TimeValue GetPredictedNextFrameTime()
        {
            return Internal.Plugin.GetPredictedNextFrameTime();
        }

        /// <summary>
        /// Predicts the position and state of an accessory anchor at a future time.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the accessory.</param>
        /// <param name="time">The future time for which to predict the anchor state.</param>
        /// <returns>The predicted accessory anchor state, or null if prediction is not available.</returns>
        public static AccessoryAnchor? PredictAnchor(string uniqueId, TimeValue time)
        {
            return Internal.Plugin.PredictAnchor(uniqueId, time);
        }

        /// <summary>
        /// Sets the light color for a controller that supports lighting.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="r">Red component (0.0 to 1.0).</param>
        /// <param name="g">Green component (0.0 to 1.0).</param>
        /// <param name="b">Blue component (0.0 to 1.0).</param>
        /// <returns>True if the color was set successfully.</returns>
        public static bool SetControllerLightColor(string uniqueId, float r, float g, float b)
        {
            return Internal.Plugin.SetControllerLightColor(uniqueId, r, g, b);
        }

        /// <summary>
        /// Create or restart the haptics engine for locality on the specified controller.
        /// Controllers which support multiple localities (excluding Default) allow
        /// an independent engine per locality. See Controller.supportedLocalities.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the controller.</param>
        /// <param name="locality">Locality of haptics. Must be supported by controller.</param>
        /// <returns>True if the haptics engine was started for controller uniqueId.</returns>
        public static bool CreateHapticsEngine(string uniqueId, HapticsLocality locality)
        {
            return Internal.Plugin.CreateHapticsEngine(uniqueId, locality);
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
        public static bool StopHapticsEngine(string uniqueId, HapticsLocality locality, CompletionCallback onCompleted)
        {
            return Internal.Plugin.StopHapticsEngine(uniqueId, locality, onCompleted);
        }

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
        /// <param name="locality">Locality of haptics. Must be supported by controller.</param>
        /// <param name="onFinished">If return value is true, onFinished will be called when playback finishes.</param>
        /// <returns>True if haptics playback was started for controller uniqueId.</returns>
        public static bool PlayHapticsData(string uniqueId, byte[] data, HapticsLocality locality, PlayHapticsFinishedCallback onFinished)
        {
            return Internal.Plugin.PlayHapticsData(uniqueId, data, locality, onFinished);
        }
    }
 
#nullable disable
}
