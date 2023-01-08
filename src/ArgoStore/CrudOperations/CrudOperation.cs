using System.Text.Json;
using ArgoStore.Config;
using Microsoft.Data.Sqlite;

namespace ArgoStore.CrudOperations;

internal abstract class CrudOperation
{
    public object Document { get; }
    public string TenantId { get; }
    public DocumentMetadata Metadata { get; }

    protected CrudOperation(DocumentMetadata metadata, object document, string tenantId)
    {
        Document = document;
        TenantId = tenantId;
        Metadata = metadata;
    }

    public abstract SqliteCommand CreateCommand(JsonSerializerOptions jsonSerializerOptions);
}