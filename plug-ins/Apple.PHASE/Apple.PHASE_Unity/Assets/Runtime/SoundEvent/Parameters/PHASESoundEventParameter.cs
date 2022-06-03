using XNode;
using UnityEngine;

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

        // Parameter ID - assigned after it gets created.
        protected long _parameterId = Helpers.InvalidId;

        /// <summary>
        /// Set the name of this parameter to the given string.
        /// </summary>
        /// <param name="inParameterName"></param>
        public void SetParameterName(string inParameterName)
        {
            _parameterName = inParameterName;
        }

        /// <returns> Returns a long representing the lower level parameter ID.</returns>
        public long GetParameterId() { return _parameterId; }

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