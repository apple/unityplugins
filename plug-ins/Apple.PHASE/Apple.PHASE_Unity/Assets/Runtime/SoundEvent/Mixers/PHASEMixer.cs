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

        /// <summary>
        /// The parent sound event node for this mixer.
        /// </summary>
        [Input(typeConstraint = TypeConstraint.Strict)] public PHASEMixer ParentNode = null;

        public override object GetValue(NodePort port)
        {
            return this;
        }
    }
}