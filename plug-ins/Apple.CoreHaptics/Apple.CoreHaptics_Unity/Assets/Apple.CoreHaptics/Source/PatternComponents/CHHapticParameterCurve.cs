using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apple.CoreHaptics
{
    [Serializable]
    public class CHHapticParameterCurve : ICHHapticPatternEntry
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
    }
}