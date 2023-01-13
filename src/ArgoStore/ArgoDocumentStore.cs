using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using ArgoStore.Config;
using ArgoStore.Helpers;
using ArgoStore.Implementations;

namespace ArgoStore;

public class ArgoDocumentStore : IArgoDocumentStore
{
    private readonly string _connectionString;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly SqlDataDefinitionExecutor _ddExec;
    private readonly ConcurrentDictionary<Type, DocumentMetadata> _docTypeMetaMap = new();

    public ArgoDocumentStore(string connectionString)
        : this(ConfigureDefault(connectionString))
    {
    }

    public ArgoDocumentStore(Action<IArgoStoreConfiguration> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        DocumentStoreConfiguration config = new DocumentStoreConfiguration();
        configure(config);

        ArgoStoreConfiguration c = config.CreateConfiguration();

        _connectionString = c.ConnectionString;
        _ddExec = new SqlDataDefinitionExecutor(_connectionString);

        _serializerOptions = CreateJsonSerializerOptions();
        _docTypeMetaMap = new ConcurrentDictionary<Type, DocumentMetadata>(c.DocumentMeta);

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

    public void RegisterDocument<T>() where T : class, new()
    {
        RegisterDocument<T>(_ => { });
    }

    public void RegisterDocument<T>(Action<IDocumentConfiguration<T>> configure) where T : class, new()
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

    private static Action<IArgoStoreConfiguration> ConfigureDefault(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        return c =>
        {
            c.ConnectionString(connectionString);
        };
    }
}
