//
//  PHASEInterface.h
//  AudioPluginPHASE
//
//  Copyright © 2021 Apple Inc. All rights reserved.
//

#ifndef PHASEInterface_h
#define PHASEInterface_h

#import "PHASEWrapper.h"

extern "C" {

namespace
{
    // 4x4 Matrix representation - this needs to be the same as interface caller
    struct Matrix4x4
    {
        float m00;
        float m10;
        float m20;
        float m30;
        float m01;
        float m11;
        float m21;
        float m31;
        float m02;
        float m12;
        float m22;
        float m32;
        float m03;
        float m13;
        float m23;
        float m33;
    };
}  // namespace

/*
    Creates a PHASE Listener.
    Returns true on success, false otherwise.
 */
bool PHASECreateListener();

/*
    Sets the transform of the listener
    Returns true on success, false otherwise.
*/
bool PHASESetListenerTransform(Matrix4x4 inTransform);

/*
    Destroys a PHASE Listener.
    Returns true on success, false otherwise.
 */
bool PHASEDestroyListener();

/*
    Creates a PHASE Source with a volumetric representation.
    Returns positive source ID if successfull, PHASEInvalidInstanceHandle on failure.
 */
int64_t PHASECreateVolumetricSource(int inVertCount,
                                    const float* inPositions,
                                    const float* inNormals,
                                    int inIndexCount,
                                    const uint32_t* inIndices);

/*
    Creates a PHASE Source with a point representation.
    Returns positive source ID if successfull, PHASEInvalidInstanceHandle on failure.
 */
int64_t PHASECreatePointSource();

/*
    Sets the transform for a given source
    Returns true on success, false otherwise.
*/
bool PHASESetSourceTransform(int64_t inSourceId, Matrix4x4 inTransform);

/*
    Destroys a PHASE Source.
*/
void PHASEDestroySource(int64_t inSourceId);

/*
    Creates a PHASE Occluder.
    Returns positive occluder ID if successfull, PHASEInvalidInstanceHandle on failure.
 */
int64_t PHASECreateOccluder(int inVertCount, const float* inPositions, const float* inNormals, int inIndexCount, const uint32_t* inIndices);

/*
    Sets the transform for a given source
    Returns true on success, false otherwise.
*/
bool PHASESetOccluderTransform(int64_t inOccluderId, Matrix4x4 inTransform);

/*
    Sets the material on this occluder
    Returns true on success, false otherwise.
*/
bool PHASESetOccluderMaterial(int64_t inOccluderId, const char* inMaterialName);

/*
    Destroys a PHASE Ocluder.
*/
void PHASEDestroyOccluder(int64_t inOccluderId);

/*
    Create a material from a preset
*/
bool PHASECreateMaterialFromPreset(const char* inName, MaterialPreset inPreset);

/*
    Destroy a material
 */
void PHASEDestroyMaterial(const char* inName);

/*
    Set scene reverb preset
 */
void PHASESetSceneReverbPreset(const int inPresetIndex);

/*
    Starts the PHASE engine
 */
bool PHASEStart();

/*
    Stops the PHASE engine
 */
void PHASEStop();

/*
    Update the PHASE engine
 */
void PHASEUpdate();
}

#endif  // PHASEInterface_h
