//
//  PHASEWrapper.mm
//  AudioPluginPHASE
//
//  Copyright © 2021 Apple Inc. All rights reserved.
//

#import <PHASE/PHASE.h>
#import <PHASE/PHASEDistanceModel.h>

#include "PHASEWrapper.h"
#include <mutex>

@interface PHASEEngineWrapper () {
    PHASEEngine* mEngine;
    PHASEListener* mListener;
    NSMutableDictionary<NSNumber*, PHASESource*>* mSources;
    NSMutableDictionary<NSNumber*, PHASEOccluder*>* mOccluders;
    NSMutableDictionary<NSNumber*, PHASESoundEvent*>* mSoundEvents;
    NSMutableDictionary<NSString*, PHASEMaterial*>* mMaterials;

    // Keep track of the number of sound events playing an action tree asset
    // For destruction purposes
    NSMutableDictionary<NSString*, NSNumber*>* mActiveSoundEventAssets;

    // For action tree programatic creation
    NSMutableDictionary* mSoundEventNodeDefinitions;
    NSMutableDictionary* mSoundEventMixerDefinitions;
    NSMutableDictionary* mSoundEventMetaParameterDefinitions;
    NSMutableDictionary* mSoundEventMappedMetaParameterDefinitions;
}

@end

@implementation PHASEEngineWrapper : NSObject

+ (id)sharedInstance
{
    static PHASEEngineWrapper* wrapper = nil;
    static std::once_flag onceFlag;
    std::call_once(onceFlag, [&self]() { wrapper = [[self alloc] init]; });
    return wrapper;
}

- (id)init
{
    if (self = [super init])
    {
        // Create engine
        mEngine = [[PHASEEngine alloc] initWithUpdateMode:PHASEUpdateModeManual];

        // Create sources dictionary
        mSources = [[NSMutableDictionary<NSNumber*, PHASESource*> alloc] init];

        // Create occluders dictionary
        mOccluders = [[NSMutableDictionary<NSNumber*, PHASEOccluder*> alloc] init];

        // Create sound events dictionary
        mSoundEvents = [[NSMutableDictionary<NSNumber*, PHASESoundEvent*> alloc] init];

        // Create materials dictionary
        mMaterials = [[NSMutableDictionary<NSString*, PHASEMaterial*> alloc] init];

        // Creeate active action tree assets tracker dictionary
        mActiveSoundEventAssets = [[NSMutableDictionary<NSString*, NSNumber*> alloc] init];

        // Create action tree nodes dictionary
        mSoundEventNodeDefinitions = [[NSMutableDictionary alloc] init];

        // Create action tree meta parameters dictionary
        mSoundEventMetaParameterDefinitions = [[NSMutableDictionary alloc] init];

        // Create mapped meta parameters dictionary
        mSoundEventMappedMetaParameterDefinitions = [[NSMutableDictionary alloc] init];

        // Create mixeres dictionary
        mSoundEventMixerDefinitions = [[NSMutableDictionary alloc] init];

        // Set default preset
        [mEngine setDefaultReverbPreset:PHASEReverbPresetMediumRoom];

        NSLog(@"Engine created successfully.");
        return self;
    }

    return nil;
}

- (BOOL)createListener
{
    if (mListener != nil)
    {
        NSLog(@"Listener already exists.");
        return NO;
    }

    // Create the listener object
    mListener = [[PHASEListener alloc] initWithEngine:mEngine];
    if (mListener == nil)
    {
        NSLog(@"Failed to create Listener.");
        return NO;
    }

    NSError* errorRef = [NSError alloc];
    const BOOL result = [mEngine.rootObject addChild:mListener error:&errorRef];
    if (!result)
    {
        NSLog(@"Failed to add listener to the scene %@.", errorRef);
        return NO;
    }

    NSLog(@"Listener created successfully.");
    return YES;
}

- (BOOL)setListenerTransform:(simd_float4x4)listenerTransform
{
    if (mListener == nil)
    {
        return NO;
    }

    mListener.transform = listenerTransform;
    return YES;
}

- (BOOL)destroyListener
{
    if (mListener == nil)
    {
        NSLog(@"Listener does not exist.");
        return NO;
    }

    // Remove from the hierarchy
    [mEngine.rootObject removeChild:mListener];

    mListener = nil;

    NSLog(@"Listener destroyed successfully.");
    return YES;
}

- (int64_t)createSourceWithMesh:(MDLMesh*)mesh
{
    // Create the shape
    PHASEShape* shape = [[PHASEShape alloc] initWithEngine:mEngine mesh:mesh];
    if (!shape)
    {
        [NSException raise:@"Node invalid" format:@"Failed to create shape."];
    }

    // Create the source object
    PHASESource* source = [[PHASESource alloc] initWithEngine:mEngine shapes:@[ shape ]];
    if (source == nil)
    {
        [NSException raise:@"Source invalid" format:@"Failed to create source."];
    }

    NSError* errorRef = [NSError alloc];
    const BOOL result = [mEngine.rootObject addChild:source error:&errorRef];
    if (!result)
    {
        [NSException raise:@"Adding child error" format:@"Failed to add object to scene."];
    }

    const int64_t sourceId = reinterpret_cast<int64_t>(source);
    [mSources setObject:source forKey:[NSNumber numberWithLongLong:sourceId]];

    return sourceId;
}

