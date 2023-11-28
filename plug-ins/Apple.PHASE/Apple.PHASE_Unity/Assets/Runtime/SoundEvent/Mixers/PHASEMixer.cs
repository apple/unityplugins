using UnityEngine;
using XNode;

namespace Apple.PHASE
{
    /// <summary>
    /// Parent class for all mixers: spatial, channel and ambient.
    /// </summary>
    public abstract class PHASEMixer : Node
    {
        // Spatial mixer ID.
        protected internal long _mixerId = Helpers.InvalidId;

        virtual public long GetMixerId() { return _mixerId; }

        /// <summary>
        /// The parent sound event node for this mixer.
        /// </summary>
        [Input(typeConstraint = TypeConstraint.Strict)] public PHASEMixer ParentNode = null;

        // Gain parameter to modify the mixer gain.
        [Output] public PHASESoundEventParameterDouble GainParameter = null;
        private PHASESoundEventParameterDouble _gainParameter;

        private const double MixMixerGain = 0.0;
        private const double MaxMixerGain = 1.0;

        public override object GetValue(NodePort port)
        {
            return this;
        }

        /// <summary>
        /// Creates a Gain Meta Parameter to enable runtime gain modification for this mixer.
        /// </summary>
        protected void CreateGainMetaParameter()
        {
            var parameterPort = GetOutputPort("GainParameter");
            bool hasValidConnection = parameterPort != null && parameterPort.Connection != null && parameterPort.Connection.node != null;
            if (!hasValidConnection)
            {
                return;
            }

            if (parameterPort.ConnectionCount > 1)
            {
                Debug.LogError($"PHASEMixer {name} with id {_mixerId} is connected to more than one Gain Parameter.");
                return;
            }

            _gainParameter = parameterPort.Connection.node as PHASESoundEventParameterDouble;

            if (_gainParameter.ParameterName.Length == 0)
            {
                Debug.LogError($"Failed to create gainMetaParameter {_gainParameter.ParameterName} for PHASEMixer {name} with id {_mixerId}. No parameter name specified in Sound Event Composer!");
                return;
            }

            _gainParameter.MinimumValue = MixMixerGain;
            _gainParameter.MaximumValue = MaxMixerGain;
            _gainParameter.Create();
   
            bool result = Helpers.PHASESetMixerGainMetaParameter(_gainParameter.ParameterId, _mixerId);
            if (!result)
            {
                Debug.LogError($"Failed to set gainMetaParameter {_gainParameter.ParameterId} for PHASEMixer {name} with id {_mixerId}.");
                _gainParameter = null;
            }
        }

        /// <summary>
        /// Set the Gain Meta Parameter for this mixer.
        /// </summary>
        public void SetGainMetaParameter(long instanceId)
        {
            if (_gainParameter != null)
            {
                Helpers.PHASESetSoundEventParameterDbl(instanceId, _gainParameter.ParameterName, _gainParameter.DefaultValue);
            }
        }

        /// <summary>
        /// Destroy the mixer in the PHASE engine.
        /// </summary>
        public void DestroyFromPHASE()
        {
            Helpers.PHASEDestroyMixer(_mixerId);
            _mixerId = Helpers.InvalidId;
        }

        void OnDestroy()
        {
            DestroyFromPHASE();
        }
    }
}