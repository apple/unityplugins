//
//  PHASESoundEventInterface.h
//  AudioPluginPHASE
//
//  Copyright © 2021 Apple Inc. All rights reserved.
//

#ifndef PHASESoundEventInterface_h
#define PHASESoundEventInterface_h

#import "PHASEWrapper.h"

extern "C" {

enum StartHandlerReason
{
    StartHandlerReasonFailure = 0,
    StartHandlerReasonFinishedPlaying = 1,
    StartHandlerReasonTerminated = 2
};

typedef void (*PHASESoundEventCompletionHandler)(StartHandlerReason reason, int64_t sourceId, int64_t soundEventId);

/*
 Registers an audio buffer.
 */
bool PHASERegisterAudioBuffer(const char* inName,
                              void* inBufferData,
                              uint32_t inSampleRate,
                              uint32_t inBufferSizeInBytes,
                              uint32_t inBitDepth,
                              uint32_t inChannelCount);

/*
 Unregisters an audio asset.
 */
void PHASEUnregisterAudioAsset(const char* inName);

/*
 Registers an audio file.
 */
bool PHASERegisterAudioFile(const char* inName, const char* inPath);

/*
 Creates a spatial mixer
 */
int64_t PHASECreateSpatialMixer(const char* inName,
                                bool inEnableDirectPath,
                                bool inEnableEarlyReflections,
                                bool inEnableLateReverb,
                                float inCullDistance,
                                float inRolloffFactor,
                                DirectivityModelParameters inSourceDirectivityModelParameters,
                                DirectivityModelParameters inListenerDirectivityModelParameters);

/*
 Creates a channel mixer
 */
int64_t PHASECreateChannelMixer(const char* inName, ChannelLayoutType inChannelLayout);

/*
 Creates an ambient mixer
 */
int64_t PHASECreateAmbientMixer(const char* inName, ChannelLayoutType inChannelLayout, Quaternion inOrientation);

/*
 Destroys a mixer
 */
void PHASEDestroyMixer(int64_t inMixerId);

/*
 Creates a sound event parameter of type integer
 */
int64_t PHASECreateSoundEventParameterInt(const char* inParameterName, int inDefaultValue, int inMinimumValue, int inMaximumValue);

/*
 Gets the parameter on a sound event of type integer
 */
int PHASEGetSoundEventParameterInt(int64_t inInstance, const char* inParamName);

/*
 Sets a parameter on a sound event of type integer
 */
bool PHASESetSoundEventParameterInt(int64_t inInstance, const char* inParamName, int inParamValue);

/*
 Creates a sound event parameter of type double
 */
int64_t PHASECreateSoundEventParameterDbl(const char* inParameterName, double inDefaultValue, double inMinimumValue, double inMaximumValue);

/*
 Gets the parameter on a sound event of type double
 */
double PHASEGetSoundEventParameterDbl(int64_t inInstance, const char* inParamName);

/*
 Sets a parameter on a sound event of type double
 */
bool PHASESetSoundEventParameterDbl(int64_t inInstance, const char* inParamName, double inParamValue);

/*
 Creates a sound event parameter of type string
 */
int64_t PHASECreateSoundEventParameterStr(const char* inParameterName, const char* inDefaultValue);

/*
 Gets the parameter on a sound event of type string
 */
const char* PHASEGetSoundEventParameterStr(int64_t inInstance, const char* inParamName);

/*
 Sets a parameter on a sound event of type string
 */
bool PHASESetSoundEventParameterStr(int64_t inInstance, const char* inParamName, const char* inParamValue);

/*
 Sets a sound event mixer gain parameter
 */
bool PHASESetMixerGainMetaParameter(int64_t inParameterId, int64_t inMixerId);

/*
 Destroys a sound event parameter
 */
void PHASEDestroySoundEventParameter(int64_t inParameterId);

/*
 Creates a mapped meta parameter
 */
int64_t PHASECreateMappedMetaParameter(int64_t inParameterId, EnvelopeParameters inEnvelopeParameters);

/*
 Destroys a mapped meta parameter
 */
void PHASEDestroyMappedMetaParameter(int64_t inParameterId);

/*
 Creates an sound event sampler node
 */
int64_t PHASECreateSoundEventSamplerNode(const char* inAssetName,
                                         int64_t inMixerId,
                                         bool inLooping,
                                         CalibrationMode inCalibrationMode,
                                         double inLevel,
                                         int64_t inRateParameterId = PHASEInvalidInstanceHandle);

/*
 Creates a sound event switch node
 */
int64_t PHASECreateSoundEventSwitchNode(int64_t inSwitchParameterId, SwitchNodeEntry* inSwitchEntries, uint32_t inNumSwitchEntries);

/*
 Creates a sound event random node
 */
int64_t PHASECreateSoundEventRandomNode(RandomNodeEntry* inRandomEntries, uint32_t inNumRandomEntries, int64_t inUniqueSelectionQueueLength);

/*
 Creates a sound event blend node
 */
int64_t PHASECreateSoundEventBlendNode(int64_t inBlendParameterId,
                                       BlendNodeEntry* inBlendEntries,
                                       uint32_t inNumBlendEntries,
                                       bool useAutoDistanceBlend);

/*
 Creates a container node with a child sound event
 */
int64_t PHASECreateSoundEventContainerNode(int64_t* inChildIds, uint32_t inNumChildren);

/*
 Destroys a sound event node.
 */
void PHASEDestroySoundEventNode(int64_t inNodeId);

/*
 Register a sound event asset.
 */
bool PHASERegisterSoundEventAsset(const char* inName, int64_t inRootNode);

/*
 Unregister an sound event asset.
 */
void PHASEUnregisterSoundEventAsset(const char* inName);

/*
 Play a sound event instance.
 */
int64_t PHASEPlaySoundEvent(const char* inName,
                            int64_t inSourceId,
                            int64_t* inMixerIds,
                            uint64_t inNumMixers,
                            PHASESoundEventCompletionHandler callback);

/*
 Stop a sound event instance.
 */
bool PHASEStopSoundEvent(int64_t inInstance);

} //extern "C"
#endif  // PHASESoundEventInterface_h
