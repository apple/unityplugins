# Apple - Game Controller

## Installation Instructions

### 1. Install Dependencies
* [Apple.Core](../../../../../Apple.Core/Apple.Core_Unity/Assets/Apple.Core/Documentation~/Apple.Core.md)

### 2. Install the Package
See the [Quick-Start Guide](../../../../../../Documentation/Quickstart.md) for general installation instructions.

## Usage
Please find an introduction to using the Game Controller plug-in below. For a comprehensive guide to Apple's Game Controller framework, please see the [Game Controller Developer Documentation](https://developer.apple.com/documentation/gamecontroller)

### Table of Contents
[Game Controller](#Game-Controller)

* [1. Initialize the Game Controller Service](#1-Initialize-the-Game-Controller-Service)
* [2. Controller Connection Handlers](#2-Controller-Connection-Handlers)
* [3. Wireless Discovery](#3-Wireless-Discovery)
* [4. Get Connected Controllers](#4-Get-Connected-Controllers)
* [5. Polling Controllers](#5-Polling-Controllers)
    * [5.1 What is Polling?](#5.1-What-is-Polling?)
    * [5.2 Poll All Connected Controllers](#5.2-Poll-All-Connected-Controllers)
    * [5.3 Poll Individual Controllers](#5.3-Poll-Individual-Controllers)
* [6. Query Controller Input State](#6-Query-Controller-Input-State)
    * [6.1 GetInputValue](#6.1-GetInputValue)
    * [6.2 GetButton](#6.2-GetButton)
    * [6.3 GetButtonDown & GetButtonUp](#6.3-GetButtonDown-&-GetButtonUp)
* [7. Controller Event Callbacks](#7-Controller-Event-Callbacks)
* [8. Controller Meta Data](#8-Controller-Meta-Data)
* [9. Get SF Symbols](#9-Get-SF-Symbols)

## Game Controller

### 1. Initialize the Game Controller Service
```csharp
// Sets up the connection handlers and starts wireless discovery
await GCControllerService.Initialize();
```
### 2. Controller Connection Handlers
```csharp
GCControllerService.ControllerConnected += OnControllerConnected;
GCControllerService.ControllerDisconnected += OnControllerDisconnected;

private void OnControllerConnected(object sender, ControllerConnectedEventArgs args)
{
    // Do something with args.Controller...
}

private void OnControllerDisconnected(object sender, ControllerConnectedEventArgs args)
{
    // Do something with args.Controller...
}
```

### 3. Wireless Discovery
Browse for nearby controllers.

See [startWirelessControllerDiscovery](https://developer.apple.com/documentation/gamecontroller/gccontroller/1458879-startwirelesscontrollerdiscovery) for more information.

```csharp
await GCControllerService.StartWirelessDiscovery();
GCControllerService.StopWirelessDiscovery();
```

### 4. Get Connected Controllers
If you have a need to fetch the connected controllers during a loop, you can fetch connected controllers via GetConnectedControllers() however it is more efficient to utilize the controller connection handlers as described above.
```csharp
var controllers = GCControllerService.GetConnectedControllers();
```

### 5. Polling Controllers 
It is the developer responsibility to poll the controllers at the frequency of their choosing. You can choose to poll all connected controllers (even ones that are not primarily used by the game) or you can choose to poll the individual controller.

#### 5.1 What is Polling?
Polling is the act of asking the native plugin layer what the current button states are for the controller. Each state as returned by the polling is a "frame" of reference for what the current states are on the controller you are targeting. 

#### 5.2 Poll All Connected Controllers
You can poll all connected controllers through this method, however it is more efficient to poll specific controllers. 
```csharp
// Calls Poll() on each connected controller...
GCControllerService.PollAllControllers();
```

#### 5.3 Poll Individual Controllers
Calling `Poll()` directly on the controllers of interest prevents time spent polling inactive controllers.
```csharp
// Grabs the latest input state...
controller.Poll();
```
### 6. Query Controller Input State
After you have polled a controller, the associated `GCController` instance updates the internal state representation. You can query the values of the internal state representation through the following methods:

#### 6.1 GetInputValue
Returns the raw float value as reported natively for the specified `GCControllerInputName`.
```csharp
var value = controller.GetInputValue(GCControllerInputName.ButtonA);
```

#### 6.2 GetButton
Returns a `bool` where true indicates the selected button is depressed. You can also optionally pass a threshold value for use with analog buttons when present.
```csharp
if(controller.GetButton(GCControllerInputName.ButtonA))
{
    // Respond to button press
}
```

You can also pass an optional threshold to control how it responds.
```csharp
if(controller.GetButton(GCControllerInputName.ButtonA, 0.75f))
{
    // Respond to button press at threshold
}
```
#### 6.3 GetButtonDown & GetButtonUp
Similar to the Unity implementation of Input.GetButtonDown & Input.GetButtonUp.
```csharp
if(controller.GetButtonDown(GCControllerInputName.ButtonA))
{
    // Respond to button press
}

if(controller.GetButtonUp(GCControllerInputName.ButtonA)
{
    // Respond to button no longer pressed
}
```
### 7. Controller Event Callbacks
You can register individual connection state callbacks and polling callbacks for a controller.
```csharp
controller.Polled += OnControllerPolled;
// Both connection and disconnection invokes...
controller.ConnectedStateChanged += OnControllerConnectionStateChanged;
```
### 8. Controller Meta Data
You can access the native handle and meta information on a particular controller.
```csharp
// Is this a built-in controller to the device?
if(controller.Handle.IsAttachedToDevice)
{
    // Respond accordingly...
}

// Check the controller type
if(controller.Handle.GetControllerType() == GCControllerType.DualSense)
{
    // Access an Sony DualSense supported controller feature
}

// If the controller type is unknown.
if(controller.Handle.GetControllerType() == GCControllerType.Unknown)
{
    // You can query product category or vendor name
    if(controller.Handle.ProductCategory == "...")
    {

    }
}
```
### 9. Get SF Symbols
Each button on a controller is represented by a [GCControllerElement](https://developer.apple.com/documentation/gamecontroller/gccontrollerelement). 

Starting with iOS 14, tvOS 14, and macOS 11 we can request a [sfSymbolsName](https://developer.apple.com/documentation/gamecontroller/gccontrollerelement/3563999-sfsymbolsname) render of the glyph for a particular button. 

```csharp
// Returns a Texture2D for the symbol based on the device...
var symbolTexture = controller.GetSymbolForInputName(GCControllerInputName.ButtonA);

// You can specify the pointSize and weight as well...
var symbolTexture = controller.GetSymbolForInputName(GCControllerInputName.ButtonA, 48, GCControllerSymbolWeight.Thin);
```
