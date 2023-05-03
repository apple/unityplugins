using System;
using AOT;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Apple.Core.Runtime;

namespace Apple.CloudKit {

	public enum NSUbiquitousKeyValueStoreChangeReasonKey : int {
		ServerChange = 1,
		InitialSyncChange = 2,
		QuotaViolationChange = 3,
		AccountChange = 4
	}	

	public static class NSUbiquitousKeyValueStore {

		private delegate void InteropUbiquitousKeyValueStoreDidChangeExternallyHandler(NSUbiquitousKeyValueStoreChangeReasonKey changeReason, IntPtr changedKeysArray);
		public delegate void UbiquitousKeyValueStoreDidChangeExternallyHandler(NSUbiquitousKeyValueStoreChangeReasonKey changeReason, IEnumerable<string> changedKeys);

		public static event UbiquitousKeyValueStoreDidChangeExternallyHandler UbiquitousKeyValueStoreDidChangeExternally;

		#region External Changes

		[DllImport(InteropUtility.DLLName)]
		private static extern void NSUbiquitousKeyValueStore_AddObserverForDidChangeExternallyNotification(InteropUbiquitousKeyValueStoreDidChangeExternallyHandler callback);

		[DllImport(InteropUtility.DLLName)]
		private static extern void NSUbiquitousKeyValueStore_RemoveObserverForDidChangeExternallyNotification();

		[MonoPInvokeCallback(typeof(InteropUbiquitousKeyValueStoreDidChangeExternallyHandler))]
		private static void OnUbiquitousKeyValueStoreDidChangeExternally(NSUbiquitousKeyValueStoreChangeReasonKey changeReason, IntPtr changedKeysArrayPointer) {
			using var changedKeys = new NSArrayString(changedKeysArrayPointer);
			UbiquitousKeyValueStoreDidChangeExternally?.Invoke(changeReason, changedKeys);
		}

		public static void AddObserverForExternalChanges() {
			NSUbiquitousKeyValueStore_AddObserverForDidChangeExternallyNotification(OnUbiquitousKeyValueStoreDidChangeExternally);
		}

		public static void RemoveObserverForExternalChanges() {
			NSUbiquitousKeyValueStore_RemoveObserverForDidChangeExternallyNotification();
		}

		#endregion

		#region Object Removal

		[DllImport(InteropUtility.DLLName)]
		private static extern void NSUbiquitousKeyValueStore_RemoveObject(string key);

		public static void RemoveObject(string key) {
			NSUbiquitousKeyValueStore_RemoveObject(key);
		}

		#endregion

		#region Synchronization

		[DllImport(InteropUtility.DLLName)]
		private static extern bool NSUbiquitousKeyValueStore_Synchronize();

		public static bool Synchronize() {
			return NSUbiquitousKeyValueStore_Synchronize();
		}

		#endregion

		#region Strings

		[DllImport(InteropUtility.DLLName)]
		private static extern string NSUbiquitousKeyValueStore_GetString(string key);

		[DllImport(InteropUtility.DLLName)]
		private static extern void NSUbiquitousKeyValueStore_SetString(string value, string key);

		public static string GetString(string key) {
			return NSUbiquitousKeyValueStore_GetString(key);
		}

		public static void SetString(string value, string key) {
			NSUbiquitousKeyValueStore_SetString(value, key);
		}

		#endregion

		#region Arrays

		//[DllImport(InteropUtility.DLLName)]
		//private static extern IntPtr NSUbiquitousKeyValueStore_GetArray(string key);

		//[DllImport(InteropUtility.DLLName)]
		//private static extern void NSUbiquitousKeyValueStore_SetArray(IntPtr value, string key);

		//public static NSArray GetArray(string key) {
		//	return new NSArray(NSUbiquitousKeyValueStore_GetArray(key));
		//}

		//public static void SetArray(NSArray value, string key) {
		//	NSUbiquitousKeyValueStore_SetArray(value, key);
		//}

		#endregion

		#region Dictionaries

		//[DllImport(InteropUtility.DLLName)]
		//private static extern Dictionary<string, object> NSUbiquitousKeyValueStore_GetDictionary(string key);

		//[DllImport(InteropUtility.DLLName)]
		//private static extern void NSUbiquitousKeyValueStore_SetDictionary(Dictionary<string, object> value, string key);

		//public static Dictionary<string, object> GetDictionary(string key) {
		//	return NSUbiquitousKeyValueStore_GetDictionary(key);
		//}

		//public static void SetDictionary(Dictionary<string, object> value, string key) {
		//	NSUbiquitousKeyValueStore_SetDictionary(value, key);
		//}

		#endregion

		#region Raw Data

		[DllImport(InteropUtility.DLLName)]
		private static extern InteropData NSUbiquitousKeyValueStore_GetData(string key);

		[DllImport(InteropUtility.DLLName)]
		private static extern void NSUbiquitousKeyValueStore_SetData(IntPtr data, int dataLength, string key);

		public static byte[] GetData(string key) {
			return NSUbiquitousKeyValueStore_GetData(key).ToBytes();
		}

		public static void SetData(byte[] data, string key) {
			var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
			NSUbiquitousKeyValueStore_SetData(handle.AddrOfPinnedObject(), data.Length, key);
		}

		#endregion

		#region Int64

		[DllImport(InteropUtility.DLLName)]
		private static extern Int64 NSUbiquitousKeyValueStore_GetInt64(string key);

		[DllImport(InteropUtility.DLLName)]
		private static extern void NSUbiquitousKeyValueStore_SetInt64(Int64 value, string key);

		public static Int64 GetLong(string key) {
			return NSUbiquitousKeyValueStore_GetInt64(key);
		}

		public static void SetLong(Int64 value, string key) {
			NSUbiquitousKeyValueStore_SetInt64(value, key);
		}

		#endregion

		#region Doubles

		[DllImport(InteropUtility.DLLName)]
		private static extern double NSUbiquitousKeyValueStore_GetDouble(string key);

		[DllImport(InteropUtility.DLLName)]
		private static extern void NSUbiquitousKeyValueStore_SetDouble(double value, string key);

		public static double GetDouble(string key) {
			return NSUbiquitousKeyValueStore_GetDouble(key);
		}

		public static void SetDouble(double value, string key) {
			NSUbiquitousKeyValueStore_SetDouble(value, key);
		}

		#endregion

		#region Bools

		[DllImport(InteropUtility.DLLName)]
		private static extern bool NSUbiquitousKeyValueStore_GetBool(string key);

		[DllImport(InteropUtility.DLLName)]
		private static extern void NSUbiquitousKeyValueStore_SetBool(bool value, string key);

		public static bool GetBool(string key) {
			return NSUbiquitousKeyValueStore_GetBool(key);
		}

		public static void SetBool(bool value, string key) {
			NSUbiquitousKeyValueStore_SetBool(value, key);
		}

		#endregion
	}

}
