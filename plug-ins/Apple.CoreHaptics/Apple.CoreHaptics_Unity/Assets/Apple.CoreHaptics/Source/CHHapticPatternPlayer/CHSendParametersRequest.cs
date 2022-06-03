using System;
using System.Runtime.InteropServices;

namespace Apple.CoreHaptics
{
    [StructLayout(LayoutKind.Sequential, Pack = 8), Serializable]
    internal struct CHSendParametersRequest
    {
        public IntPtr Parameters;
        public int ParametersLength;
        public float AtTime;
    }
}