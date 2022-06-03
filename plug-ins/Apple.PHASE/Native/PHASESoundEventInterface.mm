//
//  PHASEInterface.mm
//  AudioPluginPHASE
//
//  Copyright © 2021 Apple Inc. All rights reserved.
//

#include "PHASESoundEventInterface.h"
#import "PHASEWrapper.h"
#import "AVFoundation/AVAudioFormat.h"

extern "C" {
bool PHASERegisterAudioBuffer(const char* inName,
                              void* inBufferData,
                              uint32_t inSampleRate,
                              uint32_t inBufferSizeInBytes,
                              uint32_t inBitDepth,
                              uint32_t inChannelCount)
{
    if (inName == nullptr || inBufferData == nullptr || inSampleRate == 0 || inBufferSizeInBytes == 0 || inBitDepth == 0 ||
        inChannelCount == 0)
    {
        NSLog(@"Failed to register audio buffer - invalid parameters.");
        return false;
    }

    NSString* UID = [NSString stringWithUTF8String:inName];
    AudioStreamBasicDescription desc;
    desc.mSampleRate = inSampleRate;
    desc.mBitsPerChannel = inBitDepth;
    desc.mBytesPerFrame = (inBitDepth / 8) * inChannelCount;
    desc.mBytesPerPacket = (inBitDepth / 8) * inChannelCount;
    desc.mChannelsPerFrame = inChannelCount;
    desc.mFormatFlags = kLinearPCMFormatFlagIsFloat;
    desc.mFormatID = kAudioFormatLinearPCM;
    desc.mFramesPerPacket = 1;
    desc.mReserved = 0;
    ChannelLayoutType layoutType;
    switch (inChannelCount)
    {
        case 1:
            layoutType = ChannelLayoutTypeMono;
            break;
        case 2:
            layoutType = ChannelLayoutTypeStereo;
            break;
        case 6:
            layoutType = ChannelLayoutTypeFiveOne;
            break;
        case 8:
            layoutType = ChannelLayoutTypeSevenOne;
            break;
        default:
            NSLog(@"Unable to create buffer with unsupported channel count of %i", inChannelCount);
            return false;
    }
    AVAudioFormat* format = [[AVAudioFormat alloc]
      initWithStreamDescription:&desc
                  channelLayout:[[AVAudioChannelLayout alloc] initWithLayoutTag:[PHASEEngineWrapper getChannelLayoutTag:layoutType]]];
    NSData* data = [NSData dataWithBytes:inBufferData length:inBufferSizeInBytes];

    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper registerAudioBufferWithData:data identifier:UID audioFormat:format];
}

void PHASEUnregisterAudioAsset(const char* inName)
{
    NSString* UID = [NSString stringWithUTF8String:inName];
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper unregisterAudioBufferWithIdentifier:UID];
}

bool PHASERegisterAudioFile(const char* inName, const char* inPath)
{
    NSArray* pathComponents = @[ [NSString stringWithUTF8String:inPath], [NSString stringWithUTF8String:inName] ];
    NSURL* url = [NSURL fileURLWithPathComponents:pathComponents];
    NSString* UID = [NSString stringWithUTF8String:inName];

    NSError* error = [NSError alloc];
    AVAudioFile* audioFile = [[AVAudioFile alloc] initForReading:url error:&error];
    if (error)
    {
        NSLog(@"Failed to register audio file with error %@.", error);
        return NO;
    }

    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper registerAudioAssetWithURL:url identifier:UID audioFormat:audioFile.fileFormat];
}

int64_t PHASECreateSpatialMixer(const char* inName,
                                 bool inEnableDirectPath,
                                 bool inEnableEarlyReflections,
                                 bool inEnableLateReverb,
                                 float inCullDistance,
                                 DirectivityModelParameters inSourceDirectivityModelParameters,
                                 DirectivityModelParameters inListenerDirectivityModelParameters)
{
    if (inName == nullptr)
    {
        return PHASEInvalidInstanceHandle;
    }


    NSString* mixerName = [NSString stringWithUTF8String:inName];
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper  createSpatialMixerWithName:mixerName
                                     enableDirectPath:inEnableDirectPath
                               enableEarlyReflections:inEnableEarlyReflections
                                     enableLateReverb:inEnableLateReverb
                                         cullDistance:inCullDistance
                     sourceDirectivityModelParameters:inSourceDirectivityModelParameters
                   listenerDirectivityModelParameters:inListenerDirectivityModelParameters];
}

