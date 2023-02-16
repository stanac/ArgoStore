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
        string id = GetKey();
        
        string sql = $"DELETE FROM {Metadata.DocumentName} WHERE stringId = @id AND tenantId = @tenantId";

        SqliteCommand cmd = new SqliteCommand(sql);
        
        cmd.Parameters.AddWithValue("tenantId", TenantId);
        cmd.Parameters.AddWithValue("id", id);

        cmd.EnsureNoGuidParams();

        return cmd;
    }

    private string GetKey()
    {
        if (DocumentId != null)
        {
            return DocumentId.ToString()!;
        }

        return Metadata.SetIfNeededAndGetPrimaryKeyValue(Document!).ToString()!;
    }
}