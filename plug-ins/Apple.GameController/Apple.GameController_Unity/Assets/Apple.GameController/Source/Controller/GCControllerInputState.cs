using System.Runtime.InteropServices;

namespace Apple.GameController.Controller
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GCControllerInputState
    {
        public float ButtonHome;
        public float ButtonMenu;
        public float ButtonOptions;
        public float ButtonA;
        public float ButtonB;
        public float ButtonY;
        public float ButtonX;
        // Shoulder...
        public float ShoulderRightFront;
        public float ShoulderRightBack;
        public float ShoulderLeftFront;
        public float ShoulderLeftBack;
        // Dpad...
        public float DpadHorizontal;
        public float DpadVertical;
        // Thumbsticks...
        public float ThumbstickLeftHorizontal;
        public float ThumbstickLeftVertical;
        public float ThumbstickLeftButton;
        public float ThumbstickRightHorizontal;
        public float ThumbstickRightVertical;
        public float ThumbstickRightButton;
        // Dualshock & DualSense
        public float TouchpadButton;
        public float TouchpadPrimaryHorizontal;
        public float TouchpadPrimaryVertical;
        public float TouchpadSecondaryHorizontal;
        public float TouchpadSecondaryVertical;
        // Battery
        public float BatteryLevel;
        public int BatteryState;

        public static GCControllerInputState None = new GCControllerInputState();
    }
}