# Apple - PHASE

## Installation Instructions

### 1. Install Dependencies
* Apple.Core

### 2. Install the Package
See the [Quick-Start Guide](../../../../../Documentation/Quickstart.md) for general installation instructions.

## Samples
This plug-in includes a sample scene with SoundEvents, scripts, prefabs and audio assets.
The sample can be imported via the `Window > Package Manager`. In the packages list view, select `Apple.PHASE` and in the Samples section cilck the Import button.

DemoScene.unity contains a PHASESource that rotates around a PHASEListener and PHASEOccluder.
Additional example SoundEvents can be used by setting the SoundEvent field on the PHASESource.

The Crowd and Footsteps SoundEvents have accompanying scripts (AmbienceBlender.cs and FootstepTrigger.cs respectively) to control MetaParameter values.
These are most easily used by dragging the Crowd or Footsteps prefab into the active scene. They may then be controlled via the exposed UI elements in the Inspector.

Please note that in order to use the StereoPopcorn SoundEvent, the `Popcorn_Panned.wav` file must be copied manually from the source repository into your project. It can be found at `plug-ins/Apple.PHASE/Apple.PHASE_Unity/Assets/StreamingAssets`.

## Usage
Please find an introduction to using the PHASE Unity Plug-in below. For an overview of the PHASE.framework, please see [PHASE Developer Documentation](https://developer.apple.com/documentation/phase/)

For documentation of PHASE's C# API see [PHASEHelpers](../Runtime/PHASEHelpers.cs). 

### Table of Contents
[Spatializer](#Spatializer)

* [1. Enable PHASE Spatializer](#1-Enable-PHASE-Spatializer)
* [2. Add a PHASEListener to the Scene](#2-Add-a-PHASEListener-to-the-Scene)
* [3. Spatialize Unity AudioSources](#3-Spatialize-Unity-AudioSources)

[PHASE](#PHASE)

* [1. Create a PHASEListener](#1-Create-a-PHASEListener)  
* [2. Create a PHASESource](#2-Create-a-PHASESource)  
* [3. Create a PHASESoundEvent](#3-Create-a-PHASESoundEvent)  
* [4. Create a PHASEOccluder](#4-Create-a-PHASEOccluder)  
* [5. Control a PHASESoundEvent via script](#5-Control-a-PHASESoundEvent-via-script)  
* [6. Change global reverb setting via script](#6-Change-global-reverb-setting-via-script)

[Sound Event Nodes](#Sound-Event-Nodes)
 
* [1. Sampler Node](#1-Sampler-Node)  
* [2. Blend Node](#2-Blend-Node)  
* [3. Container Node](#3-Container-Node)  
* [4. Switch Node](#4-Switch-Node)  
* [5. Random Node](#5-Random-Node)  

[Mixers](#-Mixers)

* [1. Spatial Mixer](#1-Spatial-Mixer)  
* [2. Channel Mixer](#2-Channel-Mixer)  
* [3. Ambient Mixer](#3-Ambient-Mixer)  

## Spatializer
### 1. Enable PHASE Spatializer
Enable the PHASE Spatializer within your Unity project by navigating to `Edit > Project Settings > Audio` and selecting `PHASE Spatializer` from the `Spatializer Plugin` dropdown menu.

### 2. Add a PHASEListener to the Scene.
In order for PHASE to render the spatialized sound, your scene must include a PHASEListener. For most use-cases, simply add a PHASEListener component to the GameObject that contains your main Unity AudioListener.

### 3. Spatialize Unity AudioSources
To spatialize a Unity AudioSource, the `Spatialize` setting must be set to true and the `Spatial Blend` setting should be set to 1.


## PHASE

### 1. Create a PHASEListener 
Create a `PHASEListener` by adding the `PHASEListener` component to any existing Unity `GameObject`. 

There can only be one `PHASEListener` per Unity scene.

The `PHASEListener` represents the "ears" of the simulation and all the 3D rendering will be based on this objects transformation. It also controls the default global reverb setting that effects the rendering of early reflections and late reverberation.


### 2. Create a PHASESource
The `PHASESource` represents a sound source in the virtual scene and can playback `PHASESoundEvent` assets. A `PHASESource` can either be a volumetric or point source.

Create a `PHASESource` by adding the `PHASESource` component to any existing Unity `GameObject`. If the `GameObject` has a `Mesh Filter` component, that will be used to define the geometry of a volumetric source. If there is no `Mesh Filter`, the `PHASESource` will be treated as a point source.
 
#### Properties: 
* **Sound Event**: The sound event asset to be triggered by this source.  
* **Source Mode**: A choice between volumetric or point source. Volumetric sources require a mesh on the `GameObject`, if none is provided, a point source will be created.  
* **Direct Path Send**: A linear value representing the level of send for the Direct Path acoustic simulation.  
* **Early Reflections**: Send A linear value representing the level of send for the Early Reflections acoustic simulation.  
* **Late Reverb Send**: A linear value representing the level of send for the Late Reverb acoustic simulation.  
* **Play on Awake**: Should this source start playing as soon as the scene loads

### 3. Create a PHASESoundEvent

- You can create a new `SoundEvent` via the menu dropdown `Assets > Create > Apple > PHASE > SoundEvent` or by right-clicking in the Project Tab.
- Double-click the new `SoundEvent` asset to open the `Sound Event Composer` window.
- Create a new `PHASESoundEventNode` by right-clicking in the `Sound Event Composer` window.
- You can connect two `PHASESoundEventNode`s by dragging from the `Child Node` port of one to the `Parent` port of another.
- To move the view, right-click on an empty space in the Sound Event Composer and drag. To zoom in/out use the mouse wheel.
- You can delete nodes by selecting them and pressing the Delete key, or by right-clicking and selecting `Remove`.

For descriptions of each node see [Sound Event Nodes](#Sound-Event-Nodes)

### 4. Create a PHASEOccluder
Create a `PHASEOccluder` by adding a `PHASEOccluder` component to an existing `GameObject` that contains a `Mesh Filter`.

The `PHASEOccluder` uses the `Mesh` of the object to which it belongs to occlude sound sources. 

When created, the `PHASEOccluder` component will automatically register the object's `Mesh` for optimal use with PHASE occlusion tracing.

#### Properties: 
- **Material**: The material used for the absorption and scattering characteristics of this object.

### 5. Control a PHASESoundEvent via script

`PHASESoundEvents` can be triggered via C# scripts.

```C#
using Apple.PHASE:

// A PHASESource component should be attached to a GameObject that emits sound.
public PHASESource source;

source = gameObject.GetComponent<PHASESource>();

source.Play():

// Stop playback.
if (source.IsPlaying())
{
    source.Stop();
}
```

`PHASEMetaParameters` can also be updated via C# script, impacting the playback of a `PHASESoundEvent`.


A `PHASESoundEventParameter` should be connected to a `PHASESoundEventNode` (i.e. Switch or Random node), and that `PHASESoundEvent` should be associated with the `PHASESource` that calls the method below.

```C#
source.SetMetaParameterValue("parameter", "value");
```

See [FootstepTrigger.cs](../Demos/Scripts/FootstepTrigger.cs) for an example.

### 6. Change global reverb setting via script

The global reverb settings for PHASE are set via the `PHASEListener`, and can be changed dynamically via C# script.

```C#
using Apple.PHASE;

// A PHASEListener component should be attached to a GameObject that acts as the "ears" of the scene.
PHASEListener listener;

listener = gameObject.GetComponent<PHASEListener>();

listener.SetReverbPreset(Helpers.ReverbPresets.LargeHall);
```

## Sound Event Nodes

### 1. Sampler Node
The `PHASESoundEventSamplerNode` plays an `AudioClip` through its associated Mixer.

#### Parameters:
- **Is Streaming Asset** Indicates that the asset is in the `StreamingAssets` directory.
- **Audio Clip** The associated `AudioClip`.
- **Calibartion Mode** Select a loudness correction strategy.
- **Level** Reference level for above calibration mode.
- **Looping** Indicates if the `AudioClip` is looping.
- **Mixer** The type of mixer that the Sampler Node will use. See [Mixers](#Mixers) for more details.

### 2. Blend Node
The `PHASESoundEventBlendNode` smoothly fades between the audio of its child nodes.

#### Parameters:  
- **Full Gain At High** The playback level of audio when the blend node is at High Value.  
- **High Value** The high value threshold.   
- **Low Value** The low value threshold.  
- **Full Gain At Low** The playback level of audio when the blend node is at low value.  
- **Blend Mode** The selected blend mode. Parameter or Auto Distance.  
  - **Parameter** The blend value is controlled by a `PHASESoundEventParameterDouble`.  
  - **Auto Distance** The blend value is automatically determined by a `PHASESpatialMixer`. 

See the Crowd asset in [Demos/SoundEvents](../Demos/SoundEvents/) for an example on how this can be used.  

### 3. Container Node
The `PHASESoundEventContainerNode` plays all of its child nodes at the same time.

### 4. Switch Node
The `PHASESoundEventSwitchNode` plays one of its child nodes depending on the value of its associated `PHASESoundEventParameterString`.

#### Parameters:
- **SwitchValue**: A string value for every child node.
- **Parameter**: A `PHASESoundEventParameterString` that controls which child node is played. If the value of `Parameter` is the same as the `SwitchValue` of a child node, that child node will be played when this node is triggered.

### 5. Random Node
The `PHASESoundEventRandomNode` plays one of its child nodes by random selection.

#### Parameters:
- **Weight**: The likelihood that this child node will be selected. Minimum value of 1 is least-likely while higher values are more likely.

## Mixers

### 1. Spatial Mixer
A `PHASESpatialMixer` plays audio with 3D position, orientation, and environmental effects. Use this mixer to take advantage of occlusion, reverb modelling and cull distance.

#### Parameters:
- **Direct Path Modeler** Enables Direct Path Modeling for this Mixer. Direct Path refers to sound traveling directly from source to listener without reflections.
- **Early Reflections Modeler** Enables Early Reflections Modeling for this Mixer. Early Reflection refers to the earlier echoes along the duration of sound resonance.   
- **Late Reverb Modeler** Enables Late Reverb Modeling for this Mixer. Late Reverb refers to the later echoes along the duration of sound resonance.  
- **Cull Distance** The distance beyond which the framework doesn't process a sound source. To disable culling, set to 0.  
- **Listener Directivity Properties** The directivity properties to apply to the `PHASEListener` that "hears" this mixer.  
- **Source Directivity Properties** The directivity properties to apply to any `PHASESource` that uses this mixer.

For more information see [Spatial Mixing](https://developer.apple.com/documentation/phase/spatial_mixing?language=objc).

### 2. Channel Mixer
A `PHASEChannelMixer` plays audio in a specific channel layout mode.

#### Parameters:
- **Channel Layout**: The channel layout of the mixer: `Mono`, `Stereo`, `5.1`, `7.1`

For more information see [PHASEChannelMixerDefinition](https://developer.apple.com/documentation/phase/phasechannelmixerdefinition?language=objc).

### 3. Ambient Mixer
A `PHASEAmbientMixer` plays audio with a specific 3D orientation. The 3D orientation is determined by the pitch, yaw and roll values.

#### Parameters:
- **Pitch** Pitch of the mixer's orientation.
- **Yaw** Yaw of the mixer's orientation.
- **Roll** Roll of the mixer's orientation.
- **Channel Layout** The channel layout of the mixer: `Mono`, `Stereo`, `5.1`, `7.1`

For more information see [PHASEAmbientMixerDefinition](https://developer.apple.com/documentation/phase/phaseambientmixerdefinition?language=objc).
