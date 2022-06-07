# Apple - Core

## Installation Instructions
See the [Quick-Start Guide](../../../../../../Documentation/Quickstart.md) for general installation instructions.

## Usage
Apple.Core unifies build options and processing across all Apple Unity plug-ins. As a result all Apple Unity plug-ins are dependent upon Apple.Core. 

In general, interaction with Apple.Core will be via the Apple Build Settings section of the Project Settings window. However, it is also possible to leverage Apple.Core by creating your own build steps which are compatible with Apple.Core build step management.

### Table of Contents
[Manage Build Settings](#Manage-Build-Settings)
[Create Custom Build Steps](#Create-Custom-Build-Steps)
* [Background](#Background)
* [Define a Custom Build Step](#Define-a-Custom-Build-Step)
* [When are Build Steps Called?](#When-are-Build-Steps-Called?)

## Manage Build Settings
All Apple Unity plug-ins will expose build options through the common Apple Build Settings interface, found under Project Settings. Each Apple Unity plug-in's build settings will be automatically included here after successful import into your project.

To view build settings, open Unity's Project Settings window:

![ProjectSettings](Images~/ACD001.png)

And select Apple Build Settings from the menu. It is now possible to explore build settings for each included plug-in by clicking the icon next to the plug-in name.

![AppleBuildSettings](Images~/ACD002.png)
**Note:** The example in this screenshot shows the Apple Build Settings for a project with both the Apple.Core and Apple.GameController plug-ins installed.

## Create Custom Build Steps

### Background
Each Apple Unity plug-in potentially requires a series of settings to be configured for the final app compilation. This might include ensuring libraries are in the correct locations, updating info.plist, or updating entitlement settings. To facilitate this, Apple.Core defines a Unity PostProcessBuild step to perform each of these tasks. Furthermore, Apple.Core defines an abstract class, AppleBuildStep, to easily define this functionality for all Apple Unity plug-ins. In fact, you will find that each Apple Unity plug-in defines its own build step which derives from AppleBuildStep. It is also possible for any project that includes Apple.Core to define custom build steps.

### Define a Custom Build Step
Defining a custom build step is as simple as creating a new script and defining a new object which inherits from AppleBuildStep. From there, any of the methods defined in AppleBuildStep may be optionally overriden. Furthermore, any public fields will be automatically exposed as options in the Apple Build Settings UI.

Here's a simple example of a custom build step implementation:
```C#
using Apple.Core;

namespace MyProject.Editor
{
    public class MyCustomBuildStep : AppleBuildStep
    {
        public override void OnBeginPostProcess(AppleBuildProfile appleBuildProfile, BuildTarget buildTarget, string pathToBuiltProject)
        {
            Debug.Log("OnBeginPostProcess was called for my custom build step.");
        }
    }
}
```

For further implementation examples, check out the custom build steps defined with each Apple Unity plug-in.

### When are Build Steps Called?
Objects deriving from AppleBuildStep are invoked and their methods called as a post process to Unity's build, but prior to the final compilation of the app. Each build step may optionally define any of the following overrides:

1. OnBeginPostProcess
2. OnProcessInfoPlist
3. OnProcessEntitlements
4. OnProcessFrameworks
5. OnProcessExportPlistOptions
6. OnFinalizePostProcess

These methods will be called strictly in the order listed. Furthermore, each method listed above will be called on all objects inheriting from AppleBuildStep before moving on to the next method in the list. For example, OnBeginPostProcess will be called on all AppleBuildStep objects prior to moving on to calls to OnProcessInfoPlist.



