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
    Sets the gain linear scale value of the listener, range of [0,1]
    Given gain values outside of this range will be clamped.
    Returns true on success, false otherwise.
*/
bool PHASESetListenerGain(double inGain);

/*
    Gets the gain of the listener
    Returns double representing listener gain scalar value, range of [0,1]
*/
double PHASEGetListenerGain();

/*
    Sets the listener to automatically update orientation based on user's head-pose (with compatible devices)
 */
bool PHASESetListenerHeadTracking(bool inHeadTrack);

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
    Sets the gain linear scale value of a given source, range of [0,1].
    Given gain values outside of this range will be clamped.
    Returns true on success, false otherwise.
*/
bool PHASESetSourceGain(int64_t inSourceId, double inGain);

/*
    Gets the gain of a given source
    Returns double representing source's gain scalar value, range of [0,1].
*/
double PHASEGetSourceGain(int64_t inSourceId);

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
    Pauses the PHASE engine
 */
void PHASEPause();

/*
    Stops the PHASE engine
 */
void PHASEStop();

/*
    Update the PHASE engine
 */
void PHASEUpdate();

/*
 Set a material's property
 */
void PHASESetMaterialProperties(const char* inName, float inScale, float inShift, float inTilt);
}

#endif  // PHASEInterface_h
