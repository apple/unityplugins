using UnityEngine;
using XNode;

namespace Apple.PHASE
{
    /// <summary>
    /// Base sound event parameter class.
    /// </summary>
    [NodeWidth(250)]
    public abstract class PHASESoundEventParameter : Node
    {
        // Parameter name.
        [SerializeField] protected string _parameterName;
        public string ParameterName
        {
            get => _parameterName;
            set => _parameterName = value;
        }

        // Parameter ID - assigned after it gets created.
        protected long _parameterId = Helpers.InvalidId;
        public long ParameterId
        {
            get => _parameterId;
        }

        /// <summary>
        /// Derived classes create the specific type of parameter in the PHASE engine.
        /// </summary>
        /// <returns> Returns true if the parameter is succesfully created, false otherwise. </returns>
        public abstract bool Create();

        /// <summary>
        /// Destroy the parameter in the PHASE engine.
        /// </summary>
        public void DestroyFromPHASE()
        {
            Helpers.PHASEDestroySoundEventParameter(_parameterId);
            _parameterId = Helpers.InvalidId;
        }

        void OnDestroy()
        {
            DestroyFromPHASE();
        }
    }
}