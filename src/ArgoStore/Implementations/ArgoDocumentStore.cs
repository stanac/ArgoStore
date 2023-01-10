using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using ArgoStore.Config;
using ArgoStore.Helpers;

namespace ArgoStore.Implementations;

public class ArgoDocumentStore
{
    private readonly string _connectionString;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly SqlDataDefinitionExecutor _ddExec;
    private readonly ConcurrentDictionary<Type, DocumentMetadata> _docTypeMetaMap = new();

    public ArgoDocumentStore(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        _connectionString = connectionString;
        _ddExec = new SqlDataDefinitionExecutor(_connectionString);

        _serializerOptions = CreateJsonSerializerOptions();
    }

    public ArgoDocumentStore(Action<IArgoStoreConfiguration> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        DocumentStoreConfiguration config = new DocumentStoreConfiguration();
        configure(config);

        ArgoStoreConfiguration c = config.CreateConfiguration();

        _connectionString = c.ConnectionString;
        _ddExec = new SqlDataDefinitionExecutor(_connectionString);

        foreach (KeyValuePair<Type, DocumentMetadata> pair in _docTypeMetaMap)
        {
            _ddExec.CreateDocumentObjects(pair.Value);
        }
    }

    public IArgoDocumentSession OpenSession() => OpenSession(ArgoSession.DefaultTenant);

    public IArgoDocumentSession OpenSession(string tenantId)
    {
        return new ArgoSession(_connectionString, tenantId, _docTypeMetaMap, _serializerOptions);
    }

    public IArgoQueryDocumentSession OpenQuerySession() => OpenQuerySession(ArgoSession.DefaultTenant);

    public IArgoQueryDocumentSession OpenQuerySession(string tenantId)
    {
        return new ArgoSession(_connectionString, tenantId, _docTypeMetaMap, _serializerOptions);
    }

    public void RegisterDocumentType<T>() where T : class, new()
    {
        RegisterDocumentType<T>(_ => {});
    }

    public void RegisterDocumentType<T>(Action<IDocumentConfiguration<T>> configure) where T : class, new()
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        DocumentConfiguration<T> c = new DocumentConfiguration<T>();
        configure(c);
        DocumentMetadata meta = c.CreateMetadata();
        _ddExec.CreateDocumentObjects(meta);
        _docTypeMetaMap[typeof(T)] = meta;
    }

    private static JsonSerializerOptions CreateJsonSerializerOptions()
    {
        JsonSerializerOptions opt = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        opt.Converters.Add(new IntToBoolJsonSerializerConverterFactory());
        opt.Converters.Add(new JsonStringEnumConverter());

        return opt;
    }
}
