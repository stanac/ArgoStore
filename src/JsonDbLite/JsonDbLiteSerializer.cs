using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonDbLite
{
    /// <summary>
    /// JSON serializer used by JsonDbLite, properties are serialized as camel case, enums as strings as camel case
    /// </summary>
    public static class JsonDbLiteSerializer
    {
        private static JsonSerializerOptions _jso = CreateOptions();

        /// <summary>
        /// Serializes object
        /// </summary>
        /// <typeparam name="T">Type of the object to serialize</typeparam>
        /// <param name="obj">Object to serialize, cannot be null</param>
        /// <returns>JSON string</returns>
        public static string Serialize<T>(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return JsonSerializer.Serialize(obj, _jso);
        }

        /// <summary>
        /// Deserializes JSON into object T
        /// </summary>
        /// <typeparam name="T">Type of resulting object</typeparam>
        /// <param name="json">JSON to deserialize, cannot be null or whitespace</param>
        /// <returns>Deserialized object of type T</returns>
        public static T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException($"'{nameof(json)}' cannot be null or whitespace", nameof(json));

            return JsonSerializer.Deserialize<T>(json, _jso);
        }

        private static JsonSerializerOptions CreateOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            return options;
        }
    }
}
