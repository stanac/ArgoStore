using System.Text.Json;

namespace ArgoStore;

internal abstract class SelectValueHolder
{
    public abstract object GetValue();

    public static SelectValueHolder ParseFromJson(string json, Type resultingType, JsonSerializerOptions jsonSerializerOptions)
    {
        Type selectType = typeof(SelectValueHolder<>).MakeGenericType(resultingType);

        return JsonSerializer.Deserialize(json, selectType, jsonSerializerOptions) as SelectValueHolder;
    }
}

internal class SelectValueHolder<T> : SelectValueHolder
{
    public T Value { get; set; }

    public override object GetValue()
    {
        return Value;
    }
}