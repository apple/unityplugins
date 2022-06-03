using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityPickers.Utility
{
	public static class TypeEx
	{
		public static bool HasAttribute<T>(this Type t) where T : Attribute
		{
			return t.GetCustomAttributes(typeof(T), true).Length > 0;
		}

		[CanBeNull]
		public static T GetAttribute<T>(this Type t) where T : Attribute
		{
			return t.GetCustomAttributes(typeof(T), true).Cast<T>().FirstOrDefault();
		}

		public static bool Implements<T>(this Type t)
		{
			return t.GetInterfaces().Contains(typeof(T));
		}

		public static bool HasAttribute<T>(this MemberInfo m) where T : Attribute
		{
			return m.GetCustomAttributes(typeof(T), true).Length > 0;
		}

		[CanBeNull]
		public static T GetAttribute<T>(this MemberInfo m) where T : Attribute
		{
			return m.GetCustomAttributes(typeof(T), true).OfType<T>().FirstOrDefault();
		}

		public static IEnumerable<Type> GetSubclasses<T>(bool includeAbstract = false, Assembly assembly = null)
		{
			return GetSubclasses(typeof(T), includeAbstract, assembly);
		}

		public static IEnumerable<Type> GetSubclasses(this Type type, bool includeAbstract = false, Assembly assembly = null)
		{
			if (type.IsArray)
			{
				return GetSubclasses(type.GetElementType(), includeAbstract, assembly);
			}
			if (type.IsGenericTypeDefinition)
			{
				return GetSubclassesGeneric(type, includeAbstract, assembly);
			}

			if (assembly == null)
				assembly = type.Assembly;

			return assembly
				.GetTypes()
				.Where(t => t.IsSubclassOf(type) && (includeAbstract || !t.IsAbstract));
		}

		public static IEnumerable<Type> GetSubclassesGeneric(this Type type, bool includeAbstract = false, Assembly assembly = null)
		{
			if (assembly == null)
				assembly = type.Assembly;

			foreach (var otherType in assembly.GetTypes())
			{
				if (otherType.IsInterface || (otherType.IsAbstract && !includeAbstract))
					continue;

				var t = otherType;
				while (t != null)
				{
					if (t.IsGenericType)
						t = t.GetGenericTypeDefinition();
					if (t.IsGenericTypeDefinition && (t.IsSubclassOf(type) || (t == type)))
					{
						yield return otherType;
						t = null;
					}
					else
					{
						t = t.BaseType;
					}
				}
			}
		}

		public static IEnumerable<FieldInfo> GetUnitySerializedFields(this Type type)
		{
			var t =
				type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
			var fields = t
				.Where(f => f.IsPublic || f.HasAttribute<SerializeField>());
			if (type.BaseType != null)
			{
				fields = fields.Concat(GetUnitySerializedFields(type.BaseType));
			}
			return fields;
		}

		public static bool IsUnityCollection(this Type type)
		{
			return type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
		}

		[CanBeNull]
		public static FieldInfo GetFieldByPath(this Type type, string path)
		{
			if (path.Contains('.'))
			{
				return type.GetFieldByPath(path.Split('.'));
			}

			const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			return type.GetField(path, flags);
		}

		[CanBeNull]
		public static FieldInfo GetFieldByPath(this Type sourceType, IEnumerable<string> path)
		{
			FieldInfo fieldInfo = null;
			Type type = sourceType;
			foreach (string name in path)
			{
				fieldInfo = type.GetFieldByPath(name);
				if (fieldInfo == null)
				{
					break;
				}
				type = fieldInfo.FieldType;
			}
			return fieldInfo;
		}
	}
}