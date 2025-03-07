#include "AudioPluginUtil.h"
#include "PHASEWrapper.h"
#include "PHASEWrapperRingBuffer.h"

namespace PHASESpatializer
{
    // Static helpers
    static simd_float4x4 kRhMatrix = {
        simd_float4{ 1.0f, 0.0f, 0.0f, 0.0f },
        simd_float4{ 0.0f, 1.0f, 0.0f, 0.0f },
        simd_float4{ 0.0f, 0.0f, -1.0f, 0.0f },
        simd_float4{ 0.0f, 0.0f, 0.0f, 1.0f },
    };
    static const NSString* kSpatialMixerBaseName = @"PHASESpatialMixer";
    static const NSString* kPullStreamNodeBaseName = @"PHASEPullStream";
    static const uint16_t kNumBuffers = 4;

    enum
    {
        P_AUDIOSRCATTN,
        P_FIXEDVOLUME,
        P_CUSTOMFALLOFF,
        P_NUM
    };

    struct EffectData
    {
        float p[P_NUM];
        
        // Objects
        int64_t mSourceId = PHASEInvalidInstanceHandle;
        int64_t mStreamNodeId = PHASEInvalidInstanceHandle;
        int64_t mSoundEventId = PHASEInvalidInstanceHandle;
        int64_t mMixerId = PHASEInvalidInstanceHandle;
        
        // Asset / Mixer / Parameter names
        NSString* mAssetName;
        NSString* mMixerName;
        NSString* mPullStreamNodeName;
        NSString* mEarlyReflectionsSendMetaParameterName;
        NSString* mLateReverbSendMetaParameterName;
        
        // Buffer manager and render block
        PHASEWrapperRingBuffer* mRingBuffer;
        PHASEPullStreamRenderBlock mPullStreamRenderBlock;
        
        // Engine wraper cache
        PHASEEngineWrapper* mEngineWrapper;
        
        bool reverbInit = true;
    };

    inline bool IsHostCompatible(UnityAudioEffectState* state)
    {
        // Somewhat convoluted error checking here because hostapiversion is only supported from SDK version 1.03 (i.e. Unity 5.2) and onwards.
        // Since we are only checking for version 0x010300 here, we can't use newer fields in the UnityAudioSpatializerData struct, such as minDistance and maxDistance.
        return
            state->structsize >= sizeof(UnityAudioEffectState) &&
            state->hostapiversion >= 0x010300;
    }

    int InternalRegisterEffectDefinition(UnityAudioEffectDefinition& definition)
    {
        int numparams = P_NUM;
        definition.paramdefs = new UnityAudioParameterDefinition[numparams];
        AudioPluginUtil::RegisterParameter(definition, "AudioSrc Attn", "", 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, P_AUDIOSRCATTN, "AudioSource distance attenuation");
        AudioPluginUtil::RegisterParameter(definition, "Fixed Volume", "", 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, P_FIXEDVOLUME, "Fixed volume amount");
        AudioPluginUtil::RegisterParameter(definition, "Custom Falloff", "", 0.0f, 1.0f, 0.0f, 1.0f, 1.0f, P_CUSTOMFALLOFF, "Custom volume falloff amount (logarithmic)");
        definition.flags |= UnityAudioEffectDefinitionFlags_IsSpatializer;
        return numparams;
    }

    static UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK DistanceAttenuationCallback(UnityAudioEffectState* state, float distanceIn, float attenuationIn, float* attenuationOut)
    {
        EffectData* data = state->GetEffectData<EffectData>();
        *attenuationOut =
            data->p[P_AUDIOSRCATTN] * attenuationIn +
            data->p[P_FIXEDVOLUME] +
            data->p[P_CUSTOMFALLOFF] * (1.0f / AudioPluginUtil::FastMax(1.0f, distanceIn));
        return UNITY_AUDIODSP_OK;
    }

