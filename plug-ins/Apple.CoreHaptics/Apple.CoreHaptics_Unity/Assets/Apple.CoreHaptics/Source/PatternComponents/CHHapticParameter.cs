using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Apple.UnityJSON;

namespace Apple.CoreHaptics
{
    [Serializable]
    public class CHHapticParameter : ICHHapticPatternEntry, ISerializable
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

        public string Serialize(Serializer serializer) {
            var ret = "{\n";
            ret += $"\t\t\t\t\"ParameterID\": \"{ParameterID}\",\n";
            ret += ((FormattableString)$"\t\t\t\t\"ParameterValue\": {ParameterValue},\n").ToString(CultureInfo.InvariantCulture);
            ret += ((FormattableString)$"\t\t\t\t\"Time\": {Time}\n").ToString(CultureInfo.InvariantCulture);

            ret += "\t\t\t}\n";
            return ret;
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