using System;
using System.ComponentModel;

namespace Apple.CoreHaptics
{
	public static class CHEnumExtension
	{
		public static string GetDescription(this Enum value)
		{
			var type = value.GetType();
			var name = Enum.GetName(type, value);
			if (!(name is null))
			{
				var field = type.GetField(name);
				if (!(field is null))
				{
					if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
					{
						return attr.Description;
					}
				}
			}
			return Enum.GetName(type, value);
		}
	}
}
