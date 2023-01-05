using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace ArgoStore.CrudOperations;

internal class DeleteOperation : CrudOperation
{
    public DeleteOperation(DocumentMetadata meta, object document, string tenantId) 
        : base(meta, document, tenantId)
    {
    }

    public override SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions)
    {
        object id = Meta.GetPrimaryKeyValue(Document);

        string sql = Meta.IsKeyPropertyInt
            ? $"DELETE FROM {Meta.DocumentName} WHERE serialId = @id AND tenantId = @tenantId"
            : $"DELETE FROM {Meta.DocumentName} WHERE stringId = @id AND tenantId = @tenantId";

        SqliteCommand cmd = new SqliteCommand(sql);
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("tenantId", TenantId);

        return cmd;
    }
}