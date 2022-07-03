using ArgoStore.Configurations;

namespace ArgoStore;

public class DocumentStore
{
    private readonly Configuration _config;

    public DocumentStore(string connectionString)
    {
        _config = new Configuration(connectionString, true, new Dictionary<Type, EntityMetadata>(), TenantIdDefault.DefaultValue);
    }
    
    public DocumentStore(Action<IDocumentStoreConfiguration> configurationAction)
    {
        DocumentStoreConfiguration configToSet = new DocumentStoreConfiguration();
        configurationAction(configToSet);

        _config = configToSet.CreateConfiguration();
        CreateTablesFromConfig();
    }

    public IDocumentSession CreateSession() => CreateSession(TenantIdDefault.DefaultValue);
        
    public IDocumentSession CreateSession(string tenantId)
    {
        return new DocumentSession(_config.ChangeTenant(tenantId));
    }

    public IQueryDocumentSession CreateReadOnlySession() => CreateReadOnlySession(TenantIdDefault.DefaultValue);

    public IQueryDocumentSession CreateReadOnlySession(string tenantId)
    {
        return new DocumentSession(_config.ChangeTenant(tenantId));
    }

    private void CreateTablesFromConfig()
    {
        foreach (Type entityType in _config.GetEntityMetaCopy().Keys)
        {
            CreateTableForEntityIfNotExists(entityType);
        }
    }

    private void CreateTableForEntityIfNotExists(Type entityType)
    {
        if (entityType == null) throw new ArgumentNullException(nameof(entityType));

        Configuration config = _config.ChangeCreateEntitiesOnTheFly(true);

        using DocumentSession session = new DocumentSession(config);

        session.CreateTableForEntityIfNotExists(entityType);
    }

}