    // Instatiate the plug-in
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK CreateCallback(UnityAudioEffectState* state)
    {
        EffectData* effectData = new EffectData;
        state->effectdata = effectData;
        
        // Cache the engine wrapper
        PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
        
        // Create a point source
        effectData->mSourceId = [engineWrapper createSource];
        
        // Create the mixer
        effectData->mMixerName = [kSpatialMixerBaseName stringByAppendingFormat:@"%lld",effectData->mSourceId];
        DirectivityModelParameters sourceDirectivity { DirectivityType::None, 0, nullptr };
        DirectivityModelParameters listenerDirectivity { DirectivityType::None, 0, nullptr };
        effectData->mMixerId = [engineWrapper  createSpatialMixerWithName:effectData->mMixerName
                                                         enableDirectPath:true
                                                   enableEarlyReflections:true
                                                         enableLateReverb:true
                                                             cullDistance:state->spatializerdata->maxDistance
                                                            rolloffFactor:0.0 // Disable rolloff since Unity is doing distance attenuation
                                         sourceDirectivityModelParameters:sourceDirectivity
                                       listenerDirectivityModelParameters:listenerDirectivity];
        
        // Cache the ER / LR metaparameters for reverb mixing
        effectData->mEarlyReflectionsSendMetaParameterName = [effectData->mMixerName stringByAppendingFormat:@"%s", "EarlyReflectionsSend"];
        effectData->mLateReverbSendMetaParameterName = [effectData->mMixerName stringByAppendingFormat:@"%s", "LateReverbSend"];
        
        // Create the pull stream node
        AVAudioChannelLayout* layout = [[AVAudioChannelLayout alloc] initWithLayoutTag:kAudioChannelLayoutTag_Stereo];
        AudioStreamBasicDescription desc = { 0 };
        desc.mSampleRate = state->samplerate;
        desc.mFormatID = kAudioFormatLinearPCM;
        desc.mFormatFlags = kLinearPCMFormatFlagIsFloat | kLinearPCMFormatFlagIsNonInterleaved;
        desc.mBitsPerChannel = 32;
        desc.mChannelsPerFrame = 2;
        desc.mFramesPerPacket = 1;
        desc.mBytesPerFrame = 32 / 8;
        desc.mBytesPerPacket = desc.mBytesPerFrame * desc.mFramesPerPacket;
        AVAudioFormat* format = [[AVAudioFormat alloc]
                                  initWithStreamDescription:&desc
                                  channelLayout:layout];
        
        @try
        {
            effectData->mPullStreamNodeName = [kPullStreamNodeBaseName stringByAppendingFormat:@"%lld", effectData->mMixerId];
            effectData->mStreamNodeId = [engineWrapper createSoundEventPullStreamNodeWithAsset:effectData->mPullStreamNodeName
                                                                  mixerId:effectData->mMixerId
                                                                   format:format
                                                          calibrationMode:CalibrationMode::CalibrationModeNone
                                                                    level:1.0f];
        }
        @catch (NSException* e)
        {
            NSLog(@"PHASE Spatializer Plugin: Unable to create Sound Event Pull Stream Node with exception: %@.", e.reason);
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        }
       
        // Register the sound event asset
        effectData->mAssetName = [NSString stringWithFormat:@"SpatializerSoundEvent-%llu", effectData->mStreamNodeId];
        if (NO == [engineWrapper registerSoundEventWithName:effectData->mAssetName rootNodeId:effectData->mStreamNodeId])
        {
            NSLog(@"PHASE Spatializer Plugin: Unable to register Sound Event Asset %@", effectData->mAssetName);
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        }
        
        // Create a buffer manager to track reads and writes
        PHASEWrapperRingBuffer* ringBuffer = [[PHASEWrapperRingBuffer alloc] initWithFrameSize:state->dspbuffersize numberOfBuffers:kNumBuffers format:format];
        if (nil == ringBuffer)
        {
            NSLog(@"PHASE Spatializer Plugin: Unable to create ring buffer.");
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        }
        effectData->mRingBuffer = ringBuffer;
        
        // Cache the render block
        effectData->mPullStreamRenderBlock = ^OSStatus(BOOL* _Nonnull isSilence,
                                                     const AudioTimeStamp* _Nonnull timestamp,
                                                     AVAudioFrameCount frameCount,
                                                     AudioBufferList* _Nonnull outputData) {
            
            bool procError = false;
            
            // Does the timestamp has valid sample and host time?
            if ((timestamp->mFlags & kAudioTimeStampSampleTimeValid) <= 0)
            {
                NSLog(@"PHASE Spatializer Plugin: Invalid timestamp sample: %llu", timestamp->mHostTime);
                procError = true;
            }
            
            if ((timestamp->mHostTime) <= 0)
            {
                NSLog(@"PHASE Spatializer Plugin: Invalid host time %llu", timestamp->mHostTime);
                procError = true;
            }
            
            BOOL success = [effectData->mRingBuffer read:outputData frameCount:frameCount];
                
            // Check if the buffer if valid
            if (NO == success || procError)
            {
                *isSilence = true;
                memset(static_cast<float*>(outputData->mBuffers[0].mData), 0, sizeof(float) * frameCount);
                memset(static_cast<float*>(outputData->mBuffers[1].mData), 0, sizeof(float) * frameCount);
                return procError ? kAudio_ParamError : kAudio_NoError;
            }
        
            return noErr;
        };
        
        // Cache the engine wrapper
        effectData->mEngineWrapper = engineWrapper;
        
        // Register distance attenuation callback
        if (IsHostCompatible(state))
            state->spatializerdata->distanceattenuationcallback = DistanceAttenuationCallback;
        AudioPluginUtil::InitParametersFromDefinitions(InternalRegisterEffectDefinition, effectData->p);
        
        return UNITY_AUDIODSP_OK;
    }

