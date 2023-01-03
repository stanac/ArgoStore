using System.Linq.Expressions;
using Remotion.Linq;

namespace ArgoStore;

internal class ArgoStoreQueryable<T> : QueryableBase<T>, IArgoStoreQueryable
{
    private readonly ArgoStoreSession _session;

    public ArgoStoreQueryable(ArgoStoreSession session, ArgoStoreQueryProvider queryProvider, Expression expression) 
        : base(queryProvider, expression)
    {
        _session = session;
    }

    public ArgoStoreQueryable(ArgoStoreSession session, Expression expression)
        : base(new ArgoStoreQueryProvider(session), expression)
    {
        _session = session;
    }
}