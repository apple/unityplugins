using UnityEngine;
using System.Collections.Generic;

namespace Apple.PHASE
{
    public class PHASESoundEventMappedParameter : PHASESoundEventParameter
    {
        /// <summary>
        /// Envelope segments for use in the Unity Editor.
        /// </summary>
        [System.Serializable]
        public struct Segment
        {
            public float X;
            public float Y;
            public Helpers.EnvelopeCurveType CurveType;
        };

        [SerializeField]
        private Vector2 _startPoint = new Vector2();

        /// <summary>
        /// A list containging each envelope <c>Segment</c> for this parameter.
        /// </summary>
        [SerializeField]
        public List<Segment> EnvelopeSegments = new List<Segment>();

        /// <summary>
        /// The sound event node that is parent to this parameter.
        /// </summary>
        [Input] public PHASESoundEventParameterDouble ParentNode = null;

        /// <summary>
        /// The parameter that is controlled by this mapped parameter.
        /// </summary>
        [Output] public PHASESoundEventParameterDouble Parameter = null;
        private PHASESoundEventParameter _parameter = null;

        /// <summary>
        /// Create this mapped parameter in the PHASE engine.
        /// </summary>
        /// <returns>Returns true if succesful, false otherwise.</returns>
        public override bool Create()
        {
            _parameter = GetParameterFromPort();

            if (_parameter != null)
            {
                _parameter.Create();
            }
            else
            {
                Debug.LogError("Cannot create Sound Event mapped Parameter without a parameter");
                return false;
            }

            _parameterId = Helpers.PHASECreateMappedMetaParameter(_parameter.GetParameterId(), GetEnvelopeParameters());


            if (_parameterId == Helpers.InvalidId)
            {
                Debug.LogError("Failed to create Sound Event Mapped Parameter");
                return false;
            }
            return true;
        }


        /// <returns> Returns the envelope parameters formatted for the PHASE native plugin. </returns>
        public Helpers.EnvelopeParameters GetEnvelopeParameters()
        {
            Helpers.EnvelopeSegment[] segments = new Helpers.EnvelopeSegment[EnvelopeSegments.Count];
            for (int i = 0; i < EnvelopeSegments.Count; i++)
            {
                segments[i].X = EnvelopeSegments[i].X;
                segments[i].Y = EnvelopeSegments[i].Y;
                segments[i].CurveType = EnvelopeSegments[i].CurveType;
            }
            Helpers.EnvelopeParameters envelopeParameters = new Helpers.EnvelopeParameters
            {
                X = _startPoint.x,
                Y = _startPoint.y,
                SegmentCount = EnvelopeSegments.Count,
                EnvelopeSegments = segments
            };
            return envelopeParameters;
        }

        private PHASESoundEventParameter GetParameterFromPort()
        {
            var parameterPort = GetOutputPort("Parameter");
            if (parameterPort != null && parameterPort.Connection != null && parameterPort.Connection.node != null)
            {
                return parameterPort.Connection.node as PHASESoundEventParameter;
            }
            else
            {
                return null;
            }
        }
    }
}