- (int64_t)createSource
{
    PHASESource* source = [[PHASESource alloc] initWithEngine:mEngine];
    if (source == nil)
    {
        [NSException raise:@"Source invalid" format:@"Failed to create source."];
    }

    NSError* errorRef = [NSError alloc];
    const BOOL result = [mEngine.rootObject addChild:source error:&errorRef];
    if (!result)
    {
        [NSException raise:@"Adding child error" format:@"Failed to add object to scene."];
    }

    const int64_t sourceId = reinterpret_cast<int64_t>(source);
    [mSources setObject:source forKey:[NSNumber numberWithLongLong:sourceId]];

    return sourceId;
}

- (BOOL)setSourceTransformWithId:(int64_t)sourceId transform:(simd_float4x4)transform
{
    PHASESource* source = [mSources objectForKey:[NSNumber numberWithLongLong:sourceId]];
    if (source == nil)
    {
        return NO;
    }

    source.transform = transform;
    return YES;
}

- (void)destroySourceWithId:(int64_t)sourceId
{
    PHASESource* source = [mSources objectForKey:[NSNumber numberWithLongLong:sourceId]];
    if (source != nil)
    {
        // Remove from the hierarchy
        [mEngine.rootObject removeChild:source];

        [mSources removeObjectForKey:[NSNumber numberWithLongLong:sourceId]];

        source = nil;
    }
}

- (int64_t)createOccluderWithMesh:(MDLMesh*)mesh
{
    // Create the source node
    PHASEShape* shape = [[PHASEShape alloc] initWithEngine:mEngine mesh:mesh];
    if (!shape)
    {
        [NSException raise:@"Node invalid" format:@"Failed to create shape."];
    }

    // Create the source object
    PHASEOccluder* occluder = [[PHASEOccluder alloc] initWithEngine:mEngine shapes:@[ shape ]];
    if (occluder == nil)
    {
        [NSException raise:@"Occluder invalid" format:@"Failed to create occluder."];
    }

    NSError* errorRef = [NSError alloc];
    const BOOL result = [mEngine.rootObject addChild:occluder error:&errorRef];
    if (!result)
    {
        [NSException raise:@"Adding child error" format:@"Failed to add object to scene."];
    }

    const int64_t occluderId = reinterpret_cast<int64_t>(occluder);
    [mOccluders setObject:occluder forKey:[NSNumber numberWithLongLong:occluderId]];

    return occluderId;
}

- (BOOL)setOccluderTransformWithId:(int64_t)occluderId transform:(simd_float4x4)transform
{
    PHASEOccluder* occluder = [mOccluders objectForKey:[NSNumber numberWithLongLong:occluderId]];
    if (occluder == nil)
    {
        return NO;
    }

    occluder.transform = transform;
    return YES;
}

- (BOOL)setOccluderMaterialWithId:(int64_t)occluderId materialName:(NSString*)materialName
{
    PHASEOccluder* occluder = [mOccluders objectForKey:[NSNumber numberWithLongLong:occluderId]];
    if (occluder == nil)
    {
        return NO;
    }

    // Set the material on the occluder
    auto shapeElements = occluder.shapes[0].elements;
    for (PHASEShapeElement* element : shapeElements)
    {
        element.material = [mMaterials objectForKey:materialName];
    }

    return YES;
}

- (void)destroyOccluderWithId:(int64_t)occluderId
{
    PHASEOccluder* occluder = [mOccluders objectForKey:[NSNumber numberWithLongLong:occluderId]];
    if (occluder != nil)
    {
        // Remove from the hierarchy
        [mEngine.rootObject removeChild:occluder];

        [mOccluders removeObjectForKey:[NSNumber numberWithLongLong:occluderId]];

        occluder = nil;
    }
}

- (BOOL)createMaterialWithName:(NSString*)name preset:(MaterialPreset)preset
{
    PHASEMaterialPreset phasePreset;
    switch (preset)
    {
        case MaterialPresetCardboard:
            phasePreset = PHASEMaterialPresetCardboard;
            break;
        case MaterialPresetGlass:
            phasePreset = PHASEMaterialPresetGlass;
            break;
        case MaterialPresetBrick:
            phasePreset = PHASEMaterialPresetBrick;
            break;
        case MaterialPresetConcrete:
            phasePreset = PHASEMaterialPresetConcrete;
            break;
        case MaterialPresetDrywall:
            phasePreset = PHASEMaterialPresetDrywall;
            break;
        case MaterialPresetWood:
            phasePreset = PHASEMaterialPresetWood;
            break;
    }

    PHASEMaterial* material = [[PHASEMaterial alloc] initWithEngine:mEngine preset:phasePreset];

    if (material == nil)
    {
        NSLog(@"Failed to create material from preset.");
        return NO;
    }
    [mMaterials setObject:material forKey:name];

    return YES;
}

- (void)destroyMaterialWithName:(NSString*)name
{
    [mMaterials removeObjectForKey:name];
}

- (void)setSceneReverbWithPreset:(PHASEReverbPreset)preset
{
    [mEngine setDefaultReverbPreset:preset];
}

- (BOOL)registerAudioBufferWithData:(NSData*)data identifier:(NSString*)uid audioFormat:(AVAudioFormat*)audioFormat
{
    NSError* error = nil;
    [mEngine.assetRegistry registerSoundAssetWithData:data
                                           identifier:uid
                                               format:audioFormat
                                    normalizationMode:PHASENormalizationModeDynamic
                                                error:&error];

    if (error)
    {
        NSLog(@"Failed to register audio buffer with error %@.", error);
        return NO;
    }

    return YES;
}

