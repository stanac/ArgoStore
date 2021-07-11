namespace ArgoStore
{
    public interface IDocumentSession : IQueryDocumentSession
    {
        void Insert<T>(T entity);
    }
}
