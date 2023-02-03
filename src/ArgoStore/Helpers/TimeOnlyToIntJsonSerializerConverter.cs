#if !NETSTANDARD
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArgoStore.Helpers;

internal class TimeOnlyToIntJsonSerializerConverter : JsonConverter<TimeOnly>
{
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.Number)
        {
            JsonConverter<int> valueConverter = (JsonConverter<int>)options.GetConverter(typeof(int));
            int value = valueConverter.Read(ref reader, typeof(int), options);

            return TimeOnly.FromTimeSpan(TimeSpan.FromMilliseconds(value));
        }
        
        throw new NotSupportedException($"Unsupported JsonTokenType `{reader.TokenType}` in `{nameof(IntToBoolJsonSerializerConverter)}`");
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        int intVal = (int)(value.ToTimeSpan().TotalMilliseconds);

        writer.WriteNumberValue(intVal);
    }
}
#endif