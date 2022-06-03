using SimpleJSON;
using System;
using System.Collections.Generic;

namespace UnityJSON
{
	/// <summary>
	/// Defines the JSON serialization and deserialization options for a 
	/// field or a property. If the field or the property does not have this
	/// attribute, the default options are used. For private fields and
	/// properties, this attribute is mandatory.
	/// </summary>
	[AttributeUsage (
		AttributeTargets.Field |
		AttributeTargets.Property |
		AttributeTargets.Parameter)]
	public class JSONNodeAttribute : Attribute
	{
		private NodeOptions _options;
		private string _key = null;

		public JSONNodeAttribute (NodeOptions options) : base ()
		{
			_options = options;
		}

		public JSONNodeAttribute () : base ()
		{
			_options = NodeOptions.Default;
		}

		/// <summary>
		/// The custom key value for this field/property when serializing
		/// or deserializing. If this value is null, the field/property
		/// name is used instead.
		/// </summary>
		public string key { 
			get { return _key; }
			set { _key = value == "" ? null : value; }
		}

		/// <summary>
		/// The serialization/deserialization options associated with this
		/// field/property.
		/// </summary>
		/// <value>The options.</value>
		public NodeOptions options {
			get { return _options; }
		}
	}

	/// <summary>
	/// Defines serialization/deserialization customization for enums.
	/// If no attribute is assigned to the enum, the names of the members
	/// are simply used as strings.
	/// </summary>
	[AttributeUsage (AttributeTargets.Enum)]
	public class JSONEnumAttribute : Attribute
	{
		private bool _useIntegers = false;
		private JSONEnumMemberFormating _format = JSONEnumMemberFormating.None;
		private string _prefix;
		private string _suffix;

		/// <summary>
		/// When <c>true</c> the numeric values of the enum members are used
		/// for serialization/deserialization. Defaults to false.
		/// </summary>
		public bool useIntegers {
			get { return _useIntegers; }
			set { _useIntegers = value; }
		}

		/// <summary>
		/// Applies a formatting to the member names of the enumeration before
		/// serializing/deserializing.
		/// </summary>
		public JSONEnumMemberFormating format {
			get { return _format; }
			set { _format = value; }
		}

		/// <summary>
		/// Applies a prefix to the member names of the enumeration before
		/// serializing/deserializing. The prefix is added after the
		/// formatting is applied.
		/// </summary>
		public string prefix {
			get { return _prefix; }
			set { _prefix = value == "" ? null : value; }
		}

		/// <summary>
		/// Applies a suffix to the member names of the enumeration before
		/// serializing/deserializing. The suffix is added after the
		/// formatting is applied.
		/// </summary>
		public string suffix {
			get { return _suffix; }
			set { _suffix = value == "" ? null : value; }
		}
	}

