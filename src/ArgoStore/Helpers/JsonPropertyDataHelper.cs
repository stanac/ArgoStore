using System.Text.Json;

namespace ArgoStore.Helpers;

internal static class JsonPropertyDataHelper
{
    private const string TopTableAlias = "t1";

    public static string ExtractProperty(string propertyName) => ExtractProperty(propertyName, null);

    public static string ExtractProperty(string propertyName, string? alias)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(propertyName));
        
        propertyName = ConvertPropertyNameCase(propertyName);

        if (string.IsNullOrWhiteSpace(alias))
        {
            return $"json_extract(jsonData, '$.{propertyName}')";
        }

        if (alias == TopTableAlias)
        {
            return $"json_extract({alias}.jsonData, '$.{propertyName}')";
        }

        return $"json_extract({alias}.value, '$.{propertyName}')";
    }


    public static string ConvertPropertyNameCase(string propertyName)
    {
        if (propertyName.Contains('.'))
        {
            string[] parts = propertyName.Split('.');

            return string.Join(".", parts.Select(JsonNamingPolicy.CamelCase.ConvertName));
        }

        return JsonNamingPolicy.CamelCase.ConvertName(propertyName);
    }
}