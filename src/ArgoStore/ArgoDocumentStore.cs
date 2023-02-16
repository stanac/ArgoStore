using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using ArgoStore.Command;
using ArgoStore.Config;
using ArgoStore.Helpers;
using ArgoStore.Implementations;
using Microsoft.Extensions.Logging;

namespace ArgoStore;

/// <summary>
/// Argo document store
/// </summary>
public class ArgoDocumentStore : IArgoDocumentStore
{
    private readonly string _connectionString;
    private readonly ILoggerFactory _loggerFactory;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly SqlDataDefinitionExecutor _ddExec;
    private readonly ConcurrentDictionary<Type, DocumentMetadata> _docTypeMetaMap;

    /// <summary>
    /// Constructor with connection string
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    public ArgoDocumentStore(string connectionString)
        : this(ConfigureDefault(connectionString))
    {
    }

    /// <summary>
    /// Constructor with configuration action
    /// </summary>
    /// <param name="configure">Configure action</param>
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
        _loggerFactory = config.LoggerFactory;

        foreach (KeyValuePair<Type, DocumentMetadata> pair in _docTypeMetaMap)
        {
            _ddExec.CreateDocumentObjects(pair.Value);
        }
    }

    /// <inheritdoc />
    public IArgoQueryDocumentSession OpenQuerySession() => OpenSession(ArgoSession.DefaultTenant);

    /// <inheritdoc />
    public IArgoQueryDocumentSession OpenQuerySession(string tenantId) => OpenSession(tenantId);

    /// <inheritdoc />
    public IArgoDocumentSession OpenSession() => OpenSession(ArgoSession.DefaultTenant);

    /// <inheritdoc />
    public IArgoDocumentSession OpenSession(string tenantId)
    {
        return new ArgoSession(_connectionString, tenantId, _docTypeMetaMap, _serializerOptions, _loggerFactory);
    }

    /// <inheritdoc />
    public void RegisterDocument<T>() where T : class, new()
    {
        RegisterDocument<T>(_ => { });
    }

    /// <inheritdoc />
    public void RegisterDocument<T>(Action<IDocumentConfiguration<T>> configure) where T : class, new()
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        DocumentConfiguration<T> c = new DocumentConfiguration<T>();
        configure(c);
        DocumentMetadata meta = c.CreateMetadata();
        _ddExec.CreateDocumentObjects(meta);
        _docTypeMetaMap[typeof(T)] = meta;
    }

    /// <inheritdoc />
    public IReadOnlyList<string> ListTenants<T>()
    {
        using ArgoSession session = (ArgoSession)OpenSession();

        DocumentMetadata docMeta = session.GetRequiredMetadata<T>();

        string sql = $@"SELECT DISTINCT
            json_insert('{{}}', '$.value', tenantId)
            FROM {docMeta.DocumentName}
        ";

        ArgoCommand cmd = new ArgoCommand(sql, new ArgoCommandParameterCollection(), ArgoCommandTypes.ToList, typeof(string), false, false);
        
        ArgoCommandExecutor ex = session.CreateExecutor();

        object result = ex.ExecuteToList(cmd);

        List<string> list = (List<string>) result;

        return list;
    }

    private static JsonSerializerOptions CreateJsonSerializerOptions()
    {
        JsonSerializerOptions opt = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        opt.Converters.Add(new JsonStringEnumConverter());
        opt.Converters.Add(new IntToBoolJsonSerializerConverterFactory());

#if !NETSTANDARD
        opt.Converters.Add(new TimeOnlyToIntJsonSerializerConverterFactory());
#endif

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
