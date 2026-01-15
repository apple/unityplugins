using UnityEngine;
using Unity.Mathematics;
using System;
using AOT;
using System.Collections.Generic;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;

namespace Apple.visionOS.AccessoryTracking
{
    using SpatialController;

    public class AccessoryTrackingHelper : MonoBehaviour
    {

        [Header("Debug Settings")]
        [Tooltip("Enable this to show debug messages in the console during runtime")]
        public bool ShowDebugLogs = false;

        [Header("Don't Destroy GameObject When The Scene Changes")]
        [Tooltip("When changing scenes unity will destroy all GameObjects. Check this box to confirm it won't be destroyed")]
        public bool DontDestroyGameObjectOnSceneChange = true;

        /// <summary>
        /// A list of all the connected controller IDs
        /// </summary>
        public List<string> controllerIds = new List<string>();

        [Header("Automatically Setup and Move Transforms")]
        public bool AutomaticallySetupAccessoryTracking = true;

        public bool HideControllerAxisTransforms = false;

        public bool showWorldOriginAxis = true;

        public GameObject worldOriginAxis;

        public enum LocationOption { origin, aim, grip, gripSurface };

        public Transform LeftController;

        [SerializeField] private LocationOption LeftLocation = LocationOption.origin;

        public TMP_Text LeftControllerText;
        public Transform RightController;

        [SerializeField] private LocationOption RightLocation = LocationOption.origin;
        public TMP_Text RightControllerText;

        [Header("Controller Events")]
        public UnityEvent<string> ControllerConnectedEvent;
        public UnityEvent<string> ControllerDisconnectedEvent;

        /// <summary>
        /// An array of accessories that have been polled
        /// </summary>
        public Accessory[] accessories;

        public static Accessory.LocationName? toLocationName(LocationOption location)
        {
            switch (location)
            {
                case LocationOption.origin:
                    return null;
                case LocationOption.aim:
                    return Accessory.LocationName.aim;
                case LocationOption.grip:
                    return Accessory.LocationName.grip;
                case LocationOption.gripSurface:
                    return Accessory.LocationName.gripSurface;
                default:
                    return null;
            }
        }

        void Awake()
        {
            if (ShowDebugLogs)
            {
                Debug.Log("DontDestroyOnLoad: " + DontDestroyGameObjectOnSceneChange);
            }

            if (DontDestroyGameObjectOnSceneChange)
            {
                DontDestroyOnLoad(this.gameObject);
            }

            if (!AutomaticallySetupAccessoryTracking)
            {
                return;
            }

            if (AutomaticallySetupAccessoryTracking)
            {
                //automatically start
                SpatialControllerStart();
            }

            if (HideControllerAxisTransforms)
            {
                LeftController.gameObject.SetActive(false);
                RightController.gameObject.SetActive(false);
            }

            if (showWorldOriginAxis == false)
            {
                worldOriginAxis.SetActive(false);
            }
            else
            {
                //confirm the world origin axis gameobject is set to 0,0,0
                worldOriginAxis.transform.position = new Vector3(0, 0, 0);
            }

        }

        void OnDisable()
        {
            if (!AutomaticallySetupAccessoryTracking)
            {
                return;
            }

            SpatialControllerStop();
        }

