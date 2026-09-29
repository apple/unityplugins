extern "C" {
struct UnityAudioEffectDefinition;
typedef int (*UnityPluginGetAudioEffectDefinitionsFunc)(
    struct UnityAudioEffectDefinition*** descptr);
extern void UnityRegisterAudioPlugin(
    UnityPluginGetAudioEffectDefinitionsFunc getAudioEffectDefinitions);
extern int UnityGetAudioEffectDefinitions(UnityAudioEffectDefinition*** definitionptr);
}  // extern "C"

#if UNITY_XCODE_PROJECT_TYPE_SWIFT

#import <Foundation/Foundation.h>
#import <UnityAPI/UnityAPI-Swift.h>

@interface PHASESpatializerRegistration : NSObject
@end

@implementation PHASESpatializerRegistration

static id sPHASERuntimeInitObserver = nil;

+ (void)load
{
    sPHASERuntimeInitObserver =
        [[NSNotificationCenter defaultCenter] addObserverForName:UnityNotifications.unityDidInitializeRuntime
                                                          object:nil
                                                           queue:nil
                                                      usingBlock:^(NSNotification* note) {
            UnityRegisterAudioPlugin(UnityGetAudioEffectDefinitions);

            if (sPHASERuntimeInitObserver != nil)
            {
                [[NSNotificationCenter defaultCenter] removeObserver:sPHASERuntimeInitObserver];
                sPHASERuntimeInitObserver = nil;
            }
        }];
}

@end

#else

#import "UnityAppController.h"

@interface PHASESpatializerAppController : UnityAppController
- (void)shouldAttachRenderDelegate;
@end

@implementation PHASESpatializerAppController
- (void)shouldAttachRenderDelegate
{
    UnityRegisterAudioPlugin(UnityGetAudioEffectDefinitions);
}

@end
IMPL_APP_CONTROLLER_SUBCLASS(PHASESpatializerAppController);

#endif
