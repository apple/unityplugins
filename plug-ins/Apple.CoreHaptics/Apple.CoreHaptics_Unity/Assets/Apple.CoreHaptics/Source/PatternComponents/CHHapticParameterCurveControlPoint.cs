using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Apple.UnityJSON;

namespace Apple.CoreHaptics
{
    [Serializable]
    public class CHHapticParameterCurveControlPoint : ISerializable
    {
        public float Time;
        public float ParameterValue;
        
        public CHHapticParameterCurveControlPoint() {}

        public CHHapticParameterCurveControlPoint(float time, float parameterValue)
        {
            Time = time;
            ParameterValue = parameterValue;
        }

        public string Serialize(Serializer serializer) {
            var ret = "\t\t\t\t\t{\n";
            ret += ((FormattableString)$"\t\t\t\t\t\t\"Time\": {Time},\n").ToString(CultureInfo.InvariantCulture);
            ret += ((FormattableString)$"\t\t\t\t\t\t\"ParameterValue\": {ParameterValue}\n").ToString(CultureInfo.InvariantCulture);
            ret += "\t\t\t\t\t}";
            return ret;
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