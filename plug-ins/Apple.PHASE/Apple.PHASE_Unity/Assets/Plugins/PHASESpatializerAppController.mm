#import "UnityAppController.h"

extern "C" {
struct UnityAudioEffectDefinition;
typedef int (*UnityPluginGetAudioEffectDefinitionsFunc)(
    struct UnityAudioEffectDefinition*** descptr);
extern void UnityRegisterAudioPlugin(
    UnityPluginGetAudioEffectDefinitionsFunc getAudioEffectDefinitions);
extern int UnityGetAudioEffectDefinitions(UnityAudioEffectDefinition*** definitionptr);
}  // extern "C"

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
