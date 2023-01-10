namespace ArgoStore.Implementations;

public interface IArgoDocumentStore
{
    IArgoDocumentSession OpenSession();
    IArgoDocumentSession OpenSession(string tenantId);
    IArgoQueryDocumentSession OpenQuerySession();
    IArgoQueryDocumentSession OpenQuerySession(string tenantId);
    void RegisterDocument<T>() where T : class, new();
    void RegisterDocument<T>(Action<IDocumentConfiguration<T>> configure) where T : class, new();
}