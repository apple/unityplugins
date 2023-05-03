using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AOT;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace Apple.CloudKit.Sample
{
	public class CloudKitSample : MonoBehaviour {

		[SerializeField]
		private Toggle _boolToggle;

		[SerializeField]
		private Button _getBoolValueButton;

		[SerializeField]
		private Text _boolValueDisplay;

		[SerializeField]
		private Slider _doubleSlider;

		[SerializeField]
		private Button _getDoubleValueButton;

		[SerializeField]
		private Text _doubleValueDisplay;

		[SerializeField]
		private Slider _int64Slider;

		[SerializeField]
		private Button _getInt64ValueButton;

		[SerializeField]
		private Text _int64ValueDisplay;

		[SerializeField]
		private TMP_InputField _stringInputField;

		[SerializeField]
		private Button _setStringValueButton;

		[SerializeField]
		private Button _getStringValueButton;

		[SerializeField]
		private Text _stringValueDisplay;

		[SerializeField]
		private Button _removeAllButton;

		[SerializeField]
		private Button _synchronizeButton;

		private void OnEnable() {
			NSUbiquitousKeyValueStore.UbiquitousKeyValueStoreDidChangeExternally += NSUbiquitousKeyValueStore_UbiquitousKeyValueStoreDidChangeExternally;
			NSUbiquitousKeyValueStore.AddObserverForExternalChanges();
		}

		private void NSUbiquitousKeyValueStore_UbiquitousKeyValueStoreDidChangeExternally(NSUbiquitousKeyValueStoreChangeReasonKey changeReason, IEnumerable<string> changedKeys) {
			Debug.Log($"UbiquitousKeyValueStoreDidChangeExternally: Reason {changeReason}, Keys: {string.Join(',', changedKeys)}");
		}

		private void OnDisable() {
			NSUbiquitousKeyValueStore.RemoveObserverForExternalChanges();
		}

		private void Start() {
			OnDemandRendering.renderFrameInterval = 10;
			LoadInitialValues();
			SetupEventHandlers();
		}

		private void LoadInitialValues() {
			OnGetBoolValueButtonClicked();
			OnGetDoubleValueButtonClicked();
			OnGetInt64ValueButtonClicked();
			OnGetStringValueButtonClicked();
		}

		private void SetupEventHandlers() {
			_boolToggle.onValueChanged.AddListener(OnBoolToggleValueChanged);
			_getBoolValueButton.onClick.AddListener(OnGetBoolValueButtonClicked);
			_doubleSlider.onValueChanged.AddListener(OnDoubleToggleValueChanged);
			_getDoubleValueButton.onClick.AddListener(OnGetDoubleValueButtonClicked);
			_int64Slider.onValueChanged.AddListener(OnInt64ToggleValueChanged);
			_getInt64ValueButton.onClick.AddListener(OnGetInt64ValueButtonClicked);
			_setStringValueButton.onClick.AddListener(OnSetStringValueButtonClicked);
			_getStringValueButton.onClick.AddListener(OnGetStringValueButtonClicked);
			_removeAllButton.onClick.AddListener(RemoveAllButtonClicked);
			_synchronizeButton.onClick.AddListener(SynchronizeButtonClicked);
		}

		private void OnBoolToggleValueChanged(bool value) {
			NSUbiquitousKeyValueStore.SetBool(value, "BoolKey");
		}

		private void OnGetBoolValueButtonClicked() {
			var value = NSUbiquitousKeyValueStore.GetBool("BoolKey");
			_boolValueDisplay.text = $"{value}";
		}

		private void OnDoubleToggleValueChanged(float value) {
			NSUbiquitousKeyValueStore.SetDouble(value, "DoubleKey");
		}

		private void OnGetDoubleValueButtonClicked() {
			var value = NSUbiquitousKeyValueStore.GetDouble("DoubleKey");
			_doubleValueDisplay.text = $"{value}";
		}

		private void OnInt64ToggleValueChanged(float value) {
			NSUbiquitousKeyValueStore.SetLong((long)value, "LongKey");
		}

		private void OnGetInt64ValueButtonClicked() {
			var value = NSUbiquitousKeyValueStore.GetLong("LongKey");
			_int64ValueDisplay.text = $"{value}";
		}

		private void OnSetStringValueButtonClicked() {
			NSUbiquitousKeyValueStore.SetString(_stringInputField.text, "StringKey");
		}

		private void OnGetStringValueButtonClicked() {
			_stringValueDisplay.text = NSUbiquitousKeyValueStore.GetString("StringKey");
		}

		private void RemoveAllButtonClicked() {
			NSUbiquitousKeyValueStore.RemoveObject("BoolKey");
			NSUbiquitousKeyValueStore.RemoveObject("DoubleKey");
			NSUbiquitousKeyValueStore.RemoveObject("LongKey");
			NSUbiquitousKeyValueStore.RemoveObject("StringKey");
		}

		private void SynchronizeButtonClicked() {
			var synchronized = NSUbiquitousKeyValueStore.Synchronize();
		}
	}
}
