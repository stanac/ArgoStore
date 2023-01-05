namespace ArgoStore;

public interface IArgoQueryDocumentSession : IDisposable
{
    IArgoStoreQueryable<T> Query<T>() where T : class, new();
    T GetById<T>(object id) where T : class, new();
}