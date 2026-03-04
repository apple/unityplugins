//
//  PHASEWrapper.h
//  AudioPluginPHASE
//
//  Copyright © 2021 Apple Inc. All rights reserved.
//

#import <PHASE/PHASE.h>

#define PHASEInvalidInstanceHandle (-1)

NS_HEADER_AUDIT_BEGIN(nullability)

struct RandomNodeEntry
{
    const int64_t nodeId;
    const float weight;
};

struct SwitchNodeEntry
{
    const int64_t nodeId;
    const char* switchValue;
};

struct BlendNodeEntry
{
    const int64_t nodeId;
    const float lowValue;
    const float fullGainAtLow;
    const float fullGainAtHigh;
    const float highValue;
};

enum DirectivityType
{
    None,
    Cardioid,
    Cone
};

struct DirectivityModelSubbandParameters
{
    const float frequency;
    const float pattern;
    const float sharpness;
    const float innerAngle;
    const float outerAngle;
    const float outerGain;
};

struct DirectivityModelParameters
{
    DirectivityType directivityType;
    uint32_t subbandCount;
    DirectivityModelSubbandParameters* subbandParameters;
};

enum EnvelopeCurveType
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

struct EnvelopeSegment
{
    const float x;
    const float y;
    const EnvelopeCurveType curveType;
};

struct EnvelopeParameters
{
    float x;
    float y;
    uint32_t segmentCount;
    EnvelopeSegment* envelopeSegments;
};

enum ChannelLayoutType
{
    ChannelLayoutTypeMono,
    ChannelLayoutTypeStereo,
    ChannelLayoutTypeFiveOne,
    ChannelLayoutTypeSevenOne
};

struct Quaternion
{
    const float w;  // cooresponds to 'r' in simd_quaternion
    const float x;  // ix
    const float y;  // iy
    const float z;  // iz
};

enum MaterialPreset
{
    MaterialPresetCardboard,
    MaterialPresetGlass,
    MaterialPresetBrick,
    MaterialPresetConcrete,
    MaterialPresetDrywall,
    MaterialPresetWood
};

enum CalibrationMode
{
    CalibrationModeNone = 0,
    CalibrationModeRelativeSpl = 1,
    CalibrationModeAbsoluteSpl = 2
};



/****************************************************************************************************/
/*! @class PHASEEngineWrapper
    @abstract A PHASEEngineWrapper allows creating and managing PHASE objects
*/
@interface PHASEEngineWrapper : NSObject

/*! @method phaseEngineWrapper
    @abstract Singleton of PHASEEngineWrapper - performs all initialization requirements when accessed.
    @return shared instance
*/
+ (id)sharedInstance;

/*! @method isInitialized
    @abstract Returns whether the engine is created and started
    @return true on success, false otherwise
*/
- (BOOL)isInitialized;

/*! @method createListener
    @abstract Creates the listener object
    @return true on success, false otherwise
*/
- (BOOL)createListener;

/*! @method setListenerTransform
    @abstract Sets the 3D transform of the listener
    @param listenerTransform transform to set
    @return true on success, false otherwise
*/
- (BOOL)setListenerTransform:(simd_float4x4)listenerTransform;

/*! @method setListenerGain
    @abstract Sets the gain linear scale value of the listener, range of [0,1]
    @param listenerGain gain to set
    @return true on success, false otherwise
*/
- (BOOL)setListenerGain:(double)listenerGain;

/*! @method getListenerGain
    @abstract Gets the gain of the listener
    @return double representing listener gain scalar value, range of [0,1]
*/
- (double)getListenerGain;

/*! @method setListenerHeadTracking
    @abstract Sets the listener to head track
    @param headTrackingEnabled bool representing headTracking enabled or disabled
    @return true on success, false otherwise
*/
- (BOOL)setListenerHeadTracking:(BOOL)headTrackingEnabled;

/*! @method destroyListener
    @abstract Destroys the listener object
    @return true on success, false otherwise
*/
- (BOOL)destroyListener;

/*! @method createSourceWithMesh
    @abstract Creates a volumetric source with a given mesh
    @param mesh MDL mesh to create the source with
    @return Id of created source
*/
- (int64_t)createSourceWithMesh:(MDLMesh*)mesh;

/*! @method createSource
    @abstract Creates a point source
    @return Id of created source
*/
- (int64_t)createSource;

