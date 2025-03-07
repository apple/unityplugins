#pragma once

#include "AudioPluginInterface.h"

#include <math.h>
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <assert.h>


#include <pthread.h>
#define strcpy_s strcpy
#define vsprintf_s vsprintf

namespace AudioPluginUtil
{

typedef int (*InternalEffectDefinitionRegistrationCallback)(UnityAudioEffectDefinition& desc);

inline float FastClip(float x, float minval, float maxval) { return (fabsf(x - minval) - fabsf(x - maxval) + (minval + maxval)) * 0.5f; }
inline float FastMin(float a, float b) { return (a + b - fabsf(a - b)) * 0.5f; }
inline float FastMax(float a, float b) { return (a + b + fabsf(a - b)) * 0.5f; }
inline int FastFloor(float x) { return (int)floorf(x); } // TODO: Optimize

char* strnew(const char* src);

void RegisterParameter(
    UnityAudioEffectDefinition& desc,
    const char* name,
    const char* unit,
    float minval,
    float maxval,
    float defaultval,
    float displayscale,
    float displayexponent,
    int enumvalue,
    const char* description = NULL
    );

void InitParametersFromDefinitions(
    InternalEffectDefinitionRegistrationCallback registereffectdefcallback,
    float* params
    );

void DeclareEffect(
    UnityAudioEffectDefinition& desc,
    const char* name,
    UnityAudioEffect_CreateCallback createcallback,
    UnityAudioEffect_ReleaseCallback releasecallback,
    UnityAudioEffect_ProcessCallback processcallback,
    UnityAudioEffect_SetFloatParameterCallback setfloatparametercallback,
    UnityAudioEffect_GetFloatParameterCallback getfloatparametercallback,
    UnityAudioEffect_GetFloatBufferCallback getfloatbuffercallback,
    InternalEffectDefinitionRegistrationCallback registereffectdefcallback
    );

} // namespace AudioPluginUtil
