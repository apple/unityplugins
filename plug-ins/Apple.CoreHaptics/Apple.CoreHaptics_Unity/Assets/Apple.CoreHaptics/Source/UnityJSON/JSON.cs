using System;
using System.Collections;

namespace UnityJSON
{
	/// <summary>
	/// Provides high-level public API for the serialization and deserialization.
	/// </summary>
	public static class JSON
	{
		/// <summary>
		/// Serializes the given object into JSON string. Throws an
		/// error if the object is null.
		/// </summary>
		/// <param name="obj">Object to be serialized.</param>
		/// <param name="options">Serialization options (optional).</param>
		/// <param name="serializer">Custom serializer. If not given, 
		/// the default serializer is used.</param>
		public static string Serialize (
			object obj, 
			NodeOptions options = NodeOptions.Default,
			Serializer serializer = null)
		{
			if (obj == null) {
				throw new ArgumentNullException ("obj");
			}
			if (serializer == null) {
				serializer = Serializer.Default;
			}
			return serializer.Serialize (obj, options);
		}

		/// <summary>
		/// Serializes the given object into JSON string. Throws an
		/// error if the object is null.
		/// </summary>
		/// <param name="obj">Object to be serialized.</param>
		/// <param name="serializer">Custom serializer. Throws an error if
		/// <c>null</c>.</param>
		public static string Serialize (
			object obj, 
			Serializer serializer)
		{
			if (obj == null) {
				throw new ArgumentNullException ("obj");
			}
			if (serializer == null) {
				throw new ArgumentNullException ("serializer");
			}
			return serializer.Serialize (obj);
		}

		/// <summary>
		/// Serializes the object into JSON string.
		/// </summary>
		/// <param name="obj">Object to be serialized.</param>
		/// <param name="options">Serialization options (optional).</param>
		/// <param name="serializer">Custom serializer. If not given, 
		/// the default serializer is used.</param>
		public static string ToJSONString (
			this object obj, 
			NodeOptions options = NodeOptions.Default,
			Serializer serializer = null)
		{
			return Serialize (obj, options, serializer);
		}

		/// <summary>
		/// Serializes the object into JSON string.
		/// </summary>
		/// <param name="obj">Object to be serialized.</param>
		/// <param name="serializer">Custom serializer. Throws an error if
		/// <c>null</c>.</param>
		public static string ToJSONString (
			this object obj, 
			Serializer serializer)
		{
			return Serialize (obj, serializer);
		}

		/// <summary>
		/// Deserializes an object of the generic type from
		/// the given JSON string. Throws an exception if the string
		/// is null or not a valid JSON string.
		/// </summary>
		/// <param name="jsonString">The JSON string to deserialize from.</param>
		/// <param name="options">Deserialization options (optional).</param>
		/// <param name="deserializer">Custom deserializer. If not given, 
		/// the default deserializer is used.</param>
		/// <typeparam name="T">The type of object to deserialize.</typeparam>
		public static T Deserialize<T> (
			string jsonString,
			NodeOptions options = NodeOptions.Default,
			Deserializer deserializer = null)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			if (deserializer == null) {
				deserializer = Deserializer.Default;
			}

			SimpleJSON.JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			if (node == null) {
				throw new ArgumentException ("Argument is not a valid JSON string: " + jsonString);
			}
			return (T)deserializer.Deserialize (node, typeof(T), options);
		}

		/// <summary>
		/// Deserializes an object of the generic type from
		/// the given JSON string. Throws an exception if the string
		/// is null or not a valid JSON string.
		/// </summary>
		/// <param name="jsonString">The JSON string to deserialize from.</param>
		/// <param name="deserializer">Custom deserializer. Throws an error if
		/// <c>null</c>.</param>
		/// <typeparam name="T">The type of object to deserialize.</typeparam>
		public static T Deserialize<T> (
			string jsonString,
			Deserializer deserializer)
		{
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			if (deserializer == null) {
				throw new ArgumentNullException ("deserializer");
			}

			SimpleJSON.JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			if (node == null) {
				throw new ArgumentException ("Argument is not a valid JSON string: " + jsonString);
			}
			return (T)deserializer.Deserialize (node, typeof(T));
		}

		/// <summary>
		/// Deserializes a JSON string on a previously existing object.
		/// Throws an exception if the string is null or not a valid JSON string.
		/// </summary>
		/// <param name="obj">Object to deserialize on.</param>
		/// <param name="jsonString">The JSON string to deserialize from.</param>
		/// <param name="options">Deserialization options (optional).</param>
		/// <param name="deserializer">Custom deserializer. If not given, 
		/// the default deserializer is used.</param>
		public static void DeserializeOn (
			object obj, 
			string jsonString,
			NodeOptions options = NodeOptions.Default, 
			Deserializer deserializer = null)
		{
			if (obj == null) {
				throw new ArgumentNullException ("obj");
			}
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			if (deserializer == null) {
				deserializer = Deserializer.Default;
			}

			SimpleJSON.JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			if (node == null) {
				throw new ArgumentException ("Argument is not a valid JSON string: " + jsonString);
			}
			deserializer.DeserializeOn (obj, node, options);
		}

		/// <summary>
		/// Deserializes a JSON string on a previously existing object.
		/// Throws an exception if the string is null or not a valid JSON string.
		/// </summary>
		/// <param name="obj">Object to deserialize on.</param>
		/// <param name="jsonString">The JSON string to deserialize from.</param>
		/// <param name="deserializer">Custom deserializer. Throws an error if
		/// <c>null</c>.</param>
		public static void DeserializeOn (
			object obj, 
			string jsonString, 
			Deserializer deserializer)
		{
			if (obj == null) {
				throw new ArgumentNullException ("obj");
			}
			if (jsonString == null) {
				throw new ArgumentNullException ("jsonString");
			}
			if (deserializer == null) {
				throw new ArgumentNullException ("deserializer");
			}

			SimpleJSON.JSONNode node = SimpleJSON.JSON.Parse (jsonString);
			if (node == null) {
				throw new ArgumentException ("Argument is not a valid JSON string: " + jsonString);
			}
			deserializer.DeserializeOn (obj, node);
		}

		/// <summary>
		/// Deserializes a JSON string on the previously existing object.
		/// Throws an exception if the string is null or not a valid JSON string.
		/// </summary>
		/// <param name="obj">Object to deserialize on.</param>
		/// <param name="jsonString">The JSON string to deserialize from.</param>
		/// <param name="options">Deserialization options (optional).</param>
		/// <param name="deserializer">Custom deserializer. If not given, 
		/// the default deserializer is used.</param>
		public static void FeedJSON (
			this object obj, 
			string jsonString,
			NodeOptions options = NodeOptions.Default,
			Deserializer deserializer = null)
		{
			DeserializeOn (obj, jsonString, options, deserializer);
		}

		/// <summary>
		/// Deserializes a JSON string on the previously existing object.
		/// Throws an exception if the string is null or not a valid JSON string.
		/// </summary>
		/// <param name="obj">Object to deserialize on.</param>
		/// <param name="jsonString">The JSON string to deserialize from.</param>
		/// <param name="deserializer">Custom deserializer. Throws an error if
		/// <c>null</c>.</param>
		public static void FeedJSON (
			this object obj, 
			string jsonString,
			Deserializer deserializer)
		{
			DeserializeOn (obj, jsonString, deserializer);
		}
	}
}