- (BOOL)registerAudioAssetWithURL:(NSURL*)url identifier:(NSString*)uid audioFormat:(AVAudioFormat*)audioFormat
{
    NSError* error = nil;
    [mEngine.assetRegistry registerSoundAssetAtURL:url
                                        identifier:uid
                                         assetType:PHASEAssetTypeStreamed
                                     channelLayout:audioFormat.channelLayout
                                 normalizationMode:PHASENormalizationModeDynamic
                                             error:&error];

    if (error)
    {
        NSLog(@"Failed to register audio asset with error %@.", error);
        return NO;
    }

    return YES;
}

- (void)unregisterAudioBufferWithIdentifier:(NSString*)uid
{
    [mEngine.assetRegistry unregisterAssetWithIdentifier:uid completion:nil];
}

- (int64_t)createSpatialMixerWithName:(NSString*)mixerName
                     enableDirectPath:(BOOL)enableDirectPath
               enableEarlyReflections:(BOOL)enableEarlyReflections
                     enableLateReverb:(BOOL)enableLateReverb
                         cullDistance:(double)cullDistance
     sourceDirectivityModelParameters:(DirectivityModelParameters)sourceDirectivityModelParameters
   listenerDirectivityModelParameters:(DirectivityModelParameters)listenerDirectivityModelParameters
{
    PHASESpatialPipelineFlags spatialPipelineFlags = 0UL;
    if (enableDirectPath)
    {
        spatialPipelineFlags |= PHASESpatialPipelineFlagDirectPathTransmission;
    }
    if (enableEarlyReflections)
    {
        spatialPipelineFlags |= PHASESpatialPipelineFlagEarlyReflections;
    }
    if (enableLateReverb)
    {
        spatialPipelineFlags |= PHASESpatialPipelineFlagLateReverb;
    }

    PHASESpatialPipeline* spatialPipeline = [[PHASESpatialPipeline alloc] initWithFlags:spatialPipelineFlags];

    if (enableDirectPath == YES)
    {
        PHASENumberMetaParameterDefinition* directPathSend =
          [[PHASENumberMetaParameterDefinition alloc] initWithValue:1.0 identifier:[mixerName stringByAppendingString:@"DirectPathSend"]];
        spatialPipeline.entries[PHASESpatialCategoryDirectPathTransmission].sendLevel = 1.0;
        spatialPipeline.entries[PHASESpatialCategoryDirectPathTransmission].sendLevelMetaParameterDefinition = directPathSend;
    }

    if (enableEarlyReflections == YES)
    {
        PHASENumberMetaParameterDefinition* earlyReflectionsSend =
          [[PHASENumberMetaParameterDefinition alloc] initWithValue:1.0
                                                         identifier:[mixerName stringByAppendingString:@"EarlyReflectionsSend"]];
        spatialPipeline.entries[PHASESpatialCategoryEarlyReflections].sendLevelMetaParameterDefinition = earlyReflectionsSend;
    }

    if (enableLateReverb == YES)
    {
        PHASENumberMetaParameterDefinition* lateReverbSend =
          [[PHASENumberMetaParameterDefinition alloc] initWithValue:1.0 identifier:[mixerName stringByAppendingString:@"LateReverbSend"]];
        spatialPipeline.entries[PHASESpatialCategoryLateReverb].sendLevelMetaParameterDefinition = lateReverbSend;
    }

    // Create geometric distance model.
    PHASEGeometricSpreadingDistanceModelParameters* geometricSpreadingDistanceModelParameters =
      [[PHASEGeometricSpreadingDistanceModelParameters alloc] init];

    // Create distance model fade if we have fades
    if (cullDistance > 0.0f)
    {
        PHASEDistanceModelFadeOutParameters* distanceModelFadeOutParameters =
          [[PHASEDistanceModelFadeOutParameters alloc] initWithCullDistance:cullDistance];

        // Set fade out model on geometric distance model
        geometricSpreadingDistanceModelParameters.fadeOutParameters = distanceModelFadeOutParameters;
    }

    // Create spatial mixer
    PHASESpatialMixerDefinition* spatialMixer = [[PHASESpatialMixerDefinition alloc] initWithSpatialPipeline:spatialPipeline
                                                                                                   identifier:mixerName];
    // Attach distance model parameters to the mixer
    spatialMixer.distanceModelParameters = geometricSpreadingDistanceModelParameters;

    // Create Source Directivity Models if they exist
    switch (sourceDirectivityModelParameters.directivityType)
    {
        case Cardioid: {
            NSMutableArray* subbands = [[NSMutableArray alloc] init];
            for (int i = 0; i < sourceDirectivityModelParameters.subbandCount; i++)
            {
                // Set subband parameters
                PHASECardioidDirectivityModelSubbandParameters* cardioidSubbandParameters =
                  [[PHASECardioidDirectivityModelSubbandParameters alloc] init];

                cardioidSubbandParameters.frequency = sourceDirectivityModelParameters.subbandParameters[i].frequency;
                cardioidSubbandParameters.pattern = sourceDirectivityModelParameters.subbandParameters[i].pattern;
                cardioidSubbandParameters.sharpness = sourceDirectivityModelParameters.subbandParameters[i].sharpness;

                [subbands addObject:cardioidSubbandParameters];
            }
            PHASECardioidDirectivityModelParameters* cardioidDirectivityModel =
              [[PHASECardioidDirectivityModelParameters alloc] initWithSubbandParameters:subbands];

            // Attach source directivity model
            spatialMixer.sourceDirectivityModelParameters = cardioidDirectivityModel;
            break;
        }
        case Cone: {
            NSMutableArray* subbands = [[NSMutableArray alloc] init];
            for (int i = 0; i < sourceDirectivityModelParameters.subbandCount; i++)
            {
                // Set subband parameters
                PHASEConeDirectivityModelSubbandParameters* coneSubbandParameters =
                  [[PHASEConeDirectivityModelSubbandParameters alloc] init];

                coneSubbandParameters.frequency = sourceDirectivityModelParameters.subbandParameters[i].frequency;
                [coneSubbandParameters setInnerAngle:sourceDirectivityModelParameters.subbandParameters[i].innerAngle
                                          outerAngle:sourceDirectivityModelParameters.subbandParameters[i].outerAngle];
                coneSubbandParameters.outerGain = sourceDirectivityModelParameters.subbandParameters[i].outerGain;

                [subbands addObject:coneSubbandParameters];
            }
            PHASEConeDirectivityModelParameters* coneDirectivityModel =
              [[PHASEConeDirectivityModelParameters alloc] initWithSubbandParameters:subbands];
            // Attach source directivity model
            spatialMixer.sourceDirectivityModelParameters = coneDirectivityModel;
            break;
        }
        case None:
            break;
    }

    // Create Listener Directivity Models if they exist
    switch (listenerDirectivityModelParameters.directivityType)
    {
        case Cardioid: {
            NSMutableArray* subbands = [[NSMutableArray alloc] init];
            for (int i = 0; i < listenerDirectivityModelParameters.subbandCount; i++)
            {
                // Set subband parameters
                PHASECardioidDirectivityModelSubbandParameters* cardioidSubbandParameters =
                  [[PHASECardioidDirectivityModelSubbandParameters alloc] init];

                cardioidSubbandParameters.frequency = listenerDirectivityModelParameters.subbandParameters[i].frequency;
                cardioidSubbandParameters.pattern = listenerDirectivityModelParameters.subbandParameters[i].pattern;
                cardioidSubbandParameters.sharpness = listenerDirectivityModelParameters.subbandParameters[i].sharpness;

                [subbands addObject:cardioidSubbandParameters];
            }

            PHASECardioidDirectivityModelParameters* cardioidDirectivityModel =
              [[PHASECardioidDirectivityModelParameters alloc] initWithSubbandParameters:subbands];
            // Attach listener directivity model
            spatialMixer.listenerDirectivityModelParameters = cardioidDirectivityModel;
            break;
        }
        case Cone: {

            NSMutableArray* subbands = [[NSMutableArray alloc] init];
            for (int i = 0; i < listenerDirectivityModelParameters.subbandCount; i++)
            {
                // Set subband parameters
                PHASEConeDirectivityModelSubbandParameters* coneSubbandParameters =
                  [[PHASEConeDirectivityModelSubbandParameters alloc] init];

                coneSubbandParameters.frequency = listenerDirectivityModelParameters.subbandParameters[i].frequency;
                [coneSubbandParameters setInnerAngle:listenerDirectivityModelParameters.subbandParameters[i].innerAngle
                                          outerAngle:listenerDirectivityModelParameters.subbandParameters[i].outerAngle];
                coneSubbandParameters.outerGain = listenerDirectivityModelParameters.subbandParameters[i].outerGain;


                [subbands addObject:coneSubbandParameters];
            }

            PHASEConeDirectivityModelParameters* coneDirectivityModel =
              [[PHASEConeDirectivityModelParameters alloc] initWithSubbandParameters:subbands];
            // Attach listener directivity model
            spatialMixer.listenerDirectivityModelParameters = coneDirectivityModel;
            break;
        }
        case None:
            break;
    }

    const int64_t mixerId = reinterpret_cast<int64_t>(spatialMixer);
    [mSoundEventMixerDefinitions setObject:spatialMixer forKey:[NSNumber numberWithLongLong:mixerId]];
    return mixerId;
}

