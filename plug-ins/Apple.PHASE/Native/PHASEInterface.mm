//
//  PHASEInterface.mm
//  AudioPluginPHASE
//
//  Copyright © 2021 Apple Inc. All rights reserved.
//

#include "PHASEInterface.h"
#import "PHASEWrapper.h"
#import "AVFoundation/AVAudioFormat.h"

namespace
{
    static MDLMesh*
      CreateMesh(int inVertCount, const float* inPositions, const float* inNormals, int inIndexCount, const uint32_t* inIndices)
    {
        if (inVertCount == 0 || inPositions == nullptr || inNormals == nullptr || inIndexCount == 0 || inIndices == nullptr)
        {
            return nullptr;
        }

        // Create a mdlMesh
        MDLMesh* mdlMesh = [MDLMesh alloc];

        // Set the vert count
        mdlMesh.vertexCount = inVertCount;

        // Fill the vertex positions into the vertex buffer array
        [mdlMesh addAttributeWithName:MDLVertexAttributePosition format:MDLVertexFormatFloat3];
        NSData* positionNsData = [NSData dataWithBytes:inPositions length:inVertCount * 3 * sizeof(float)];
        [mdlMesh.vertexBuffers[0] fillData:positionNsData offset:0];

        // Fill the normals into the vertex buffer array
        [mdlMesh addAttributeWithName:MDLVertexAttributeNormal format:MDLVertexFormatFloat3];
        NSData* normalNsData = [NSData dataWithBytes:inNormals length:inVertCount * 3 * sizeof(float)];
        [mdlMesh.vertexBuffers[1] fillData:normalNsData offset:0];

        // add the submesh
        size_t numSubmeshes = 1;
        NSMutableArray<MDLSubmesh*>* arrayOfSubmeshes = [[NSMutableArray alloc] initWithCapacity:numSubmeshes];

        // Fill the index data in the index buffer
        const auto indexSize = inIndexCount * sizeof(uint32_t);
        NSData* nsDataIdx = [NSData dataWithBytes:inIndices length:indexSize];
        MDLMeshBufferData* idxBuffer = [[MDLMeshBufferData alloc] initWithType:MDLMeshBufferTypeIndex length:indexSize];
        [idxBuffer fillData:nsDataIdx offset:0];

        // Create a material
        MDLScatteringFunction* scatteringFunction = [MDLPhysicallyPlausibleScatteringFunction new];
        MDLMaterial* material = [[MDLMaterial alloc] initWithName:@"plausibleMaterial" scatteringFunction:scatteringFunction];

        // Create the submesh
        MDLSubmesh* submesh = [[MDLSubmesh alloc] initWithIndexBuffer:idxBuffer
                                                           indexCount:inIndexCount
                                                            indexType:MDLIndexBitDepthUInt32
                                                         geometryType:MDLGeometryTypeTriangles
                                                             material:material];

        // Add the submesh to the array and set the array on the mesh
        [arrayOfSubmeshes addObject:submesh];
        mdlMesh.submeshes = arrayOfSubmeshes;

        return mdlMesh;
    }

    static simd_float4x4 GetTransform(Matrix4x4 inTransform)
    {
        const simd_float4x4 transform = {
            simd_float4{ inTransform.m00, inTransform.m01, inTransform.m02, inTransform.m03 },
            simd_float4{ inTransform.m10, inTransform.m11, inTransform.m12, inTransform.m13 },
            simd_float4{ inTransform.m20, inTransform.m21, inTransform.m22, inTransform.m23 },
            simd_float4{ inTransform.m30, inTransform.m31, inTransform.m32, inTransform.m33 },
        };

        return transform;
    }
}

extern "C" {

bool PHASECreateListener()
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper createListener];
}

bool PHASESetListenerTransform(Matrix4x4 inTransform)
{
    const simd_float4x4 listenerTransform = GetTransform(inTransform);
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper setListenerTransform:listenerTransform];
}

