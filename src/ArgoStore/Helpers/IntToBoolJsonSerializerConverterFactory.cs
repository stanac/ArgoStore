using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArgoStore.Helpers;

internal class IntToBoolJsonSerializerConverterFactory : JsonConverterFactory
{
    private static readonly IntToBoolJsonSerializerConverter _converter = new();

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(bool);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return _converter;
    }
}