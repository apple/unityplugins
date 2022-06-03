using UnityEngine;

namespace Apple.PHASE
{
    /// <summary>
    /// Class responsible for managing parameters of a channel mixer.
    /// </summary>
    public class PHASEChannelMixer : PHASEMixer
    {
        /// <summary>
        /// The channel layout for this channel mixer.
        /// </summary>
        [SerializeField] private Helpers.ChannelLayoutType _channelLayout = Helpers.ChannelLayoutType.Mono;

        /// <summary>
        /// Creates the channel mixer in the PHASE engine if it doesn't already exist and returns the mixer ID.
        /// </summary>
        /// <returns> A long representing the unique lower level ID of this mixer.</returns>
        public override long GetMixerId()
        {
            if (_mixerId == Helpers.InvalidId)
            {
                _mixerId = Helpers.PHASECreateChannelMixer(name, _channelLayout);
                if (_mixerId == Helpers.InvalidId)
                {
                    Debug.LogError("Failed to create PHASE channel mixer.");
                }
            }
            return _mixerId;
        }
    }
}