        void Update()
        {

            if (!AutomaticallySetupAccessoryTracking)
            {
                return;
            }

            //get all the accessories
            accessories = AccessoryTracking.GetConnectedAccessories();

            //loop through all the accessories
            foreach (var accessory in accessories)
            {
                var uid = accessory.source.uniqueId;
                var hand = accessory.inherentChirality;
                var state = AccessoryTracking.PollController(uid);

                if (state.accessoryAnchors.Length > 0)
                {
                    // DefaultFrameLatencySeconds is only used if prediction doesn't return a valid future time
                    const double DefaultFrameLatencySeconds = 0.022;
                    var timeDisplayed = SpatialControllerUtils.GetPredictAnchorTime(DefaultFrameLatencySeconds);

                    var accessoryAnchor = AccessoryTracking.PredictAnchor(uid, timeDisplayed);
                    if (accessoryAnchor != null)
                    {
                        if (hand == AccessoryChirality.Left)
                        {
                            var location = toLocationName(LeftLocation);
                            if (location.HasValue)
                            {
                                SpatialControllerUtils.ApplyAccessoryAnchorTransform(LeftController, accessoryAnchor, location.Value, ARKitCoordinateSpace.Correction.None);
                            }
                            else
                            {
                                SpatialControllerUtils.ApplyAccessoryAnchorTransform(LeftController, accessoryAnchor);
                            }
                        }
                        else if (hand == AccessoryChirality.Right)
                        {
                            var location = toLocationName(RightLocation);
                            if (location.HasValue)
                            {
                                SpatialControllerUtils.ApplyAccessoryAnchorTransform(RightController, accessoryAnchor, location.Value, ARKitCoordinateSpace.Correction.None);
                            }
                            else
                            {
                                SpatialControllerUtils.ApplyAccessoryAnchorTransform(RightController, accessoryAnchor);
                            }
                        }
                    }
                }

                // get the input on the controller
                var input = state.input;

                ///
                /// A list of all the button presses
                ///

                var isLeftController = state.accessory.inherentChirality;

                //float from 0 or 1
                var buttonAValue = input.buttons[ControllerInputName.ButtonA].value;
                var buttonBValue = input.buttons[ControllerInputName.ButtonB].value;
                var menuButton = input.buttons[ControllerInputName.ButtonMenu].value;
                var gripButton = input.buttons[ControllerInputName.ButtonGrip].value;
                var thumbStickButton = input.buttons[ControllerInputName.ButtonThumbstick].value;

                //trigger - float from 0 to 1
                var triggerButton = input.buttons[ControllerInputName.ButtonTrigger].value;

                // float from -1 to 1
                var thumbStickX = input.dpads[ControllerInputName.DPadThumbstick].xAxis;
                var thumbStickY = input.dpads[ControllerInputName.DPadThumbstick].yAxis;

                bool anyButtonPressed = false;
                if (SpatialControllerUtils.IsLeftController(state.accessory))
                {
                    //this is the left controller
                    string buttonsText = "";
                    if (buttonAValue != 0)
                    {
                        buttonsText += "[ ]"; // square
                        anyButtonPressed = true;
                        if (ShowDebugLogs)
                        {
                            Debug.Log("Square Button Press Event: " + buttonAValue);
                        }
                    }
                    else
                    {
                        buttonsText += "  ";
                    }
                    if (buttonBValue != 0)
                    {
                        buttonsText += "/\\"; // triangle
                        anyButtonPressed = true;
                        if (ShowDebugLogs)
                        {
                            Debug.Log("Triangle Button Press Event: " + buttonBValue);
                        }
                    }
                    else
                    {
                        buttonsText += "  ";
                    }
                    if (menuButton != 0)
                    {
                        buttonsText += "Sh"; // share
                        anyButtonPressed = true;
                        if (ShowDebugLogs)
                        {
                            Debug.Log("Share Button Press Event: " + menuButton);
                        }
                    }
                    else
                    {
                        buttonsText += "  ";
                    }
                    if (triggerButton != 0)
                    {
                        buttonsText += "L1";
                        anyButtonPressed = true;
                        if (ShowDebugLogs)
                        {
                            Debug.Log("L1 Trigger Press Event: " + triggerButton);
                        }
                    }
                    else
                    {
                        buttonsText += "  ";
                    }
                    if (gripButton != 0)
                    {
                        buttonsText += "L2";
                        anyButtonPressed = true;
                        if (ShowDebugLogs)
                        {
                            Debug.Log("L2 Press Event: " + gripButton);
                        }
                    }
                    else
                    {
                        buttonsText += "  ";
                    }
                    if (thumbStickButton != 0)
                    {
                        buttonsText += "L3";
                        anyButtonPressed = true;
                        if (ShowDebugLogs)
                        {
                            Debug.Log("L3 Press Event: " + thumbStickButton);
                        }
                    }
                    else
                    {
                        buttonsText += "  ";
                    }
                    if (!anyButtonPressed)
                    {
                        buttonsText = "Press Any Button";
                    }
                    LeftControllerText.text = buttonsText;
                }
                else if (SpatialControllerUtils.IsRightController(state.accessory))
                {
                    //this is the right controller
                    string buttonsText = "";
                    if (buttonAValue != 0)
                    {
                        buttonsText += "X"; // cross
                        anyButtonPressed = true;
                        if (ShowDebugLogs)
                        {
                            Debug.Log("Cross Button Press Event: " + buttonAValue);
                        }
                    }
                    else
                    {
                        buttonsText += "  ";
                    }
                    if (buttonBValue != 0)
                    {
                        buttonsText += "( )"; // circle
                        anyButtonPressed = true;
                        if (ShowDebugLogs)
                        {
                            Debug.Log("Circle Button Press Event: " + buttonBValue);
                        }
                    }
                    else
                    {
                        buttonsText += "  ";
                    }
                    if (menuButton != 0)
                    {
                        buttonsText += "Op"; // options
                        anyButtonPressed = true;
                        if (ShowDebugLogs)
                        {
                            Debug.Log("Options Button Press Event: " + menuButton);
                        }
                    }
                    else
                    {
                        buttonsText += "  ";
                    }
                    if (triggerButton != 0)
                    {
                        buttonsText += "R1";
                        anyButtonPressed = true;
                        if (ShowDebugLogs)
                        {
                            Debug.Log("R1 Trigger Press Event: " + triggerButton);
                        }
                    }
                    else
                    {
                        buttonsText += "  ";
                    }
                    if (gripButton != 0)
                    {
                        buttonsText += "R2";
                        anyButtonPressed = true;
                        if (ShowDebugLogs)
                        {
                            Debug.Log("R2 Press Event: " + gripButton);
                        }
                    }
                    else
                    {
                        buttonsText += "  ";
                    }
                    if (thumbStickButton != 0)
                    {
                        buttonsText += "R3";
                        anyButtonPressed = true;
                        if (ShowDebugLogs)
                        {
                            Debug.Log("R3 Press Event: " + thumbStickButton);
                        }
                    }
                    else
                    {
                        buttonsText += "  ";
                    }
                    if (!anyButtonPressed)
                    {
                        buttonsText = "Press Any Button";
                    }
                    RightControllerText.text = buttonsText;
                }
            }
        }