    // Destroy plug-in instance
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ReleaseCallback(UnityAudioEffectState* state)
    {
        EffectData* effectData = state->GetEffectData<EffectData>();
        
        // Destroy the objects
        if (nil != effectData)
        {
            PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
            [engineWrapper stopSoundEventWithId:effectData->mSoundEventId];
        }
        
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK SetFloatParameterCallback(UnityAudioEffectState* state, int index, float value)
    {
        EffectData* data = state->GetEffectData<EffectData>();
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        data->p[index] = value;
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK GetFloatParameterCallback(UnityAudioEffectState* state, int index, float* value, char *valuestr)
    {
        EffectData* data = state->GetEffectData<EffectData>();
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        if (value != NULL)
            *value = data->p[index];
        if (valuestr != NULL)
            valuestr[0] = 0;
        return UNITY_AUDIODSP_OK;
    }

    int UNITY_AUDIODSP_CALLBACK GetFloatBufferCallback(UnityAudioEffectState* state, const char* name, float* buffer, int numsamples)
    {
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ProcessCallback(UnityAudioEffectState* state, float* inbuffer, float* outbuffer, unsigned int length, int inchannels, int outchannels)
    {
        // Check that I/O formats are right and that the host API supports this feature
        if (inchannels != 2 || outchannels != 2 ||
            !IsHostCompatible(state) || state->spatializerdata == NULL)
        {
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        }
        
        EffectData* effectData = state->GetEffectData<EffectData>();
        
        // Play the sound event here so we can start pulling immediately
        bool wasInitialized = (effectData->mSoundEventId != PHASEInvalidInstanceHandle);
        if (false == wasInitialized)
        {
            if ([effectData->mEngineWrapper isInitialized])
            {
                @try
                {
                    effectData->mSoundEventId = [effectData->mEngineWrapper playSoundEventWithName:effectData->mAssetName
                                                                                          sourceId:effectData->mSourceId
                                                                                          mixerIds:&effectData->mMixerId
                                                                                         numMixers:1
                                                                                        streamName:effectData->mPullStreamNodeName
                                                                                       renderBlock:effectData->mPullStreamRenderBlock
                                                                            completionHandlerBlock:^(PHASESoundEventStartHandlerReason reason, int64_t sourceId, int64_t soundEventId) {
                        NSLog(@"Finished playing back sound event with reason %ld, sourceId %llu, soundEventId %llu.",  static_cast<long>(reason), sourceId, soundEventId);
                        if (nil != effectData)
                        {
                            PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
                            [engineWrapper destroySourceWithId:effectData->mSourceId];
                            [engineWrapper destroyMixerWithId:effectData->mMixerId];
                            [engineWrapper unregisterSoundEventWithName:effectData->mAssetName];
                            effectData->mRingBuffer = nil;
                            delete effectData;
                        }
                        
                    }];
                }
                @catch (NSException* exception)
                {
                    NSLog(@"Unable to play Sound Event with asset %@ due to exception: %@.", effectData->mAssetName, exception);\
                    return UNITY_AUDIODSP_ERR_UNSUPPORTED;
                }
            }
        }
        
        // Set source position
        float* sourceMatrix = &state->spatializerdata->sourcematrix[0];
        simd_float4x4 sourceTransform = {
            simd_float4{ sourceMatrix[0], sourceMatrix[1], sourceMatrix[2], sourceMatrix[3] },
            simd_float4{ sourceMatrix[4], sourceMatrix[5], sourceMatrix[6], sourceMatrix[7] },
            simd_float4{ sourceMatrix[8], sourceMatrix[9], sourceMatrix[10], sourceMatrix[11] },
            simd_mul(simd_float4{ sourceMatrix[12], sourceMatrix[13], sourceMatrix[14], sourceMatrix[15] }, kRhMatrix)
        };
        
        if (NO == [effectData->mEngineWrapper setSourceTransformWithId:effectData->mSourceId transform:sourceTransform])
        {
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        }
        
        // Set reverb levels
        if (NO == [effectData->mEngineWrapper setMetaParameterWithId:effectData->mSoundEventId parameterName:effectData->mEarlyReflectionsSendMetaParameterName doubleValue:effectData->reverbInit?1.0:state->spatializerdata->reverbzonemix])
        {
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        }
        
        if (NO == [effectData->mEngineWrapper setMetaParameterWithId:effectData->mSoundEventId parameterName:effectData->mLateReverbSendMetaParameterName doubleValue:effectData->reverbInit?1.0:state->spatializerdata->reverbzonemix])
        {
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        }
        effectData->reverbInit = false;
        // If we just initialized trigger an update so we start the sound faster and flush the correct source position
        if (false == wasInitialized)
        {
            [effectData->mEngineWrapper update];
        }

        // Write into our buffers
        [effectData->mRingBuffer write:inbuffer frameCount:length];
        // return silence to unity
        memset(outbuffer, 0, sizeof(float) * length * outchannels);
        
        return UNITY_AUDIODSP_OK;
    }
}
