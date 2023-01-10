namespace ArgoStore.Config;

internal class DocumentStoreConfiguration : IArgoStoreConfiguration
{
    private string _connectionString;
    private bool _createNonConfiguredEntities;
    private readonly Dictionary<Type, DocumentConfiguration> _entityConfigs = new();

    public void ConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        _connectionString = connectionString;
    }

    public IDocumentConfiguration<TDocument> Document<TDocument>() where TDocument : class, new()
    {
        throw new NotImplementedException();
    }

    public void CreateNotConfiguredEntities(bool createNonConfiguredEntities)
    {
        _createNonConfiguredEntities = createNonConfiguredEntities;
    }

    public IDocumentConfiguration<TEntity> Entity<TEntity>() where TEntity : class, new()
    {
        Type entityType = typeof(TEntity);

        if (_entityConfigs.ContainsKey(entityType))
        {
            throw new InvalidOperationException($"Entity {entityType.Name} is already configured");
        }

        DocumentConfiguration<TEntity> DocumentConfiguration = new DocumentConfiguration<TEntity>();

        _entityConfigs[entityType] = DocumentConfiguration;
        return DocumentConfiguration;
    }

    public ArgoStoreConfiguration CreateConfiguration()
    {
        EnsureValid();

        Dictionary<Type, DocumentMetadata> meta = CreateMetadata();

        return new ArgoStoreConfiguration(
            _connectionString, _createNonConfiguredEntities, meta
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