using UnityEngine;

namespace Apple.PHASE
{
    /// <summary>
    /// Class responsible for managing parameters of a PHASE spatial mixer.
    /// </summary>
    [NodeWidth(250)]
    public class PHASESpatialMixer : PHASEMixer
    {
        // Modelers to enable for rendering.
        /// <summary>
        /// Set true to enable the direct path modeler.
        /// </summary>
        [SerializeField] private bool _directPathModeler = true;
        /// <summary>
        /// Set true to enable the early reflections modeler.
        /// </summary>
        [SerializeField] private bool _earlyReflectionModeler = true;
        /// <summary>
        /// Set true to enable the late reverb modeler.
        /// </summary>
        [SerializeField] private bool _lateReverbModeler = true;

        /// <summary>
        /// The distance at which the sound is no longer heard. A value of 0 disables culling.
        /// </summary>
        [Min(0.0f)]
        [SerializeField] private float _cullDistance = 0.0f;

        /// <summary>
        /// The directivity shape for the listener.
        /// </summary>
        [SerializeField] private Helpers.DirectivityPreset _listenerDirectivityPreset = Helpers.DirectivityPreset.None;
        [Range(1f, 4f)]
        [SerializeField] private float _listenerDirectivitySharpness = 1f;
        [Range(0f, 360f)]
        [SerializeField] private float _listenerDirectivityInnerAngle = 90f;
        [Range(0f, 360f)]
        [SerializeField] private float _listenerDirectivityOuterAngle = 215f;

        /// <summary>
        /// The directivity shape for sources associated with this mixer.
        /// </summary>
        [SerializeField] private Helpers.DirectivityPreset _sourceDirectivityPreset = Helpers.DirectivityPreset.None;
        [Range(1f, 4f)]
        [SerializeField] private float _sourceDirectivitySharpness = 1f;
        [Range(0f, 360f)]
        [SerializeField] private float _sourceDirectivityInnerAngle = 90f;
        [Range(0f, 360f)]
        [SerializeField] private float _sourceDirectivityOuterAngle = 215f;

        private float[] _subbandFrequencies = { 200f, 1500f, 5000f };

        /// <summary>
        /// Creates the spatial mixer in the PHASE engine if it doesn't already exist and returns the mixer ID.
        /// </summary>
        /// <returns> A long representing the unique lower level ID of this mixer.</returns>
        override public long GetMixerId()
        {
            if (_mixerId == Helpers.InvalidId)
            {
                // Create source directivity model parameters.
                Helpers.DirectivityModelParameters sourceDirectivityModelParameters = new Helpers.DirectivityModelParameters();
                sourceDirectivityModelParameters.DirectivityType = GetDirectivityTypeFromPreset(_sourceDirectivityPreset);
                if (sourceDirectivityModelParameters.DirectivityType != Helpers.DirectivityType.None)
                {
                    int sourceSubbandCount = 3;
                    sourceDirectivityModelParameters.SubbandCount = sourceSubbandCount;
                    sourceDirectivityModelParameters.SubbandParameters = new Helpers.DirectivityModelSubbandParameters[sourceSubbandCount];
                    for (int i = 0; i < sourceSubbandCount; i++)
                    {
                        sourceDirectivityModelParameters.SubbandParameters[i] = GetSubbandParametersFromPreset(_sourceDirectivityPreset);
                        sourceDirectivityModelParameters.SubbandParameters[i].Frequency = _subbandFrequencies[i];
                        switch (sourceDirectivityModelParameters.DirectivityType)
                        {
                            case Helpers.DirectivityType.Cardioid:
                                sourceDirectivityModelParameters.SubbandParameters[i].Sharpness = _sourceDirectivitySharpness;
                                break;
                            case Helpers.DirectivityType.Cone:
                                sourceDirectivityModelParameters.SubbandParameters[i].InnerAngle = _sourceDirectivityInnerAngle;
                                sourceDirectivityModelParameters.SubbandParameters[i].OuterAngle = _sourceDirectivityOuterAngle;
                                break;
                        }
                    }
                }
                else
                {
                    sourceDirectivityModelParameters.SubbandCount = 0;
                    sourceDirectivityModelParameters.SubbandParameters = null;
                }

                // Create listener directivity model parameters.
                Helpers.DirectivityModelParameters listenerDirectivityModelParameters = new Helpers.DirectivityModelParameters();
                listenerDirectivityModelParameters.DirectivityType = GetDirectivityTypeFromPreset(_listenerDirectivityPreset);
                if (listenerDirectivityModelParameters.DirectivityType != Helpers.DirectivityType.None)
                {
                    int listenerSubbandCount = 3;
                    listenerDirectivityModelParameters.SubbandParameters = new Helpers.DirectivityModelSubbandParameters[listenerSubbandCount];
                    listenerDirectivityModelParameters.SubbandCount = listenerSubbandCount;
                    for (int i = 0; i < listenerSubbandCount; i++)
                    {
                        listenerDirectivityModelParameters.SubbandParameters[i] = GetSubbandParametersFromPreset(_listenerDirectivityPreset);
                        listenerDirectivityModelParameters.SubbandParameters[i].Frequency = _subbandFrequencies[i];
                        switch (listenerDirectivityModelParameters.DirectivityType)
                        {
                            case Helpers.DirectivityType.Cardioid:
                                listenerDirectivityModelParameters.SubbandParameters[i].Sharpness = _listenerDirectivitySharpness;
                                break;
                            case Helpers.DirectivityType.Cone:
                                listenerDirectivityModelParameters.SubbandParameters[i].InnerAngle = _listenerDirectivityInnerAngle;
                                listenerDirectivityModelParameters.SubbandParameters[i].OuterAngle = _listenerDirectivityOuterAngle;
                                break;
                        }
                    }
                }
                else
                {
                    listenerDirectivityModelParameters.SubbandCount = 0;
                    listenerDirectivityModelParameters.SubbandParameters = null;
                }

                // Create spatial mixer.
                _mixerId = Helpers.PHASECreateSpatialMixer(name, _directPathModeler, _earlyReflectionModeler, _lateReverbModeler, _cullDistance, sourceDirectivityModelParameters, listenerDirectivityModelParameters);

                if (_mixerId == Helpers.InvalidId)
                {
                    Debug.LogError("Failed to create PHASE spatial mixer.");
                }
            }
            return _mixerId;
        }

