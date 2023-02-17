namespace ArgoStore;

public interface IArgoStoreQueryable<T> : IQueryable<T>, IOrderedQueryable<T>
{
    // string ToSqlString();
}