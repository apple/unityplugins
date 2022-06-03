using SimpleJSON;
using System;
using System.Collections.Generic;

namespace UnityJSON
{
	/// <summary>
	/// Provides custom serialization for an object. If a
	/// class or a struct inherits this interface, the serializer
	/// simply calls this method to perform serialization. Please
	/// notice that instances of ISerializable will not get
	/// serialization lifecycle events even if they implement
	/// ISerializationListener interface.
	/// </summary>
	public interface ISerializable
	{
		/// <summary>
		/// Serializes the object into JSON string. You
		/// can use the helper methods in the serializer.
		/// </summary>
		string Serialize (Serializer serializer);
	}

	/// <summary>
	/// Provides custom deserialization for an object after it
	/// is instantiated. You may still need to create your own
	/// deserializer if your class / struct does not have a
	/// constructor without parameters.
	/// 
	/// If a class/struct inherits this interface, the deserializer
	/// completely leaves the deserialization process to the
	/// interface after the possible instantiation of the object.
	/// Please notice that instances of IDeserializable will not get
	/// deserialization lifecycle events even if they implement
	/// IDeserializationListener interface.
	/// </summary>
	public interface IDeserializable
	{
		/// <summary>
		/// Feeds the JSON node into the object. You can use the
		/// helper methods from the deserializer.
		/// </summary>
		void Deserialize (JSONNode node, Deserializer deserializer);
	}

	/// <summary>
	/// Listens for the serialization process on the object.
	/// </summary>
	public interface ISerializationListener
	{
		/// <summary>
		/// Called just before the serialization of the object begins.
		/// This call will always be followed by either OnSerializationSucceeded
		/// or OnSerializationFailed but not both.
		/// </summary>
		void OnSerializationWillBegin (Serializer serializer);

		/// <summary>
		/// Called immediately after a successful completion of this
		/// object's serialization.
		/// </summary>
		void OnSerializationSucceeded (Serializer serializer);

		/// <summary>
		/// Called when the serialization of the object fails for any
		/// reason. This will be called just before throwing the exception.
		/// </summary>
		void OnSerializationFailed (Serializer serializer);
	}

	/// <summary>
	/// Listens for the deserialization process on the object.
	/// </summary>
	public interface IDeserializationListener
	{
		/// <summary>
		/// Called just before the deserialization of the object begins.
		/// This call will always be followed by either OnDeserializationSucceeded
		/// or OnDeserializationFailed but not both.
		/// </summary>
		void OnDeserializationWillBegin (Deserializer deserializer);

		/// <summary>
		/// Called immediately after a successful completion of this
		/// object's deserialization.
		/// </summary>
		void OnDeserializationSucceeded (Deserializer deserializer);

		/// <summary>
		/// Called when the deserialization of the object fails for any
		/// reason. This will be called just before throwing the exception.
		/// </summary>
		void OnDeserializationFailed (Deserializer deserializer);
	}
}