int64_t PHASECreateChannelMixer(const char* inName, ChannelLayoutType inChannelLayout)
{
    if (inName == nullptr)
    {
        return PHASEInvalidInstanceHandle;
    }

    NSString* mixerName = [NSString stringWithUTF8String:inName];
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper createChannelMixerWithName:mixerName channelLayout:inChannelLayout];
}


int64_t PHASECreateAmbientMixer(const char* inName, ChannelLayoutType inChannelLayout, Quaternion inOrientation)
{
    if (inName == nullptr)
    {
        return PHASEInvalidInstanceHandle;
    }

    NSString* mixerName = [NSString stringWithUTF8String:inName];
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper createAmbientMixerWithName:mixerName channelLayout:inChannelLayout orientation:inOrientation];
}

void PHASEDestroyMixer(int64_t inMixerId)
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper destroyMixerWithId:inMixerId];
}

int64_t PHASECreateSoundEventParameterInt(const char* inParameterName, int inDefaultValue)
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    NSString* parameterName = [NSString stringWithUTF8String:inParameterName];
    return [engineWrapper createMetaParameterWithName:parameterName defaultIntValue:inDefaultValue];
}

bool PHASESetSoundEventParameterInt(int64_t inInstance, const char* inParamName, int inParamValue)
{
    NSString* paramName = [NSString stringWithUTF8String:inParamName];
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper setMetaParameterWithId:inInstance parameterName:paramName intValue:inParamValue];
}

int64_t PHASECreateSoundEventParameterDbl(const char* inParameterName, double inDefaultValue)
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    NSString* parameterName = [NSString stringWithUTF8String:inParameterName];
    return [engineWrapper createMetaParameterWithName:parameterName defaultDblValue:inDefaultValue];
}

bool PHASESetSoundEventParameterDbl(int64_t inInstance, const char* inParamName, double inParamValue)
{
    NSString* paramName = [NSString stringWithUTF8String:inParamName];
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper setMetaParameterWithId:inInstance parameterName:paramName doubleValue:inParamValue];
}

int64_t PHASECreateSoundEventParameterStr(const char* inParameterName, const char* inDefaultValue)
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    NSString* parameterName = [NSString stringWithUTF8String:inParameterName];
    NSString* defaultValue = [NSString stringWithUTF8String:inDefaultValue];
    return [engineWrapper createMetaParameterWithName:parameterName defaultStrValue:defaultValue];
}

bool PHASESetSoundEventParameterStr(int64_t inInstance, const char* inParamName, const char* inParamValue)
{
    NSString* paramName = [NSString stringWithUTF8String:inParamName];
    NSString* paramValue = [NSString stringWithUTF8String:inParamValue];
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper setMetaParameterWithId:inInstance parameterName:paramName stringValue:paramValue];
}

void PHASEDestroySoundEventParameter(int64_t inParameterId)
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper destroyMetaParameterWithId:inParameterId];
}

int64_t PHASECreateMappedMetaParameter(int64_t inParameterId, EnvelopeParameters inEnvelopeParameters)
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper createMappedMetaParameterWithParameterId:inParameterId envelopeParameters:inEnvelopeParameters];
}

void PHASEDestroyMappedMetaParameter(int64_t inParameterId)
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    [engineWrapper destoryMappedMetaParameterWithId:inParameterId];
}

int64_t PHASECreateSoundEventSamplerNode(const char* inAssetName,
                                         int64_t inMixerId,
                                         bool inLooping,
                                         CalibrationMode inCalibrationMode,
                                         double inLevel)
{
    @try
    {
        NSString* assetName = [NSString stringWithUTF8String:inAssetName];
        PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
        return [engineWrapper createSoundEventSamplerNodeWithAsset:assetName
                                                          mixerId:inMixerId
                                                           looping:inLooping
                                                   calibrationMode:inCalibrationMode
                                                             level:inLevel];
    }
    @catch (NSException* e)
    {
        NSLog(@"Unable to create Sound Event Sampler Node.");
        return PHASEInvalidInstanceHandle;
    }
}

