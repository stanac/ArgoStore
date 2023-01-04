using System.Linq.Expressions;
using Remotion.Linq;

namespace ArgoStore;

internal class ArgoStoreQueryable<T> : QueryableBase<T>, IArgoStoreQueryable<T>
{
    private readonly ArgoSession _session;

    public ArgoStoreQueryable(ArgoSession session, ArgoStoreQueryProvider queryProvider, Expression expression) 
        : base(queryProvider, expression)
    {
        _session = session;
    }

    public ArgoStoreQueryable(ArgoSession session, Expression expression)
        : base(new ArgoStoreQueryProvider(session), expression)
    {
        _session = session;
    }

    public ArgoStoreQueryable(ArgoSession session)
        : base(new ArgoStoreQueryProvider(session))
    {
    }
}