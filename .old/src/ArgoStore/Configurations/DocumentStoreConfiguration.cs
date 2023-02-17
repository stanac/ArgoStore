namespace ArgoStore.Configurations;

internal class DocumentStoreConfiguration : IDocumentStoreConfiguration
{
    private string _connectionString;
    private bool _createNonConfiguredEntities;
    private readonly Dictionary<Type, EntityConfiguration> _entityConfigs = new();

    public void ConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        _connectionString = connectionString;
    }

    public void CreateNotConfiguredEntities(bool createNonConfiguredEntities)
    {
        _createNonConfiguredEntities = createNonConfiguredEntities;
    }

    public IEntityConfiguration<TEntity> Entity<TEntity>() where TEntity : class, new()
    {
        Type entityType = typeof(TEntity);

        if (_entityConfigs.ContainsKey(entityType))
        {
            throw new InvalidOperationException($"Entity {entityType.Name} is already configured");
        }

        EntityConfiguration<TEntity> entityConfiguration = new EntityConfiguration<TEntity>();

        _entityConfigs[entityType] = entityConfiguration;
        return entityConfiguration;
    }

    public Configuration CreateConfiguration()
    {
        EnsureValid();

        Dictionary<Type, EntityMetadata> meta = CreateMetadata();

        return new Configuration(
            _connectionString, _createNonConfiguredEntities,
            meta, TenantIdDefault.DefaultValue
        );
    }

    private Dictionary<Type, EntityMetadata> CreateMetadata()
    {
        Dictionary<Type, EntityMetadata> meta = new Dictionary<Type, EntityMetadata>();

        foreach (KeyValuePair<Type, EntityConfiguration> c in _entityConfigs)
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