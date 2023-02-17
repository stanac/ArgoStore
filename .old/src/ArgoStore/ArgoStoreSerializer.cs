using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArgoStore;

/// <summary>
/// JSON serializer used by ArgoStore, properties are serialized as camel case, enums as strings as camel case,
/// using System.CommandText.JsonSerializer.
/// </summary>
public class ArgoStoreSerializer : IArgoStoreSerializer
{
    private readonly JsonSerializerOptions _jso = CreateOptions();

    /// <summary>
    /// Serializes object
    /// </summary>
    /// <typeparam name="T">Type of the object to serialize</typeparam>
    /// <param name="obj">Object to serialize, cannot be null</param>
    /// <returns>JSON string</returns>
    public string Serialize<T>(T obj)
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
    public T Deserialize<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException($"'{nameof(json)}' cannot be null or whitespace", nameof(json));

        return JsonSerializer.Deserialize<T>(json, _jso);
    }

    /// <summary>
    /// Deserializes JSON into object of resultType
    /// </summary>
    /// <param name="json">JSON to deserialize, cannot be null or whitespace</param>
    /// <param name="resultType">Type to deserialize to</param>
    /// <returns>Deserialized object of resultType</returns>
    public object Deserialize(string json, Type resultType)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException($"'{nameof(json)}' cannot be null or whitespace", nameof(json));

        return JsonSerializer.Deserialize(json, resultType, _jso);
    }

    public string ConvertPropertyNameToCorrectCase(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or whitespace", nameof(propertyName));

        return JsonNamingPolicy.CamelCase.ConvertName(propertyName);
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