//
//  AudioPluginPHASE_Tests.mm
//  AudioPluginPHASE
//
//  Copyright © 2019 Apple Inc. All rights reserved.
//

#import <XCTest/XCTest.h>

#include "PHASEWrapper.h"
#include "PhaseInterface.h"
#include "PhaseSoundEventInterface.h"
#include <thread>
#include <chrono>
#include "PHASEWrapperRingBuffer.h"

enum PHASEMixerType
{
    AmbientMixer = 0,
    ChannelMixer = 1,
    SpatialMixer = 2
};

@interface AudioPluginPHASE_Tests : XCTestCase {
    struct MeshData
    {
        int mVertexCount;
        float* mVertexPositions;
        float* mVertexNormals;
        uint32_t mIndexCount;
        uint32_t* mIndices;
    };
    MeshData mMeshData;
    AudioBuffer mAudioBuffer;
    DirectivityModelParameters mDirectivityModelParameters;
}
@end

@implementation AudioPluginPHASE_Tests

- (void)setUp
{
    // Create mesh
    simd_float3 extent = { 1.0f, 1.0f, 1.0f };
    simd_uint3 segments = { 1, 1, 1 };
    MDLMesh* mesh = [[MDLMesh alloc] initBoxWithExtent:extent
                                              segments:segments
                                         inwardNormals:false
                                          geometryType:MDLGeometryTypeTriangles
                                             allocator:nil];
    mMeshData.mVertexCount = static_cast<int>([mesh vertexCount]);

    // Get the position data from the mesh and allocate a contiguous buffer to pass to the interface
    MDLVertexAttributeData* positionData = [mesh vertexAttributeDataForAttributeNamed:MDLVertexAttributePosition];
    const float* positionDataPtr = static_cast<const float*>([positionData dataStart]);
    mMeshData.mVertexPositions = static_cast<float*>(malloc(mMeshData.mVertexCount * sizeof(float) * 3));

    // Get the normal data from the mesh and allocate a contiguous buffer to pass to the interface
    MDLVertexAttributeData* normalData = [mesh vertexAttributeDataForAttributeNamed:MDLVertexAttributeNormal];
    const float* normalDataPtr = static_cast<const float*>([normalData dataStart]);
    mMeshData.mVertexNormals = static_cast<float*>(malloc(mMeshData.mVertexCount * sizeof(float) * 3));

    // Go through the vertices and pack the data for positions and normals
    for (int vertIdx = 0, elIdx = 0; vertIdx < mMeshData.mVertexCount; ++vertIdx, elIdx += 3)
    {
        const int posStride = vertIdx * (static_cast<int>([positionData stride]) / sizeof(float));
        mMeshData.mVertexPositions[elIdx] = *(positionDataPtr + posStride);
        mMeshData.mVertexPositions[elIdx + 1] = *(positionDataPtr + posStride + 1);
        mMeshData.mVertexPositions[elIdx + 2] = *(positionDataPtr + posStride + 2);

        const int normStride = vertIdx * (static_cast<int>([normalData stride]) / sizeof(float));
        mMeshData.mVertexNormals[elIdx] = *(normalDataPtr + normStride);
        mMeshData.mVertexNormals[elIdx + 1] = *(normalDataPtr + normStride + 1);
        mMeshData.mVertexNormals[elIdx + 2] = *(normalDataPtr + normStride + 2);
    }

    // Get the index data from the submesh
    MDLSubmesh* submesh = [[mesh submeshes] objectAtIndex:0];
    MDLMeshBufferData* indexData = [submesh indexBuffer];
    const uint16_t* indexDataPtr = static_cast<const uint16_t*>([[indexData data] bytes]);

    // The data in the index buffer is stored as a uint16 so build a uint32_t array for the plugin
    mMeshData.mIndexCount = static_cast<uint32_t>([submesh indexCount]);
    mMeshData.mIndices = static_cast<uint32_t*>(malloc(mMeshData.mIndexCount * sizeof(uint32_t)));
    for (int idx = 0; idx < mMeshData.mIndexCount; ++idx)
    {
        mMeshData.mIndices[idx] = indexDataPtr[idx];
    }

    // Create a test buffer
    mAudioBuffer.mDataByteSize = 48000 * sizeof(float);
    mAudioBuffer.mData = malloc(mAudioBuffer.mDataByteSize);
    mAudioBuffer.mNumberChannels = 1;

    float* fltPtr = (float*) mAudioBuffer.mData;
    for (int i = 0; i < 48000; ++i)
    {
        fltPtr[i] = sin((M_PI * 2.0f * 440.0f) / 48000.0f * i);
    }

    PHASEStart();
}

- (void)tearDown
{
    free(mMeshData.mIndices);
    free(mMeshData.mVertexNormals);
    free(mMeshData.mVertexPositions);
    free(mAudioBuffer.mData);

    PHASEStop();
}

