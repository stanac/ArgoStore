using System.Text.Json;
using ArgoStore.Command;
using ArgoStore.Config;
using ArgoStore.CrudOperations;

namespace ArgoStore.Implementations;

internal class ArgoSession : IArgoDocumentSession
{
    public string TenantId { get; }
    public IReadOnlyDictionary<string, DocumentMetadata> DocumentTypes { get; }
    private readonly string _connectionString;
    private readonly JsonSerializerOptions _serializerOptions;
    internal const string DefaultTenant = "DEFAULT";

    private readonly List<CrudOperation> _crudOps = new();

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

    public T GetById<T>(object id) where T : class, new()
    {
        if (id is null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        DocumentMetadata meta = GetRequiredMetadata<T>();

        if (id.GetType() != meta.KeyPropertyType)
        {
            throw new InvalidOperationException($"Expected id to be of type `{meta.KeyPropertyType.FullName}` but got `{id.GetType().FullName}`.");
        }

        string pkName = meta.IsKeyPropertyInt
            ? "serialId"
            : "stringId";

        string sql = $"""
                SELECT jsonData 
                FROM {meta.DocumentName}
                WHERE {pkName} = @key
                AND tenantId = @tenantId
            """;

        ArgoCommandParameterCollection parameters = new();

        parameters.AddWithName("key", id);
        parameters.AddWithName("tenantId", TenantId);

        ArgoCommand cmd = new ArgoCommand(sql, parameters, ArgoCommandTypes.FirstOrDefault, typeof(T), true, false);

        ArgoCommandExecutor exec = CreateExecutor();
        object result = exec.ExecuteFirstOrDefault(cmd);

        if (result == null)
        {
            return null;
        }

        return (T)result;
    }

    internal ArgoCommandExecutor CreateExecutor()
    {
        return new ArgoCommandExecutor(_connectionString, _serializerOptions);
    }

    public void Insert<T>(T[] entities) where T : class, new()
    {
        DocumentMetadata meta = GetRequiredMetadata<T>();

        foreach (T entity in entities)
        {
            _crudOps.Add(new InsertOperation(meta, entity, TenantId));
        }
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

    public void DiscardChanges()
    {
        _crudOps.Clear();
    }

    public void Dispose()
    {
        DiscardChanges();
    }
}