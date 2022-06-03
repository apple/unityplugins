namespace UnityJSON
{
	/// <summary>
	/// Serialization/deserialization options for a single
	/// JSON node. These can be defined for the fields and properties
	/// of a class with the use of the JSONNodeAttribute. If no
	/// attribute is given, the default options are used.
	/// </summary>
	public enum NodeOptions
	{
		/// <summary>
		/// Default options for the serialization and deserialization
		/// of this node which disables all the other node options.
		/// </summary>
		Default = 0,

		/// <summary>
		/// If <c>true</c> the field or the property is not serialized.
		/// This can be used with public fields / properties to
		/// ignore them during serialization.
		/// </summary>
		DontSerialize = 1,

		/// <summary>
		/// If <c>true</c> the field or the property is not deserialized.
		/// This can be used with public fields / properties to
		/// ignore them during deserialization.
		/// </summary>
		DontDeserialize = 1 << 1,

		/// <summary>
		/// Per default, if a field or a property has the value <c>null</c>
		/// then it is ignored at serialization. When used, this option
		/// forces the field to be serialized as either null or undefined
		/// (see Serializer.useUndefinedForNull).
		/// </summary>
		SerializeNull = 1 << 2,

		/// <summary>
		/// Per default if the JSON data does not match the C# type, the
		/// deserializer throws an error. When used, this forces the deserializer
		/// to ignore this error and simply return null from the deserialization.
		/// </summary>
		IgnoreTypeMismatch = 1 << 3,

		/// <summary>
		/// If the C# type cannot be instantiated, then an InstantiationException is thrown.
		/// This can for instance happen for classes with custom constructors
		/// that are not defined in the deserializer. When used, this option
		/// forces the deserializer to ignore this error and simply return null 
		/// from the deserialization.
		/// </summary>
		IgnoreInstantiationError = 1 << 4,

		/// <summary>
		/// Ignore both the type mismatch and instantiation errors for
		/// deserialization.
		/// </summary>
		IgnoreDeserializationTypeErrors = IgnoreTypeMismatch | IgnoreInstantiationError,

		/// <summary>
		/// When deserializing a JSON string, the null or undefined values are
		/// not assigned and simply ignored. When used, this option makes sure
		/// that the null values are applied to this field or property. If this
		/// has a primitive type, then the default value (0 for integer, false
		/// for boolean etc) is assigned.
		/// </summary>
		DontAssignNull = 1 << 5,

		/// <summary>
		/// By default, the custom classes (all except string, nullables and
		/// enumerables) are not assigned anew but rather reused. For instance,
		/// assume we have classes A and B, the class A has field classB of
		/// type B, class B has a field intField of type int. When deserializing A, 
		/// the deserializer by default sets the value of intField directly instead
		/// of creating a new instance of class B with the new intField value. When
		/// ReplaceDeserialized option is used on the field classB, the deserializer
		/// creates a new instance of class B and assigns it directly instead of
		/// working with the current value.
		/// </summary>
		ReplaceDeserialized = 1 << 6
	}

	/// <summary>
	/// General serialization/deserialization options assigned to
	/// all instances of a class or a struct. You can specify these options
	/// by using a JSONObjectAttribute.
	/// </summary>
	public enum ObjectOptions
	{
		/// <summary>
		/// Default options for the serialization and deserialization
		/// of this class or struct which disables all the other object options.
		/// </summary>
		Default = 0,

		/// <summary>
		/// Ignores the properties when serializing or deserializing. This
		/// can also be used to simply increase the performance if no properties
		/// are used for the serialization / deserialization.
		/// </summary>
		IgnoreProperties = 1,

		/// <summary>
		/// Per default, the static fields and properties are ignored for
		/// the serialization and deserialization. When used, all public
		/// and non-public static fields and properties are considered for
		/// serialization and deserialization just like the non-static ones.
		/// </summary>
		IncludeStatic = 1 << 1,

		/// <summary>
		/// When deserializing the JSON, there can be unknown nodes that
		/// do not match the class or struct definition. If there also isn't
		/// any field / property with JSONExtrasAttribute to collect the unknown
		/// keys, then an exception is thrown. This option prevents the
		/// deserializer from throwing that exception.
		/// </summary>
		IgnoreUnknownKey = 1 << 2,

		/// <summary>
		/// The class or the struct is handled as a tuple (JSON array) rather
		/// than a dictionary. This automatically ignores properties both for
		/// serialization and deserialization. For serialization, the fields are
		/// serialized in the order they are defined in an array without keys.
		/// As for deserialization, the elements are passed to the constructor
		/// in the order they are defined. Deserialization does not take place
		/// for other fields and properties.
		/// 
		/// Tuple formatted classes and structs do not support JSON extras.
		/// </summary>
		TupleFormat = 1 << 3 | IgnoreProperties,
	}

	/// <summary>
	/// The types that are supported for deserialization for a general
	/// System.Object type.
	/// </summary>
	public enum ObjectTypes
	{
		/// <summary>
		/// Default value simply deserializes the JSON objects into
		/// their C# counterparts as in strings, booleans, doubles
		/// and arrays and dictionaries thereof.
		/// </summary>
		JSON = String | Bool | Number | Array | Dictionary,

		/// <summary>
		/// Allows all types including custom types.
		/// </summary>
		All = JSON | Custom,

		/// <summary>
		/// Allows strings.
		/// </summary>
		String = 1,

		/// <summary>
		/// Allows booleans.
		/// </summary>
		Bool = 1 << 1,

		/// <summary>
		/// Allows numbers in form of doubles.
		/// </summary>
		Number = 1 << 2,

		/// <summary>
		/// Allows arrays of the supported
		/// object types.
		/// </summary>
		Array = 1 << 3,

		/// <summary>
		/// Allows dictionaries with string keys
		/// and values of supported object types.
		/// </summary>
		Dictionary = 1 << 4,

		/// <summary>
		/// Allows custom types. These must be defined
		/// in a separate type array.
		/// </summary>
		Custom = 1 << 5
	}

	/// <summary>
	/// Formatting to be applied to enum members before
	/// serializing or deserializing them.
	/// </summary>
	public enum JSONEnumMemberFormating
	{
		/// <summary>
		/// No formatting is applied.
		/// </summary>
		None,

		/// <summary>
		/// The member name is lowecased.
		/// </summary>
		Lowercased,

		/// <summary>
		/// The member name is uppercased.
		/// </summary>
		Uppercased,

		/// <summary>
		/// The member name is capitalized with the
		/// only first letter capital.
		/// </summary>
		Capitalized
	}

	/// <summary>
	/// Provides helper methods to query options.
	/// </summary>
	public static class OptionsExtensions
	{
		/// <summary>
		/// Returns <c>true</c> if NodeOptions.DontSerialize is not set.
		/// </summary>
		public static bool IsSerialized (this NodeOptions options)
		{
			return (options & NodeOptions.DontSerialize) == 0;
		}

		/// <summary>
		/// Returns <c>true</c> if NodeOptions.DontDeserialize is not set.
		/// </summary>
		public static bool IsDeserialized (this NodeOptions options)
		{
			return (options & NodeOptions.DontDeserialize) == 0;
		}

		/// <summary>
		/// Returns <c>true</c> if NodeOptions.ShouldSerializeNull is set.
		/// </summary>
		public static bool ShouldSerializeNull (this NodeOptions options)
		{
			return (options & NodeOptions.SerializeNull) == NodeOptions.SerializeNull;
		}

		/// <summary>
		/// Returns <c>true</c> if NodeOptions.IgnoreTypeMismatch is set.
		/// </summary>
		public static bool ShouldIgnoreTypeMismatch (this NodeOptions options)
		{
			return (options & NodeOptions.IgnoreTypeMismatch) == NodeOptions.IgnoreTypeMismatch;
		}

		/// <summary>
		/// Returns <c>true</c> if NodeOptions.IgnoreUnknownType is set.
		/// </summary>
		public static bool ShouldIgnoreUnknownType (this NodeOptions options)
		{
			return (options & NodeOptions.IgnoreInstantiationError) == NodeOptions.IgnoreInstantiationError;
		}

		/// <summary>
		/// Returns <c>true</c> if NodeOptions.DontAssignNull is not set.
		/// </summary>
		public static bool ShouldAssignNull (this NodeOptions options)
		{
			return (options & NodeOptions.DontAssignNull) == 0;
		}

		/// <summary>
		/// Returns <c>true</c> if NodeOptions.ReplaceDeserialized is set.
		/// </summary>
		public static bool ShouldReplaceWithDeserialized (this NodeOptions options)
		{
			return (options & NodeOptions.ReplaceDeserialized) == NodeOptions.ReplaceDeserialized;
		}

		/// <summary>
		/// Returns <c>true</c> if ObjectOptions.IgnoreProperties is set.
		/// </summary>
		public static bool ShouldIgnoreProperties (this ObjectOptions options)
		{
			return (options & ObjectOptions.IgnoreProperties) == ObjectOptions.IgnoreProperties;
		}

		/// <summary>
		/// Returns <c>true</c> if ObjectOptions.IncludeStatic is not set.
		/// </summary>
		public static bool ShouldIgnoreStatic (this ObjectOptions options)
		{
			return (options & ObjectOptions.IncludeStatic) == 0;
		}

		/// <summary>
		/// Returns <c>true</c> if ObjectOptions.IgnoreUnknownKey is not set.
		/// </summary>
		public static bool ShouldThrowAtUnknownKey (this ObjectOptions options)
		{
			return (options & ObjectOptions.IgnoreUnknownKey) == 0;
		}

		/// <summary>
		/// Returns <c>true</c> if ObjectOptions.TupleFormat is not set.
		/// </summary>
		public static bool ShouldUseTupleFormat (this ObjectOptions options)
		{
			return (options & ObjectOptions.TupleFormat) == ObjectOptions.TupleFormat;
		}

		/// <summary>
		/// Returns <c>true</c> if ObjectTypes.String is set.
		/// </summary>
		public static bool SupportsString (this ObjectTypes types)
		{
			return (types & ObjectTypes.String) == ObjectTypes.String;
		}

		/// <summary>
		/// Returns <c>true</c> if ObjectTypes.Bool is set.
		/// </summary>
		public static bool SupportsBool (this ObjectTypes types)
		{
			return (types & ObjectTypes.Bool) == ObjectTypes.Bool;
		}

		/// <summary>
		/// Returns <c>true</c> if ObjectTypes.Number is set.
		/// </summary>
		public static bool SupportsNumber (this ObjectTypes types)
		{
			return (types & ObjectTypes.Number) == ObjectTypes.Number;
		}

		/// <summary>
		/// Returns <c>true</c> if ObjectTypes.Array is set.
		/// </summary>
		public static bool SupportsArray (this ObjectTypes types)
		{
			return (types & ObjectTypes.Array) == ObjectTypes.Array;
		}

		/// <summary>
		/// Returns <c>true</c> if ObjectTypes.Dictionary is set.
		/// </summary>
		public static bool SupportsDictionary (this ObjectTypes types)
		{
			return (types & ObjectTypes.Dictionary) == ObjectTypes.Dictionary;
		}

		/// <summary>
		/// Returns <c>true</c> if ObjectTypes.Custom is set.
		/// </summary>
		public static bool SupportsCustom (this ObjectTypes types)
		{
			return (types & ObjectTypes.Custom) == ObjectTypes.Custom;
		}
	}
}
