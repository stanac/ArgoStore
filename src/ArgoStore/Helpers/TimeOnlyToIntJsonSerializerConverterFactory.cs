#if !NETSTANDARD
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArgoStore.Helpers;

internal class TimeOnlyToIntJsonSerializerConverterFactory : JsonConverterFactory
{
    private static readonly TimeOnlyToIntJsonSerializerConverter _converter = new();

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(TimeOnly);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return _converter;
    }
}

#endif