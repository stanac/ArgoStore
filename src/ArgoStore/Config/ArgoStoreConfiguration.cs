namespace ArgoStore.Config;

internal class ArgoStoreConfiguration
{
    public ArgoStoreConfiguration(string connectionString, Dictionary<Type, DocumentMetadata> entityMeta)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
        
        ConnectionString = connectionString;
        DocumentMeta = entityMeta ?? throw new ArgumentNullException(nameof(entityMeta));
    }

    public string ConnectionString { get; }
    public IReadOnlyDictionary<Type, DocumentMetadata> DocumentMeta { get; }
}