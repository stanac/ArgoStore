using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace ArgoStore.CrudOperations;

internal class UpsertOperation : CrudOperation
{
    public UpsertOperation(DocumentMetadata meta, object document, string tenantId)
        : base(meta, document, tenantId)
    {
    }

    public override SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions)
    {
        object key = Meta.GetPrimaryKeyValue(Document, out bool isDefaultKey);

        throw new NotImplementedException();
        
        string pkName = Meta.IsKeyPropertyInt
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