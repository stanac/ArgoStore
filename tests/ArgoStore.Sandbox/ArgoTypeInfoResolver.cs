using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ArgoStore.Sandbox;

public class ArgoTypeInfoResolver : IJsonTypeInfoResolver
{
    private readonly IDictionary<Type, JsonTypeInfo> _typeInfos = new ConcurrentDictionary<Type, JsonTypeInfo>();

    public void Register<T>(JsonTypeInfo<T> typeInfo)
    {
        _typeInfos[typeof(T)] = typeInfo;
    }

    public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        if (_typeInfos.TryGetValue(type, out var info))
        {
            return info;
        }

        return null;
    }
}