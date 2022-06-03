using System.Runtime.InteropServices;

namespace Apple.CoreHaptics
{
    [StructLayout(LayoutKind.Sequential, Pack = 8), System.Serializable]
    internal struct CHSendParameter
    {
        public int ParameterID;
        public float RelativeTime;
        public float Value;

        public CHSendParameter(CHHapticParameter hapticParameter)
        {
            ParameterID = (int)hapticParameter.ParameterID;
            RelativeTime = hapticParameter.Time;
            Value = hapticParameter.ParameterValue;
        }
    }
}