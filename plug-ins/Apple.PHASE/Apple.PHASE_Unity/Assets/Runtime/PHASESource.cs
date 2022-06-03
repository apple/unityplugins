using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using AOT;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Apple.PHASE
{
    /// <summary>
    /// Class representing a source in the PHASE engine.
    /// </summary>
    public class PHASESource : MonoBehaviour
    {
        private bool _toBeDestroyed = false;

        /// <summary>
        /// Sound event asset to play.
        /// </summary>
        [SerializeField] private PHASESoundEventNodeGraph _soundEvent = null;

        // Is this a volumetric or point source?
        enum SourceMode
        {
            VolumetricSource,
            PointSource
        }
        /// <summary>
        /// Selects whether this is a volumetric or point source.
        /// </summary>
        [SerializeField] private SourceMode _sourceMode = SourceMode.VolumetricSource;

        /// <summary>
        /// Direct Path send value.
        /// </summary>
        [Range(0.0f, 1.0f)]
        public float DirectPathSend = 1.0f;

        /// <summary>
        /// Early reflections send value.
        /// </summary>
        [Range(0.0f, 1.0f)]
        public float EarlyReflectionsSend = 0.2f;

        /// <summary>
        /// Late reverberation send value. 
        /// </summary>
        [Range(0.0f, 1.0f)]
        public float LateReverbSend = 0.2f;

        /// <summary>
        /// When true, this source will play when Awake() is called.
        /// </summary>
        [SerializeField] private bool _playOnAwake = true;

        // Active action tree instance on this source.
        private List<long> _soundEventInstance = new List<long>();

        // Source id to store.
        private long _sourceId = Helpers.InvalidId;

        private static Dictionary<long, PHASESource> _registeredSources = new Dictionary<long, PHASESource>();

        // Transform to use.
        private Transform _transform;

        // Mesh to use.
        private MeshFilter _mesh;

        // Dictionary of mixers related to this source's sound event.
        private List<PHASEMixer> _mixers = new List<PHASEMixer>();

        // Reference to PHASEListener in scene.
        private static PHASEListener _listener = null;

        // Awake is called before the scene starts.
        void Awake()
        {
            _transform = GetComponent<Transform>();
            _mesh = GetComponent<MeshFilter>();

            if (_mesh != null && _sourceMode == SourceMode.VolumetricSource)
            {
                Helpers.MeshData meshData = Helpers.GetMeshData(_mesh, _transform);

                // Pin the arrays so we can pass these as pointers to the plugin.
                GCHandle gcVertices = GCHandle.Alloc(meshData.Vertices, GCHandleType.Pinned);
                GCHandle gcNormals = GCHandle.Alloc(meshData.Normals, GCHandleType.Pinned);
                GCHandle gcIndices = GCHandle.Alloc(meshData.Indices, GCHandleType.Pinned);

                // Create a source.
                _sourceId = Helpers.PHASECreateVolumetricSource(meshData.VertexCount, gcVertices.AddrOfPinnedObject(), gcNormals.AddrOfPinnedObject(), meshData.IndexCount, gcIndices.AddrOfPinnedObject());

                // Release the pinned data.
                gcVertices.Free();
                gcNormals.Free();
                gcIndices.Free();
            }
            else
            {
                _sourceId = Helpers.PHASECreatePointSource();
            }

            if (_sourceId == Helpers.InvalidId)
            {
                Debug.LogError("Failed to create PHASE Source");
            }
            else
            {
                _registeredSources.Add(_sourceId, this);
            }
        }

        // Start is called before the first frame update.
        private void Start()
        {
            if (_playOnAwake)
            {
                Play();
            }
        }

        [MonoPInvokeCallback(typeof(Helpers.PHASESoundEventCompletionCallback))]
        static void SoundEventCallback(Helpers.PHASESoundEventStartHandlerReason reason, long sourceId, long soundEventId)
        {
            PHASESource source;
            if (_registeredSources.TryGetValue(sourceId, out source))
            {
                source.CompletionHandler(reason, soundEventId);
            }
            else
            {
                Debug.LogError($"Unable to find source with id {sourceId} for callback with reason: {reason}.");
            }
        }

        void CompletionHandler(Helpers.PHASESoundEventStartHandlerReason reason, long soundEventId)
        {
            switch (reason)
            {
                case Helpers.PHASESoundEventStartHandlerReason.PHASESoundEventStartHandlerReasonError:
                    Debug.LogError($"Error in PHASESource {name} with ID {_sourceId}");
                    break;
                case Helpers.PHASESoundEventStartHandlerReason.PHASESoundEventStartHandlerReasonFinishedPlaying:
                    break;
                case Helpers.PHASESoundEventStartHandlerReason.PHASESoundEventStartHandlerReasonTerminated:
                    Debug.Log("PHASESource was killed");
                    break;
            }
            if (_toBeDestroyed && _registeredSources.Count == 1)
            {
                _registeredSources.Remove(_sourceId);
            }
            _soundEventInstance.Remove(soundEventId);
        }

        /// <summary>
        /// Play the sound event on this source.
        /// </summary>
        public void Play()
        {
            if (_soundEvent == null)
            {
                Debug.LogError("Invalid PHASESoundEvent on PHASESource.");
                return;
            }

            if (!_soundEvent.IsRegistered())
            {
                _soundEvent.Register();
            }

            _mixers = _soundEvent.GetMixers();
            long[] mixerIds = GetMixerIds();

            var instanceId = Helpers.PHASEPlaySoundEvent(_soundEvent.name, _sourceId, mixerIds, (uint)mixerIds.Length, SoundEventCallback);
            if (instanceId == Helpers.InvalidId)
            {
                Debug.LogError($"Failed to play sound event: {_soundEvent.name}.");
            }
            else
            {
                _soundEventInstance.Add(instanceId);
                SetSendParameters(instanceId);
            }
        }

        private long[] GetMixerIds()
        {
            List<long> mixerIds = new List<long>();
            foreach (PHASEMixer entry in _mixers)
            {
                mixerIds.Add(entry.GetMixerId());
            }
            return mixerIds.ToArray();
        }

        /// <summary>
        /// Stop the playing sound event on this source. 
        /// </summary>
        public void Stop()
        {
            List<long> toBeDeleted = new List<long>();
            if (IsPlaying())
            {
                foreach (long instanceId in _soundEventInstance)
                {
                    bool result = Helpers.PHASEStopSoundEvent(instanceId);
                    if (result == false)
                    {
                        Debug.LogError("Failed to stop sound event instance.");
                    }

                    toBeDeleted.Add(instanceId);
                }
                _mixers = null;

                foreach (long instanceId in toBeDeleted)
                {
                    _soundEventInstance.Remove(instanceId);
                }
            }
        }

        /// <summary>
        /// Check if the sound event instance is playing.
        /// </summary>
        /// <returns> True if the sound event is playing, false otherwise. </returns>
        public bool IsPlaying()
        {
            return _soundEventInstance.Count > 0;
        }

        protected virtual void ManualUpdate()
        {
            if (_transform != null && _sourceId != Helpers.InvalidId)
            {
                Matrix4x4 phaseTransform = Helpers.GetPhaseTransform(_transform);
                bool result = Helpers.PHASESetSourceTransform(_sourceId, phaseTransform);
                if (result == false)
                {
                    Debug.LogError("Failed to set transform on source " + _sourceId);
                }
            }

            foreach (long instanceId in _soundEventInstance)
            {
                SetSendParameters(instanceId);
            }
        }

        protected internal static void UpdateSources()
        {
            foreach (KeyValuePair<long, PHASESource> entry in _registeredSources)
            {
                entry.Value.ManualUpdate();
            }
        }

        // Adjust send parameters to modelers.
        void SetSendParameters(long instanceId)
        {
            if (_mixers != null)
            {
                foreach (PHASEMixer entry in _mixers)
                {
                    if (entry is PHASESpatialMixer)
                    {
                        SetSendParametersForSpatialMixer(entry.name, instanceId);
                    }
                }
            }
        }

        void SetSendParametersForSpatialMixer(string mixer_name, long instanceId)
        {
            bool result = Helpers.PHASESetSoundEventParameterDbl(instanceId, mixer_name + "DirectPathSend", DirectPathSend);
            if (result == false)
            {
                Debug.LogError($"Failed to set direct path send on sound event {instanceId}.");
            }

            result = Helpers.PHASESetSoundEventParameterDbl(instanceId, mixer_name + "EarlyReflectionsSend", EarlyReflectionsSend);
            if (result == false)
            {
                Debug.LogError($"Failed to set early reflections send on sound event {instanceId}.");
            }

            result = Helpers.PHASESetSoundEventParameterDbl(instanceId, mixer_name + "LateReverbSend", LateReverbSend);
            if (result == false)
            {
                Debug.LogError($"Failed to set late reverb send on sound event {instanceId}.");
            }
        }

        /// <summary>
        /// Set a meta parameter of type integer associated with this source's sound event.
        /// </summary>
        /// <param name="inParamName"> Name of the parameter to set. </param>
        /// <param name="inValue"> Value to set the parameter to. </param>
        public void SetMetaParameterValue(string inParamName, int inValue)
        {
            foreach (long instanceId in _soundEventInstance)
            {
                bool result = Helpers.PHASESetSoundEventParameterInt(instanceId, inParamName, inValue);
                if (result == false)
                {
                    Debug.LogError($"Failed to set meta parameter {inParamName}  with value {inValue} on sound event {instanceId}.");
                }
            }
        }

        /// <summary>
        /// Set a meta parameter of type double associated with this source's sound event.
        /// </summary>
        /// <param name="inParamName"> Name of the parameter to set. </param>
        /// <param name="inValue"> Value to set the parameter to. </param>
        public void SetMetaParameterValue(string inParamName, double inValue)
        {
            foreach (long instanceId in _soundEventInstance)
            {
                bool result = Helpers.PHASESetSoundEventParameterDbl(instanceId, inParamName, inValue);
                if (result == false)
                {
                    Debug.LogError($"Failed to set meta parameter {inParamName} with value {inValue} on sound event {instanceId}.");
                }
            }
        }

        /// <summary>
        /// Set a meta parameter of type string associated with this source's sound event.
        /// </summary>
        /// <param name="inParamName"> Name of the parameter to set. </param>
        /// <param name="inValue"> Value to set the parameter to. </param>
        public void SetMetaParameterValue(string inParamName, string inValue)
        {
            foreach (long instanceId in _soundEventInstance)
            {
                bool result = Helpers.PHASESetSoundEventParameterStr(instanceId, inParamName, inValue);
                if (result == false)
                {
                    Debug.LogError($"Failed to set meta parameter {inParamName} with value {inValue} on sound event {instanceId}.");
                }
            }
        }

        /// <summary>
        /// Destroy this source from the PHASE engine.
        /// </summary>
        public void DestroyFromPHASE()
        {
            Stop();
            Helpers.PHASEDestroySource(_sourceId);
            _toBeDestroyed = true;
            _sourceId = Helpers.InvalidId;
        }

        // Stop is called when the object stops.
        void OnDestroy()
        {
            DestroyFromPHASE();
        }

        /// <summary>
        /// Return the unique ID of this source.
        /// </summary>
        /// <returns> The unique ID of this source. </returns>
        public long GetSourceId()
        {
            return _sourceId;
        }

        void OnDisable()
        {
            DestroyFromPHASE();
        }

        void OnEnable()
        {
            if (_sourceId == Helpers.InvalidId)
            {
                Awake();
                Start();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_soundEvent)
            {
                _mixers = _soundEvent.GetMixers();
            }
            if (_mixers == null)
            {
                return;
            }

            // The listener doesn't know about action trees or mixers,
            // So we need to send it the info it needs to draw the visualization.
            if (_listener == null)
            {
                PHASEListener[] listeners = FindObjectsOfType<PHASEListener>();
                if (listeners.Length > 0)
                {
                    // Only one listener per scene allowed.
                    _listener = listeners[0];
                }
            }

            if (_listener != null)
            {
                _listener.AddMixers(_mixers);
            }
            foreach (PHASEMixer entry in _mixers)
            {
                if (entry is PHASESpatialMixer)
                {
                    PHASESpatialMixer mixer = entry as PHASESpatialMixer;
                    if (Selection.Contains(mixer.GetInstanceID()))
                    {
                        Helpers.DirectivityModelSubbandParameters subbandParameters = mixer.GetSourceDirectivityModelSubbandParameters();
                        switch (mixer.GetSourceDirectivityType())
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