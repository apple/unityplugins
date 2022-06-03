using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityPickers
{
	public class TypePicker : ValuePicker<Type>
	{
		public static void Button(
			[NotNull] string buttonText,
			[NotNull] Func<IEnumerable<Type>> valuesCollector,
			[NotNull] Action<Type> callback,
			bool showNow = false,
			[CanBeNull] GUIStyle style = null,
			[NotNull] params GUILayoutOption[] options)
		{
			Button(
				GetWindow<TypePicker>,
				buttonText,
				valuesCollector,
				callback,
				showNow,
				style,
				options
			);
		}

		public static void Button(
			Rect rect,
			[NotNull] string buttonText,
			[NotNull] Func<IEnumerable<Type>> valuesCollector,
			[NotNull] Action<Type> callback,
			bool showNow = false,
			[CanBeNull] GUIStyle style = null)
		{
			Button(
				GetWindow<TypePicker>,
				rect,
				buttonText,
				valuesCollector,
				callback,
				showNow,
				style
			);
		}

		protected override string GetValueName(Type value)
		{
			return value.Name;
		}
	}
}