        private Helpers.DirectivityModelSubbandParameters GetSubbandParametersFromPreset(Helpers.DirectivityPreset preset)
        {
            Helpers.DirectivityModelSubbandParameters subband = new Helpers.DirectivityModelSubbandParameters();
            switch (preset)
            {
                case Helpers.DirectivityPreset.Omni:
                    subband.Pattern = 0f;
                    break;
                case Helpers.DirectivityPreset.Cone:
                    subband.OuterGain = 1f;
                    break;
                case Helpers.DirectivityPreset.Cardioid:
                    subband.Pattern = 0.5f;
                    break;
                case Helpers.DirectivityPreset.Hypercardioid:
                    subband.Pattern = 0.75f;
                    break;
                case Helpers.DirectivityPreset.Hypocardioid:
                    subband.Pattern = 0.25f;
                    break;
                case Helpers.DirectivityPreset.FigureEight:
                    subband.Pattern = 1f;
                    break;

            }
            return subband;
        }

        private Helpers.DirectivityType GetDirectivityTypeFromPreset(Helpers.DirectivityPreset preset)
        {
            Helpers.DirectivityType directivityType = Helpers.DirectivityType.None;
            switch (preset)
            {
                case Helpers.DirectivityPreset.None:
                    directivityType = Helpers.DirectivityType.None;
                    break;
                case Helpers.DirectivityPreset.Omni:
                    directivityType = Helpers.DirectivityType.Cardioid;
                    break;
                case Helpers.DirectivityPreset.Cone:
                    directivityType = Helpers.DirectivityType.Cone;
                    break;
                case Helpers.DirectivityPreset.Cardioid:
                    directivityType = Helpers.DirectivityType.Cardioid;
                    break;
                case Helpers.DirectivityPreset.Hypercardioid:
                    directivityType = Helpers.DirectivityType.Cardioid;
                    break;
                case Helpers.DirectivityPreset.Hypocardioid:
                    directivityType = Helpers.DirectivityType.Cardioid;
                    break;
                case Helpers.DirectivityPreset.FigureEight:
                    directivityType = Helpers.DirectivityType.Cardioid;
                    break;
            }
            return directivityType;
        }

        internal Helpers.DirectivityType GetSourceDirectivityType()
        {
            return GetDirectivityTypeFromPreset(_sourceDirectivityPreset);
        }

        internal Helpers.DirectivityType GetListenerDirectivityType()
        {
            return GetDirectivityTypeFromPreset(_listenerDirectivityPreset);
        }

        internal Helpers.DirectivityModelSubbandParameters GetSourceDirectivityModelSubbandParameters()
        {
            Helpers.DirectivityModelSubbandParameters subbandParameters;
            subbandParameters = GetSubbandParametersFromPreset(_sourceDirectivityPreset);
            switch (GetSourceDirectivityType())
            {
                case Helpers.DirectivityType.Cardioid:
                    subbandParameters.Sharpness = _sourceDirectivitySharpness;
                    break;
                case Helpers.DirectivityType.Cone:
                    subbandParameters.InnerAngle = _sourceDirectivityInnerAngle;
                    subbandParameters.OuterAngle = _sourceDirectivityOuterAngle;
                    break;
            }
            return subbandParameters;
        }

        internal Helpers.DirectivityModelSubbandParameters GetListenerDirectivityModelSubbandParameters()
        {
            Helpers.DirectivityModelSubbandParameters subbandParameters;
            subbandParameters = GetSubbandParametersFromPreset(_listenerDirectivityPreset);
            switch (GetListenerDirectivityType())
            {
                case Helpers.DirectivityType.Cardioid:
                    subbandParameters.Sharpness = _listenerDirectivitySharpness;
                    break;
                case Helpers.DirectivityType.Cone:
                    subbandParameters.InnerAngle = _listenerDirectivityInnerAngle;
                    subbandParameters.OuterAngle = _listenerDirectivityOuterAngle;
                    break;
            }
            return subbandParameters;
        }
    }
}