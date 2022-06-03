using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityJSON
{
	/// <summary>
	/// An exception during the deserialization process.
	/// </summary>
	public class DeserializationException : Exception
	{
		public DeserializationException () : base ()
		{
		}

		public DeserializationException (string message) : base (message)
		{
		}
	}

	/// <summary>
	/// Deserializes JSON string either into newly instantiated or
	/// previously existing objects.
	/// </summary>
	public class Deserializer
	{
		private static Deserializer _default = new Deserializer ();

		/// <summary>
		/// The default deserializer to be used when no deserializer is given.
		/// You can set this to your own default deserializer. Uses the
		/// #Simple deserializer by default.
		/// </summary>
		public static Deserializer Default {
			get { return _default; }
			set {
				if (value == null) {
					throw new ArgumentNullException ("default deserializer");
				}
				_default = value;
			}
		}

		/// <summary>
		/// The initial deserializer that is provided by the framework.
		/// </summary>
		public static readonly Deserializer Simple = new Deserializer ();

		private Instantiater _instantiater = Instantiater.Default;

		/// <summary>
		/// The instantiater associated with this deserializer. If not set,
		/// the simple instantiater is used.
		/// </summary>
		public Instantiater instantiater {
			get { return _instantiater; }
			set {
				if (value == null) {
					throw new ArgumentNullException ("instantiater");
				}
				_instantiater = value;
			}
		}

		protected Deserializer ()
		{
		}

		/// <summary>
		/// Tries to deserialize the JSON node onto the given object. It is guaranteed
		/// that the object is not null. This will be called before trying any other
		/// deserialization method. Subclasses should override this method to perform
		/// their own deserialization logic.
		/// </summary>
		protected virtual bool TryDeserializeOn (
			object obj,
			JSONNode node,
			NodeOptions options,
			HashSet<string> ignoredKeys)
		{
			return false;
		}

		/// <summary>
		/// Deserializes the JSON string directly on the object. Throws an
		/// ArgumentNullException of the object is <c>null</c>. This will first
		/// try #TryDeserialize method and then IDeserializable.Deserialize if the
		/// object implements the interface. This performs a general deserialization
		/// and classes should prefer specific methods for manual deserialization.
		/// </summary>
		public void DeserializeOn (
			object obj, 
			string jsonString, 
			NodeOptions options = NodeOptions.Default,
			HashSet<string> ignoredKeys = null)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			DeserializeOn (obj, node, options, ignoredKeys);
		}

		/// <summary>
		/// Deserializes the JSON node directly on the object. Throws an
		/// ArgumentNullException of the object is <c>null</c>. This will first
		/// try #TryDeserialize method and then IDeserializable.Deserialize if the
		/// object implements the interface. This performs a general deserialization
		/// and classes should prefer specific methods for manual deserialization.
		/// </summary>
		public void DeserializeOn (
			object obj, 
			JSONNode node, 
			NodeOptions options = NodeOptions.Default,
			HashSet<string> ignoredKeys = null)
		{
			if (obj == null) {
				throw new ArgumentNullException ("obj");
			}
			if (node == null) {
				throw new ArgumentNullException ("node");
			}

			Type type = obj.GetType ();
			if (type.IsEnum) {
				throw new ArgumentException ("Cannot deserialize on enums.");
			} else if (type.IsPrimitive) {
				throw new ArgumentException ("Cannot deserialize on primitive types.");
			}

			if (ignoredKeys == null) {
				ignoredKeys = new HashSet<string> ();
			}
			_DeserializeOn (obj, node, options, ignoredKeys);
		}

		/// <summary>
		/// Deserializes the JSON string directly on the object. Throws an
		/// ArgumentNullException of the object is <c>null</c>. This ignore
		/// manual deserialization options (see Deserializer.TryDeserialize and
		/// IDeserializable.Deserialize).
		/// </summary>
		public void DeserializeByParts (
			object obj, 
			string jsonString, 
			NodeOptions options = NodeOptions.Default,
			HashSet<string> ignoredKeys = null)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			DeserializeByParts (obj, node, options, ignoredKeys);
		}

		/// <summary>
		/// Deserializes the JSON node directly on the object. Throws an
		/// ArgumentNullException of the object is <c>null</c>. This ignore
		/// manual deserialization options (see Deserializer.TryDeserialize and
		/// IDeserializable.Deserialize).
		/// </summary>
		public void DeserializeByParts (
			object obj, 
			JSONNode node, 
			NodeOptions options = NodeOptions.Default,
			HashSet<string> ignoredKeys = null)
		{
			if (obj == null) {
				throw new ArgumentNullException ("obj");
			}
			if (node == null) {
				throw new ArgumentNullException ("node");
			}

			Type type = obj.GetType ();
			if (type.IsEnum) {
				throw new ArgumentException ("Cannot deserialize on enums.");
			} else if (type.IsPrimitive) {
				throw new ArgumentException ("Cannot deserialize on primitive types.");
			}

			if (ignoredKeys == null) {
				ignoredKeys = new HashSet<string> ();
			}
			_DeserializeByParts (obj, node, options, ignoredKeys);
		}

		/// <summary>
		/// Deserializes the JSON string to a new object of the requested type. This
		/// will first insantiate an object of the target type. The instantiation
		/// will use the associated Instantiater for custom types. If an object can be
		/// instantiated, #DeserializeOn method is used to feed the JSON into the object.
		/// </summary>
		/// <param name="jsonString">JSON string to deserialize.</param>
		/// <param name="type">Requested type of the deserialized object.</param>
		/// <param name="options">Deserialization options for the node (optional).</param>
		public object Deserialize (
			string jsonString, 
			Type type, 
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return Deserialize (node, type, options);
		}

		/// <summary>
		/// Deserializes the JSON node to a new object of the requested type. This
		/// will first insantiate an object of the target type. The instantiation
		/// will use the associated Instantiater for custom types. If an object can be
		/// instantiated, #DeserializeOn method is used to feed the JSON into the object.
		/// </summary>
		/// <param name="node">JSON node to deserialize.</param>
		/// <param name="type">Requested type of the deserialized object.</param>
		/// <param name="options">Deserialization options for the node (optional).</param>
		public object Deserialize (
			JSONNode node, 
			Type type, 
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			return Deserialize (node, type, options, ObjectTypes.JSON, null);
		}

		/// <summary>
		/// Deserializes a JSON string into a C# System.Object type. If no restrictions
		/// are given, the deserialized types can be doubles, booleans, strings, and arrays
		/// and dictionaries thereof. Restricted types can allow custom types to create
		/// classes or structs instead of dictionaries.
		/// </summary>
		/// <param name="jsonString">JSON string to deserialize.</param>
		/// <param name="restrictedTypes">Restricted types for the object.</param>
		/// <param name="customTypes">Allowed custom types for the object. Restrictions
		/// must allow custom types if not <c>null</c>.</param>
		/// <param name="options">Deserialization options.</param>
		public object DeserializeToObject (
			string jsonString,
			ObjectTypes restrictedTypes = ObjectTypes.JSON,
			Type[] customTypes = null,
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToObject (jsonString, restrictedTypes, customTypes, options);
		}

		/// <summary>
		/// Deserializes a JSON node into a C# System.Object type. If no restrictions
		/// are given, the deserialized types can be doubles, booleans, strings, and arrays
		/// and dictionaries thereof. Restricted types can allow custom types to create
		/// classes or structs instead of dictionaries.
		/// </summary>
		/// <param name="node">JSON node to deserialize.</param>
		/// <param name="restrictedTypes">Restricted types for the object.</param>
		/// <param name="customTypes">Allowed custom types for the object. Restrictions
		/// must allow custom types if not <c>null</c>.</param>
		/// <param name="options">Deserialization options.</param>
		public object DeserializeToObject (
			JSONNode node,
			ObjectTypes restrictedTypes = ObjectTypes.JSON,
			Type[] customTypes = null,
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			if (customTypes != null) {
				if (!restrictedTypes.SupportsCustom ()) {
					throw new ArgumentException ("Restrictions do not allow custom types.");
				}
				foreach (Type type in customTypes) {
					if (!Util.IsCustomType (type)) {
						throw new ArgumentException ("Unsupported custom type: " + type);
					}
				}
			}
			return _DeserializeToObject (node, options, restrictedTypes, customTypes);
		}

		/// <summary>
		/// Deserializes a JSON string into a System.Nullable object.
		/// </summary>
		public Nullable<T> DeserializeToNullable<T> (
			string jsonString,
			NodeOptions options = NodeOptions.Default) where T : struct
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToNullable<T> (node, options);
		}

		/// <summary>
		/// Deserializes a JSON node into a System.Nullable object.
		/// </summary>
		public Nullable<T> DeserializeToNullable<T> (
			JSONNode node,
			NodeOptions options = NodeOptions.Default) where T : struct
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			if (node.IsNull || node.Tag == JSONNodeType.None) {
				return null;
			}
			return (Nullable<T>)Deserialize (node, typeof(T), options);
		}

		/// <summary>
		/// Deserializes a JSON string into an integer. Throws an ArgumentNullException
		/// if the string is <c>null</c>. Throws a CastException if the node does
		/// not contain an integer.
		/// </summary>
		public int DeserializeToInt (
			string jsonString,
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToInt (node, options);
		}

		/// <summary>
		/// Deserializes a JSON node into an integer. Throws an ArgumentNullException
		/// if the node is <c>null</c>. Throws a CastException if the node does
		/// not contain an integer.
		/// </summary>
		public int DeserializeToInt (
			JSONNode node,
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			return (int)_DeserializeToInt (node, options);
		}

		/// <summary>
		/// Deserializes a JSON string into an unsigned integer. Throws an ArgumentNullException
		/// if the string is <c>null</c>. Throws a CastException if the node does
		/// not contain an unsigned integer.
		/// </summary>
		public uint DeserializeToUInt (
			string jsonString,
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToUInt (node, options);
		}

		/// <summary>
		/// Deserializes a JSON node into an unsigned integer. Throws an ArgumentNullException
		/// if the node is <c>null</c>. Throws a CastException if the node does
		/// not contain an unsigned integer.
		/// </summary>
		public uint DeserializeToUInt (
			JSONNode node,
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			return (uint)_DeserializeToUInt (node, options);
		}

		/// <summary>
		/// Deserializes a JSON string into a byte. Throws an ArgumentNullException
		/// if the string is <c>null</c>. Throws a CastException if the node does
		/// not contain a byte.
		/// </summary>
		public byte DeserializeToByte (
			string jsonString,
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToByte (node, options);
		}

		/// <summary>
		/// Deserializes a JSON node into a byte. Throws an ArgumentNullException
		/// if the node is <c>null</c>. Throws a CastException if the node does
		/// not contain a byte.
		/// </summary>
		public byte DeserializeToByte (
			JSONNode node,
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			return (byte)_DeserializeToByte (node, options);
		}

		/// <summary>
		/// Deserializes a JSON string into a boolean. Throws an ArgumentNullException
		/// if the string is <c>null</c>. Throws a CastException if the node does
		/// not contain a boolean.
		/// </summary>
		public bool DeserializeToBool (
			string jsonString,
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToBool (node, options);
		}

		/// <summary>
		/// Deserializes a JSON node into a boolean. Throws an ArgumentNullException
		/// if the node is <c>null</c>. Throws a CastException if the node does
		/// not contain a boolean.
		/// </summary>
		public bool DeserializeToBool (
			JSONNode node,
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			return (bool)_DeserializeToBool (node, options);
		}

		/// <summary>
		/// Deserializes a JSON string into a float. Throws an ArgumentNullException
		/// if the string is <c>null</c>. Throws a CastException if the node does
		/// not contain a float.
		/// </summary>
		public float DeserializeToFloat (
			string jsonString,
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToFloat (node, options);
		}

		/// <summary>
		/// Deserializes a JSON node into a float. Throws an ArgumentNullException
		/// if the node is <c>null</c>. Throws a CastException if the node does
		/// not contain a float.
		/// </summary>
		public float DeserializeToFloat (
			JSONNode node,
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			return (float)_DeserializeToFloat (node, options);
		}

		/// <summary>
		/// Deserializes a JSON string into a double. Throws an ArgumentNullException
		/// if the string is <c>null</c>. Throws a CastException if the node does
		/// not contain a double.
		/// </summary>
		public double DeserializeToDouble (
			string jsonString,
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToDouble (node, options);
		}

		/// <summary>
		/// Deserializes a JSON node into a double. Throws an ArgumentNullException
		/// if the node is <c>null</c>. Throws a CastException if the node does
		/// not contain a double.
		/// </summary>
		public double DeserializeToDouble (
			JSONNode node,
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			return (double)_DeserializeToDouble (node, options);
		}

		/// <summary>
		/// Deserializes a JSON string into a long. Throws an ArgumentNullException
		/// if the string is <c>null</c>. Throws a CastException if the node does
		/// not contain a long.
		/// </summary>
		public long DeserializeToLong (
			string jsonString,
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToLong (node, options);
		}

		/// <summary>
		/// Deserializes a JSON node into a long. Throws an ArgumentNullException
		/// if the node is <c>null</c>. Throws a CastException if the node does
		/// not contain a long.
		/// </summary>
		public long DeserializeToLong (
			JSONNode node,
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			return (long)_DeserializeToLong (node, options);
		}

		/// <summary>
		/// Deserializes a JSON string into a string. Throws an ArgumentNullException
		/// if the node is <c>null</c>.
		/// </summary>
		public string DeserializeToString (
			string jsonString,
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToString (node, options);
		}

		/// <summary>
		/// Deserializes a JSON node into a string.
		/// </summary>
		public string DeserializeToString (
			JSONNode node,
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			if (node.IsNull || node.Tag == JSONNodeType.None) {
				return null;
			}
			return _DeserializeToString (node, options);
		}

		/// <summary>
		/// Deserializes a JSON string into an enum. Throws an ArgumentNullException
		/// if the string is <c>null</c>. Throws an ArgumentException if the generic
		/// type T is not an enum.
		/// </summary>
		public T DeserializeToEnum<T> (
			string jsonString,
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToEnum<T> (node, options);
		}

		/// <summary>
		/// Deserializes a JSON node into an enum. Throws an ArgumentNullException
		/// if the node is <c>null</c>. Throws an ArgumentException if the generic
		/// type T is not an enum.
		/// </summary>
		public T DeserializeToEnum<T> (
			JSONNode node,
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			if (!typeof(T).IsEnum) {
				throw new ArgumentException ("Generic type is not an enum.");
			}
			return (T)_DeserializeToEnum (node, typeof(T), options);
		}

		/// <summary>
		/// Deserializes a JSON string into a generic list.
		/// </summary>
		public List<T> DeserializeToList<T> (
			string jsonString,
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToList<T> (node, options);
		}

		/// <summary>
		/// Deserializes a JSON node into a generic list.
		/// </summary>
		public List<T> DeserializeToList<T> (
			JSONNode node,
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			if (node.IsNull || node.Tag == JSONNodeType.None) {
				return null;
			}
			var list = new List<T> ();
			_FeedList (list, node, typeof(T), options);
			return list;
		}

		/// <summary>
		/// Deserializes a JSON string into a System.Object list. If no restrictions
		/// are given, the deserialized types can be doubles, booleans, strings, and arrays
		/// and dictionaries thereof. Restricted types can allow custom types to create
		/// classes or structs instead of dictionaries.
		/// </summary>
		/// <param name="jsonString">JSON string to deserialize.</param>
		/// <param name="restrictedTypes">Restricted types for the object.</param>
		/// <param name="customTypes">Allowed custom types for the object. Restrictions
		/// must allow custom types if not <c>null</c>.</param>
		/// <param name="options">Deserialization options.</param>
		public List<object> DeserializeToObjectList (
			string jsonString,
			ObjectTypes restrictedTypes = ObjectTypes.JSON,
			Type[] customTypes = null,
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToObjectList (node, restrictedTypes, customTypes, options);
		}

		/// <summary>
		/// Deserializes a JSON node into a System.Object list. If no restrictions
		/// are given, the deserialized types can be doubles, booleans, strings, and arrays
		/// and dictionaries thereof. Restricted types can allow custom types to create
		/// classes or structs instead of dictionaries.
		/// </summary>
		/// <param name="node">JSON node to deserialize.</param>
		/// <param name="restrictedTypes">Restricted types for the object.</param>
		/// <param name="customTypes">Allowed custom types for the object. Restrictions
		/// must allow custom types if not <c>null</c>.</param>
		/// <param name="options">Deserialization options.</param>
		public List<object> DeserializeToObjectList (
			JSONNode node,
			ObjectTypes restrictedTypes = ObjectTypes.JSON,
			Type[] customTypes = null,
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			if (node.IsNull || node.Tag == JSONNodeType.None) {
				return null;
			}
			var list = new List<object> ();
			_FeedList (list, node, typeof(object), options, restrictedTypes, customTypes);
			return list;
		}

		/// <summary>
		/// Deserializes a JSON string into a generic dictionary.
		/// </summary>
		public Dictionary<K, V> DeserializeToDictionary<K, V> (
			string jsonString,
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToDictionary<K, V> (node, options);
		}

		/// <summary>
		/// Deserializes a JSON node into a generic dictionary.
		/// </summary>
		public Dictionary<K, V> DeserializeToDictionary<K, V> (
			JSONNode node,
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			if (node.IsNull || node.Tag == JSONNodeType.None) {
				return null;
			}
			var dictionary = new Dictionary<K, V> ();
			_FeedDictionary (dictionary, node, typeof(K), typeof(V), options);
			return dictionary;
		}

		/// <summary>
		/// Deserializes a JSON string into a dictionary with value type System.Object. If 
		/// no restrictions are given, the deserialized value types can be doubles, booleans, 
		/// strings, and arrays and dictionaries thereof. Restricted types can allow custom 
		/// types to create classes or structs instead of dictionaries.
		/// </summary>
		/// <param name="jsonString">JSON string to deserialize.</param>
		/// <param name="restrictedTypes">Restricted types for the values.</param>
		/// <param name="customTypes">Allowed custom types for the values. Restrictions
		/// must allow custom types if not <c>null</c>.</param>
		/// <param name="options">Deserialization options.</param>
		public Dictionary<K, object> DeserializeToObjectDictionary<K> (
			string jsonString,
			ObjectTypes restrictedTypes = ObjectTypes.JSON,
			Type[] customTypes = null,
			NodeOptions options = NodeOptions.Default)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return DeserializeToObjectDictionary<K> (
				node, 
				restrictedTypes, 
				customTypes, 
				options);
		}

		/// <summary>
		/// Deserializes a JSON node into a dictionary with value type System.Object. If 
		/// no restrictions are given, the deserialized value types can be doubles, booleans, 
		/// strings, and arrays and dictionaries thereof. Restricted types can allow custom 
		/// types to create classes or structs instead of dictionaries.
		/// </summary>
		/// <param name="node">JSON node to deserialize.</param>
		/// <param name="restrictedTypes">Restricted types for the values.</param>
		/// <param name="customTypes">Allowed custom types for the values. Restrictions
		/// must allow custom types if not <c>null</c>.</param>
		/// <param name="options">Deserialization options.</param>
		public Dictionary<K, object> DeserializeToObjectDictionary<K> (
			JSONNode node,
			ObjectTypes restrictedTypes = ObjectTypes.JSON,
			Type[] customTypes = null,
			NodeOptions options = NodeOptions.Default)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			if (node.IsNull || node.Tag == JSONNodeType.None) {
				return null;
			}
			var dictionary = new Dictionary<K, object> ();
			_FeedDictionary (
				dictionary, 
				node, 
				typeof(K), 
				typeof(object), 
				options, 
				restrictedTypes, 
				customTypes);
			return dictionary;
		}

		internal object Deserialize (
			JSONNode node, 
			Type targetType, 
			NodeOptions options, 
			ObjectTypes restrictedTypes,
			Type[] customTypes)
		{
			if (node.IsNull || node.Tag == JSONNodeType.None) {
				return null;
			}

			if (targetType == typeof(object)) {
				return _DeserializeToObject (node, options, restrictedTypes, customTypes);
			}

			if (targetType.IsValueType) {
				if (targetType.IsEnum) {
					return _DeserializeToEnum (node, targetType, options);
				} else if (targetType.IsPrimitive) {
					return _DeserializeToPrimitive (node, targetType, options);
				}
			} else {
				if (targetType == typeof(string)) {
					return _DeserializeToString (node, options);
				} else if (Nullable.GetUnderlyingType (targetType) != null) {
					return _DeserializeToNullable (node, targetType, options);
				} else if (typeof(IList).IsAssignableFrom (targetType)) {
					return _DeserializeToIList (node, targetType, options, restrictedTypes, customTypes);
				} else if (Util.IsDictionary (targetType)) {
					return _DeserializeToIDictionary (node, targetType, options, restrictedTypes, customTypes);
				}
			}
			return _DeserializeToCustom (node, targetType, options);
		}

		private object _Deserialize (
			JSONNode node,
			Type type,
			NodeOptions options, 
			MemberInfo memberInfo)
		{
			var typeAttribute = memberInfo == null 
				? null : Util.GetAttribute<RestrictTypeAttribute> (memberInfo);
			ObjectTypes types = typeAttribute == null ? ObjectTypes.JSON : typeAttribute.types;
			Type[] customTypes = typeAttribute == null ? null : typeAttribute.customTypes;
			return Deserialize (node, type, options, types, customTypes);
		}

		private object _DeserializeToObject (
			JSONNode node, 
			NodeOptions options, 
			ObjectTypes restrictedTypes,
			Type[] customTypes)
		{
			if (node.IsArray) {
				if (!restrictedTypes.SupportsArray ()) {
					return _HandleMismatch (options, "Arrays are not supported for object.");
				}
				return _DeserializeToArray (
					node, 
					typeof(object), 
					options,
					restrictedTypes,
					customTypes);
			} else if (node.IsBoolean) {
				if (!restrictedTypes.SupportsBool ()) {
					return _HandleMismatch (options, "Bools are not supported for object.");
				}
				return node.AsBool;
			} else if (node.IsNumber) {
				if (!restrictedTypes.SupportsNumber ()) {
					return _HandleMismatch (options, "Numbers are not supported for object.");
				}
				return node.AsDouble;
			} else if (node.IsObject) {
				if (restrictedTypes.SupportsCustom () && customTypes != null) {
					foreach (Type customType in customTypes) {
						try {
							var obj = Deserialize (node, customType, NodeOptions.Default);
							if (obj != null) {
								return obj;
							}
						} catch (Exception) {
						}
					}
				}

				if (!restrictedTypes.SupportsDictionary ()) {
					return _HandleMismatch (options, "Dictionaries are not supported for object.");
				}
				return _DeserializeToGenericDictionary (
					node, 
					typeof(string), 
					typeof(object), 
					options,
					restrictedTypes,
					customTypes);
			} else if (node.IsString) {
				if (!restrictedTypes.SupportsString ()) {
					return _HandleMismatch (options, "Strings are not supported for object.");
				}
				return _DeserializeToString (node, options);
			} else {
				return null;
			}
		}

		private object _DeserializeToNullable (JSONNode node, Type nullableType, NodeOptions options)
		{
			Type underlyingType = Nullable.GetUnderlyingType (nullableType);
			return Deserialize (node, underlyingType, options);
		}

		private object _DeserializeToPrimitive (JSONNode node, Type type, NodeOptions options)
		{
			if (type == typeof(int)) {
				return _DeserializeToInt (node, options);
			} else if (type == typeof(byte)) {
				return _DeserializeToByte (node, options);
			} else if (type == typeof(long)) {
				return _DeserializeToLong (node, options);
			} else if (type == typeof(uint)) {
				return _DeserializeToUInt (node, options);
			} else if (type == typeof(bool)) {
				return _DeserializeToBool (node, options);
			} else if (type == typeof(float)) {
				return _DeserializeToFloat (node, options);
			} else if (type == typeof(double)) {
				return _DeserializeToDouble (node, options);
			} else {
				return _HandleUnknown (options, "Unknown primitive type " + type);
			}
		}

		private string _DeserializeToString (JSONNode node, NodeOptions options)
		{
			if (!node.IsString) {
				return _HandleMismatch (options, "Expected string, found: " + node) as string;
			} else {
				return node.Value;
			}
		}

		private object _DeserializeToInt (JSONNode node, NodeOptions options)
		{
			if (node.IsNumber) {
				int value;
				if (int.TryParse (node.Value, out value)) {
					return value;
				}
			}
			return _HandleMismatch (options, "Expected integer, found " + node);
		}

		private object _DeserializeToUInt (JSONNode node, NodeOptions options)
		{
			if (node.IsNumber) {
				uint value;
				if (uint.TryParse (node.Value, out value)) {
					return value;
				}
			}
			return _HandleMismatch (options, "Expected unsigned integer, found " + node);
		}

		private object _DeserializeToByte (JSONNode node, NodeOptions options)
		{
			if (node.IsNumber) {
				byte value;
				if (byte.TryParse (node.Value, out value)) {
					return value;
				}
			}
			return _HandleMismatch (options, "Expected byte, found " + node);
		}

		private object _DeserializeToLong (JSONNode node, NodeOptions options)
		{
			if (node.IsNumber) {
				long value;
				if (long.TryParse (node.Value, out value)) {
					return value;
				}
			}
			return _HandleMismatch (options, "Expected long, found " + node);
		}

		private object _DeserializeToFloat (JSONNode node, NodeOptions options)
		{
			if (node.IsNumber) {
				return node.AsFloat;
			}
			return _HandleMismatch (options, "Expected float, found " + node);
		}

		private object _DeserializeToDouble (JSONNode node, NodeOptions options)
		{
			if (node.IsNumber) {
				return node.AsDouble;
			}
			return _HandleMismatch (options, "Expected double, found " + node);
		}

		private object _DeserializeToBool (JSONNode node, NodeOptions options)
		{
			if (node.IsBoolean) {
				return node.AsBool;
			}
			return _HandleMismatch (options, "Expected integer, found " + node);
		}

		private object _DeserializeToEnum (JSONNode node, Type enumType, NodeOptions options)
		{
			Func<object> handleError = () => _HandleMismatch (
				                           options, "Expected enum of type " + enumType + ", found: " + node);

			var enumAttribute = Util.GetAttribute<JSONEnumAttribute> (enumType);
			if (enumAttribute != null && enumAttribute.useIntegers && node.IsNumber) {
				try {
					return Enum.ToObject (enumType, _DeserializeToInt (node, options));
				} catch (Exception) {
				}
			} else if (node.IsString) {
				string value = node.Value;
				if (enumAttribute != null) {
					if (enumAttribute.prefix != null) {
						if (!value.StartsWith (enumAttribute.prefix)) {
							return handleError ();
						} else {
							value = value.Substring (enumAttribute.prefix.Length);
						}
					}
					if (enumAttribute.suffix != null) {
						if (!value.EndsWith (enumAttribute.suffix)) {
							return handleError ();
						} else {
							value = value.Substring (0, value.Length - enumAttribute.suffix.Length);
						}
					}
				}
				try {
					return Enum.Parse (enumType, value, true);
				} catch (Exception) {
				}
			}
			return handleError ();
		}

		private IDictionary _DeserializeToIDictionary (
			JSONNode node, 
			Type dictionaryType, 
			NodeOptions options,
			ObjectTypes types,
			Type[] customTypes)
		{
			Type genericType = dictionaryType.IsGenericType ? (dictionaryType.IsGenericTypeDefinition 
				? dictionaryType : dictionaryType.GetGenericTypeDefinition ()) : null;
			if (dictionaryType == typeof(IDictionary)) {
				return _DeserializeToGenericDictionary (
					node, 
					typeof(string), 
					typeof(object), 
					options,
					types,
					customTypes);
			} else if (genericType == typeof(IDictionary<,>) || genericType == typeof(Dictionary<,>)) {
				var args = dictionaryType.GetGenericArguments ();
				return _DeserializeToGenericDictionary (
					node, 
					args [0], 
					args [1], 
					options,
					types,
					customTypes);
			} else {
				return _HandleUnknown (options, "Unknown dictionary type " + dictionaryType) as IDictionary;
			}
		}

		private IList _DeserializeToIList (
			JSONNode node, 
			Type listType, 
			NodeOptions options,
			ObjectTypes types,
			Type[] customTypes)
		{
			Type genericType = listType.IsGenericType ? (listType.IsGenericTypeDefinition 
				? listType : listType.GetGenericTypeDefinition ()) : null;
			if (listType == typeof(Array)) {
				return _DeserializeToArray (
					node, 
					typeof(object), 
					options,
					types,
					customTypes);
			} else if (listType.IsArray) {
				return _DeserializeToArray (
					node, 
					listType.GetElementType (), 
					options,
					types,
					customTypes);
			} else if (listType == typeof(IList)) {
				return _DeserializeToGenericList (
					node, 
					typeof(object), 
					options,
					types,
					customTypes);
			} else if (genericType == typeof(IList<>) || genericType == typeof(List<>)) {
				return _DeserializeToGenericList (
					node, 
					listType.GetGenericArguments () [0], 
					options,
					types,
					customTypes);
			} else {
				return _HandleUnknown (options, "Unknown list type " + listType) as IList;
			}
		}

		private Array _DeserializeToArray (
			JSONNode node, 
			Type elementType, 
			NodeOptions options,
			ObjectTypes types,
			Type[] customTypes)
		{
			IList list = _DeserializeToGenericList (
				             node, 
				             elementType, 
				             options,
				             types,
				             customTypes);
			Array array = Array.CreateInstance (elementType, list.Count);
			list.CopyTo (array, 0);
			return array;
		}

		private IList _DeserializeToGenericList (
			JSONNode node, 
			Type genericArgument, 
			NodeOptions options,
			ObjectTypes types = ObjectTypes.JSON,
			Type[] customTypes = null)
		{
			IList list = (IList)Activator.CreateInstance (typeof(List<>).MakeGenericType (genericArgument));
			_FeedList (list, node, genericArgument, options, types, customTypes);
			return list;
		}

		private void _FeedList (
			IList list,
			JSONNode node, 
			Type genericArgument, 
			NodeOptions options,
			ObjectTypes types = ObjectTypes.JSON,
			Type[] customTypes = null)
		{
			if (node.IsArray) {
				JSONArray array = node as JSONArray;
				IEnumerator enumerator = array.GetEnumerator ();
				while (enumerator.MoveNext ()) {
					JSONNode child = (JSONNode)enumerator.Current;
					// Throws an error if needed.
					list.Add (Deserialize (
						child, 
						genericArgument, 
						options & ~NodeOptions.ReplaceDeserialized,
						types,
						customTypes));
				}
			} else {
				_HandleMismatch (options, "Expected an array, found " + node);
			}
		}

		private IDictionary _DeserializeToGenericDictionary (
			JSONNode node,
			Type keyType,
			Type valueType,
			NodeOptions options,
			ObjectTypes types = ObjectTypes.JSON,
			Type[] customTypes = null)
		{
			IDictionary dictionary = (IDictionary)Activator
				.CreateInstance (typeof(Dictionary<,>)
					.MakeGenericType (keyType, valueType));
			_FeedDictionary (dictionary, node, keyType, valueType, options, types, customTypes);
			return dictionary;
		}

		private void _FeedDictionary (
			IDictionary dictionary,
			JSONNode node,
			Type keyType,
			Type valueType,
			NodeOptions options,
			ObjectTypes types = ObjectTypes.JSON,
			Type[] customTypes = null)
		{
			if (node.IsObject) {
				JSONObject obj = node as JSONObject;
				IEnumerator enumerator = obj.GetEnumerator ();
				while (enumerator.MoveNext ()) {
					var pair = (KeyValuePair<string, JSONNode>)enumerator.Current;
					// Use default field options to throw at any error.
					object key = Deserialize (
						             new JSONString (pair.Key), 
						             keyType, 
						             NodeOptions.Default,
						             ObjectTypes.JSON,
						             null /* customTypes */);

					// Throws an error if needed.
					object value = Deserialize (
						               pair.Value, 
						               valueType, 
						               options & ~NodeOptions.ReplaceDeserialized,
						               types,
						               customTypes);
					dictionary.Add (key, value);
				}
			} else {
				_HandleMismatch (options, "Expected a dictionary, found " + node);
			}
		}

		private object _DeserializeToCustom (JSONNode node, Type targetType, NodeOptions options)
		{
			InstantiationData instantiationData = instantiater.Instantiate (
				                                      node,
				                                      targetType,
				                                      null /* referingType */,
				                                      options,
				                                      this);
				
			if (!instantiationData.needsDeserialization) {
				return instantiationData.instantiatedObject;
			}
			_DeserializeOn (
				instantiationData.instantiatedObject,
				node,
				options,
				instantiationData.ignoredKeys);
			return instantiationData.instantiatedObject;
		}

		private void _DeserializeOn (
			object obj, 
			JSONNode node,
			NodeOptions options,
			HashSet<string> ignoredKeys)
		{
			if (TryDeserializeOn (obj, node, options, ignoredKeys)) {
				return;
			}
			if (obj is IDeserializable) {
				(obj as IDeserializable).Deserialize (node, this);
				return;
			}
			_DeserializeByParts (obj, node, options, ignoredKeys);
		}

		private void _DeserializeByParts (
			object obj, 
			JSONNode node,
			NodeOptions options,
			HashSet<string> ignoredKeys)
		{
			var listener = obj as IDeserializationListener;
			if (listener != null) {
				listener.OnDeserializationWillBegin (this);
			}

			if (node.IsObject) {
				try {
					Type type = obj.GetType ();

					MemberInfo extrasMember = null;
					JSONExtrasAttribute extrasAttribute = null;
					Dictionary<string, object> extras = new Dictionary<string, object> ();

					var members = _GetDeserializedClassMembers (type, out extrasMember, out extrasAttribute);
					IEnumerator enumerator = (node as JSONObject).GetEnumerator ();

					var extrasTypeAttribute = extrasMember == null 
						? null : Util.GetAttribute<RestrictTypeAttribute> (extrasMember);
					ObjectTypes extrasTypes = extrasTypeAttribute == null 
						? ObjectTypes.JSON : extrasTypeAttribute.types;
					Type[] extrasCustomTypes = extrasTypeAttribute == null 
						? null : extrasTypeAttribute.customTypes;

					while (enumerator.MoveNext ()) {
						var pair = (KeyValuePair<string, JSONNode>)enumerator.Current;
						if (ignoredKeys.Contains (pair.Key)) {
							continue;
						}

						if (members.ContainsKey (pair.Key)) {
							_DeserializeClassMember (obj, members [pair.Key], pair.Value);
						} else {
							if (extrasMember != null) {
								extras.Add (pair.Key, _DeserializeToObject (
									pair.Value, 
									extrasAttribute.options,
									extrasTypes,
									extrasCustomTypes));
								continue;
							}

							var objectAttribute = Util.GetAttribute<JSONObjectAttribute> (type);
							if (objectAttribute == null || objectAttribute.options.ShouldThrowAtUnknownKey ()) {
								throw new DeserializationException ("The key " + pair.Key + " does not exist "
								+ "in class " + type);
							}
						}
					}

					if (extrasMember != null) {
						if (extras.Count != 0 || extrasAttribute.options.ShouldAssignNull ()) {
							Util.SetMemberValue (extrasMember, obj, extras);
						}
					}

					if (listener != null) {
						listener.OnDeserializationSucceeded (this);
					}
				} catch (Exception exception) {
					if (listener != null) {
						listener.OnDeserializationFailed (this);
					}
					throw exception;
				}
			} else if (node.IsNull || node.Tag == JSONNodeType.None) {
				if (listener != null) {
					listener.OnDeserializationSucceeded (this);
				}
			} else {
				if (listener != null) {
					listener.OnDeserializationFailed (this);
				}
				_HandleMismatch (options, "Expected a JSON object, found " + node);
			}
		}

		private void _DeserializeClassMember (
			object filledObject, 
			List<MemberInfo> memberInfos, 
			JSONNode node)
		{
			for (int i = 0; i < memberInfos.Count; i++) {
				var memberInfo = memberInfos [i];
				var fieldAttribute = Util.GetAttribute<JSONNodeAttribute> (memberInfo);
				var options = fieldAttribute != null ? fieldAttribute.options : NodeOptions.Default;

				try {
					Type type = Util.GetMemberType (memberInfo);
					if (node.IsObject
					    && !type.IsValueType
					    && !typeof(IDictionary).IsAssignableFrom (type)
					    && !options.ShouldReplaceWithDeserialized ()) {
						var value = Util.GetMemberValue (memberInfo, filledObject);
						if (value != null) {
							_DeserializeOn (value, node, options, new HashSet<string> ());
							return;
						}
					}

					object deserialized = _Deserialize (node, type, options, memberInfo);
					if (deserialized != null || options.ShouldAssignNull ()) {
						Util.SetMemberValue (memberInfo, filledObject, deserialized);
						return;
					}
				} catch (Exception ex) {
					if (i == memberInfos.Count - 1) {
						throw ex;
					}
				}
			}
		}

		private Dictionary<string, List<MemberInfo>> _GetDeserializedClassMembers (
			Type classType,
			out MemberInfo extrasMember,
			out JSONExtrasAttribute extrasAttribute)
		{
			JSONObjectAttribute objectAttribute = Util.GetAttribute<JSONObjectAttribute> (classType);
			Dictionary<string, List<MemberInfo>> members = new Dictionary<string, List<MemberInfo>> ();

			var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			if (objectAttribute != null) {
				if (!objectAttribute.options.ShouldIgnoreStatic ()) {
					flags |= BindingFlags.Static;
				}
				if (objectAttribute.options.ShouldUseTupleFormat ()) {
					throw new ArgumentException ("Cannot deserialize on a tuple formatted object.");
				}
			}

			extrasMember = null;
			extrasAttribute = null;

			foreach (var fieldInfo in classType.GetFields(flags)) {
				if (extrasMember == null) {
					if (Util.IsJSONExtrasMember (fieldInfo, out extrasAttribute)) {
						extrasMember = fieldInfo;
						continue;
					}
				}

				var fieldAttribute = Util.GetAttribute<JSONNodeAttribute> (fieldInfo);
				if (fieldAttribute != null && !fieldAttribute.options.IsDeserialized ()) {
					continue;
				} else if (!fieldInfo.IsLiteral && (fieldInfo.IsPublic || fieldAttribute != null)) {
					string key = (fieldAttribute != null && fieldAttribute.key != null) 
						? fieldAttribute.key : fieldInfo.Name;
					if (!members.ContainsKey (key)) {
						members [key] = new List<MemberInfo> ();
					}
					members [key].Add (fieldInfo);
				}
			}

			if (objectAttribute == null || !objectAttribute.options.ShouldIgnoreProperties ()) {
				foreach (var propertyInfo in classType.GetProperties(flags)) {
					if (extrasMember == null) {
						if (Util.IsJSONExtrasMember (propertyInfo, out extrasAttribute)) {
							extrasMember = propertyInfo;
							continue;
						}
					}

					var fieldAttribute = Util.GetAttribute<JSONNodeAttribute> (propertyInfo);
					if (fieldAttribute != null && !fieldAttribute.options.IsDeserialized ()) {
						continue;
					} else if (propertyInfo.GetIndexParameters ().Length == 0 && propertyInfo.CanWrite &&
					           (fieldAttribute != null || propertyInfo.GetSetMethod (false) != null)) {
						string key = (fieldAttribute != null && fieldAttribute.key != null) 
							? fieldAttribute.key : propertyInfo.Name;
						if (!members.ContainsKey (key)) {
							members [key] = new List<MemberInfo> ();
						}
						members [key].Add (propertyInfo);
					}
				}
			}

			return members;
		}

		private object _HandleMismatch (NodeOptions options, string message)
		{
			if (!options.ShouldIgnoreTypeMismatch ()) {
				throw new DeserializationException (message);
			} else {
				return null;
			}
		}

		private object _HandleUnknown (NodeOptions options, string message)
		{
			if (!options.ShouldIgnoreUnknownType ()) {
				throw new DeserializationException (message);
			} else {
				return null;
			}
		}
	}
}
