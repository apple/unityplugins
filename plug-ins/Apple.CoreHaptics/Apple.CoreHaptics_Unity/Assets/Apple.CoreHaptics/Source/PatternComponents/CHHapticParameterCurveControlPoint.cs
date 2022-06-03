using System;
using System.Runtime.InteropServices;

namespace Apple.CoreHaptics
{
    [Serializable]
    public class CHHapticParameterCurveControlPoint
    {
        public float Time;
        public float ParameterValue;
        
        public CHHapticParameterCurveControlPoint() {}

        public CHHapticParameterCurveControlPoint(float time, float parameterValue)
        {
            Time = time;
            ParameterValue = parameterValue;
        }
    }

    /// <summary>
    /// A helper struct for passing CHHapticParameterCurves across the C# / Objc boundary
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MarshallingParameterCurveControlPoint
    {
        float Time;
        float ParameterValue;

        public MarshallingParameterCurveControlPoint(float time, float parameterValue)
        {
            Time = time;
            ParameterValue = parameterValue;
        }

        public MarshallingParameterCurveControlPoint(CHHapticParameterCurveControlPoint point)
        {
            Time = point.Time;
            ParameterValue = point.ParameterValue;
        }
    }
}