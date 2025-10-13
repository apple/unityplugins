using UnityEngine;

namespace Apple.PHASE
{
    /// <summary>
    /// Sound event parameter of type double.
    /// </summary>
    public class PHASESoundEventParameterDouble : PHASESoundEventParameter
    {
        /// <summary>
        /// The parent node of this parameter.
        /// </summary>
        [Input] public PHASESoundEventParameterDouble ParentNode = null;

        // Default double value.
        [SerializeField] private double _defaultValue;
        public double DefaultValue
        {
            get => _defaultValue;
        }

        // Minimum value of the parameter.
        private double _minimumValue = double.MinValue;
        public double MinimumValue
        {
            get => _minimumValue;
            set => _minimumValue = value;
        }

        // Maximum value of the parameter.
        private double _maximumValue = double.MaxValue;
        public double MaximumValue
        {
            get => _maximumValue;
            set => _maximumValue = value;
        }

        /// <summary>
        /// Sets the parameter to the given name and value.
        /// </summary>
        /// <param name="inParameterName"> Set the parameter to this string. </param>
        /// <param name="inDefaultValue"> Set the parameter to this value. </param>
        public void SetParameterDouble(string inParameterName, double inDefaultValue)
        {
            _defaultValue = inDefaultValue;
            _parameterName = inParameterName;
            Create();
        }

        /// <summary>
        /// Creates a new parameter of type double in the PHASE engine.
        /// </summary>
        /// <returns> True if succesful, false otherwise. </returns>
        public override bool Create()
        {
            if (_parameterId != Helpers.InvalidId) return true;
            _parameterId = Helpers.PHASECreateSoundEventParameterDbl(_parameterName, _defaultValue, _minimumValue, _maximumValue);
            if (_parameterId == Helpers.InvalidId)
            {
                Debug.Log("Failed to create sound event meta parameter");
                return false;
            }
            return true;
        }
    }

}