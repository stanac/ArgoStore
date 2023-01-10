using ArgoStore.Implementations;

namespace ArgoStore.Config;

internal class DocumentStoreConfiguration : IArgoStoreConfiguration
{
    private string _connectionString;
    private readonly Dictionary<Type, DocumentConfiguration> _entityConfigs = new();

    public void ConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        _connectionString = connectionString;
    }

    public IDocumentConfiguration<TDocument> RegisterDocument<TDocument>() where TDocument : class, new()
    {
        return RegisterDocument<TDocument>(_ => {});
    }

    public IDocumentConfiguration<TDocument> RegisterDocument<TDocument>(Action<IDocumentConfiguration<TDocument>> configure) where TDocument : class, new()
    {
        Type entityType = typeof(TDocument);

        if (_entityConfigs.ContainsKey(entityType))
        {
            throw new InvalidOperationException($"Entity {entityType.Name} is already configured");
        }

        DocumentConfiguration<TDocument> DocumentConfiguration = new DocumentConfiguration<TDocument>();

        _entityConfigs[entityType] = DocumentConfiguration;
        return DocumentConfiguration;
    }
    
    public ArgoStoreConfiguration CreateConfiguration()
    {
        EnsureValid();

        Dictionary<Type, DocumentMetadata> meta = CreateMetadata();

        return new ArgoStoreConfiguration(
            _connectionString, meta
        );
    }

    private Dictionary<Type, DocumentMetadata> CreateMetadata()
    {
        Dictionary<Type, DocumentMetadata> meta = new Dictionary<Type, DocumentMetadata>();

        foreach (KeyValuePair<Type, DocumentConfiguration> c in _entityConfigs)
        {
            if (meta.ContainsKey(c.Key))
            {
                throw new InvalidOperationException($"Entity type `{c.Key.FullName}` has been configured more than once.");
            }

            meta[c.Key] = c.Value.CreateMetadata();
        }

        return meta;
    }

    private void EnsureValid()
    {
        if (_connectionString == null)
        {
            throw new InvalidOperationException("Connection string not set");
        }
    }
}