using Unity.VisualScripting;
using UnityEngine;
//include the namespace so we can access accessory tracking
using Apple.visionOS.AccessoryTracking;
using Apple.visionOS.SpatialController;
using UnityEngine.Events;
using UnityEditor;
using System.Text;

public class ExampleUsage : MonoBehaviour
{

    public AccessoryTrackingHelper ACTrackingHelper;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ACTrackingHelper == null)
        {
            Debug.LogAssertion("ACTracking Was Not Assigned", ACTrackingHelper);
        }

        ACTrackingHelper.ControllerConnectedEvent = new UnityEvent<string>();
        ACTrackingHelper.ControllerConnectedEvent.AddListener(ControllerConnected);

        ACTrackingHelper.ControllerConnectedEvent = new UnityEvent<string>();
        ACTrackingHelper.ControllerDisconnectedEvent.AddListener(ControllerDisconnected);

    }

    // Update is called once per frame
    void Update()
    {
        ///
        /// Get all the accessories that are connected to this device
        ///
        ACTrackingHelper.accessories = AccessoryTracking.GetConnectedAccessories();

        ///
        /// Loop through all the currently connected accessories.
        /// Accessories are spatial controllers with spatial tracking.
        ///
        /// Note that you can also GetConnectedControllers() to see
        /// currently connecnted controllers, which may also include
        /// non-spatial controllers. These can be polled for input
        /// state (buttons, battery, etc.) but not for anchor transforms.
        ///
        foreach (var accessory in ACTrackingHelper.accessories)
        {
            // Get the uid of the accessory source controller.
            // This is used to refer to the controller in other calls
            // that request state.
            var uid = accessory.source.uniqueId;

            // What is the handedness of the controller?
            var hand = accessory.inherentChirality;

            // Poll the state of the accessory controller.
            // This is the mort important step as it will give us updated
            // information on the transforms and input from the accessory.
            var state = AccessoryTracking.PollController(uid);



            //make sure the device see the anchors. If it doesn't. It will return 0 (zero)
            if (state.accessoryAnchors.Length > 0)
            {
                // DefaultFrameLatencySeconds is only used if prediction doesn't return a valid future time
                const double DefaultFrameLatencySeconds = 0.022;
                var timeDisplayed = SpatialControllerUtils.GetPredictAnchorTime(DefaultFrameLatencySeconds);

                // Get a predicted anchor transform.
                // Predicting transforms at the expected time of display is important
                // to avoid the higher latency rendered location lagging behind the 
                // visible location of the controller in low latency passthrough.
                var accessoryAnchor = AccessoryTracking.PredictAnchor(uid, timeDisplayed);

                if (accessoryAnchor != null)
                {
                    //check what hand the accessory is
                    if (hand == AccessoryChirality.Left)
                    {
                        //apply the transform here
                        // SpatialControllerUtils.ApplyAccessoryAnchorTransform(LeftController, accessoryAnchor);
                    }
                    else if (hand == AccessoryChirality.Right)
                    {
                        //apply the transform here
                        // SpatialControllerUtils.ApplyAccessoryAnchorTransform(RightController, accessoryAnchor);
                    }
                }
            }

            ///
            /// Get the input button state returned by the earlier PollController(uid).
            /// 
            var input = state.input;

            // Get the battery state, if any
            // var battery = state.battery;
            // Debug.Log("{uid} {controllerName} Battery Level: " + battery.level +" State: "+ battery.state);

            var controllerName = "";
            if (SpatialControllerUtils.IsLeftController(state.accessory))
            {
                //this is the left controller
                controllerName = "left";
            }
            else if (SpatialControllerUtils.IsRightController(state.accessory))
            {
                //this is the right controller
                controllerName = "right";
            }
            else
            {
                controllerName = "controller";
            }

            // Specific buttons can be accessed by ControllerInputName:
            // If you know this controller has this input:
            // var buttonA = input.buttons[ControllerInputName.ButtonA];
            // Or if you don't know whether this controller has the button in question:
            // ButtonState buttonA;
            // if (input.buttons.TryGetValue(ControllerInputName.ButtonA, out buttonA)) {}

            // Here, just logging every button while it is pressed:
            foreach (var entry in input.buttons)
            {
                var inputName = entry.Key;
                var button = entry.Value;
                // Get the nice controller specific name for this button, if available.
                var info = AccessoryTracking.GetControllerInputInfoForInputName(uid, inputName);
                var localizedName = info?.localizedName ?? inputName.ToString();
                if (button.isAnalog)
                {
                    // analog: value is float in range 0 to 1, isPressed if value > 0
                    if (button.isPressed)
                    {
                        Debug.Log($"{uid} {controllerName} {localizedName} depression is {button.value}");
                    }
                }
                else
                {
                    // digital: value is float 0 or 1
                    if (button.isPressed)
                    {
                        Debug.Log($"{uid} {controllerName} {localizedName} is pressed");
                    }
                }
            }

            // Thumbsticks and other directional inputs have float 2 axis values from -1 to 1:
            foreach (var entry in input.dpads)
            {
                var inputName = entry.Key;
                var dpad = entry.Value;

                // Get the nice controller specific name for this button, if available.
                var info = AccessoryTracking.GetControllerInputInfoForInputName(uid, inputName);
                var localizedName = info?.localizedName ?? inputName.ToString();

                if (dpad.xAxis != 0 || dpad.yAxis != 0)
                {
                    Debug.Log($"{uid} {controllerName} {localizedName} position is ({dpad.xAxis},{dpad.yAxis})");
                }
            }
        }
    }

    // Returns a simple AHAP JSON format haptic data set for testing
    public byte[] simpleHapticsData()
    {
        string AHAP_json = "{ \"Version\" : 1, \"Metadata\" : { \"Project\" : \"Haptic Controllers\", \"Created\" : \"9 June 2020\", \"Description\" : \"A single hit at full intensity\" }, \"Pattern\" : [ { \"Event\" : { \"EventType\" : \"HapticTransient\", \"EventParameters\" : [ { \"ParameterID\" : \"HapticIntensity\", \"ParameterValue\" : 1.000000 } ], \"Time\" : 0 } } ] }";
        return Encoding.ASCII.GetBytes(AHAP_json);
    }

    // Play AHAP JSON format data on controller uuid's haptics engine:
    public bool PlayHapticOnController(string uid, byte[] AHAPdata)
    {
        byte[] data = simpleHapticsData();
        PlayHapticsFinishedCallback OnPlaybackFinished = (Error error) =>
        {
            if (error != null)
            {
                Debug.Log($"PlayHapticsData({uid}) stopped with error({error.code}): {error.localizedDescription}");
            }
            else
            {
                Debug.Log($"PlayHapticsData({uid}) finished");
            }
            return HapticEngineFinishedAction.StopEngine;
        };
        Debug.Log($"PlayHapticsData({uid}, {AHAPdata.Length} bytes) started...");
        if (!AccessoryTracking.PlayHapticsData(uid, AHAPdata, HapticsLocality.Default, OnPlaybackFinished))
        {
            Debug.Log($"PlayHapticsData({uid}, {AHAPdata.Length} bytes) failed!");
            return false;
        }
        return true;
    }

    // Load an AHAP format JSON file added as Resources/MyAHAP.bytes and
    // play it on controller uuid's haptics engine:
    public bool PlayHapticOnController(string uid, string resourceName)
    {
        TextAsset ahapFile = Resources.Load<TextAsset>(resourceName);
        if (ahapFile == null)
        {
            Debug.Log($"PlayHapticOnController({uid}, {resourceName}) failed: Resource {resourceName}.bytes not found!");
            return false;
        }
        return PlayHapticOnController(uid, ahapFile.bytes);
    }

    public void ControllerConnected(string uid)
    {
        Debug.Log("Controller Connected: " + uid);
    }

    public void ControllerDisconnected(string uid)
    {
        Debug.Log("Controller Disconnected: " + uid);
    }
}