	/// <summary>
	/// Defines general serialization/deserialization options applied to the
	/// custom class or struct. These options are not node-specific and are
	/// applied every time that class/struct is used. If the class or the
	/// struct does not have this attribute, the default options are used.
	/// </summary>
	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Struct)]
	public class JSONObjectAttribute : Attribute
	{
		private ObjectOptions _options;

		public JSONObjectAttribute (ObjectOptions options) : base ()
		{
			_options = options;
		}

		public JSONObjectAttribute () : base ()
		{
			_options = ObjectOptions.Default;
		}

		/// <summary>
		/// The general serialization/deserialization options associated
		/// with this class/struct.
		/// </summary>
		public ObjectOptions options {
			get { return _options; }
		}
	}

	/// <summary>
	/// Defines the field/property where the unknown keys for a class or
	/// a struct can be deserialized. Can only be used together with a
	/// field or property of type Dictionary<string, object>. If there
	/// are multiple fields/properties with this attribute, only the first
	/// one is used.
	/// </summary>
	[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property)]
	public class JSONExtrasAttribute : Attribute
	{
		private NodeOptions _options;

		public JSONExtrasAttribute (NodeOptions options) : base ()
		{
			_options = options;
		}

		public JSONExtrasAttribute () : base ()
		{
			_options = NodeOptions.Default;
		}

		/// <summary>
		/// The serialization/deserialization options associated with
		/// this field/property.
		/// </summary>
		public NodeOptions options {
			get { return _options; }
		}
	}

	/// <summary>
	/// Restricts the deserialization types for a field/property of type
	/// object. This can also be used to add custom types to the deserialization
	/// process as per default only primitive types, and arrays and dictionaries
	/// thereof are created.
	/// 
	/// This attribute can be used with fields and properties of type object,
	/// list/array of objects or a dictionary with value type object. For dictionaries,
	/// the restriction is only applied to the value type.
	/// </summary>
	[AttributeUsage (
		AttributeTargets.Field |
		AttributeTargets.Property |
		AttributeTargets.Parameter)]
	public class RestrictTypeAttribute : Attribute
	{
		private ObjectTypes _types;
		private Type[] _customTypes;

		public RestrictTypeAttribute (ObjectTypes types) : base ()
		{
			_types = types;
		}

		public RestrictTypeAttribute (ObjectTypes types, params Type[] customTypes) : base ()
		{
			if (customTypes == null) {
				throw new ArgumentNullException ("customTypes");
			}
			_types = types;

			if (!_types.SupportsCustom ()) {
				throw new ArgumentException ("Attribute does not support custom types.");
			}

			List<Type> typeList = new List<Type> ();
			HashSet<Type> typeSet = new HashSet<Type> ();
			foreach (Type type in customTypes) {
				if (type != null
				    && !typeSet.Contains (type)
				    && Util.IsCustomType (type)) {
					typeSet.Add (type);
					typeList.Add (type);
				}
			}

			if (typeList.Count != 0) {
				_customTypes = typeList.ToArray ();
			}
		}

		/// <summary>
		/// The types that are allowed for this field/property.
		/// </summary>
		public ObjectTypes types {
			get { return _types; }
		}

		/// <summary>
		/// Custom types that can be deserialized for this object. ObjectTypes
		/// must allow custom types or an exception is thrown. The order of the
		/// types are important as they will be tried one by one by the
		/// deserializer.
		/// </summary>
		public Type[] customTypes {
			get { return _customTypes; }
		}
	}

	/// <summary>
	/// Adapts the instantiated class. If the node has the given 
	/// key / value pair, then the referenced type is instantiated.
	/// This can be used together with interfaces or abstract classes
	/// to determine the final class to be instantiated. One class or
	/// interface can have multiple conditional attributes. The conditions
	/// are proved in the order they are given and the type of the first 
	/// fulfilled condition is used.
	/// </summary>
	[AttributeUsage (
		AttributeTargets.Class |
		AttributeTargets.Interface, 
		AllowMultiple = true)]
	public class ConditionalInstantiationAttribute : Attribute
	{
		private Type _reference;
		private string _key;
		private object _value;

		public ConditionalInstantiationAttribute (
			Type reference, 
			string key, 
			object value) : base ()
		{
			if (reference == null) {
				throw new ArgumentNullException ("reference");
			}
			if (key == null) {
				throw new ArgumentNullException ("key");
			}
			if (value == null) {
				throw new ArgumentNullException ("value");
			}
			_reference = reference;
			_key = key;
			_value = value;
		}

		/// <summary>
		/// The reference type to be instantiated.
		/// </summary>
		public Type referenceType {
			get { return _reference; }
		}

		/// <summary>
		/// The key for the key / value condition pair.
		/// </summary>
		public string key {
			get { return _key; }
		}

		/// <summary>
		/// The value for the key / value condition pair.
		/// </summary>
		public object value {
			get { return _value; }
		}

		/// <summary>
		/// Removes the key / value pair if the condition is
		/// matched. This can be useful if the pair does not
		/// contain any data for the class / struct in order to
		/// prevent any unknown key errors.
		/// </summary>
		public bool ignoreConditionKey { get; set; }
	}

	/// <summary>
	/// Adapts the default type to be instantiated for this class
	/// or interface. This is checked after all of the conditional
	/// attributes (see ConditionalInstantiationAttribute) fail.
	/// </summary>
	[AttributeUsage (
		AttributeTargets.Class |
		AttributeTargets.Interface)]
	public class DefaultInstantiationAttribute : Attribute
	{
		private Type _reference;

		public DefaultInstantiationAttribute (Type reference) : base ()
		{
			if (reference == null) {
				throw new ArgumentNullException ("reference");
			}
			_reference = reference;
		}

		/// <summary>
		/// The reference type to be instantiated.
		/// </summary>
		public Type referenceType {
			get { return _reference; }
		}
	}

	/// <summary>
	/// Marks a constructor that is used to deserialize objects.
	/// Every class can use this attribute maximum one times. The
	/// parameters can have JSONNodeAttributes.
	/// </summary>
	[AttributeUsage (AttributeTargets.Constructor)]
	public class JSONConstructorAttribute : Attribute
	{
		public JSONConstructorAttribute () : base ()
		{
		}
	}
}
