using System;

namespace ArgoStore
{
    /// <summary>
    /// JSON Serializer contract used for de/serialization of entities
    /// </summary>
    public interface IArgoStoreSerializer
    {
        /// <summary>
        /// Deserializes JSON into resultType
        /// </summary>
        /// <param name="json">JSON to deserialize</param>
        /// <param name="resultType">Type of resulting object</param>
        /// <returns>Deserialized object</returns>
        object Deserialize(string json, Type resultType);

        /// <summary>
        /// Serializes passed object into JSON
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object to serialize</param>
        /// <returns>JSON of serialized object</returns>
        string Serialize<T>(T obj);

        /// <summary>
        /// Converts propertyName to correct case used in DB
        /// </summary>
        /// <param name="propertyName">Property name to case-convert</param>
        /// <returns>Property name with correct case used in DB</returns>
        string ConvertPropertyNameToCorrectCase(string propertyName);
    }
}