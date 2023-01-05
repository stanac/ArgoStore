using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace ArgoStore.CrudOperations;

internal class InsertOperation : CrudOperation
{
    public InsertOperation(DocumentMetadata meta, object document, string tenantId) : base(meta, document, tenantId)
    {
    }

    public override SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions)
    {
        object key = Meta.SetIfNeededAndGetPrimaryKeyValue(Document, out bool insertKey);

        object guidId = Meta.IsKeyPropertyInt
            ? Guid.NewGuid().ToString()
            : key;

        insertKey = insertKey && Meta.IsKeyPropertyInt;

        string sql = insertKey 
            ? $"""
                INSERT INTO {Meta.DocumentName} (serialId, stringId, jsonData, tenantId, createdAt)
                VALUES (@serialId, @guidId, @jsonData, @tenantId, @createdAt)
              """
            : $"""
                INSERT INTO {Meta.DocumentName} (stringId, jsonData, tenantId, createdAt)
                VALUES (@guidId, @jsonData, @tenantId, @createdAt)
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