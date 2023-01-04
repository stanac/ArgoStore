namespace ArgoStore;

public interface IArgoDocumentSession : IArgoQueryDocumentSession
{
    void Insert<T>(T entity);
    void SaveChanges();
}