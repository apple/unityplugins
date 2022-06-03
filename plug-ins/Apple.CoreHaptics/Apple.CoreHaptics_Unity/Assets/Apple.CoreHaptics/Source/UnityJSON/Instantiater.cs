using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityJSON
{
	/// <summary>
	/// An exception during the instantiation process.
	/// </summary>
	public class InstantiationException : Exception
	{
		public InstantiationException () : base ()
		{
		}

		public InstantiationException (string message) : base (message)
		{
		}
	}

	/// <summary>
	/// The data associated with an instantiated object. Other than
	/// the object itself, holds reference to additional information
	/// regarding the upcoming deserialization process.
	/// </summary>
	public struct InstantiationData
	{
		/// <summary>
		/// An instantiated data representing a null object.
		/// </summary>
		public static readonly InstantiationData Null = new InstantiationData ();

		/// <summary>
		/// The instantiated object that will be deserialized.
		/// </summary>
		public object instantiatedObject { 
			get { return _instantiatedObject; }
			set { _instantiatedObject = value; }
		}

		/// <summary>
		/// Whether deserialization is necessary. If the instantiated
		/// object is <c>null</c> this value always returns <c>false</c>.
		/// </summary>
		public bool needsDeserialization { 
			get { return _instantiatedObject != null && _needsDeserialization; }
			set { _needsDeserialization = value; }
		}

		/// <summary>
		/// The keys that should be ignored during the deserialization.
		/// These keys for instance can be for instance used for instantiation
		/// with a constructor.
		/// </summary>
		public HashSet<string> ignoredKeys {
			get {
				if (_ignoredKeys == null) {
					_ignoredKeys = new HashSet<string> ();
				}
				return _ignoredKeys;
			}
			set { _ignoredKeys = value; }
		}

		private object _instantiatedObject;
		private bool _needsDeserialization;
		private HashSet<string> _ignoredKeys;
	}

	/// <summary>
	/// Instantiates an instance of a type.
	/// </summary>
	public class Instantiater
	{
		public static readonly Instantiater Default = new Instantiater ();

		protected Instantiater ()
		{
		}

		/// <summary>
		/// Instantiates an instance of a type. First, TryInstantiate method is
		/// called for custom instantiation. If that fails, then the class is queried for
		/// conditional and default substitute types. If no such types exist or
		/// match, then an object of the given type is created directly. If the 
		/// class or the struct has a constructor with the attribute 
		/// JSONConstructorAttribute, then that constructor is used, otherwise
		/// the default constructor is used.
		/// 
		/// If the instantiated object is null, then InstantiationData.Null is
		/// returned.
		/// </summary>
		/// <param name="node">JSON node to create the instance from. This
		/// might be used to decide the type of the object or for the arguments
		/// of a constructor.</param>
		/// <param name="targetType">Target type of the object.</param>
		/// <param name="referingType">Refering type that is only set if this
		/// is a substitute type for another type.</param>
		/// <param name="options">Instantiation options.</param>
		/// <param name="deserializer">Deserializer to use for the instantiation.</param>
		public InstantiationData Instantiate (
			JSONNode node, 
			Type targetType,
			Type referingType = null,
			NodeOptions options = NodeOptions.Default,
			Deserializer deserializer = null)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			if (targetType == null) {
				throw new ArgumentNullException ("targetType");
			}
			if (deserializer == null) {
				deserializer = Deserializer.Default;
			}
			return _Instantiate (node, targetType, referingType, options, deserializer);
		}

		/// <summary>
		/// Instantiates an instance of a type. First, TryInstantiate method is
		/// called for custom instantiation. If that fails, then the class is queried for
		/// conditional and default substitute types. If no such types exist or
		/// match, then an object of the given type is created directly. If the 
		/// class or the struct has a constructor with the attribute 
		/// JSONConstructorAttribute, then that constructor is used, otherwise
		/// the default constructor is used.
		/// 
		/// If the instantiated object is null, then InstantiationData.Null is
		/// returned.
		/// </summary>
		/// <param name="jsonString">JSON string to create the instance from. This
		/// might be used to decide the type of the object or for the arguments
		/// of a constructor.</param>
		/// <param name="targetType">Target type of the object.</param>
		/// <param name="referingType">Refering type that is only set if this
		/// is a substitute type for another type.</param>
		/// <param name="options">Instantiation options.</param>
		/// <param name="deserializer">Deserializer to use for the instantiation.</param>
		public InstantiationData Instantiate (
			string jsonString, 
			Type targetType,
			Type referingType = null,
			NodeOptions options = NodeOptions.Default,
			Deserializer deserializer = null)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return Instantiate (node, targetType, referingType, options, deserializer);
		}

		/// <summary>
		/// Instantiates an object with a suitable constructor. If the 
		/// class or the struct has a constructor with the attribute 
		/// JSONConstructorAttribute, then that constructor is used, otherwise
		/// the default constructor is used. 
		/// 
		/// If the instantiated object is null, then InstantiationData.Null is
		/// returned.
		/// </summary>
		/// <param name="jsonString">JSON string to create the instance from. This
		/// might be used for the arguments of the constructor.</param>
		/// <param name="targetType">Target type.</param>
		/// <param name="options">Options.</param>
		/// <param name="deserializer">Deserializer.</param>
		public InstantiationData InstantiateWithConstructor (
			string jsonString, 
			Type targetType,
			NodeOptions options = NodeOptions.Default,
			Deserializer deserializer = null)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			return InstantiateWithConstructor (node, targetType, options, deserializer);
		}

		/// <summary>
		/// Instantiates an object with a suitable constructor. If the 
		/// class or the struct has a constructor with the attribute 
		/// JSONConstructorAttribute, then that constructor is used, otherwise
		/// the default constructor is used. 
		/// 
		/// If the instantiated object is null, then InstantiationData.Null is
		/// returned.
		/// </summary>
		/// <param name="node">JSON node to create the instance from. This
		/// might be used for the arguments of the constructor.</param>
		/// <param name="targetType">Target type.</param>
		/// <param name="options">Options.</param>
		/// <param name="deserializer">Deserializer.</param>
		public InstantiationData InstantiateWithConstructor (
			JSONNode node, 
			Type targetType,
			NodeOptions options = NodeOptions.Default,
			Deserializer deserializer = null)
		{
			if (node == null) {
				throw new ArgumentNullException ("node");
			}
			if (targetType == null) {
				throw new ArgumentNullException ("targetType");
			}
			if (deserializer == null) {
				deserializer = Deserializer.Default;
			}

			return _InstantiateWithConstructor (node, targetType, options, deserializer);
		}

		/// <summary>
		/// Tries to instantiate an object of a specific type. This performs
		/// application specific instantiation and subclasses should override
		/// this method to perform their own logic. If the instantiated object is
		/// null, then InstantiationData.Null should be returned.
		/// </summary>
		/// <param name="node">JSON node to create the instance from. This
		/// might be used to decide the type of the object or for the arguments
		/// of a constructor.</param>
		/// <param name="targetType">Target type of the object.</param>
		/// <param name="referingType">Refering type that is only set if this
		/// is a substitute type for another type.</param>
		/// <param name="options">Instantiation options.</param>
		/// <param name="deserializer">Deserializer to use for the instantiation.</param>
		protected virtual bool TryInstantiate (
			JSONNode node,
			Type targetType,
			Type referingType,
			NodeOptions options,
			Deserializer deserializer,
			out InstantiationData instantiationData)
		{
			instantiationData = InstantiationData.Null;
			return false;
		}

		private InstantiationData _Instantiate (
			JSONNode node, 
			Type targetType,
			Type referingType,
			NodeOptions options,
			Deserializer deserializer)
		{
			InstantiationData instantiationData;
			if (TryInstantiate (
				    node, 
				    targetType,
				    referingType,
				    options, 
				    deserializer, 
				    out instantiationData)) {
				return instantiationData;
			}

			if (node.IsNull || node.Tag == JSONNodeType.None) {
				return InstantiationData.Null;
			}

			if (referingType != targetType) {
				if (node.IsObject) {
					var conditionalAttributes = targetType
					.GetCustomAttributes (typeof(ConditionalInstantiationAttribute), false);
					foreach (object attribute in conditionalAttributes) {
						var condition = attribute as ConditionalInstantiationAttribute;
						if (Equals (node [condition.key].Value, condition.value.ToString ())) {
							instantiationData = _Instantiate (
								node, 
								condition.referenceType,
								targetType,
								options,
								deserializer);
							if (condition.ignoreConditionKey) {
								instantiationData.ignoredKeys = new HashSet<string> () { condition.key };
							}
							return instantiationData;
						}
					}
				}

				var defaultAttribute = Util.GetAttribute<DefaultInstantiationAttribute> (targetType);
				if (defaultAttribute != null) {
					return _Instantiate (
						node, 
						defaultAttribute.referenceType,
						targetType,
						options,
						deserializer);
				}
			}

			return _InstantiateWithConstructor (
				node,
				targetType,
				options,
				deserializer);
		}

		private InstantiationData _InstantiateWithConstructor (
			JSONNode node, 
			Type targetType,
			NodeOptions options,
			Deserializer deserializer)
		{
			if (node.IsNull || node.Tag == JSONNodeType.None) {
				return InstantiationData.Null;
			}

			JSONObjectAttribute objectAttribute = Util.GetAttribute<JSONObjectAttribute> (targetType);
			bool useTupleFormat = objectAttribute != null 
				? objectAttribute.options.ShouldUseTupleFormat () : false;
			if (useTupleFormat && !node.IsArray) {
				throw new InstantiationException ("Expected JSON array, found " + node.Tag);
			} else if (!useTupleFormat && !node.IsObject) {
				throw new InstantiationException ("Expected JSON object, found " + node.Tag);
			}

			ConstructorInfo[] constructors = targetType.GetConstructors (
				                                 BindingFlags.Instance |
				                                 BindingFlags.Public |
				                                 BindingFlags.NonPublic);
			foreach (ConstructorInfo constructorInfo in constructors) {
				var constructorAttribute = Util.GetAttribute<JSONConstructorAttribute> (constructorInfo);
				if (constructorAttribute != null) {
					return _InstantiateWithConstructor (node, constructorInfo, deserializer, useTupleFormat);
				}
			}

			try {
				InstantiationData instantiationData = new InstantiationData ();
				instantiationData.instantiatedObject = Activator.CreateInstance (targetType);
				instantiationData.needsDeserialization = node.Count != 0;
				return instantiationData;
			} catch (Exception) {
				return _HandleError (options, "Type " + targetType
				+ " does not have a suitable constructor.");
			}
		}

		private InstantiationData _InstantiateWithConstructor (
			JSONNode node,
			ConstructorInfo constructorInfo,
			Deserializer deserializer,
			bool useTupleFormat)
		{
			ParameterInfo[] parameters = constructorInfo.GetParameters ();
			object[] parameterValues = new object[parameters.Length];
			HashSet<string> ignoredKeys = new HashSet<string> ();

			for (int i = 0; i < parameterValues.Length; i++) {
				var nodeAttribute = Util.GetAttribute<JSONNodeAttribute> (parameters [i]);
				var restrictAttribute = Util.GetAttribute<RestrictTypeAttribute> (parameters [i]);

				string key = nodeAttribute != null && nodeAttribute.key != null
					? nodeAttribute.key : parameters [i].Name;
				JSONNode parameterNode = useTupleFormat ? node [i] : node [key];

				ObjectTypes restrictedTypes = restrictAttribute == null 
					? ObjectTypes.JSON : restrictAttribute.types;
				Type[] customTypes = restrictAttribute == null ? null : restrictAttribute.customTypes;

				parameterValues [i] = deserializer.Deserialize (
					parameterNode,
					parameters [i].ParameterType,
					nodeAttribute == null ? NodeOptions.Default : nodeAttribute.options,
					restrictedTypes,
					customTypes);

				if (!useTupleFormat) {
					ignoredKeys.Add (key);
				}
			}

			InstantiationData instantiationData = new InstantiationData ();
			instantiationData.instantiatedObject = constructorInfo.Invoke (parameterValues);
			instantiationData.needsDeserialization = !useTupleFormat && ignoredKeys.Count != node.Count;
			instantiationData.ignoredKeys = ignoredKeys;
			return instantiationData;
		}

		private InstantiationData _HandleError (NodeOptions options, string message)
		{
			if (options.ShouldIgnoreUnknownType ()) {
				return InstantiationData.Null;
			} else {
				throw new InstantiationException (message);
			}
		}
	}
}
