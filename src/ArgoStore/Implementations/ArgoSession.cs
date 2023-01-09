using System.Linq.Expressions;
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

    public void Insert<T>(T[] documents) where T : class, new()
    {
        DocumentMetadata meta = GetRequiredMetadata<T>();

        foreach (T doc in documents)
        {
            _crudOps.Add(new InsertOperation(meta, doc, TenantId));
        }
    }
    
    public void DeleteById<T>(params object[] documentIds)
    {
        if (documentIds == null) throw new ArgumentNullException(nameof(documentIds));

        if (documentIds.Any(x => x is null))
        {
            throw new ArgumentException("Array contains null value", nameof(documentIds));
        }

        if (documentIds.Length == 0)
        {
            return;
        }
        
        DocumentMetadata meta = GetRequiredMetadata<T>();

        Type idType = GetValidateSingleDocumentIdsType(documentIds);

        if (idType != meta.KeyPropertyType)
        {
            throw new ArgumentException(
                $"Expected document id to be of type `{meta.KeyPropertyType.FullName}` but got `{idType.FullName}`", 
                nameof(documentIds)
                );
        }

        foreach (object documentId in documentIds)
        {
            _crudOps.Add(new DeleteOperation(meta, null, TenantId, documentId));
        }
    }

    public void Delete(params object[] document)
    {
        if (document == null) throw new ArgumentNullException(nameof(document));
        if (document.Any(x => x is null))
        {
            throw new ArgumentException("Collection cannot contain null", nameof(document));
        }

        foreach (object o in document)
        {
            DeleteInner(o);
        }
    }
    
    public void DeleteWhere<T>(Expression<Func<T, bool>> predicate)
        where T : class, new()
    {
        List<T> items = Query<T>().Where(predicate).ToList();

        foreach (T item in items)
        {
            DeleteInner(item);
        }
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

    private void DeleteInner(object document)
    {
        if (document == null) throw new ArgumentNullException(nameof(document));

        DocumentMetadata meta = GetRequiredMetadata(document.GetType());

        _crudOps.Add(new DeleteOperation(meta, document, TenantId, null));
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

    private Type GetValidateSingleDocumentIdsType(object[] documentIds)
    {
        Type[] types = new Type[documentIds.Length];

        for (int i = 0; i < documentIds.Length; i++)
        {
            types[i] = documentIds[i].GetType();
        }

        if (types.Distinct().Count() != 1)
        {
            throw new ArgumentException("Expected values to be of same type.", nameof(documentIds));
        }

        return types[0];
    }

}