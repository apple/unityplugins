using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apple.GameController;
using Apple.GameController.Controller;

public class SampleLogic : MonoBehaviour
{
    // Constants
    public const string kNoController = "No controller connected.";

    public static readonly Color kWhite      = new Color(1,1,1);
    public static readonly Color kGray       = new Color(197/255f,197/255f,197/255f);
    public static readonly Color kDarkGray   = new Color(113/255f,113/255f,116/255f);
    public static readonly Color kAlphaScale = new Color(1,1,1,.25f);

    public static readonly Color kRed        = new Color(255/255f,69/255f,58/255f);
    public static readonly Color kGreen      = new Color(50/255f,215/255f,75/255f);
    public static readonly Color kBlue       = new Color(10/255f,132/255f,254/255f);
    public static readonly Color kYellow     = new Color(255/255f,214/255f,15/255f);

    // UI Elements
    public UIText titleText;
    public UIText batteryText;
    
    public UISlider leftTrigger;
    public UISlider rightTrigger;
    
    public UIButtonImage leftBumperImage;
    public UIButtonImage rightBumperImage;

    public UIButtonImage buttonAImage;
    public UIButtonImage buttonBImage;
    public UIButtonImage buttonXImage;
    public UIButtonImage buttonYImage;
    
    public UIButton      homeButton;
    public UIButtonImage homeButtonImage;
    public UIButtonImage optionsButtonImage;
    public UIButtonImage menuButtonImage;

    // Position changes based upon connected controller
    public UIButton      leftThumbstickButton;
    public UIIndicator2D leftThumbstickIndicator;
    
    public UIIndicator2D rightThumbstickIndicator;

    // Position changes based upon connected controller
    public UIButton      DPadButton;
    public UIIndicator2D DPadIndicator;

    public GameObject touchpadUI;
    public UITouchpadIndicator touchpadIndicator;
    public UIButtonImage touchpadButtonImage;

    // Current controller instance
    GCController _controller;
    GCControllerType _controllerType;

    System.Random _rand;

    // Start is called before the first frame update
    void Start()
    {
        _controller = null;
        titleText.SetText(kNoController);
        batteryText.SetText("");
        _rand = new System.Random();

        GCControllerService.Initialize();

        // Grab the first controller found
        var controllers = GCControllerService.GetConnectedControllers();
        foreach (GCController controller in controllers)
        {
            Debug.Log("Found controller: " + controller.Handle.ProductCategory);
            _setController(controller);
            break;
        }

        GCControllerService.ControllerConnected    += _onControllerConnected;
        GCControllerService.ControllerDisconnected += _onControllerDisconnected;
    }

    // --
    private void _setController(GCController c)
    {
        _controller = c;
        _controller.SetLightColor(_rand.Next(20,256)/255f, _rand.Next(20,256)/255f, _rand.Next(20,256)/255f);
        
        titleText.SetText(_controller.Handle.ProductCategory);

        _controllerType = c.Handle.GetControllerType();

        switch (_controllerType)
        {
            case GCControllerType.DualShock:
            case GCControllerType.DualSense:
                touchpadUI.SetActive(true);
                homeButton.SetLocalPosition(0, -205);
                DPadButton.SetLocalPosition(-425, 25);
                leftThumbstickButton.SetLocalPosition(-195, -205);

                buttonAImage.SetTexture(_controller.GetSymbolForInputName(GCControllerInputName.ButtonSouth, GCControllerSymbolScale.Medium, GCControllerRenderingMode.AlwaysTemplate));
                buttonBImage.SetTexture(_controller.GetSymbolForInputName(GCControllerInputName.ButtonEast, GCControllerSymbolScale.Medium, GCControllerRenderingMode.AlwaysTemplate));
                buttonXImage.SetTexture(_controller.GetSymbolForInputName(GCControllerInputName.ButtonWest, GCControllerSymbolScale.Medium, GCControllerRenderingMode.AlwaysTemplate));
                buttonYImage.SetTexture(_controller.GetSymbolForInputName(GCControllerInputName.ButtonNorth, GCControllerSymbolScale.Medium, GCControllerRenderingMode.AlwaysTemplate));
                break;

            case GCControllerType.Unknown:
            case GCControllerType.XboxOne:
            default:
                touchpadUI.SetActive(false);
                homeButton.SetLocalPosition(0, 250);
                DPadButton.SetLocalPosition(-195, -205);
                leftThumbstickButton.SetLocalPosition(-425, 25);

                buttonAImage.SetTexture(_controller.GetSymbolForInputName(GCControllerInputName.ButtonSouth, GCControllerSymbolScale.Medium, GCControllerRenderingMode.AlwaysTemplate));
                buttonBImage.SetTexture(_controller.GetSymbolForInputName(GCControllerInputName.ButtonEast, GCControllerSymbolScale.Medium, GCControllerRenderingMode.AlwaysTemplate));
                buttonXImage.SetTexture(_controller.GetSymbolForInputName(GCControllerInputName.ButtonWest, GCControllerSymbolScale.Medium, GCControllerRenderingMode.AlwaysTemplate));
                buttonYImage.SetTexture(_controller.GetSymbolForInputName(GCControllerInputName.ButtonNorth, GCControllerSymbolScale.Medium, GCControllerRenderingMode.AlwaysTemplate));
                break;
        }
    }

    // --
    private void _onControllerConnected(object sender, ControllerConnectedEventArgs args)
    {
        Debug.Log("Controller Connected: " + args.Controller.Handle.ProductCategory);

        if (_controller == null)
        {
            _setController(args.Controller);
        }
    }

