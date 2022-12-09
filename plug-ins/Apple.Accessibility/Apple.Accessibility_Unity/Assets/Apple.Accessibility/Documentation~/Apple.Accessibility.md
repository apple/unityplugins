# Apple - Accessibility

## Installation Instructions

### 1. Install Dependencies
* Apple.Core

### 2. Install the Package
See the [Quick-Start Guide](../../../../../../Documentation/Quickstart.md) for general installation instructions.

## Usage
Please find an introduction to using Apple's Accessibility Plug-in below. For a comprehensive guide to Accessibility on Apple devices, please see [Accessibility Developer Documentation](https://developer.apple.com/documentation/accessibility/)

The plugin supports Unity 2020.3 and above, but should work with older versions of Unity.

### Table of Contents
[Accessibility Elements](#Accessibility-Elements)

* [1. Add AccessibilityNode](#1-Add-AccessibilityNode)
* [2. Add Accessibility Traits and Labels](#2-Add-Accessibility-Traits-and-Labels)
* [3. Assigning Accessibility Properties](#3-Assigning-Accessibility-Properties)
* [4. Test with VoiceOver](#4-Test-with-VoiceOver)
* [5. Advanced Topics](#5-Advanced-Topics)

[Dynamic Type](#Dynamic-Type)

* [1. Check Settings](#1-Check-Settings)
* [2. Listening for Changes](#2-Listening-for-Changes)

[UI Accommodations](#UI-Accommodations)

* [1. Check Settings](#1-Check-Settings)
* [2. Listening for Changes](#2-Listening-for-Changes)

## Accessibility Elements

### 1. Add AccessibilityNode
To enable Apple's Accessibility system to work with a `GameObject`, the object requires an `AccessibilityNode` component.

Optionally configure the `AccessibilityNode` to be a model view container. (see [Advanced Features](#6-Advanced-Features))

### 2. Add Accessibility Traits and Labels
Select the traits that best describe the node's behavior.

The most common trait is `AccessibilityTrait.Button`, which informs assistive technologies users that this is a tappable button. Note that `AccessibilityTrait` is a bitmask, so it is possible for a node to have multiple traits. Please refer to [UIAccessibilityTraits Apple Developer docs](https://developer.apple.com/documentation/uikit/uiaccessibility/uiaccessibilitytraits) for more detailed information on accessibility traits.

Next, assign a `Label` to `AccessibilityNode` to add a localized description of this accessibility node. For example, a back button might have "Back" as its `Label`, while a background music settings slider might have "Background Music" instead. For choosing the best label for assistive technologies, please refer to [Writing Great Accessibility Labels Apple WWDC talk](https://developer.apple.com/videos/play/wwdc2019/254/).

### 3. Assigning Accessibility Properties
Accessibility properties can be assigned programmatically:

```C#
using Apple.Accessibility;

var node = GetComponent<AccessibilityNode>();
node.AccessibilityLabel = MyHelper.LocalizedString("Back");
```

In addition, you could assign a callback for accessibility properties that changes frequently:

```C#
var healthNode = GetComponent<AccessibilityNode>();
healthNode.accessibilityValueDelegate = () =>
{
    return this.currentHealth.ToString();
};
```
Note: a delegate will take precedence over other assignment methods.

### 4. Test with VoiceOver
After building and installing a game to an Apple device, test the Accessibility features using VoiceOver. VoiceOver can be turned on or off under `Settings > Accessibility > VoiceOver`. (Note that device-navigation changes significantly when VoiceOver is turned on. See [Turn on and practice VoiceOver on iPhone Apple Support docs](https://support.apple.com/guide/iphone/turn-on-and-practice-voiceover-iph3e2e415f/ios) for more information on how to navigate a device with VoiceOver on)

VoiceOver can also be switched on or off with one of the following methods:

1. Triple-click the side button (on an iPhone with Face ID).
2. Triple-click the Home button (on an iPhone with a Home button).
3. Activate Siri and say “Turn on VoiceOver” or “Turn off VoiceOver.”

Now that VoiceOver is on, tapping on buttons and game objects that have `AccessibilityNode` components should result in the VoiceOver system reading out descriptions, which means assistive technologies users will be able to properly interact with them.

### 5. Advanced Topics
`AccessibilityNode` follows a tree structure model that helps encode the accessibility hierarchy. This means that an `AccessibilityNode` can be added to non-interactive HUD containers like a `UIPanel`. By setting `isModal` to `true`, either programmatically or in the Editor, the `UIPanel` element indicates whether VoiceOver should ignore the accessibility elements within views that are siblings of the element.

Please refer to Apple Support docs [accessibilityViewIsModal](https://developer.apple.com/documentation/objectivec/nsobject/1615089-accessibilityviewismodal?language=objc)

## Dynamic Type

### 1. Check Settings
Dynamic Type is a system-wide feature which allows users to scale font size. Try this out for yourself by going to `Settings > Display & Brightness > Text Size`. To incorporate a user's text size preferences into a Unity app, use the text size multiplier from the `AccessibilitySettings` script:

```C#
var multiplier = AccessibilitySettings.PreferredContentSizeMultiplier;
```

Scale text elements using the multiplier:

```C#
GetComponent<Text>().textSize = multiplier * 48;
```

Or read the user's preferred size category in place of the multiplier:

```C#
ContentSizeCategory category = AccessibilitySettings.PreferredContentSizeCategory;
```

### 2. Listening for Changes
Sometimes an app might need to change its behavior when a user changes their accessibility settings.

For example, to subscribe to a user changing their text size preference and respond accordingly, use:

```C#
void Start()
{
    AccessibilitySettings.onPreferredContentSizeChanged.AddListener(OnContentSizeChanged);
}

private void OnContentSizeChanged()
{
    // refresh text sizes
}
```

## UI Accommodations

### 1. Check Settings
Access to user accessibility settings can be found with the AccessibilitySettings API. For example, to check whether the user has turned on the Reduce Motion setting in `Settings > Accessibility > Motion`:

```C#
var isReduceMotionOn = AccessibilitySettings.IsReduceMotionEnabled;
```

Check `AccessibilitySettings.cs` for a complete list of supported Accessibility Settings. Please refer to [Accessibility for UIKit Apple Support docs](https://developer.apple.com/documentation/uikit/accessibility_for_uikit?language=objc) for more details on each setting.

### 2. Listening for Changes
Sometimes an app might need to change its behavior when a user changes his or her accessibility settings.

For example, if a user changes the "Switch Control" setting, you could subscribe to the event by:

```C#
void Start()
{
    AccessibilitySettings.onIsSwitchControlRunningChanged.AddListener(SwitchControlSettingChanged);
}

private void SwitchControlSettingChanged()
{
    ...
}
```
