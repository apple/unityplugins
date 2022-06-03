using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;

namespace Apple.PHASE
{
    /// <summary>
    /// Class representing a sampler node in the PHASE engine.
    /// </summary>
    [NodeWidth(250)]
    public class PHASESoundEventSamplerNode : PHASESoundEventNode
    {
        /// <summary>
        /// An array of valid audio file extensions supported by PHASE.
        /// </summary>
        public static string[] m_validAudioExtensions = { ".wav", ".ogg", ".aiff", ".aif", ".mp3", "m4a", "caf", "flac", "w64", "ac3" };

        /// <summary>
        /// Set true to stream audio files from within the StreamingAssets directory.
        /// </summary>
        [SerializeField] private bool _isStreamingAsset = false;
        /// <summary>
        /// The <c>AudioClip</c> to be played by this sampler node.
        /// </summary>
        [SerializeField] private AudioClip _audioClip = null;
        /// <summary>
        /// Set true to loop the audio played by this sampler node.
        /// </summary>
        [SerializeField] private bool _looping = true;
        /// <summary>
        /// The Calibration Mode of this sampler node: None, RelativeSPL or AbsoluteSPL.
        /// </summary>
        [SerializeField] private Helpers.CalibrationMode _calibrationMode = Helpers.CalibrationMode.RelativeSpl;
        /// <summary>
        /// The level used in CalibrationMode.None.
        /// </summary>
        [Range(0,1)]
        [SerializeField] private float _levelNone = 1;
        /// <summary>
        /// The level used in CalibrationMode.RelativeSpl.
        /// </summary>
        [Range(-200, 12)]
        [SerializeField] private float _levelRelativeSpl = 0;
        /// <summary>
        /// The level used in CalibrationMode.AbsoluteSpl.
        /// </summary>
        [Range(0, 120)]
        [SerializeField] private float _levelAbsoluteSpl = 85;
        [SerializeField] private PHASEMixer _mixer = null;
        /// <summary>
        /// The <c>PHASEMixer</c> associated with this sampler node.
        /// </summary>
        [Output] public PHASEMixer Mixer = null;
        [SerializeField] private Object _streamingAssetAudioClip;
        [SerializeField] private string _assetName = "";
        [SerializeField] private string _streamingAssetSubDirectory = "";

        static List<string> _registeredAssets = new List<string>();

        /// <summary>
        /// Create this sampler node in the PHASE engine.
        /// </summary>
        /// <returns> Return true on success, false otherwise. </returns>
        public override bool Create()
        {
            _mixer = GetMixerFromPort();
            if (_mixer == null)
            {
                Debug.LogError($"Cannot register PHASESamplerNode named: {name} with no valid mixer.");
                return false;
            }

            if (_isStreamingAsset)
            {
                string path = Path.Combine(Application.streamingAssetsPath, _streamingAssetSubDirectory);

                bool result = Helpers.PHASERegisterAudioFile(_assetName, path);
                if (result == false)
                {
                    Debug.LogError($"Failed to register PHASE audio file: {_assetName} for node: {name}.");
                    return false;
                }
                _registeredAssets.Add(_assetName);
            }
            else
            {
                _assetName = _audioClip.name;
                // Load audio data and register it to the runtime.
                if (!_registeredAssets.Contains(_assetName))
                {
                    _audioClip.LoadAudioData();
                    float[] samples = new float[_audioClip.samples * _audioClip.channels];
                    _audioClip.GetData(samples, 0);
                    GCHandle gcAudioClip = GCHandle.Alloc(samples, GCHandleType.Pinned);
                    uint dataSizeInBytes = (uint)(_audioClip.samples * _audioClip.channels * sizeof(float));
                    uint bitDepth = 32;
                    uint channelCount = (uint)_audioClip.channels;
                    uint sampleRate = (uint)_audioClip.frequency;
                    bool result = Helpers.PHASERegisterAudioBuffer(_assetName, gcAudioClip.AddrOfPinnedObject(), sampleRate, dataSizeInBytes, bitDepth, channelCount);
                    if (result == false)
                    {
                        Debug.LogError($"Failed to register PHASE audio buffer from AudioClip: {_assetName} for node {name}.");
                        return false;
                    }

                    // Free the handle and unload the data as its been copied underneath
                    gcAudioClip.Free();
                    _audioClip.UnloadAudioData();
                    _registeredAssets.Add(_assetName);
                }
            }

            // Create sampler node
            m_nodeId = Helpers.PHASECreateSoundEventSamplerNode(_assetName, _mixer.GetMixerId(), _looping, _calibrationMode, GetLevel());
            if (m_nodeId == Helpers.InvalidId)
            {
                Debug.LogError($"Failed to create PHASE sampler node: {name}.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Set the subdirectory of the StreamingAssets directory where the streaming audio asset is found.
        /// </summary>
        /// <param name="subdirectory"></param>
        public void SetSubDirectory(string subdirectory)
        {
            _streamingAssetSubDirectory = subdirectory;
        }

        /// <summary>
        /// Set the name of the streaming audio asset.
        /// </summary>
        /// <param name="newName"></param>
        public void SetAssetName(string newName)
        {
            _assetName = newName;
        }

        /// <summary>
        /// Destroy this sampler node from the PHASE engine.
        /// </summary>
        public override void DestroyFromPHASE()
        {
            Helpers.PHASEUnregisterAudioAsset(_assetName);
            _registeredAssets.Remove(_assetName);
            _mixer.DestroyFromPHASE();
            base.DestroyFromPHASE();
        }

        /// <summary>
        /// Return a <c>List</c> of all mixers associated with this sampler node.
        /// </summary>
        /// <returns>A <c>List</c> of all mixers associated with this sampler node.</returns>
        public override List<PHASEMixer> GetMixers()
        {
            _mixer = GetMixerFromPort();
            if (_mixer != null)
            {
                return new List<PHASEMixer> { _mixer };
            }
            else
            {
                return new List<PHASEMixer>();
            }
        }

        private float GetLevel()
        {
            switch (_calibrationMode)
            {
                case Helpers.CalibrationMode.None:
                    return _levelNone;
                case Helpers.CalibrationMode.RelativeSpl:
                    return _levelRelativeSpl;
                case Helpers.CalibrationMode.AbsoluteSpl:
                    return _levelAbsoluteSpl;
                default:
                    return 0f;
            }
        }
    }
}