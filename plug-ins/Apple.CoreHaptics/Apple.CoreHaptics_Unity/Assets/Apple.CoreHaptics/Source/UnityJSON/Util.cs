using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Apple.UnityJSON
{
	internal static class Util
	{
		internal static T GetAttribute<T> (MemberInfo info) where T : Attribute
		{
			object[] attributes = info.GetCustomAttributes (typeof(T), true);
			return (attributes == null || attributes.Length == 0) ? null : attributes [0] as T;
		}

		internal static T GetAttribute<T> (ParameterInfo info) where T : Attribute
		{
			object[] attributes = info.GetCustomAttributes (typeof(T), true);
			return (attributes == null || attributes.Length == 0) ? null : attributes [0] as T;
		}

		internal static Type GetMemberType (MemberInfo memberInfo)
		{
			return memberInfo is FieldInfo ?
				(memberInfo as FieldInfo).FieldType : (memberInfo as PropertyInfo).PropertyType;
		}

		internal static object GetMemberValue (MemberInfo memberInfo, object obj)
		{
			if (memberInfo is FieldInfo) {
				return (memberInfo as FieldInfo).GetValue (obj);
			} else {
				return (memberInfo as PropertyInfo).GetValue (obj, null);
			}
		}

		internal static void SetMemberValue (MemberInfo memberInfo, object obj, object value)
		{
			if (memberInfo is FieldInfo) {
				(memberInfo as FieldInfo).SetValue (obj, value);
			} else {
				(memberInfo as PropertyInfo).SetValue (obj, value, null);
			}
		}

		internal static bool IsJSONExtrasMember (MemberInfo memberInfo, out JSONExtrasAttribute attribute)
		{
			Type type = GetMemberType (memberInfo);
			if (type != typeof(System.Collections.Generic.Dictionary<string, object>)) {
				attribute = null;
				return false;
			}

			attribute = GetAttribute<JSONExtrasAttribute> (memberInfo);
			return !(attribute is null);
		}

		internal static bool IsCustomType (Type type)
		{
			return !type.IsEnum && !type.IsPrimitive && (type.IsValueType
			|| (!typeof(IEnumerable).IsAssignableFrom (type)
			&& Nullable.GetUnderlyingType (type) == null
			&& type != typeof(object)));
		}

		internal static bool IsDictionary (Type type)
		{
			return typeof(IDictionary).IsAssignableFrom (type) ||
			(type.IsGenericType && type.GetGenericTypeDefinition () == typeof(IDictionary<,>));
		}
	}
}
