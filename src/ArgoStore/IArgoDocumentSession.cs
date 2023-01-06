namespace ArgoStore;

public interface IArgoDocumentSession : IArgoQueryDocumentSession
{
    void Insert<T>(params T[] entities) where T : class, new();
    void SaveChanges();
    void DiscardChanges();
}