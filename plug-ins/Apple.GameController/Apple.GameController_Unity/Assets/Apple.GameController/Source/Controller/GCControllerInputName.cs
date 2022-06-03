namespace Apple.GameController.Controller
{
    public enum GCControllerInputName : int
    {
        ButtonHome = 0,
        ButtonMenu = 1,
        ButtonOptions = 2,


        /// <summary>
        /// Represents ButtonA in the Apple documentation for GCController.
        /// </summary>
        ButtonSouth = 3,
        /// <summary>
        /// Represents ButtonB in the Apple documentation for GCController.
        /// </summary>
        ButtonEast = 4,
        /// <summary>
        /// Represents ButtonY in the Apple documentation for GCController.
        /// </summary>
        ButtonNorth = 5,
        /// <summary>
        /// Represents ButtonX in the Apple documentation for GCCOntroller.
        /// </summary>
        ButtonWest = 6,

        // Shoulder...
        ShoulderRightFront = 7,
        ShoulderRightBack = 8,
        ShoulderLeftFront = 9,
        ShoulderLeftBack = 10,

        // Dpad Axis...
        DpadHorizontal = 11,
        DpadVertical = 12,

        // Thumbstick...
        ThumbstickLeftHorizontal = 13,
        ThumbstickLeftVertical = 14,
        ThumbstickLeftButton = 15,
        ThumbstickRightHorizontal = 16,
        ThumbstickRightVertical = 17,
        ThumbstickRightButton = 18,

        // Dualshock & DualSense
        TouchpadButton = 19,
        TouchpadPrimaryHorizontal = 20,
        TouchpadPrimaryVertical = 21,
        TouchpadSecondaryHorizontal= 22,
        TouchpadSecondaryVertical = 23,

        // Unity specific Dpad...
        DpadLeft = 24,
        DpadRight = 25,
        DpadUp = 26,
        DpadDown = 27
    }
}