/*! @method setSourceTransformWithId
    @abstract Sets the 3D transform of the source
    @param sourceId source ID to update transform for
    @param transform transform to set
    @return true on success, false otherwise
*/
- (BOOL)setSourceTransformWithId:(int64_t)sourceId transform:(simd_float4x4)transform;

/*! @method setSourceGainWithId
    @abstract Sets the gain linear scale value of the source, range of [0,1]. Given gain values outside of this range will be clamped.
    @param sourceId  source ID to update gain for
    @param sourceGain gain to set on source
    @return true on success, false otherwise
*/
- (BOOL)setSourceGainWithId:(int64_t)sourceId sourceGain:(double)sourceGain;

/*! @method getSourceGainWithId
    @abstract Gets the gain of the source
    @param sourceId  source ID to get gain for
    @return double representing source's gain scalar value, range of [0,1]
*/
- (double)getSourceGainWithId:(int64_t)sourceId;

/*! @method destroySourceWithId
    @abstract Destroys a source with a given Id
    @param sourceId id of source to destroy
*/
- (void)destroySourceWithId:(int64_t)sourceId;

/*! @method createOccluderWithMesh
    @abstract Creates an occluder with a given mesh
    @param mesh MDL mesh to create the occluder with
    @return Id of created occluder
*/
- (int64_t)createOccluderWithMesh:(MDLMesh*)mesh;

/*! @method setOccluderTransformWithId
    @abstract Sets the 3D transform of the occluder
    @param occluderId source ID to update transform for
    @param transform transform to set
    @return true on success, false otherwise
*/
- (BOOL)setOccluderTransformWithId:(int64_t)occluderId transform:(simd_float4x4)transform;

/*! @method setOccluderMaterial
    @abstract Sets a material of a given name in an occluder
    @param occluderId occluder ID to set material on
    @param materialName name of material to set
    @return true on success, false otherwise
*/
- (BOOL)setOccluderMaterialWithId:(int64_t)occluderId materialName:(NSString*)materialName;

/*! @method destroyOccluderWithId
    @param occluderId id of occluder to destroy
    @abstract Destroys the occluder with a given ID
*/
- (void)destroyOccluderWithId:(int64_t)occluderId;

/*! @method createMaterialWithName
    @abstract Creates a material from the given preset
    @param name name of the material to create
    @param preset material preset of type MaterialPreset
    @return true on success false otherwise
 */
- (BOOL)createMaterialWithName:(NSString*)name preset:(MaterialPreset)preset;

/*! @method destroyMaterialWithName
    @param name name of the material to destroy
    @abstract Destroys an existing material
*/
- (void)destroyMaterialWithName:(NSString*)name;

/*! @method setSceneReverbWithPreset
    @abstract Sets a reverb preset in the scene
    @param preset Reverb preset to set on scene
*/
- (void)setSceneReverbWithPreset:(PHASEReverbPreset)preset;

/*! @method registerAudioBufferWithData
    @abstract Registers an audio buffer in the system (data gets copied internally)
    @param data buffer data to register
    @param identifier unique identifier of buffer to register
    @param audioFormat audio format of the buffer data
    @return true on success, false otherwise
*/
- (BOOL)registerAudioBufferWithData:(NSData*)data identifier:(NSString*)identifier audioFormat:(AVAudioFormat*)audioFormat;

/*! @method registerAudioAssetWithURL
    @abstract Registers an audio file into the system
    @param url url of the audio asset to register
    @param identifier unique identifier of buffer to register
    @param audioFormat audio format of the buffer data
    @return true on success, false otherwise
*/
- (BOOL)registerAudioAssetWithURL:(NSURL*)url identifier:(NSString*)identifier audioFormat:(AVAudioFormat*)audioFormat;

/*! @method unregisterAudioBufferWithIndentifier
    @param identifier unique identifier of buffer to unregister
    @abstract Unregisters an audio buffer with a given identifier
*/
- (void)unregisterAudioBufferWithIdentifier:(NSString*)identifier;

