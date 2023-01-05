namespace ArgoStore;

public interface IArgoDocumentSession : IArgoQueryDocumentSession
{
    void Insert<T>(T entity) where T : class, new();
    void SaveChanges();
}