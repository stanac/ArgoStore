using System.Text.Json;
using ArgoStore.Config;
using ArgoStore.Helpers;
using Microsoft.Data.Sqlite;

namespace ArgoStore.CrudOperations;

internal class DeleteOperation : CrudOperation
{
    public DeleteOperation(DocumentMetadata metadata, object? document, string tenantId, object? documentId)
        : base(metadata, document, tenantId, documentId)
    {
    }

    public override SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions)
    {
        object? id = DocumentId ?? Metadata.GetPrimaryKeyValue(Document!, out _);

        string idPropName = Metadata.IsKeyPropertyInt
            ? "serialId"
            : "stringId";

        string sql = $"DELETE FROM {Metadata.DocumentName} WHERE {idPropName} = @id AND tenantId = @tenantId";

        SqliteCommand cmd = new SqliteCommand(sql);

        if (id is Guid g)
        {
            cmd.Parameters.AddWithValue("id", g.ToString().ToLower());
        }
        else
        {
            cmd.Parameters.AddWithValue("id", id);
        }

        cmd.Parameters.AddWithValue("tenantId", TenantId);

        cmd.EnsureNoGuidParams();

        return cmd;
    }
}