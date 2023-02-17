using System.Linq.Expressions;
using Remotion.Linq;

namespace ArgoStore.Implementations;

internal class ArgoStoreQueryable<T> : QueryableBase<T>, IArgoStoreQueryable<T>
{
    public ArgoStoreQueryable(ArgoStoreQueryProvider queryProvider, Expression expression)
        : base(queryProvider, expression)
    {
    }

    public ArgoStoreQueryable(ArgoSession session, Expression expression)
        : base(new ArgoStoreQueryProvider(session), expression)
    {
    }

    public ArgoStoreQueryable(ArgoSession session)
        : base(new ArgoStoreQueryProvider(session))
    {
    }
}