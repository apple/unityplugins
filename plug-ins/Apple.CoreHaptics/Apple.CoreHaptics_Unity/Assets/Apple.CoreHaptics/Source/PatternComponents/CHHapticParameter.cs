using System;
using System.Runtime.InteropServices;

namespace Apple.CoreHaptics
{
    [Serializable]
    public class CHHapticParameter : ICHHapticPatternEntry
    {
        public CHHapticDynamicParameterID ParameterID;
        public float ParameterValue;
        public float Time;

        public CHHapticParameter()
        {
            ParameterID = CHHapticDynamicParameterID.HapticIntensityControl;
            ParameterValue = 1.0f;
            Time = 0f;
        }

        public CHHapticParameter(CHHapticDynamicParameterID parameterId, float parameterValue)
        {
            ParameterID = parameterId;
            ParameterValue = parameterValue;
            Time = 0f;
        }

        public CHHapticParameter(CHHapticDynamicParameterID parameterId, float parameterValue, float time)
        {
            ParameterID = parameterId;
            ParameterValue = parameterValue;
            Time = time;
        }
    }

    /// <summary>
    /// A helper struct for passing CHHapticDynamicParameters across the C# / Objc boundary
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MarshallingDynamicParameter
    {
        int ParameterID;
        float Time;
        float ParameterValue;

        public MarshallingDynamicParameter(int paramId, float time, float paramValue)
        {
            ParameterID = paramId;
            Time = time;
            ParameterValue = paramValue;
        }
    }
}