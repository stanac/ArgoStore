using Remotion.Linq;
using System.Linq.Expressions;

namespace ArgoStore;

internal class ArgoStoreQueryProvider : IQueryProvider
{
    private readonly ArgoSession _session;

    public ArgoStoreQueryProvider(ArgoSession session)
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
        ArgoQueryModelVisitor v = VisitAndBuild(expression);
        ArgoCommand cmd = v.CommandBuilder.Build(_session.DocumentTypes, _session.TenantId);

        ArgoCommandExecutor exec = _session.CreateExecutor();
        return (TResult)exec.Execute(cmd);
    }

    internal ArgoQueryModelVisitor VisitAndBuild(Expression expression)
    {
        QueryModel query = new ArgoStoreQueryParser().GetParsedQuery(expression);

        ArgoQueryModelVisitor v = new();
        v.VisitQueryModel(query);

        return v;
    }
}