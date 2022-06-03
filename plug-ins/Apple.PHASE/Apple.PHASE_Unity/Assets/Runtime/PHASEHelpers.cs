using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace Apple.PHASE
{
    /// <summary>
    /// A helper class containing methods and data structures for communicating with the PHASE native plugin.
    /// </summary>
    static public class Helpers
    {
        /// <summary>
        /// The name of the plugin DLL based on which platform is being used.
        /// </summary>
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
        public const string PluginDllName = "AudioPluginPHASE";
#elif UNITY_IOS
        public const string PluginDllName = "__Internal";
#endif

        /// <summary>
        /// The value representing an invalid ID in the PHASE engine.
        /// </summary>
        /// <remarks> This value is returned as a failure on methods that return unique IDs from the PHASE engine. </remarks>
        static public long InvalidId = -1;

        #region PHASE global native plugin functions.
        /// <summary>
        /// Start the PHASE engine.
        /// </summary>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASEStart();

        /// <summary>
        /// Stop the PHASE engine.
        /// </summary>
        [DllImport(PluginDllName)] public static extern void PHASEStop();

        /// <summary>
        /// Manually update the PHASE engine.
        /// </summary>
        [DllImport(PluginDllName)] public static extern void PHASEUpdate();

        /// <summary>
        /// Set the global reverb preset in the PHASE engine.
        /// </summary>
        /// <param name="inPresetIndex"> The int value of the preset from the ReverbPreset enum.</param>
        /// <returns> True on success, false otherwise.</returns>
        /// <see cref="ReverbPresets"/>
        [DllImport(PluginDllName)] public static extern bool PHASESetSceneReverbPreset(int inPresetIndex);

        /// <summary>
        /// Enumeration containing all the supported reverb presets.
        /// </summary>
        public enum ReverbPresets
        {
            None = 0,
            Cathedral,
            LargeChamber,
            LargeHall,
            LargeRoom1,
            LargeRoom2,
            MechanicsHall,
            MediumChamber,
            MediumHall1,
            MediumHall2,
            MediumHall3,
            MediumRoom,
            SmallRoom
        };
        #endregion

        #region PHASE Source native plugin functions.
        /// <summary>
        /// Create a point source in the PHASE engine.
        /// </summary>
        /// <returns> The unique ID of the created source, returns <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreatePointSource();

        /// <summary>
        /// Create a volumetric source in the PHASE engine from given <c>Mesh</c> parameters.
        /// </summary>
        /// <param name="inVertCount"> Number of vertices in the <c>Mesh</c> representing this volumetric source. </param>
        /// <param name="inPositions"> Array of positions in the <c>Mesh</c> representing this volumetric source. </param>
        /// <param name="inNormals"> Array of normals in the <c>Mesh</c> representing this volumetric source. </param>
        /// <param name="inIndexCount"> Number of indices in the <c>Mesh</c> representing this volumetric source. </param>
        /// <param name="inIndices"> Array of indices in the <c>Mesh</c> representing this volumetric source. </param>
        /// <returns>Returns unique ID of the created source, returns <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreateVolumetricSource(int inVertCount, [In] IntPtr inPositions, [In] IntPtr inNormals, int inIndexCount, [In] IntPtr inIndices);

        /// <summary>
        /// Destroy the given source from the PHASE engine.
        /// </summary>
        /// <param name="inSourceId">The unique ID representing the source to destroy. </param>
        [DllImport(PluginDllName)] public static extern void PHASEDestroySource(long inSourceId);

        /// <summary>
        /// Set the transform of the source in the PHASE engine.
        /// </summary>
        /// <param name="inSourceId"> The unique ID representing the source. </param>
        /// <param name="inTransform"> A <c>Matrix4x4</c> representing the source's transform. </param>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASESetSourceTransform(long inSourceId, Matrix4x4 inTransform);
        #endregion

        #region PHASE Listener native plugin functions
        /// <summary>
        /// Create the listener in the PHASE engine.
        /// </summary>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASECreateListener();

        /// <summary>
        /// Destroy the listener from the PHASE engine.
        /// </summary>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASEDestroyListener();

        /// <summary>
        /// Set the transform of the listener in the PHASE engine.
        /// </summary>
        /// <param name="inTransform"> A <c>Matrix4x4</c> representing the listener's transform. </param>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASESetListenerTransform(Matrix4x4 inTransform);
        #endregion

        #region Mixer native plugin methods and data structures.
        /// <summary>
        /// Create an ambient mixer in the PHASE engine.
        /// </summary>
        /// <param name="inMixerName"> The unique name of the mixer to create. </param>
        /// <param name="inChannelLayout"> The channel layout of the mixer. </param>
        /// <param name="orientation"> A <c>PHASEQuaternion</c> representing the orientation of this mixer. </param>
        /// <returns> The unique ID of this mixer, returns <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreateAmbientMixer(string inMixerName, ChannelLayoutType inChannelLayout, PHASEQuaternion orientation);

        /// <summary>
        /// Create a channel mixer in the PHASE engine.
        /// </summary>
        /// <param name="inMixerName"> The unique name of the mixer to create. </param>
        /// <param name="inChannelLayout"> The channel layout of the mixer. </param>
        /// <returns> The unique ID of this mixer, returns <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreateChannelMixer(string inMixerName, ChannelLayoutType inChannelLayout);

        /// <summary>
        /// Create a spatial mixer in the PHASE engine.
        /// </summary>
        /// <param name="inMixerName"> The unique name of the mixer to create. </param>
        /// <param name="inEnableDirectPath"> Set true to enable the direct path modeler. </param>
        /// <param name="inEnableEarlyReflections"> Set true to enable the early reflections modeler. </param>
        /// <param name="inEnableLateReverb"> Set true to enable the late reverb modeler. </param>
        /// <param name="inCullDistance"> Value representing the distance at which the system no longer processes sound for this object. A value of 0 disables culling. </param>
        /// <param name="sourceDirectivityModelParameters"> Directivity parameters for associated sources.</param>
        /// <param name="listenerDirectivityModelParameters"> Directiviy parameters for the associated listener. </param>
        /// <returns> The unique ID of this mixer, returns <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreateSpatialMixer(string inMixerName, bool inEnableDirectPath, bool inEnableEarlyReflections, bool inEnableLateReverb, float inCullDistance, DirectivityModelParameters sourceDirectivityModelParameters, DirectivityModelParameters listenerDirectivityModelParameters);

        /// <summary>
        /// Destroy the given mixer from the PHASE engine.
        /// </summary>
        /// <param name="inMixerId"> The unique ID representing the mixer to destroy. </param>
        [DllImport(PluginDllName)] public static extern void PHASEDestroyMixer(long inMixerId);

        /// <summary>
        /// Struct representing a quaternion in PHASE.
        /// /// </summary>
        /// <remarks>
        /// PHASE coordinates.
        /// <list type="bullet">
        /// <item>
        /// <description> Right is in the positive X-axis direction. </description>
        /// </item>
        /// <item>
        /// <description> Up is in the positive Y-axis direction. </description>
        /// </item>
        /// <item>
        /// <description> Forward is in the negative Z-axis direction. </description>
        /// </item>
        /// </list>
        /// </remarks>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct PHASEQuaternion
        {
            public float W;
            public float X;
            public float Y;
            public float Z;
        }

        /// <summary>
        /// Enumeration representing supported channel layouts in PHASE.
        /// </summary>
        public enum ChannelLayoutType
        {
            Mono,
            Stereo,
            [InspectorName("5.1")]
            FiveOne,
            [InspectorName("7.1")]
            SevenOne
        };

        /// <summary>
        /// Enumeration containing supported directivity types in PHASE
        /// </summary>
        /// <see cref="PHASESpatialMixer"/>
        public enum DirectivityType
        {
            None,
            Cardioid,
            Cone
        };

        /// <summary>
        /// Enumeration representing different directivity presets.
        /// </summary>
        /// <see cref="PHASESpatialMixer"/>
        public enum DirectivityPreset
        {
            None,
            Cone,
            Omni,
            Cardioid,
            Hypercardioid,
            Hypocardioid,
            FigureEight
        };

        /// <summary>
        /// Struct representing directivity model parameters to send to PHASE native plugin.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DirectivityModelParameters
        {
            public DirectivityType DirectivityType;
            public int SubbandCount;
            public DirectivityModelSubbandParameters[] SubbandParameters;
        };

        /// <summary>
        /// Struct representing a subband parameters of directivity model to send to PHASE native plugin.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DirectivityModelSubbandParameters
        {
            public float Frequency;
            public float Pattern;
            public float Sharpness;
            public float InnerAngle;
            public float OuterAngle;
            public float OuterGain;
        };
        #endregion

        #region Sound Event native plugin methods and data structures.
        /// <summary>
        /// Registers a given audio buffer with the PHASE engine.
        /// </summary>
        /// <param name="inName"> Unique string representing the audio buffer. </param>
        /// <param name="inBufferData"> Array of the audio buffer data. </param>
        /// <param name="inSampleRate"> Sample rate of the audio buffer. </param>
        /// <param name="inBufferSizeInBytes"> Size of the audio buffer in bytes. </param>
        /// <param name="inBitDepth"> Bit depth of the audio buffer. </param>
        /// <param name="inChannelCount"> Number of channels in the audio buffer. </param>
        /// <returns> True on succes, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASERegisterAudioBuffer(string inName, [In] IntPtr inBufferData, uint inSampleRate, uint inBufferSizeInBytes, uint inBitDepth, uint inChannelCount);

        /// <summary>
        /// Register a given audio file with the PHASE engine.
        /// </summary>
        /// <param name="inName"> Name of the audio file. </param>
        /// <param name="inPath"> Path to the audio file. </param>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASERegisterAudioFile(string inName, string inPath);

        /// <summary>
        /// Unregister an audio asset from the PHASE engine.
        /// </summary>
        /// <param name="inName"> Unique name of the audio asset to unregister. </param>
        [DllImport(PluginDllName)] public static extern void PHASEUnregisterAudioAsset(string inName);

        /// <summary>
        /// Register a sound event asset with the PHASE engine.
        /// </summary>
        /// <param name="inName"> Unique name of the sound event asset to register. </param>
        /// <param name="inRootNodeId"> The unique ID of the sound event root node to register. </param>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASERegisterSoundEventAsset(string inName, long inRootNodeId);

        /// <summary>
        /// Unregister sound event asset from the PHASE engine.
        /// </summary>
        /// <param name="inName"> Unique name of the sound event asset to unregister. </param>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASEUnregisterSoundEventAsset(string inName);

        /// <summary>
        /// Create a sound event sampler node with the PHASE engine.
        /// </summary>
        /// <param name="inAssetName"> Name of the audio asset to play with this sampler node. </param>
        /// <param name="inMixerId"> ID of the mixer associated with this sampler node. </param>
        /// <param name="inLooping"> Set true to loop the audio on this sampler node. </param>
        /// <param name="inCalibrationMode"> Calibration mode of this sampler node. </param>
        /// <param name="inLevel"> Volume level of the sampler node. </param>
        /// <returns> The unique ID of this node, or <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreateSoundEventSamplerNode(string inAssetName, long inMixerId, bool inLooping, CalibrationMode inCalibrationMode, double inLevel);

        /// <summary>
        /// Create a sound event switch node with the PHASE engine.
        /// </summary>
        /// <param name="inSwitchParameterId"> ID of the string parameter associated with this node. </param>
        /// <param name="inSwitchEntries"> Array of switch entries. </param>
        /// <param name="inNumSwitchEntries"> Number of switch entries. </param>
        /// <returns> The unique ID of this node, or <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreateSoundEventSwitchNode(long inSwitchParameterId, SwitchNodeEntry[] inSwitchEntries, uint inNumSwitchEntries);

        /// <summary>
        /// Create a sound event random node with the PHASE engine.
        /// </summary>
        /// <param name="inRandomEntries"> Array of random entries. </param>
        /// <param name="inNumRandomEntries"> Number of random entries. </param>
        /// <returns> The unique ID of this node, or <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreateSoundEventRandomNode([In] IntPtr inRandomEntries, uint inNumRandomEntries);

        /// <summary>
        /// Create a sound event container node with the PHASE engine.
        /// </summary>
        /// <param name="inSubtreeIds"> Array of subtrees in this container node. </param>
        /// <param name="inNumSubtrees"> Number of subtrees. </param>
        /// <returns> The unique ID of this node, or <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreateSoundEventContainerNode(long[] inSubtreeIds, int inNumSubtrees);

        /// <summary>
        /// Create a sound event blend node with the PHASE engine.
        /// </summary>
        /// <param name="inBlendParameterId"> If <c>useAutoDistanceBlend</c> is true: the ID of the mixer associated with this blend node. If <c>useAutoDistanceBlend</c> is false: ID of the double parameter associated with this node. </param>
        /// <param name="entries"> Array of blend node entries. </param>
        /// <param name="numEntries"> Number of blend node entries. </param>
        /// <param name="useAutoDistanceBlend"> Set true to use </param>
        /// <returns> The unique ID of this node, or <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreateSoundEventBlendNode(long inBlendParameterId, BlendNodeEntry[] entries, int numEntries, bool useAutoDistanceBlend);

        /// <summary>
        /// Destroy the given sound event node from the PHASE engine.
        /// </summary>
        /// <param name="inNodeId"> The unique ID representing the sound event node to destroy. </param>
        [DllImport(PluginDllName)] public static extern void PHASEDestroySoundEventNode(long inNodeId);

        /// <summary>
        /// Play a provided sound event.
        /// </summary>
        /// <param name="inName"> Unique name of the sound event to play. </param>
        /// <param name="inSourceId"> Unique ID of the <c>PHASESource</c> associated with this sound event. </param>
        /// <param name="mixerIds"> Array of <c>PHASEMixer</c> IDs associated with this sound event. </param>
        /// <param name="numMixers"> Number of entries in the <c>mixerIds</c> array. </param>
        /// <param name="completionHandler"> Completion handler that is called when playback is complete. </param>
        /// <returns> The unique ID of this sound event, or <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASEPlaySoundEvent(string inName, long inSourceId, long[] mixerIds, uint numMixers, PHASESoundEventCompletionCallback completionHandler);

        /// <summary>
        /// Stop playing a provided sound event.
        /// </summary>
        /// <param name="inInstance"> The unique ID of the sound event to stop. </param>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASEStopSoundEvent(long inInstance);

        // PHASE parameter methods.

        /// <summary>
        /// Create a sound event parameter of type double in the PHASE engine.
        /// </summary>
        /// <param name="inParameterName"> The unique name of the sound event parameter. </param>
        /// <param name="inDefaultValue"> The default value of the sound event parameter. </param>
        /// <returns> The unique ID of this parameter, or <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreateSoundEventParameterDbl(string inParameterName, double inDefaultValue);

        /// <summary>
        /// Create a sound event parameter of type integer in the PHASE engine.
        /// </summary>
        /// <param name="inParameterName"> The unique name of the sound event parameter. </param>
        /// <param name="inDefaultValue"> The default value of the sound event parameter. </param>
        /// <returns> The unique ID of this parameter, or <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreateSoundEventParameterInt(string inParameterName, int inDefaultValue);

        /// <summary>
        /// Create a sound event parameter of type string in the PHASE engine.
        /// </summary>
        /// <param name="inParameterName"> The unique name of the sound event parameter. </param>
        /// <param name="inDefaultValue"> The default value of the sound event parameter. </param>
        /// <returns> The unique ID of this parameter, or <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreateSoundEventParameterStr(string inParameterName, string inDefaultValue);

        /// <summary>
        /// Create a mapped meta parameter in the PHASE engine.
        /// </summary>
        /// <param name="inParameterId"> The unique ID of the paramter that this meta parameter controls. </param>
        /// <param name="inEnvelopeParameters"> Envelope paramters that define the meta parameters curves. </param>
        /// <returns> The unique ID of this parameter, or <c>InvalidId</c> on failure. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreateMappedMetaParameter(long inParameterId, EnvelopeParameters inEnvelopeParameters);

        /// <summary>
        /// Set the value of the given sound event parameter of type integer.
        /// </summary>
        /// <param name="inInstance"> Unique ID of the sound event associated with this parameter. </param>
        /// <param name="inParamName"> Name of the parameter. </param>
        /// <param name="inParamValue"> Value to set on this parameter. </param>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASESetSoundEventParameterInt(long inInstance, string inParamName, int inParamValue);

        /// <summary>
        /// Set the value of the given sound event parameter of type double.
        /// </summary>
        /// <param name="inInstance"> Unique ID of the sound event associated with this parameter. </param>
        /// <param name="inParamName"> Name of the parameter. </param>
        /// <param name="inParamValue"> Value to set on this parameter. </param>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASESetSoundEventParameterDbl(long inInstance, string inParamName, double inParamValue);

        /// <summary>
        /// Set the value of the given sound event parameter of type string.
        /// </summary>
        /// <param name="inInstance"> Unique ID of the sound event associated with this parameter. </param>
        /// <param name="inParamName"> Name of the parameter. </param>
        /// <param name="inParamValue"> Value to set on this parameter. </param>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASESetSoundEventParameterStr(long inInstance, string inParamName, string inParamValue);

        /// <summary>
        /// Destroy the given sound event parameter from the PHASE engine.
        /// </summary>
        /// <param name="inParameterId"> Unique ID of the sound event parameter to destroy. </param>
        [DllImport(PluginDllName)] public static extern void PHASEDestroySoundEventParameter(long inParameterId);

        /// <summary>
        /// <list type="bullet">
        /// <item>
        /// <description>None: An option that specifies no loudness calibration. </description>
        /// </item>
        /// <item>
        /// <description> RelativeSpl: A sound pressure level that's tuned for the device.</description>
        /// </item>
        /// </list>
        /// <item>
        /// <description> AbsoluteSpl: A sound pressure level based on the current output device.</description>
        /// </item>
        /// </list>
        /// </summary>
        public enum CalibrationMode
        {
            None,
            RelativeSpl,
            AbsoluteSpl
        };

        /// <summary>
        /// Minimum level value for <c>CalibrationMode.RelativeSpl</c>.
        /// </summary>
        static readonly public float RelativeSplMin = -200;

        /// <summary>
        /// Maximum level value for <c>CalibrationMode.RelativeSpl</c>.
        /// </summary>
        static readonly public float RelativeSplMax = 12;

        /// <summary>
        /// Minimum level value for <c>CalibrationMode.AbsoluteSpl</c>.
        /// </summary>
        static readonly public float AbsoluteSplMin = 0;

        /// <summary>
        /// Maximum level value for <c>CalibrationMode.AbsoluteSpl</c>.
        /// </summary>
        static readonly public float AbsoluteSplMax = 120;

        /// <summary>
        /// Minimum gain value for <c>CalibrationMode.None</c>.
        /// </summary>
        static readonly public float GainMin = 0;

        /// <summary>
        /// Maximum gain value for <c>CalibrationMode.None</c>.
        /// </summary>
        static readonly public float GainMax = 1;

        /// <summary>
        /// Structure defining a switch node entry that gets passed to the native plugin.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct SwitchNodeEntry
        {
            public long NodeId;
            [MarshalAs(UnmanagedType.LPStr)] public string SwitchValue;
        }

        /// <summary>
        /// Structure defining a blend node entry that gets passed to the native plugin.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct BlendNodeEntry
        {
            // Node id.
            public long NodeId;

            // Fade parameters.
            public float LowValue;
            public float FullGainAtLow;
            public float FullGainAtHigh;
            public float HighValue;
        }

        /// <summary>
        /// Structure defining a random node entry that gets passed to the native plugin.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RandomNodeEntry
        {
            // Node id.
            public long NodeId;

            // Weight of this entry.
            public float Weight;
        }

        /// <summary>
        /// Enumeration representing different curve types.
        /// </summary>
        public enum EnvelopeCurveType
        {
            EnvelopeCurveTypeLinear,
            EnvelopeCurveTypeSquared,
            EnvelopeCurveTypeInverseSquared,
            EnvelopeCurveTypeCubed,
            EnvelopeCurveTypeInverseCubed,
            EnvelopeCurveTypeSine,
            EnvelopeCurveTypeInverseSine,
            EnvelopeCurveTypeSigmoid,
            EnvelopeCurveTypeInverseSigmoid
        };

        /// <summary>
        /// Envelope Segments to be sent to native plugin.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct EnvelopeSegment
        {
            public float X;
            public float Y;
            public EnvelopeCurveType CurveType;
        };

        /// <summary>
        /// Envelope parameters to be sent to native plugin.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct EnvelopeParameters
        {
            public float X;
            public float Y;
            public long SegmentCount;
            public EnvelopeSegment[] EnvelopeSegments;
        }

        /// <summary>
        /// Completion callback called when a sound event finishes playback.
        /// </summary>
        /// <param name="reason"> Reason that the sound event stopped playback. </param>
        /// <param name="sourceId"> ID of the associated <c>PHASESource</c>. </param>
        /// <param name="soundEventId"> ID of the associated sound event. </param>
        public delegate void PHASESoundEventCompletionCallback(PHASESoundEventStartHandlerReason reason, long sourceId, long soundEventId);

        /// <summary>
        /// Enumeration representing the reasons a sound event stopped playing.
        /// </summary>
        public enum PHASESoundEventStartHandlerReason
        {
            PHASESoundEventStartHandlerReasonError = 0,
            PHASESoundEventStartHandlerReasonFinishedPlaying = 1,
            PHASESoundEventStartHandlerReasonTerminated = 2,
        }
        #endregion

        #region Occluder and Material native plugin methods and data structures.

        /// <summary>
        /// Create an occluder in the PHASE engine.
        /// </summary>
        /// <param name="inVertCount"> Number of vertices on the occluders <c>Mesh</c>. </param>
        /// <param name="inPositions"> Array of positions on the occluders <c>Mesh</c>. </param>
        /// <param name="inNormals"> Array of normals on the occluders <c>Mesh</c>. </param>
        /// <param name="inIndexCount"> Number of indices on the occluders <c>Mesh</c>. </param>
        /// <param name="inIndices"> Array of indices on the occluders <c>Mesh</c>. </param>
        /// <returns> The unique ID of the occluder on success, <c>InvalidId</c> otherwise. </returns>
        [DllImport(PluginDllName)] public static extern long PHASECreateOccluder(int inVertCount, [In] IntPtr inPositions, [In] IntPtr inNormals, int inIndexCount, [In] IntPtr inIndices);

        /// <summary>
        /// Destroy the given occluder from the PHASE engine.
        /// </summary>
        /// <param name="inOccluderId"> Unique ID of the occluder to destroy. </param>
        [DllImport(PluginDllName)] public static extern void PHASEDestroyOccluder(long inOccluderId);

        /// <summary>
        /// Set the transform of the given occluder.
        /// </summary>
        /// <param name="inOccluderId"> Unique ID of the occluder to update. </param>
        /// <param name="inTransform"> <c>Matrix4x4</c> representing the transform of the occluder. </param>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASESetOccluderTransform(long inOccluderId, Matrix4x4 inTransform);

        /// <summary>
        /// Set the material on the given occluder.
        /// </summary>
        /// <param name="inOccluderId"> Unique ID of the occluder to update. </param>
        /// <param name="inMaterialName"> Name of the material to set on the occluder. </param>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASESetOccluderMaterial(long inOccluderId, string inMaterialName);

        /// <summary>
        /// Create a material from a preset in the PHASE engine.
        /// </summary>
        /// <param name="inName"> Name of the material to create. </param>
        /// <param name="material"> Preset representing the material's parameters. </param>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASECreateMaterialFromPreset(string inName, MaterialPreset material);

        /// <summary>
        /// Destroy the given material from the PHASE engine.
        /// </summary>
        /// <param name="inName"> Name of the material to destroy. </param>
        /// <returns> True on success, false otherwise. </returns>
        [DllImport(PluginDllName)] public static extern bool PHASEDestroyMaterial(string inName);

        /// <summary>
        /// Enumeration of material presets available in PHASE.
        /// </summary>
        public enum MaterialPreset
        {
            Cardboard,
            Glass,
            Brick,
            Concrete,
            Drywall,
            Wood
        }
        #endregion

        #region Mesh data structures and helper functions.

        /// <summary>
        /// Struct representing mesh data to pass to the PHASE native plugin.
        /// </summary>
        public struct MeshData
        {
            public Vector3[] Vertices;
            public Vector3[] Normals;
            public int[] Indices;
            public int IndexCount;
            public int VertexCount;
        };

        /// <summary>
        /// Retrieves mesh data formatted for PHASE from a mesh filter and a transform.
        /// </summary>
        /// <param name="meshFilter"> Unity MeshFilter to reformat for PHASE. </param>
        /// <param name="transform"> Unity Transform to reformate for PHASE. </param>
        /// <returns> Struct of MeshData formatted for PHASE. </returns>
        static public MeshData GetMeshData(MeshFilter meshFilter, Transform transform)
        {
            MeshData meshData = new MeshData();

            // Get the mesh.
            Mesh mesh = new Mesh();

            // Combine sub-meshes if they exist.
            if (meshFilter.sharedMesh.subMeshCount > 1)
            {
                // Copy all mesh properties so we don't alter sharedMesh.
                mesh.vertices = meshFilter.sharedMesh.vertices;
                mesh.triangles = meshFilter.sharedMesh.triangles;
                mesh.normals = meshFilter.sharedMesh.normals;
                mesh.tangents = meshFilter.sharedMesh.tangents;
                mesh.SetTriangles(mesh.triangles, 0);
                mesh.subMeshCount = 1;
            }
            else
            {
                mesh = meshFilter.sharedMesh;
            }

            // Get index and vertex count.
            meshData.VertexCount = mesh.vertexCount;
            meshData.IndexCount = (int)mesh.GetIndexCount(0);

            // Get the mesh data.
            var vertices = mesh.vertices;
            var normals = mesh.normals;
            var indices = mesh.GetIndices(0);

            // Convert the vert and normal positions to right handed for the plugin.
            meshData.Vertices = new Vector3[mesh.vertexCount];
            meshData.Normals = new Vector3[mesh.vertexCount];
            Vector3 combinedScale = GetCombinedHierachyScale(transform);
            for (uint vertIdx = 0; vertIdx < mesh.vertexCount; ++vertIdx)
            {
                meshData.Vertices[vertIdx] = RhConversionMat.MultiplyVector(vertices[vertIdx]);
                meshData.Vertices[vertIdx].x *= combinedScale.x;
                meshData.Vertices[vertIdx].y *= combinedScale.y;
                meshData.Vertices[vertIdx].z *= combinedScale.z;
                meshData.Normals[vertIdx] = RhConversionMat.MultiplyVector(normals[vertIdx]);
            }

            // Convert the triangle indices to right handed winding for the plugin.
            meshData.Indices = new int[meshData.IndexCount];
            for (uint idx = 0; idx < meshData.IndexCount; idx += 3)
            {
                meshData.Indices[idx] = indices[idx];
                meshData.Indices[idx + 1] = indices[idx + 2];
                meshData.Indices[idx + 2] = indices[idx + 1];
            }

            return meshData;
        }

        /// <summary>
        /// Matrix to convert to right-handed coordinate system.
        /// </summary>
        static public Matrix4x4 RhConversionMat = new Matrix4x4(new Vector4(Vector3.right[0], Vector3.right[1], Vector3.right[2], 0),
                                                                new Vector4(Vector3.up[0], Vector3.up[1], Vector3.up[2], 0),
                                                                new Vector4(Vector3.back[0], Vector3.back[1], Vector3.back[2], 0),
                                                                new Vector4(0, 0, 0, 1));

        // 
        /// <summary>
        /// Converts a Unity transform to a PHASE transform (Left-Handed to Right-Handed).
        /// </summary>
        /// <param name="inTransform"> Unity based transform to convert to PHASE coordinates. </param>
        /// <returns> A <c>Matrix4x4</c> representing a transform in PHASE coordinates. </returns>
        static public Matrix4x4 GetPhaseTransform(Transform inTransform)
        {
            Matrix4x4 phaseTransform = new Matrix4x4(inTransform.right, inTransform.up, inTransform.forward, new Vector4());
            Vector3 position = RhConversionMat * inTransform.position;
            phaseTransform.m30 = position.x;
            phaseTransform.m31 = position.y;
            phaseTransform.m32 = position.z;
            phaseTransform.m33 = 1.0f;
            return phaseTransform;
        }

        static private Vector3 GetCombinedHierachyScale(Transform inTransform)
        {
            Vector3 combinedScale = inTransform.localScale;
            if (inTransform.parent != null)
            {
                Transform currentLevel = inTransform.parent;
                while (currentLevel.parent != null)
                {
                    combinedScale.x *= currentLevel.localScale.x;
                    combinedScale.y *= currentLevel.localScale.y;
                    combinedScale.z *= currentLevel.localScale.z;
                    currentLevel = currentLevel.parent;
                }
                combinedScale.x *= currentLevel.localScale.x;
                combinedScale.y *= currentLevel.localScale.y;
                combinedScale.z *= currentLevel.localScale.z;
            }
            return combinedScale;
        }
        #endregion
    }
}