+ (AudioChannelLayoutTag)getChannelLayoutTag:(ChannelLayoutType)channelLayout
{
    AudioChannelLayoutTag layoutTag;
    switch (channelLayout)
    {
        case ChannelLayoutTypeMono:
            layoutTag = kAudioChannelLayoutTag_Mono;
            break;
        case ChannelLayoutTypeStereo:
            layoutTag = kAudioChannelLayoutTag_Stereo;
            break;
        case ChannelLayoutTypeFiveOne:
            layoutTag = kAudioChannelLayoutTag_MPEG_5_1_B;
            break;
        case ChannelLayoutTypeSevenOne:
            layoutTag = kAudioChannelLayoutTag_MPEG_7_1_B;
            break;
        default:
            NSLog(@"Failed to get channel layout tag for unsuported channel layout.");
    }
    return layoutTag;
}

- (int64_t)createChannelMixerWithName:(NSString*)mixerName channelLayout:(ChannelLayoutType)channelLayout
{
    AudioChannelLayoutTag layoutTag = [PHASEEngineWrapper getChannelLayoutTag:channelLayout];
    AVAudioChannelLayout* layout = [[AVAudioChannelLayout alloc] initWithLayoutTag:layoutTag];
    PHASEChannelMixerDefinition* channelMixer = [[PHASEChannelMixerDefinition alloc] initWithChannelLayout:layout];
    const int64_t mixerId = reinterpret_cast<int64_t>(channelMixer);
    [mSoundEventMixerDefinitions setObject:channelMixer forKey:[NSNumber numberWithLongLong:mixerId]];
    return mixerId;
}