        ///
        /// API
        /// 


        public void SpatialControllerStart()
        {
#if UNITY_VISIONOS
            AccessoryTracking.Init();
            AccessoryTracking.AddAccessoryConnectionHandlers(OnAccessoryConnected, OnAccessoryDisconnected, this);
#else
            Debug.LogError("Apple.SpatialController is only supported in Build Platform: visionOS");      
#endif
        }

        public void SpatialControllerStop()
        {
#if UNITY_VISIONOS
            AccessoryTracking.RemoveAccessoryConnectionHandlers(OnAccessoryConnected, OnAccessoryDisconnected, this);
            AccessoryTracking.Destroy();
#else
            Debug.LogError("Apple.SpatialController is only supported in Build Platform: visionOS");      
#endif
        }


#if UNITY_VISIONOS
        //they should subscribe to a delegate that hides this monoinvoke
        [MonoPInvokeCallback(typeof(AccessoryConnectionCallback<AccessoryTrackingHelper>))]
        void OnAccessoryConnected(Accessory accessory, AccessoryTrackingHelper context)
        {
            string uid = accessory.source.uniqueId;
            Debug.Log($"AccessoryTracking: accessory connected: {uid}");
            context.controllerIds.Add(uid);

            if (ControllerConnectedEvent != null)
            {
                ControllerConnectedEvent.Invoke(uid);
            }
        }

        [MonoPInvokeCallback(typeof(AccessoryConnectionCallback<AccessoryTrackingHelper>))]
        void OnAccessoryDisconnected(Accessory accessory, AccessoryTrackingHelper context)
        {
            string uid = accessory.source.uniqueId;
            print($"AccessoryTracking: accessory disconnected: {uid}");
            context.controllerIds.Remove(uid);

            if (ControllerDisconnectedEvent != null)
            {
                ControllerDisconnectedEvent.Invoke(uid);
            }
        }
#endif

    }

}

