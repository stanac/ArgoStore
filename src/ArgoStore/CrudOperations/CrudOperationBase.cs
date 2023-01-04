using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace ArgoStore.CrudOperations;

internal abstract class CrudOperationBase
{
    public object Document { get; }
    public string TenantId { get; }
    public DocumentMetadata Meta { get; }

    protected CrudOperationBase(DocumentMetadata meta, object document, string tenantId)
    {
        Document = document;
        TenantId = tenantId;
        Meta = meta;
    }

    public abstract SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions);
}