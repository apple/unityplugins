//
//  PHASEWrapper.h
//  AudioPluginPHASE
//
//  Copyright © 2021 Apple Inc. All rights reserved.
//

#import <PHASE/PHASE.h>

#define PHASEInvalidInstanceHandle (-1)

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

/*! @method unregisterAudioBufferWithUID
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
    @param sourceDirectivityModelParameters directivity parameters for mixer source ( cone or cardioid)
    @param listenerDirectivityModelParameters directivity parameters for mixer listener ( cone or cardioid)
    @return mixer id
*/
- (int64_t)createSpatialMixerWithName:(NSString*)mixerName
                     enableDirectPath:(BOOL)enableDirectPath
               enableEarlyReflections:(BOOL)enableEarlyReflections
                     enableLateReverb:(BOOL)enableLateReverb
                         cullDistance:(double)cullDistance
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
    @return parameter id
*/
- (int64_t)createMetaParameterWithName:(NSString*)name defaultIntValue:(int)defaultIntValue;

/*! @method setMetaParameterWithId
    @abstract Sets the value of a given parameter of type integer on an action tree instance
    @param instanceId action tree instance to set parameter on
    @param parameterName name of the parameter to set
    @param intValue integer value of the parameter to set
    @return true on success false otherwise
*/
- (BOOL)setMetaParameterWithId:(int64_t)instanceId parameterName:(NSString*)parameterName intValue:(int)intValue;

/*! @method createMetaParameterWithName
    @abstract Creates a double meta parameter with a name and default value
    @param name parameter name to create
    @param defaultDblValue default double value of parameter
    @return parameter id
*/
- (int64_t)createMetaParameterWithName:(NSString*)name defaultDblValue:(double)defaultDblValue;

/*! @method setMetaParameterWithId
    @abstract Sets the value of a given parameter of type double on an action tree instance
    @param instanceId instance to set parameter on
    @param parameterName name of the parameter to set
    @param doubleValue double value of the parameter to set
    @return true on success false otherwise
*/
- (BOOL)setMetaParameterWithId:(int64_t)instanceId parameterName:(NSString*)parameterName doubleValue:(double)doubleValue;

/*! @method createMetaParameterWithName
    @abstract Creates a string action tree meta parameter with a name and default value
    @param name parameter name to create
    @param defaultStrValue default stringvalue of parameter
    @return parameter id
*/
- (int64_t)createMetaParameterWithName:(NSString*)name defaultStrValue:(NSString*)defaultStrValue;

/*! @method setMetaParameterWithId
    @abstract Sets the value of a given parameter of type string on an action tree instance
    @param instanceId instance to set parameter on
    @param parameterName name of the parameter to set
    @param stringValue string value of the parameter to set
    @return true on success false otherwise
*/
- (BOOL)setMetaParameterWithId:(int64_t)instanceId parameterName:(NSString*)parameterName stringValue:(NSString*)stringValue;

/*! @method destroyMetaParameterWithId
    @abstract Destroys an action tree meta parameter
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

/*! @method createSoundEventSamplerNodeWithAsset
    @abstract Creates an action tree sampler node with a given asset name and parameters
    @param assetName name of the asset to create sampler node for
    @param mixerId mixer Id to attach this sampler to
    @param looping whether the asset loops or not
    @param calibrationMode the PHASECalibrationMode of the sampler node
    @param level the volume level
    @return true on success, false otherwise
*/
- (int64_t)createSoundEventSamplerNodeWithAsset:(NSString*)assetName
                                        mixerId:(int64_t)mixerId
                                        looping:(BOOL)looping
                                calibrationMode:(CalibrationMode)calibrationMode
                                          level:(double)level;

/*! @method createSoundEventSwitchNodeWithParameter
    @abstract creates an action tree switch node with a given meta parameter id and switch entries
    @param parameterId meta parameter Id that controls the switch node
    @param switchEntries entries containing the different action tree nodes to switch from
*/
- (int64_t)createSoundEventSwitchNodeWithParameter:(int64_t)parameterId switchEntries:(NSDictionary*)switchEntries;

/*! @method createSoundEventRandomNodeWithEntries
    @abstract creates an action tree random node with given random entries to select from
    @param randomEntries entries to randomize
*/
- (int64_t)createSoundEventRandomNodeWithEntries:(NSDictionary*)randomEntries;

/*! @method createSoundEventBlendNodeWithParameter
    @abstract creates an action tree blend node with a given meta parameter id and blend entries
    @param parameterId meta parameter Id that controls the switch node
    @param blendRanges array of ranges
    @param numRanges number of Ranges
    @param useAutoDistanceBlend if true then the parameter passed in is a spatial mixer otherwise a manually controlled numeric parameter
*/
- (int64_t)createSoundEventBlendNodeWithParameter:(int64_t)parameterId
                                      blendRanges:(BlendNodeEntry*)blendRanges
                                        numRanges:(uint32_t)numRanges
                             useAutoDistanceBlend:(bool)useAutoDistanceBlend;

/*! @method createSoundEventContainerNodeWithSubtree
    @param subtreeIds array of child node ids
    @param numSubtrees number of child nodes in the subtreeId array
    @return container node instance id
 */
- (int64_t)createSoundEventContainerNodeWithSubtree:(int64_t*)subtreeIds numSubtrees:(uint32_t)numSubtrees;

/*! @method destroySoundEventNodeWithId
    @param nodeId node id to destroy
    @abstract destroys an action tree node
*/
- (void)destroySoundEventNodeWithId:(int64_t)nodeId;

/*! @method registerSoundEventWithName
    @abstract Creates and registers an action tree asset with a given name and root node Id
    @param name unique name to register the action tree with
    @param rootNodeId  id of the root node of the action tree
    @return true on success, false otherwise
*/
- (BOOL)registerSoundEventWithName:(NSString*)name rootNodeId:(int64_t)rootNodeId;

/*! @method unregisterSoundEventWithName
    @abstract Unregisters an action tree with a given name
    @param name name of action tree to unregister
*/
- (void)unregisterSoundEventWithName:(NSString*)name;

/*! @method playSoundEventWithName
    @abstract Plays an action tree
    @param name name of action tree to play
    @param sourceId source Id of source to play action tree from
    @param completionHandler callback function called when SoundEvent is completed
    @return playing instance id
*/
- (int64_t)playSoundEventWithName:(NSString*)name
                         sourceId:(int64_t)sourceId
                         mixerIds:(int64_t*)mixerIds
                        numMixers:(uint64_t)numMixers
                completionHandler:
                  (void (*)(PHASESoundEventStartHandlerReason reason, int64_t sourceId, int64_t soundEventId))completionHandler;

/*! @method stopSoundEventWithId
    @abstract Stops an action tree instance
    @param instanceId instance Id to stop
    @return true on success false otherwise
*/
- (BOOL)stopSoundEventWithId:(int64_t)instanceId;

/*! @method start
    @abstract Starts the PHASE engine
*/
- (BOOL)start;

/*! @method stop
    @abstract Stops the PHASE engine
*/
- (void)stop;

/*! @method update
    @abstract Updates the PHASE engine
*/
- (void)update;

@end
