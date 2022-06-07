# Apple - Core Haptics

## Table of Contents
[Installation Instructions](#Installation-Instructions)

[Samples](#Samples)

[Usage](#Usage)

[Core Haptics](#Core-Haptics)
* [1. Create a CHHapticEngine](#1-Create-a-CHHapticEngine)
* [2. Design a Haptic Pattern](#2-Design-a-Haptic-Pattern)
* [3. Create a CHHapticPatternPlayer](#3-Create-a-CHHapticPatternPlayer)
* [4. Start the Engine and the Player](#4-Start-the-Engine-and-the-Player)
* [5. More on Haptic Patterns](#5-More-on-Haptic-Patterns)
* [6. Advanced Features](#6-Advanced-Features)

[UIFeedbackGenerator](#UIFeedbackGenerator)
* [1. UIFeedbackGenerator Usage](#1-UIFeedbackGenerator-Usage)

## Installation Instructions

### 1. Install Dependencies
* Apple.Core

### 2. Install the Package
See the [Quick-Start Guide](../../../../../../Documentation/Quickstart.md) for general installation instructions.

## Samples
This plug-in comes with two samples: _HapticRicochet_ and _SampleAHAPs_.

In the _SampleAHAPs_ demo, you will find a collection of AHAP files that could be useful in a variety of gaming scenarios. Feel free to include them in your project or use them as a starting point for creating your own haptic assets! (See below for some examples of how to incorporate these assets into your game.)

_HapticRicochet_ is a more fully-featured application that showcases some of the ways the CoreHaptics API can be leveraged to add haptics to a real game. For some examples, check out the methods `HasTexture.set`, `SetupHapticPlayers`, `OnCollisionEnter2D`, and `BallTapped` in `Ricochet/Scripts/BallManager.cs`, as well as `OnCollisionEnter2D` in  `Ricochet/Scripts/ShieldBehavior.cs`.

Please note that one additional step, after adding the sample to your project via the Unity Package Manager, is required for _HapticRicochet_ to build and run properly: There are some audio files that must be located in your project at the `Assets/StreamingAssets` path. These must be copied manually from the source repository into your project. They can be found at `plug-ins/Apple.CoreHaptics/Apple.CoreHaptics_Unity/Assets/StreamingAssets`.

## Usage
Please find an introduction to using Core Haptics below. For a more comprehensive guide to Core Haptics, please see the [Core Haptics Developer Documentation](https://developer.apple.com/documentation/corehaptics). For guidance on how and when to use haptics to enhance your users' experience, please check out the [Human Interface Guidelines for Haptics](https://developer.apple.com/design/human-interface-guidelines/ios/user-interaction/haptics/). 

In addition to the Core Haptics framework, this Plug-In also makes UIKit's UIFeedbackGenerator API available to Unity C#. A QuickStart guide for this API is also included below. For a comprehensive guide to using UIFeedbackGenerator, please see the [UIFeedbackGenerator Developer Documention](https://developer.apple.com/documentation/uikit/uifeedbackgenerator)

## Core Haptics

### 1. Create a `CHHapticEngine`
If you want your app to play custom haptics, you need to create a hapticengine. The haptic engine establishes the connection between your app and the underlying device hardware. Even though you can define a haptic pattern without an engine, you need the engine to play that pattern.

Create an engine like this:
```C#
using Apple.CoreHaptics;

var eng = new CHHapticEngine();
```

Optionally configure advanced engine properties (see [Advanced Features](#6-Advanced-Features)) 

### 2. Design a Haptic Pattern
Please refer to the [Core Haptics Apple Developer docs](https://developer.apple.com/documentation/corehaptics/chhapticpattern) for more detailed information on haptic patterns.

In this Plug-In, a haptic pattern is stored in a `CHHapticPattern` object, which can be created from an AHAP file or with an array of `CHHapticEvents`, `CHHapticParameters`, and `CHHapticParameterCurves`. Note that at least one `CHHapticEvent` must be provided, either with an array or in a file-based AHAP, for a pattern to play any content. `CHHapticParameters` and `CHHapticParameterCurves` are optional. For more information about AHAPs, please refer to the [Representing Haptic Patterns in AHAP Files](https://developer.apple.com/documentation/corehaptics/representing_haptic_patterns_in_ahap_files)

`CHHapticEvent` and `CHHapticPattern` objects can be created like this:
```C#
using Apple.CoreHaptics;

var events = new List<CHHapticEvent>
{
    new CHHapticTransientEvent {
        Time = 0f
    },
    new CHHapticContinuousEvent {
        Time = 0.5f,
        EventDuration = 1f
    }
};
var patternOne = new CHHapticPattern(events);
```

To learn more about creating haptic events and haptic patterns in Unity C#, see [Haptic Patterns](#5-More-on-Haptic-Patterns) below.

To create a `CHHapticPattern` object from an AHAP, place the AHAP in your project's `Resources` directory. If the file's name is "MyAHAP.ahap", create the `CHHapticPattern` like this:
```C#
using Apple.CoreHaptics;

var myAhapResource = Resources.Load<TextAsset>("MyAHAP");
var myPattern = new CHHapticPattern(myAhapResource)
```

**Note:** Be sure that any AudioCustom events in your AHAP reference your project's StreamingAssets directory, and that the Audio wave files are located at the proper subdirectory. The Plug-In will automatically verify these file paths when the AHAP is added to your project. To re-check, right-click on the AHAP in the project hierarchy and select "Reimport".

### 3. Create a `CHHapticPatternPlayer`
Now that you have an engine and a pattern, you can make a pattern player like this:
```C#
using Apple.CoreHaptics;

var hapticPlayer = eng.MakePlayer(myPattern);
```

A single haptic engine can be used to create many pattern players. However, at least one pattern player is needed per `CHHapticPattern`.

### 4. Start the Engine and the Player
Now that you have an engine, a pattern, and a player to play the pattern, your app is ready for haptic playback! Trigger the haptics like this:
```C#
using Apple.CoreHaptics;

// Start the engine when your app needs to be ready to play haptics
engine.Start();

// Play the haptics when desired, perhaps in an OnCollisionEnter()
hapticPlayer.Play();

// Stop the engine when haptics are no longer expected to save power
engine.Stop();
```

### 5. More on Haptic Patterns
Haptic patterns are made up of 3 types of objects: `CHHapticEvent`, `CHHapticParameter`, and `CHHapticParameterCurve`.

#### `CHHapticEvent`

There are 4 types of `CHHapticEvents`: `CHHapticTransientEvent`, `CHHapticContinuousEvent`, `CHAudioContinuousEvent`, and `CHAudioCustomEvent`. Each has its own characteristics.

| Event | Type | Description |
|:----------:|:-----------:|:-----------:|
|`CHHapticTransientEvent`| Haptic | A brief impulse occurring at a specific point in time.|
|`CHHapticContinuousEvent`| Haptic | A haptic event with a looped waveform of arbitrary length.|
|`CHHapticAudioContinuous`| Audio | An audio event with a looped waveform of arbitrary length.|
|`CHHapticAudioCustom`| Audio | An audio event using a waveform that you supply.|

Also note that the character of each event can be further shaped with the use of `CHHapticEventParameters`. For haptic events, the parameter types are `HapticIntensity` and `HapticSharpness`. For audio events, the types are `AudioVolume`, `AudioPan`, `AudioPitch`, and `AudioBrightness`. Additionally, parameters that shape an event's ramp-in and ramp-out profiles can be specified for all events except `CHHapticTransientEvent`. These parameter types are `AttackTime`, `DecayTime`, `ReleaseTime`, and `Sustained`. For more details on each parameter type and how it affects the character of an event, please see [CHHapticEvent.ParameterID](https://developer.apple.com/documentation/corehaptics/chhapticevent/parameterid) and the related topics.

Here is an example of using event parameters to modify some events:
```C#
using Apple.CoreHaptics;

var transientEvent = new CHHapticTransientEvent
{
    Time = 0.5f,
    EventParameters = new List<CHHapticEventParameter>
    {
        new CHHapticEventParameter(CHHapticEventParameterID.HapticIntensity, 1f),
        new CHHapticEventParmaeter(CHHapticEventParameterID.HapticSharpness, 0f)
    }
};

var audioContinuousEvent = new CHHapticAudioContinuousEvent
{
    Time = 0f,
    EventDuration = 2f,
    EventParameters = new List<CHHapticEventParameter>
    {
        new CHHapticEventParameter(CHHapticEventParameterID.AudioVolume, 0.5f),
        new CHHapticEventParameter(CHHapticEventParameterID.AudioPitch, 1f)
    }
};
```

#### `CHHapticParameter`
A `CHHapticParameter` shapes the character of `CHHapticEvents` similar to how `CHHapticEventParameters` do, except that it applies to all `CHHapticEvents` in the pattern from the specified time onward (or until a new Parameter of the same type is specified). For example, specifying a `HapticIntensity` value of 0.5 at time 0 would reduce all haptic events' intensities in the pattern by half. This parameter would be created like this:
```C#
using Apple.CoreHaptics;

var intensityParam = new CHHapticParameter(CHHapticDynamicParameterID.HapticIntensityControl, 0.5f);
```
For more details, see [CHHapticDynamicParameter](https://developer.apple.com/documentation/corehaptics/chhapticdynamicparameter).

#### `CHHapticParameterCurve`
A `CHHapticParameterCurve` behaves much like a `CHHapticParameter`, except that value changes over time according to the array of `CHHapticParameterCurveControlPoints`. For example, if you wanted to modify a pattern's audio events such that the volume ramped up from 0 and then back down again, you might create a `CHHapticParameterCurve` like this:
```C#
using Apple.CoreHaptics;

var paramCurve = new CHHapticParameterCurve{
    ParameterID = CHHapticDynamicParameterID.AudioVolumeControl,
    ParameterCurveControlPoints = new List<CHHapticParameterCurveControlPoint> {
        new CHHapticParameterCurveControlPoint(0f, 0f),     // At time 0, volume 0
        new CHHapticParameterCurveControlPoint(0.5f, 1f),   // At time 0.5, full volume
        new CHHapticParameterCurveControlPoint(1f, 0f)      // At time 1, back to volume 0
    }
};
```

### 6. Advanced Features
Core Haptics enables a wide variety of advanced haptics interactions such as looping a pattern, receiving notifications when playback finishes, modifying content in real time, and advanced engine properties. Read on for some examples.

#### Looping
To loop a pattern, you will need to use a `CHHapticAdvancedPatternPlayer` instead of a `CHHapticPatternPlayer`. The advanced player should only be used when you need these additional features. Here's how you would create and play an advanced player with looping:
```C#
using Apple.CoreHaptics;

// As always, we need an engine
var eng = CHHapticEngine();

// Start it now for convenience
eng.Start();

// Let's use a few CHHapticTransientEvents with a parameter curve to shape their intensity from low to high and back to low.
var events = new List<CHHapticEvent>();
for (float i = 0; i < 1f; i += 0.1f)
{
    events.Add(new CHHapticTransientEvent { Time = i });
}
var pattern = new CHHapticPattern(
    events,
    new List<CHHapticParameterCurve>
    {
        new CHHapticParameterCurve
        {
            ParameterID = CHHapticDynamicParameterID.HapticIntensityControl,
            ParameterCurveControlPoints = new List<CHHapticParameterCurveControlPoint> {
                new CHHapticParameterCurveControlPoint(0f, 0.25f),     // Time 0, 1/4 intensity
                new CHHapticParameterCurveControlPoint(0.5f, 1f),   // Time 0.5, full intensity
                new CHHapticParameterCurveControlPoint(1f, 0.25f)      // Time 1, back to 1/4 intensity
            }
        }
    }
);

// Create the advanced player
var advPlayer = eng.MakeAdvancedPlayer(pattern);

// Enable Looping and set the loop endpoint
advPlayer.LoopEnabled = true;
advPlayer.LoopEnd = 1f;

// Play the player
advPlayer.Play();

// Eventually stop the player
advPlayer.Stop();
```

#### Notification when playback ends
`CHHapticAdvancedPatternPlayer`s can also provide a notification when they finish playback, which could be used, for example, to coordinate ending an associated animation. This event can be subscribed to like this:

```C#
using Apple.CoreHaptics;

private CHHapticEngine _eng;
private CHHapticAdvancedPatternPlayer _advPlayer;

void HapticCompletionHandler()
{
    Debug.Log("Advanced haptic pattern completed.");
    // Take any action here
}

void SetupHaptics()
{
    _eng = new CHHapticEngine();
    _eng.Start();
    
    var ahap = Resources.Load<TextAsset>("MyAhap");
    _advPlayer = _eng.MakeAdvancedPlayer(ahap);
    
    // When the advanced player's pattern finishes playing, it will call any methods added to its `CompletionHandler`
    _advPlayer.CompletionHandler += HapticCompletionHandler;
    
}

void OnCollisionEnter2D(Collision2D collision) {
    _advPlayer.Play();
}
```

#### Modifying Playback in Real-Time
Although a `CHHapticPatternPlayer`'s pattern cannot be changed once created, it is possible to shape its playback in real time. For example, imagine an object that plays a haptic pattern when it appears. Perhaps the Intensity of the pattern should be weaker if the user's character is farther away from it. You could create a pattern and player with a prearranged pattern, but modify its `HapticIntensityControl` using a `CHHapticDynamicParameter` like this:
```C#
using Apple.CoreHaptics;

private CHHapticEngine _eng;
private CHHapticAdvancedPatternPlayer _advPlayer;

void SetupHaptics()
{
    _eng = new CHHApticEngine();
    _eng.Start();
    
    var ahap = Resources.Load<TextAsset>("MyAhap");
    _advPlayer = _eng.MakeAdvancedPlayer(ahap);
}

// Provide a normalized distance to make intensity in the range [0, 1]
void SpecialObjectSpawned(float normalizedDistance)
{
    _advPlayer.Play();
    _advPlayer.SendParameters(new List<CHHapticParameter> {
        new CHHapticParameter(
            CHHapticDynamicParameterID.HapticIntensityControl,
            normalizedDistance
        )
    });
}
```

#### Advanced Engine Properties
`CHHapticEngine` has some additional properties that might be useful to control haptic playback.
- `PlaysHapticsOnly`: Setting this property to true causes the engine to ignore all audio events, such as `CHHapticAudioContinuous` and `CHHapticAudioCustom`. This also reduces latency of starting haptic playback. _Important_: Changing the value of this property on a running engine has no effect until you stop and restart the engine.
- `IsMutedForAudio`: A Boolean that indicates whether the engine mutes audio. This can be toggled while an engine is running, even during playback.
- `IsMutedForHaptics`: A Boolean that indicates whether the engine mutes haptics. Like its audio counterpart, it can be toggled during playback.
- `IsAutoShutdownEnabled`: A Boolean value that indicates whether the haptic engine starts and stops automatically on request from one of its pattern players, or when idle. When this property is enabled, the engine shuts down after approximately two minutes of inactivity. If you manually manage the engine’s life cycle (`IsAutoShutdownEnabled = false`), save power by stopping the engine when it’s not in use with `eng.Stop()`.

## UIFeedbackGenerator

### 1. UIFeedbackGenerator Usage

#### Create the Generator

Create any of the three generator types like this:
```C#
using Apple.UIFBG;

var impactGenerator = new UIImpactFeedbackGenerator(UIImpactFeedbackGenerator.UIImpactType.Heavy);
var selectionGenerator = new UISelectionFeedbackGenerator();
var notificationGenerator = new UINotificationFeedbackGenerator();
```

The `UIImpactFeedbackGenerator` can be created with five different impact types: `Heavy`, `Light`, `Medium`, `Rigid`, and `Soft`, all members of the `UIImpactFeedbackGenerator.UIImpactType` `enum`.

#### Optionally Prepare the Generator
Once created, each generator can optionally be prepared for lower-latency playback by calling it's `Prepare()` method. Continuing the example from above:

```C#
selectionGenerator.Prepare();
```

#### Trigger the Generator

Finally, to trigger the generator, call its `Trigger()` method. For the selection generator, `Trigger()` takes no arguments. The impact generator's `Trigger()` method takes an optional `intensity` parameter which scales the output's strength from 0 to 1.0. The notification generator's `Trigger()` requires a parameter specifiying the type of notification, one of `Error`, `Success`, or `Warning` from the `UINotificationFeedbackGenerator.UINotficationStyle` `enum`. Again, continuing from our above example:

```C#

selectionGenerator.Trigger();

impactGenerator.Trigger();
// OR for 3/4 strength output
impactGenerator.Trigger(0.75f);

notificationGenerator.Trigger(UINotificationFeedbackGenerator.UINotificationStyle.Success);
// OR
notificationGenerator.Trigger(UINotificationFeedbackGenerator.UINotificationStyle.Error);
```