- (void)helperTestCreateAndSetInitialMixerGainParameter:(PHASEMixerType)mixerType gainToSet:(double)gainToSet
{
    bool gainToSetInValidRange = gainToSet >= 0.0 && gainToSet <= 1.0;
    char mixerName[] = "TestMixerName";
    char gainParamName[] = "MixerGain";
    
    int64_t mixerId = PHASEInvalidInstanceHandle;
    switch (mixerType)
    {
        case PHASEMixerType::AmbientMixer:
            mixerId = PHASECreateAmbientMixer(mixerName, ChannelLayoutTypeMono, { 0, 0, 0, 1 });
            break;
        case PHASEMixerType::ChannelMixer:
            mixerId = PHASECreateChannelMixer(mixerName, ChannelLayoutTypeMono);
            break;
        case PHASEMixerType::SpatialMixer:
            mixerId = PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, 1.0f, mDirectivityModelParameters, mDirectivityModelParameters);
            break;
    }
    
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);
    
    // Create the mixer gain parameter and set the metaGainParameter on the soundevent
    const int64_t gainParameterId = PHASECreateSoundEventParameterDbl(gainParamName, gainToSet, 0.0, 1.0);
    if (gainToSetInValidRange)
    {
        XCTAssert(gainParameterId != PHASEInvalidInstanceHandle);
    }
    else
    {
        XCTAssert(gainParameterId == PHASEInvalidInstanceHandle);
    }
    
    bool result = PHASESetMixerGainMetaParameter(gainParameterId, mixerId);
    if (gainToSetInValidRange)
    {
        XCTAssert(result == true);
    }
    else
    {
        XCTAssert(result == false);
    }
    
    // Create the listener
    result = PHASECreateListener();
    XCTAssert(result == true);

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Register the buffer asset
    char assetName[] = "TestBufferAsset";
    result = PHASERegisterAudioBuffer(assetName, mAudioBuffer.mData, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char soundEventName[] = "TestSoundEvent";
    result = PHASERegisterSoundEventAsset(soundEventName, samplerId);
    XCTAssert(result == true);

    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyPlaySoundEventCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent(soundEventName, sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    
    const double mixerGain = PHASEGetSoundEventParameterDbl(instance, gainParamName);
    
    if (gainToSetInValidRange)
    {
        XCTAssert(mixerGain == gainToSet);
    }
    else
    {
        XCTAssert(mixerGain != gainToSet);
    }
    
    result = PHASEStopSoundEvent(instance);
    XCTAssert(result == true);

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(soundEventName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the source
    PHASEDestroySource(sourceId);

    // Destroy the listener
    PHASEDestroyListener();

    // Destory gain parameter
    PHASEDestroySoundEventParameter(gainParameterId);
    
    // Destroy the mixer
    PHASEDestroyMixer(mixerId);
}


- (void)helperTestSetDynamicallyMixerGainParameter:(PHASEMixerType)mixerType gainToSet:(double)gainToSet
{
    bool gainToSetInValidRange = gainToSet >= 0.0 && gainToSet <= 1.0;
    double initialGain = 0.5f;
    char gainParamName[] = "MixerGain";
    
    // Create the listener
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Register the buffer asset
    char assetName[] = "TestBufferAsset";
    result = PHASERegisterAudioBuffer(assetName, mAudioBuffer.mData, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    char mixerName[] = "TestMixername";
    int64_t mixerId = PHASEInvalidInstanceHandle;
    
    switch (mixerType)
    {
        case PHASEMixerType::AmbientMixer:
            mixerId = PHASECreateAmbientMixer(mixerName, ChannelLayoutTypeMono, { 0, 0, 0, 1 });
            break;
        case PHASEMixerType::ChannelMixer:
            mixerId = PHASECreateChannelMixer(mixerName, ChannelLayoutTypeMono);
            break;
        case PHASEMixerType::SpatialMixer:
            mixerId = PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, 1.0f, mDirectivityModelParameters, mDirectivityModelParameters);
            break;
    }
   
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);
    
    // Create the mixer gain parameter and set the metaGainParameter on the soundevent
    const int64_t gainParameterId = PHASECreateSoundEventParameterDbl(gainParamName, initialGain, 0.0, 1.0);
    XCTAssert(gainParameterId != PHASEInvalidInstanceHandle);
    
    result = PHASESetMixerGainMetaParameter(gainParameterId, mixerId);
    XCTAssert(result == true);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char soundEventName[] = "TestSoundEvent";
    result = PHASERegisterSoundEventAsset(soundEventName, samplerId);
    XCTAssert(result == true);

    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyPlaySoundEventCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent(soundEventName, sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);

    for (int i = 0; i < 100; ++i)
    {
        PHASEUpdate();

        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));

        if (i == 50)
        {
            // Dynamically change the mixer gain
            bool result = PHASESetSoundEventParameterDbl(instance, gainParamName, gainToSet);
            XCTAssert(result == true);
            double updatedGain = PHASEGetSoundEventParameterDbl(instance, gainParamName);
            
            if(gainToSetInValidRange)
            {
                XCTAssert(updatedGain == gainToSet);
            }
            else
            {
                XCTAssert(updatedGain != gainToSet); //value should have been clamped by PHASE engine
            }
        }
    }
    
    result = PHASEStopSoundEvent(instance);
    XCTAssert(result == true);

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(soundEventName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the source
    PHASEDestroySource(sourceId);

    // Destroy the listener
    PHASEDestroyListener();
    
    // Destory gain parameter
    PHASEDestroySoundEventParameter(gainParameterId);
    
    // Destroy the mixer
    PHASEDestroyMixer(mixerId);
}

