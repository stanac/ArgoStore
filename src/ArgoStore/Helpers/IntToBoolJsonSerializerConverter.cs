using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArgoStore.Helpers;

internal class IntToBoolJsonSerializerConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.True)
        {
            return true;
        }
        else if (reader.TokenType is JsonTokenType.False)
        {
            return false;
        }
        else if (reader.TokenType is JsonTokenType.Number)
        {
            JsonConverter<int> valueConverter = (JsonConverter<int>)options.GetConverter(typeof(int));
            int value = valueConverter.Read(ref reader, typeof(int), options);

            if (value == 0) return false;
            if (value == 1) return true;

        }

        throw new NotSupportedException("633b9d72a547");
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}