- (int64_t)createAmbientMixerWithName:(NSString*)mixerName
                         channelLayout:(ChannelLayoutType)channelLayout
                           orientation:(Quaternion)orientation
{
    AudioChannelLayoutTag layoutTag = [PHASEEngineWrapper getChannelLayoutTag:channelLayout];
    AVAudioChannelLayout* layout = [[AVAudioChannelLayout alloc] initWithLayoutTag:layoutTag];
    simd_quatf quat = simd_quaternion(orientation.x, orientation.y, orientation.z, orientation.w);
    PHASEAmbientMixerDefinition* ambientMixer = [[PHASEAmbientMixerDefinition alloc] initWithChannelLayout:layout
                                                                                               orientation:quat
                                                                                                identifier:mixerName];
    const int64_t mixerId = reinterpret_cast<int64_t>(ambientMixer);
    [mSoundEventMixerDefinitions setObject:ambientMixer forKey:[NSNumber numberWithLongLong:mixerId]];
    return mixerId;
}

- (void)destroyMixerWithId:(int64_t)mixerId
{
    [mSoundEventMixerDefinitions removeObjectForKey:[NSNumber numberWithLongLong:mixerId]];
}

- (int64_t)createMetaParameterWithName:(NSString*)name defaultIntValue:(int)defaultIntValue
{
    PHASENumberMetaParameterDefinition* param = [[PHASENumberMetaParameterDefinition alloc] initWithValue:defaultIntValue identifier:name];

    const int64_t paramId = reinterpret_cast<int64_t>(param);
    [mSoundEventMetaParameterDefinitions setObject:param forKey:[NSNumber numberWithLongLong:paramId]];
    return paramId;
}

- (BOOL)setMetaParameterWithId:(int64_t)instanceId parameterName:(NSString*)parameterName intValue:(int)intValue
{
    PHASESoundEvent* soundEvent = mSoundEvents[[NSNumber numberWithLongLong:instanceId]];
    if (soundEvent == nil || soundEvent.metaParameters[parameterName] == nil)
    {
        return NO;
    }
    
    if ([soundEvent.metaParameters[parameterName] isKindOfClass:[PHASENumberMetaParameter class]])
    {
        soundEvent.metaParameters[parameterName].value = [NSNumber numberWithInt:intValue];
        return YES;
    }
    else
    {
        NSLog(@"Warning: PHASE API missuse, cannot set class %@ with value of type int.",
              [soundEvent.metaParameters[parameterName] class]);
        return NO;
    }


}

- (int64_t)createMetaParameterWithName:(NSString*)name defaultDblValue:(double)defaultDblValue
{
    PHASENumberMetaParameterDefinition* param = [[PHASENumberMetaParameterDefinition alloc] initWithValue:defaultDblValue identifier:name];

    const int64_t paramId = reinterpret_cast<int64_t>(param);
    [mSoundEventMetaParameterDefinitions setObject:param forKey:[NSNumber numberWithLongLong:paramId]];
    return paramId;
}

- (BOOL)setMetaParameterWithId:(int64_t)instanceId parameterName:(NSString*)parameterName doubleValue:(double)doubleValue
{
    PHASESoundEvent* soundEvent = mSoundEvents[[NSNumber numberWithLongLong:instanceId]];
    if (soundEvent == nil || soundEvent.metaParameters[parameterName] == nil)
    {
        return NO;
    }

    if ([soundEvent.metaParameters[parameterName] isKindOfClass:[PHASENumberMetaParameter class]])
    {
        soundEvent.metaParameters[parameterName].value = [NSNumber numberWithDouble:doubleValue];
        return YES;
    }
    else
    {
        NSLog(@"Warning: PHASE API missuse, cannot set class %@ with value of type double.",
              [soundEvent.metaParameters[parameterName] class]);
        return NO;
    }
}

- (int64_t)createMetaParameterWithName:(NSString*)name defaultStrValue:(NSString*)defaultStrValue
{
    PHASEStringMetaParameterDefinition* param = [[PHASEStringMetaParameterDefinition alloc] initWithValue:defaultStrValue identifier:name];

    const int64_t paramId = reinterpret_cast<int64_t>(param);
    [mSoundEventMetaParameterDefinitions setObject:param forKey:[NSNumber numberWithLongLong:paramId]];
    return paramId;
}

- (BOOL)setMetaParameterWithId:(int64_t)instanceId parameterName:(NSString*)parameterName stringValue:(NSString*)stringValue;
{
    PHASESoundEvent* soundEvent = mSoundEvents[[NSNumber numberWithLongLong:instanceId]];
    if (soundEvent == nil || soundEvent.metaParameters[parameterName] == nil)
    {
        return NO;
    }

    if ([soundEvent.metaParameters[parameterName] isKindOfClass:[PHASEStringMetaParameter class]])
    {
        soundEvent.metaParameters[parameterName].value = stringValue;
        return YES;
    }
    else
    {
        NSLog(@"Warning: PHASE API missuse, cannot set class %@ with value of type string.",
              [soundEvent.metaParameters[parameterName] class]);
        return NO;
    }
}

- (void)destroyMetaParameterWithId:(int64_t)parameterId
{
    [mSoundEventMetaParameterDefinitions removeObjectForKey:[NSNumber numberWithLongLong:parameterId]];
}