bool PHASEDestroyListener()
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper destroyListener];
}

int64_t PHASECreateVolumetricSource(int inVertCount,
                                    const float* inPositions,
                                    const float* inNormals,
                                    int inIndexCount,
                                    const uint32_t* inIndices)
{
    // Create a mesh from data
    MDLMesh* mdlMesh = CreateMesh(inVertCount, inPositions, inNormals, inIndexCount, inIndices);
    if (nullptr == mdlMesh)
    {
        NSLog(@"Unable to create MDL Mesh for PHASE Source.");
        return PHASEInvalidInstanceHandle;
    }

    // Try and create a source with this mesh
    @try
    {
        PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
        return [engineWrapper createSourceWithMesh:mdlMesh];
    }
    @catch (NSException* e)
    {
        NSLog(@"Unable to create PHASE Source.");
        return PHASEInvalidInstanceHandle;
    }
}

int64_t PHASECreatePointSource()
{
    @try
    {
        PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
        return [engineWrapper createSource];
    }
    @catch (NSException* e)
    {
        NSLog(@"Unable to create PHASE Source.");
        return PHASEInvalidInstanceHandle;
    }
}

bool PHASESetSourceTransform(int64_t inSourceId, Matrix4x4 inTransform)
{
    const simd_float4x4 sourceTransform = GetTransform(inTransform);
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper setSourceTransformWithId:inSourceId transform:sourceTransform];
}

void PHASEDestroySource(int64_t inSourceId)
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper destroySourceWithId:inSourceId];
}

int64_t PHASECreateOccluder(int inVertCount, const float* inPositions, const float* inNormals, int inIndexCount, const uint32_t* inIndices)
{
    // Create a mesh from data
    MDLMesh* mdlMesh = CreateMesh(inVertCount, inPositions, inNormals, inIndexCount, inIndices);
    if (nullptr == mdlMesh)
    {
        NSLog(@"Unable to create MDL Mesh for PHASE Occluder.");
        return PHASEInvalidInstanceHandle;
    }

    // Try and create an occluder with this mesh
    @try
    {
        PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
        return [engineWrapper createOccluderWithMesh:mdlMesh];
    }
    @catch (NSException* e)
    {
        NSLog(@"Failed to create PHASE Occluder.");
        return PHASEInvalidInstanceHandle;
    }
}

bool PHASESetOccluderTransform(int64_t inOccluderId, Matrix4x4 inTransform)
{
    const simd_float4x4 occluderTransform = GetTransform(inTransform);
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper setOccluderTransformWithId:inOccluderId transform:occluderTransform];
}

bool PHASESetOccluderMaterial(int64_t inOccluderId, const char* inMaterialName)
{
    NSString* materialName = [NSString stringWithUTF8String:inMaterialName];
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper setOccluderMaterialWithId:inOccluderId materialName:materialName];
}

void PHASEDestroyOccluder(int64_t inOccluderId)
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper destroyOccluderWithId:inOccluderId];
}

bool PHASECreateMaterialFromPreset(const char* inName, MaterialPreset inPreset)
{
    NSString* materialName = [NSString stringWithUTF8String:inName];
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper createMaterialWithName:materialName preset:inPreset];
}

void PHASEDestroyMaterial(const char* inName)
{
    NSString* materialName = [NSString stringWithUTF8String:inName];
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    [engineWrapper destroyMaterialWithName:materialName];
}

void PHASESetSceneReverbPreset(const int inPresetIndex)
{
    PHASEReverbPreset preset = (PHASEReverbPreset) inPresetIndex;
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    [engineWrapper setSceneReverbWithPreset:preset];
}

bool PHASEStart()
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    return [engineWrapper start];
}

void PHASEStop()
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    [engineWrapper stop];
}

void PHASEUpdate()
{
    PHASEEngineWrapper* engineWrapper = [PHASEEngineWrapper sharedInstance];
    [engineWrapper update];
}
}
