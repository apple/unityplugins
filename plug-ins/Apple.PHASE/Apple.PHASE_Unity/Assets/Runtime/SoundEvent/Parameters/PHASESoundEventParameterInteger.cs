using UnityEngine;

namespace Apple.PHASE
{
    /// <summary>
    /// Sound event parameter of type integer.
    /// </summary>
    public class PHASESoundEventParameterInteger : PHASESoundEventParameter
    {
        /// <summary>
        /// The parent node of this parameter.
        /// </summary>
        [Input] public PHASESoundEventParameterInteger ParentNode = null;

        // Default int value.
        [SerializeField] private int _defaultValue;
        public int DefaultValue
        {
            get => _defaultValue;
        }

        // Minimum value of the parameter.
        private int _minimumValue = int.MinValue;

        // Maximum value of the parameter.
        private int _maximumValue = int.MaxValue;

        /// <summary>
        /// Sets the parameter to the given name and value.
        /// </summary>
        /// <param name="inParameterName"> Set the parameter to this string. </param>
        /// <param name="inDefaultValue"> Set the parameter to this value. </param>
        public void SetParameterInteger(string inParameterName, int inDefaultValue)
        {
            _defaultValue = inDefaultValue;
            _parameterName = inParameterName;
            Create();
        }

        /// <summary>
        /// Creates a new parameter of type integer in the PHASE engine.
        /// </summary>
        /// <returns> True if succesful, false otherwise. </returns>
        public override bool Create()
        {
            if (_parameterId != Helpers.InvalidId) return true;
            _parameterId = Helpers.PHASECreateSoundEventParameterInt(_parameterName, _defaultValue, _minimumValue, _maximumValue);
            if (_parameterId == Helpers.InvalidId)
            {
                Debug.Log("Failed to create sound event meta parameter");
                return false;
            }
            return true;
        }
    }
}