int64_t PHASECreateSoundEventSwitchNode(int64_t inSwitchParameterId, SwitchNodeEntry* inSwitchEntries, uint32_t inNumSwitchEntries)
{
    NSMutableDictionary* switchEntries = [[NSMutableDictionary alloc] initWithCapacity:inNumSwitchEntries];
    for (uint32_t entryIdx = 0; entryIdx < inNumSwitchEntries; ++entryIdx)
    {
        int64_t nodeId = inSwitchEntries[entryIdx].nodeId;
        NSString* switchValue = [NSString stringWithUTF8String:inSwitchEntries[entryIdx].switchValue];
        [switchEntries setObject:switchValue forKey:[NSNumber numberWithLongLong:nodeId]];
    }

    @try
    {
        PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
        return [engineWrapper createSoundEventSwitchNodeWithParameter:inSwitchParameterId switchEntries:switchEntries];
    }
    @catch (NSException* e)
    {
        NSLog(@"Unable to create Sound Event Switch Node.");
        return PHASEInvalidInstanceHandle;
    }
}

int64_t PHASECreateSoundEventRandomNode(RandomNodeEntry* inRandomEntries, uint32_t inNumRandomEntries)
{
    NSMutableDictionary* randomEntries = [[NSMutableDictionary alloc] initWithCapacity:inNumRandomEntries];
    for (uint32_t entryIdx = 0; entryIdx < inNumRandomEntries; ++entryIdx)
    {
        int64_t nodeId = inRandomEntries[entryIdx].nodeId;
        NSNumber* weight = [NSNumber numberWithFloat:inRandomEntries[entryIdx].weight];
        [randomEntries setObject:weight forKey:[NSNumber numberWithLongLong:nodeId]];
    }

    @try
    {
        PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
        return [engineWrapper createSoundEventRandomNodeWithEntries:randomEntries];
    }
    @catch (NSException* e)
    {
        NSLog(@"Unable to create Sound Event Random Node.");
        return PHASEInvalidInstanceHandle;
    }
}

int64_t PHASECreateSoundEventBlendNode(int64_t inBlendParameterId,
                                       BlendNodeEntry* inBlendEntries,
                                       uint32_t inNumBlendEntries,
                                       bool useAutoDistanceBlend)
{
    @try
    {
        PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
        return [engineWrapper createSoundEventBlendNodeWithParameter:inBlendParameterId
                                                         blendRanges:inBlendEntries
                                                           numRanges:inNumBlendEntries
                                                useAutoDistanceBlend:useAutoDistanceBlend];
    }
    @catch (NSException* e)
    {
        NSLog(@"Unable to create Sound Event Blend Node.");
        return PHASEInvalidInstanceHandle;
    }
}

int64_t PHASECreateSoundEventContainerNode(int64_t* inSubtreeIds, uint32_t inNumSubtrees)
{
    @try
    {
        PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
        return [engineWrapper createSoundEventContainerNodeWithSubtree:inSubtreeIds numSubtrees:inNumSubtrees];
    }
    @catch (NSException* exception)
    {
        NSLog(@"Unable to create Sound Event Container Node");
        return PHASEInvalidInstanceHandle;
    }
}

void PHASEDestroySoundEventNode(int64_t inNodeId)
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    [engineWrapper destroySoundEventNodeWithId:inNodeId];
}

bool PHASERegisterSoundEventAsset(const char* inName, int64_t inRootNode)
{
    NSString* name = [NSString stringWithUTF8String:inName];
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper registerSoundEventWithName:name rootNodeId:inRootNode];
}

void PHASEUnregisterSoundEventAsset(const char* inName)
{
    NSString* name = [NSString stringWithUTF8String:inName];
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    [engineWrapper unregisterSoundEventWithName:name];
}

int64_t PHASEPlaySoundEvent(const char* inName,
                            int64_t inSourceId,
                            int64_t* inMixerIds,
                            uint64_t inNumMixers,
                            PHASESoundEventCompletionHandler completionHandler)
{
    @try
    {
        NSString* name = [NSString stringWithUTF8String:inName];
        PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
        return [engineWrapper playSoundEventWithName:name
                                            sourceId:inSourceId
                                            mixerIds:inMixerIds
                                           numMixers:inNumMixers
                                   completionHandler:completionHandler];
    }
    @catch (NSException* exception)
    {
        NSLog(@"Unable to play Sound Event %@.", [NSString stringWithUTF8String:inName]);
        NSLog(@"%@", exception);
        return PHASEInvalidInstanceHandle;
    }
}

bool PHASEStopSoundEvent(int64_t inInstance)
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper stopSoundEventWithId:inInstance];
}
}
