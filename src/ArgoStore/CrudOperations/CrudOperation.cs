using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace ArgoStore.CrudOperations;

internal abstract class CrudOperation
{
    public object Document { get; }
    public string TenantId { get; }
    public DocumentMetadata Meta { get; }

    protected CrudOperation(DocumentMetadata meta, object document, string tenantId)
    {
        Document = document;
        TenantId = tenantId;
        Meta = meta;
    }

    public abstract SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions);
}