using System.Linq.Expressions;

namespace ArgoStore;

internal class ArgoStoreSession : IArgoDocumentSession
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

    public IArgoStoreQueryable<T> Query<T>() where T : class, new()
    {
        return new ArgoStoreQueryable<T>(this);
    }

    public void Dispose()
    {
        // no op
    }
}