    // --
    private void _onControllerDisconnected(object sender, ControllerConnectedEventArgs args)
    {
        Debug.Log("Controller Disconnected: " + args.Controller.Handle.ProductCategory);

        if (_controller == args.Controller)
        {
            _controller = null;
            titleText.SetText(kNoController);
            batteryText.SetText("");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_controller != null)
        {
            _controller.Poll();

            //
            var value = _controller.GetInputValue(GCControllerInputName.ShoulderLeftBack);
            leftTrigger.SetValue(value);

            //
            value = _controller.GetInputValue(GCControllerInputName.ShoulderRightBack);
            rightTrigger.SetValue(value);

            //
            if (_controller.GetButton(GCControllerInputName.ShoulderLeftFront))
            {
                leftBumperImage.SetColor(kDarkGray);
            }
            else
            {
                leftBumperImage.SetColor(new Color(0,0,0,0));
            }

            //
            if (_controller.GetButton(GCControllerInputName.ShoulderRightFront))
            {
                rightBumperImage.SetColor(kDarkGray);
            }
            else
            {
                rightBumperImage.SetColor(new Color(0,0,0,0));
            }

            //
            if (_controller.GetButton(GCControllerInputName.ButtonSouth))
            {
                buttonAImage.SetColor(kGreen);
            }
            else
            {
                buttonAImage.SetColor(kGreen * kAlphaScale);
            }

            //
            if (_controller.GetButton(GCControllerInputName.ButtonEast))
            {
                buttonBImage.SetColor(kRed);
            }
            else
            {
                buttonBImage.SetColor(kRed * kAlphaScale);
            }

            //
            if (_controller.GetButton(GCControllerInputName.ButtonWest))
            {
                buttonXImage.SetColor(kBlue);
            }
            else
            {
                buttonXImage.SetColor(kBlue * kAlphaScale);
            }

            //
            if (_controller.GetButton(GCControllerInputName.ButtonNorth))
            {
                buttonYImage.SetColor(kYellow);
            }
            else
            {
                buttonYImage.SetColor(kYellow * kAlphaScale);
            }

            //
            if (_controller.GetButton(GCControllerInputName.ButtonHome))
            {
                homeButtonImage.SetColor(kGray);
            }
            else
            {
                homeButtonImage.SetColor(kGray * kAlphaScale);
            }

            //
            if (_controller.GetButton(GCControllerInputName.ButtonOptions))
            {
                optionsButtonImage.SetColor(kWhite);
            }
            else
            {
                optionsButtonImage.SetColor(kWhite * kAlphaScale);
            }

            //
            if (_controller.GetButton(GCControllerInputName.ButtonMenu))
            {
                menuButtonImage.SetColor(kWhite);
            }
            else
            {
                menuButtonImage.SetColor(kWhite * kAlphaScale);
            }

            //
            float xValue = _controller.GetInputValue(GCControllerInputName.ThumbstickLeftHorizontal);
            float yValue = _controller.GetInputValue(GCControllerInputName.ThumbstickLeftVertical);

            leftThumbstickIndicator.SetLocalPosition(xValue, yValue);

            //
            xValue = _controller.GetInputValue(GCControllerInputName.ThumbstickRightHorizontal);
            yValue = _controller.GetInputValue(GCControllerInputName.ThumbstickRightVertical);

            rightThumbstickIndicator.SetLocalPosition(xValue, yValue);

            //
            DPadIndicator.SetLocalPosition(_controller.InputState.DpadHorizontal, _controller.InputState.DpadVertical);

            //
            if (_controllerType == GCControllerType.DualShock || _controllerType == GCControllerType.DualSense)
            {
                if (_controller.GetButton(GCControllerInputName.TouchpadButton))
                {
                    touchpadButtonImage.SetColor(kDarkGray);
                }
                else
                {
                    touchpadButtonImage.SetColor(new Color(0,0,0,0));
                }

                // input range: [-1, 1]
                float hValue = _controller.GetInputValue(GCControllerInputName.TouchpadPrimaryHorizontal);
                float vValue = _controller.GetInputValue(GCControllerInputName.TouchpadPrimaryVertical);

                touchpadIndicator.SetLocalPosition(0, hValue, vValue);

                hValue = _controller.GetInputValue(GCControllerInputName.TouchpadSecondaryHorizontal);
                vValue = _controller.GetInputValue(GCControllerInputName.TouchpadSecondaryVertical);

                touchpadIndicator.SetLocalPosition(1, hValue, vValue);
            }

            //
            if (_controller.Handle.HasBattery)
            {
                string batteryStatusString = "Battery";
                GCBatteryState batteryState = _controller.GetBatteryState();

                switch (batteryState)
                {
                    case GCBatteryState.Unknown:
                        batteryStatusString += " Unknown";
                        break;

                    case GCBatteryState.Discharging:
                        batteryStatusString += " Discharging";
                        break;

                    case GCBatteryState.Charging:
                        batteryStatusString += " Charging";
                        break;

                    case GCBatteryState.Full:
                        batteryStatusString += " Full";
                        break;

                    default: break;
                }

                uint batteryPercentage = (uint)(_controller.GetBatteryLevel() * 100f);
                batteryStatusString += ": [" + batteryPercentage + "%]";

                batteryText.SetText(batteryStatusString);
            }
            else
            {
                batteryText.SetText("");
            }
        }
    }
}
