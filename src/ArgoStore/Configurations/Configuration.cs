namespace ArgoStore.Configurations;

internal class Configuration
{
    public Configuration(string connectionString, bool createEntitiesOnTheFly, Dictionary<Type, EntityMetadata> entityMeta, string tenantId)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        if (string.IsNullOrWhiteSpace(tenantId)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(tenantId));

        ConnectionString = connectionString;
        CreateEntitiesOnTheFly = createEntitiesOnTheFly;
        TenantId = tenantId;
        EntityMeta = entityMeta ?? throw new ArgumentNullException(nameof(entityMeta));
    }

    public string ConnectionString { get; }
    public bool CreateEntitiesOnTheFly { get; }
    public string TenantId { get; }
    public IReadOnlyDictionary<Type, EntityMetadata> EntityMeta { get; }
    public IArgoStoreSerializer Serializer { get; } = new ArgoStoreSerializer();
    
    internal EntityMetadata GetOrCreateEntityMetadata(Type entityType)
    {
        if (EntityMeta.TryGetValue(entityType, out EntityMetadata m))
        {
            return m;
        }

        m = new EntityMetadata(entityType);

        ((Dictionary<Type, EntityMetadata>)EntityMeta)[entityType] = m;

        return m;
    }

    public Configuration ChangeTenant(string newTenantId)
    {
        return new Configuration(ConnectionString, CreateEntitiesOnTheFly, (Dictionary<Type, EntityMetadata>)EntityMeta, newTenantId);
    }

    public Configuration ChangeCreateEntitiesOnTheFly(bool createEntitiesOnTheFly)
    {
        return new Configuration(ConnectionString, createEntitiesOnTheFly, (Dictionary<Type, EntityMetadata>)EntityMeta, TenantId);
    }

    public Dictionary<Type, EntityMetadata> GetEntityMetaCopy()
    {
        Dictionary<Type, EntityMetadata> ret = new();

        foreach (KeyValuePair<Type, EntityMetadata> pair in EntityMeta)
        {
            ret[pair.Key] = pair.Value;
        }

        return ret;
    }
}
