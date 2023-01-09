namespace ArgoStore.Config;

internal class ArgoStoreConfiguration
{
    public ArgoStoreConfiguration(string connectionString, bool createEntitiesOnTheFly, Dictionary<Type, DocumentMetadata> entityMeta, string tenantId)
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
    public IReadOnlyDictionary<Type, DocumentMetadata> EntityMeta { get; }

    internal DocumentMetadata GetOrCreateDocumentMetadata(Type entityType)
    {
        if (EntityMeta.TryGetValue(entityType, out DocumentMetadata m))
        {
            return m;
        }

        m = new DocumentMetadata(entityType);

        ((Dictionary<Type, DocumentMetadata>)EntityMeta)[entityType] = m;

        return m;
    }

    public ArgoStoreConfiguration ChangeTenant(string newTenantId)
    {
        return new ArgoStoreConfiguration(ConnectionString, CreateEntitiesOnTheFly, (Dictionary<Type, DocumentMetadata>)EntityMeta, newTenantId);
    }

    public ArgoStoreConfiguration ChangeCreateEntitiesOnTheFly(bool createEntitiesOnTheFly)
    {
        return new ArgoStoreConfiguration(ConnectionString, createEntitiesOnTheFly, (Dictionary<Type, DocumentMetadata>)EntityMeta, TenantId);
    }

    public Dictionary<Type, DocumentMetadata> GetEntityMetaCopy()
    {
        Dictionary<Type, DocumentMetadata> ret = new();

        foreach (KeyValuePair<Type, DocumentMetadata> pair in EntityMeta)
        {
            ret[pair.Key] = pair.Value;
        }

        return ret;
    }
}