- (int64_t)createMappedMetaParameterWithParameterId:(int64_t)parameterId envelopeParameters:(EnvelopeParameters)envelopeParameters
{
    // Create Envelope
    NSMutableArray<PHASEEnvelopeSegment*>* envelopeSegments = [NSMutableArray new];
    for (int i = 0; i < envelopeParameters.segmentCount; i++)
    {
        enum PHASECurveType type;
        switch (envelopeParameters.envelopeSegments[i].curveType)
        {
            case EnvelopeCurveTypeLinear:
                type = PHASECurveTypeLinear;
                break;
            case EnvelopeCurveTypeSquared:
                type = PHASECurveTypeSquared;
                break;
            case EnvelopeCurveTypeInverseSquared:
                type = PHASECurveTypeInverseSquared;
                break;
            case EnvelopeCurveTypeCubed:
                type = PHASECurveTypeCubed;
                break;
            case EnvelopeCurveTypeInverseCubed:
                type = PHASECurveTypeInverseCubed;
                break;
            case EnvelopeCurveTypeSine:
                type = PHASECurveTypeSine;
                break;
            case EnvelopeCurveTypeInverseSine:
                type = PHASECurveTypeInverseSine;
                break;
            case EnvelopeCurveTypeSigmoid:
                type = PHASECurveTypeSigmoid;
                break;
            case EnvelopeCurveTypeInverseSigmoid:
                type = PHASECurveTypeInverseSigmoid;
                break;
        }
        PHASEEnvelopeSegment* segment = [[PHASEEnvelopeSegment alloc]
          initWithEndPoint:simd_make_double2(envelopeParameters.envelopeSegments[i].x, envelopeParameters.envelopeSegments[i].y)
                 curveType:type];
        [envelopeSegments addObject:segment];
    }
    PHASEEnvelope* envelope = [[PHASEEnvelope alloc] initWithStartPoint:simd_make_double2(envelopeParameters.x, envelopeParameters.y)
                                                               segments:envelopeSegments];

    PHASENumberMetaParameterDefinition* metaParameter =
      [mSoundEventMetaParameterDefinitions objectForKey:[NSNumber numberWithLongLong:parameterId]];
    PHASEMappedMetaParameterDefinition* mappedMetaParameter =
      [[PHASEMappedMetaParameterDefinition alloc] initWithInputMetaParameterDefinition:metaParameter envelope:envelope];

    const int64_t mappedMetaParamId = reinterpret_cast<int64_t>(mappedMetaParameter);
    [mSoundEventMetaParameterDefinitions setObject:mappedMetaParameter forKey:[NSNumber numberWithLongLong:mappedMetaParamId]];
    return mappedMetaParamId;
}

- (void)destoryMappedMetaParameterWithId:(int64_t)parameterId
{
    [mSoundEventMetaParameterDefinitions removeObjectForKey:[NSNumber numberWithLongLong:parameterId]];
}

- (int64_t)createSoundEventSamplerNodeWithAsset:(NSString*)assetName
                                        mixerId:(int64_t)mixerId
                                        looping:(BOOL)looping
                                calibrationMode:(CalibrationMode)calibrationMode
                                          level:(double)level
{
    // Find the mixer
    PHASEMixerDefinition* mixer = [mSoundEventMixerDefinitions objectForKey:[NSNumber numberWithLongLong:mixerId]];
    if (mixer == nil)
    {
        [NSException raise:@"Mixer invalid" format:@"Failed to create sampler node."];
    }

    // Create sampler
    PHASESamplerNodeDefinition* sampler = [[PHASESamplerNodeDefinition alloc] initWithSoundAssetIdentifier:assetName
                                                                                           mixerDefinition:mixer];
    sampler.rate = 1;
    sampler.cullOption = PHASECullOptionSleepWakeAtRealtimeOffset;
    sampler.playbackMode = looping ? PHASEPlaybackModeLooping : PHASEPlaybackModeOneShot;

    PHASECalibrationMode outCalibrationMode;
    switch (calibrationMode)
    {
        case CalibrationModeNone:
            outCalibrationMode = PHASECalibrationModeNone;
            break;
        case CalibrationModeAbsoluteSpl:
            outCalibrationMode = PHASECalibrationModeAbsoluteSpl;
            break;
        case CalibrationModeRelativeSpl:
            outCalibrationMode = PHASECalibrationModeRelativeSpl;
            break;
    }
    [sampler setCalibrationMode:outCalibrationMode level:level];

    const int64_t samplerId = reinterpret_cast<int64_t>(sampler);
    [mSoundEventNodeDefinitions setObject:sampler forKey:[NSNumber numberWithLongLong:samplerId]];
    return samplerId;
}

- (int64_t)createSoundEventSwitchNodeWithParameter:(int64_t)parameterId switchEntries:(NSDictionary*)switchEntries
{
    PHASEStringMetaParameterDefinition* param =
      [mSoundEventMetaParameterDefinitions objectForKey:[NSNumber numberWithLongLong:parameterId]];
    if (param == nil)
    {
        [NSException raise:@"Parameter invalid" format:@"Failed to find switch parameter."];
    }

    PHASESwitchNodeDefinition* switchNode = [[PHASESwitchNodeDefinition alloc] initWithSwitchMetaParameterDefinition:param];
    if (switchNode == nil)
    {
        [NSException raise:@"Switch create failed" format:@"Failed to create switch node."];
    }

    for (id entry in switchEntries)
    {
        PHASESoundEventNodeDefinition* node = [mSoundEventNodeDefinitions objectForKey:[NSNumber numberWithLongLong:[entry longLongValue]]];
        if (node == nil)
        {
            [NSException raise:@"Switch create failed" format:@"Failed to create switch node."];
        }
        [switchNode addSubtree:node switchValue:[switchEntries objectForKey:entry]];
    }

    const int64_t switchId = reinterpret_cast<int64_t>(switchNode);
    [mSoundEventNodeDefinitions setObject:switchNode forKey:[NSNumber numberWithLongLong:switchId]];
    return switchId;
}

