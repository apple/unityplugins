using System.Runtime.InteropServices;

namespace Apple.GameController.Controller
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GCControllerConnectionStateChangedResponse
    {
        public bool IsConnected;
        public GCControllerHandle ControllerHandle;
    }
}