/*! @method createSpatialMixerWithName
    @abstract Creates a spatial mixer with a given name and parameters
    @param mixerName name of the mixer to create
    @param enableDirectPath enable direct path modeling for this mixer
    @param enableEarlyReflections enable early reflection modeling for this mixer
    @param enableLateReverb enable late reverb modeling for this mixer
    @param cullDistance distance at which the systems stops processing the sound
    @param rolloffFactor rolloff factor for the geometric spreading distance model
    @param sourceDirectivityModelParameters directivity parameters for mixer source ( cone or cardioid)
    @param listenerDirectivityModelParameters directivity parameters for mixer listener ( cone or cardioid)
    @return mixer id
*/
- (int64_t)createSpatialMixerWithName:(NSString*)mixerName
                     enableDirectPath:(BOOL)enableDirectPath
               enableEarlyReflections:(BOOL)enableEarlyReflections
                     enableLateReverb:(BOOL)enableLateReverb
                         cullDistance:(double)cullDistance
                        rolloffFactor:(float)rolloffFactor
     sourceDirectivityModelParameters:(DirectivityModelParameters)sourceDirectivityModelParameters
   listenerDirectivityModelParameters:(DirectivityModelParameters)listenerDirectivityModelParameters;

/*! @method createChannelMixerWithName
    @abstract Creates a channel mixer with a given name and parameters
    @param mixerName name of the mixer to create
    @param channelLayout channel layout tag
    @return mixer id
*/
- (int64_t)createChannelMixerWithName:(NSString*)mixerName channelLayout:(ChannelLayoutType)channelLayout;

/*! @method createAmbientMixerWithName
    @abstract Creates an ambient mixer with a given name and parameters
    @param mixerName name of the mixer to create
    @param channelLayout channel layout tag
    @param orientation quaterion comprised of 4 float values x, y, z, w
    @return mixer id
*/
- (int64_t)createAmbientMixerWithName:(NSString*)mixerName
                        channelLayout:(ChannelLayoutType)channelLayout
                          orientation:(Quaternion)orientation;

/*! @method destroyMixerWithId
    @param mixerId mixer Id to destroy
    @abstract Destroys a mixer with the given id
*/
- (void)destroyMixerWithId:(int64_t)mixerId;

/*!  @method getChannelLayoutTag
     @param channelLayout The internal ChannelLayoutType id.
     @return The ChannelLayoutTag corresponding to channelLayout
     @abstract Converts an internal ChannelLayoutType to an AudioChannelLayoutTag.
 */
+ (AudioChannelLayoutTag)getChannelLayoutTag:(ChannelLayoutType)channelLayout;

/*! @method createMetaParameterWithName
    @abstract Creates a integer meta parameter with a name and default value
    @param name parameter name to create
    @param defaultIntValue default integer value of parameter
    @param minimumValue minimum value of the parameter
    @param maximumValue maximum value of the parameter
    @return parameter id
*/
- (int64_t)createMetaParameterWithName:(NSString*)name defaultIntValue:(int)defaultIntValue minimumValue:(int)minimumValue maximumValue:(int)maximumValue;

/*! @method getMetaParameterIntValueWithId
    @abstract Gets the value of a given parameter of type integer on a sound event instance
    @param instanceId sound event instance to get parameter from
    @param parameterName name of the parameter to get
    @return the value of the parameter
*/
- (int)getMetaParameterIntValueWithId:(int64_t)instanceId parameterName:(NSString*)parameterName;

/*! @method setMetaParameterWithId
    @abstract Sets the value of a given parameter of type integer on an sound event instance
    @param instanceId sound event instance to set parameter on
    @param parameterName name of the parameter to set
    @param intValue integer value of the parameter to set
    @return true on success false otherwise
*/
- (BOOL)setMetaParameterWithId:(int64_t)instanceId parameterName:(NSString*)parameterName intValue:(int)intValue;

/*! @method createMetaParameterWithName
    @abstract Creates a double meta parameter with a name and default value
    @param name parameter name to create
    @param defaultDblValue default double value of parameter
    @param minimumValue minimum value of the parameter
    @param maximumValue maximum value of the parameter
    @return parameter id
*/
- (int64_t)createMetaParameterWithName:(NSString*)name defaultDblValue:(double)defaultDblValue minimumValue:(double)minimumValue maximumValue:(double)maximumValue;

/*! @method getMetaParameterDblValueWithId
    @abstract Gets the value of a given parameter of type double on a sound event instance
    @param instanceId sound event instance to get parameter from
    @param parameterName name of the parameter to get
    @return the value of the parameter
*/
- (double)getMetaParameterDblValueWithId:(int64_t)instanceId parameterName:(NSString*)parameterName;