- (void)helperTestCreateAndSetInitialSamplerRateParameter:(double)rateToSet
{
    bool rateToSetInValidRange = rateToSet >= 0.25 && rateToSet <= 4.0;
    char rateParamName[] = "SamplerRate";
    
    int64_t mixerId = PHASECreateChannelMixer("TestMixerName", ChannelLayoutTypeMono);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);
    
    // Create the sampler rate parameter and set the rateMetaParameter on the soundevent
    const int64_t rateParameterId = PHASECreateSoundEventParameterDbl(rateParamName, rateToSet, 0.25, 4.0);
    if (rateToSetInValidRange)
    {
        XCTAssert(rateParameterId != PHASEInvalidInstanceHandle);
    }
    else
    {
        XCTAssert(rateParameterId == PHASEInvalidInstanceHandle);
    }
    
    // Create the listener
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Register the buffer asset
    char assetName[] = "TestBufferAsset";
    result = PHASERegisterAudioBuffer(assetName, mAudioBuffer.mData, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == true);
    
    // Create the sampler node with the rateMetaParameter
    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1, rateParameterId);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char soundEventName[] = "TestSoundEvent";
    result = PHASERegisterSoundEventAsset(soundEventName, samplerId);
    XCTAssert(result == true);

    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyPlaySoundEventCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent(soundEventName, sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    
    PHASEUpdate();
    std::this_thread::sleep_for(std::chrono::milliseconds(1000));
    
    // Verify the sampler rate was set correctly on the sound event instance
    if (rateToSetInValidRange)
    {
        XCTAssert(PHASEGetSoundEventParameterDbl(instance, rateParamName) == rateToSet);
    }
    
    result = PHASEStopSoundEvent(instance);
    XCTAssert(result == true);

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(soundEventName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the source
    PHASEDestroySource(sourceId);

    // Destroy the listener
    PHASEDestroyListener();

    // Destory gain parameter
    PHASEDestroySoundEventParameter(rateParameterId);
    
    // Destroy the mixer
    PHASEDestroyMixer(mixerId);
}


- (void)helperTestSetDynamicallySamplerRateParameter:(double)rateToSet
{
    bool rateToSetInValidRange = rateToSet >= 0.25 && rateToSet <= 4.0;
    double initialRate = 1.0f;
    char rateParamName[] = "SamplerRate";
    
    // Create the listener
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Register the buffer asset
    char assetName[] = "TestBufferAsset";
    result = PHASERegisterAudioBuffer(assetName, mAudioBuffer.mData, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    int64_t mixerId = PHASECreateChannelMixer("TestMixername", ChannelLayoutTypeMono);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);
    
    // Create the sampler rate parameter and set the rateMetaParameter on the soundevent
    const int64_t rateParameterId = PHASECreateSoundEventParameterDbl(rateParamName, initialRate, 0.25, 4.0);
    XCTAssert(rateParameterId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1, rateParameterId);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char soundEventName[] = "TestSoundEvent";
    result = PHASERegisterSoundEventAsset(soundEventName, samplerId);
    XCTAssert(result == true);

    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyPlaySoundEventCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent(soundEventName, sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    
    double rate = PHASEGetSoundEventParameterDbl(instance, rateParamName);
    XCTAssert(rate == initialRate);

    for (int i = 0; i < 100; ++i)
    {
        PHASEUpdate();

        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));

        if (i == 50)
        {
            // Dynamically change the mixer gain
            bool result = PHASESetSoundEventParameterDbl(instance, rateParamName, rateToSet);
            XCTAssert(result == true);
            double updatedRate = PHASEGetSoundEventParameterDbl(instance, rateParamName);
            
            if(rateToSetInValidRange)
            {
                XCTAssert(updatedRate == rateToSet);
            }
            else
            {
                XCTAssert(updatedRate != rateToSet); //value should have been clamped by PHASE engine
            }
        }
    }
    
    result = PHASEStopSoundEvent(instance);
    XCTAssert(result == true);

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(soundEventName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the source
    PHASEDestroySource(sourceId);

    // Destroy the listener
    PHASEDestroyListener();
    
    // Destory gain parameter
    PHASEDestroySoundEventParameter(rateParameterId);
    
    // Destroy the mixer
    PHASEDestroyMixer(mixerId);
}

- (void)testCreateDestroyListener
{

    // Create the listener once - it should initialize the singleton engine and create successfully
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Destroy the listener - should succeed.
    result = PHASEDestroyListener();
    XCTAssert(result == true);

    // Destroy the listener again - should fail as it no longer exists.
    result = PHASEDestroyListener();
    XCTAssert(result == false);
}

- (void)testSetListenerTransform
{

    Matrix4x4 transform = { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f };

    // Try and set the transform - should fail as no listener yet
    bool result = PHASESetListenerTransform(transform);
    XCTAssert(result == false);

    // Create the listener
    result = PHASECreateListener();
    XCTAssert(result == true);

    // Try and set the transform - should succeed
    result = PHASESetListenerTransform(transform);
    XCTAssert(result == true);

    result = PHASEDestroyListener();
    XCTAssert(result == true);
}

- (void)testSetListenerGain
{
    double gain = 0.5f;

    // Try and set the gain - should fail as no listener yet
    bool result = PHASESetListenerGain(gain);
    XCTAssert(result == false);

    // Create the listener
    result = PHASECreateListener();
    XCTAssert(result == true);

    // Try and set the gain - should succeed
    result = PHASESetListenerGain(gain);
    XCTAssert(result == true);

    result = PHASEDestroyListener();
    XCTAssert(result == true);
}

- (void)testTrySetListenerGainWithInvalidValue
{
    double gain = -1.0f;
    
    // Create the listener
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Try and set the gain outside valid range of [0,1] - should clamp the value
    result = PHASESetListenerGain(gain);
    XCTAssert(result == true);
    
    const double listenerGain = PHASEGetListenerGain();
    XCTAssert(listenerGain >= 0.0 && listenerGain <= 1.0);
    
    result = PHASEDestroyListener();
    XCTAssert(result == true);
}

- (void)testCreateDestroySource
{
    // Create source with 0 verts
    int64_t invalidSource =
      PHASECreateVolumetricSource(0, mMeshData.mVertexPositions, mMeshData.mVertexNormals, mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(invalidSource == PHASEInvalidInstanceHandle);

    // Create source with no positions
    invalidSource =
      PHASECreateVolumetricSource(mMeshData.mVertexCount, nullptr, mMeshData.mVertexNormals, mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(invalidSource == PHASEInvalidInstanceHandle);

    // Create source with no normals
    invalidSource =
      PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, nullptr, mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(invalidSource == PHASEInvalidInstanceHandle);

    // Create source with 0 indices
    invalidSource =
      PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals, 0, mMeshData.mIndices);
    XCTAssert(invalidSource == PHASEInvalidInstanceHandle);

    // Create source with no indices
    invalidSource = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                mMeshData.mIndexCount, nullptr);
    XCTAssert(invalidSource == PHASEInvalidInstanceHandle);

    // Create a bunch of sources - should succeed
    int64_t sourceId1 = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                    mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(sourceId1 != PHASEInvalidInstanceHandle);

    int64_t sourceId2 = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                    mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(sourceId2 != PHASEInvalidInstanceHandle);

    int64_t sourceId3 = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                    mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(sourceId3 != PHASEInvalidInstanceHandle);

    // Destroy the sources
    PHASEDestroySource(sourceId1);
    PHASEDestroySource(sourceId2);
    PHASEDestroySource(sourceId3);
}

- (void)testCreateDestroyPointSource
{
    // Create a bunch of sources - should succeed
    int64_t sourceId1 = PHASECreatePointSource();
    XCTAssert(sourceId1 != PHASEInvalidInstanceHandle);

    int64_t sourceId2 = PHASECreatePointSource();
    XCTAssert(sourceId2 != PHASEInvalidInstanceHandle);

    int64_t sourceId3 = PHASECreatePointSource();
    XCTAssert(sourceId3 != PHASEInvalidInstanceHandle);

    // Destroy the sources
    PHASEDestroySource(sourceId1);
    PHASEDestroySource(sourceId2);
    PHASEDestroySource(sourceId3);
}

- (void)testSetSourceTransform
{
    Matrix4x4 transform = { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f };

    // Try and set the transform - should fail as invalid sourceId
    bool result = PHASESetSourceTransform(0, transform);
    XCTAssert(result == false);

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Try and set the transform - should succeed
    result = PHASESetSourceTransform(sourceId, transform);
    XCTAssert(result == true);

    PHASEDestroySource(sourceId);
}

- (void)testSetSourceGain
 {
     double gain = 0.5f;
     
     // Try and set the gain - should fail as invalid sourceId
     bool result = PHASESetSourceGain(0, gain);
     XCTAssert(result == false);

     // Create a source
     const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                          mMeshData.mIndexCount, mMeshData.mIndices);
     XCTAssert(sourceId != PHASEInvalidInstanceHandle);

     // Try and set the gain - should succeed
     result = PHASESetSourceGain(sourceId, gain);
     XCTAssert(result == true);
     
     PHASEDestroySource(sourceId);
 }

 - (void)testTrySetSourceGainWithInvalidValue
 {
     double gain = -1.0f;

     // Create a source
     const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                          mMeshData.mIndexCount, mMeshData.mIndices);
     XCTAssert(sourceId != PHASEInvalidInstanceHandle);

     // Try and set the gain outside valid range of [0,1] - should clamp the value
     bool result = PHASESetSourceGain(sourceId, gain);
     XCTAssert(result == true);
     
     const double sourceGain = PHASEGetSourceGain(sourceId);
     XCTAssert(sourceGain >= 0.0 && sourceGain <= 1.0);
     
     PHASEDestroySource(sourceId);
 }

