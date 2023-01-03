namespace ArgoStore;

internal class ArgoStoreSession
{
    public string TenantId { get; }
    private readonly string _connectionString;
    internal const string DefaultTenant = "default";

    public ArgoStoreSession(string connectionString)
        : this(connectionString, DefaultTenant)
    {
    }

    public ArgoStoreSession(string connectionString, string tenantId)
    {
        TenantId = tenantId;
        _connectionString = connectionString;
    }
}