using System.Text.Json;
using ArgoStore.Config;
using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;

namespace ArgoStore.CrudOperations;

internal class UpsertOperation : CrudOperation
{
    public UpsertOperation(DocumentMetadata metadata, object document, string tenantId)
        : base(metadata, document, tenantId, null)
    {
    }

    public override SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions)
    {
        object? key = Metadata.GetPrimaryKeyValue(Document!, out bool isDefaultKey);

        if (isDefaultKey)
        {
            if (!Metadata.IsKeyPropertyInt)
            {
                Metadata.SetIfNeededAndGetPrimaryKeyValue(Document!, out _);
            }
        }
        
        string pkName = Metadata.IsKeyPropertyInt
            ? "serialId"
            : "stringId";

        long updatedAt = Clock.Default.GetCurrentUtcMilliseconds();

        string jsonData = JsonSerializer.Serialize(Document, jsonSerializerOptions);

        string sql = $"""
                
            """;

        SqliteCommand cmd = new SqliteCommand(sql);

        cmd.Parameters.AddWithValue("jsonData", jsonData);
        cmd.Parameters.AddWithValue("updatedAt", updatedAt);
        cmd.Parameters.AddWithValue("key", key);
        cmd.Parameters.AddWithValue("tenantId", TenantId);

        return cmd;
    }
}