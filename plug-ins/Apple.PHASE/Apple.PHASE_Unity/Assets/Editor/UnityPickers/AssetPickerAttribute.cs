using JetBrains.Annotations;
using UnityEngine;

namespace UnityPickers
{
	public class AssetPickerAttribute : PropertyAttribute
	{
		[NotNull]
		public string Path;

		public AssetPickerAttribute(string path = "")
		{
			Path = path;
		}
	}
}