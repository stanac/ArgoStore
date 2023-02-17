using System.Text.Json;
using ArgoStore.Config;
using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;

namespace ArgoStore.CrudOperations;

internal class InsertOperation : CrudOperation
{
    public InsertOperation(DocumentMetadata metadata, object document, string tenantId) : base(metadata, document, tenantId, null)
    {
        if (metadata.IsKeyPropertyString)
        {
            var key = metadata.SetIfNeededAndGetPrimaryKeyValue(document);

            if (key == null)
            {
                throw new InvalidOperationException($"Failed to insert `{metadata.DocumentType.FullName}`. Primary key of type string must be set.");
            }
        }
    }

    public override SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions)
    {
        if (jsonSerializerOptions == null) throw new ArgumentNullException(nameof(jsonSerializerOptions));
        
        string key = Metadata.SetIfNeededAndGetPrimaryKeyValue(Document!).ToString()!;
        
        string sql =
            $"""
                INSERT INTO {Metadata.DocumentName} (stringId, jsonData, tenantId, createdAt)
                VALUES (@stringId, json(@jsonData), @tenantId, @createdAt)
            """;

        SqliteCommand cmd = new SqliteCommand(sql);
        
        cmd.Parameters.AddWithValue("stringId", key);

        string jsonData = JsonSerializer.Serialize(Document, jsonSerializerOptions);
        cmd.Parameters.AddWithValue("jsonData", jsonData);

        cmd.Parameters.AddWithValue("tenantId", TenantId);
        cmd.Parameters.AddWithValue("createdAt", Clock.Default.GetCurrentUtcMilliseconds());

        return cmd;
    }
}