using Remotion.Linq;
using System.Linq.Expressions;
using ArgoStore.Command;
using ArgoStore.Config;

namespace ArgoStore.Implementations;

internal class ArgoStoreQueryProvider : IQueryProvider
{
    private readonly ArgoSession _session;

    public ArgoStoreQueryProvider(ArgoSession session)
    {
        _session = session;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        throw new NotSupportedException("CreateQuery non generic not supported.");
    }

    public IQueryable<T> CreateQuery<T>(Expression expression)
    {
        return new ArgoStoreQueryable<T>(this, expression);
    }

    public object Execute(Expression expression)
    {
        throw new NotSupportedException("Execute non generic not supported.");
    }
    
    public TResult Execute<TResult>(Expression expression)
    {
        ArgoQueryModelVisitor v = VisitAndBuild(expression);
        ArgoCommand cmd = v.CommandBuilder.Build(_session.TenantId);

        ArgoCommandExecutor exec = _session.CreateExecutor();
        TResult? result = (TResult?)exec.Execute(cmd);

        return result!;
    }

    internal ArgoQueryModelVisitor VisitAndBuild(Expression expression)
    {
        QueryModel query = new ArgoStoreQueryParser().GetParsedQuery(expression);
        DocumentMetadata meta = _session.DocumentTypesMetaMap[query.MainFromClause.ItemType];
        
        ArgoQueryModelVisitor v = new(meta);
        v.VisitQueryModel(query);

        return v;
    }
}