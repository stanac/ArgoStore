using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace ArgoStore.CrudOperations;

internal class DeleteOperation : CrudOperation
{
    public DeleteOperation(DocumentMetadata metadata, object document, string tenantId) 
        : base(metadata, document, tenantId)
    {
    }

    public override SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions)
    {
        object id = Metadata.GetPrimaryKeyValue(Document, out _);

        string sql = Metadata.IsKeyPropertyInt
            ? $"DELETE FROM {Metadata.DocumentName} WHERE serialId = @id AND tenantId = @tenantId"
            : $"DELETE FROM {Metadata.DocumentName} WHERE stringId = @id AND tenantId = @tenantId";

        SqliteCommand cmd = new SqliteCommand(sql);
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("tenantId", TenantId);

        return cmd;
    }
}