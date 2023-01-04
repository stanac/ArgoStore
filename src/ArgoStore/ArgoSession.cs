using System.Text.Json;
using ArgoStore.CrudOperations;

namespace ArgoStore;

internal class ArgoSession : IArgoDocumentSession
{
    public string TenantId { get; }
    public IReadOnlyDictionary<string, DocumentMetadata> DocumentTypes { get; }
    private readonly string _connectionString;
    private readonly JsonSerializerOptions _serializerOptions;
    internal const string DefaultTenant = "DEFAULT";

    private readonly List<CrudOperationBase> _crudOps = new();

    public ArgoSession(string connectionString, IReadOnlyDictionary<string, DocumentMetadata> documentTypes, JsonSerializerOptions serializerOptions)
        : this(connectionString, DefaultTenant, documentTypes, serializerOptions)
    {
    }

    public ArgoSession(string connectionString, string tenantId, IReadOnlyDictionary<string, DocumentMetadata> documentTypes, JsonSerializerOptions serializerOptions)
    {
        TenantId = tenantId;
        DocumentTypes = documentTypes ?? throw new ArgumentNullException(nameof(documentTypes));
        _connectionString = connectionString;
        _serializerOptions = serializerOptions;
    }

    public IArgoStoreQueryable<T> Query<T>() where T : class, new()
    {
        return new ArgoStoreQueryable<T>(this);
    }

    internal ArgoCommandExecutor CreateExecutor()
    {
        return new ArgoCommandExecutor(_connectionString, _serializerOptions);
    }

    public void Insert<T>(T entity)
    {
        DocumentMetadata meta = GetRequiredMetadata<T>();

        _crudOps.Add(new CrudOperationInsert(meta, entity, TenantId));
    }

    private DocumentMetadata GetRequiredMetadata<T>()
    {
        return GetRequiredMetadata(typeof(T));
    }

    private DocumentMetadata GetRequiredMetadata(Type type)
    {
        KeyValuePair<string, DocumentMetadata>[] items = DocumentTypes.Where(x => x.Value.DocumentType == type).ToArray();

        if (items.Length == 0)
        {
            throw new InvalidOperationException($"Metadata for document type `{type.FullName}` not registered.");
        }

        return items[0].Value;
    }

    public void SaveChanges()
    {
        if (_crudOps.Any())
        {
            ArgoCommandExecutor exec = CreateExecutor();
            exec.ExecuteInTransaction(_crudOps, _serializerOptions);
            _crudOps.Clear();
        }
    }

    public void Dispose()
    {
        // no op
    }

}