- (void)testCreateDestroyOccluder
{
    // Create source with 0 verts
    int64_t invalidOccluder =
      PHASECreateOccluder(0, mMeshData.mVertexPositions, mMeshData.mVertexNormals, mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(invalidOccluder == PHASEInvalidInstanceHandle);

    // Create source with no positions
    invalidOccluder =
      PHASECreateOccluder(mMeshData.mVertexCount, nullptr, mMeshData.mVertexNormals, mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(invalidOccluder == PHASEInvalidInstanceHandle);

    // Create source with no normals
    invalidOccluder =
      PHASECreateOccluder(mMeshData.mVertexCount, mMeshData.mVertexPositions, nullptr, mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(invalidOccluder == PHASEInvalidInstanceHandle);

    // Create source with 0 indices
    invalidOccluder =
      PHASECreateOccluder(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals, 0, mMeshData.mIndices);
    XCTAssert(invalidOccluder == PHASEInvalidInstanceHandle);

    // Create source with no indices
    invalidOccluder =
      PHASECreateOccluder(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals, mMeshData.mIndexCount, nullptr);
    XCTAssert(invalidOccluder == PHASEInvalidInstanceHandle);

    // Create a bunch of occluders - should succeed
    int64_t occluderId1 = PHASECreateOccluder(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                              mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(occluderId1 != PHASEInvalidInstanceHandle);

    int64_t occluderId2 = PHASECreateOccluder(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                              mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(occluderId2 != PHASEInvalidInstanceHandle);

    int64_t occluderId3 = PHASECreateOccluder(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                              mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(occluderId3 != PHASEInvalidInstanceHandle);

    // Destroy the occluders
    PHASEDestroyOccluder(occluderId1);
    PHASEDestroyOccluder(occluderId2);
    PHASEDestroyOccluder(occluderId3);
}

- (void)testSetOccluderTransform
{
    Matrix4x4 transform = { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f };

    // Try and set the transform - should fail as invalid sourceId
    bool result = PHASESetOccluderTransform(0, transform);
    XCTAssert(result == false);

    // Create a source
    const int64_t occluderId = PHASECreateOccluder(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                   mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(occluderId != PHASEInvalidInstanceHandle);

    // Try and set the transform - should succeed
    result = PHASESetOccluderTransform(occluderId, transform);
    XCTAssert(result == true);

    PHASEDestroyOccluder(occluderId);
}

- (void)testRegisterUnregisterAudioAsset
{
    char name[] = "Test";

    // Register the buffer with invalid name
    bool result =
      PHASERegisterAudioBuffer(nullptr, mAudioBuffer.mData, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == false);

    // Register the buffer with invalid data
    result = PHASERegisterAudioBuffer(name, nullptr, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == false);

    // Register the buffer with invalid sample rate
    result = PHASERegisterAudioBuffer(name, nullptr, 0, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == false);

    // Register the buffer with invalid data size
    result = PHASERegisterAudioBuffer(name, nullptr, 48000, 0, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == false);

    // Register the buffer with invalid bit depth
    result = PHASERegisterAudioBuffer(name, nullptr, 48000, mAudioBuffer.mDataByteSize, 0, mAudioBuffer.mNumberChannels);
    XCTAssert(result == false);

    // Register the buffer with invalid channel count
    result = PHASERegisterAudioBuffer(name, nullptr, 48000, mAudioBuffer.mDataByteSize, 32, 0);
    XCTAssert(result == false);

    // Register the buffer
    result = PHASERegisterAudioBuffer(name, mAudioBuffer.mData, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(name);
}

- (void)testRegisterAudioAssetTwice
{
    char name[] = "Test";

    // Register the buffer
    bool result = PHASERegisterAudioBuffer(name, mAudioBuffer.mData, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == true);
    
    // Register again with the same name
    result = PHASERegisterAudioBuffer(name, mAudioBuffer.mData, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(name);
}

- (void)testRegisterUnregisterMultiChannelAudioBuffers
{
    char monoName[] = "StereoAudioBuffer";
    // Create a stereo test buffer
    AudioBuffer mStereoAudioBuffer;
    mStereoAudioBuffer.mDataByteSize = 48000 * sizeof(float);
    mStereoAudioBuffer.mNumberChannels = 2;
    mStereoAudioBuffer.mData = malloc(mStereoAudioBuffer.mDataByteSize * mStereoAudioBuffer.mNumberChannels);
    // Register the buffer
    bool result = PHASERegisterAudioBuffer(monoName, mStereoAudioBuffer.mData, 48000, mStereoAudioBuffer.mDataByteSize, 32,
                                           mStereoAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(monoName);

    // Create 5.1 channel
    char fiveOneName[] = "5.1ChannelAudioBuffer";
    AudioBuffer m51ChannelAudioBuffer;
    m51ChannelAudioBuffer.mDataByteSize = 48000 * sizeof(float);
    m51ChannelAudioBuffer.mNumberChannels = 6;
    m51ChannelAudioBuffer.mData = malloc(m51ChannelAudioBuffer.mDataByteSize * m51ChannelAudioBuffer.mNumberChannels);
    result = PHASERegisterAudioBuffer(fiveOneName, m51ChannelAudioBuffer.mData, 48000, m51ChannelAudioBuffer.mDataByteSize, 32,
                                      m51ChannelAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(fiveOneName);

    // Create 7.1 channel
    char sevenOneName[] = "7.1ChannelAudioBuffer";
    AudioBuffer m71ChannelAudioBuffer;
    m71ChannelAudioBuffer.mDataByteSize = 48000 * sizeof(float);
    m71ChannelAudioBuffer.mNumberChannels = 8;
    m71ChannelAudioBuffer.mData = malloc(m71ChannelAudioBuffer.mDataByteSize * m71ChannelAudioBuffer.mNumberChannels);
    result = PHASERegisterAudioBuffer(sevenOneName, m71ChannelAudioBuffer.mData, 48000, m71ChannelAudioBuffer.mDataByteSize, 32,
                                      m71ChannelAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(sevenOneName);
}

- (void)testRegisterUnregisterAudioAssetFromFile
{
    char assetName[] = "anechoic-vox-mono.wav";

    // Get the path to the test bundle's resources
    const char* path = [[[NSBundle bundleForClass:[self class]] resourcePath] UTF8String];
    bool result = PHASERegisterAudioFile(assetName, path);
    XCTAssert(result == true);

    PHASEUnregisterAudioAsset(assetName);
}

- (void)testRegisterInvalidMultiChannelAudioBuffers
{
    char quadName[] = "QuadAudioBuffer";
    // Create a quad test buffer
    AudioBuffer mQuadAudioBuffer;
    mQuadAudioBuffer.mDataByteSize = 48000 * sizeof(float);
    mQuadAudioBuffer.mNumberChannels = 4;
    mQuadAudioBuffer.mData = malloc(mQuadAudioBuffer.mDataByteSize * mQuadAudioBuffer.mNumberChannels);
    // Register the buffer
    bool result = PHASERegisterAudioBuffer(quadName, mQuadAudioBuffer.mData, 48000, mQuadAudioBuffer.mDataByteSize, 32,
                                           mQuadAudioBuffer.mNumberChannels);
    XCTAssert(result == false);

    char surroundName[] = "SurroundAudioBuffer";
    // Create a surround test buffer
    AudioBuffer mSurroundAudioBuffer;
    mSurroundAudioBuffer.mDataByteSize = 48000 * sizeof(float);
    mSurroundAudioBuffer.mNumberChannels = 5;
    mSurroundAudioBuffer.mData = malloc(mSurroundAudioBuffer.mDataByteSize * mSurroundAudioBuffer.mNumberChannels);
    // Register the buffer
    result = PHASERegisterAudioBuffer(surroundName, mSurroundAudioBuffer.mData, 48000, mSurroundAudioBuffer.mDataByteSize, 32,
                                      mSurroundAudioBuffer.mNumberChannels);
    XCTAssert(result == false);
}

- (void)testPlayMultiChannelAudioWithChannelMixer
{
    // Create the listener
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);

    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Create 7.1 channel buffer
    char assetName[] = "7.1ChannelAudioBuffer";
    AudioBuffer m71ChannelAudioBuffer;
    m71ChannelAudioBuffer.mDataByteSize = 48000 * sizeof(float);
    m71ChannelAudioBuffer.mNumberChannels = 8;
    m71ChannelAudioBuffer.mData = malloc(m71ChannelAudioBuffer.mDataByteSize * m71ChannelAudioBuffer.mNumberChannels);
    float* fltPtr = (float*) m71ChannelAudioBuffer.mData;
    int index = 0;
    // Generate different sine tone for each channel
    for (int i = 0; i < m71ChannelAudioBuffer.mNumberChannels; i++)
    {
        for (int i = 0; i < 48000; ++i)
        {
            float pitch = 440.0f + (i * 100.0f);
            fltPtr[index + i] = sin((M_PI * 2.0f * pitch) / 48000.0f * i);
        }
        index += 48000;
    }
    result = PHASERegisterAudioBuffer(assetName, m71ChannelAudioBuffer.mData, 48000, m71ChannelAudioBuffer.mDataByteSize, 32,
                                      m71ChannelAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    char mixerName[] = "TestMixerName";
    int64_t mixerId = PHASECreateChannelMixer(mixerName, ChannelLayoutTypeSevenOne);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, false, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char soundEventName[] = "TestSoundEvent";
    PHASERegisterSoundEventAsset(soundEventName, samplerId);

    finishedOneshotCallbackExpectation = [self expectationWithDescription:@"Sound Event OneShot finished playing."];
    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyOneShotCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestSoundEvent", sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    for (int i = 0; i < 100; ++i)
    {
        PHASEUpdate();
        // Pretend we're updating the objects every 16ms (ie. 60hz).
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }
    [self waitForExpectationsWithTimeout:10 handler:nil];

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(soundEventName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the mixer
    PHASEDestroyMixer(mixerId);

    // Destroy the listener
    PHASEDestroyListener();

    // Destroy the source
    PHASEDestroySource(sourceId);
}

- (void)testPlayMonoAudioWithAmbientMixer
{
    // Create the listener
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);

    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Create Mono channel buffer
    char assetName[] = "Mono ChannelAudioBuffer";
    AudioBuffer mMonoChannelAudioBuffer;
    mMonoChannelAudioBuffer.mDataByteSize = 48000 * sizeof(float);
    mMonoChannelAudioBuffer.mNumberChannels = 1;
    mMonoChannelAudioBuffer.mData = malloc(mMonoChannelAudioBuffer.mDataByteSize * mMonoChannelAudioBuffer.mNumberChannels);
    float* fltPtr = (float*) mMonoChannelAudioBuffer.mData;
    int index = 0;
    // Generate different sine tone for each channel
    for (int i = 0; i < mMonoChannelAudioBuffer.mNumberChannels; i++)
    {
        for (int i = 0; i < 48000; ++i)
        {
            float pitch = 440.0f + (i * 100.0f);
            fltPtr[index + i] = sin((M_PI * 2.0f * pitch) / 48000.0f * i);
        }
        index += 48000;
    }
    result = PHASERegisterAudioBuffer(assetName, mMonoChannelAudioBuffer.mData, 48000, mMonoChannelAudioBuffer.mDataByteSize, 32,
                                      mMonoChannelAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    char mixerName[] = "TestMixerName";
    Quaternion quat = { 0, 0, 0, 1 };
    int64_t mixerId = PHASECreateAmbientMixer(mixerName, ChannelLayoutTypeMono, quat);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, false, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char soundEventName[] = "TestSoundEvent";
    PHASERegisterSoundEventAsset(soundEventName, samplerId);

    finishedOneshotCallbackExpectation = [self expectationWithDescription:@"Sound Event OneShot finished playing."];
    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyOneShotCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestSoundEvent", sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    for (int i = 0; i < 100; ++i)
    {
        PHASEUpdate();
        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }
    [self waitForExpectationsWithTimeout:10 handler:nil];

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(soundEventName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the mixer
    PHASEDestroyMixer(mixerId);

    // Destroy the listener
    PHASEDestroyListener();
}

- (void)testPlayStereoAudioWithAmbientMixer
{
    // Create the listener
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);

    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Create Stereo channel buffer
    char assetName[] = "Stereo ChannelAudioBuffer";
    AudioBuffer mStereoChannelAudioBuffer;
    mStereoChannelAudioBuffer.mDataByteSize = 48000 * sizeof(float);
    mStereoChannelAudioBuffer.mNumberChannels = 2;
    mStereoChannelAudioBuffer.mData = malloc(mStereoChannelAudioBuffer.mDataByteSize * mStereoChannelAudioBuffer.mNumberChannels);
    float* fltPtr = (float*) mStereoChannelAudioBuffer.mData;
    int index = 0;
    // Generate different sine tone for each channel
    for (int i = 0; i < mStereoChannelAudioBuffer.mNumberChannels; i++)
    {
        for (int i = 0; i < 48000; ++i)
        {
            float pitch = 440.0f + (i * 100.0f);
            fltPtr[index + i] = sin((M_PI * 2.0f * pitch) / 48000.0f * i);
        }
        index += 48000;
    }
    result = PHASERegisterAudioBuffer(assetName, mStereoChannelAudioBuffer.mData, 48000, mStereoChannelAudioBuffer.mDataByteSize, 32,
                                      mStereoChannelAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    char mixerName[] = "TestMixerName";
    Quaternion quat = { 0, 0, 0, 1 };
    int64_t mixerId = PHASECreateAmbientMixer(mixerName, ChannelLayoutTypeStereo, quat);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, false, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char soundEventName[] = "TestSoundEvent";
    PHASERegisterSoundEventAsset(soundEventName, samplerId);

    finishedOneshotCallbackExpectation = [self expectationWithDescription:@"Sound Event OneShot finished playing."];
    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyOneShotCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestSoundEvent", sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    for (int i = 0; i < 100; ++i)
    {
        PHASEUpdate();
        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }
    [self waitForExpectationsWithTimeout:10 handler:nil];

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(soundEventName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the mixer
    PHASEDestroyMixer(mixerId);

    // Destroy the listener
    PHASEDestroyListener();
}
- (void)testPlay51AudioWithAmbientMixer
{
    // Create the listener
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);

    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Create 5.1 channel buffer
    char assetName[] = "5.1ChannelAudioBuffer";
    AudioBuffer m51ChannelAudioBuffer;
    m51ChannelAudioBuffer.mDataByteSize = 48000 * sizeof(float);
    m51ChannelAudioBuffer.mNumberChannels = 6;
    m51ChannelAudioBuffer.mData = malloc(m51ChannelAudioBuffer.mDataByteSize * m51ChannelAudioBuffer.mNumberChannels);
    float* fltPtr = (float*) m51ChannelAudioBuffer.mData;
    int index = 0;
    // Generate different sine tone for each channel
    for (int i = 0; i < m51ChannelAudioBuffer.mNumberChannels; i++)
    {
        for (int i = 0; i < 48000; ++i)
        {
            float pitch = 440.0f + (i * 100.0f);
            fltPtr[index + i] = sin((M_PI * 2.0f * pitch) / 48000.0f * i);
        }
        index += 48000;
    }
    result = PHASERegisterAudioBuffer(assetName, m51ChannelAudioBuffer.mData, 48000, m51ChannelAudioBuffer.mDataByteSize, 32,
                                      m51ChannelAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    char mixerName[] = "TestMixerName";
    Quaternion quat = { 0, 0, 0, 1 };
    int64_t mixerId = PHASECreateAmbientMixer(mixerName, ChannelLayoutTypeFiveOne, quat);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, false, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char soundEventName[] = "TestSoundEvent";
    PHASERegisterSoundEventAsset(soundEventName, samplerId);

    finishedOneshotCallbackExpectation = [self expectationWithDescription:@"Sound Event OneShot finished playing."];
    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyOneShotCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestSoundEvent", sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    for (int i = 0; i < 100; ++i)
    {
        PHASEUpdate();
        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }
    [self waitForExpectationsWithTimeout:10 handler:nil];

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(soundEventName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the mixer
    PHASEDestroyMixer(mixerId);

    // Destroy the listener
    PHASEDestroyListener();
}

- (void)testPlay71AudioWithAmbientMixer
{
    // Create the listener
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);

    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Create 7.1 channel buffer
    char assetName[] = "7.1ChannelAudioBuffer";
    AudioBuffer m71ChannelAudioBuffer;
    m71ChannelAudioBuffer.mDataByteSize = 48000 * sizeof(float);
    m71ChannelAudioBuffer.mNumberChannels = 8;
    m71ChannelAudioBuffer.mData = malloc(m71ChannelAudioBuffer.mDataByteSize * m71ChannelAudioBuffer.mNumberChannels);
    float* fltPtr = (float*) m71ChannelAudioBuffer.mData;
    int index = 0;
    // Generate different sine tone for each channel
    for (int i = 0; i < m71ChannelAudioBuffer.mNumberChannels; i++)
    {
        for (int i = 0; i < 48000; ++i)
        {
            float pitch = 440.0f + (i * 100.0f);
            fltPtr[index + i] = sin((M_PI * 2.0f * pitch) / 48000.0f * i);
        }
        index += 48000;
    }
    result = PHASERegisterAudioBuffer(assetName, m71ChannelAudioBuffer.mData, 48000, m71ChannelAudioBuffer.mDataByteSize, 32,
                                      m71ChannelAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    char mixerName[] = "TestMixerName";
    Quaternion quat = { 0, 0, 0, 1 };
    int64_t mixerId = PHASECreateAmbientMixer(mixerName, ChannelLayoutTypeSevenOne, quat);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, false, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char soundEventName[] = "TestSoundEvent";
    PHASERegisterSoundEventAsset(soundEventName, samplerId);

    finishedOneshotCallbackExpectation = [self expectationWithDescription:@"Sound Event OneShot finished playing."];
    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyOneShotCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestSoundEvent", sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    for (int i = 0; i < 100; ++i)
    {
        PHASEUpdate();
        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }
    [self waitForExpectationsWithTimeout:10 handler:nil];

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(soundEventName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the mixer
    PHASEDestroyMixer(mixerId);

    // Destroy the listener
    PHASEDestroyListener();
}

- (void)testCreateChannelMixer
{
    char mixerName[] = "TestChanneoMixerName";
    int64_t mixerId = PHASECreateChannelMixer(mixerName, ChannelLayoutTypeSevenOne);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    // Destroy the mixer
    PHASEDestroyMixer(mixerId);
}

- (void)testCreateAmbientMixer
{
    char mixerName[] = "TestAmbientMixerName";
    Quaternion quat = { 0, 0, 0, 1 };
    int64_t mixerId = PHASECreateAmbientMixer(mixerName, ChannelLayoutTypeSevenOne, quat);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    // Destroy the mixer
    PHASEDestroyMixer(mixerId);
}

- (void)testCreateSpatialMixer
{
    char mixerName[] = "TestSpatialMixerName";
    int64_t mixer =
      PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, 1.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixer != PHASEInvalidInstanceHandle);
}

- (void)testRegisterUnregisterSoundEvent
{
    // Register the buffer asset
    char assetName[] = "TestBufferAsset";
    bool result =
      PHASERegisterAudioBuffer(assetName, mAudioBuffer.mData, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    char mixerName[] = "TestMixerName";
    int64_t mixerId =
      PHASECreateSpatialMixer(mixerName, true, true, true, 10.0f, 1.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char soundEventName[] = "TestSoundEvent";
    PHASERegisterSoundEventAsset(soundEventName, samplerId);

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(soundEventName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);
}

void MyPlaySoundEventCompletionHandler(StartHandlerReason reason, int64_t sourceId, int64_t soundEventId)
{
    switch (reason)
    {
        case StartHandlerReasonFinishedPlaying:
            printf("SoundEvent stopped because it finished playing");
            break;
        case StartHandlerReasonTerminated:
            printf("SoundEvent stopped because it was killed.");
            break;
        case StartHandlerReasonFailure:
            printf("SoundEvent stopped because it had a failure.");
            break;
    }
}

XCTestExpectation* finishedOneshotCallbackExpectation;

void MyOneShotCompletionHandler(StartHandlerReason reason, int64_t source, int64_t soundEvent)
{
    XCTAssertEqual(reason, StartHandlerReasonFinishedPlaying);
    [finishedOneshotCallbackExpectation fulfill];
}

- (void)testPlayOneShotSamplerSoundEvent
{
    // Create the listener
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);

    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Register the buffer asset
    char assetName[] = "TestBufferAsset";
    result = PHASERegisterAudioBuffer(assetName, mAudioBuffer.mData, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    char mixerName[] = "TestMixername";
    int64_t mixerId =
      PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, 1.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, false, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char soundEventName[] = "TestSoundEvent";
    result = PHASERegisterSoundEventAsset(soundEventName, samplerId);
    XCTAssert(result == true);

    finishedOneshotCallbackExpectation = [self expectationWithDescription:@"Sound Event OneShot finished playing."];
    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyOneShotCompletionHandler;
    int64_t mixerIds = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestSoundEvent", sourceId, &mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    for (int i = 0; i < 150; ++i)
    {
        PHASEUpdate();
        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }
    [self waitForExpectationsWithTimeout:5 handler:nil];

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(soundEventName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the source
    PHASEDestroySource(sourceId);

    // Destroy the listener
    PHASEDestroyListener();
}

- (void)testPlayStopSamplerSoundEvent
{
    // Create the listener
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Register the buffer asset
    char assetName[] = "TestBufferAsset";
    result = PHASERegisterAudioBuffer(assetName, mAudioBuffer.mData, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    char mixerName[] = "TestMixername";
    int64_t mixerId =
      PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, 1.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char soundEventName[] = "TestSoundEvent";
    result = PHASERegisterSoundEventAsset(soundEventName, samplerId);
    XCTAssert(result == true);

    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyPlaySoundEventCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestSoundEvent", sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);

    for (int i = 0; i < 100; ++i)
    {
        PHASEUpdate();

        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));

        if (i == 50)
        {
            // Must concat mixer name and "DirectPathSend" to set this parameter
            bool result = PHASESetSoundEventParameterDbl(instance, "TestMixernameDirectPathSend", 0.0f);
            XCTAssert(result == true);
        }
    }

    result = PHASEStopSoundEvent(instance);
    XCTAssert(result == true);

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(soundEventName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the source
    PHASEDestroySource(sourceId);

    // Destroy the listener
    PHASEDestroyListener();
}

- (void)testCreateMappedMetaParameter
{
    // Create an Envelope.
    EnvelopeSegment inEnvelopeSegments = { .x = 0.5, .y = 0.5, .curveType = EnvelopeCurveTypeSine };

    EnvelopeParameters inEnvelopeParameters = { .x = 0, .y = 0, .segmentCount = 1, .envelopeSegments = &inEnvelopeSegments };
    int64_t inParameterId = PHASECreateSoundEventParameterDbl("ChirpRiseRate", 0.1, std::numeric_limits<double>::lowest(), std::numeric_limits<double>::max());

    int64_t mappedMetaParameterId = PHASECreateMappedMetaParameter(inParameterId, inEnvelopeParameters);

    XCTAssert(mappedMetaParameterId != PHASEInvalidInstanceHandle);

    PHASEDestroyMappedMetaParameter(mappedMetaParameterId);
}

- (void)testSetInvalidValueOnMetaParameter
{
    char doubleParamName[] = "testDoubleParam";
    int64_t parameterId = PHASECreateSoundEventParameterDbl(doubleParamName, 1.0f, std::numeric_limits<double>::lowest(), std::numeric_limits<double>::max());
    bool result = PHASESetSoundEventParameterStr(parameterId, doubleParamName, "thisShouldFail");
    XCTAssert(result == false);

    PHASEDestroySoundEventParameter(parameterId);

    char stringParamName[] = "testStringParam";
    parameterId = PHASECreateSoundEventParameterStr(stringParamName, "default");
    result = PHASESetSoundEventParameterDbl(parameterId, stringParamName, 1.0f);
    XCTAssert(result == false);

    PHASEDestroySoundEventParameter(parameterId);
    
    char intParamName[] = "testIntParam";
    parameterId = PHASECreateSoundEventParameterInt(intParamName, 1, std::numeric_limits<int>::lowest(), std::numeric_limits<int>::max());
    result = PHASESetSoundEventParameterDbl(parameterId, stringParamName, 1.0f);
    XCTAssert(result == false);

    PHASEDestroySoundEventParameter(parameterId);
}

- (void)testCreateAndSetAmbientMixerGainMetaParameter
 {
     [self helperTestCreateAndSetInitialMixerGainParameter:PHASEMixerType::AmbientMixer gainToSet:0.5];
 }

- (void)testCreateAndSetChannelMixerGainMetaParameter
 {
     [self helperTestCreateAndSetInitialMixerGainParameter:PHASEMixerType::ChannelMixer gainToSet:0.5];
 }

- (void)testCreateAndSetSpatialMixerGainMetaParameter
 {
     [self helperTestCreateAndSetInitialMixerGainParameter:PHASEMixerType::SpatialMixer gainToSet:0.5];
 }

- (void)testSetDynamicallyAmbientMixerGainMetaParameter
 {
     [self helperTestSetDynamicallyMixerGainParameter:PHASEMixerType::AmbientMixer gainToSet:0.2];
 }

- (void)testSetDynamicallyChannelMixerGainMetaParameter
 {
     [self helperTestSetDynamicallyMixerGainParameter:PHASEMixerType::ChannelMixer gainToSet:0.2];
 }

- (void)testSetDynamicallySpatialMixerGainMetaParameter
 {
     [self helperTestSetDynamicallyMixerGainParameter:PHASEMixerType::SpatialMixer gainToSet:0.2];
 }

- (void)testTryCreateAndSetAmbientMixerGainWithInvalidValue
{
    [self helperTestCreateAndSetInitialMixerGainParameter:PHASEMixerType::AmbientMixer gainToSet:-0.5];
}

- (void)testTryCreateAndSetChannelMixerGainWithInvalidValue
{
    [self helperTestCreateAndSetInitialMixerGainParameter:PHASEMixerType::ChannelMixer gainToSet:-0.5];
}

- (void)testTryCreateAndSetSpatialMixerGainWithInvalidValue
{
    [self helperTestCreateAndSetInitialMixerGainParameter:PHASEMixerType::SpatialMixer gainToSet:-0.5];
}

 - (void)testTrySetAmbientMixerGainDynamicallyWithInvalidValue
 {
     [self helperTestSetDynamicallyMixerGainParameter:PHASEMixerType::AmbientMixer gainToSet:-0.5];
 }

- (void)testTrySetChannelMixerGainDynamicallyWithInvalidValue
{
    [self helperTestSetDynamicallyMixerGainParameter:PHASEMixerType::ChannelMixer gainToSet:-0.5];
}

- (void)testTrySetSpatialMixerGainDynamicallyWithInvalidValue
{
    [self helperTestSetDynamicallyMixerGainParameter:PHASEMixerType::SpatialMixer gainToSet:-0.5];
}

- (void)testCreateAndSetSamplerRateMetaParameter
 {
     [self helperTestCreateAndSetInitialSamplerRateParameter:0.5];
 }

- (void)testSetDynamicallySamplerRateMetaParameter
 {
     [self helperTestSetDynamicallySamplerRateParameter:0.5];
 }

- (void)testCreateAndSetSamplerRateMetaParameterWithInvalidValue
 {
     [self helperTestCreateAndSetInitialSamplerRateParameter:0.1];
 }

- (void)testSetDynamicallySamplerRateMetaParameterWithInvalidValue
 {
     [self helperTestSetDynamicallySamplerRateParameter:4.5];
 }

- (void)testCreateBlendNodeWithMappedMetaParameter
{
    // Create an Envelope
    EnvelopeSegment inEnvelopeSegments = { .x = 0.5, .y = 0.5, .curveType = EnvelopeCurveTypeSine };

    EnvelopeParameters inEnvelopeParameters = { .x = 0, .y = 0, .segmentCount = 1, .envelopeSegments = &inEnvelopeSegments };
    int64_t inParameterId = PHASECreateSoundEventParameterDbl("Default", 0.1, std::numeric_limits<double>::lowest(), std::numeric_limits<double>::max());
    int64_t mappedMetaParameterId = PHASECreateMappedMetaParameter(inParameterId, inEnvelopeParameters);
    XCTAssert(mappedMetaParameterId != PHASEInvalidInstanceHandle);

    // Create the listener
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Register the buffer asset
    char assetName[] = "TestBufferAsset";
    result = PHASERegisterAudioBuffer(assetName, mAudioBuffer.mData, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    char mixerName[] = "TestMixerName";
    int64_t mixerId =
      PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, 1.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    // Create a sampler node
    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Create a blend node
    BlendNodeEntry blendEntries[] = { { .nodeId = samplerId, .lowValue = 0, .fullGainAtLow = 1, .fullGainAtHigh = 0, .highValue = 1 } };

    int64_t blendId = PHASECreateSoundEventBlendNode(mappedMetaParameterId, blendEntries, 1, false);
    XCTAssert(blendId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char blendSoundEventName[] = "TestBlendSoundEvent";
    result = PHASERegisterSoundEventAsset(blendSoundEventName, blendId);
    XCTAssert(result == true);

    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyPlaySoundEventCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestBlendSoundEvent", sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);

    for (int i = 0; i < 100; ++i)
    {
        PHASEUpdate();

        // Pretend we're updating the objects every 16ms (ie. 60hz).
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }

    result = PHASEStopSoundEvent(instance);
    XCTAssert(result == true);

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(blendSoundEventName);

    // Unregister the buffer.
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the source.
    PHASEDestroySource(sourceId);

    // Destroy the listener.
    PHASEDestroyListener();

    PHASEDestroyMappedMetaParameter(mappedMetaParameterId);
}

- (void)testSwitchNode
{
    // Create the listener.
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Create a source.
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    char assetName[] = "anechoic-vox-mono.wav";

    // Get the path to the test bundle's resources.
    const char* path = [[[NSBundle bundleForClass:[self class]] resourcePath] UTF8String];
    result = PHASERegisterAudioFile(assetName, path);
    XCTAssert(result == true);

    char mixerName[] = "TestMixername";
    int64_t mixerId =
      PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, 1.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    char mixerName2[] = "TestMixername2";
    int64_t mixerId2 =
      PHASECreateSpatialMixer(mixerName2, true, false, false, 0.0f, 1.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId2 != PHASEInvalidInstanceHandle);

    int64_t samplerId2 = PHASECreateSoundEventSamplerNode(assetName, mixerId2, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId2 != PHASEInvalidInstanceHandle);

    char mixerName3[] = "TestMixername3";
    int64_t mixerId3 =
      PHASECreateSpatialMixer(mixerName3, false, true, true, 0.0f, 1.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId3 != PHASEInvalidInstanceHandle);

    int64_t samplerId3 = PHASECreateSoundEventSamplerNode(assetName, mixerId3, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId3 != PHASEInvalidInstanceHandle);

    // Create the switch node.
    char switchWord[] = "SwitchTest";
    char switch1[] = "test1";
    char switch2[] = "test2";
    char switch3[] = "test3";
    int64_t switchParameterId = PHASECreateSoundEventParameterStr(switchWord, switch1);
    SwitchNodeEntry switchNodeEntries[] = { { .nodeId = samplerId, .switchValue = switch1 },
                                            { .nodeId = samplerId2, .switchValue = switch2 },
                                            { .nodeId = samplerId3, .switchValue = switch3 } };
    int64_t soundEventId = PHASECreateSoundEventSwitchNode(switchParameterId, switchNodeEntries, 3);
    XCTAssert(soundEventId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char soundEventName[] = "TestSoundEvent";
    result = PHASERegisterSoundEventAsset(soundEventName, soundEventId);
    XCTAssert(result == true);

    // Play sound event.
    int64_t mixerIds[] = { mixerId, mixerId2, mixerId3 };

    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyPlaySoundEventCompletionHandler;
    int64_t instance = PHASEPlaySoundEvent(soundEventName, sourceId, mixerIds, 3, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);

    for (int i = 0; i < 200; ++i)
    {
        PHASEUpdate();

        // Pretend we're updating the objects every 16ms (ie. 60hz).
        std::this_thread::sleep_for(std::chrono::milliseconds(16));

        if (i == 50)
        {
            // Change the switch value.
            result = PHASESetSoundEventParameterStr(instance, switchWord, switch2);
            XCTAssert(result == true);
        }

        if (i == 100)
        {
            // Change the switch value.
            result = PHASESetSoundEventParameterStr(instance, switchWord, switch3);
            XCTAssert(result == true);
        }

        if (i == 150)
        {
            // Change the switch value.
            result = PHASESetSoundEventParameterStr(instance, switchWord, switch1);
            XCTAssert(result == true);
        }
    }

    result = PHASEStopSoundEvent(instance);
    XCTAssert(result == true);

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(soundEventName);

    // Unregister the buffer.
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the source.
    PHASEDestroySource(sourceId);

    // Destroy the listener.
    PHASEDestroyListener();
}

- (void)testCreateMaterialFromPreset
{
    char materialName[] = "Test Material Name";
    MaterialPreset kPresets[] = { MaterialPresetCardboard, MaterialPresetGlass,   MaterialPresetBrick,
                                  MaterialPresetConcrete,  MaterialPresetDrywall, MaterialPresetWood };
    for (auto preset : kPresets)
    {
        bool result = PHASECreateMaterialFromPreset(materialName, preset);
        XCTAssert(result == true);
        PHASEDestroyMaterial(materialName);
    }
}

- (void)testMovingSourceAndListener
{
    Matrix4x4 transform = { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f };

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);
    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Try and set the transform - should succeed
    bool result = PHASESetSourceTransform(sourceId, transform);
    XCTAssert(result == true);

    // Create the listener
    result = PHASECreateListener();
    XCTAssert(result == true);

    // Try and set the transform - should succeed
    result = PHASESetListenerTransform(transform);
    XCTAssert(result == true);

    // Register the buffer asset
    char assetName[] = "TestBufferAsset";
    result = PHASERegisterAudioBuffer(assetName, mAudioBuffer.mData, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    char mixerName[] = "TestMixername";
    int64_t mixerId =
      PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, 1.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset
    char soundEventName[] = "TestSoundEvent";
    result = PHASERegisterSoundEventAsset(soundEventName, samplerId);
    XCTAssert(result == true);

    // Play sound event
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyPlaySoundEventCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent(soundEventName, sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);


    for (int i = 0; i < 200; ++i)
    {
        PHASEUpdate();
        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));

        Matrix4x4 stepTransform = { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f };
        stepTransform.m30 += i * 5.0f;
        result = PHASESetListenerTransform(stepTransform);
        result = PHASESetSourceTransform(sourceId, stepTransform);
    }

    result = PHASEDestroyListener();
    XCTAssert(result == true);

    PHASEDestroySource(sourceId);

    PHASEUnregisterAudioAsset(assetName);

    PHASEUnregisterSoundEventAsset(soundEventName);
}

-(void) testDestroyListenerDuringPlayback
{
    // Create the listener
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Create a source
    const int64_t sourceId = PHASECreateVolumetricSource(mMeshData.mVertexCount, mMeshData.mVertexPositions, mMeshData.mVertexNormals,
                                                         mMeshData.mIndexCount, mMeshData.mIndices);

    XCTAssert(sourceId != PHASEInvalidInstanceHandle);

    // Register the buffer asset
    char assetName[] = "TestBufferAsset";
    result = PHASERegisterAudioBuffer(assetName, mAudioBuffer.mData, 48000, mAudioBuffer.mDataByteSize, 32, mAudioBuffer.mNumberChannels);
    XCTAssert(result == true);

    char mixerName[] = "TestMixername";
    int64_t mixerId =
      PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, 1.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char soundEventName[] = "TestSoundEvent";
    result = PHASERegisterSoundEventAsset(soundEventName, samplerId);
    XCTAssert(result == true);

    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyOneShotCompletionHandler;
    int64_t mixerIds = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestSoundEvent", sourceId, &mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    for (int i = 0; i < 250; ++i)
    {
        PHASEUpdate();
        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
        if (i == 75)
        {
            // Destroy the listener
            PHASEDestroyListener();
        }
        if (i == 150)
        {
            PHASECreateListener();
        }
    }
    
    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(soundEventName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the source
    PHASEDestroySource(sourceId);
    
    // Destroy the listener
    PHASEDestroyListener();
}

- (void)testSetListenerHeadTracking
{
    PHASESetListenerHeadTracking(true);
    PHASESetListenerHeadTracking(false);
    PHASESetListenerHeadTracking(true);

}

- (void)testRingBuffer
{
    unsigned int length = 48000;
    float* inbuffer = new float[48000*2];

    // Create AVAudioFormat
    AVAudioChannelLayout* layout = [[AVAudioChannelLayout alloc] initWithLayoutTag:kAudioChannelLayoutTag_Stereo];
    AudioStreamBasicDescription desc = { 0 };
    desc.mSampleRate = 48000;
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

    PHASEWrapperRingBuffer* mRingBuffer = [[PHASEWrapperRingBuffer alloc] initWithFrameSize:length numberOfBuffers:2 format:format];

    // Generate different sine tone for each channel inter-leaved
    for (int j = 0; j < 2; j++)
    {
        for (int i = 0; i < length; i++)
        {
            float pitch = 440 + (i * 100);
            inbuffer[(i*2) + j] = sinf((M_PI * 2 * pitch) / 48000 * i);
        }
    }

    [mRingBuffer write:inbuffer frameCount:length];

    // Create an audio buffer list with the input data
    AudioBufferList outputData;
    outputData.mNumberBuffers = 2;
    outputData.mBuffers[0].mNumberChannels = 1;
    outputData.mBuffers[0].mDataByteSize = sizeof(float) * 48000;
    outputData.mBuffers[0].mData = new float[48000];

    outputData.mBuffers[1].mNumberChannels = 1;
    outputData.mBuffers[1].mDataByteSize = sizeof(float) * 48000;
    outputData.mBuffers[1].mData = new float[48000];

    BOOL success = [mRingBuffer read:&outputData frameCount:length];
    XCTAssertTrue(success);
}

@end
