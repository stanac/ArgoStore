using System.Text.Json;

namespace ArgoStore.Helpers;

internal static class JsonPropertyDataHelper
{
    public static string ExtractProperty(string propertyName) => ExtractProperty(propertyName, null);

    public static string ExtractProperty(string propertyName, string? alias)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(propertyName));

        propertyName = ConvertPropertyNameCase(propertyName);

        if (string.IsNullOrWhiteSpace(alias))
        {
            return $"json_extract(jsonData, '$.{propertyName}')";
        }

        return $"json_extract({alias}.jsonData, '$.{propertyName}')";
    }


    public static string ConvertPropertyNameCase(string propertyName)
    {
        return JsonNamingPolicy.CamelCase.ConvertName(propertyName);
    }
}