using UnityEditor;
using UnityEngine;
using UnityPickers.Utility;

namespace UnityPickers
{
	[CustomPropertyDrawer(typeof(AssetPickerAttribute))]
	[CustomPropertyDrawer(typeof(ScriptableObject), true)]
	public class AssetPickerDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var assetType = fieldInfo.FieldType;
			if (assetType.IsUnityCollection())
				assetType = assetType.GetElementType();

			if (assetType == null)
				return;

			var a = fieldInfo.GetAttribute<AssetPickerAttribute>();

			AssetPicker.PropertyField(
				position, property, fieldInfo,
				label, assetType,
				he => a == null || he.Path.Contains(a.Path)
			);
		}

	}
}