- (int64_t)createSoundEventRandomNodeWithEntries:(NSDictionary*)randomEntries
{
    PHASERandomNodeDefinition* randomNode = [[PHASERandomNodeDefinition alloc] init];
    if (randomNode == nil)
    {
        [NSException raise:@"Random create failed" format:@"Failed to create random node."];
    }

    for (id entry in randomEntries)
    {
        PHASERandomNodeDefinition* node = [mSoundEventNodeDefinitions objectForKey:[NSNumber numberWithLongLong:[entry longLongValue]]];
        if (node == nil)
        {
            [NSException raise:@"Random create failed" format:@"Failed to create random node."];
        }
        NSNumber* weight = [randomEntries objectForKey:entry];
        [randomNode addSubtree:node weight:weight];
    }

    const int64_t randomId = reinterpret_cast<int64_t>(randomNode);
    [mSoundEventNodeDefinitions setObject:randomNode forKey:[NSNumber numberWithLongLong:randomId]];
    return randomId;
}

- (int64_t)createSoundEventBlendNodeWithParameter:(int64_t)parameterId
                                      blendRanges:(BlendNodeEntry*)blendRanges
                                        numRanges:(uint32_t)numRanges
                             useAutoDistanceBlend:(bool)useAutoDistanceBlend
{
    PHASEBlendNodeDefinition* blendNode = nil;
    if (useAutoDistanceBlend)
    {
        PHASESpatialMixerDefinition* mixer = [mSoundEventMixerDefinitions objectForKey:[NSNumber numberWithLongLong:parameterId]];

        if (mixer == nil)
        {
            [NSException raise:@"Mixer invalid" format:@"Failed to find mixer for auto distance blending."];
        }

        blendNode = [[PHASEBlendNodeDefinition alloc] initDistanceBlendWithSpatialMixerDefinition:mixer];
        if (blendNode == nil)
        {
            [NSException raise:@"Blend create failed" format:@"Failed to create blend node."];
        }
    }
    else
    {
        PHASENumberMetaParameterDefinition* param =
          [mSoundEventMetaParameterDefinitions objectForKey:[NSNumber numberWithLongLong:parameterId]];
        if (param == nil)
        {
            [NSException raise:@"Parameter invalid" format:@"Failed to find blend parameter."];
        }

        blendNode = [[PHASEBlendNodeDefinition alloc] initWithBlendMetaParameterDefinition:param];
        if (blendNode == nil)
        {
            [NSException raise:@"Blend create failed" format:@"Failed to create blend node."];
        }
    }

    for (int rangeIdx = 0; rangeIdx < numRanges; ++rangeIdx)
    {
        const bool isLeftRange = (rangeIdx == 0);
        const bool isRightRange = (rangeIdx == (numRanges - 1));

        NSNumber* rangeNodeId = [NSNumber numberWithLongLong:blendRanges[rangeIdx].nodeId];
        PHASESoundEventNodeDefinition* rangeNode = [mSoundEventNodeDefinitions objectForKey:rangeNodeId];
        if (rangeNode == nil)
        {
            [NSException raise:@"Blend range create failed" format:@"Failed to create blend range node."];
        }

        if (isLeftRange)
        {
            [blendNode addRangeForInputValuesBelow:blendRanges[rangeIdx].highValue
                                   fullGainAtValue:blendRanges[rangeIdx].fullGainAtHigh
                                     fadeCurveType:PHASECurveTypeLinear
                                           subtree:rangeNode];
        }
        else if (isRightRange)
        {
            [blendNode addRangeForInputValuesAbove:blendRanges[rangeIdx].lowValue
                                   fullGainAtValue:blendRanges[rangeIdx].fullGainAtLow
                                     fadeCurveType:PHASECurveTypeLinear
                                           subtree:rangeNode];
        }
        else
        {
            [blendNode addRangeForInputValuesBetween:blendRanges[rangeIdx].lowValue
                                           highValue:blendRanges[rangeIdx].fullGainAtHigh
                                  fullGainAtLowValue:blendRanges[rangeIdx].fullGainAtLow
                                 fullGainAtHighValue:blendRanges[rangeIdx].highValue
                                    lowFadeCurveType:PHASECurveTypeLinear
                                   highFadeCurveType:PHASECurveTypeLinear
                                             subtree:rangeNode];
        }
    }

    const int64_t blendId = reinterpret_cast<int64_t>(blendNode);
    [mSoundEventNodeDefinitions setObject:blendNode forKey:[NSNumber numberWithLongLong:blendId]];
    return blendId;
}

- (int64_t)createSoundEventContainerNodeWithSubtree:(int64_t*)subtreeIds numSubtrees:(uint32_t)numSubtrees
{
    if (subtreeIds == nil)
    {
        [NSException raise:@"subtree invalid" format:@"Failed to create Container Node with nil subtreeIds."];
    }
    if (numSubtrees <= 0)
    {
        [NSException raise:@"number of subtrees invalid" format:@"Failed to create Container Node with invalid numSubtrees."];
    }
    PHASEContainerNodeDefinition* containerNode = [[PHASEContainerNodeDefinition alloc] init];
    for (int i = 0; i < numSubtrees; i++)
    {
        NSNumber* nodeId = [NSNumber numberWithLongLong:subtreeIds[i]];
        PHASESoundEventNodeDefinition* subtree = [mSoundEventNodeDefinitions objectForKey:nodeId];
        [containerNode addSubtree:subtree];
    }
    const int64_t containerId = reinterpret_cast<int64_t>(containerNode);
    [mSoundEventNodeDefinitions setObject:containerNode forKey:[NSNumber numberWithLongLong:containerId]];
    return containerId;
}

