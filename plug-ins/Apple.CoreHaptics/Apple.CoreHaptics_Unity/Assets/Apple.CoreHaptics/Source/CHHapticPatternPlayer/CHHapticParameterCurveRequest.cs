using System;
using System.Linq;
using System.Runtime.InteropServices;
	
namespace Apple.CoreHaptics
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct CHHapticParameterCurveRequest
	{
		private int ParameterID;
		private double Time;
		private IntPtr Points;
		private int PointsLength;

        private GCHandle _pointsHandle;

        public CHHapticParameterCurveRequest(CHHapticParameterCurve curve)
        {
			ParameterID = (int)curve.ParameterID;
			Time = curve.Time;

			var points = curve.ParameterCurveControlPoints.Select(p => new MarshallingParameterCurveControlPoint(p)).ToArray();
			_pointsHandle = GCHandle.Alloc(points, GCHandleType.Pinned);

			Points = _pointsHandle.AddrOfPinnedObject();
			PointsLength = curve.ParameterCurveControlPoints.Count;
        }

        public void FreeHandle()
        {
			_pointsHandle.Free();
        }
    }
}