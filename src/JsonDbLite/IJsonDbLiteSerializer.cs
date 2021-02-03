using System;

namespace JsonDbLite
{
    public interface IJsonDbLiteSerializer
    {
        T Deserialize<T>(string json);
        object Deserialize(string json, Type resultType);
        string Serialize<T>(T obj);
        string ConvertPropertyNameToCorrectCase(string propertyName);
    }
}