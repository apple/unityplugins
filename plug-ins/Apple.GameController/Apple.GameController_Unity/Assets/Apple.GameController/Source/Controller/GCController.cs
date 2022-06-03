using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using AOT;

namespace Apple.GameController.Controller
{
    public class GCController
    {
        private Dictionary<GCControllerInputName, bool> _buttonPressStates;
        private Dictionary<GCControllerInputName, bool> _previousButtonPressStates;

        public event EventHandler<ConnectionStateChangeEventArgs> ConnectedStateChanged;
        public event EventHandler Polled;

        public GCControllerHandle Handle;
        public GCControllerInputState InputState;        
        public bool IsConnected;

        public GCController(GCControllerHandle handle)
        {
            _buttonPressStates = new Dictionary<GCControllerInputName, bool>();
            _previousButtonPressStates = new Dictionary<GCControllerInputName, bool>();

            IsConnected = true;
            Handle = handle;
        }

        internal void NotifiyConnectedStateChanged(ConnectionStateChangeEventArgs args)
        {
            ConnectedStateChanged?.Invoke(this, args);
        }

        public Texture2D GetSymbolForInputName(GCControllerInputName inputName, GCControllerSymbolScale symbolScale = GCControllerSymbolScale.Medium, GCControllerRenderingMode renderingMode = GCControllerRenderingMode.Automatic)
        {
            return GCControllerService.GetSymbolForInputName(Handle, inputName, symbolScale, renderingMode);
        }

        public bool GetButton(GCControllerInputName inputName, float threshold = 0.25f)
        {
            var value = GetInputValue(inputName);
            var result = Mathf.Abs(value) >= threshold;

            return result;
        }

        public bool GetButtonDown(GCControllerInputName inputName, float threshold = 0.25f)
        {
            // Ensure our state is set...
            if (!_buttonPressStates.ContainsKey(inputName))
                _buttonPressStates[inputName] = false;

            if (!_previousButtonPressStates.ContainsKey(inputName))
                _previousButtonPressStates[inputName] = false;

            var pressed = GetButton(inputName, threshold);
            var isButtonDown = false;

            if (!_previousButtonPressStates[inputName] && pressed)
            {
                isButtonDown = true;
            }

            // Always set the last state...
            _buttonPressStates[inputName] = pressed;

            return isButtonDown;
        }

        public bool GetButtonUp(GCControllerInputName inputName, float threshold = 0.25f)
        {
            // Ensure our state is set...
            if (!_buttonPressStates.ContainsKey(inputName))
                _buttonPressStates[inputName] = false;

            if (!_previousButtonPressStates.ContainsKey(inputName))
                _previousButtonPressStates[inputName] = false;

            var pressed = GetButton(inputName, threshold);
            var isButtonUp = false;

            if (_previousButtonPressStates[inputName] && !pressed)
            {
                isButtonUp = true;
            }

            // Always set the last state...
            _buttonPressStates[inputName] = pressed;

            return isButtonUp;
        }

        public float GetInputValue(GCControllerInputName inputName)
        {
            switch (inputName)
            {
                case GCControllerInputName.ButtonHome:
                    return InputState.ButtonHome;
                case GCControllerInputName.ButtonMenu:
                    return InputState.ButtonMenu;
                case GCControllerInputName.ButtonOptions:
                    return InputState.ButtonOptions;
                case GCControllerInputName.ButtonSouth:
                    return InputState.ButtonA;
                case GCControllerInputName.ButtonEast:
                    return InputState.ButtonB;
                case GCControllerInputName.ButtonNorth:
                    return InputState.ButtonY;
                case GCControllerInputName.ButtonWest:
                    return InputState.ButtonX;
                case GCControllerInputName.ShoulderRightFront:
                    return InputState.ShoulderRightFront;
                case GCControllerInputName.ShoulderRightBack:
                    return InputState.ShoulderRightBack;
                case GCControllerInputName.ShoulderLeftFront:
                    return InputState.ShoulderLeftFront;
                case GCControllerInputName.ShoulderLeftBack:
                    return InputState.ShoulderLeftBack;
                case GCControllerInputName.DpadHorizontal:
                    return InputState.DpadHorizontal;
                    // DPad Specialized for Unity...
                case GCControllerInputName.DpadRight:
                    return Mathf.Clamp(InputState.DpadHorizontal, 0, 1);
                case GCControllerInputName.DpadLeft:
                    return Mathf.Clamp(InputState.DpadHorizontal, -1, 0);
                case GCControllerInputName.DpadUp:
                    return Mathf.Clamp(InputState.DpadVertical, 0, 1);
                case GCControllerInputName.DpadDown:
                    return Mathf.Clamp(InputState.DpadVertical, -1, 0);
                case GCControllerInputName.DpadVertical:
                    return InputState.DpadVertical;
                case GCControllerInputName.ThumbstickLeftHorizontal:
                    return InputState.ThumbstickLeftHorizontal;
                case GCControllerInputName.ThumbstickLeftVertical:
                    return InputState.ThumbstickLeftVertical;
                case GCControllerInputName.ThumbstickLeftButton:
                    return InputState.ThumbstickLeftButton;
                case GCControllerInputName.ThumbstickRightHorizontal:
                    return InputState.ThumbstickRightHorizontal;
                case GCControllerInputName.ThumbstickRightVertical:
                    return InputState.ThumbstickRightVertical;
                case GCControllerInputName.ThumbstickRightButton:
                    return InputState.ThumbstickRightButton;
                // Dualshock & DualSense
                case GCControllerInputName.TouchpadButton:
                    return InputState.TouchpadButton;
                case GCControllerInputName.TouchpadPrimaryHorizontal:
                    return InputState.TouchpadPrimaryHorizontal;
                case GCControllerInputName.TouchpadPrimaryVertical:
                    return InputState.TouchpadPrimaryVertical;
                case GCControllerInputName.TouchpadSecondaryHorizontal:
                    return InputState.TouchpadSecondaryHorizontal;
                case GCControllerInputName.TouchpadSecondaryVertical:
                    return InputState.TouchpadSecondaryVertical;
                default:
                    return 0;
            }
        }
        public float GetBatteryLevel()
        {
            return InputState.BatteryLevel;
        }

        public GCBatteryState GetBatteryState()
        {
            switch (InputState.BatteryState)
            {
                case 0: return GCBatteryState.Discharging;
                case 1: return GCBatteryState.Charging;
                case 2: return GCBatteryState.Full;
                default: return GCBatteryState.Unknown;
            }
        }

        public void Poll()
        {
            _previousButtonPressStates = new Dictionary<GCControllerInputName, bool>(_buttonPressStates);            
            foreach (var key in _buttonPressStates.Keys.ToList())
            {
                _buttonPressStates[key] = false;
            }

            InputState = IsConnected ? GCControllerService.PollController(Handle) : GCControllerInputState.None;
            Polled?.Invoke(this, EventArgs.Empty);
        }

        // --
        public void SetLightColor(float red, float green, float blue)
        {
            GCControllerService.SetControllerLightColor(Handle, red, green, blue);
        }
    }
} 