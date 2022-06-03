using UnityEngine;

namespace UnityPickers
{
	public class AssetPathFilterAttribute : PropertyAttribute
	{
		public string[] Filters;

		public AssetPathFilterAttribute(params string[] filters)
		{
			Filters = filters;
		}
	}
}