/*! @method setMetaParameterWithIdDbl
    @abstract Sets the value of a given parameter of type double on a sound event instance
    @param instanceId instance to set parameter on
    @param parameterName name of the parameter to set
    @param doubleValue double value of the parameter to set
    @return true on success false otherwise
*/
- (BOOL)setMetaParameterWithId:(int64_t)instanceId parameterName:(NSString*)parameterName doubleValue:(double)doubleValue;

/*! @method createMetaParameterWithName
    @abstract Creates a string sound event meta parameter with a name and default value
    @param name parameter name to create
    @param defaultStrValue default stringvalue of parameter
    @return parameter id
*/
- (int64_t)createMetaParameterWithName:(NSString*)name defaultStrValue:(NSString*)defaultStrValue;

/*! @method getMetaParameterStrValueWithId
    @abstract Gets the value of a given parameter of type string on a sound event instance
    @param instanceId sound event instance to get parameter from
    @param parameterName name of the parameter to get
    @return the value of the parameter
*/
- (NSString*)getMetaParameterStrValueWithId:(int64_t)instanceId parameterName:(NSString*)parameterName;

/*! @method setMetaParameterWithId
    @abstract Sets the value of a given parameter of type string on a sound event instance
    @param instanceId instance to set parameter on
    @param parameterName name of the parameter to set
    @param stringValue string value of the parameter to set
    @return true on success false otherwise
*/
- (BOOL)setMetaParameterWithId:(int64_t)instanceId parameterName:(NSString*)parameterName stringValue:(NSString*)stringValue;

/*! @method destroyMetaParameterWithId
    @abstract Destroys an sound event meta parameter
    @param parameterId parameter id to destroy
*/
- (void)destroyMetaParameterWithId:(int64_t)parameterId;

/*! @method createMappedMetaParameterWithParameterId
    @param parameterId of the input parameter
    @param envelopeParameters parameters of the envelope to be created
    @return return the instance ID of the mapped meta parameter
*/
- (int64_t)createMappedMetaParameterWithParameterId:(int64_t)parameterId envelopeParameters:(EnvelopeParameters)envelopeParameters;

/*! @method destoryMappedMetaParameterWithId
    @param parameterId id of the mapped meta parameter
*/
- (void)destoryMappedMetaParameterWithId:(int64_t)parameterId;

/*! @method setMixerGainParameterOnMixerWithId
    @abstract Sets the gainMetaParameterDefinition to the given parameter on the given mixer
    @param parameterId parameter used to define the gainMetaParameter of the given mixer
    @param mixerId mixerId mixer to set gainMetaParameterDefinition on
    @return true on success false otherwise
*/
- (BOOL)setMixerGainParameterOnMixerWithId:(int64_t)parameterId mixerId:(int64_t)mixerId;

/*! @method createSoundEventSamplerNodeWithAsset
    @abstract Creates an sound event sampler node with a given asset name and parameters
    @param assetName name of the asset to create sampler node for
    @param mixerId mixer Id to attach this sampler to
    @param rateParameterId  rate meta parameter Id to associated with this sampler
    @param looping whether the asset loops or not
    @param calibrationMode the PHASECalibrationMode of the sampler node
    @param level the volume level
    @return true on success, false otherwise
*/
- (int64_t)createSoundEventSamplerNodeWithAsset:(NSString*)assetName
                                        mixerId:(int64_t)mixerId
                                rateParameterId:(int64_t)rateParameterId
                                        looping:(BOOL)looping
                                calibrationMode:(CalibrationMode)calibrationMode
                                          level:(double)level;

/*! @method createSoundEventPullStreamNodeWithAsset
    @abstract Creates a sound event pull stream node with a given asset name and parameters
    @param assetName name of the asset to create sampler node for
    @param mixerId mixer Id to attach this stream to
    @param format the AVAudioFormat for the stream
    @param calibrationMode the PHASECalibrationMode of the sampler node
    @param level the volume level
    @return true on success, false otherwise
*/
- (int64_t)createSoundEventPullStreamNodeWithAsset:(NSString*)assetName
                                           mixerId:(int64_t)mixerId
                                            format:(AVAudioFormat*)format
                                   calibrationMode:(CalibrationMode)calibrationMode
                                             level:(double)level;

