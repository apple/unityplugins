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

        /// <summary>
        /// Sets the parameter to the given name and value.
        /// </summary>
        /// <param name="inParameterName"> Set the parameter to this string. </param>
        /// <param name="inDefaultValue"> Set the parameter to this value. </param>
        public void SetParameterInteger(string inParameterName, int inDefaultValue)
        {
            _defaultValue = inDefaultValue;
            SetParameterName(inParameterName);
            Create();
        }

        /// <summary>
        /// Creates a new parameter of type integer in the PHASE engine.
        /// </summary>
        /// <returns> True if succesful, false otherwise. </returns>
        public override bool Create()
        {
            _parameterId = Helpers.PHASECreateSoundEventParameterInt(_parameterName, _defaultValue);
            if (_parameterId == Helpers.InvalidId)
            {
                Debug.Log("Failed to create action tree meta parameter");
                return false;
            }
            return true;
        }
    }
}