- (void)destroySoundEventNodeWithId:(int64_t)nodeId
{
    [mSoundEventNodeDefinitions removeObjectForKey:[NSNumber numberWithLongLong:nodeId]];
}

- (BOOL)registerSoundEventWithName:(NSString*)name rootNodeId:(int64_t)rootNodeId
{
    // Create a tree builder with the sampler node
    PHASESoundEventNodeDefinition* rootNode = [mSoundEventNodeDefinitions objectForKey:[NSNumber numberWithLongLong:rootNodeId]];
    if (rootNode == nil)
    {
        NSLog(@"Root node not found.");
        return NO;
    }

    // Register action tree
    NSError* error = nil;
    [mEngine.assetRegistry registerSoundEventAssetWithRootNode:rootNode identifier:name error:&error];
    if (error)
    {
        NSLog(@"Failed to register action tree with error %@.", error);
        return NO;
    }

    return YES;
}

- (void)unregisterSoundEventWithName:(NSString*)name
{
    [mEngine.assetRegistry unregisterAssetWithIdentifier:name
                                              completion:^(bool success) {
                                                if (!success)
                                                {
                                                    NSLog(@"Failed to unregister SoundEvent with name: %@", name);
                                                }
                                              }];
}

- (int64_t)playSoundEventWithName:(NSString*)name
                         sourceId:(int64_t)sourceId
                         mixerIds:(int64_t*)mixerIds
                        numMixers:(uint64_t)numMixers
                completionHandler:
                  (void (*)(PHASESoundEventStartHandlerReason reason, int64_t sourceId, int64_t soundEventId))completionHandler
{
    PHASESource* source = [mSources objectForKey:[NSNumber numberWithLongLong:sourceId]];
    if (source == nil)
    {
        [NSException raise:@"Source invalid" format:@"Failed to find PHASE Source to play action tree on."];
    }

    PHASEMixerParameters* outMixerParameters = [[PHASEMixerParameters alloc] init];
    for (int i = 0; i < numMixers; i++)
    {
        PHASEMixerDefinition* mixer = [mSoundEventMixerDefinitions objectForKey:[NSNumber numberWithLongLong:mixerIds[i]]];
        if ([mixer isKindOfClass:[PHASESpatialMixerDefinition class]])
        {
            [outMixerParameters addSpatialMixerParametersWithIdentifier:mixer.identifier
                                                                 source:[mSources objectForKey:[NSNumber numberWithLongLong:sourceId]]
                                                               listener:mListener];
        }
        else if ([mixer isKindOfClass:[PHASEAmbientMixerDefinition class]])
        {
            [outMixerParameters addAmbientMixerParametersWithIdentifier:mixer.identifier listener:mListener];
        }
    }

    PHASESoundEvent* soundEvent = nil;
    NSError* error = nil;
    if (numMixers == 0)
    {
        soundEvent = [[PHASESoundEvent alloc] initWithEngine:mEngine assetIdentifier:name error:&error];
    }
    else
    {
        soundEvent = [[PHASESoundEvent alloc] initWithEngine:mEngine assetIdentifier:name mixerParameters:outMixerParameters error:&error];
    }
    if (error != nil)
    {
        NSLog(@"Error creating sound event: %@", name);
        NSLog(@"%@", error);
    }
    if (soundEvent == nil)
    {
        [NSException raise:@"Sound event invalid." format:@"Failed to create sound event from action tree."];
    }
    const int64_t soundEventId = reinterpret_cast<int64_t>(soundEvent);

    [soundEvent startWithCompletion:^(PHASESoundEventStartHandlerReason reason) {
      @synchronized(self->mSoundEvents)
      {
          if (completionHandler != nil)
          {
              completionHandler(reason, sourceId, soundEventId);
          }
          [self->mSoundEvents removeObjectForKey:[NSNumber numberWithLongLong:soundEventId]];

          // Decrement the number of events referencing the tree asset
          self->mActiveSoundEventAssets[name] = [NSNumber numberWithInt:self->mActiveSoundEventAssets[name].intValue - 1];
      }
    }];
    @synchronized(mSoundEvents)
    {
        [mSoundEvents setObject:soundEvent forKey:[NSNumber numberWithLongLong:soundEventId]];

        // Increment the number of events referencing the tree asset
        mActiveSoundEventAssets[name] = [NSNumber numberWithInt:mActiveSoundEventAssets[name].intValue + 1];
    }

    return soundEventId;
}

- (BOOL)stopSoundEventWithId:(int64_t)instanceId
{
    PHASESoundEvent* soundEvent = mSoundEvents[[NSNumber numberWithLongLong:instanceId]];
    if (soundEvent == nil)
    {
        return NO;
    }

    [soundEvent stopAndInvalidate];
    return YES;
}

- (BOOL)start
{
    // Start the engine
    NSError* startErrorRef = [NSError alloc];
    BOOL startErrorRet = [mEngine startAndReturnError:&startErrorRef];
    if (!startErrorRet)
    {
        NSLog(@"Failed to start PHASEEngine with error %@.", startErrorRef);
    }

    return startErrorRet;
}

- (void)stop
{
    [mEngine stop];
}

- (void)update
{
    [mEngine update];
}

@end
