using System.Runtime.InteropServices;

namespace Apple.GameController.Controller
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GCControllerHandle
    {
        private const string _xboxControllerType       = "Xbox One";
        private const string _dualShockControllerType  = "DualShock 4";
        private const string _dualSenseControllerType  = "DualSense";
        private const string _siriRemoteControllerType = "Siri Remote";

        public string UniqueId;
        public string ProductCategory;
        public string VendorName;
        [MarshalAs(UnmanagedType.I1)]
        public bool IsAttachedToDevice;
        [MarshalAs(UnmanagedType.I1)]
        public bool HasHaptics;
        [MarshalAs(UnmanagedType.I1)]
        public bool HasLight;
        [MarshalAs(UnmanagedType.I1)]
        public bool HasBattery;

        public GCControllerType GetControllerType()
        {
            if (ProductCategory == _xboxControllerType)
                return GCControllerType.XboxOne;

            if (ProductCategory == _dualShockControllerType)
                return GCControllerType.DualShock;

            if (ProductCategory == _dualSenseControllerType)
                return GCControllerType.DualSense;

            if (ProductCategory == _siriRemoteControllerType)
                return GCControllerType.SiriRemote;

            return GCControllerType.Unknown;
        }
    }
}