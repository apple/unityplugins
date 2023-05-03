# Apple - CloudKit

## Installation Instructions

### 1. Install Dependencies
* Apple.Core

### 2. Install the Package
See the [Quick-Start Guide](../../../../../../Documentation/Quickstart.md) for general installation instructions.

## Usage
For more information on iCloud key-value store please see [NSUbiquitousKeyValueStore](https://developer.apple.com/documentation/foundation/nsubiquitouskeyvaluestore). For a comprehensive guide to CloudKit on Apple devices, please see [CloudKit Developer Documentation](https://developer.apple.com/documentation/cloudkit/)

## Table of Contents
1. [NSUbiquitousKeyValueStore](#1-nsubiquitouskeyvaluestore)

### 1. NSUbiquitousKeyValueStore
##### [NSUbiquitousKeyValueStore - Apple Developer Documentation](https://developer.apple.com/documentation/foundation/nsubiquitouskeyvaluestore)

#### 1.1 Getting and Setting Values
##### Bool
```csharp
NSUbiquitousKeyValueStore.SetBool(true, "BoolKey");
bool b = NSUbiquitousKeyValueStore.GetBool("BoolKey");
```

##### Double
```csharp
NSUbiquitousKeyValueStore.SetDouble(3.14d, "DoubleKey");
double d = NSUbiquitousKeyValueStore.GetDouble("DoubleKey");
```

##### Int64
```csharp
NSUbiquitousKeyValueStore.SetLong(long.MaxValue, "LongKey");
long l = NSUbiquitousKeyValueStore.GetLong("LongKey");
```

##### String
```csharp
NSUbiquitousKeyValueStore.SetString("Hello CloudKit!", "StringKey");
string s = NSUbiquitousKeyValueStore.GetString("StringKey");
```

#### 1.2 Synchronize with iCloud
```csharp
var synchronized = NSUbiquitousKeyValueStore.Synchronize();
```

#### 1.3 Remove Values
```csharp
NSUbiquitousKeyValueStore.RemoveObject("BoolKey");
```

#### 1.4 Change Notifications
##### Observing External Changes
```csharp
public class CloudKitSample : MonoBehaviour {

	private void OnEnable() {
		NSUbiquitousKeyValueStore.UbiquitousKeyValueStoreDidChangeExternally += KeyValueStoreDidChangeExternally;
		NSUbiquitousKeyValueStore.AddObserverForExternalChanges();
	}
	
	private void KeyValueStoreDidChangeExternally(NSUbiquitousKeyValueStoreChangeReasonKey changeReason, IEnumerable<string> changedKeys) {
		Debug.Log($"KeyValueStoreDidChangeExternally: Reason {changeReason}, Keys: {string.Join(',', changedKeys)}");
	}

	private void OnDisable() {
		NSUbiquitousKeyValueStore.RemoveObserverForExternalChanges();
	}
}

```
##### Reasons for External Changes
```csharp
public enum NSUbiquitousKeyValueStoreChangeReasonKey : int {
	ServerChange = 1,
	InitialSyncChange = 2,
	QuotaViolationChange = 3,
	AccountChange = 4
}	
```
