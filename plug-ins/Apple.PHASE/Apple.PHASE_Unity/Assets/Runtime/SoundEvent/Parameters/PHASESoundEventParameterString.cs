using UnityEngine;

namespace Apple.PHASE
{
    /// <summary>
    /// Sound event parameter of type string.
    /// </summary>
    public class PHASESoundEventParameterString : PHASESoundEventParameter
    {
        /// <summary>
        /// The parent node of this parameter.
        /// </summary>
        [Input(typeConstraint = TypeConstraint.Strict)] public PHASESoundEventParameterString ParentNode = null;

        // Default string value.
        [SerializeField] private string _defaultValue;
        public string DefaultValue
        {
            get => _defaultValue;
        }

        /// <summary>
        /// Sets the parameter to the given name and value.
        /// </summary>
        /// <param name="inParameterName"> Set the parameter to this string. </param>
        /// <param name="inDefaultValue"> Set the parameter to this value. </param>
        public void SetParameterString(string inParameterName, string inDefaultValue)
        {
            _defaultValue = inDefaultValue;
            _parameterName = inParameterName;
            Create();
        }

        /// <summary>
        /// Creates a new parameter of type string in the PHASE engine.
        /// </summary>
        /// <returns> True if succesful, false otherwise. </returns>
        public override bool Create()
        {
            if (_parameterId != Helpers.InvalidId) return true;
            _parameterId = Helpers.PHASECreateSoundEventParameterStr(_parameterName, _defaultValue);
            if (_parameterId == Helpers.InvalidId)
            {
                Debug.Log("Failed to create sound event meta parameter");
                return false;
            }
            return true;
        }
    }
}