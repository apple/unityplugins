# Apple - Core Haptics

## Installation Instructions

### 1. Install Dependencies
* Apple.Core 0.0.1

### 2. Install the Package
Launch the Unity Package Manager found under `Windows/Package Manager`.

#### Installing from a tarball
Move the package tarball into your Unity project folder or a subdirectory, such as `/Packages`.
Under the Package Manager +▾ dropdown menu, select `Add package from tarball...` option.
Browse to the tarball, select it, and click open.

### 3. Running the Demo App
1. From the PackageManager, select `Apple.CoreHaptics` and add Samples/Bounce to your project.
2. Open the scene "HapticBounce", then open your build settings and select "Add Open Scenes".
3. Close the Build Settings windw
4. Under `Demos/Bounce/HapticPatterns`, find the audio file called "RubberBounce". Move this to a new directory at `Assets/StreamingAssets/CoreHaptics/Bounce/`
5. Click the "CHBuildProfile" object, and find it's Inspector. Click the "Build" button and follow the prompts to get to an Xcode project.
6. Run the Xcode project on an iPhone to feel haptics as a ball bounces around the screen!

## Usage

### 1. Playing Basic Haptics
For this example, we'll use Apple's AHAP file format, a json-based format for storing and sharing haptic content. You should find a few examples available in the include Sample and / or on Box.
```csharp
using Apple.CoreHaptics {

    // Set up a HapticEngine and start it running
    CHHapticEngine engine = new CHHapticEngine();
    engine.Start();

    // Create a pattern or use an AHAP -- let's say you have an AHAP in your project at ".../Resources/MyAhap.ahap"
    TextAsset ahap = Resources.Load<TextAsset>("MyAhap");

    // Create a PatternPlayer or AdvancedPatternPlayer
    CHHapticPatternPlayer hapticPlayer = engine.MakePlayer(ahap);

    // When appropriate, tell your pattern to play
    hapticPlayer.Play();

    // Once you're done playing haptics, it's always a good idea to stop your HapticEngine and conserve power
    engine.Stop();
}
```
### 2. Programmatic Pattern Creation
You can also make a pattern on the fly.
Patterns consist of an array of Events, Parameters, and ParameterCurves.
Events come in 4 types: `CHHapticTransientEvent`, `CHHapticContinuousEvent`, `CHHapticAudioCustomEvent`, `CHHapticAudioContinuousEvent`.
Events can have `EventParameters` to modify their haptic or audio playback qualities, such as `HapticIntensity` or `AudioVolume`.
An obvious application for these would be a response to the velocity / force of a particular collision. Let's look at an example:

```csharp
using Apple.CoreHaptics{
    // Assumes that a locally-accessible CHHapticEngine has been created and started.

    private void PlayCollisionHaptic(float collisionVelocity)
    {
        CHHapticTransientEvent hapticEvent = new CHHapticTransientEvent
        {
            Time = 0f,
            EventParameters = new List<CHHapticEventParameters> {
                // HapticIntensity is on a 0.0 to 1.0 scale, so normalize by some factor
                new CHHapticEventParameter(CHHapticEventParameterID.HapticIntensity, collisionVelocity / MAX_VELOCITY);
            }
        }

        CHHapticAudioCustomEvent audioEvent = new CHHapticAudioCustomEvent
        {
            Time = 0f,
            EventWaveformPath = Path.Combine(Application.streamingAssetsPath, "CollisionSound.wav"),
            EventParameters = new List<CHHapticEventParameters> {
                // AudioVolume is on a 0.0 to 1.0 scale, so normalize by some factor
                new CHHapticEventParameter(CHHapticEventParameterID.AudioVolume, collisionVelocity / MAX_VELOCITY);
            }
        }

        // A player's pattern cannot be modified once it's been created,
        // so you'll need a new one each time you want to use a different pattern
        CHHapticPatternPlayer player = _engine.MakePlayer(new List<CHHapticEvent>{hapticEvent, audioEvent});
        player.Play();
    }
}
```

### 3. AdvancedPatternPlayer Functionality
`CHHapticAdvancedPatternPlayer`s have additional functionality, such as looping, modified playback rate, pause/resume, and completion notification
```csharp
[SerializeField]
TextAsset _ahap;

CHHapticEngine _eng = new CHHapticEngine();
CHHapticAdvancedPatternPlayer player = _eng.MakeAdvancedPlayer(_ahap);
player.LoopEnabled = true;
player.LoopEnd = 2.0f; // Any positive value of your choice!
player.PlaybackRate = 2.0f; // Play back at double speed

_eng.Start();
player.Play();
player.Pause(_eng.CurrentTime + 1); // Pause 1 second in the future
// ... some time or frames pass
player.Resume(); // Resume immediately

private void PlayerHasCompleted()
{
    Debug.Log("Maybe you could end an animation once haptics are over..?");
}

player.FinishedPlaying += PlayerHasCompleted
player.LoopEnabled = false; // Disable looping so we can test the above completion handler behavior
```

### 4. Direct-playback mode
The haptic engine is capable of playing back haptics without any player involvement if you need a quick and dirty fire-and-forget it mode:
```csharp
[SerializeField]
TextAsset _ahap;

CHHapticEngine _eng = new CHHapticEngine();
_eng.Start();

_eng.PlayPatternFromCHAHAP(_ahap);
// or similarly
_eng.PlayPatternFromJson(_ahap.text);
```

### 5. Advanced Engine Management Features
The haptic engine can be created in two modes: haptics & audio or haptics-only. In Haptics-only mode, latency is slightly lower, but no audio can be played. The engine must be started AFTER configuring this property (or stopped and restarted if this property is changed).
```csharp
CHHapticEngine eng = new CHHapticEngine();
eng.PlaysHapticsOnly = true;
eng.Start();
```

The engine also features two mute switches: one for audio, and one for haptics. Assuming you've created a haptics & audio engine (default behavior) then you can selectively mute one or the other output modes. These properties do not require engine restart to take effect:
```csharp
CHHapticEngine eng = new CHHapticEngine();
eng.IsMutedForAudio = true;
eng.IsMutedForHaptics = true;
```

Finally, the haptic engine can also be configured to start automatically when needed for haptic playback, and stop itself some time in the future once it has remained idle.
You should note that leaving an engine running unnecessarily is bad for power usage. However, starting the engine for every instance of haptic playback is slightly higher latency. It's a balancing act.
```csharp
[SerializeField]
TextAsset _ahap;

CHHapticEngine eng = new CHHapticEngine();
eng.IsAutoShutdownEnabled = true;

CHHapticPatternPlayer hapticPlayer = eng.MakePlayer(_ahap);

// Later, when you need to play haptics...

// We can skip this call because of the above
// eng.Start();

hapticPlayer.Play();
```
