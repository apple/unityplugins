using UnityEngine;

namespace Apple.PHASE
{
    /// <summary>
    /// Class responsible for managing parameters of an ambient mixer.
    /// </summary>
    [NodeWidth(250)]
    public class PHASEAmbientMixer : PHASEMixer
    {
        /// <summary>
        /// Pitch represents rotation about the <c>x</c> axis.
        /// </summary>
        [SerializeField] [Range(0, 360)] private float _pitch = 0f;
        /// <summary>
        /// Yaw represents rotation about the <c>y</c> axis.
        /// </summary>
        [SerializeField] [Range(0, 360)] private float _yaw = 0f;
        /// <summary>
        /// Roll represents rotation about the <c>z</c> axis.
        /// </summary>
        [SerializeField] [Range(0, 360)] private float _roll = 0f;

        /// <summary>
        /// The channel layout of this mixer.
        /// </summary>
        [SerializeField] private Helpers.ChannelLayoutType _channelLayout = Helpers.ChannelLayoutType.Mono;

        /// <summary>
        /// Creates the ambient mixer in the PHASE engine if it doesn't already exist and returns the mixer ID.
        /// </summary>
        /// <returns> A long representing the unique lower level ID of this mixer.</returns>
        public override long GetMixerId()
        {
            if (_mixerId == Helpers.InvalidId)
            {
                var pitchQuat = Quaternion.AngleAxis(_pitch, Vector3.left);
                var yawQuat = Quaternion.AngleAxis(_yaw, Vector3.up);
                var rollQuat = Quaternion.AngleAxis(_roll, Vector3.back);
                Quaternion orientation = rollQuat * yawQuat * pitchQuat;
                orientation.Normalize();
                Helpers.PHASEQuaternion phaseOrientation = new Helpers.PHASEQuaternion
                {
                    W = orientation.w,
                    X = orientation.x,
                    Y = orientation.y,
                    Z = orientation.z
                };
                _mixerId = Helpers.PHASECreateAmbientMixer(name, _channelLayout, phaseOrientation);
                if (_mixerId == Helpers.InvalidId)
                {
                    Debug.LogError("Failed to create PHASE channel mixer.");
                }
            }
            return _mixerId;
        }
    }

}