/*! @method createSoundEventSwitchNodeWithParameter
    @abstract creates a sound event switch node with a given meta parameter id and switch entries
    @param parameterId meta parameter Id that controls the switch node
    @param switchEntries entries containing the different sound event nodes to switch from
*/
- (int64_t)createSoundEventSwitchNodeWithParameter:(int64_t)parameterId switchEntries:(NSDictionary*)switchEntries;

/*! @method createSoundEventRandomNodeWithEntries
    @abstract creates a sound event random node with given random entries to select from
    @param randomEntries entries to randomize
    @param uniqueSelectionQueueLength Subtrees will not be repeated until after this random node is activated uniqueSelectionQueueLength number of times.
*/
- (int64_t)createSoundEventRandomNodeWithEntries:(NSDictionary*)randomEntries uniqueSelectionQueueLength:(int64_t)uniqueSelectionQueueLength;

/*! @method createSoundEventBlendNodeWithParameter
    @abstract creates a sound event blend node with a given meta parameter id and blend entries
    @param parameterId meta parameter Id that controls the switch node
    @param blendRanges array of ranges
    @param numRanges number of Ranges
    @param useAutoDistanceBlend if true then the parameter passed in is a spatial mixer otherwise a manually controlled numeric parameter
*/
- (int64_t)createSoundEventBlendNodeWithParameter:(int64_t)parameterId
                                      blendRanges:(BlendNodeEntry*)blendRanges
                                        numRanges:(uint32_t)numRanges
                             useAutoDistanceBlend:(bool)useAutoDistanceBlend;

/*! @method createSoundEventContainerNodeWithChild
    @param childIds array of child node ids
    @param numChildren number of child nodes in the childIds array
    @return container node instance id
 */
- (int64_t)createSoundEventContainerNodeWithChild:(int64_t*)childIds numChildren:(uint32_t)numChildren;

/*! @method destroySoundEventNodeWithId
    @param nodeId node id to destroy
    @abstract destroys an sound event node
*/
- (void)destroySoundEventNodeWithId:(int64_t)nodeId;

/*! @method registerSoundEventWithName
    @abstract Creates and registers an sound event asset with a given name and root node Id
    @param name unique name to register the sound event with
    @param rootNodeId  id of the root node of the sound event
    @return true on success, false otherwise
*/
- (BOOL)registerSoundEventWithName:(NSString*)name rootNodeId:(int64_t)rootNodeId;

/*! @method unregisterSoundEventWithName
    @abstract Unregisters an sound event with a given name
    @param name name of sound event to unregister
*/
- (void)unregisterSoundEventWithName:(NSString*)name;

/*! @method playSoundEventWithName
    @abstract Play the sound event
    @param name name of sound event to play
    @param sourceId source Id of source to play sound event from
    @param mixerIds an array of mixerIds
    @param numMixers the length of the above mixerId array
    @param streamName name of the Pull Stream Node when the SoundEvent is of type PHASEPullStreamSoundEvent
    @param renderBlock the render block, only used when the SoundEvent is of type PHASEPullStreamSoundEvent
    @param completionHandlerBlock callback function called when SoundEvent is completed
    @return true on success, false otherwise
*/
- (int64_t)playSoundEventWithName:(NSString*)name
                      sourceId:(int64_t)sourceId
                      mixerIds:(int64_t*)mixerIds
                     numMixers:(int64_t)numMixers
                    streamName:(nullable NSString*)streamName
                   renderBlock:(nullable PHASEPullStreamRenderBlock)renderBlock
        completionHandlerBlock:(void (^_Nullable)(PHASESoundEventStartHandlerReason reason, int64_t sourceId, int64_t soundEventId))completionHandlerBlock;

/*! @method stopSoundEventWithId
    @abstract Stops an sound event instance
    @param instanceId instance Id to stop
    @return true on success false otherwise
*/
- (BOOL)stopSoundEventWithId:(int64_t)instanceId;

/*! @method start
    @abstract Starts the PHASE engine
*/
- (BOOL)start;

/*! @method pause
    @abstract Pauses the PHASE engine
*/
- (void)pause;

/*! @method stop
    @abstract Stops the PHASE engine
*/
- (void)stop;

/*! @method update
    @abstract Updates the PHASE engine
*/
- (void)update;

NS_HEADER_AUDIT_END(nullability)
@end
