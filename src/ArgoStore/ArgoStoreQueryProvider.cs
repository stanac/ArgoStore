using System.Linq.Expressions;

namespace ArgoStore;

internal class ArgoStoreQueryProvider : IQueryProvider
{
    private readonly ArgoStoreSession _session;

    public ArgoStoreQueryProvider(ArgoStoreSession session)
    {
        _session = session;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        throw new NotSupportedException();
    }

    public IQueryable<T> CreateQuery<T>(Expression expression)
    {
        return new ArgoStoreQueryable<T>(_session, this, expression);
    }

    public object Execute(Expression expression)
    {
        throw new NotSupportedException();
    }

    public TResult Execute<TResult>(Expression expression)
    {
        throw new NotImplementedException();
    }
}