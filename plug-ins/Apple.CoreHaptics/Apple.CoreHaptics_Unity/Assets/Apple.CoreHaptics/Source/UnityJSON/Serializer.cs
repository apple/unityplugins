using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Apple.UnityJSON
{
	/// <summary>
	/// Serializes an object into JSON string. Subclasses should override
	/// the #TrySerialize method.
	/// </summary>
	public class Serializer
	{
		private const string _kUndefined = "undefined";
		private const string _kNull = "null";
		private const string _kTrue = "true";
		private const string _kFalse = "false";

		private static Serializer _default = new Serializer();

		/// <summary>
		/// The default serializer to be used when no serializer is given.
		/// You can set this to your own default serializer. Uses the
		/// #Simple serializer by default.
		/// </summary>
		public static Serializer Default
		{
			get { return _default; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("default serializer");
				}
				_default = value;
			}
		}

		/// <summary>
		/// The initial serializer that is provided by the framework.
		/// </summary>
		private static readonly Serializer simple = new Serializer();

		/// <summary>
		/// When set to true, the keyword undefined is used instead of null
		/// when serializing. Defaults to false.
		/// </summary>
		public bool useUndefinedForNull = false;

		protected Serializer()
		{
		}

		/// <summary>
		/// Tries to perform application-specific serialization. This is called
		/// first when the #Serialize method is called, and performs internal
		/// serialization if it returns <c>false</c>. This method is called before
		/// ISerializable.Serialize.
		/// </summary>
		/// <param name="obj">Object to serialize. Object is guaranteed to be non-null.</param>
		/// <param name="options">Serialization options.</param>
		/// <param name="serialized">The serialized JSON string.</param>
		protected virtual bool TrySerialize(
			object obj,
			NodeOptions options,
			out string serialized)
		{
			serialized = null;
			return false;
		}

		/// <summary>
		/// Serializes any object into JSON string according to the given
		/// options. This will first run through the #TrySerialize method and
		/// then ISerializable.Serialize method if the object implements it.
		/// If these are not implemented, then the framework serialization is
		/// performed.
		/// 
		/// This is the general serialization function and an object specific
		/// function should be prefered when performing manual serialization.
		/// </summary>
		public string Serialize(object obj, NodeOptions options = NodeOptions.Default)
		{
			if (obj == null)
			{
				return SerializeNull(options);
			}

			string result = null;
			if (TrySerialize(obj, options, out result))
			{
				return result;
			}

			ISerializable serializable = obj as ISerializable;
			if (serializable != null)
			{
				return serializable.Serialize(this);
			}

			if (obj != null)
			{
				Type type = obj.GetType();
				if (type.IsValueType)
				{
					if (type.IsEnum)
					{
						result = _SerializeEnum((Enum)obj);
					}
					else if (type.IsPrimitive)
					{
						if (obj is bool)
						{
							result = SerializeBool((bool)obj);
						}
						else
						{
							result = obj.ToString();
						}
					}
					else if (obj is float floatValue)
					{
						// Format floats with a decimal, not a comma
						result = floatValue.ToString(new CultureInfo("en-US", false));
					}
					else
					{
						if (obj is DictionaryEntry)
						{
							result = _SerializeDictionaryEntry((DictionaryEntry)obj, options);
						}
						else if (obj is Vector2)
						{
							result = SerializeVector2((Vector2)obj);
						}
						else if (obj is Vector3)
						{
							result = SerializeVector3((Vector3)obj);
						}
						else if (obj is Vector4)
						{
							result = SerializeVector4((Vector4)obj);
						}
						else if (obj is Quaternion)
						{
							result = SerializeQuaternion((Quaternion)obj);
						}
						else if (obj is Color)
						{
							result = SerializeColor((Color)obj);
						}
						else if (obj is Rect)
						{
							result = SerializeRect((Rect)obj);
						}
						else if (obj is Bounds)
						{
							result = SerializeBounds((Bounds)obj);
						}
						else
						{
							result = _SerializeCustom(obj, options);
						}
					}
				}
				else
				{
					if (obj is string)
					{
						result = _SerializeString(obj as string);
					}
					else if (Nullable.GetUnderlyingType(type) != null)
					{
						result = _SerializeNullable(obj, options);
					}
					else if (obj is IEnumerable)
					{
						string enumerableJSON = _SerializeEnumarable(obj as IEnumerable, options);
						if (obj is IDictionary)
						{
							result = "{" + enumerableJSON + "}";
						}
						else
						{
							result = "[" + enumerableJSON + "]";
						}
					}
					else
					{
						result = _SerializeCustom(obj, options);
					}
				}
			}

			if (result == null)
			{
				return SerializeNull(options);
			}
			else
			{
				return result;
			}
		}

		/// <summary>
		/// Serializes <c>null</c> if NodeOptions.SerializeNull is used or
		/// returns <c>null</c> otherwise. Null can be serialized as
		/// "null" or "undefined" according to the value of #useUndefinedForNull.
		/// </summary>
		public string SerializeNull(NodeOptions options)
		{
			return options.ShouldSerializeNull() ?
				(useUndefinedForNull ? _kUndefined : _kNull) : null;
		}

		/// <summary>
		/// Serializes a string into JSON string with the optional
		/// node options.
		/// </summary>
		public string SerializeString(
			string stringValue,
			NodeOptions options = NodeOptions.Default)
		{
			if (stringValue == null)
			{
				return SerializeNull(options);
			}
			return _SerializeString(stringValue);
		}

		/// <summary>
		/// Serializes an enum into JSON string. Throws ArgumentNullException
		/// if the value is <c>null</c>.
		/// </summary>
		public string SerializeEnum(Enum enumValue)
		{
			if (enumValue == null)
			{
				throw new ArgumentNullException("enumValue");
			}
			return _SerializeEnum(enumValue);
		}

		/// <summary>
		/// Serializes a System.Nullable<T> object into JSON string.
		/// Throws an ArgumentException if the object is not of a
		/// nullable type.
		/// </summary>
		public string SerializeNullable(
			object nullable,
			NodeOptions options = NodeOptions.Default)
		{
			if (nullable == null)
			{
				return SerializeNull(options);
			}
			if (Nullable.GetUnderlyingType(nullable.GetType()) == null)
			{
				throw new ArgumentException("Argument is not a nullable object.");
			}
			return _SerializeNullable(nullable, options);
		}

		/// <summary>
		/// Serializes a boolean value.
		/// </summary>
		public string SerializeBool(bool boolValue)
		{
			return boolValue ? _kTrue : _kFalse;
		}

		/// <summary>
		/// Serializes Unity Vector2 into JSON string.
		/// </summary>
		public string SerializeVector2(Vector2 vector)
		{
			return "{\"x\":" + vector.x + ",\"y\":" + vector.y + "}";
		}

		/// <summary>
		/// Serializes Unity Vector3 into JSON string.
		/// </summary>
		public string SerializeVector3(Vector3 vector)
		{
			return "{\"x\":" + vector.x + ",\"y\":" + vector.y + ",\"z\":" + vector.z + "}";
		}

		/// <summary>
		/// Serializes Unity Vector4 into JSON string.
		/// </summary>
		public string SerializeVector4(Vector4 vector)
		{
			return "{\"x\":" + vector.x + ",\"y\":" + vector.y
			+ ",\"z\":" + vector.z + ",\"w\":" + vector.w + "}";
		}

		/// <summary>
		/// Serializes Unity Quaternion into JSON string.
		/// </summary>
		public string SerializeQuaternion(Quaternion quaternion)
		{
			return "{\"x\":" + quaternion.x + ",\"y\":" + quaternion.y
			+ ",\"z\":" + quaternion.z + ",\"w\":" + quaternion.w + "}";
		}

		/// <summary>
		/// Serializes Unity Color into JSON string.
		/// </summary>
		public string SerializeColor(Color color)
		{
			return "{\"r\":" + color.r + ",\"g\":" + color.g
			+ ",\"b\":" + color.b + ",\"a\":" + color.a + "}";
		}

		/// <summary>
		/// Serializes Unity Rect into JSON string.
		/// </summary>
		public string SerializeRect(Rect rect)
		{
			return "{\"x\":" + rect.x + ",\"y\":" + rect.y
			+ ",\"width\":" + rect.width + ",\"height\":" + rect.height + "}";
		}

		/// <summary>
		/// Serializes Unity Bounds into JSON string.
		/// </summary>
		public string SerializeBounds(Bounds bounds)
		{
			return "{\"center\":" + SerializeVector3(bounds.center)
			+ ",\"extents\":" + SerializeVector3(bounds.extents) + "}";
		}

		/// <summary>
		/// Serializes an object by its fields and properties. This will
		/// ignore custom serializations of the object (see Serializer.TrySerialize
		/// and ISerializable.Serialize). This will throw an argument exception if
		/// the object is a non-struct value type (primitives and enums).
		/// </summary>
		public string SerializeByParts(object obj, NodeOptions options = NodeOptions.Default)
		{
			if (obj == null)
			{
				return SerializeNull(options);
			}
			Type type = obj.GetType();
			if (type.IsPrimitive || type.IsEnum)
			{
				throw new ArgumentException("Cannot serialize non-struct value types by parts.");
			}

			return _SerializeCustom(obj, options);
		}

		private string _SerializeString(string stringValue)
		{
			return "\"" + stringValue.Replace("\"", "\\\"") + "\"";
		}

		private string _SerializeEnum(Enum obj)
		{
			Type type = obj.GetType();
			JSONEnumAttribute enumAttribute = Util.GetAttribute<JSONEnumAttribute>(type);
			if (enumAttribute != null)
			{
				if (enumAttribute.useIntegers)
				{
					return (Convert.ToInt32(obj)).ToString();
				}
				else
				{
					string formatted = _FormatEnumMember(obj.ToString(), enumAttribute.format);
					if (enumAttribute.prefix != null)
					{
						formatted = enumAttribute.prefix + formatted;
					}
					if (enumAttribute.suffix != null)
					{
						formatted += enumAttribute.suffix;
					}
					return _SerializeString(formatted);
				}
			}
			else
			{
				return _SerializeString(obj.ToString());
			}
		}

		private string _SerializeNullable(object nullable, NodeOptions options)
		{
			Type type = nullable.GetType();
			if (!(bool)(type.GetProperty("HasValue").GetValue(nullable, null)))
			{
				return SerializeNull(options);
			}

			return Serialize(type.GetProperty("Value").GetValue(nullable, null), options);
		}

		private string _SerializeEnumarable(IEnumerable enumerable, NodeOptions options)
		{
			if (enumerable is IList)
			{
				// Always serialize nulls for arrays.
				options |= NodeOptions.SerializeNull;
			}
			string joined = _Join(enumerable, obj => Serialize(obj, options));
			if (joined == null)
			{
				return SerializeNull(options);
			}
			else
			{
				return joined;
			}
		}

		private string _SerializeDictionaryEntry(DictionaryEntry entry, NodeOptions options)
		{
			// Don't serialize nulls for keys. If a key is null, its pair
			// shouldn't be serialized as undefined:value.
			NodeOptions keyOptions = options & ~NodeOptions.SerializeNull;
			string serializedKey = Serialize(entry.Key, keyOptions);
			if (serializedKey == null)
			{
				return SerializeNull(options);
			}

			string valueKey = Serialize(entry.Value, options);
			if (valueKey == null)
			{
				return null;
			}
			else
			{
				string jsonKey = entry.Key is string ? serializedKey : _SerializeString(serializedKey);
				return jsonKey + ":" + valueKey;
			}
		}

		private string _SerializeCustom(object obj, NodeOptions options)
		{
			ISerializationListener listener = obj as ISerializationListener;
			if (listener != null)
			{
				listener.OnSerializationWillBegin(this);
			}

			try
			{
				IEnumerable<string> enumerable = new string[] { };
				MemberInfo extrasMember = null;
				JSONExtrasAttribute extrasAttribute = null;

				// Find member info and attribute for extras while going over the
				// fields and properties.
				Func<MemberInfo, bool> isNotExtras = m =>
				{
					if (extrasMember == null && Util.IsJSONExtrasMember(m, out extrasAttribute))
					{
						extrasMember = m;
						return false;
					}
					else
					{
						return true;
					}
				};

				Type type = obj.GetType();
				JSONObjectAttribute objectAttribute = Util.GetAttribute<JSONObjectAttribute>(type);
				var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
				bool useTupleFormat = false;
				if (objectAttribute != null)
				{
					if (!objectAttribute.options.ShouldIgnoreStatic())
					{
						flags |= BindingFlags.Static;
					}
					if (objectAttribute.options.ShouldUseTupleFormat())
					{
						useTupleFormat = true;
					}
				}

				enumerable = enumerable.Concat(
					from f in type.GetFields(flags)
					where isNotExtras(f) && _IsValidFieldInfo(f)
					select _SerializeCustomField(obj, f, useTupleFormat));

				if (objectAttribute == null || !objectAttribute.options.ShouldIgnoreProperties())
				{
					enumerable = enumerable.Concat(
						from p in type.GetProperties(flags)
						where isNotExtras(p) && _IsValidPropertyInfo(p)
						select _SerializeCustomProperty(obj, p, useTupleFormat));
				}

				// Serialize all properties and fields.
				var result = _Join(enumerable, o => o as string);

				// Serialize the extras if there are any.
				if (!useTupleFormat && extrasMember != null)
				{
					var extras = Util.GetMemberValue(extrasMember, obj) as IEnumerable;
					if (extras != null)
					{
						result += (result == "" ? "" : ",")
						+ _SerializeEnumarable(extras, extrasAttribute.options);
					}
				}

				if (listener != null)
				{
					listener.OnSerializationSucceeded(this);
				}

				if (useTupleFormat)
				{
					return "[" + result + "]";
				}
				else
				{
					return "{" + result + "}";
				}
			}
			catch (Exception exception)
			{
				if (listener != null)
				{
					listener.OnSerializationFailed(this);
				}
				throw exception;
			}
		}

		private bool _IsValidFieldInfo(FieldInfo fieldInfo)
		{
			return fieldInfo.IsPublic || Attribute.IsDefined(fieldInfo, typeof(JSONNodeAttribute), true);
		}

		private bool _IsValidPropertyInfo(PropertyInfo propertyInfo)
		{
			return propertyInfo.GetIndexParameters().Length == 0 && propertyInfo.CanRead &&
			(propertyInfo.GetGetMethod(false) != null ||
			Attribute.IsDefined(propertyInfo, typeof(JSONNodeAttribute), true));
		}

		private string _FormatEnumMember(string member, JSONEnumMemberFormating format)
		{
			switch (format)
			{
				case JSONEnumMemberFormating.Lowercased:
					return member.ToLower();
				case JSONEnumMemberFormating.Uppercased:
					return member.ToUpper();
				case JSONEnumMemberFormating.Capitalized:
					return Char.ToUpper(member[0]) + member.Substring(1).ToLower();
				default:
					return member;
			}
		}

		private string _Join(IEnumerable enumerable, Func<object, string> serializer)
		{
			string result = "";
			bool firstAdded = false;

			IEnumerator enumerator = enumerable is IDictionary ?
				(enumerable as IDictionary).GetEnumerator() :
				enumerable.GetEnumerator();

			while (enumerator.MoveNext())
			{
				string serialized = serializer(enumerator.Current);
				if (serialized != null)
				{
					string prefix = firstAdded ? "," : "";
					firstAdded = true;
					result += prefix + serialized;
				}
			}
			return result;
		}

		private string _SerializeCustomField(object obj, FieldInfo fieldInfo, bool useTupleFormat)
		{
			return _SerializeCustomMember(fieldInfo, fieldInfo.GetValue(obj), useTupleFormat);
		}

		private string _SerializeCustomProperty(object obj, PropertyInfo propertyInfo, bool useTupleFormat)
		{
			return _SerializeCustomMember(propertyInfo, propertyInfo.GetValue(obj, null), useTupleFormat);
		}

		private string _SerializeCustomMember(MemberInfo keyMemberInfo, object value, bool useTupleFormat)
		{
			JSONNodeAttribute attribute = Util.GetAttribute<JSONNodeAttribute>(keyMemberInfo);
			NodeOptions options = attribute == null ? NodeOptions.Default : attribute.options;
			if (!options.IsSerialized())
			{
				return null;
			}

			string valueString = Serialize(value, options);
			if (valueString != null || options.ShouldSerializeNull())
			{
				if (useTupleFormat)
				{
					return valueString == null ? _kUndefined : valueString;
				}
				string key = (attribute != null && attribute.key != null) ? attribute.key : keyMemberInfo.Name;
				return _SerializeString(key) + ":" + (valueString == null ? _kUndefined : valueString);
			}
			else
			{
				return null;
			}
		}
	}
}
