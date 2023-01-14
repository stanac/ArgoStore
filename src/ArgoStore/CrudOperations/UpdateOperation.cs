using System.Text.Json;
using ArgoStore.Config;
using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;

namespace ArgoStore.CrudOperations;

internal class UpdateOperation : CrudOperation
{
    public UpdateOperation(DocumentMetadata metadata, object document, string tenantId) 
        : base(metadata, document, tenantId, null)
    {
    }

    public override SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions)
    {
        object? key = Metadata.GetPrimaryKeyValue(Document!, out _);

        string pkName = Metadata.IsKeyPropertyInt
            ? "serialId"
            : "stringId";

        long updatedAt = Clock.Default.GetCurrentUtcMilliseconds();

        string jsonData = JsonSerializer.Serialize(Document, jsonSerializerOptions);

        string sql = $"""
                UPDATE {Metadata.DocumentName}
                SET
                    jsonData = @jsonData,
                    updatedAt = @updatedAt
                WHERE 
                    {pkName} = @key
                    AND tenantId = @tenantId
            """ ;

        SqliteCommand cmd = new SqliteCommand(sql);

        if (key is Guid g)
        {
            cmd.Parameters.AddWithValue("key", g.ToString().ToLower());
        }
        else
        {
            cmd.Parameters.AddWithValue("key", key);
        }
        
        cmd.Parameters.AddWithValue("jsonData", jsonData);
        cmd.Parameters.AddWithValue("updatedAt", updatedAt);
        cmd.Parameters.AddWithValue("tenantId", TenantId);

        cmd.EnsureNoGuidParams();

        return cmd;
    }
}