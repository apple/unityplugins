using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using Apple.UnityJSON;

namespace Apple.CoreHaptics
{
    [Serializable]
    public class CHHapticParameterCurve : ICHHapticPatternEntry, ISerializable
    {
        public CHHapticDynamicParameterID ParameterID;
        public float Time;
        public List<CHHapticParameterCurveControlPoint> ParameterCurveControlPoints = new List<CHHapticParameterCurveControlPoint>();

        public AnimationCurve GetAnimationCurve()
        {
            var curve = new AnimationCurve();

            foreach (var point in ParameterCurveControlPoints)
            {
                curve.AddKey(point.Time, point.ParameterValue);
            }

            return curve;
        }

        public void UpdateControlPointsFromAnimationCurve(AnimationCurve curve)
        {
            ParameterCurveControlPoints.Clear();

            foreach (var key in curve.keys)
            {
                ParameterCurveControlPoints.Add(new CHHapticParameterCurveControlPoint(key.time, Mathf.Clamp(key.value, -1, 1)));
            }
        }

        public string Serialize(Serializer serializer) {
            var ret = "{\n";
            ret += $"\t\t\t\t\"ParameterID\": \"{ParameterID}\",\n";
            ret += ((FormattableString)$"\t\t\t\t\"Time\": {Time},\n").ToString(CultureInfo.InvariantCulture);
            ret += $"\t\t\t\t\"ParameterCurveControlPoints\": [\n";
            for(int idx=0; idx<ParameterCurveControlPoints.Count; idx++) {
                ret += ParameterCurveControlPoints[idx].Serialize(serializer);
                if (idx < ParameterCurveControlPoints.Count+1) {
                    ret += ",\n";
                } else {
                    ret += "\n";
                }
            }
            ret += "\t\t\t\t]";
            ret += "\t\t\t}\n";
            return ret;
        }
    }
}