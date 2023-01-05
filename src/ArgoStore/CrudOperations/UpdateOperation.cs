using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace ArgoStore.CrudOperations;

internal class UpdateOperation : CrudOperation
{
    public UpdateOperation(DocumentMetadata meta, object document, string tenantId) 
        : base(meta, document, tenantId)
    {
    }

    public override SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions)
    {
        object key = Meta.GetPrimaryKeyValue(Document, out _);

        string pkName = Meta.IsKeyPropertyInt
            ? "serialId"
            : "stringId";

        long updatedAt = Clock.Default.GetCurrentUtcMilliseconds();

        string jsonData = JsonSerializer.Serialize(Document, jsonSerializerOptions);

        string sql = $"""
                UPDATE {Meta.DocumentName}
                SET
                    jsonData = @jsonData,
                    updatedAt = @updatedAt
                WHERE 
                    {pkName} = @key
                    AND tenantId = @tenantId
            """ ;

        SqliteCommand cmd = new SqliteCommand(sql);

        cmd.Parameters.AddWithValue("jsonData", jsonData);
        cmd.Parameters.AddWithValue("updatedAt", updatedAt);
        cmd.Parameters.AddWithValue("key", key);
        cmd.Parameters.AddWithValue("tenantId", TenantId);

        return cmd;
    }
}