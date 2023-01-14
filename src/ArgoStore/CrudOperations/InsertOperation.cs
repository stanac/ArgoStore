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
            var key = metadata.GetPrimaryKeyValue(document, out _);

            if (key == null)
            {
                throw new InvalidOperationException($"Failed to insert `{metadata.DocumentType.FullName}`. Primary key of type string must be set.");
            }
        }
    }

    public override SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions)
    {
        if (jsonSerializerOptions == null) throw new ArgumentNullException(nameof(jsonSerializerOptions));

        object key = Metadata.SetIfNeededAndGetPrimaryKeyValue(Document!, out bool insertKey);

        object guidId = Metadata.IsKeyPropertyInt
            ? Guid.NewGuid().ToString().ToLower()
            : key;

        if (guidId is Guid g)
        {
            guidId = g.ToString().ToLower();
        }

        insertKey = insertKey && Metadata.IsKeyPropertyInt;

        string sql = insertKey 
            ? $"""
                INSERT INTO {Metadata.DocumentName} (serialId, stringId, jsonData, tenantId, createdAt)
                VALUES (@serialId, @guidId, json(@jsonData), @tenantId, @createdAt)
                RETURNING serialId
              """
            : $"""
                INSERT INTO {Metadata.DocumentName} (stringId, jsonData, tenantId, createdAt)
                VALUES (@guidId, json(@jsonData), @tenantId, @createdAt)
                RETURNING serialId
              """;

        SqliteCommand cmd = new SqliteCommand(sql);

        if (insertKey)
        {
            cmd.Parameters.AddWithValue("serialId", key);
        }

        cmd.Parameters.AddWithValue("guidId", guidId);

        string jsonData = JsonSerializer.Serialize(Document, jsonSerializerOptions);
        cmd.Parameters.AddWithValue("jsonData", jsonData);

        cmd.Parameters.AddWithValue("tenantId", TenantId);
        cmd.Parameters.AddWithValue("createdAt", Clock.Default.GetCurrentUtcMilliseconds());

        return cmd;
    }
}