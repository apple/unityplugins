//
//  AudioPluginPHASE_Tests.mm
//  AudioPluginPHASE
//
//  Copyright © 2019 Apple Inc. All rights reserved.
//

#import <XCTest/XCTest.h>

#import "PHASEWrapper.h"
#include "PhaseInterface.h"
#include "PhaseSoundEventInterface.h"
#include <thread>
#include <chrono>

struct Vector4
{
    float x;
    float y;
    float z;
    float w;
};

struct Vector3
{
    float x;
    float y;
    float z;
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

- (void)testCreateDestroyListener
{

    // Create the listener once - it should initialize the singleton engine and create successfully
    bool result = PHASECreateListener();
    XCTAssert(result == true);

    // Create the listener again - should fail as it already exists.
    result = PHASECreateListener();
    XCTAssert(result == false);

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
    char treeName[] = "TestTree";
    PHASERegisterSoundEventAsset(treeName, samplerId);

    finishedOneshotCallbackExpectation = [self expectationWithDescription:@"Sound Event OneShot finished playing."];
    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyOneShotCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestTree", sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    for (int i = 0; i < 100; ++i)
    {
        PHASEUpdate();
        // Pretend we're updating the objects every 16ms (ie. 60hz).
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }
    [self waitForExpectationsWithTimeout:10 handler:nil];

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(treeName);

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
    Quaternion quat = { 0, 0, 0, 0 };
    int64_t mixerId = PHASECreateAmbientMixer(mixerName, ChannelLayoutTypeMono, quat);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, false, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char treeName[] = "TestTree";
    PHASERegisterSoundEventAsset(treeName, samplerId);

    finishedOneshotCallbackExpectation = [self expectationWithDescription:@"Sound Event OneShot finished playing."];
    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyOneShotCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestTree", sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    for (int i = 0; i < 100; ++i)
    {
        PHASEUpdate();
        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }
    [self waitForExpectationsWithTimeout:10 handler:nil];

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(treeName);

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
    Quaternion quat = { 0, 0, 0, 0 };
    int64_t mixerId = PHASECreateAmbientMixer(mixerName, ChannelLayoutTypeStereo, quat);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, false, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char treeName[] = "TestTree";
    PHASERegisterSoundEventAsset(treeName, samplerId);

    finishedOneshotCallbackExpectation = [self expectationWithDescription:@"Sound Event OneShot finished playing."];
    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyOneShotCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestTree", sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    for (int i = 0; i < 100; ++i)
    {
        PHASEUpdate();
        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }
    [self waitForExpectationsWithTimeout:10 handler:nil];

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(treeName);

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
    Quaternion quat = { 0, 0, 0, 0 };
    int64_t mixerId = PHASECreateAmbientMixer(mixerName, ChannelLayoutTypeFiveOne, quat);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, false, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char treeName[] = "TestTree";
    PHASERegisterSoundEventAsset(treeName, samplerId);

    finishedOneshotCallbackExpectation = [self expectationWithDescription:@"Sound Event OneShot finished playing."];
    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyOneShotCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestTree", sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    for (int i = 0; i < 100; ++i)
    {
        PHASEUpdate();
        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }
    [self waitForExpectationsWithTimeout:10 handler:nil];

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(treeName);

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
    Quaternion quat = { 0, 0, 0, 0 };
    int64_t mixerId = PHASECreateAmbientMixer(mixerName, ChannelLayoutTypeSevenOne, quat);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, false, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char treeName[] = "TestTree";
    PHASERegisterSoundEventAsset(treeName, samplerId);

    finishedOneshotCallbackExpectation = [self expectationWithDescription:@"Sound Event OneShot finished playing."];
    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyOneShotCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestTree", sourceId, mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    for (int i = 0; i < 100; ++i)
    {
        PHASEUpdate();
        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }
    [self waitForExpectationsWithTimeout:10 handler:nil];

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(treeName);

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
    Quaternion quat = { 0, 0, 0, 0 };
    int64_t mixerId = PHASECreateAmbientMixer(mixerName, ChannelLayoutTypeSevenOne, quat);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    // Destroy the mixer
    PHASEDestroyMixer(mixerId);
}

- (void)testCreateSpatialMixer
{
    char mixerName[] = "TestSpatialMixerName";
    int64_t mixer =
      PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, mDirectivityModelParameters, mDirectivityModelParameters);
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
      PHASECreateSpatialMixer(mixerName, true, true, true, 10.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char treeName[] = "TestTree";
    PHASERegisterSoundEventAsset(treeName, samplerId);

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(treeName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);
}

void MySourcePointTestCallback(Vector4* inSourcePoints, int inNumPoints, int64_t inSourceId)
{
    for (int ptIdx = 0; ptIdx < inNumPoints; ++ptIdx)
    {
        printf("%lld, %f, %f, %f, %f\n", inSourceId, inSourcePoints[ptIdx].x, inSourcePoints[ptIdx].y, inSourcePoints[ptIdx].z,
               inSourcePoints[ptIdx].w);
    }
}

void MyOccluderTestCallback(Vector3* inVoxelPositions, Vector3 inVoxelsSize, int inNumVoxels, int64_t inOccluderId)
{
    for (int voxIdx = 0; voxIdx < inNumVoxels; ++voxIdx)
    {
        printf("%lld, %f, %f, %f\n", inOccluderId, inVoxelPositions[voxIdx].x, inVoxelPositions[voxIdx].y, inVoxelPositions[voxIdx].z);
    }
}

void MyPlaySoundEventCompletionHandler(PHASESoundEventStartHandlerReason reason, int64_t sourceId, int64_t soundEventId)
{
    switch (reason)
    {
        case PHASESoundEventStartHandlerReasonFinishedPlaying:
            printf("SoundEvent stopped because it finished playing");
            break;
        case PHASESoundEventStartHandlerReasonTerminated:
            printf("SoundEvent stopped because it was killed.");
            break;
        case PHASESoundEventStartHandlerReasonFailure:
            printf("SoundEvent stopped because it had a failure.");
            break;
    }
}

XCTestExpectation* finishedOneshotCallbackExpectation;

void MyOneShotCompletionHandler(PHASESoundEventStartHandlerReason reason, int64_t source, int64_t soundEvent)
{
    XCTAssertEqual(reason, PHASESoundEventStartHandlerReasonFinishedPlaying);
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
      PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, false, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char treeName[] = "TestTree";
    result = PHASERegisterSoundEventAsset(treeName, samplerId);
    XCTAssert(result == true);

    finishedOneshotCallbackExpectation = [self expectationWithDescription:@"Sound Event OneShot finished playing."];
    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyOneShotCompletionHandler;
    int64_t mixerIds = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestTree", sourceId, &mixerIds, 1, soundEventCompletionHandler);
    XCTAssert(instance != PHASEInvalidInstanceHandle);
    for (int i = 0; i < 150; ++i)
    {
        PHASEUpdate();
        // Pretend we're updating the objects every 16ms (ie. 60hz)
        std::this_thread::sleep_for(std::chrono::milliseconds(16));
    }
    [self waitForExpectationsWithTimeout:5 handler:nil];

    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(treeName);

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
      PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char treeName[] = "TestTree";
    result = PHASERegisterSoundEventAsset(treeName, samplerId);
    XCTAssert(result == true);

    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyPlaySoundEventCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestTree", sourceId, mixerIds, 1, soundEventCompletionHandler);
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
    PHASEUnregisterSoundEventAsset(treeName);

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
    int64_t inParameterId = PHASECreateSoundEventParameterDbl("ChirpRiseRate", 0.1);

    int64_t mappedMetaParameterId = PHASECreateMappedMetaParameter(inParameterId, inEnvelopeParameters);

    XCTAssert(mappedMetaParameterId != PHASEInvalidInstanceHandle);

    PHASEDestroyMappedMetaParameter(mappedMetaParameterId);
}

- (void)testSetInvalidValueOnMetaParameter
{
    char doubleParamName[] = "testDoubleParam";
    int64_t parameterId = PHASECreateSoundEventParameterDbl(doubleParamName, 1.0f);
    bool result = PHASESetSoundEventParameterStr(parameterId, doubleParamName, "thisShouldFail");
    XCTAssert(result == false);

    PHASEDestroySoundEventParameter(parameterId);

    char stringParamName[] = "testStringParam";
    parameterId = PHASECreateSoundEventParameterStr(stringParamName, "default");
    result = PHASESetSoundEventParameterDbl(parameterId, stringParamName, 1.0f);
    XCTAssert(result == false);

    PHASEDestroySoundEventParameter(parameterId);
    
    char intParamName[] = "testIntParam";
    parameterId = PHASECreateSoundEventParameterInt(intParamName, 1);
    result = PHASESetSoundEventParameterDbl(parameterId, stringParamName, 1.0f);
    XCTAssert(result == false);

    PHASEDestroySoundEventParameter(parameterId);
}

- (void)testCreateBlendNodeWithMappedMetaParameter
{
    // Create an Envelope
    EnvelopeSegment inEnvelopeSegments = { .x = 0.5, .y = 0.5, .curveType = EnvelopeCurveTypeSine };

    EnvelopeParameters inEnvelopeParameters = { .x = 0, .y = 0, .segmentCount = 1, .envelopeSegments = &inEnvelopeSegments };
    int64_t inParameterId = PHASECreateSoundEventParameterDbl("Default", 0.1);
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
      PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    // Create a sampler node
    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Create a blend node
    BlendNodeEntry blendEntries[] = { { .nodeId = samplerId, .lowValue = 0, .fullGainAtLow = 1, .fullGainAtHigh = 0, .highValue = 1 } };

    int64_t blendId = PHASECreateSoundEventBlendNode(mappedMetaParameterId, blendEntries, 1, false);
    XCTAssert(blendId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char blendTreeName[] = "TestBlendTree";
    result = PHASERegisterSoundEventAsset(blendTreeName, blendId);
    XCTAssert(result == true);

    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyPlaySoundEventCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestBlendTree", sourceId, mixerIds, 1, soundEventCompletionHandler);
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
    PHASEUnregisterSoundEventAsset(blendTreeName);

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
      PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    char mixerName2[] = "TestMixername2";
    int64_t mixerId2 =
      PHASECreateSpatialMixer(mixerName2, true, false, false, 0.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId2 != PHASEInvalidInstanceHandle);

    int64_t samplerId2 = PHASECreateSoundEventSamplerNode(assetName, mixerId2, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId2 != PHASEInvalidInstanceHandle);

    char mixerName3[] = "TestMixername3";
    int64_t mixerId3 =
      PHASECreateSpatialMixer(mixerName3, false, true, true, 0.0f, mDirectivityModelParameters, mDirectivityModelParameters);
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
    char treeName[] = "TestTree";
    result = PHASERegisterSoundEventAsset(treeName, soundEventId);
    XCTAssert(result == true);

    // Play sound event.
    int64_t mixerIds[] = { mixerId, mixerId2, mixerId3 };

    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyPlaySoundEventCompletionHandler;
    int64_t instance = PHASEPlaySoundEvent(treeName, sourceId, mixerIds, 3, soundEventCompletionHandler);
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
    PHASEUnregisterSoundEventAsset(treeName);

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
      PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset
    char treeName[] = "TestTree";
    result = PHASERegisterSoundEventAsset(treeName, samplerId);
    XCTAssert(result == true);

    // Play sound event
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyPlaySoundEventCompletionHandler;
    int64_t mixerIds[] = { mixerId };
    int64_t instance = PHASEPlaySoundEvent(treeName, sourceId, mixerIds, 1, soundEventCompletionHandler);
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

    PHASEUnregisterSoundEventAsset(treeName);
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
      PHASECreateSpatialMixer(mixerName, true, true, true, 0.0f, mDirectivityModelParameters, mDirectivityModelParameters);
    XCTAssert(mixerId != PHASEInvalidInstanceHandle);

    int64_t samplerId = PHASECreateSoundEventSamplerNode(assetName, mixerId, true, CalibrationModeRelativeSpl, 1);
    XCTAssert(samplerId != PHASEInvalidInstanceHandle);

    // Register the sound event asset.
    char treeName[] = "TestTree";
    result = PHASERegisterSoundEventAsset(treeName, samplerId);
    XCTAssert(result == true);

    // Play sound event.
    PHASESoundEventCompletionHandler soundEventCompletionHandler = MyOneShotCompletionHandler;
    int64_t mixerIds = { mixerId };
    int64_t instance = PHASEPlaySoundEvent("TestTree", sourceId, &mixerIds, 1, soundEventCompletionHandler);
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
    result = PHASEStopSoundEvent(instance);
    XCTAssert(result == true);
    
    // Unregister the sound event asset.
    PHASEUnregisterSoundEventAsset(treeName);

    // Unregister the buffer
    PHASEUnregisterAudioAsset(assetName);

    // Destroy the source
    PHASEDestroySource(sourceId);
    
    // Destroy the listener
    PHASEDestroyListener();
}

@end
