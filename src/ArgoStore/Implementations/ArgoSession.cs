using System.Linq.Expressions;
using System.Text.Json;
using ArgoStore.Command;
using ArgoStore.Config;
using ArgoStore.CrudOperations;
using ArgoStore.Helpers;
using ArgoStore.Statements;
using Microsoft.Extensions.Logging;

namespace ArgoStore.Implementations;

internal class ArgoSession : IArgoDocumentSession
{
    public string TenantId { get; }
    public IReadOnlyDictionary<Type, DocumentMetadata> DocumentTypesMetaMap { get; }
    private readonly string _connectionString;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<ArgoSession> _logger;
    private readonly SessionId _sessionId = new();
    internal const string DefaultTenant = "DEFAULT";
    internal bool UseCaching { get; set; }

    private readonly List<CrudOperation> _crudOps = new();
    
    public ArgoSession(string connectionString, string tenantId, IReadOnlyDictionary<Type, DocumentMetadata> documentTypes,
        JsonSerializerOptions serializerOptions, ILoggerFactory loggerFactory)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(tenantId));

        TenantId = tenantId;
        DocumentTypesMetaMap = documentTypes ?? throw new ArgumentNullException(nameof(documentTypes));
        _connectionString = connectionString;
        _serializerOptions = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<ArgoSession>();

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Started session: {SessionId}", _sessionId.Id);
        }
    }

    public IArgoStoreQueryable<T> Query<T>() where T : class, new()
    {
        DebugLogMethodStart<T>("Query<T>");

        return new ArgoStoreQueryable<T>(this);
    }

    public T? GetById<T>(object id) where T : class, new()
    {
        DebugLogMethodStart<T>("GetById<T>");
        id = id.ToString()!;

        if (id is null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        DocumentMetadata meta = GetRequiredMetadata<T>();
        
        string sql = $"""
                SELECT jsonData 
                FROM {meta.DocumentName}
                WHERE stringId = @key
                AND tenantId = @tenantId
            """;

        ArgoCommandParameterCollection parameters = new(new FromAlias());
        parameters.AddWithName("key", id);
        parameters.AddWithName("tenantId", TenantId);

        ArgoCommand cmd = new ArgoCommand(sql, parameters, ArgoCommandTypes.FirstOrDefault, typeof(T), true, false);

        ArgoCommandExecutor exec = CreateExecutor();
        object? result = exec.ExecuteFirstOrDefault(cmd, null);

        if (result == null)
        {
            return null;
        }

        return (T)result;
    }

    public void Insert<T>(T[] documents) where T : class, new()
    {
        DebugLogMethodStart<T>("Insert<T>");

        DocumentMetadata meta = GetRequiredMetadata<T>();

        foreach (T doc in documents)
        {
            _crudOps.Add(new InsertOperation(meta, doc, TenantId));
        }
    }

    public void Update<T>(T[] documents) where T : class, new()
    {
        DebugLogMethodStart<T>("Update<T>");

        DocumentMetadata meta = GetRequiredMetadata<T>();

        foreach (T doc in documents)
        {
            _crudOps.Add(new UpdateOperation(meta, doc, TenantId));
        }
    }
    
    public void Upsert<T>(T[] documents) where T : class, new()
    {
        DebugLogMethodStart<T>("Upsert<T>");

        DocumentMetadata meta = GetRequiredMetadata<T>();

        foreach (T doc in documents)
        {
            _crudOps.Add(new UpsertOperation(meta, doc, TenantId));
        }
    }

    public void DeleteById<T>(params object[] documentIds)
    {
        DebugLogMethodStart<T>("DeleteById<T>");

        if (documentIds == null) throw new ArgumentNullException(nameof(documentIds));

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
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
        DebugLogMethodStart("Delete");

        if (document == null) throw new ArgumentNullException(nameof(document));
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
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
        DebugLogMethodStart<T>("DeleteWhere<T>");

        List<T> items = Query<T>().Where(predicate).ToList();

        foreach (T item in items)
        {
            DeleteInner(item);
        }
    }

    public void SaveChanges()
    {
        DebugLogMethodStart("SaveChanges");

        if (_crudOps.Any())
        {
            ArgoCommandExecutor exec = CreateExecutor();
            exec.ExecuteInTransaction(_crudOps, _serializerOptions);
            _crudOps.Clear();
        }
    }

    public void DiscardChanges()
    {
        DebugLogMethodStart("DiscardChanges");

        _crudOps.Clear();
    }

    public void Dispose()
    {
        DebugLogMethodStart("Dispose");

        _crudOps.Clear();
    }
    
    internal ArgoCommandExecutor CreateExecutor()
    {
        return new ArgoCommandExecutor(
            _connectionString,
            _serializerOptions,
            _loggerFactory.CreateLogger<ArgoCommandExecutor>(),
            _sessionId);
    }

    internal DocumentMetadata GetRequiredMetadata<T>()
    {
        return GetRequiredMetadata(typeof(T));
    }

    private void DebugLogMethodStart(string methodName, params object[] args)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            methodName = $"Method {methodName} start. SessionId: {{SessionId}}";

            List<object> argsList = new(args) { _sessionId.Id };

            _logger.LogDebug(methodName, argsList.ToArray());
        }
    }

    private void DebugLogMethodStart<T>(string methodName, params object[] args)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            methodName = $"Method {methodName} start. SessionId: {{SessionId}}, type: {{Type}}";

            List<object> argsList = new (args) {_sessionId.Id, typeof(T).FullName ?? typeof(T).Name};

            _logger.LogDebug(methodName, argsList.ToArray());
        }
    }

    private void DeleteInner(object document)
    {
        if (document == null) throw new ArgumentNullException(nameof(document));

        DocumentMetadata meta = GetRequiredMetadata(document.GetType());

        _crudOps.Add(new DeleteOperation(meta, document, TenantId, null));
    }

    private DocumentMetadata GetRequiredMetadata(Type type)
    {
        if (DocumentTypesMetaMap.TryGetValue(type, out DocumentMetadata? meta))
        {
            return meta;
        }

        throw new InvalidOperationException($"Type `{type.FullName}` is not registered.");
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