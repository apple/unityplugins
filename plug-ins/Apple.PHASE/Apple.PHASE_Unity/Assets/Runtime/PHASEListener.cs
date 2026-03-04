using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Apple.PHASE
{
    /// <summary>
    /// Represents a listener in the PHASE engine.
    /// </summary>
    /// <remarks> Only one listener can exist at a time. </remarks>
    [DisallowMultipleComponent]
    public class PHASEListener : MonoBehaviour
    {
        // Transform to store.
        private Transform _transform;

        // List of mixers.
        private List<PHASEMixer> _mixers = new List<PHASEMixer>();

        /// <summary>
        /// Dynamically controlled gain scalar value of the listener
        /// </summary>
        [Range(0.0f, 1.0f)]
        [SerializeField] private double _gain = 1.0;

        /// <summary>
        /// Global default reverb setting.
        /// </summary>
        [SerializeField] private Helpers.ReverbPresets _reverbPreset = Helpers.ReverbPresets.MediumRoom;
        private Helpers.ReverbPresets _lastReverb;

        /// <summary>
        /// Automatic Listener Head-Tracking Enabled
        /// </summary>
        [SerializeField] private bool _automaticHeadTrackingEnabled = false;
        private bool _lastAutomaticHeadTrackingEnabled;

        // Is this listener registered with PHASE?
        private bool _registered = false;

        // Awake is called before the scene starts.
        void Awake()
        {
            bool result = Helpers.PHASEStart();
            if (result == false)
            {
                Debug.LogError("Failed to start PHASE Engine");
            }

            CreateListener();
            
            Helpers.PHASESetListenerHeadTracking(_automaticHeadTrackingEnabled);

            // Store the transform object (transform itself gets updated).
            _transform = GetComponent<Transform>();

            SetupReverb();
        }

        void CreateListener()
        {
            // Create the PHASE Listener.
            _registered = Helpers.PHASECreateListener();
            if (_registered == false)
            {
                Debug.LogError("Failed to create PHASE Listener");
            }
        }

        // Update is called once per frame.
        void LateUpdate()
        {
            if (_registered)
            {
                if (_transform != null)
                {
                    // Transpose position to a row matrix and convert the matrix to right-handed.
                    Matrix4x4 phaseTransform = Helpers.GetPhaseTransform(_transform);
                    bool result = Helpers.PHASESetListenerTransform(phaseTransform);
                    if (result == false)
                    {
                        Debug.LogError("Failed to set transform on listener");
                    }
                }
                
                UpdateListener();

                UpdateGain();

                PHASESource.UpdateSources();

                Helpers.PHASEUpdate();
            }
        }

        // Stop is called when the object stops.
        void OnDestroy()
        {
            if (_registered)
            {
                bool result = Helpers.PHASEDestroyListener();
                if (result == false)
                {
                    Debug.LogError("Failed to destroy PHASE Listener");
                }
                else
                {
                    _registered = false;
                }
            }
        }

        void OnApplicationQuit()
        {
            PHASESoundEventNodeGraph.UnregisterAll();
            Helpers.PHASEStop();
        }

        /// <summary>
        /// Give the listener access to a list of mixers.
        /// </summary>
        /// <param name="mixers"> A <c>List</c> of <c>PHASEMixer</c>es.</param>
        /// <remarks> This method is used for listener directivity visualization. </remarks>
        public void AddMixers(List<PHASEMixer> mixers)
        {
            if (mixers != null)
            {
                _mixers.AddRange(mixers);
            }
        }

        private void OnDisable()
        {
            OnDestroy();
        }

        private void OnEnable()
        {
            if (_registered == false)
            {
                CreateListener();
            }
        }

        void SetupReverb()
        {
            // Set the reverb preset in the scene.
            _lastReverb = _reverbPreset;
            Helpers.PHASESetSceneReverbPreset((int)_reverbPreset);
        }

        private void UpdateGain()
        {
            var result = Helpers.PHASESetListenerGain(_gain);
            if (result == false)
            {
                Debug.LogError("Failed to set gain on listener");
            }
        }

        /// <summary>
        /// Get the gain scalar value.
        /// </summary>
        /// <returns> A <c>double</c> representing the gain scalar value </returns>
        public double GetGain()
        {
            return Helpers.PHASEGetListenerGain();
        }

        /// <summary>
        /// Set the gain scalar value.
        /// </summary>
        /// <param name="gain"> The value of the new gain. </param>
        public void SetGain(double gain)
        {
            if (gain < 0.0 || gain > 1.0)
            {
                Debug.LogWarning("Listener gain {gain} you are trying to set is not within [0, 1], value will be clamped to valid range in PHASE Engine.");
            }
            _gain = gain;

            UpdateGain();
        }

        /// <summary>
        /// Set the global default reverb to the given preset.
        /// </summary>
        /// <param name="preset"> The value of the new global reverb preset. </param>
        public void SetReverbPreset(Helpers.ReverbPresets preset)
        {
            _reverbPreset = preset;
            if (_lastReverb != _reverbPreset)
            {
                _lastReverb = _reverbPreset;
                Helpers.PHASESetSceneReverbPreset((int)_reverbPreset);
            }
        }

        /// <summary>
        /// Set the automatic listener head-tracking to the given value.
        /// </summary>
        /// <param name="automaticHeadTrackingEnabled"> Is automaticHeadTrackingEnabled. </param>
        public void SetAutomaticListenerHeadTracking(bool automaticHeadTrackingEnabled)
        {
            _lastAutomaticHeadTrackingEnabled = _automaticHeadTrackingEnabled;
            _automaticHeadTrackingEnabled = automaticHeadTrackingEnabled;
            bool result = Helpers.PHASESetListenerHeadTracking(_automaticHeadTrackingEnabled);
            if (result == false)
            {
                Debug.LogError("Failed to set listener head-tracking.");
            }
            else
            {
                Debug.Log($"Set automatic listener head-tracking: {_automaticHeadTrackingEnabled}");
            }
        }

        /// <summary>
        /// Get the current automatic listener head-tracking value.
        /// </summary>
        /// <returns> A <c>bool</c> representing if head-tracking is enabled. </returns>
        public bool GetAutomaticListenerHeadTracking()
        {
            return _automaticHeadTrackingEnabled;
        }

        private void UpdateListener()
        {
            if (_lastAutomaticHeadTrackingEnabled != _automaticHeadTrackingEnabled)
            {
                SetAutomaticListenerHeadTracking(_automaticHeadTrackingEnabled);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach (PHASEMixer entry in _mixers)
            {
                if (entry is PHASESpatialMixer)
                {
                    PHASESpatialMixer mixer = entry as PHASESpatialMixer;
                    if (Selection.Contains(mixer.GetInstanceID()))
                    {
                        Helpers.DirectivityModelSubbandParameters subbandParameters = mixer.GetListenerDirectivityModelSubbandParameters();
                        switch (mixer.GetListenerDirectivityType())
                        {
                            case Helpers.DirectivityType.None:
                                break;
                            case Helpers.DirectivityType.Cone:
                                DrawConeWithParameters(subbandParameters);
                                break;
                            case Helpers.DirectivityType.Cardioid:
                                DrawCardioidWithParameters(subbandParameters);
                                break;

                        }
                    }
                }
            }
        }

        private void DrawConeWithParameters(Helpers.DirectivityModelSubbandParameters parameters)
        {
            Color innerConeColor = new Color(0f, 0f, 0.7f, 0.7f);
            Mesh innerCone = PHASEDirectivityVisualization.GenerateArcMesh(Mathf.Deg2Rad * parameters.InnerAngle);
            innerCone.name = "innerCone";
            Gizmos.color = innerConeColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawMesh(innerCone);

            Color outerConeColor = new Color(0f, 0f, 1f, 0.5f);
            Mesh outerCone = PHASEDirectivityVisualization.GenerateArcMesh(Mathf.Deg2Rad * parameters.OuterAngle);
            outerCone.name = "outerCone";
            Gizmos.color = outerConeColor;
            Gizmos.DrawMesh(outerCone);
        }

        private void DrawCardioidWithParameters(Helpers.DirectivityModelSubbandParameters parameters)
        {
            Color cardioidColor = new Color(0f, 0f, 1f, 0.5f);
            Gizmos.color = cardioidColor;

            Mesh cardioid = PHASEDirectivityVisualization.GenerateCardioidMesh(parameters.Pattern, parameters.Sharpness);
            cardioid.name = "cardioid";
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawMesh(cardioid);
        }
#endif
    }
}
