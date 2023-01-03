namespace ArgoStore;

public class ArgoDocumentStore
{
    private readonly string _connectionString;

    public ArgoDocumentStore(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

        _connectionString = connectionString;
    }

    public IArgoQueryDocumentSession CreateQuerySession()
    {
        return new ArgoStoreSession(_connectionString);
    }

    public IArgoQueryDocumentSession CreateQuerySession(string tenantId)
    {
        return new ArgoStoreSession(_connectionString, tenantId);
    }
}